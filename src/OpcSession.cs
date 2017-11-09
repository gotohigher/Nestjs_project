﻿
using Opc.Ua.Client;
using System;
using System.Collections.Generic;
using System.Linq;

namespace OpcPublisher
{
    using Newtonsoft.Json;
    using Opc.Ua;
    using System.Text.RegularExpressions;
    using System.Threading;
    using System.Threading.Tasks;
    using static OpcPublisher.OpcMonitoredItem;
    using static OpcPublisher.PublisherTelemetryConfiguration;
    using static OpcPublisher.Workarounds.TraceWorkaround;
    using static OpcStackConfiguration;
    using static Program;
    using static PublisherNodeConfiguration;

    /// <summary>
    /// Class to manage the OPC monitored items, which are the nodes we need to publish.
    /// </summary>
    public class OpcMonitoredItem
    {
        public enum OpcMonitoredItemState
        {
            Unmonitored = 0,
            UnmonitoredNamespaceUpdateRequested,
            Monitored,
            RemovalRequested,
        }

        public enum OpcMonitoredItemConfigurationType
        {
            NodeId = 0,
            ExpandedNodeId
        }

        public string DisplayName;
        public OpcMonitoredItemState State;
        public uint AttributeId;
        public MonitoringMode MonitoringMode;
        public int RequestedSamplingInterval;
        public int SamplingInterval;
        public uint QueueSize;
        public bool DiscardOldest;
        public MonitoredItemNotificationEventHandler Notification;
        public Uri EndpointUri;
        public MonitoredItem OpcUaClientMonitoredItem;
        public NodeId ConfigNodeId;
        public ExpandedNodeId ConfigExpandedNodeId;
        public OpcMonitoredItemConfigurationType ConfigType;

        /// <summary>
        /// Ctor using NodeId (ns syntax for namespace).
        /// </summary>
        public OpcMonitoredItem(NodeId nodeId, Uri sessionEndpointUri, bool requestNamespaceUpdate = false)
        {
            ConfigNodeId = nodeId;
            ConfigExpandedNodeId = null;
            ConfigType = OpcMonitoredItemConfigurationType.NodeId;
            Init(sessionEndpointUri);
            if (requestNamespaceUpdate)
            {
                State = OpcMonitoredItemState.UnmonitoredNamespaceUpdateRequested;
            }
        }

        /// <summary>
        /// Ctor using ExpandedNodeId (nsu syntax for namespace).
        /// </summary>
        public OpcMonitoredItem(ExpandedNodeId expandedNodeId, Uri sessionEndpointUri, bool requestNamespaceUpdate = false)
        {
            ConfigNodeId = null;
            ConfigExpandedNodeId = expandedNodeId;
            ConfigType = OpcMonitoredItemConfigurationType.ExpandedNodeId;
            Init(sessionEndpointUri);
            if (requestNamespaceUpdate)
            {
                State = OpcMonitoredItemState.UnmonitoredNamespaceUpdateRequested;
            }
        }

        /// <summary>
        /// Checks if the monitored item does monitor the node described by the given objects.
        /// </summary>
        public bool IsMonitoringThisNode(NodeId nodeId, ExpandedNodeId expandedNodeId, NamespaceTable namespaceTable)
        {
            if (State == OpcMonitoredItemState.RemovalRequested)
            {
                return false;
            }
            if (ConfigType == OpcMonitoredItemConfigurationType.NodeId)
            {
                if (nodeId != null)
                {
                    if (ConfigNodeId == nodeId)
                    {
                        return true;
                    }
                }
                if (expandedNodeId != null)
                {
                    string namespaceUri = namespaceTable.ToArray().ElementAtOrDefault(ConfigNodeId.NamespaceIndex);
                    if (expandedNodeId.NamespaceUri != null && expandedNodeId.NamespaceUri.Equals(namespaceUri, StringComparison.OrdinalIgnoreCase))
                    {
                        if (expandedNodeId.Identifier.ToString().Equals(ConfigNodeId.Identifier.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
            }
            if (ConfigType == OpcMonitoredItemConfigurationType.ExpandedNodeId)
            {
                if (nodeId != null)
                {
                    int namespaceIndex = namespaceTable.GetIndex(ConfigExpandedNodeId?.NamespaceUri);
                    if (nodeId.NamespaceIndex == namespaceIndex)
                    {
                        if (nodeId.Identifier.ToString().Equals(ConfigExpandedNodeId.Identifier.ToString(), StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                if (expandedNodeId != null)
                {
                    if (ConfigExpandedNodeId.NamespaceUri != null && 
                        ConfigExpandedNodeId.NamespaceUri.Equals(expandedNodeId.NamespaceUri, StringComparison.OrdinalIgnoreCase) &&
                        ConfigExpandedNodeId.Identifier.ToString().Equals(expandedNodeId.Identifier.ToString(), StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public class MessageData
        {
            public MessageData()
            {
                MonitoredItemInfo = new MonitoredItemObjectInfo();
                ValueInfo = new ValueObjectInfo();
            }

            public class MonitoredItemObjectInfo
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string ApplicationUri;
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string DisplayName;
            }

            public class ValueObjectInfo
            {
                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string Value;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string SourceTimestamp;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string Status;

                [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
                public string StatusCode;
            }

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string EndpointUrl;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string NodeId;

            [JsonProperty("MonitoredItem", NullValueHandling = NullValueHandling.Ignore)]
            public MonitoredItemObjectInfo MonitoredItemInfo;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string ApplicationUri;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string DisplayName;

            public const string ValueInfoReplacementTag = "__ValueInfo__ToReplace__";
            [JsonProperty(ValueInfoReplacementTag, NullValueHandling = NullValueHandling.Ignore)]
            public ValueObjectInfo ValueInfo;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Value;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string SourceTimestamp;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string StatusCode;

            [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
            public string Status;

        }

        /// <summary>
        /// The notification that the data for a monitored item has changed on an OPC UA server.
        /// </summary>
        public void MonitoredItem_Notification(MonitoredItem monitoredItem, MonitoredItemNotificationEventArgs args)
        {
            try
            {
                if (args == null || args.NotificationValue == null || monitoredItem == null || monitoredItem.Subscription == null || monitoredItem.Subscription.Session == null)
                {
                    return;
                }

                MonitoredItemNotification notification = args.NotificationValue as MonitoredItemNotification;
                if (notification == null)
                {
                    return;
                }

                DataValue value = notification.Value as DataValue;
                if (value == null)
                {
                    return;
                }

                MessageData messageData = new MessageData();
                ServerTelemetryConfiguration telemetryConfiguration = GetServerTelemetryConfiguration(EndpointUri.AbsoluteUri);
                string telemetryValue = string.Empty;

                // process the NodeId configuration value (as configured in the publisher node configuration file)
                if (telemetryConfiguration.NodeId.Publish)
                {
                    telemetryValue = ConfigType == OpcMonitoredItemConfigurationType.NodeId ? ConfigNodeId.ToString() : ConfigExpandedNodeId.ToString();
                    if (!string.IsNullOrEmpty(telemetryConfiguration.NodeId.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.NodeId.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.NodeId = telemetryValue;
                }

                // process the EndpointUrl configuration value (as configured in the publisher node configuration file)
                if (telemetryConfiguration.EndpointUrl.Publish)
                {
                    telemetryValue = EndpointUri.AbsoluteUri;
                    if (!string.IsNullOrEmpty(telemetryConfiguration.EndpointUrl.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.EndpointUrl.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.EndpointUrl = telemetryValue;
                }

                // process MonitoredItem server ApplicationUri property
                if (telemetryConfiguration.MonitoredItem.ApplicationUri.Publish == true)
                {
                    // use the ApplicationUri as reported in the MonitoredItem and append the shopfloor domain if configured
                    telemetryValue = (monitoredItem.Subscription.Session.Endpoint.Server.ApplicationUri + (string.IsNullOrEmpty(OpcSession.ShopfloorDomain) ? "" : $":{OpcSession.ShopfloorDomain}"));
                    if (!string.IsNullOrEmpty(telemetryConfiguration.MonitoredItem.ApplicationUri.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.MonitoredItem.ApplicationUri.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.ApplicationUri = messageData.MonitoredItemInfo.ApplicationUri = telemetryValue;
                }

                // process MonitoredItem DisplayName property
                if (telemetryConfiguration.MonitoredItem.DisplayName.Publish == true)
                {
                    // use the DisplayName as reported in the MonitoredItem
                    telemetryValue = monitoredItem.DisplayName;
                    if (!string.IsNullOrEmpty(telemetryConfiguration.MonitoredItem.DisplayName.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.MonitoredItem.DisplayName.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.DisplayName = messageData.MonitoredItemInfo.DisplayName = telemetryValue;
                }

                // process flat setting for MonitoredItem
                if (telemetryConfiguration.MonitoredItem.Flat == true)
                {
                    messageData.MonitoredItemInfo = null;
                }
                else
                {
                    messageData.ApplicationUri = null;
                    messageData.DisplayName = null;
                }

                // process notification Value Value property
                if (telemetryConfiguration.Value.Value.Publish == true)
                {
                    // use the Value as reported in the notification event argument
                    telemetryValue = value.Value.ToString();
                    if (!string.IsNullOrEmpty(telemetryConfiguration.Value.Value.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.Value.Value.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.Value = messageData.ValueInfo.Value = telemetryValue;
                }

                // process notification Value SourceTimestamp property
                if (telemetryConfiguration.Value.SourceTimestamp.Publish == true)
                {
                    // use the SourceTimestamp as reported in the notification event argument in univeral sortable format
                    telemetryValue = value.SourceTimestamp.ToString("u");
                    if (!string.IsNullOrEmpty(telemetryConfiguration.Value.SourceTimestamp.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.Value.SourceTimestamp.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.SourceTimestamp = messageData.ValueInfo.SourceTimestamp = telemetryValue;
                }

                // process notification Value StatusCode property
                if (telemetryConfiguration.Value.StatusCode.Publish == true)
                {
                    // use the SourceTimestamp as reported in the notification event argument in univeral sortable format
                    telemetryValue = value.StatusCode.Code.ToString("X");
                    if (!string.IsNullOrEmpty(telemetryConfiguration.Value.StatusCode.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.Value.StatusCode.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.StatusCode = messageData.ValueInfo.StatusCode = telemetryValue;
                }


                // process notification Value StatusCode property as symbolic name
                if (telemetryConfiguration.Value.Status.Publish == true)
                {
                    // use the SourceTimestamp as reported in the notification event argument in univeral sortable format
                    telemetryValue = StatusCode.LookupSymbolicId(value.StatusCode.Code);
                    if (!string.IsNullOrEmpty(telemetryConfiguration.Value.Status.Pattern))
                    {
                        Regex regex = new Regex(telemetryConfiguration.Value.Status.Pattern);
                        Match match = regex.Match(telemetryValue);
                        if (match.Groups[0].Success)
                        {
                            telemetryValue = string.Empty;
                            foreach (var group in match.Groups.Skip(1))
                            {
                                telemetryValue += group.Value;
                            }
                        }
                    }
                    messageData.Status = messageData.ValueInfo.Status = telemetryValue;
                }

                // process flat setting for Value
                bool valueIsFlat = false;
                if (telemetryConfiguration.Value.Flat == true)
                {
                    messageData.ValueInfo = null;
                    valueIsFlat = true;
                }
                else
                {
                    messageData.Value = null;
                    messageData.SourceTimestamp = null;
                    messageData.StatusCode = null;
                    messageData.Status = null;
                    valueIsFlat = false;
                }

                // serialize the object and patch the value name in case we are no flat
                string json = JsonConvert.SerializeObject(messageData);
                if (valueIsFlat == false)
                {
                    json = json.Replace(MessageData.ValueInfoReplacementTag, "Value");
                }

                // add message to fifo send queue
                Trace(Utils.TraceMasks.OperationDetail, $"Enqueue a new message from subscription {monitoredItem.Subscription.Id}");
                Trace(Utils.TraceMasks.OperationDetail, $" with publishing interval: {monitoredItem.Subscription.PublishingInterval} and sampling interval: {monitoredItem.SamplingInterval}):");
                Trace(Utils.TraceMasks.OperationDetail, $"   EndpointUrl: {EndpointUri.AbsoluteUri}");
                Trace(Utils.TraceMasks.OperationDetail, $"   DisplayName: {monitoredItem.DisplayName}");
                Trace(Utils.TraceMasks.OperationDetail, $"   Value: {value}");
                IotHubCommunication.Enqueue(json);
            }
            catch (Exception e)
            {
                Trace(e, "Error processing monitored item notification");
            }
        }

        /// <summary>
        /// Init instance variables.
        /// </summary>
        private void Init(Uri sessionEndpointUri)
        {
            State = OpcMonitoredItemState.Unmonitored;
            DisplayName = string.Empty;
            AttributeId = Attributes.Value;
            MonitoringMode = MonitoringMode.Reporting;
            RequestedSamplingInterval = OpcSamplingInterval;
            QueueSize = 0;
            DiscardOldest = true;
            Notification = new MonitoredItemNotificationEventHandler(MonitoredItem_Notification);
            EndpointUri = sessionEndpointUri;
        }
    }

    /// <summary>
    /// Class to manage OPC subscriptions. We create a subscription for each different publishing interval
    /// on an Endpoint.
    /// </summary>
    public class OpcSubscription
    {
        public List<OpcMonitoredItem> OpcMonitoredItems;
        public int RequestedPublishingInterval;
        public double PublishingInterval;
        public Subscription OpcUaClientSubscription;

        public OpcSubscription(int? publishingInterval)
        {
            RequestedPublishingInterval = publishingInterval ?? OpcPublishingInterval;
            PublishingInterval = RequestedPublishingInterval;
            OpcMonitoredItems = new List<OpcMonitoredItem>();
        }
    }

    /// <summary>
    /// Class to manage OPC sessions.
    /// </summary>
    public class OpcSession
    {
        public enum SessionState
        {
            Disconnected = 0,
            Connecting,
            Connected,
        }

        public Uri EndpointUri;
        public Session OpcUaClientSession;
        public SessionState State;
        public List<OpcSubscription> OpcSubscriptions;
        public uint UnsuccessfulConnectionCount;
        public uint MissedKeepAlives;
        public int PublishingInterval;
        public KeepAliveEventHandler StandardKeepAliveEventHandlerAsync;

        public uint SessionTimeout { get; }

        public static bool FetchOpcNodeDisplayName
        {
            get => _fetchOpcNodeDisplayName;
            set => _fetchOpcNodeDisplayName = value;
        }

        public static string ShopfloorDomain
        {
            get => _shopfloorDomain;
            set => _shopfloorDomain = value;
        }

        public bool UseSecurity
        {
            get => _useSecurity;
            set => _useSecurity = value;
        }

        public int GetNumberOfOpcSubscriptions()
        {
            int result = 0;
            try
            {
                _opcSessionSemaphore.WaitAsync();
                result = OpcSubscriptions.Count();
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
            return result;
        }

        public int GetNumberOfOpcMonitoredItems()
        {
            int result = 0;
            try
            {
                _opcSessionSemaphore.WaitAsync();
                var subscriptions = OpcSessions.SelectMany(s => s.OpcSubscriptions);
                foreach (var subscription in subscriptions)
                {
                    result += subscription.OpcMonitoredItems.Count(i => i.State == OpcMonitoredItemState.Monitored);
                }
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
            return result;
        }

        /// <summary>
        /// Ctor for the session.
        /// </summary>
        public OpcSession(Uri endpointUri, bool useSecurity, uint sessionTimeout)
        {
            State = SessionState.Disconnected;
            EndpointUri = endpointUri;
            SessionTimeout = sessionTimeout * 1000;
            OpcSubscriptions = new List<OpcSubscription>();
            UnsuccessfulConnectionCount = 0;
            MissedKeepAlives = 0;
            PublishingInterval = OpcPublishingInterval;
            _useSecurity = useSecurity;
            _opcSessionSemaphore = new SemaphoreSlim(1);
            _namespaceTable = new NamespaceTable();
        }

        /// <summary>
        /// This task is executed regularily and ensures:
        /// - disconnected sessions are reconnected.
        /// - monitored nodes are no longer monitored if requested to do so.
        /// - monitoring for a node starts if it is required.
        /// - unused subscriptions (without any nodes to monitor) are removed.
        /// - sessions with out subscriptions are removed.
        /// </summary>
        public async Task ConnectAndMonitorAsync(CancellationToken ct)
        {
            bool updateConfigFileRequired = false;
            try
            {
                await ConnectSession(ct);

                updateConfigFileRequired = await MonitorNodes(ct);

                updateConfigFileRequired |= await StopMonitoringNodes(ct);

                await RemoveUnusedSubscriptions(ct);

                await RemoveUnusedSessions(ct);

                // update the config file if required
                if (updateConfigFileRequired)
                {
                    await UpdateNodeConfigurationFile();
                }
            }
            catch (Exception e)
            {
                Trace(e, $"Error in ConnectAndMonitorAsync. (message: {e.Message})");
            }
        }

        /// <summary>
        /// Connects the session if it is disconnected.
        /// </summary>
        public async Task ConnectSession(CancellationToken ct)
        {
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                // if the session is already connected or shutdown in progress, return
                if (State == SessionState.Connected || ct.IsCancellationRequested)
                {
                    return;
                }

                Trace($"Connect and monitor session and nodes on endpoint '{EndpointUri.AbsoluteUri}'.");
                State = SessionState.Connecting;
                try
                {
                    // release the session to not block for high network timeouts.
                    _opcSessionSemaphore.Release();

                    // start connecting
                    EndpointDescription selectedEndpoint = CoreClientUtils.SelectEndpoint(EndpointUri.AbsoluteUri, _useSecurity);
                    ConfiguredEndpoint configuredEndpoint = new ConfiguredEndpoint(null, selectedEndpoint, EndpointConfiguration.Create(PublisherOpcApplicationConfiguration));
                    uint timeout = SessionTimeout * ((UnsuccessfulConnectionCount >= OpcSessionCreationBackoffMax) ? OpcSessionCreationBackoffMax : UnsuccessfulConnectionCount + 1);
                    Trace($"Create {(_useSecurity ? "secured" : "unsecured")} session for endpoint URI '{EndpointUri.AbsoluteUri}' with timeout of {timeout} ms.");
                    OpcUaClientSession = await Session.Create(
                            PublisherOpcApplicationConfiguration,
                            configuredEndpoint,
                            true,
                            false,
                            PublisherOpcApplicationConfiguration.ApplicationName,
                            timeout,
                            new UserIdentity(new AnonymousIdentityToken()),
                            null);

                    if (OpcUaClientSession != null)
                    {
                        Trace($"Session successfully created with Id {OpcUaClientSession.SessionId}.");
                        if (!selectedEndpoint.EndpointUrl.Equals(configuredEndpoint.EndpointUrl.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                        {
                            Trace($"the Server has updated the EndpointUrl to '{selectedEndpoint.EndpointUrl}'");
                        }

                        // init object state and install keep alive
                        UnsuccessfulConnectionCount = 0;
                        OpcUaClientSession.KeepAliveInterval = OpcKeepAliveIntervalInSec * 1000;
                        StandardKeepAliveEventHandlerAsync = async (session, keepAliveEventArgs) => await StandardClient_KeepAlive(session, keepAliveEventArgs);
                        OpcUaClientSession.KeepAlive += StandardKeepAliveEventHandlerAsync;

                        // fetch the namespace array and cache it. it will not change as long the session exists.
                        DataValue namespaceArrayNodeValue = OpcUaClientSession.ReadValue(VariableIds.Server_NamespaceArray);
                        _namespaceTable.Update(namespaceArrayNodeValue.GetValue<string[]>(null));

                        // show the available namespaces
                        Trace($"The session to endpoint '{selectedEndpoint.EndpointUrl}' has {_namespaceTable.Count} entries in its namespace array:");
                        int i = 0;
                        foreach (var ns in _namespaceTable.ToArray())
                        {
                            Trace($"Namespace index {i++}: {ns}");
                        }

                        // fetch the minimum supported item sampling interval from the server.
                        DataValue minSupportedSamplingInterval = OpcUaClientSession.ReadValue(VariableIds.Server_ServerCapabilities_MinSupportedSampleRate);
                        _minSupportedSamplingInterval = minSupportedSamplingInterval.GetValue(0);
                        Trace($"The server on endpoint '{selectedEndpoint.EndpointUrl}' supports a minimal sampling interval of {_minSupportedSamplingInterval} ms.");
                    }
                }
                catch (Exception e)
                {
                    Trace(e, $"Session creation to endpoint '{EndpointUri.AbsoluteUri}' failed {++UnsuccessfulConnectionCount} time(s). Please verify if server is up and Publisher configuration is correct.");
                    State = SessionState.Disconnected;
                    OpcUaClientSession = null;
                    return;
                }
                finally
                {
                    await _opcSessionSemaphore.WaitAsync();
                    if (OpcUaClientSession != null)
                    {
                        State = SessionState.Connected;
                    }
                    else
                    {
                        State = SessionState.Disconnected;
                    }
                }
            }
            catch (Exception e)
            {
                Trace(e, $"Error in ConnectSessions. (message: {e.Message})");
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
        }

        /// <summary>
        /// Monitoring for a node starts if it is required.
        /// </summary>
        public async Task<bool> MonitorNodes(CancellationToken ct)
        {
            bool requestConfigFileUpdate = false;
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                // if the session is not connected or shutdown in progress, return
                if (State != SessionState.Connected || ct.IsCancellationRequested)
                {
                    return requestConfigFileUpdate;
                }

                // ensure all nodes in all subscriptions of this session are monitored.
                foreach (var opcSubscription in OpcSubscriptions)
                {
                    // create the subscription, if it is not yet there.
                    if (opcSubscription.OpcUaClientSubscription == null)
                    {
                        int revisedPublishingInterval;
                        opcSubscription.OpcUaClientSubscription = CreateSubscription(opcSubscription.RequestedPublishingInterval, out revisedPublishingInterval);
                        opcSubscription.PublishingInterval = revisedPublishingInterval;
                        Trace($"Create subscription on endpoint '{EndpointUri.AbsoluteUri}' requested OPC publishing interval is {opcSubscription.RequestedPublishingInterval} ms. (revised: {revisedPublishingInterval} ms)");
                    }

                    // process all unmonitored items.
                    var unmonitoredItems = opcSubscription.OpcMonitoredItems.Where(i => (i.State == OpcMonitoredItemState.Unmonitored || i.State == OpcMonitoredItemState.UnmonitoredNamespaceUpdateRequested));

                    int additionalMonitoredItemsCount = 0;
                    int monitoredItemsCount = 0;
                    bool haveUnmonitoredItems = false;
                    if (unmonitoredItems.Count() != 0)
                    {
                        haveUnmonitoredItems = true;
                        monitoredItemsCount = opcSubscription.OpcMonitoredItems.Count(i => (i.State == OpcMonitoredItemState.Monitored));
                        Trace($"Start monitoring items on endpoint '{EndpointUri.AbsoluteUri}'. Currently monitoring {monitoredItemsCount} items.");
                    }
                    foreach (var item in unmonitoredItems)
                    {
                        // if the session is disconnected or a shutdown is in progress, we stop trying and wait for the next cycle
                        if (State == SessionState.Disconnected || ct.IsCancellationRequested)
                        {
                            break;
                        }

                        NodeId currentNodeId = null;
                        try
                        {
                            // update the namespace of the node if requested. there are two cases where this is requested:
                            // 1) publishing requests via the OPC server method are raised using a NodeId format. for those
                            //    the NodeId format is converted into an ExpandedNodeId format
                            // 2) ExpandedNodeId configuration file entries do not have at parsing time a session to get
                            //    the namespace index. this is set now.
                            if (item.State == OpcMonitoredItemState.UnmonitoredNamespaceUpdateRequested)
                            {
                                if (item.ConfigType == OpcMonitoredItemConfigurationType.ExpandedNodeId)
                                {
                                    int namespaceIndex = GetNamespaceIndex(item.ConfigExpandedNodeId?.NamespaceUri);
                                    if (namespaceIndex < 0)
                                    {
                                        Trace($"The namespace URI of node '{item.ConfigExpandedNodeId.ToString()}' could be not mapped to a namespace index.");
                                    }
                                    else
                                    {
                                        item.ConfigExpandedNodeId = new ExpandedNodeId(item.ConfigExpandedNodeId.Identifier, (ushort)namespaceIndex, item.ConfigExpandedNodeId?.NamespaceUri, 0);
                                    }
                                }
                                if (item.ConfigType == OpcMonitoredItemConfigurationType.NodeId)
                                {
                                    string namespaceUri = GetNamespaceUri(item.ConfigNodeId.NamespaceIndex);
                                    if (string.IsNullOrEmpty(namespaceUri))
                                    {
                                        Trace($"The namespace index of node '{item.ConfigNodeId.ToString()}' is invalid and the node format could not be updated.");
                                    }
                                    else
                                    {
                                        item.ConfigExpandedNodeId = new ExpandedNodeId(item.ConfigNodeId.Identifier, item.ConfigNodeId.NamespaceIndex, namespaceUri, 0);
                                        item.ConfigType = OpcMonitoredItemConfigurationType.ExpandedNodeId;
                                    }
                                }
                                item.State = OpcMonitoredItemState.Unmonitored;
                            }

                            // lookup namespace index if ExpandedNodeId format has been used and build NodeId identifier.
                            if (item.ConfigType == OpcMonitoredItemConfigurationType.ExpandedNodeId)
                            {
                                int namespaceIndex = GetNamespaceIndex(item.ConfigExpandedNodeId?.NamespaceUri);
                                if (namespaceIndex < 0)
                                {
                                    Trace($"Syntax or namespace URI of ExpandedNodeId '{item.ConfigExpandedNodeId.ToString()}' is invalid and will be ignored.");
                                    continue;
                                }
                                currentNodeId = new NodeId(item.ConfigExpandedNodeId.Identifier, (ushort)namespaceIndex);
                            }
                            else
                            {
                                currentNodeId = item.ConfigNodeId;
                            }

                            // if configured, get the DisplayName for the node, otherwise use the nodeId
                            Node node;
                            if (FetchOpcNodeDisplayName == true)
                            {
                                node = OpcUaClientSession.ReadNode(currentNodeId);
                                item.DisplayName = node.DisplayName.Text ?? currentNodeId.ToString();
                            }
                            else
                            {
                                item.DisplayName = currentNodeId.ToString();
                            }

                            // add the new monitored item.
                            MonitoredItem monitoredItem = new MonitoredItem()
                            {
                                StartNodeId = currentNodeId,
                                AttributeId = item.AttributeId,
                                DisplayName = item.DisplayName,
                                MonitoringMode = item.MonitoringMode,
                                SamplingInterval = item.RequestedSamplingInterval,
                                QueueSize = item.QueueSize,
                                DiscardOldest = item.DiscardOldest
                            };
                            monitoredItem.Notification += item.Notification;
                            opcSubscription.OpcUaClientSubscription.AddItem(monitoredItem);
                            opcSubscription.OpcUaClientSubscription.SetPublishingMode(true);
                            opcSubscription.OpcUaClientSubscription.ApplyChanges();
                            item.OpcUaClientMonitoredItem = monitoredItem;
                            item.State = OpcMonitoredItemState.Monitored;
                            item.EndpointUri = EndpointUri;
                            Trace($"Created monitored item for node '{currentNodeId.ToString()}' in subscription with id '{opcSubscription.OpcUaClientSubscription.Id}' on endpoint '{EndpointUri.AbsoluteUri}'");
                            if (item.RequestedSamplingInterval != monitoredItem.SamplingInterval)
                            {
                                Trace($"Sampling interval: requested: {item.RequestedSamplingInterval}; revised: {monitoredItem.SamplingInterval}");
                                item.SamplingInterval = monitoredItem.SamplingInterval;
                            }
                            if (additionalMonitoredItemsCount++ % 50 == 0)
                            {
                                Trace($"Now monitoring {monitoredItemsCount + additionalMonitoredItemsCount} items in subscription with id '{opcSubscription.OpcUaClientSubscription.Id}'");
                            }
                        }
                        catch (Exception e) when (e.GetType() == typeof(ServiceResultException))
                        {
                            ServiceResultException sre = (ServiceResultException)e;
                            switch ((uint)sre.Result.StatusCode)
                            {
                                case StatusCodes.BadSessionIdInvalid:
                                    {
                                        Trace($"Session with Id {OpcUaClientSession.SessionId} is no longer available on endpoint '{EndpointUri}'. Cleaning up.");
                                        // clean up the session
                                        InternalDisconnectAsync();
                                        break;
                                    }
                                case StatusCodes.BadNodeIdInvalid:
                                case StatusCodes.BadNodeIdUnknown:
                                    {
                                        Trace($"Failed to monitor node '{currentNodeId.Identifier}' on endpoint '{EndpointUri}'.");
                                        Trace($"OPC UA ServiceResultException is '{sre.Result}'. Please check your publisher configuration for this node.");
                                        break;
                                    }
                                default:
                                    {
                                        Trace($"Unhandled OPC UA ServiceResultException '{sre.Result}' when monitoring node '{currentNodeId.Identifier}' on endpoint '{EndpointUri}'. Continue.");
                                        break;
                                    }
                            }
                        }
                        catch (Exception e)
                        {
                            Trace(e, $"Failed to monitor node '{currentNodeId.Identifier}' on endpoint '{EndpointUri}'");
                        }
                    }
                    if (haveUnmonitoredItems == true)
                    {
                        monitoredItemsCount = opcSubscription.OpcMonitoredItems.Count(i => (i.State == OpcMonitoredItemState.Monitored));
                        Trace($"Done processing unmonitored items on endpoint '{EndpointUri.AbsoluteUri}'. Now monitoring {monitoredItemsCount} items in subscription with id '{opcSubscription.OpcUaClientSubscription.Id}'.");
                    }
                }
                // request a config file update, if everything is successfully monitored
                requestConfigFileUpdate = true;
            }
            catch (Exception e)
            {
                Trace(e, $"Error in MonitorNodes. (message: {e.Message})");
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
            return requestConfigFileUpdate;
        }

        /// <summary>
        /// Checks if there are monitored nodes tagged to stop monitoring.
        /// </summary>
        public async Task<bool> StopMonitoringNodes(CancellationToken ct)
        {
            bool requestConfigFileUpdate = false;
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                // if session is not connected or shutdown is in progress, return
                if (State != SessionState.Connected || ct.IsCancellationRequested)
                {
                    return requestConfigFileUpdate;
                }

                foreach (var opcSubscription in OpcSubscriptions)
                {
                    // remove items tagged to stop in the stack
                    var itemsToRemove = opcSubscription.OpcMonitoredItems.Where(i => i.State == OpcMonitoredItemState.RemovalRequested);
                    if (itemsToRemove.Any())
                    {
                        Trace($"Remove nodes in subscription with id {opcSubscription.OpcUaClientSubscription.Id} on endpoint '{EndpointUri.AbsoluteUri}'");
                        try
                        {
                            opcSubscription.OpcUaClientSubscription.RemoveItems(itemsToRemove.Select(i => i.OpcUaClientMonitoredItem));
                            Trace($"There are now {opcSubscription.OpcUaClientSubscription.MonitoredItemCount} monitored items in this subscription.");
                        }
                        catch
                        {
                            // nodes may be tagged for stop before they are monitored, just continue
                        }
                        // remove them in our data structure
                        opcSubscription.OpcMonitoredItems.RemoveAll(i => i.State == OpcMonitoredItemState.RemovalRequested);
                        Trace($"There are now {opcSubscription.OpcMonitoredItems.Count} items managed by publisher for this subscription.");
                        requestConfigFileUpdate = true;
                    }
                }
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
            return requestConfigFileUpdate;
        }

        /// <summary>
        /// Checks if there are subscriptions without any monitored items and remove them.
        /// </summary>
        public async Task RemoveUnusedSubscriptions(CancellationToken ct)
        {
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                // if session is not connected or shutdown is in progress, return
                if (State != SessionState.Connected || ct.IsCancellationRequested)
                {
                    return;
                }

                // remove the subscriptions in the stack
                var subscriptionsToRemove = OpcSubscriptions.Where(i => i.OpcMonitoredItems.Count == 0);
                if (subscriptionsToRemove.Any())
                {
                    Trace($"Remove unused subscriptions on endpoint '{EndpointUri}'.");
                    OpcUaClientSession.RemoveSubscriptions(subscriptionsToRemove.Select(s => s.OpcUaClientSubscription));
                    Trace($"There are now {OpcUaClientSession.SubscriptionCount} subscriptions in this sessopm.");
                }
                // remove them in our data structures
                OpcSubscriptions.RemoveAll(s => s.OpcMonitoredItems.Count == 0);
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }

        }

        /// <summary>
        /// Checks if there are session without any subscriptions and remove them.
        /// </summary>
        public async Task RemoveUnusedSessions(CancellationToken ct)
        {
            try
            {
                await OpcSessionsListSemaphore.WaitAsync();

                // if session is not connected or shutdown is in progress, return
                if (State != SessionState.Connected || ct.IsCancellationRequested)
                {
                    return;
                }

                // remove sssions in the stack
                var sessionsToRemove = OpcSessions.Where(s => s.OpcSubscriptions.Count == 0);
                foreach (var sessionToRemove in sessionsToRemove)
                {
                    Trace($"Remove unused session on endpoint '{EndpointUri}'.");
                    await sessionToRemove.ShutdownAsync();
                }
                // remove then in our data structures
                OpcSessions.RemoveAll(s => s.OpcSubscriptions.Count == 0);
            }
            finally
            {
                OpcSessionsListSemaphore.Release();
            }
        }

        /// <summary>
        /// Disconnects a session and removes all subscriptions on it and marks all nodes on those subscriptions
        /// as unmonitored.
        /// </summary>
        public async Task DisconnectAsync()
        {
            await _opcSessionSemaphore.WaitAsync();

            InternalDisconnectAsync();

            _opcSessionSemaphore.Release();
        }

        /// <summary>
        /// Returns the namespace URI for a namespace index.
        /// </summary>
        public string GetNamespaceUri(int namespaceIndex)
        {
            try
            {
                _opcSessionSemaphore.WaitAsync();

                return _namespaceTable.ToArray().ElementAtOrDefault(namespaceIndex);
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
        }

        /// <summary>
        /// Returns the namespace index for a namespace URI.
        /// </summary>
        public int GetNamespaceIndex(string namespaceUri)
        {
            try
            {
                _opcSessionSemaphore.WaitAsync();

                return _namespaceTable.GetIndex(namespaceUri);
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
        }


        /// <summary>
        /// Internal disconnect method. Caller must have taken the _opcSessionSemaphore.
        /// </summary>
        private void InternalDisconnectAsync()
        {
            try
            {
                foreach (var opcSubscription in OpcSubscriptions)
                {
                    try
                    {
                        OpcUaClientSession.RemoveSubscription(opcSubscription.OpcUaClientSubscription);
                    }
                    catch
                    {
                        // the session might be already invalidated. ignore.
                    }
                    try
                    {
                        opcSubscription.OpcUaClientSubscription.Delete(true);
                    }
                    catch
                    {
                        // the subscription might be already invalidated. ignore.
                    }
                    opcSubscription.OpcUaClientSubscription = null;

                    // mark all monitored items as unmonitored
                    foreach (var opcMonitoredItem in opcSubscription.OpcMonitoredItems)
                    {
                        // tag all monitored items as unmonitored
                        if (opcMonitoredItem.State == OpcMonitoredItemState.Monitored)
                        {
                            opcMonitoredItem.State = OpcMonitoredItemState.Unmonitored;
                        }
                    }
                }
                try
                {
                    OpcUaClientSession.Close();
                }
                catch
                {
                    // the session might be already invalidated. ignore.
                }
                OpcUaClientSession = null;
            }
            catch (Exception e)
            {
                Trace(e, $"Error in InternalDisconnectAsync. (message: {e.Message})");
            }
            State = SessionState.Disconnected;
            MissedKeepAlives = 0;
        }

        /// <summary>
        /// Adds a node to be monitored. If there is no subscription with the requested publishing interval,
        /// one is created.
        /// </summary>
        public async Task AddNodeForMonitoring(NodeId nodeId, ExpandedNodeId expandedNodeId, int opcPublishingInterval, int opcSamplingInterval, CancellationToken ct)
        {
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                if (ct.IsCancellationRequested)
                {
                    return;
                }

                // check if there is already a subscription with the same publishing interval, which could be used to monitor the node
                OpcSubscription opcSubscription = OpcSubscriptions.FirstOrDefault(s => s.RequestedPublishingInterval == opcPublishingInterval);
                
                // if there was none found, create one
                if (opcSubscription == null)
                {
                    opcSubscription = new OpcSubscription(opcPublishingInterval);
                    OpcSubscriptions.Add(opcSubscription);
                    Trace($"AddNodeForMonitoring: No matching subscription with publishing interval of {opcPublishingInterval} found'. Requested to create a new one.");
                }

                // if it is already published, we do nothing, else we create a new monitored item
                if (!IsNodePublishedInSession(nodeId, expandedNodeId))
                {
                    OpcMonitoredItem opcMonitoredItem = null;
                    // add a new item to monitor
                    if (expandedNodeId == null)
                    {
                        opcMonitoredItem = new OpcMonitoredItem(nodeId, EndpointUri, true);
                    }
                    else
                    {
                        opcMonitoredItem = new OpcMonitoredItem(expandedNodeId, EndpointUri);
                    }
                    opcMonitoredItem.RequestedSamplingInterval = opcSamplingInterval;
                    opcSubscription.OpcMonitoredItems.Add(opcMonitoredItem);
                    Trace($"AddNodeForMonitoring: Added item with nodeId '{(expandedNodeId == null ? nodeId.ToString() : expandedNodeId.ToString())}' for monitoring.");

                    // trigger the actual OPC communication with the server to be done
                    Task.Run(async () => await ConnectAndMonitorAsync(ct));
                }
            }
            catch (Exception e)
            {
                Trace(e, $"AddNodeForMonitoring: Exception while trying to add node '{nodeId.ToString()}' for monitoring. (message: '{e.Message}'");
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
        }

        /// <summary>
        /// Tags a monitored node to stop monitoring and remove it.
        /// </summary>
        public async Task RequestMonitorItemRemoval(NodeId nodeId, ExpandedNodeId expandedNodeId, CancellationToken ct)
        {
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                if (ct.IsCancellationRequested)
                {
                    return;
                }

                // tag all monitored items with nodeId to stop monitoring.
                // if the node to tag is specified as NodeId, it will also tag nodes configured
                // in ExpandedNodeId format.
                foreach (var opcSubscription in OpcSubscriptions)
                {
                    var opcMonitoredItems = opcSubscription.OpcMonitoredItems.Where(m => { return m.IsMonitoringThisNode(nodeId, expandedNodeId, _namespaceTable); });
                    foreach (var opcMonitoredItem in opcMonitoredItems)
                    {
                        // tag it for removal.
                        opcMonitoredItem.State = OpcMonitoredItemState.RemovalRequested;
                        Trace("RequestMonitorItemRemoval: Node " +
                            $"'{(expandedNodeId == null ? nodeId.ToString() : expandedNodeId.ToString())}' tagged to stop monitoring.");
                    }
                }

                // trigger the actual OPC communication with the server to be done
                Task.Run(async () => await ConnectAndMonitorAsync(ct));
            }
            catch (Exception e)
            {
                Trace(e, $"RequestMonitorItemRemoval: Exception while trying to tag node '{nodeId.ToString()}' to stop monitoring. (message: '{e.Message}'");
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
        }

        /// <summary>
        /// Checks if the node specified by either the given NodeId or ExpandedNodeId on the given endpoint is published in the session.
        /// </summary>
        private bool IsNodePublishedInSession(NodeId nodeId, ExpandedNodeId expandedNodeId)
        {
            try
            {
                _opcSessionSemaphore.WaitAsync();

                foreach (var opcSubscription in OpcSubscriptions)
                {
                    if (opcSubscription.OpcMonitoredItems.Any(m => { return m.IsMonitoringThisNode(nodeId, expandedNodeId, _namespaceTable); }))
                    {
                        return true;
                    }
                }
            }
            catch (Exception e)
            {
                Trace(e, "Check if node is published failed.");
            }
            finally
            {
                _opcSessionSemaphore.Release();
            }
            return false;
        }

        /// <summary>
        /// Checks if the node specified by either the given NodeId or ExpandedNodeId on the given endpoint is published.
        /// </summary>
        public static bool IsNodePublished(NodeId nodeId, ExpandedNodeId expandedNodeId, Uri endpointUri)
        {
            try
            {
                OpcSessionsListSemaphore.WaitAsync();

                // itereate through all sessions, subscriptions and monitored items and create config file entries
                foreach (var opcSession in OpcSessions)
                {
                    if (opcSession.EndpointUri.AbsoluteUri.Equals(endpointUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase))
                    {
                        if (opcSession.IsNodePublishedInSession(nodeId, expandedNodeId))
                        {
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Trace(e, "Check if node is published failed.");
            }
            finally
            {
                OpcSessionsListSemaphore.Release();
            }
            return false;
        }

    /// <summary>
    /// Shutdown the current session if it is connected.
    /// </summary>
    public async Task ShutdownAsync()
        {
            try
            {
                await _opcSessionSemaphore.WaitAsync();

                // if the session is connected, close it.
                if (State == SessionState.Connected)
                {
                    try
                    {
                        foreach (var opcSubscription in OpcSubscriptions)
                        {
                            Trace($"Removing {opcSubscription.OpcUaClientSubscription.MonitoredItemCount} monitored items from subscription with id '{opcSubscription.OpcUaClientSubscription.Id}'.");
                            opcSubscription.OpcUaClientSubscription.RemoveItems(opcSubscription.OpcUaClientSubscription.MonitoredItems);
                        }
                        Trace($"Removing {OpcUaClientSession.SubscriptionCount} subscriptions from session.");
                        while (OpcSubscriptions.Count > 0)
                        {
                            OpcSubscription opcSubscription = OpcSubscriptions.ElementAt(0);
                            OpcSubscriptions.RemoveAt(0);
                            Subscription opcUaClientSubscription = opcSubscription.OpcUaClientSubscription;
                            opcUaClientSubscription.Delete(true);
                        }
                        Trace($"Closing session to endpoint URI '{EndpointUri.AbsoluteUri}' closed successfully.");
                        OpcUaClientSession.Close();
                        State = SessionState.Disconnected;
                        Trace($"Session to endpoint URI '{EndpointUri.AbsoluteUri}' closed successfully.");
                    }
                    catch (Exception e)
                    {
                        Trace(e, $"Error while closing session to endpoint '{EndpointUri.AbsoluteUri}'.");
                        State = SessionState.Disconnected;
                        return;
                    }
                }
            }
            finally
            {
                _opcSessionSemaphore.Release();
                _opcSessionSemaphore.Dispose();
                _opcSessionSemaphore = null;
            }
        }

        /// <summary>
        /// Create a subscription in the session.
        /// </summary>
        private Subscription CreateSubscription(int requestedPublishingInterval, out int revisedPublishingInterval)
        {
            Subscription subscription = new Subscription()
            {
                PublishingInterval = requestedPublishingInterval,
            };
            // need to happen before the create to set the Session property.
            OpcUaClientSession.AddSubscription(subscription);
            subscription.Create();
            Trace($"Created subscription with id {subscription.Id} on endpoint '{EndpointUri.AbsoluteUri}'");
            if (requestedPublishingInterval != subscription.PublishingInterval)
            {
                Trace($"Publishing interval: requested: {requestedPublishingInterval}; revised: {subscription.PublishingInterval}");
            }
            revisedPublishingInterval = subscription.PublishingInterval;
            return subscription;
        }

        /// <summary>
        /// Handler for the standard "keep alive" event sent by all OPC UA servers
        /// </summary>
        private async Task StandardClient_KeepAlive(Session session, KeepAliveEventArgs e)
        {
            // Ignore if we are shutting down.
            if (ShutdownTokenSource.IsCancellationRequested == true)
            {
                return;
            }

            if (e != null && session != null && session.ConfiguredEndpoint != null && OpcUaClientSession != null)
            {
                try
                {
                    if (!ServiceResult.IsGood(e.Status))
                    {
                        Trace($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} has Status: {e.Status}");
                        Trace($"Outstanding requests: {session.OutstandingRequestCount}, Defunct requests: {session.DefunctRequestCount}");
                        Trace($"Good publish requests: {session.GoodPublishRequestCount}, KeepAlive interval: {session.KeepAliveInterval}");
                        Trace($"SessionId: {session.SessionId}");

                        if (State == SessionState.Connected)
                        {
                            MissedKeepAlives++;
                            Trace($"Missed KeepAlives: {MissedKeepAlives}");
                            if (MissedKeepAlives >= OpcKeepAliveDisconnectThreshold)
                            {
                                Trace($"Hit configured missed keep alive threshold of {OpcKeepAliveDisconnectThreshold}. Disconnecting the session to endpoint {session.ConfiguredEndpoint.EndpointUrl}.");
                                session.KeepAlive -= StandardKeepAliveEventHandlerAsync;
                                Task.Run(async () => await DisconnectAsync());
                            }
                        }
                    }
                    else
                    {
                        if (MissedKeepAlives != 0)
                        {
                            // Reset missed keep alive count
                            Trace($"Session endpoint: {session.ConfiguredEndpoint.EndpointUrl} got a keep alive after {MissedKeepAlives} {(MissedKeepAlives == 1 ? "was" : "were")} missed.");
                            MissedKeepAlives = 0;
                        }
                    }
                }
                catch (Exception ex)
                {
                    Trace(ex, $"Error in keep alive handling for endpoint '{session.ConfiguredEndpoint.EndpointUrl}'. (message: '{ex.Message}'");
                }
            }
            else
            {
                Trace("Keep alive arguments seems to be wrong.");
            }
        }

        private static string _shopfloorDomain;
        private static bool _fetchOpcNodeDisplayName = false;
        private bool _useSecurity = true;
        private SemaphoreSlim _opcSessionSemaphore;
        private NamespaceTable _namespaceTable;
        private double _minSupportedSamplingInterval;
    }
}

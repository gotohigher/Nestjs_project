
using Opc.Ua;
using Opc.Ua.Server;
using System;
using System.Collections.Generic;

namespace OpcPublisher
{
    using Newtonsoft.Json;
    using System.Linq;
    using static IotHubMessaging;
    using static OpcPublisher.OpcMonitoredItem;
    using static OpcPublisher.Program;
    using static OpcPublisher.Workarounds.TraceWorkaround;
    using static OpcStackConfiguration;
    using static PublisherNodeConfiguration;


    public class PublisherNodeManager : CustomNodeManager2
    {
        public PublisherNodeManager(Opc.Ua.Server.IServerInternal server, ApplicationConfiguration configuration)
        : base(server, configuration, Namespaces.PublisherApplications)
        {
            SystemContext.NodeIdFactory = this;
        }

        /// <summary>
        /// Creates the NodeId for the specified node.
        /// </summary>
        public override NodeId New(ISystemContext context, NodeState node)
        {
            BaseInstanceState instance = node as BaseInstanceState;

            if (instance != null && instance.Parent != null)
            {
                string id = instance.Parent.NodeId.Identifier as string;

                if (id != null)
                {
                    return new NodeId(id + "_" + instance.SymbolicName, instance.Parent.NodeId.NamespaceIndex);
                }
            }

            return node.NodeId;
        }

        /// <summary>
        /// Creates a new folder.
        /// </summary>
        private FolderState CreateFolder(NodeState parent, string path, string name)
        {
            FolderState folder = new FolderState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = ObjectTypeIds.FolderType,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                EventNotifier = EventNotifiers.None
            };

            if (parent != null)
            {
                parent.AddChild(folder);
            }

            return folder;
        }


        /// <summary>
        /// Does any initialization required before the address space can be used.
        /// </summary>
        /// <remarks>
        /// The externalReferences is an out parameter that allows the node manager to link to nodes
        /// in other node managers. For example, the 'Objects' node is managed by the CoreNodeManager and
        /// should have a reference to the root folder node(s) exposed by this node manager.  
        /// </remarks>
        public override void CreateAddressSpace(IDictionary<NodeId, IList<IReference>> externalReferences)
        {
            lock (Lock)
            {
                IList<IReference> references = null;

                if (!externalReferences.TryGetValue(ObjectIds.ObjectsFolder, out references))
                {
                    externalReferences[ObjectIds.ObjectsFolder] = references = new List<IReference>();
                }

                FolderState root = CreateFolder(null, "OpcPublisher", "OpcPublisher");
                root.AddReference(ReferenceTypes.Organizes, true, ObjectIds.ObjectsFolder);
                references.Add(new NodeStateReference(ReferenceTypes.Organizes, false, root.NodeId));
                root.EventNotifier = EventNotifiers.SubscribeToEvents;
                AddRootNotifier(root);

                List<BaseDataVariableState> variables = new List<BaseDataVariableState>();

                try
                {
                    FolderState dataFolder = CreateFolder(root, "Data", "Data");

                    const string connectionStringItemName = "ConnectionString";
                    DataItemState item = CreateDataItemVariable(dataFolder, connectionStringItemName, connectionStringItemName, BuiltInType.String, ValueRanks.Scalar, AccessLevels.CurrentWrite);
                    item.Value = String.Empty;

                    FolderState methodsFolder = CreateFolder(root, "Methods", "Methods");

                    MethodState publishNodeMethod = CreateMethod(methodsFolder, "PublishNode", "PublishNode");
                    SetPublishNodeMethodProperties(ref publishNodeMethod);

                    MethodState unpublishNodeMethod = CreateMethod(methodsFolder, "UnpublishNode", "UnpublishNode");
                    SetUnpublishNodeMethodProperties(ref unpublishNodeMethod);

                    MethodState getPublishedNodesMethod = CreateMethod(methodsFolder, "GetPublishedNodes", "GetPublishedNodes");
                    SetGetPublishedNodesMethodProperties(ref getPublishedNodesMethod);
                }
                catch (Exception e)
                {
                    Utils.Trace(e, "Error creating the address space.");
                }

                AddPredefinedNode(SystemContext, root);
            }
        }

        /// <summary>
        /// Sets properies of the GetPublishedNodes method.
        /// </summary>
        private void SetGetPublishedNodesMethodProperties(ref MethodState method)
        {
            // define input arguments
            method.InputArguments = new PropertyState<Argument[]>(method)
            {
                NodeId = new NodeId(method.BrowseName.Name + "InArgs", NamespaceIndex),
                BrowseName = BrowseNames.InputArguments
            };
            method.InputArguments.DisplayName = method.InputArguments.BrowseName.Name;
            method.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.InputArguments.DataType = DataTypeIds.Argument;
            method.InputArguments.ValueRank = ValueRanks.OneDimension;

            method.InputArguments.Value = new Argument[]
            {
                            new Argument() { Name = "EndpointUri", Description = "Endpoint URI of the OPC UA server to return the published nodes for.",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
            };

            // set output arguments
            method.OutputArguments = new PropertyState<Argument[]>(method)
            {
                NodeId = new NodeId(method.BrowseName.Name + "OutArgs", NamespaceIndex),
                BrowseName = BrowseNames.OutputArguments
            };
            method.OutputArguments.DisplayName = method.OutputArguments.BrowseName.Name;
            method.OutputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.OutputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.OutputArguments.DataType = DataTypeIds.Argument;
            method.OutputArguments.ValueRank = ValueRanks.OneDimension;

            method.OutputArguments.Value = new Argument[]
            {
                        new Argument() { Name = "Published nodes", Description = "List of the nodes published by Publisher",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
            };
            method.OnCallMethod = new GenericMethodCalledEventHandler(OnGetPublishedNodesCall);
        }

        /// <summary>
        /// Sets properies of the PublishNode method.
        /// </summary>
        private void SetPublishNodeMethodProperties(ref MethodState method)
        {
            // define input arguments
            method.InputArguments = new PropertyState<Argument[]>(method)
            {
                NodeId = new NodeId(method.BrowseName.Name + "InArgs", NamespaceIndex),
                BrowseName = BrowseNames.InputArguments
            };
            method.InputArguments.DisplayName = method.InputArguments.BrowseName.Name;
            method.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.InputArguments.DataType = DataTypeIds.Argument;
            method.InputArguments.ValueRank = ValueRanks.OneDimension;

            method.InputArguments.Value = new Argument[]
            {
                            new Argument() { Name = "NodeId", Description = "NodeId of the node to publish in NodeId format.",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar },
                            new Argument() { Name = "EndpointUri", Description = "Endpoint URI of the OPC UA server owning the node.",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar }
            };

            method.OnCallMethod = new GenericMethodCalledEventHandler(OnPublishNodeCall);
        }

        /// <summary>
        /// Sets properies of the UnpublishNode method.
        /// </summary>
        private void SetUnpublishNodeMethodProperties(ref MethodState method)
        {
            // define input arguments
            method.InputArguments = new PropertyState<Argument[]>(method)
            {
                NodeId = new NodeId(method.BrowseName.Name + "InArgs", NamespaceIndex),
                BrowseName = BrowseNames.InputArguments
            };
            method.InputArguments.DisplayName = method.InputArguments.BrowseName.Name;
            method.InputArguments.TypeDefinitionId = VariableTypeIds.PropertyType;
            method.InputArguments.ReferenceTypeId = ReferenceTypeIds.HasProperty;
            method.InputArguments.DataType = DataTypeIds.Argument;
            method.InputArguments.ValueRank = ValueRanks.OneDimension;

            method.InputArguments.Value = new Argument[]
            {
                            new Argument() { Name = "NodeId", Description = "NodeId of the node to publish in NodeId format.",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar },
                            new Argument() { Name = "EndpointUri", Description = "Endpoint URI of the OPC UA server owning the node.",  DataType = DataTypeIds.String, ValueRank = ValueRanks.Scalar },
            };

            method.OnCallMethod = new GenericMethodCalledEventHandler(OnUnpublishNodeCall);
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private DataItemState CreateDataItemVariable(NodeState parent, string path, string name, BuiltInType dataType, int valueRank, byte accessLevel)
        {
            DataItemState variable = new DataItemState(parent);
            variable.ValuePrecision = new PropertyState<double>(variable);
            variable.Definition = new PropertyState<string>(variable);

            variable.Create(
                SystemContext,
                null,
                variable.BrowseName,
                null,
                true);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.NodeId = new NodeId(path, NamespaceIndex);
            variable.BrowseName = new QualifiedName(path, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.None;
            variable.UserWriteMask = AttributeWriteMask.None;
            variable.DataType = (uint)dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = Opc.Ua.TypeInfo.GetDefaultValue((uint)dataType, valueRank, Server.TypeTree);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            variable.ValuePrecision.Value = 2;
            variable.ValuePrecision.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.ValuePrecision.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Definition.Value = String.Empty;
            variable.Definition.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Definition.UserAccessLevel = AccessLevels.CurrentReadOrWrite;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        /// <summary>
        /// Creates a new variable using type Numeric as NodeId.
        /// </summary>
        private DataItemState CreateDataItemVariable(NodeState parent, uint id, string name, BuiltInType dataType, int valueRank, byte accessLevel)
        {
            DataItemState variable = new DataItemState(parent);
            variable.ValuePrecision = new PropertyState<double>(variable);
            variable.Definition = new PropertyState<string>(variable);

            variable.Create(
                SystemContext,
                null,
                variable.BrowseName,
                null,
                true);

            variable.SymbolicName = name;
            variable.ReferenceTypeId = ReferenceTypes.Organizes;
            variable.NodeId = new NodeId(id, NamespaceIndex);
            variable.BrowseName = new QualifiedName(name, NamespaceIndex);
            variable.DisplayName = new LocalizedText("en", name);
            variable.WriteMask = AttributeWriteMask.None;
            variable.UserWriteMask = AttributeWriteMask.None;
            variable.DataType = (uint)dataType;
            variable.ValueRank = valueRank;
            variable.AccessLevel = accessLevel;
            variable.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Historizing = false;
            variable.Value = Opc.Ua.TypeInfo.GetDefaultValue((uint)dataType, valueRank, Server.TypeTree);
            variable.StatusCode = StatusCodes.Good;
            variable.Timestamp = DateTime.UtcNow;

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            variable.ValuePrecision.Value = 2;
            variable.ValuePrecision.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.ValuePrecision.UserAccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Definition.Value = String.Empty;
            variable.Definition.AccessLevel = AccessLevels.CurrentReadOrWrite;
            variable.Definition.UserAccessLevel = AccessLevels.CurrentReadOrWrite;

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        /// <summary>
        /// Creates a new variable.
        /// </summary>
        private BaseDataVariableState CreateVariable(NodeState parent, string path, string name, NodeId dataType, int valueRank)
        {
            BaseDataVariableState variable = new BaseDataVariableState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypes.Organizes,
                TypeDefinitionId = VariableTypeIds.BaseDataVariableType,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                UserWriteMask = AttributeWriteMask.DisplayName | AttributeWriteMask.Description,
                DataType = dataType,
                ValueRank = valueRank,
                AccessLevel = AccessLevels.CurrentReadOrWrite,
                UserAccessLevel = AccessLevels.CurrentReadOrWrite,
                Historizing = false,
                StatusCode = StatusCodes.Good,
                Timestamp = DateTime.UtcNow
            };

            if (valueRank == ValueRanks.OneDimension)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0 });
            }
            else if (valueRank == ValueRanks.TwoDimensions)
            {
                variable.ArrayDimensions = new ReadOnlyList<uint>(new List<uint> { 0, 0 });
            }

            if (parent != null)
            {
                parent.AddChild(variable);
            }

            return variable;
        }

        /// <summary>
        /// Creates a new method.
        /// </summary>
        private MethodState CreateMethod(NodeState parent, string path, string name)
        {
            MethodState method = new MethodState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                NodeId = new NodeId(path, NamespaceIndex),
                BrowseName = new QualifiedName(path, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                Executable = true,
                UserExecutable = true
            };

            if (parent != null)
            {
                parent.AddChild(method);
            }

            return method;
        }

        /// <summary>
        /// Creates a new method using type Numeric for the NodeId.
        /// </summary>
        private MethodState CreateMethod(NodeState parent, uint id, string name)
        {
            MethodState method = new MethodState(parent)
            {
                SymbolicName = name,
                ReferenceTypeId = ReferenceTypeIds.HasComponent,
                NodeId = new NodeId(id, NamespaceIndex),
                BrowseName = new QualifiedName(name, NamespaceIndex),
                DisplayName = new LocalizedText("en", name),
                WriteMask = AttributeWriteMask.None,
                UserWriteMask = AttributeWriteMask.None,
                Executable = true,
                UserExecutable = true
            };

            if (parent != null)
            {
                parent.AddChild(method);
            }

            return method;
        }

        /// <summary>
        /// Method to start monitoring a node and publish the data to IoTHub. Executes synchronously.
        /// </summary>
        private ServiceResult OnPublishNodeCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            if (string.IsNullOrEmpty(inputArguments[0] as string) || string.IsNullOrEmpty(inputArguments[1] as string))
            {
                Trace("PublishNode: Invalid Arguments when trying to publish a node.");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide all arguments as strings!");
            }

            NodeId nodeId = null;
            Uri endpointUri = null;
            try
            {
                nodeId = NodeId.Parse(inputArguments[0] as string);
                endpointUri = new Uri(inputArguments[1] as string);
            }
            catch (UriFormatException)
            {
                Trace($"PublishNode: The EndpointUri has an invalid format '{inputArguments[1] as string}'!");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide a valid OPC UA endpoint URL as second argument!");
            }
            catch (Exception e)
            {
                Trace(e, $"PublishNode: The NodeId has an invalid format '{inputArguments[0] as string}'!");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide a valid OPC UA NodeId in NodeId format as first argument!");
            }

            // find/create a session to the endpoint URL and start monitoring the node.
            try
            {
                // lock the publishing configuration till we are done
                OpcSessionsListSemaphore.WaitAsync();

                if (ShutdownTokenSource.IsCancellationRequested)
                {
                    return ServiceResult.Create(StatusCodes.BadUnexpectedError, $"Publisher shutdown in progress.");
                }

                // find the session we need to monitor the node
                OpcSession opcSession = null;
                opcSession = OpcSessions.FirstOrDefault(s => s.EndpointUri.AbsoluteUri.Equals(endpointUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase));
                string namespaceUri = null;
                ExpandedNodeId expandedNodeId = null;

                // add a new session.
                if (opcSession == null)
                {
                    // create new session info.
                    opcSession = new OpcSession(endpointUri, true, OpcSessionCreationTimeout);
                    OpcSessions.Add(opcSession);
                    Trace($"PublishNode: No matching session found for endpoint '{endpointUri.OriginalString}'. Requested to create a new one.");
                }
                else
                {
                    Trace($"PublishNode: Session found for endpoint '{endpointUri.OriginalString}'");

                    // check if node is already published
                    namespaceUri = opcSession.GetNamespaceUri(nodeId.NamespaceIndex);
                    if (string.IsNullOrEmpty(namespaceUri))
                    {
                        return ServiceResult.Create(StatusCodes.BadUnexpectedError, $"The namespace index of the node id is invalid.");
                    }
                    expandedNodeId = new ExpandedNodeId(nodeId.Identifier, nodeId.NamespaceIndex, namespaceUri, 0);
                }

                // add the node info to the subscription with the default publishing interval, execute syncronously
                opcSession.AddNodeForMonitoring(nodeId, expandedNodeId, OpcPublishingInterval, OpcSamplingInterval, ShutdownTokenSource.Token).Wait();
                Trace($"PublishNode: Requested to monitor item with NodeId '{nodeId.ToString()}' (PublishingInterval: {OpcPublishingInterval}, SamplingInterval: {OpcSamplingInterval})");
            }
            catch (Exception e)
            {
                Trace(e, $"PublishNode: Exception while trying to configure publishing node '{nodeId.ToString()}'");
                return ServiceResult.Create(e, StatusCodes.BadUnexpectedError, $"Unexpected error publishing node: {e.Message}");
            }
            finally
            {
                OpcSessionsListSemaphore.Release();
            }
            return ServiceResult.Good;
        }

        /// <summary>
        /// Method to remove the node from the subscription and stop publishing telemetry to IoTHub. Executes synchronously.
        /// </summary>
        private ServiceResult OnUnpublishNodeCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            if (string.IsNullOrEmpty(inputArguments[0] as string) || string.IsNullOrEmpty(inputArguments[1] as string))
            {
                Trace("UnpublishNode: Invalid arguments!");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide all arguments!");
            }

            NodeId nodeId = null;
            Uri endpointUri = null;
            try
            {
                nodeId = NodeId.Parse(inputArguments[0] as string);
                endpointUri = new Uri(inputArguments[1] as string);
            }
            catch (UriFormatException)
            {
                Trace($"UnpublishNode: The endpointUrl is invalid '{inputArguments[1] as string}'!");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide a valid OPC UA endpoint URL as second argument!");
            }

            // find the session and stop monitoring the node.
            try
            {
                if (ShutdownTokenSource.IsCancellationRequested)
                {
                    return ServiceResult.Create(StatusCodes.BadUnexpectedError, $"Publisher shutdown in progress.");
                }

                // find the session we need to monitor the node
                OpcSession opcSession = null;
                try
                {
                    OpcSessionsListSemaphore.WaitAsync();
                    opcSession = OpcSessions.FirstOrDefault(s => s.EndpointUri.AbsoluteUri.Equals(endpointUri.AbsoluteUri, StringComparison.OrdinalIgnoreCase));
                }
                catch
                {
                    opcSession = null;
                }
                finally
                {
                    OpcSessionsListSemaphore.Release();

                }


                ExpandedNodeId expandedNodeId = null;
                if (opcSession == null)
                {
                    // do nothing if there is no session for this endpoint.
                    Trace($"UnpublishNode: Session for endpoint '{endpointUri.OriginalString}' not found.");
                    return ServiceResult.Create(StatusCodes.BadSessionIdInvalid, "Session for endpoint of node to unpublished not found!");
                }
                else
                {
                    // check if node is already published
                    string namespaceUri = opcSession.GetNamespaceUri(nodeId.NamespaceIndex);
                    if (string.IsNullOrEmpty(namespaceUri))
                    {
                        return ServiceResult.Create(StatusCodes.BadUnexpectedError, $"The namespace index of the node id is invalid.");
                    }
                    expandedNodeId = new ExpandedNodeId(nodeId.Identifier, nodeId.NamespaceIndex, namespaceUri, 0);
                    if (!OpcSession.IsNodePublished(nodeId, expandedNodeId, endpointUri))
                    {
                        Trace($"UnpublishNode: Node with id '{nodeId.Identifier.ToString()}' on endpoint '{endpointUri.OriginalString}' is not published.");
                        return ServiceResult.Good;
                    }
                }

                // remove the node from the sessions monitored items list.
                opcSession.RequestMonitorItemRemoval(nodeId, expandedNodeId, ShutdownTokenSource.Token).Wait();
                Trace("UnpublishNode: Requested to stop monitoring of node.");
            }
            catch (Exception e)
            {
                Trace(e, $"UnpublishNode: Exception while trying to configure publishing node '{nodeId.ToString()}'");
                return ServiceResult.Create(e, StatusCodes.BadUnexpectedError, $"Unexpected error unpublishing node: {e.Message}");
            }
            return ServiceResult.Good;
        }

        /// <summary>
        /// Method to get the list of published nodes. Executes synchronously.
        /// The format of the returned node description is using NodeId format. The assumption
        /// is that the caller is able to access the namespace array of the server
        /// on the endpoint URL(s) themselve and do the correct mapping.
        /// </summary>
        private ServiceResult OnGetPublishedNodesCall(ISystemContext context, MethodState method, IList<object> inputArguments, IList<object> outputArguments)
        {
            Uri endpointUri = null;

            if (string.IsNullOrEmpty(inputArguments[0] as string))
            {
                Trace("GetPublishedNodes: Return all published nodes");
            }
            else
            {
                try
                {
                    endpointUri = new Uri(inputArguments[0] as string);
                }
                catch (UriFormatException)
                {
                    Trace($"GetPublishedNodes: The endpointUrl is invalid '{inputArguments[0] as string}'!");
                    return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide a valid OPC UA endpoint URL as first argument!");
                }
            }

            // get the list of published nodes in NodeId format
            outputArguments[0] = JsonConvert.SerializeObject(GetPublisherConfigurationFileEntries(endpointUri, OpcMonitoredItemConfigurationType.NodeId, false));
            Trace("GetPublishedNodes: Success!");

            return ServiceResult.Good;
        }

        /// <summary>
        /// Data node in the server which registers ourselves with IoT Hub when this node is written. Executes synchronously.
        /// </summary>
        public ServiceResult OnConnectionStringWrite(ISystemContext context, NodeState node, NumericRange indexRange, QualifiedName dataEncoding, ref object value, ref StatusCode statusCode, ref DateTime timestamp)
        {
            var connectionString = value as string;
            if (string.IsNullOrEmpty(connectionString))
            {
                Trace("ConnectionStringWrite: Invalid Argument!");
                return ServiceResult.Create(StatusCodes.BadArgumentsMissing, "Please provide all arguments as strings!");
            }

            statusCode = StatusCodes.Bad;
            timestamp = DateTime.Now;

            // read current connection string and compare to the one passed in
            string currentConnectionString = SecureIoTHubToken.ReadAsync(PublisherOpcApplicationConfiguration.ApplicationName, IotDeviceCertStoreType, IotDeviceCertStorePath).Result;
            if (connectionString.Equals(currentConnectionString, StringComparison.OrdinalIgnoreCase))
            {
                Trace("ConnectionStringWrite: Connection string up to date!");
                return ServiceResult.Create(StatusCodes.Bad, "Connection string already up-to-date!");
            }

            Trace($"ConnectionStringWrite: Attempting to configure publisher with connection string: {connectionString}");

            // configure publisher and write connection string
            try
            {
                IotHubCommunication.ConnectionStringWriteAsync(connectionString).Wait();
            }
            catch (Exception e)
            {
                statusCode = StatusCodes.Bad;
                Trace(e, $"ConnectionStringWrite: Exception while trying to create IoTHub client and store device connection string in cert store");
                return ServiceResult.Create(StatusCodes.Bad, "Publisher registration failed: " + e.Message);
            }

            statusCode = StatusCodes.Good;
            Trace("ConnectionStringWrite: Success!");

            return statusCode;
        }
    }
}
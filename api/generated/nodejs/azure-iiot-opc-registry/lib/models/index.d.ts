/*
 * Copyright (c) Microsoft Corporation. All rights reserved.
 * Licensed under the MIT License. See License.txt in the project root for license information.
 *
 * Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
 * Changes may cause incorrect behavior and will be lost if the code is regenerated.
 */

import * as moment from "moment";

/**
 * Endpoint Activation Filter model
 */
export interface EndpointActivationFilterApiModel {
  /**
   * Certificate trust list identifiers to use for
   * activation, if null, all certificates are
   * trusted.  If empty list, no certificates are
   * trusted which is equal to no filter.
   */
  trustLists?: string[];
  /**
   * Endpoint security policies to filter against.
   * If set to null, all policies are in scope.
   */
  securityPolicies?: string[];
  /**
   * Possible values include: 'Best', 'Sign', 'SignAndEncrypt', 'None'
   */
  securityMode?: string;
}

/**
 * Application registration request
 */
export interface ServerRegistrationRequestApiModel {
  /**
   * Discovery url to use for registration
   */
  discoveryUrl: string;
  /**
   * Registration id
   */
  id?: string;
  activationFilter?: EndpointActivationFilterApiModel;
}

/**
 * Application information
*/
export interface ApplicationRegistrationRequestApiModel {
  /**
   * Unique application uri
  */
  applicationUri: string;
  /**
   * Possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'
  */
  applicationType?: string;
  /**
   * Product uri of the application.
  */
  productUri?: string;
  /**
   * Default name of the server or client.
  */
  applicationName?: string;
  /**
   * Locale of default name
  */
  locale?: string;
  /**
   * Site of the application
  */
  siteId?: string;
  /**
   * Localized names key off locale id.
  */
  localizedNames?: { [propertyName: string]: string };
  /**
   * The OPC UA defined capabilities of the server.
  */
  capabilities?: string[];
  /**
   * Discovery urls of the server.
  */
  discoveryUrls?: string[];
  /**
   * The discovery profile uri of the server.
  */
  discoveryProfileUri?: string;
  /**
   * Gateway server uri
  */
  gatewayServerUri?: string;
}

/**
 * Result of an application registration
*/
export interface ApplicationRegistrationResponseApiModel {
  /**
   * New id application was registered under
  */
  id?: string;
}

/**
 * Registry operation log model
*/
export interface RegistryOperationApiModel {
  /**
   * Operation User
  */
  authorityId: string;
  /**
   * Operation time
  */
  time: Date;
}

/**
 * Application info model
*/
export interface ApplicationInfoApiModel {
  /**
   * Unique application id
  */
  applicationId?: string;
  /**
   * Possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'
  */
  applicationType?: string;
  /**
   * Unique application uri
  */
  applicationUri?: string;
  /**
   * Product uri
  */
  productUri?: string;
  /**
   * Default name of application
  */
  applicationName?: string;
  /**
   * Locale of default name - defaults to "en"
  */
  locale?: string;
  /**
   * Localized Names of application keyed on locale
  */
  localizedNames?: { [propertyName: string]: string };
  /**
   * Application public cert
  */
  certificate?: Buffer;
  /**
   * The capabilities advertised by the server.
  */
  capabilities?: string[];
  /**
   * Discovery urls of the server
  */
  discoveryUrls?: string[];
  /**
   * Discovery profile uri
  */
  discoveryProfileUri?: string;
  /**
   * Gateway server uri
  */
  gatewayServerUri?: string;
  /**
   * Host addresses of server application or null
  */
  hostAddresses?: string[];
  /**
   * Site of the application
  */
  siteId?: string;
  /**
   * Discoverer that registered the application
  */
  discovererId?: string;
  /**
   * Last time application was seen
  */
  notSeenSince?: Date;
  created?: RegistryOperationApiModel;
  updated?: RegistryOperationApiModel;
}

/**
 * List of registered applications
*/
export interface ApplicationInfoListApiModel {
  /**
   * Application infos
  */
  items?: ApplicationInfoApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Discovery configuration
*/
export interface DiscoveryConfigApiModel {
  /**
   * Address ranges to scan (null == all wired nics)
  */
  addressRangesToScan?: string;
  /**
   * Network probe timeout
  */
  networkProbeTimeoutMs?: number;
  /**
   * Max network probes that should ever run.
  */
  maxNetworkProbes?: number;
  /**
   * Port ranges to scan (null == all unassigned)
  */
  portRangesToScan?: string;
  /**
   * Port probe timeout
  */
  portProbeTimeoutMs?: number;
  /**
   * Max port probes that should ever run.
  */
  maxPortProbes?: number;
  /**
   * Probes that must always be there as percent of max.
  */
  minPortProbesPercent?: number;
  /**
   * Delay time between discovery sweeps in seconds
  */
  idleTimeBetweenScansSec?: number;
  /**
   * List of preset discovery urls to use
  */
  discoveryUrls?: string[];
  /**
   * List of locales to filter with during discovery
  */
  locales?: string[];
  activationFilter?: EndpointActivationFilterApiModel;
}

/**
 * Discovery request
*/
export interface DiscoveryRequestApiModel {
  /**
   * Id of discovery request
  */
  id?: string;
  /**
   * Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'
  */
  discovery?: string;
  configuration?: DiscoveryConfigApiModel;
}

/**
 * Endpoint model
*/
export interface EndpointApiModel {
  /**
   * Endpoint url to use to connect with
  */
  url: string;
  /**
   * Alternative endpoint urls that can be used for
   * accessing and validating the server
  */
  alternativeUrls?: string[];
  /**
   * Possible values include: 'Best', 'Sign', 'SignAndEncrypt', 'None'
  */
  securityMode?: string;
  /**
   * Security policy uri to use for communication
   * default to best.
  */
  securityPolicy?: string;
  /**
   * Endpoint certificate that was registered.
  */
  certificate?: Buffer;
}

/**
 * Authentication Method model
*/
export interface AuthenticationMethodApiModel {
  /**
   * Authentication method id
  */
  id: string;
  /**
   * Possible values include: 'None', 'UserName', 'X509Certificate', 'JwtToken'
  */
  credentialType?: string;
  /**
   * Security policy to use when passing credential.
  */
  securityPolicy?: string;
  /**
   * Method specific configuration
  */
  configuration?: any;
}

/**
 * Endpoint registration model
*/
export interface EndpointRegistrationApiModel {
  /**
   * Registered identifier of the endpoint
  */
  id: string;
  /**
   * Original endpoint url of the endpoint
  */
  endpointUrl?: string;
  /**
   * Registered site of the endpoint
  */
  siteId?: string;
  /**
   * Supervisor that manages the endpoint.
  */
  supervisorId?: string;
  /**
   * Discoverer that registered the endpoint
  */
  discovererId?: string;
  endpoint: EndpointApiModel;
  /**
   * Security level of the endpoint
  */
  securityLevel?: number;
  /**
   * Supported authentication methods that can be selected to
   * obtain a credential and used to interact with the endpoint.
  */
  authenticationMethods?: AuthenticationMethodApiModel[];
}

/**
 * Application with list of endpoints
*/
export interface ApplicationRegistrationApiModel {
  application: ApplicationInfoApiModel;
  /**
   * List of endpoint twins
  */
  endpoints?: EndpointRegistrationApiModel[];
  /**
   * Possible values include: 'Unknown', 'Low', 'Medium', 'High'
  */
  securityAssessment?: string;
}

/**
 * Application registration update request
*/
export interface ApplicationRegistrationUpdateApiModel {
  /**
   * Product uri
  */
  productUri?: string;
  /**
   * Default name of the server or client.
  */
  applicationName?: string;
  /**
   * Locale of default name - defaults to "en"
  */
  locale?: string;
  /**
   * Localized names keyed off locale id.
   * To remove entry, set value for locale id to null.
  */
  localizedNames?: { [propertyName: string]: string };
  /**
   * Application public cert
  */
  certificate?: Buffer;
  /**
   * Capabilities of the application
  */
  capabilities?: string[];
  /**
   * Discovery urls of the application
  */
  discoveryUrls?: string[];
  /**
   * Discovery profile uri
  */
  discoveryProfileUri?: string;
  /**
   * Gateway server uri
  */
  gatewayServerUri?: string;
}

/**
 * List of application sites
*/
export interface ApplicationSiteListApiModel {
  /**
   * Distinct list of sites applications were registered in.
  */
  sites?: string[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Application information
*/
export interface ApplicationRegistrationQueryApiModel {
  /**
   * Possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'
  */
  applicationType?: string;
  /**
   * Application uri
  */
  applicationUri?: string;
  /**
   * Product uri
  */
  productUri?: string;
  /**
   * Name of application
  */
  applicationName?: string;
  /**
   * Locale of application name - default is "en"
  */
  locale?: string;
  /**
   * Application capability to query with
  */
  capability?: string;
  /**
   * Discovery profile uri
  */
  discoveryProfileUri?: string;
  /**
   * Gateway server uri
  */
  gatewayServerUri?: string;
  /**
   * Supervisor or site the application belongs to.
  */
  siteOrGatewayId?: string;
  /**
   * Whether to include apps that were soft deleted
  */
  includeNotSeenSince?: boolean;
}

/**
 * Query by id
*/
export interface ApplicationRecordQueryApiModel {
  /**
   * Starting record id
  */
  startingRecordId?: number;
  /**
   * Max records to return
  */
  maxRecordsToReturn?: number;
  /**
   * Application name
  */
  applicationName?: string;
  /**
   * Application uri
  */
  applicationUri?: string;
  /**
   * Possible values include: 'Server', 'Client', 'ClientAndServer', 'DiscoveryServer'
  */
  applicationType?: string;
  /**
   * Product uri
  */
  productUri?: string;
  /**
   * Server capabilities
  */
  serverCapabilities?: string[];
}

/**
 * Application with optional list of endpoints
*/
export interface ApplicationRecordApiModel {
  /**
   * Record id
  */
  recordId: number;
  application: ApplicationInfoApiModel;
}

/**
 * Create response
*/
export interface ApplicationRecordListApiModel {
  /**
   * Applications found
  */
  applications?: ApplicationRecordApiModel[];
  /**
   * Last counter reset
  */
  lastCounterResetTime: Date;
  /**
   * Next record id
  */
  nextRecordId: number;
}

/**
 * Discoverer registration model
*/
export interface DiscovererApiModel {
  /**
   * Discoverer id
  */
  id: string;
  /**
   * Site of the discoverer
  */
  siteId?: string;
  /**
   * Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'
  */
  discovery?: string;
  discoveryConfig?: DiscoveryConfigApiModel;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
  /**
   * Whether the registration is out of sync between
   * client (module) and server (service) (default: false).
  */
  outOfSync?: boolean;
  /**
   * Whether discoverer is connected on this registration
  */
  connected?: boolean;
}

/**
 * Discoverer update request
*/
export interface DiscovererUpdateApiModel {
  /**
   * Site the discoverer is part of
  */
  siteId?: string;
  /**
   * Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'
  */
  discovery?: string;
  discoveryConfig?: DiscoveryConfigApiModel;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
}

/**
 * Discoverer registration list
*/
export interface DiscovererListApiModel {
  /**
   * Registrations
  */
  items?: DiscovererApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Discoverer registration query
*/
export interface DiscovererQueryApiModel {
  /**
   * Site of the discoverer
  */
  siteId?: string;
  /**
   * Possible values include: 'Off', 'Local', 'Network', 'Fast', 'Scan'
  */
  discovery?: string;
  /**
   * Included connected or disconnected
  */
  connected?: boolean;
}

/**
 * Endpoint registration model
*/
export interface EndpointInfoApiModel {
  registration: EndpointRegistrationApiModel;
  /**
   * Application id endpoint is registered under.
  */
  applicationId: string;
  /**
   * Possible values include: 'Deactivated', 'Activated', 'ActivatedAndConnected'
  */
  activationState?: string;
  /**
   * Possible values include: 'Connecting', 'NotReachable', 'Busy', 'NoTrust',
   * 'CertificateInvalid', 'Ready', 'Error'
  */
  endpointState?: string;
  /**
   * Whether the registration is out of sync
  */
  outOfSync?: boolean;
  /**
   * Last time endpoint was seen
  */
  notSeenSince?: Date;
}

/**
 * Endpoint registration list
*/
export interface EndpointInfoListApiModel {
  /**
   * Endpoint registrations
  */
  items?: EndpointInfoApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Endpoint query
*/
export interface EndpointRegistrationQueryApiModel {
  /**
   * Endoint url for direct server access
  */
  url?: string;
  /**
   * Certificate of the endpoint
  */
  certificate?: Buffer;
  /**
   * Possible values include: 'Best', 'Sign', 'SignAndEncrypt', 'None'
  */
  securityMode?: string;
  /**
   * Security policy uri
  */
  securityPolicy?: string;
  /**
   * Whether the endpoint was activated
  */
  activated?: boolean;
  /**
   * Whether the endpoint is connected on supervisor.
  */
  connected?: boolean;
  /**
   * Possible values include: 'Connecting', 'NotReachable', 'Busy', 'NoTrust',
   * 'CertificateInvalid', 'Ready', 'Error'
  */
  endpointState?: string;
  /**
   * Whether to include endpoints that were soft deleted
  */
  includeNotSeenSince?: boolean;
  /**
   * Discoverer id to filter with
  */
  discovererId?: string;
  /**
   * Application id to filter
  */
  applicationId?: string;
  /**
   * Supervisor id to filter with
  */
  supervisorId?: string;
  /**
   * Site or gateway id to filter with
  */
  siteOrGatewayId?: string;
}

/**
 * Gateway registration model
*/
export interface GatewayApiModel {
  /**
   * Gateway id
  */
  id: string;
  /**
   * Site of the Gateway
  */
  siteId?: string;
  /**
   * Whether Gateway is connected on this registration
  */
  connected?: boolean;
}

/**
 * Supervisor registration model
*/
export interface SupervisorApiModel {
  /**
   * Supervisor id
  */
  id: string;
  /**
   * Site of the supervisor
  */
  siteId?: string;
  /**
   * Supervisor public client cert
  */
  certificate?: Buffer;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
  /**
   * Whether the registration is out of sync between
   * client (module) and server (service) (default: false).
  */
  outOfSync?: boolean;
  /**
   * Whether supervisor is connected on this registration
  */
  connected?: boolean;
}

/**
 * Default publisher agent configuration
*/
export interface PublisherConfigApiModel {
  /**
   * Capabilities
  */
  capabilities?: { [propertyName: string]: string };
  /**
   * Interval to check job
  */
  jobCheckInterval?: string;
  /**
   * Heartbeat interval
  */
  heartbeatInterval?: string;
  /**
   * Parallel jobs
  */
  maxWorkers?: number;
  /**
   * Job orchestrator endpoint url
  */
  jobOrchestratorUrl?: string;
}

/**
 * Publisher registration model
*/
export interface PublisherApiModel {
  /**
   * Publisher id
  */
  id: string;
  /**
   * Site of the publisher
  */
  siteId?: string;
  /**
   * Publisher public client cert
  */
  certificate?: Buffer;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
  configuration?: PublisherConfigApiModel;
  /**
   * Whether the registration is out of sync between
   * client (module) and server (service) (default: false).
  */
  outOfSync?: boolean;
  /**
   * Whether publisher is connected on this registration
  */
  connected?: boolean;
}

/**
 * Gateway info model
*/
export interface GatewayInfoApiModel {
  gateway: GatewayApiModel;
  supervisor?: SupervisorApiModel;
  publisher?: PublisherApiModel;
  discoverer?: DiscovererApiModel;
}

/**
 * Gateway registration update request
*/
export interface GatewayUpdateApiModel {
  /**
   * Site of the Gateway
  */
  siteId?: string;
}

/**
 * Gateway registration list
*/
export interface GatewayListApiModel {
  /**
   * Registrations
  */
  items?: GatewayApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Gateway registration query
*/
export interface GatewayQueryApiModel {
  /**
   * Site of the Gateway
  */
  siteId?: string;
  /**
   * Included connected or disconnected
  */
  connected?: boolean;
}

/**
 * Publisher registration update request
*/
export interface PublisherUpdateApiModel {
  /**
   * Site of the publisher
  */
  siteId?: string;
  configuration?: PublisherConfigApiModel;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
}

/**
 * Publisher registration list
*/
export interface PublisherListApiModel {
  /**
   * Registrations
  */
  items?: PublisherApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Publisher registration query
*/
export interface PublisherQueryApiModel {
  /**
   * Site of the publisher
  */
  siteId?: string;
  /**
   * Included connected or disconnected
  */
  connected?: boolean;
}

/**
 * Status response model
*/
export interface StatusResponseApiModel {
  /**
   * Name of this service
  */
  name?: string;
  /**
   * Operational status
  */
  status?: string;
  /**
   * Current time
  */
  readonly currentTime?: string;
  /**
   * Start time of service
  */
  readonly startTime?: string;
  /**
   * Up time of service
  */
  readonly upTime?: number;
  /**
   * Value generated at bootstrap by each instance of the service and
   * used to correlate logs coming from the same instance. The value
   * changes every time the service starts.
  */
  readonly uid?: string;
  /**
   * A property bag with details about the service
  */
  readonly properties?: { [propertyName: string]: string };
  /**
   * A property bag with details about the internal dependencies
  */
  readonly dependencies?: { [propertyName: string]: string };
  readonly metadata?: { [propertyName: string]: string };
}

/**
 * Supervisor registration update request
*/
export interface SupervisorUpdateApiModel {
  /**
   * Site of the supervisor
  */
  siteId?: string;
  /**
   * Possible values include: 'Error', 'Information', 'Debug', 'Verbose'
  */
  logLevel?: string;
}

/**
 * Endpoint Activation status model
*/
export interface EndpointActivationStatusApiModel {
  /**
   * Identifier of the endoint
  */
  id: string;
  /**
   * Possible values include: 'Deactivated', 'Activated', 'ActivatedAndConnected'
  */
  activationState?: string;
}

/**
 * Supervisor runtime status
*/
export interface SupervisorStatusApiModel {
  /**
   * Edge device id
  */
  deviceId: string;
  /**
   * Module id
  */
  moduleId?: string;
  /**
   * Site id
  */
  siteId?: string;
  /**
   * Endpoint activation status
  */
  endpoints?: EndpointActivationStatusApiModel[];
}

/**
 * Supervisor registration list
*/
export interface SupervisorListApiModel {
  /**
   * Registrations
  */
  items?: SupervisorApiModel[];
  /**
   * Continuation or null if final
  */
  continuationToken?: string;
}

/**
 * Supervisor registration query
*/
export interface SupervisorQueryApiModel {
  /**
   * Site of the supervisor
  */
  siteId?: string;
  /**
   * Included connected or disconnected
  */
  connected?: boolean;
}

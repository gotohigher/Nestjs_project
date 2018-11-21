// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Api.Registry.Clients {
    using Microsoft.Azure.IIoT.OpcUa.Api.Registry.Models;
    using Microsoft.Azure.IIoT.Diagnostics;
    using Microsoft.Azure.IIoT.Http;
    using Newtonsoft.Json;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Implementation of v1 registry service api.
    /// </summary>
    public class RegistryServiceClient : IRegistryServiceApi {

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="config"></param>
        /// <param name="logger"></param>
        public RegistryServiceClient(IHttpClient httpClient, IRegistryConfig config,
            ILogger logger) :
            this (httpClient, config.OpcUaRegistryServiceUrl,
                config.OpcUaRegistryServiceResourceId, logger){
        }

        /// <summary>
        /// Create service client
        /// </summary>
        /// <param name="httpClient"></param>
        /// <param name="serviceUri"></param>
        /// <param name="resourceId"></param>
        /// <param name="logger"></param>
        public RegistryServiceClient(IHttpClient httpClient, string serviceUri,
            string resourceId, ILogger logger) {
            if (string.IsNullOrEmpty(serviceUri)) {
                throw new ArgumentNullException(nameof(serviceUri),
                    "Please configure the Url of the registry micro service.");
            }
            _serviceUri = serviceUri;
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _resourceId = resourceId;
        }

        /// <inheritdoc/>
        public async Task<StatusResponseApiModel> GetServiceStatusAsync() {
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/status", _resourceId);
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<StatusResponseApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<SupervisorListApiModel> ListSupervisorsAsync(
            string continuation, bool? onlyServerState, int? pageSize) {
            var uri = new UriBuilder($"{_serviceUri}/v1/supervisors");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            if (continuation != null) {
                request.AddHeader(kContinuationTokenHeaderKey, continuation);
            }
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<SupervisorListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<SupervisorListApiModel> QuerySupervisorsAsync(
            SupervisorQueryApiModel query, bool? onlyServerState, int? pageSize) {
            var uri = new UriBuilder($"{_serviceUri}/v1/supervisors/query");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            request.SetContent(query);
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<SupervisorListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<SupervisorApiModel> GetSupervisorAsync(
            string supervisorId, bool? onlyServerState) {
            if (string.IsNullOrEmpty(supervisorId)) {
                throw new ArgumentNullException(nameof(supervisorId));
            }
            var uri = new UriBuilder($"{_serviceUri}/v1/supervisors/{supervisorId}");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<SupervisorApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task UpdateSupervisorAsync(SupervisorUpdateApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/supervisors",
                _resourceId);
            request.SetContent(content);
            var response = await _httpClient.PatchAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task RegisterAsync(ServerRegistrationRequestApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.DiscoveryUrl == null) {
                throw new ArgumentNullException(nameof(content.DiscoveryUrl));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications",
                _resourceId);
            request.SetContent(content);
            request.Options.Timeout = 60000;
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task DiscoverAsync(DiscoveryRequestApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications/discover",
                _resourceId);
            request.SetContent(content);
            request.Options.Timeout = 60000;
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task<ApplicationRegistrationResponseApiModel> RegisterAsync(
            ApplicationRegistrationRequestApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            if (content.ApplicationUri == null) {
                throw new ArgumentNullException(nameof(content.ApplicationUri));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications",
                _resourceId);
            request.SetContent(content);
            var response = await _httpClient.PutAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<ApplicationRegistrationResponseApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task UpdateApplicationAsync(ApplicationRegistrationUpdateApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications",
                _resourceId);
            request.SetContent(content);
            var response = await _httpClient.PatchAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task<ApplicationRegistrationApiModel> GetApplicationAsync(
            string applicationId) {
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications/{applicationId}",
                _resourceId);
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<ApplicationRegistrationApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<ApplicationInfoListApiModel> QueryApplicationsAsync(
            ApplicationRegistrationQueryApiModel query, int? pageSize) {
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications/query",
                _resourceId);
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            request.SetContent(query);
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<ApplicationInfoListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<ApplicationInfoListApiModel> ListApplicationsAsync(
            string continuation, int? pageSize) {
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications",
                _resourceId);
            if (continuation != null) {
                request.AddHeader(kContinuationTokenHeaderKey, continuation);
            }
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<ApplicationInfoListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<ApplicationSiteListApiModel> ListSitesAsync(
            string continuation, int? pageSize) {
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications/sites",
                _resourceId);
            if (continuation != null) {
                request.AddHeader(kContinuationTokenHeaderKey, continuation);
            }
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<ApplicationSiteListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task UnregisterApplicationAsync(string applicationId) {
            if (string.IsNullOrEmpty(applicationId)) {
                throw new ArgumentNullException(nameof(applicationId));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/applications/{applicationId}",
                _resourceId);
            var response = await _httpClient.DeleteAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task PurgeDisabledApplicationsAsync(TimeSpan notSeenFor) {
            var request = _httpClient.NewRequest(
                $"{_serviceUri}/v1/applications?notSeenFor={notSeenFor}", _resourceId);
            var response = await _httpClient.DeleteAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task<EndpointInfoListApiModel> ListEndpointsAsync(string continuation,
            bool? onlyServerState, int? pageSize) {
            var uri = new UriBuilder($"{_serviceUri}/v1/endpoints");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            if (continuation != null) {
                request.AddHeader(kContinuationTokenHeaderKey, continuation);
            }
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<EndpointInfoListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<EndpointInfoListApiModel> QueryEndpointsAsync(EndpointRegistrationQueryApiModel query,
            bool? onlyServerState, int? pageSize) {
            var uri = new UriBuilder($"{_serviceUri}/v1/endpoints/query");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            if (pageSize != null) {
                request.AddHeader(kPageSizeHeaderKey, pageSize.ToString());
            }
            request.SetContent(query);
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<EndpointInfoListApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task<EndpointInfoApiModel> GetEndpointAsync(string endpointId,
            bool? onlyServerState) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            var uri = new UriBuilder($"{_serviceUri}/v1/endpoints/{endpointId}");
            if (onlyServerState ?? false) {
                uri.Query = "onlyServerState=true";
            }
            var request = _httpClient.NewRequest(uri.Uri, _resourceId);
            var response = await _httpClient.GetAsync(request).ConfigureAwait(false);
            response.Validate();
            return JsonConvertEx.DeserializeObject<EndpointInfoApiModel>(
                response.GetContentAsString());
        }

        /// <inheritdoc/>
        public async Task UpdateEndpointAsync(EndpointRegistrationUpdateApiModel content) {
            if (content == null) {
                throw new ArgumentNullException(nameof(content));
            }
            var request = _httpClient.NewRequest($"{_serviceUri}/v1/endpoints",
                _resourceId);
            request.SetContent(content);
            var response = await _httpClient.PatchAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task ActivateEndpointAsync(string endpointId) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            var request = _httpClient.NewRequest(
                $"{_serviceUri}/v1/endpoints/{endpointId}/activate", _resourceId);
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        /// <inheritdoc/>
        public async Task DeactivateEndpointAsync(string endpointId) {
            if (string.IsNullOrEmpty(endpointId)) {
                throw new ArgumentNullException(nameof(endpointId));
            }
            var request = _httpClient.NewRequest(
                $"{_serviceUri}/v1/endpoints/{endpointId}/deactivate", _resourceId);
            var response = await _httpClient.PostAsync(request).ConfigureAwait(false);
            response.Validate();
        }

        private const string kContinuationTokenHeaderKey = "x-ms-continuation";
        private const string kPageSizeHeaderKey = "x-ms-max-item-count";
        private readonly IHttpClient _httpClient;
        private readonly ILogger _logger;
        private readonly string _serviceUri;
        private readonly string _resourceId;
    }
}

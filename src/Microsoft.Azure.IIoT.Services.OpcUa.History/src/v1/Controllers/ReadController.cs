// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Services.OpcUa.History.v1.Controllers {
    using Microsoft.Azure.IIoT.Services.OpcUa.History.v1.Auth;
    using Microsoft.Azure.IIoT.Services.OpcUa.History.v1.Filters;
    using Microsoft.Azure.IIoT.Services.OpcUa.History.v1.Models;
    using Microsoft.Azure.IIoT.OpcUa.History;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Threading.Tasks;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;

    /// <summary>
    /// Historic events access services
    /// </summary>
    [Route(VersionInfo.PATH + "/read")]
    [ExceptionsFilter]
    [Produces(ContentEncodings.MimeTypeJson)]
    [Authorize(Policy = Policies.CanRead)]
    public class ReadController : Controller {

        /// <summary>
        /// Create controller with service
        /// </summary>
        /// <param name="historian"></param>
        /// <param name="client"></param>
        public ReadController(IHistorianServices<string> historian, IHistoricAccessServices<string> client) {
            _client = client ?? throw new ArgumentNullException(nameof(client));
            _historian = historian ?? throw new ArgumentNullException(nameof(historian));
        }

        /// <summary>
        /// Read historic events
        /// </summary>
        /// <remarks>
        /// Read historic events of a node if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The historic events</returns>
        [HttpPost("{endpointId}/events")]
        public async Task<HistoryReadResponseApiModel<HistoricEventApiModel[]>> HistoryReadEventsAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadRequestApiModel<ReadEventsDetailsApiModel> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadEventsAsync(
                endpointId, request.ToServiceModel(d => d.ToServiceModel()));
            return HistoryReadResponseApiModel<HistoricEventApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricEventApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read next batch of historic events
        /// </summary>
        /// <remarks>
        /// Read next batch of historic events of a node using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read next request</param>
        /// <returns>The historic events</returns>
        [HttpPost("{endpointId}/events/next")]
        public async Task<HistoryReadNextResponseApiModel<HistoricEventApiModel[]>> HistoryReadEventsNextAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadNextRequestApiModel request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadEventsNextAsync(
                endpointId, request.ToServiceModel());
            return HistoryReadNextResponseApiModel<HistoricEventApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricEventApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read historic processed values at specified times
        /// </summary>
        /// <remarks>
        /// Read processed history values of a node if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The historic values</returns>
        [HttpPost("{endpointId}/values")]
        public async Task<HistoryReadResponseApiModel<HistoricValueApiModel[]>> HistoryReadValuesAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadRequestApiModel<ReadValuesDetailsApiModel> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadValuesAsync(
                endpointId, request.ToServiceModel(d => d.ToServiceModel()));
            return HistoryReadResponseApiModel<HistoricValueApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricValueApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read historic values at specified times
        /// </summary>
        /// <remarks>
        /// Read historic values of a node if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The historic values</returns>
        [HttpPost("{endpointId}/values/pick")]
        public async Task<HistoryReadResponseApiModel<HistoricValueApiModel[]>> HistoryReadValuesAtTimesAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadRequestApiModel<ReadValuesAtTimesDetailsApiModel> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadValuesAtTimesAsync(
                endpointId, request.ToServiceModel(d => d.ToServiceModel()));
            return HistoryReadResponseApiModel<HistoricValueApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricValueApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read historic processed values at specified times
        /// </summary>
        /// <remarks>
        /// Read processed history values of a node if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The historic values</returns>
        [HttpPost("{endpointId}/values/processed")]
        public async Task<HistoryReadResponseApiModel<HistoricValueApiModel[]>> HistoryReadProcessedValuesAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadRequestApiModel<ReadProcessedValuesDetailsApiModel> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadProcessedValuesAsync(
                endpointId, request.ToServiceModel(d => d.ToServiceModel()));
            return HistoryReadResponseApiModel<HistoricValueApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricValueApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read historic modified values at specified times
        /// </summary>
        /// <remarks>
        /// Read processed history values of a node if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The historic values</returns>
        [HttpPost("{endpointId}/values/modified")]
        public async Task<HistoryReadResponseApiModel<HistoricValueApiModel[]>> HistoryReadModifiedValuesAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadRequestApiModel<ReadModifiedValuesDetailsApiModel> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadModifiedValuesAsync(
                endpointId, request.ToServiceModel(d => d.ToServiceModel()));
            return HistoryReadResponseApiModel<HistoricValueApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricValueApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read next batch of historic values
        /// </summary>
        /// <remarks>
        /// Read next batch of historic values of a node using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read next request</param>
        /// <returns>The historic values</returns>
        [HttpPost("{endpointId}/values/next")]
        public async Task<HistoryReadNextResponseApiModel<HistoricValueApiModel[]>> HistoryReadValueNextAsync(
            string endpointId,
            [FromBody] [Required] HistoryReadNextRequestApiModel request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _historian.HistoryReadValueNextAsync(
                endpointId, request.ToServiceModel());
            return HistoryReadNextResponseApiModel<HistoricValueApiModel[]>.Create(readresult,
                d => d?.Select(v => v == null ? null : new HistoricValueApiModel(v)).ToArray());
        }

        /// <summary>
        /// Read history using json details
        /// </summary>
        /// <remarks>
        /// Read node history if available using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read request</param>
        /// <returns>The history read response</returns>
        [HttpPost("{endpointId}/raw")]
        public async Task<HistoryReadResponseApiModel<JToken>> HistoryReadRawAsync(
            string endpointId, [FromBody] [Required] HistoryReadRequestApiModel<JToken> request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _client.HistoryReadAsync(
                endpointId, request.ToServiceModel(d => d));
            return HistoryReadResponseApiModel<JToken>.Create(readresult, d => d);
        }

        /// <summary>
        /// Read next batch of history as json
        /// </summary>
        /// <remarks>
        /// Read next batch of node history values using historic access.
        /// The endpoint must be activated and connected and the module client
        /// and server must trust each other.
        /// </remarks>
        /// <param name="endpointId">The identifier of the activated endpoint.</param>
        /// <param name="request">The history read next request</param>
        /// <returns>The history read response</returns>
        [HttpPost("{endpointId}/raw/next")]
        public async Task<HistoryReadNextResponseApiModel<JToken>> HistoryReadRawNextAsync(
            string endpointId, [FromBody] [Required] HistoryReadNextRequestApiModel request) {
            if (request == null) {
                throw new ArgumentNullException(nameof(request));
            }
            var readresult = await _client.HistoryReadNextAsync(
                endpointId, request.ToServiceModel());
            return HistoryReadNextResponseApiModel<JToken>.Create(readresult, d => d);
        }

        private readonly IHistoricAccessServices<string> _client;
        private readonly IHistorianServices<string> _historian;
    }
}

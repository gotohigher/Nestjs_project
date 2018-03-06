// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcTwin.Services.Cloud {
    using Microsoft.Azure.IoTSolutions.Common.Diagnostics;
    using Microsoft.Azure.IoTSolutions.OpcTwin.Services.External;
    using Microsoft.Azure.IoTSolutions.OpcTwin.Services.Models;
    using Microsoft.Azure.IoTSolutions.OpcTwin.Services.Client;
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Validate endpoint through proxy or jobs.
    /// </summary>
    public class OpcUaCompositeValidator : IOpcUaEndpointValidator {

        /// <summary>
        /// Create endpoint validator
        /// </summary>
        /// <param name="jobs"></param>
        /// <param name="proxy"></param>
        /// <param name="logger"></param>
        public OpcUaCompositeValidator(IIoTHubJobServices jobs, IOpcUaClient proxy,
            ILogger logger) {

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // Create composite of edge micro service + proxy control
            _edge = new OpcUaTwinValidator(jobs, logger);
            _proxy = new OpcUaEndpointValidator(proxy, logger);
        }

        /// <summary>
        /// Validate
        /// </summary>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        public async Task<ServerEndpointModel> ValidateAsync(
            EndpointModel endpoint) {
            try {
                return await _edge.ValidateAsync(endpoint);
            }
            catch (Exception e) {
                _logger.Error("Failed validation call on all edge services. Trying proxy",
                    () => e);
            }
            return await _proxy.ValidateAsync(endpoint);
        }

        private readonly OpcUaTwinValidator _edge;
        private readonly OpcUaEndpointValidator _proxy;
        private readonly ILogger _logger;
    }
}

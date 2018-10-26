// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Services.Registry.v1.Models {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Newtonsoft.Json;
    using System.ComponentModel;
    using System.Collections.Generic;

    /// <summary>
    /// Twin activation filter model
    /// </summary>
    public class TwinActivationFilterApiModel {

        /// <summary>
        /// Default constructor
        /// </summary>
        public TwinActivationFilterApiModel() { }

        /// <summary>
        /// Create from service model
        /// </summary>
        /// <param name="model"></param>
        public TwinActivationFilterApiModel(TwinActivationFilterModel model) {
            SecurityPolicies = model.SecurityPolicies;
            SecurityPolicies = model.SecurityPolicies;
            SecurityMode = model.SecurityMode;
        }

        /// <summary>
        /// Convert back to service model
        /// </summary>
        /// <returns></returns>
        public TwinActivationFilterModel ToServiceModel() {
            return new TwinActivationFilterModel {
                TrustLists = TrustLists,
                SecurityPolicies = SecurityPolicies,
                SecurityMode = SecurityMode
            };
        }

        /// <summary>
        /// Certificate trust list identifiers to use for
        /// activation, if null, all certificates are
        /// trusted.  If empty list, no certificates are
        /// trusted which is equal to no filter.
        /// </summary>
        [JsonProperty(PropertyName = "trustLists",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public List<string> TrustLists { get; set; }

        /// <summary>
        /// Endpoint security policies to filter against.
        /// If set to null, all policies are in scope.
        /// </summary>
        [JsonProperty(PropertyName = "securityPolicies",
           NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public List<string> SecurityPolicies { get; set; }

        /// <summary>
        /// Security mode level to activate. If null,
        /// then <see cref="SecurityMode.Best"/> is assumed.
        /// </summary>
        [JsonProperty(PropertyName = "securityMode",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public SecurityMode? SecurityMode { get; set; }
    }
}


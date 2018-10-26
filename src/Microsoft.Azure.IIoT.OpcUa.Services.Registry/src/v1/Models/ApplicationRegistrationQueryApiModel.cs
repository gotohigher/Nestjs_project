// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Services.Registry.v1.Models {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Application information
    /// </summary>
    public class ApplicationRegistrationQueryApiModel {

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationRegistrationQueryApiModel() { }

        /// <summary>
        /// Create from service model
        /// </summary>
        /// <param name="model"></param>
        public ApplicationRegistrationQueryApiModel(ApplicationRegistrationQueryModel model) {
            ApplicationType = model.ApplicationType;
            ApplicationUri = model.ApplicationUri;
            ProductUri = model.ProductUri;
            ApplicationName = model.ApplicationName;
            Locale = model.Locale;
            Capabilities = model.Capabilities;
            SiteOrSupervisorId = model.SiteOrSupervisorId;
            IncludeNotSeenSince = model.IncludeNotSeenSince;
        }

        /// <summary>
        /// Convert back to service model
        /// </summary>
        /// <returns></returns>
        public ApplicationRegistrationQueryModel ToServiceModel() {
            return new ApplicationRegistrationQueryModel {
                ApplicationType = ApplicationType,
                ApplicationUri = ApplicationUri,
                ProductUri = ProductUri,
                ApplicationName = ApplicationName,
                Locale = Locale,
                Capabilities = Capabilities,
                SiteOrSupervisorId = SiteOrSupervisorId,
                IncludeNotSeenSince = IncludeNotSeenSince
            };
        }

        /// <summary>
        /// Type of application
        /// </summary>
        [JsonProperty(PropertyName = "applicationType",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public ApplicationType? ApplicationType { get; set; }

        /// <summary>
        /// Application uri
        /// </summary>
        [JsonProperty(PropertyName = "applicationUri",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string ApplicationUri { get; set; }

        /// <summary>
        /// Product uri
        /// </summary>
        [JsonProperty(PropertyName = "productUri",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string ProductUri { get; set; }

        /// <summary>
        /// Name of application
        /// </summary>
        [JsonProperty(PropertyName = "applicationName",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Locale of application name - default is "en"
        /// </summary>
        [JsonProperty(PropertyName = "locale",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string Locale { get; set; }

        /// <summary>
        /// Application capabilities
        /// </summary>
        [JsonProperty(PropertyName = "capabilities",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public HashSet<string> Capabilities { get; set; }

        /// <summary>
        /// Supervisor or site the application belongs to.
        /// </summary>
        [JsonProperty(PropertyName = "siteOrSupervisorId",
           NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string SiteOrSupervisorId { get; set; }

        /// <summary>
        /// Whether to include apps that were soft deleted
        /// </summary>
        [JsonProperty(PropertyName = "includeNotSeenSince",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public bool? IncludeNotSeenSince { get; set; }
    }
}


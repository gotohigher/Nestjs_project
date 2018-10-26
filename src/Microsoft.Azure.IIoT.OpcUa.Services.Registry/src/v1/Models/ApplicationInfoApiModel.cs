// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Services.Registry.v1.Models {
    using Microsoft.Azure.IIoT.OpcUa.Registry.Models;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;

    /// <summary>
    /// Application model
    /// </summary>
    public class ApplicationInfoApiModel {

        /// <summary>
        /// Default constructor
        /// </summary>
        public ApplicationInfoApiModel() { }

        /// <summary>
        /// Create model from service model
        /// </summary>
        /// <param name="model"></param>
        public ApplicationInfoApiModel(ApplicationInfoModel model) {
            ApplicationId = model.ApplicationId;
            ApplicationType = model.ApplicationType;
            ApplicationUri = model.ApplicationUri;
            ApplicationName = model.ApplicationName;
            Locale = model.Locale;
            Certificate = model.Certificate;
            ProductUri = model.ProductUri;
            NotSeenSince = model.NotSeenSince;
            SiteId = model.SiteId;
            HostAddresses = model.HostAddresses;
            SupervisorId = model.SupervisorId;
            DiscoveryProfileUri = model.DiscoveryProfileUri;
            DiscoveryUrls = model.DiscoveryUrls;
            Capabilities = model.Capabilities;
        }

        /// <summary>
        /// Create service model from model
        /// </summary>
        public ApplicationInfoModel ToServiceModel() {
            return new ApplicationInfoModel {
                ApplicationId = ApplicationId,
                ApplicationType = ApplicationType,
                ApplicationUri = ApplicationUri,
                ApplicationName = ApplicationName,
                Locale = Locale,
                Certificate = Certificate,
                ProductUri = ProductUri,
                NotSeenSince = NotSeenSince,
                SiteId = SiteId,
                HostAddresses = HostAddresses,
                SupervisorId = SupervisorId,
                DiscoveryProfileUri = DiscoveryProfileUri,
                DiscoveryUrls = DiscoveryUrls,
                Capabilities = Capabilities
            };
        }

        /// <summary>
        /// Unique application id
        /// </summary>
        [JsonProperty(PropertyName = "applicationId")]
        public string ApplicationId { get; set; }

        /// <summary>
        /// Type of application
        /// </summary>
        [JsonProperty(PropertyName = "applicationType")]
        public ApplicationType ApplicationType { get; set; }

        /// <summary>
        /// Unique application uri
        /// </summary>
        [JsonProperty(PropertyName = "applicationUri")]
        public string ApplicationUri { get; set; }

        /// <summary>
        /// Product uri
        /// </summary>
        [JsonProperty(PropertyName = "productUri",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string ProductUri { get; set; }

        /// <summary>
        /// Name of server
        /// </summary>
        [JsonProperty(PropertyName = "applicationName",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string ApplicationName { get; set; }

        /// <summary>
        /// Locale of name - defaults to "en"
        /// </summary>
        [JsonProperty(PropertyName = "locale",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string Locale { get; set; }

        /// <summary>
        /// Application public cert
        /// </summary>
        [JsonProperty(PropertyName = "certificate",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public byte[] Certificate { get; set; }

        /// <summary>
        /// Capabilities
        /// </summary>
        [JsonProperty(PropertyName = "capabilities",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public HashSet<string> Capabilities { get; set; }

        /// <summary>
        /// Discovery urls of the server
        /// </summary>
        [JsonProperty(PropertyName = "discoveryUrls",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public HashSet<string> DiscoveryUrls { get; set; }

        /// <summary>
        /// Discovery profile uri
        /// </summary>
        [JsonProperty(PropertyName = "discoveryProfileUri",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string DiscoveryProfileUri { get; set; }

        /// <summary>
        /// Host addresses of server application or null
        /// </summary>
        [JsonProperty(PropertyName = "hostAddresses",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public HashSet<string> HostAddresses { get; set; }

        /// <summary>
        /// Site of the application
        /// </summary>
        [JsonProperty(PropertyName = "siteId",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string SiteId { get; set; }

        /// <summary>
        /// Supervisor having registered the application
        /// </summary>
        [JsonProperty(PropertyName = "supervisorId",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public string SupervisorId { get; set; }

        /// <summary>
        /// Last time application was seen
        /// </summary>
        [JsonProperty(PropertyName = "notSeenSince",
            NullValueHandling = NullValueHandling.Ignore)]
        [DefaultValue(null)]
        public DateTime? NotSeenSince { get; set; }
    }
}

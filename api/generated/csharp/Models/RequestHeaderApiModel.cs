// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.Opc.History.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Request header model
    /// </summary>
    public partial class RequestHeaderApiModel
    {
        /// <summary>
        /// Initializes a new instance of the RequestHeaderApiModel class.
        /// </summary>
        public RequestHeaderApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the RequestHeaderApiModel class.
        /// </summary>
        /// <param name="locales">Optional list of locales in preference
        /// order.</param>
        public RequestHeaderApiModel(CredentialApiModel elevation = default(CredentialApiModel), IList<string> locales = default(IList<string>), DiagnosticsApiModel diagnostics = default(DiagnosticsApiModel))
        {
            Elevation = elevation;
            Locales = locales;
            Diagnostics = diagnostics;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "elevation")]
        public CredentialApiModel Elevation { get; set; }

        /// <summary>
        /// Gets or sets optional list of locales in preference order.
        /// </summary>
        [JsonProperty(PropertyName = "locales")]
        public IList<string> Locales { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "diagnostics")]
        public DiagnosticsApiModel Diagnostics { get; set; }

    }
}

// <auto-generated>
// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See License.txt in the project root for
// license information.
//
// Code generated by Microsoft (R) AutoRest Code Generator 1.0.0.0
// Changes may cause incorrect behavior and will be lost if the code is
// regenerated.
// </auto-generated>

namespace Microsoft.Azure.IIoT.OpcUa.Api.Vault.Models
{
    using Newtonsoft.Json;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    public partial class QueryApplicationsResponseApiModel
    {
        /// <summary>
        /// Initializes a new instance of the QueryApplicationsResponseApiModel
        /// class.
        /// </summary>
        public QueryApplicationsResponseApiModel()
        {
            CustomInit();
        }

        /// <summary>
        /// Initializes a new instance of the QueryApplicationsResponseApiModel
        /// class.
        /// </summary>
        public QueryApplicationsResponseApiModel(System.DateTime lastCounterResetTime, int nextRecordId, IList<ApplicationRecordApiModel> applications = default(IList<ApplicationRecordApiModel>))
        {
            Applications = applications;
            LastCounterResetTime = lastCounterResetTime;
            NextRecordId = nextRecordId;
            CustomInit();
        }

        /// <summary>
        /// An initialization method that performs custom operations like setting defaults
        /// </summary>
        partial void CustomInit();

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "applications")]
        public IList<ApplicationRecordApiModel> Applications { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "lastCounterResetTime")]
        public System.DateTime LastCounterResetTime { get; set; }

        /// <summary>
        /// </summary>
        [JsonProperty(PropertyName = "nextRecordId")]
        public int NextRecordId { get; set; }

        /// <summary>
        /// Validate the object.
        /// </summary>
        /// <exception cref="Rest.ValidationException">
        /// Thrown if validation fails
        /// </exception>
        public virtual void Validate()
        {
        }
    }
}

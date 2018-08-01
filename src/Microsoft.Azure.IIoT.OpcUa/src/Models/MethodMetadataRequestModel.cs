// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Models {

    /// <summary>
    /// Requests method metadata
    /// </summary>
    public class MethodMetadataRequestModel {

        /// <summary>
        /// Method id to request meta data for
        /// </summary>
        public string MethodId { get; set; }

        /// <summary>
        /// Elevation
        /// </summary>
        public AuthenticationModel Elevation { get; set; }
    }
}

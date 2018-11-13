// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Twin.Models {

    /// <summary>
    /// Attribute to read
    /// </summary>
    public class AttributeReadRequestModel {

        /// <summary>
        /// Node to read from or write to (mandatory)
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// Attribute to read or write
        /// </summary>
        public NodeAttribute Attribute { get; set; }
    }
}

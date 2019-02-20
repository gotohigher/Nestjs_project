// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.OpcUa.Twin.Models {
    /// <summary>
    /// Browse nodes by path
    /// </summary>
    public class BrowsePathRequestModel {

        /// <summary>
        /// Node to browse.
        /// (default: RootFolder).
        /// </summary>
        public string NodeId { get; set; }

        /// <summary>
        /// The path elements of the path to browse from node.
        /// (mandatory)
        /// </summary>
        public string[] PathElements { get; set; }

        /// <summary>
        /// Whether to read variable values on target nodes.
        /// (default is false)
        /// </summary>
        public bool? ReadVariableValues { get; set; }

        /// <summary>
        /// Whether to only return the raw node id
        /// information and not read the target node.
        /// (default is false)
        /// </summary>
        public bool? NodeIdsOnly { get; set; }

        /// <summary>
        /// Optional header
        /// </summary>
        public RequestHeaderModel Header { get; set; }
    }
}

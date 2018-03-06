﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcTwin.Services {
    using Microsoft.Azure.IoTSolutions.OpcTwin.Services.Models;
    using System.Threading.Tasks;

    /// <summary>
    /// Supervisor registry
    /// </summary>
    public interface IOpcUaSupervisorRegistry {

        /// <summary>
        /// Get all supervisors in paged form
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        Task<SupervisorListModel> ListSupervisorsAsync(
            string continuation);

        /// <summary>
        /// Get supervisor registration by identifer.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<SupervisorModel> GetSupervisorAsync(
            string id);

        /// <summary>
        /// Update supervisor, e.g. set discovery mode
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateSupervisorAsync(
            SupervisorUpdateModel request);
    }
}
﻿// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IoTSolutions.OpcTwin.EdgeService {
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// Supervisor service
    /// </summary>
    public interface IOpcUaSupervisorServices {

        /// <summary>
        /// Start new twin with given connection string
        /// </summary>
        /// <param name="secret"></param>
        /// <returns></returns>
        Task StartTwinAsync(string id, string secret);

        /// <summary>
        /// Stop twin and dispose...
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task StopTwinAsync(string id);
    }
}
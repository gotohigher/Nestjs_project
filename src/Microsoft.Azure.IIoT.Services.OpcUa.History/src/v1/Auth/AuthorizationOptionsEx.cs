// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Services.OpcUa.History.v1 {
    using Microsoft.Azure.IIoT.Services.OpcUa.History.v1.Auth;
    using Microsoft.Azure.IIoT.Services.Auth;
    using Microsoft.AspNetCore.Authorization;

    /// <summary>
    /// AuthorizationOptions extension
    /// </summary>
    public static class AuthorizationOptionsEx {

        /// <summary>
        /// Add v1 policies to options
        /// </summary>
        /// <param name="options"></param>
        /// <param name="withAuthorization"></param>
        /// <param name="useRoleBasedAccess"></param>
        public static void AddV1Policies(this AuthorizationOptions options,
            bool withAuthorization, bool useRoleBasedAccess) {

            if (!withAuthorization) {
                options.AddNoOpPolicies(Policies.All());
                return;
            }

            options.AddPolicy(Policies.CanRead, policy =>
                policy.RequireAuthenticatedUser());

            if (!useRoleBasedAccess) {
                options.AddPolicy(Policies.CanUpdate, policy =>
                    policy.RequireAuthenticatedUser());
            }
            else {
                options.AddPolicy(Policies.CanUpdate, policy =>
                    policy.RequireAuthenticatedUser()
                        .Require(ControlRights));
            }
        }

        /// <summary>
        /// Admin has admin role or execute and can do all system functions
        /// </summary>
        public static bool AdminRights(AuthorizationHandlerContext context) {
            return
                context.User.IsInRole(Roles.Admin) ||
                context.User.HasClaim(c => c.Type == Claims.Execute);
        }

        /// <summary>
        /// Control has Sign, Admin or Writer role, or has execute claim
        /// </summary>
        public static bool ControlRights(AuthorizationHandlerContext context) {
            return
                context.User.IsInRole(Roles.Write) ||
                context.User.IsInRole(Roles.Admin) ||
                context.User.IsInRole(Roles.Sign) ||
                context.User.HasClaim(c => c.Type == Claims.Execute);
        }
    }
}

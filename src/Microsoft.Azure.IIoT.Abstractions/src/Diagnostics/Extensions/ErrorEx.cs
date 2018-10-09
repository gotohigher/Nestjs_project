// ------------------------------------------------------------
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//  Licensed under the MIT License (MIT). See License.txt in the repo root for license information.
// ------------------------------------------------------------

namespace Microsoft.Azure.IIoT.Diagnostics {
    using System;
    using System.Diagnostics.Contracts;
    using System.Runtime.CompilerServices;

    /// <summary>
    /// Error logging extensions for logger interface
    /// </summary>
    public static class ErrorEx {

        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger, string message,
            Exception exception = null,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                exception, message);
        }

        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        /// <param name="arg1"></param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger, string message,
            object arg1,
            Exception exception,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                exception, message, arg1);
        }

        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger, string message,
            object arg1, object arg2,
            Exception exception,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                exception, message, arg1, arg2);
        }

        /// <summary>
        /// Write error message
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="exception">exception</param>
        /// <param name="arg1"></param>
        /// <param name="arg2"></param>
        /// <param name="arg3"></param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger, string message,
            object arg1, object arg2, object arg3,
            Exception exception,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                exception, message, arg1, arg2, arg3);
        }

        /// <summary>
        /// Write error message for backcompat to pcs
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="context">function context</param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger,
            string message, Action context,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            Contract.Requires(context != null);
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                null, message);
        }

        /// <summary>
        /// Write error message for backcompat to pcs
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="context">function context</param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger,
            string message, Func<object> context,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            Contract.Requires(context != null);
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                null, message, context());
        }

        /// <summary>
        /// Write error message for backcompat to pcs
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="message">message</param>
        /// <param name="exception">function context</param>
        /// <param name="method"></param>
        /// <param name="fileName"></param>
        /// <param name="lineNumber"></param>
        public static void Error(this ILogger logger,
            string message, Func<Exception> exception,
            [CallerMemberName] string method = null,
            [CallerFilePath] string fileName = null,
            [CallerLineNumber] int lineNumber = 0) {
            logger.Log(method, fileName, lineNumber, LogLevel.Error,
                exception(), message);
        }
    }
}

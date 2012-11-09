using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Refraxion
{
    /// <summary>
    /// The interface for logging messages, and a conduit for use in a task
    /// </summary>
    public interface ILog
    {
        /// <summary>
        /// Logs the normal.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void LogNormal(string format, params object[] parameters);
        /// <summary>
        /// Logs the verbose.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void LogVerbose(string format, params object[] parameters);
        /// <summary>
        /// Logs the warning.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void LogWarning(string format, params object[] parameters);
        /// <summary>
        /// Logs the error.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The parameters.</param>
        void LogError(string format, params object[] parameters);
        /// <summary>
        /// Logs the exception.
        /// </summary>
        /// <param name="x">The x.</param>
        void LogException(Exception x);
    }
}

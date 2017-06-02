using System;
#if !NO_ASYNC
using System.Threading.Tasks;
#endif

namespace Emzi0767
{
    /// <summary>
    /// Represents a base class from which all log receivers derive.
    /// </summary>
    public abstract class BaseLogReceiver : IDisposable
    {
        /// <summary>
        /// Gets or sets this logger's settings.
        /// </summary>
        public LoggerSettings Settings { get; internal set; }

        /// <summary>
        /// Synchronously logs a supplied line to the output.
        /// </summary>
        /// <param name="timestamp">Line's timestamp.</param>
        /// <param name="level">Event's severity.</param>
        /// <param name="tag">Line's tag.</param>
        /// <param name="line">The line to log.</param>
        public abstract void LogLine(DateTime timestamp, LogLevel level, string tag, string line);

#if !NO_ASYNC
        /// <summary>
        /// Asynchronously logs a supplied line to the output.
        /// </summary>
        /// <param name="timestamp">Line's timestamp.</param>
        /// <param name="level">Event's severity.</param>
        /// <param name="tag">Line's tag.</param>
        /// <param name="line">The line to log.</param>
        /// <returns>Asynchronous operation.</returns>
        public abstract Task LogLineAsync(DateTime timestamp, LogLevel level, string tag, string line);
#endif

        /// <summary>
        /// Disposes this log receiver.
        /// </summary>
        public abstract void Dispose();
    }
}

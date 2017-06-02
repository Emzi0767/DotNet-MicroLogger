using System;
using System.IO;
#if !NO_ASYNC
using System.Threading.Tasks;
#endif

namespace Emzi0767.LogReceivers
{
    /// <summary>
    /// Represents a log receiver that logs to a text writer.
    /// </summary>
    public class TextWriterLogReceiver : BaseLogReceiver
    {
        private TextWriter OutputTextWriter { get; }
        private bool _disposed = false;

        /// <summary>
        /// Creates a new log receiver that logs to a text writer.
        /// </summary>
        /// <param name="output_writer">Output <see cref="TextWriter"/>.</param>
        public TextWriterLogReceiver(TextWriter output_writer)
        {
            this.OutputTextWriter = output_writer;
        }

        /// <summary>
        /// Synchronously logs a supplied line to the output.
        /// </summary>
        /// <param name="timestamp">Line's timestamp.</param>
        /// <param name="level">Event's severity.</param>
        /// <param name="tag">Line's tag.</param>
        /// <param name="line">The line to log.</param>
        public override void LogLine(DateTime timestamp, LogLevel level, string tag, string line)
        {
            var str = LogHelperMethods.FormatLine(this.Settings, timestamp, level, tag, line);
            this.OutputTextWriter.WriteLine(str);
        }

#if !NO_ASYNC
        /// <summary>
        /// Asynchronously logs a supplied line to the output.
        /// </summary>
        /// <param name="timestamp">Line's timestamp.</param>
        /// <param name="level">Event's severity.</param>
        /// <param name="tag">Line's tag.</param>
        /// <param name="line">The line to log.</param>
        /// <returns>Asynchronous operation.</returns>
        public override async Task LogLineAsync(DateTime timestamp, LogLevel level, string tag, string line)
        {
            var str = LogHelperMethods.FormatLine(this.Settings, timestamp, level, tag, line);
            await this.OutputTextWriter.WriteLineAsync(str);
        }
#endif

        /// <summary>
        /// Disposes this log receiver.
        /// </summary>
        public override void Dispose()
        {
            if (this._disposed)
                return;

            this._disposed = true;

            this.OutputTextWriter.Dispose();
        }
    }
}
using System;
using System.IO;
using System.Text;
#if !NO_ASYNC
using System.Threading.Tasks;
#endif

namespace Emzi0767.LogReceivers
{
    /// <summary>
    /// Represents a log receiver that logs to a stream.
    /// </summary>
    public class StreamLogReceiver : BaseLogReceiver
    {
        private Stream OutputStream { get; }
        private StreamWriter OutputStreamWriter { get; }
        private Encoding OutputEncoding { get; }
        private bool _disposed = false;

        /// <summary>
        /// Creates a new log receiver that logs to a stream.
        /// </summary>
        /// <param name="output_stream">Output stream.</param>
        public StreamLogReceiver(Stream output_stream)
            : this(output_stream, new UTF8Encoding(false))
        { }

        /// <summary>
        /// Creates a new log receiver that logs to a stream.
        /// </summary>
        /// <param name="output_stream">Output stream.</param>
        /// <param name="encoding">Output encoding.</param>
        public StreamLogReceiver(Stream output_stream, Encoding encoding)
        {
            this.OutputStream = output_stream;
            this.OutputEncoding = encoding;
            this.OutputStreamWriter = new StreamWriter(this.OutputStream, this.OutputEncoding);
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
            this.OutputStreamWriter.WriteLine(str);
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
            await this.OutputStreamWriter.WriteLineAsync(str);
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

            this.OutputStreamWriter.Dispose();
            this.OutputStream.Dispose();
        }
    }
}
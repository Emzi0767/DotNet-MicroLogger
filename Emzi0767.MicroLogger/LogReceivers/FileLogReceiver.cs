#if !(PORTABLE || NETSTANDARD1_0)
using System;
using System.IO;
using System.Text;
#if !NO_ASYNC
using System.Threading.Tasks;
#endif

namespace Emzi0767.LogReceivers
{
    /// <summary>
    /// Represents a log receiver that logs to a file.
    /// </summary>
    public class FileLogReceiver : BaseLogReceiver
    {
        private FileStream OutputFile { get; }
        private StreamWriter OutputFileWriter { get; }
        private Encoding FileEncoding { get; }
        private bool _disposed = false;

        /// <summary>
        /// Creates a new log receiver that logs to a file.
        /// </summary>
        /// <param name="filename">Output file name.</param>
        public FileLogReceiver(string filename)
            : this(File.OpenWrite(filename), new UTF8Encoding(false))
        { }

        /// <summary>
        /// Creates a new log receiver that logs to a file.
        /// </summary>
        /// <param name="output_stream">Output file stream.</param>
        public FileLogReceiver(FileStream output_stream)
            : this(output_stream, new UTF8Encoding(false))
        { }

        /// <summary>
        /// Creates a new log receiver that logs to a file.
        /// </summary>
        /// <param name="filename">Output file name.</param>
        /// <param name="encoding">Output encoding.</param>
        public FileLogReceiver(string filename, Encoding encoding)
            : this(File.OpenWrite(filename), encoding)
        { }

        /// <summary>
        /// Creates a new log receiver that logs to a file.
        /// </summary>
        /// <param name="output_stream">Output file stream.</param>
        /// <param name="encoding">Output encoding.</param>
        public FileLogReceiver(FileStream output_stream, Encoding encoding)
        {
            this.OutputFile = output_stream;
            this.OutputFile.Seek(0, SeekOrigin.End);
            this.FileEncoding = encoding;
            this.OutputFileWriter = new StreamWriter(this.OutputFile, this.FileEncoding);
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
            this.OutputFileWriter.WriteLine(str);
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
            await this.OutputFileWriter.WriteLineAsync(str);
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

            this.OutputFileWriter.Dispose();
            this.OutputFile.Dispose();
        }
    }
}
#endif
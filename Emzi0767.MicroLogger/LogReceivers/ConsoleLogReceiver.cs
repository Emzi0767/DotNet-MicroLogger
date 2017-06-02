#if !(PORTABLE || NETSTANDARD1_0)
using System;
#if !NO_ASYNC
using System.Threading.Tasks;
#endif

namespace Emzi0767.LogReceivers
{
    /// <summary>
    /// Represents a log receiver that logs to a file.
    /// </summary>
    public class ConsoleLogReceiver : BaseLogReceiver
    {
        private bool _disposed = false;

        /// <summary>
        /// Creates a new log receiver that logs to console.
        /// </summary>
        /// <param name="filename">Output file name.</param>
        public ConsoleLogReceiver()
        { }

        /// <summary>
        /// Synchronously logs a supplied line to the output.
        /// </summary>
        /// <param name="timestamp">Line's timestamp.</param>
        /// <param name="level">Event's severity.</param>
        /// <param name="tag">Line's tag.</param>
        /// <param name="line">The line to log.</param>
        public override void LogLine(DateTime timestamp, LogLevel level, string tag, string line)
        {
            if (level == LogLevel.Critical)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;

                var str = LogHelperMethods.FormatLine(this.Settings, timestamp, level, tag, line);
                Console.WriteLine(str);

                Console.ResetColor();
                return;
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(string.Concat("[{0:", this.Settings.DateTimeFormat, "}] "), timestamp);

            switch (level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write("[{0}] ", LogHelperMethods.FixedWidth(level.ToString(), 8, this.Settings.PaddingCharacter));

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[{0}] ", LogHelperMethods.FixedWidth(tag, this.Settings.TagLength, this.Settings.PaddingCharacter));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);

            Console.ResetColor();
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
        public override Task LogLineAsync(DateTime timestamp, LogLevel level, string tag, string line)
        {
            if (level == LogLevel.Critical)
            {
                Console.BackgroundColor = ConsoleColor.Red;
                Console.ForegroundColor = ConsoleColor.Black;

                var str = LogHelperMethods.FormatLine(this.Settings, timestamp, level, tag, line);
                Console.WriteLine(str);

                Console.ResetColor();

                return Task.Delay(0);
            }

            Console.BackgroundColor = ConsoleColor.Black;
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.Write(string.Concat("[{0:", this.Settings.DateTimeFormat, "}] "), timestamp);

            switch (level)
            {
                case LogLevel.Error:
                    Console.ForegroundColor = ConsoleColor.Red;
                    break;

                case LogLevel.Warning:
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    break;

                case LogLevel.Info:
                    Console.ForegroundColor = ConsoleColor.Cyan;
                    break;

                case LogLevel.Debug:
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    break;

                case LogLevel.Verbose:
                    Console.ForegroundColor = ConsoleColor.Green;
                    break;

                default:
                    Console.ForegroundColor = ConsoleColor.White;
                    break;
            }
            Console.Write("[{0}] ", LogHelperMethods.FixedWidth(level.ToString(), 8, this.Settings.PaddingCharacter));

            Console.ForegroundColor = ConsoleColor.Blue;
            Console.Write("[{0}] ", LogHelperMethods.FixedWidth(tag, this.Settings.TagLength, this.Settings.PaddingCharacter));

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(line);

            Console.ResetColor();

            return Task.Delay(0);
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
        }
    }
}
#endif
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
#if !NO_ASYNC
using System.Threading;
using System.Threading.Tasks;
#endif

namespace Emzi0767
{
    /// <summary>
    /// Represents a MicroLogger interface.
    /// </summary>
    public class MicroLogger : IDisposable
    {
        #region Public Properties
#if !PORTABLE
        /// <summary>
        /// Gets or sets whether to output messages to <see cref="Debug"/>.
        /// 
        /// Defaults to false.
        /// </summary>
        public bool OutputToDebug { get; set; } = false;
#endif

        /// <summary>
        /// Gets or sets the character used to pad strings.
        /// 
        /// Defaults to ' ' (space).
        /// </summary>
        public char PaddingCharacter { get; set; } = ' ';
        
        /// <summary>
        /// Gets or sets the tag used when logged message has no tag.
        /// 
        /// Defaults to "stdout".
        /// </summary>
        public string DefaultTag { get; set; } = "stdout";

        /// <summary>
        /// Gets or sets the length of message tags.
        /// 
        /// Defaults to 15.
        /// </summary>
        public int TagLength { get; set; } = 15;

        /// <summary>
        /// Gets or sets the format used to format timestamps.
        /// 
        /// Defaults to yyyy-MM-dd HH:mm:ss zzz.
        /// </summary>
        public string DateTimeFormat { get; set; } = "yyyy-MM-dd HH:mm:ss zzz";

        /// <summary>
        /// Gets or sets the maximum logging level.
        /// 
        /// Defaults to LogLevel.Error (2).
        /// </summary>
        public LogLevel LoggingLevel { get; set; } = LogLevel.Error;
        #endregion

        #region Private Properties and Fields
        private List<TextWriter> LogOutputs { get; set; }

        private bool _disposed = false;
#if NO_ASYNC
        private object _lock = new object();
#else
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
#endif

#endregion

        #region Constructor and Initialization
        /// <summary>
        /// Creates a new MicroLogger instance.
        /// </summary>
        public MicroLogger()
        {
            this.LogOutputs = new List<TextWriter>();
        }

        /// <summary>
        /// Registers a new logger output.
        /// </summary>
        /// <param name="tw"><see cref="TextWriter"/> to register as output.</param>
        public void RegisterOutput(TextWriter tw)
        {
            if (this.LogOutputs.Contains(tw))
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif
            this.LogOutputs.Add(tw);
#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
        }

        /// <summary>
        /// Unregisters an existing logger output.
        /// </summary>
        /// <param name="tw"><see cref="TextWriter"/> to unregister as output.</param>
        public void UnregisterOutput(TextWriter tw)
        {
            if (this.LogOutputs.Contains(tw))
            {
#if NO_ASYNC
                lock (_lock)
                {
#else
                this._semaphore.Wait();
#endif
                this.LogOutputs.Remove(tw);
#if NO_ASYNC
                }
#else
                this._semaphore.Release();
#endif
            }
        }
#endregion

        #region Synchronous Logging
        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTime timestamp, LogLevel level, string tag, string format, params object[] args)
        {
            var outstr = string.Format(format, args);
            var olines = this.SplitLines(outstr);
            olines = this.FormatLines(timestamp, level, tag, olines);
            
#if !PORTABLE
#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

                if (this.OutputToDebug)
                foreach (var line in olines)
                    Debug.WriteLine(line);

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
#endif

            if ((int)level > (int)this.LoggingLevel)
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

            foreach (var output in this.LogOutputs)
            {
                foreach (var line in olines)
                    output.WriteLine(line);

                output.Flush();
            }

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTimeOffset timestamp, LogLevel level, string tag, string format, params object[] args)
        {
            this.Log(timestamp.LocalDateTime, level, tag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(LogLevel level, string tag, string format, params object[] args)
        {
            this.Log(DateTime.Now, level, tag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTime timestamp, string tag, string format, params object[] args)
        {
            this.Log(timestamp, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTimeOffset timestamp, string tag, string format, params object[] args)
        {
            this.Log(timestamp.LocalDateTime, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTime timestamp, LogLevel level, string format, params object[] args)
        {
            this.Log(timestamp, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTimeOffset timestamp, LogLevel level, string format, params object[] args)
        {
            this.Log(timestamp.LocalDateTime, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(string tag, string format, params object[] args)
        {
            this.Log(DateTime.Now, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(LogLevel level, string format, params object[] args)
        {
            this.Log(DateTime.Now, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTime timestamp, string format, params object[] args)
        {
            this.Log(timestamp, LogLevel.Info, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTimeOffset timestamp, string format, params object[] args)
        {
            this.Log(timestamp.LocalDateTime, LogLevel.Info, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(string format, params object[] args)
        {
            this.Log(DateTime.Now, LogLevel.Info, this.DefaultTag, format, args);
        }
        #endregion

#if !NO_ASYNC
        #region Asynchronous Logging
        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public async Task LogAsync(DateTime timestamp, LogLevel level, string tag, string format, params object[] args)
        {
            var outstr = string.Format(format, args);
            var olines = this.SplitLines(outstr);
            olines = this.FormatLines(timestamp, level, tag, olines);
            
#if !PORTABLE
#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

                if (this.OutputToDebug)
                foreach (var line in olines)
                    Debug.WriteLine(line);

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
#endif

            if ((int)level > (int)this.LoggingLevel)
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            await this._semaphore.WaitAsync();
#endif

            foreach (var output in this.LogOutputs)
            {
                foreach (var line in olines)
                    await output.WriteLineAsync(line);

                await output.FlushAsync();
            }

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTimeOffset timestamp, LogLevel level, string tag, string format, params object[] args)
        {
            return this.LogAsync(timestamp.LocalDateTime, level, tag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="level">Log event's level.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(LogLevel level, string tag, string format, params object[] args)
        {
            return this.LogAsync(DateTime.Now, level, tag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTime timestamp, string tag, string format, params object[] args)
        {
            return this.LogAsync(timestamp, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTimeOffset timestamp, string tag, string format, params object[] args)
        {
            return this.LogAsync(timestamp.LocalDateTime, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTime timestamp, LogLevel level, string format, params object[] args)
        {
            return this.LogAsync(timestamp, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTimeOffset timestamp, LogLevel level, string format, params object[] args)
        {
            return this.LogAsync(timestamp.LocalDateTime, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="tag">Log message's tag.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(string tag, string format, params object[] args)
        {
            return this.LogAsync(DateTime.Now, LogLevel.Info, tag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="level">Log event's level.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(LogLevel level, string format, params object[] args)
        {
            return this.LogAsync(DateTime.Now, level, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTime timestamp, string format, params object[] args)
        {
            return this.LogAsync(timestamp, LogLevel.Info, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTimeOffset timestamp, string format, params object[] args)
        {
            return this.LogAsync(timestamp.LocalDateTime, LogLevel.Info, this.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(string format, params object[] args)
        {
            return this.LogAsync(DateTime.Now, LogLevel.Info, this.DefaultTag, format, args);
        }
        #endregion
#endif

        #region Disposing and Deinitialization
        /// <summary>
        /// Disposes this logger instance.
        /// </summary>
        public void Dispose()
            {
                if (this._disposed)
                    throw new ObjectDisposedException("This logger instance is already disposed.");

                this._disposed = true;

                foreach (var output in this.LogOutputs)
                    output.Dispose();
            }
    #endregion

        #region Helper Methods
        /// <summary>
        /// Formats a string to make it fixed width.
        /// </summary>
        /// <param name="input">Input string to format.</param>
        /// <param name="width">Width of the output string.</param>
        /// <returns>String of fixed width.</returns>
        private string FixedWidth(string input, int width)
        {
            if (input.Length == width)
                return input;

            if (input.Length > width)
                return input.Substring(0, width);

            return input.PadRight(width, this.PaddingCharacter);
        }

        /// <summary>
        /// Splits input string by newline characters.
        /// </summary>
        /// <param name="input">Input string to split.</param>
        /// <returns>Enumerator which yields split strings.</returns>
        private IEnumerable<string> SplitLines(string input)
        {
            var lastlf = false;
            var sp = -1;
            var ep = -1;
            for (var i = 0; i < input.Length; i++)
            {
                if (sp == -1)
                    sp = i;

                if (input[i] == '\n' || input[i] == '\r')
                {
                    if (lastlf && ep != -1)
                        continue;
                    ep = i;
                    lastlf = true;
                }

                if (sp != -1 && ep != -1 && sp < ep)
                {
                    yield return input.Substring(sp, ep - sp);
                    ep = -1;
                    sp = -1;
                }
            }

            if (sp != -1)
                yield return input.Substring(sp);
        }

        /// <summary>
        /// Formats the lines for outputting them to log outputs.
        /// </summary>
        /// <param name="timestamp">Timestamp to put on the lines.</param>
        /// <param name="level">Level to put on the lines.</param>
        /// <param name="tag">Tag to put on the lines.</param>
        /// <param name="input">Input lines.</param>
        /// <returns>Formatted lines.</returns>
        private IEnumerable<string> FormatLines(DateTime timestamp, LogLevel level, string tag, IEnumerable<string> input)
        {
            var fstring = string.Concat("[{0:", this.DateTimeFormat, "}] [{1}] [{2}] ");
            fstring = string.Format(fstring, timestamp, this.FixedWidth(tag, this.TagLength), this.FixedWidth(level.ToString(), 8));

            foreach (var line in input)
                yield return string.Concat(fstring, line);
        }
        #endregion
    }
}

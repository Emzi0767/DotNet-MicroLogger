using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        /// <summary>
        /// Gets or sets the settings for this logger instance.
        /// </summary>
        public LoggerSettings Settings { get; set; }
        #endregion

        #region Private Properties and Fields
        private List<BaseLogReceiver> LogOutputs { get; set; }

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
        public MicroLogger(LoggerSettings settings)
        {
            this.Settings = settings;
            this.LogOutputs = new List<BaseLogReceiver>();
        }

        /// <summary>
        /// Registers a new logger output.
        /// </summary>
        /// <param name="tw"><see cref="BaseLogReceiver"/> to register as output.</param>
        public void RegisterOutput(BaseLogReceiver tw)
        {
            if (this.LogOutputs.Contains(tw))
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif
            tw.Settings = this.Settings;
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
        /// <param name="tw"><see cref="BaseLogReceiver"/> to unregister as output.</param>
        public void UnregisterOutput(BaseLogReceiver tw)
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
            var olines = LogHelperMethods.SplitLines(outstr);
            
#if !PORTABLE
#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

                if (this.Settings.OutputToDebug)
                foreach (var line in olines)
                    Debug.WriteLine(line);

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
#endif

            if ((int)level > (int)this.Settings.LoggingLevel)
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

            foreach (var output in this.LogOutputs)
                foreach (var line in olines)
                    output.LogLine(timestamp, level, tag, line);

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
            this.Log(timestamp, level, this.Settings.DefaultTag, format, args);
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
            this.Log(timestamp.LocalDateTime, level, this.Settings.DefaultTag, format, args);
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
            this.Log(DateTime.Now, level, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTime timestamp, string format, params object[] args)
        {
            this.Log(timestamp, LogLevel.Info, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(DateTimeOffset timestamp, string format, params object[] args)
        {
            this.Log(timestamp.LocalDateTime, LogLevel.Info, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Logs a message to all registered log outputs.
        /// </summary>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public void Log(string format, params object[] args)
        {
            this.Log(DateTime.Now, LogLevel.Info, this.Settings.DefaultTag, format, args);
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
            var olines = LogHelperMethods.SplitLines(outstr);
            
#if !PORTABLE
#if NO_ASYNC
            lock (_lock)
            {
#else
            this._semaphore.Wait();
#endif

                if (this.Settings.OutputToDebug)
                foreach (var line in olines)
                    Debug.WriteLine(line);

#if NO_ASYNC
            }
#else
            this._semaphore.Release();
#endif
#endif

            if ((int)level > (int)this.Settings.LoggingLevel)
                return;

#if NO_ASYNC
            lock (_lock)
            {
#else
            await this._semaphore.WaitAsync();
#endif
            
            foreach (var output in this.LogOutputs)
                foreach (var line in olines)
                    await output.LogLineAsync(timestamp, level, tag, line);

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
            return this.LogAsync(timestamp, level, this.Settings.DefaultTag, format, args);
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
            return this.LogAsync(timestamp.LocalDateTime, level, this.Settings.DefaultTag, format, args);
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
            return this.LogAsync(DateTime.Now, level, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTime timestamp, string format, params object[] args)
        {
            return this.LogAsync(timestamp, LogLevel.Info, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="timestamp">Log message's timestamp.</param>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(DateTimeOffset timestamp, string format, params object[] args)
        {
            return this.LogAsync(timestamp.LocalDateTime, LogLevel.Info, this.Settings.DefaultTag, format, args);
        }

        /// <summary>
        /// Asynchronously logs a message to all registered log outputs.
        /// </summary>
        /// <param name="format">Format of the output string.</param>
        /// <param name="args">Arguments to use for formatting the output string.</param>
        public Task LogAsync(string format, params object[] args)
        {
            return this.LogAsync(DateTime.Now, LogLevel.Info, this.Settings.DefaultTag, format, args);
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
                return;

            this._disposed = true;

                foreach (var output in this.LogOutputs)
                    output.Dispose();
        }
        #endregion
    }
}

namespace Emzi0767
{
    /// <summary>
    /// Represents MicroLogger settings.
    /// </summary>
    public sealed class LoggerSettings
    {
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
    }
}

using System;
using System.Collections.Generic;

namespace Emzi0767
{
    /// <summary>
    /// Various helper methods for MicroLogger.
    /// </summary>
    public static class LogHelperMethods
    {
        /// <summary>
        /// Formats a string to make it fixed width.
        /// </summary>
        /// <param name="input">Input string to format.</param>
        /// <param name="width">Width of the output string.</param>
        /// <returns>String of fixed width.</returns>
        public static string FixedWidth(string input, int width, char padding_char)
        {
            if (input.Length == width)
                return input;

            if (input.Length > width)
                return input.Substring(0, width);

            return input.PadRight(width, padding_char);
        }

        /// <summary>
        /// Splits input string by newline characters.
        /// </summary>
        /// <param name="input">Input string to split.</param>
        /// <returns>Enumerator which yields split strings.</returns>
        public static IEnumerable<string> SplitLines(string input)
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
        public static string FormatLine(LoggerSettings settings, DateTime timestamp, LogLevel level, string tag, string line)
        {
            var fstring = string.Concat("[{0:", settings.DateTimeFormat, "}] [{1}] [{2}] {3}");
            fstring = string.Format(fstring, timestamp, FixedWidth(tag, settings.TagLength, settings.PaddingCharacter), FixedWidth(level.ToString(), 8, settings.PaddingCharacter), line);

            return fstring;
        }
    }
}

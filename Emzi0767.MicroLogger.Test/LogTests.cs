using System;
using System.IO;
using System.Threading.Tasks;
using Emzi0767.LogReceivers;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Emzi0767.Test
{
    [TestClass]
    public class LogTests
    {
        [TestMethod]
        public void TestSync1()
        {
            var dt = DateTime.Now;
            var refstr = string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Test           ] [Info    ] This is a test. Number 42!", Environment.NewLine);

            using (var sw = new StringWriter())
            using (var lr = new TextWriterLogReceiver(sw))
            using (var logger = new MicroLogger(new LoggerSettings { LoggingLevel = LogLevel.Verbose }))
            {
                logger.RegisterOutput(lr);

                logger.Log(dt, LogLevel.Info, "Test", "This is a test. Number {0}!", 42);

                var str = sw.ToString();
                Assert.AreEqual(refstr, str);
            }
        }

        [TestMethod]
        public void TestSync2()
        {
            var dt = DateTime.Now;
            var refstrs = new[] 
            {
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Test           ] [Info    ] This is a test. Number 42!"),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [stdout         ] [Info    ] This time, with a double of 4.2."),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [stdout         ] [Verbose ] We're very verbose with this one."),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Another test   ] [Critical] Oh snap!"),
            };
            var refstr = string.Concat(string.Join(Environment.NewLine, refstrs), Environment.NewLine);

            using (var sw = new StringWriter())
            using (var lr = new TextWriterLogReceiver(sw))
            using (var logger = new MicroLogger(new LoggerSettings { LoggingLevel = LogLevel.Verbose }))
            {
                logger.RegisterOutput(lr);

                logger.Log(dt, LogLevel.Info, "Test", "This is a test. Number {0}!", 42);
                logger.Log(dt, LogLevel.Info, "This time, with a double of {0:0.0}.", 4.23);
                logger.Log(dt, LogLevel.Verbose, "We're very verbose with this one.");
                logger.Log(dt, LogLevel.Critical, "Another test", "Oh snap!");

                var str = sw.ToString();
                Assert.AreEqual(refstr, str);
            }
        }

        [TestMethod]
        public async Task TestAsync1()
        {
            var dt = DateTime.Now;
            var refstr = string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Test           ] [Info    ] This is a test. Number 42!", Environment.NewLine);

            using (var sw = new StringWriter())
            using (var lr = new TextWriterLogReceiver(sw))
            using (var logger = new MicroLogger(new LoggerSettings { LoggingLevel = LogLevel.Verbose }))
            {
                logger.RegisterOutput(lr);

                await logger.LogAsync(dt, LogLevel.Info, "Test", "This is a test. Number {0}!", 42);

                var str = sw.ToString();
                Assert.AreEqual(refstr, str);
            }
        }

        [TestMethod]
        public async Task TestAsync2()
        {
            var dt = DateTime.Now;
            var refstrs = new[]
            {
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Test           ] [Info    ] This is a test. Number 42!"),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [stdout         ] [Info    ] This time, with a double of 4.2."),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [stdout         ] [Verbose ] We're very verbose with this one."),
                string.Concat("[", dt.ToString("yyyy-MM-dd HH:mm:ss zzz"), "] [Another test   ] [Critical] Oh snap!"),
            };
            var refstr = string.Concat(string.Join(Environment.NewLine, refstrs), Environment.NewLine);

            using (var sw = new StringWriter())
            using (var lr = new TextWriterLogReceiver(sw))
            using (var logger = new MicroLogger(new LoggerSettings { LoggingLevel = LogLevel.Verbose }))
            {
                logger.RegisterOutput(lr);

                await logger.LogAsync(dt, LogLevel.Info, "Test", "This is a test. Number {0}!", 42);
                await logger.LogAsync(dt, LogLevel.Info, "This time, with a double of {0:0.0}.", 4.23);
                await logger.LogAsync(dt, LogLevel.Verbose, "We're very verbose with this one.");
                await logger.LogAsync(dt, LogLevel.Critical, "Another test", "Oh snap!");

                var str = sw.ToString();
                Assert.AreEqual(refstr, str);
            }
        }
    }
}

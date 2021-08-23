using InbuiltLogger.Logging;
using System;
using System.IO;
using Xunit;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace InbuiltLogger
{
    public class InbuiltLogTest
    {
        [Fact]
        public void Log4NetLoggerTest()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(dir, "InbuiltLogger-log4net.Test.log");

            InbuiltLog.SetFactory(new Log4NetLoggerFactory());
            InbuiltLogger.Logging.InbuiltLogger logger = InbuiltLog.For(typeof(InbuiltLogTest));

            const string message = "This is a test...";

            logger.Debug(message);
            
            var actual = File.ReadAllText(filePath).IndexOf(message, StringComparison.CurrentCultureIgnoreCase);

            Assert.True(actual >= 0, "Logger does not work...");
        }

        [Fact]
        public void FileLoggerTest()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(dir, $"{nameof(FileLoggerTest)}.log");

            InbuiltLog.SetFactory(new InbuiltFileLoggerFactory(filePath));
            InbuiltLogger.Logging.InbuiltLogger logger = InbuiltLog.For(typeof(InbuiltLogTest));

            const string message = "This is a test...";

            logger.Debug(message);

            var actual = File.ReadAllText(filePath).IndexOf(message, StringComparison.CurrentCultureIgnoreCase);

            Assert.True(actual >= 0, "Logger does not work...");
        }

        [Fact]
        public void MultiLogTest()
        {
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(dir, $"{nameof(MultiLogTest)}.log");

            var consoleLogger = new InbuiltConsoleLogger();
            var fileLogger = new InbuiltFileLogger(filePath);
            var multiLogger = new InbuiltMultipleLoggerSink(consoleLogger, fileLogger);

            var factory = new InbuiltMultipleLoggerFactory(multiLogger);
            InbuiltLog.SetFactory(factory);

            InbuiltLogger.Logging.InbuiltLogger logger = InbuiltLog.For(typeof(InbuiltLogTest));

            const string message = "This is a test...";

            logger.Debug(message);

            var actual = File.ReadAllText(filePath).IndexOf(message, StringComparison.CurrentCultureIgnoreCase);

            Assert.True(actual >= 0, "Logger does not work...");
        }

        [Fact]
        public void NullLogger()
        {
            InbuiltLogger.Logging.InbuiltLogger logger = InbuiltLog.For(typeof(InbuiltLogTest));

            logger.Debug("test");
        }
    }
}
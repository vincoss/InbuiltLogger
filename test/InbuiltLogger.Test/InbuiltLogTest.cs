using InbuiltLogger.Logging;
using NUnit.Framework;
using System;
using System.IO;

[assembly: log4net.Config.XmlConfigurator(ConfigFile = "log4net.config", Watch = true)]

namespace InbuiltLogger
{
    [TestFixture]
    public class InbuiltLogTest
    {
        [Test]
        public void Test()
        {
            InbuiltLog.SetFactory(new Log4NetLoggerFactory());
            InbuiltLogger.Logging.InbuiltLogger logger = InbuiltLog.For(typeof(InbuiltLogTest));

            const string message = "This is a test...";

            logger.Debug(message);
            
            var dir = AppDomain.CurrentDomain.BaseDirectory;
            var actual = File.ReadAllText(string.Format(@"{0}\InbuiltLogger.Test.log", dir)).IndexOf(message, StringComparison.CurrentCultureIgnoreCase);

            Assert.IsTrue(actual >= 0, "Logger does not work...");
        }
    }
}
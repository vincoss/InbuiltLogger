using log4net;
using log4net.Config;
using System;
using System.IO;


namespace InbuiltLogger.Logging
{
    public class Log4NetLoggerFactory : MarshalByRefObject, InbuiltLoggerFactory
    {
        public Log4NetLoggerFactory() : this("log4net.Config")
        {
        }

        public Log4NetLoggerFactory(string configFileName)
        {
            if (string.IsNullOrWhiteSpace(configFileName))
            {
                throw new ArgumentNullException(nameof(configFileName));
            }
            XmlConfigurator.ConfigureAndWatch(GetConfigFile(configFileName));
        }

        public InbuiltLogger CreateLogger(Type type)
        {
            return new Log4NetLogger(Create(type));
        }

        private log4net.Core.ILogger Create(Type type)
        {
            return LogManager.GetLogger(type).Logger;
        }

        private static FileInfo GetConfigFile(string fileName)
        {
            FileInfo result;
            if (Path.IsPathRooted(fileName))
            {
                result = new FileInfo(fileName);
            }
            else
            {
                result = new FileInfo(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, fileName));
            }
            return result;
        }
    }
}

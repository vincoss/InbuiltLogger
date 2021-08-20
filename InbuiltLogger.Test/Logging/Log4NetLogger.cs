using log4net.Core;
using log4net.Util;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace InbuiltLogger.Logging
{
    public class Log4NetLogger : MarshalByRefObject, InbuiltLogger
    {
        private readonly log4net.Core.ILogger _logger;
        private static readonly Type DeclaringType = typeof(Log4NetLogger);

        internal Log4NetLogger(log4net.Core.ILogger logger)
        {
            _logger = logger;
        }

        #region ILogger members

        public bool IsEnabled(InbuiltLogLevel level)
        {
            switch (level)
            {
                case InbuiltLogLevel.Information:
                    {
                        return _logger.IsEnabledFor(Level.Info);
                    }
                case InbuiltLogLevel.Debug:
                    {
                        return _logger.IsEnabledFor(Level.Debug);
                    }
                case InbuiltLogLevel.Warning:
                    {
                        return _logger.IsEnabledFor(Level.Warn);
                    }
                case InbuiltLogLevel.Error:
                    {
                        return _logger.IsEnabledFor(Level.Error);
                    }
                case InbuiltLogLevel.Fatal:
                    {
                        return _logger.IsEnabledFor(Level.Fatal);
                    }
            }
            return false;
        }

        public void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args)
        {
            if (IsEnabled(level) == false)
            {
                return;
            }
            if (args == null || args.Length == 0)
            {
                switch (level)
                {
                    case InbuiltLogLevel.Information:
                        {
                            _logger.Log(DeclaringType, Level.Info, format, exception);
                            break;
                        }
                    case InbuiltLogLevel.Debug:
                        {
                            _logger.Log(DeclaringType, Level.Debug, format, exception);
                            break;
                        }
                    case InbuiltLogLevel.Warning:
                        {
                            _logger.Log(DeclaringType, Level.Warn, format, exception);
                            break;
                        }
                    case InbuiltLogLevel.Error:
                        {
                            _logger.Log(DeclaringType, Level.Error, format, exception);
                            break;
                        }
                    case InbuiltLogLevel.Fatal:
                        {
                            _logger.Log(DeclaringType, Level.Fatal, format, exception);
                            break;
                        }
                }
            }
            else
            {
                switch (level)
                {
                    case InbuiltLogLevel.Debug:
                        {
                            _logger.Log(DeclaringType, Level.Debug, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        }
                    case InbuiltLogLevel.Information:
                        {
                            _logger.Log(DeclaringType, Level.Info, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        }
                    case InbuiltLogLevel.Warning:
                        {
                            _logger.Log(DeclaringType, Level.Warn, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        }
                    case InbuiltLogLevel.Error:
                        {
                            _logger.Log(DeclaringType, Level.Error, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        }
                    case InbuiltLogLevel.Fatal:
                        {
                            _logger.Log(DeclaringType, Level.Fatal, new SystemStringFormat(CultureInfo.InvariantCulture, format, args), exception);
                            break;
                        }
                }
            }
        }

        #endregion
    }
}

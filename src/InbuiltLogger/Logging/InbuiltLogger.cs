using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;


namespace InbuiltLogger.Logging
{
    public enum InbuiltLogLevel : byte
    {
        Debug,
        Information,
        Warning,
        Error,
        Fatal
    }

    public interface InbuiltLogger
    {
        bool IsEnabled(InbuiltLogLevel level);

        void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args);
    }

    public interface InbuiltLoggerFactory
    {
        InbuiltLogger CreateLogger(Type type);
    }

    public sealed class InbuiltLog
    {
        private static InbuiltLoggerFactory _factory;

        static InbuiltLog()
        {
            _factory = new InbuiltNullLoggerFactory();
        }

        #region For
        public static InbuiltLogger For(Type type)
        {
            if (type == null)
            {
                throw new ArgumentNullException(nameof(type));
            }
            return GetLogger(type);
        }

        public static InbuiltLogger For(object itemThatRequiresLoggingServices)
        {
            if (itemThatRequiresLoggingServices == null)
            {
                throw new ArgumentNullException(nameof(itemThatRequiresLoggingServices));
            }
            return For(itemThatRequiresLoggingServices.GetType());
        }

        public static void SetFactory(InbuiltLoggerFactory factory)
        {
            if (factory == null)
            {
                throw new ArgumentNullException(nameof(factory));
            }
            _factory = factory;
        }
        #endregion


        #region GetLogger
        private static InbuiltLogger GetLogger(Type type)
        {
            return _factory.CreateLogger(type);
        }
        #endregion
    }

    public static class InbuiltLoggingExtensions
    {
        private const string DateTimeFormat = "yyyy-MM-dd-HH:mm:ss.fffffff zzz";

        public static void Debug(this InbuiltLogger logger, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Debug, null, message, null);
        }

        public static void Debug(this InbuiltLogger logger, Exception exception, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Debug, exception, message, null);
        }

        public static void Debug(this InbuiltLogger logger, string format, params object[] args)
        {
            LogInternal(logger, InbuiltLogLevel.Debug, null, format, args);
        }

        public static void Debug(this InbuiltLogger logger, Exception exception, string format, params object[] args)
        {
            LogInternal(logger, InbuiltLogLevel.Debug, exception, format, args);
        }

        public static void Error(this InbuiltLogger logger, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Error, null, message, null);
        }

        public static void Error(this InbuiltLogger logger, Exception exception, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Error, exception, message, null);
        }

        public static void Error(this InbuiltLogger logger, string format, params object[] args)
        {
            LogInternal(logger, InbuiltLogLevel.Error, null, format, args);
        }

        public static void Error(this InbuiltLogger logger, Exception exception, string format, params object[] args)
        {
            LogInternal(logger, InbuiltLogLevel.Error, exception, format, args);
        }

        public static void Warning(this InbuiltLogger logger, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Warning, null, message, null);
        }

        public static void Info(this InbuiltLogger logger, string message)
        {
            LogInternal(logger, InbuiltLogLevel.Information, null, message, null);
        }

        public static string GetPretext(InbuiltLogLevel level)
        {
            string pretext;
            switch (level)
            {
                case InbuiltLogLevel.Information:
                    pretext = $"{DateTimeOffset.Now.ToString(DateTimeFormat)} [INF] [{Thread.CurrentThread.ManagedThreadId}]";
                    break;
                case InbuiltLogLevel.Debug:
                    pretext = $"{DateTimeOffset.Now.ToString(DateTimeFormat)} [DBG] [{Thread.CurrentThread.ManagedThreadId}]";
                    break;
                case InbuiltLogLevel.Warning:
                    pretext = $"{DateTimeOffset.Now.ToString(DateTimeFormat)} [WRN] [{Thread.CurrentThread.ManagedThreadId}]";
                    break;
                case InbuiltLogLevel.Error:
                    pretext = $"{DateTimeOffset.Now.ToString(DateTimeFormat)} [ERR] [{Thread.CurrentThread.ManagedThreadId}]";
                    break;
                case InbuiltLogLevel.Fatal:
                    pretext = $"{DateTimeOffset.Now.ToString(DateTimeFormat)} [FTL] [{Thread.CurrentThread.ManagedThreadId}]";
                    break;
                default:
                    pretext = "";
                    break;
            }
            return pretext;
        }

        #region Private methods

        private static void LogInternal(InbuiltLogger logger, InbuiltLogLevel level, Exception exception, string format, object[] objects)
        {
            if (logger.IsEnabled(level))
            {
                logger.Log(level, exception, format, objects);
            }
        }

        #endregion
    }

    public class InbuiltConsoleLogger : InbuiltLogger
    {
        public bool IsEnabled(InbuiltLogLevel level)
        {
            return true;
        }

        public void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args)
        {
            string message = format;
            if (args != null && args.Length > 0)
            {
                message = string.Format(format, args);
            }

            string log = $"{InbuiltLoggingExtensions.GetPretext(level)} {message}\r\n";

            Console.WriteLine(log);

            if (exception != null)
            {
                Console.WriteLine(exception);
            }
        }
    }

    public class InbuiltConsoleLoggerFactory : InbuiltLoggerFactory
    {
        private static readonly InbuiltLogger Logger = new InbuiltConsoleLogger();

        public InbuiltLogger CreateLogger(Type type)
        {
            return Logger;
        }
    }

    public class InbuiltFileLoggerFactory : InbuiltLoggerFactory
    {
        private readonly InbuiltLogger Logger;

        public InbuiltFileLoggerFactory(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentNullException(filePath);
            }
            Logger = new InbuiltFileLogger(filePath);
        }

        public InbuiltLogger CreateLogger(Type type)
        {
            return Logger;
        }
    }

    public class InbuiltNullLogger : InbuiltLogger
    {
        public bool IsEnabled(InbuiltLogLevel level)
        {
            return false;
        }

        public void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args)
        {
            // Hola
        }

        public string GetDeviceInfo()
        {
            return null;
        }
    }

    public class InbuiltNullLoggerFactory : InbuiltLoggerFactory
    {
        private static readonly InbuiltLogger Logger = new InbuiltNullLogger();

        public InbuiltLogger CreateLogger(Type type)
        {
            return Logger;
        }
    }

    public class InbuiltFileLogger : InbuiltLogger
    {
        private readonly string _logPath;

        public InbuiltFileLogger(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                throw new ArgumentNullException(nameof(path));
            }
            _logPath = path;
        }

        public bool IsEnabled(InbuiltLogLevel level)
        {
            return true;
        }

        public void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args)
        {
            string message = format;
            if (args != null && args.Length > 0)
            {
                message = string.Format(format, args);
            }

            string log = $"{InbuiltLoggingExtensions.GetPretext(level)} {message}\r\n";

            Write(log);

            if (exception != null)
            {
                Write($"{exception}\r\n");
            }
        }

        private void Write(string str)
        {
            if (string.IsNullOrWhiteSpace(str))
            {
                return;
            }
            using (var fs = CreateStream(_logPath, true, FileShare.ReadWrite))
            {
                byte[] info = Encoding.UTF8.GetBytes(str);
                fs.Write(info, 0, info.Length);
            }
        }

        private Stream CreateStream(string filename, bool append, FileShare fileShare)
        {
            string directoryFullName = Path.GetDirectoryName(filename);

            if (!Directory.Exists(directoryFullName))
            {
                Directory.CreateDirectory(directoryFullName);
            }

            FileMode fileOpenMode = append ? FileMode.Append : FileMode.Create;
            return new FileStream(filename, fileOpenMode, FileAccess.Write, fileShare);
        }

        public class MultipleInbuiltLogger : InbuiltLogger
        {
            private readonly InbuiltLogger[] _logs;

            public MultipleInbuiltLogger(params InbuiltLogger[] logs)
            {
                var otherMultipleLogs = logs.OfType<MultipleInbuiltLogger>().ToArray();

                this._logs = logs
                    .Except(otherMultipleLogs)
                    .Concat(otherMultipleLogs.SelectMany(l => l._logs))
                    .ToArray();
            }

            public bool IsEnabled(InbuiltLogLevel level)
            {
                return true;
            }

            public void Log(InbuiltLogLevel level, Exception exception, string format, params object[] args)
            {
                foreach (var log in _logs)
                {
                    log.Log(level, exception, format, args);  
                }
            }
        }
    }
}

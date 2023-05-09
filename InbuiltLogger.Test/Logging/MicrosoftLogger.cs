using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace InbuiltLogger.Logging
{
    /// <summary>
    /// Microsoft.Extensions.Logging implemenation
    /// </summary>
    /// <example>
    /// Register
    /// services.Replace(new ServiceDescriptor(typeof(ILogger<>), typeof(InbuiltLogger.Logging.MicrosoftInbuiltLogger<>), ServiceLifetime.Singleton));
    /// </example>
    /// <typeparam name="T"></typeparam>
    public class MicrosoftInbuiltLogger<T> : ILogger<T>
    {
        private InbuiltLogger _logger;

        public MicrosoftInbuiltLogger()
        {
            _logger = InbuiltLog.For(typeof(T));
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            string Formatter(TState innserState, Exception innerException)
            {
                // additional logic goes here, in my case that was extracting additional information from custom exceptions
                var message = formatter(innserState, innerException) ?? string.Empty;
                return message;
            }
            var level = Map(logLevel);
            var message = formatter(state, exception);
            _logger.Log(level, exception, message);
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            var level = Map(logLevel);
            return _logger.IsEnabled(level);
        }

        public IDisposable BeginScope<TState>(TState state)
        {
            throw new NotSupportedException();
        }

        private static InbuiltLogLevel Map(LogLevel logLevel)
        {
            switch (logLevel)
            {
                case LogLevel.Debug: return InbuiltLogLevel.Debug;
                case LogLevel.Information: return InbuiltLogLevel.Information;
                case LogLevel.Warning: return InbuiltLogLevel.Warning;
                case LogLevel.Error: return InbuiltLogLevel.Error;
                case LogLevel.Critical: return InbuiltLogLevel.Fatal;
                default: return InbuiltLogLevel.Debug;
            }
        }
    }
}

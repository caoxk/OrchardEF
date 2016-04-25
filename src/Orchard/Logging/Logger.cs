using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orchard.Logging {
    public class Logger<T> : Microsoft.Extensions.Logging.ILogger<T> {
        private readonly Microsoft.Extensions.Logging.ILogger _logger;

        /// <summary>
        /// Creates a new <see cref="Logger{T}"/>.
        /// </summary>
        /// <param name="factory">The factory.</param>
        public Logger(Microsoft.Extensions.Logging.ILoggerFactory factory) {
            _logger = factory.CreateLogger<T>();
        }

        IDisposable Microsoft.Extensions.Logging.ILogger.BeginScopeImpl(object state) {
            return _logger.BeginScopeImpl(state);
        }

        bool Microsoft.Extensions.Logging.ILogger.IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) {
            return _logger.IsEnabled(logLevel);
        }

        void Microsoft.Extensions.Logging.ILogger.Log(Microsoft.Extensions.Logging.LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter) {
            _logger.Log(logLevel, eventId, state, exception, formatter);
        }
    }
}

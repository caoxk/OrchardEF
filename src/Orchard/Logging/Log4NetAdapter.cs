using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orchard.Logging {
    public class Log4NetAdapter : Microsoft.Extensions.Logging.ILogger {

        public Log4NetAdapter(ILogger logger) {
            Logger = logger;
        }

        public ILogger Logger { get; set; }

        public IDisposable BeginScopeImpl(object state) {
            return null;
        }

        public bool IsEnabled(Microsoft.Extensions.Logging.LogLevel logLevel) {
            switch (logLevel) {
                case Microsoft.Extensions.Logging.LogLevel.Verbose:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    return Logger.IsEnabled(LogLevel.Debug);
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    return Logger.IsEnabled(LogLevel.Information);
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    return Logger.IsEnabled(LogLevel.Warning);
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    return Logger.IsEnabled(LogLevel.Error);
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    return Logger.IsEnabled(LogLevel.Fatal);
                default:
                    throw new ArgumentException($"Unknown log level {logLevel}.", nameof(logLevel));
            }
        }

        public void Log(Microsoft.Extensions.Logging.LogLevel logLevel, int eventId, object state, Exception exception, Func<object, Exception, string> formatter) {
            if (!IsEnabled(logLevel)) {
                return;
            }
            string message = null;
            if (null != formatter) {
                message = formatter(state, exception);
            }
            else {
                message = LogFormatter.Formatter(state, exception);
            }
            switch (logLevel) {
                case Microsoft.Extensions.Logging.LogLevel.Verbose:
                case Microsoft.Extensions.Logging.LogLevel.Debug:
                    Logger.Debug(message, exception);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Information:
                    Logger.Information(message, exception);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Warning:
                    Logger.Warning(message, exception);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Error:
                    Logger.Error(message, exception);
                    break;
                case Microsoft.Extensions.Logging.LogLevel.Critical:
                    Logger.Fatal(message, exception);
                    break;
                default:
                    Logger.Warning($"Encountered unknown log level {logLevel}, writing out as Info.");
                    Logger.Information(message, exception);
                    break;
            }
        }
    }
}

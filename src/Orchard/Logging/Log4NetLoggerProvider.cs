using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Orchard.Logging {
    public class Log4NetProvider : ILoggerProvider {
        private IDictionary<string, Microsoft.Extensions.Logging.ILogger> _loggers
            = new Dictionary<string, Microsoft.Extensions.Logging.ILogger>();

        private readonly ILogger _logger;

        public Log4NetProvider(ILogger logger) {
            _logger = logger;
        }

        public Microsoft.Extensions.Logging.ILogger CreateLogger(string name) {
            if (!_loggers.ContainsKey(name)) {
                lock (_loggers) {
                    // Have to check again since another thread may have gotten the lock first
                    if (!_loggers.ContainsKey(name)) {
                        _loggers[name] = new Log4NetAdapter(_logger);
                    }
                }
            }
            return _loggers[name];
        }

        public void Dispose() {
            _loggers.Clear();
            _loggers = null;
        }
    }
}

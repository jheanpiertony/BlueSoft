using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace Common.Services
{
    public class LogerService : ILogerService
    {
        private ILogger<LogerService> _logger;

        public LogerService(ILogger<LogerService> logger)
        {
            _logger = logger;
        }
        public void CraerLogs()
        {
            _logger.LogInformation("Mensaje de informacion");
            _logger.LogWarning("Mensaje de advertencia");
            _logger.LogError("Mensaje de error");
        }
    }
}

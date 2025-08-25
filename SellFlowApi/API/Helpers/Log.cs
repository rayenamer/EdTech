using System;

namespace API.Helpers;

public class Log
{
    private readonly ILogger<Log> _logger;

    public Log(ILogger<Log> logger)
    {
        _logger = logger;
    }

    public void LogInformation(string message)
    {
        _logger.LogInformation("╔══════════════════════════════════════════════════════════════╗");
        _logger.LogInformation("║                                                              ║");
        _logger.LogInformation("║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║");
        _logger.LogInformation($"║             🚀 {message} 🚀             ║");
        _logger.LogInformation("║░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░░║");
        _logger.LogInformation("║                                                              ║");
        _logger.LogInformation("╚══════════════════════════════════════════════════════════════╝");
    }
}

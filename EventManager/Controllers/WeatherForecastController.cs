using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Contracts;
using Microsoft.AspNetCore.Mvc;

namespace EventManager.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private ILoggerManager _logger;

        public WeatherForecastController(ILoggerManager logger)
        {
            Logger = logger;
        }

        public ILoggerManager Logger { get => _logger; set => _logger = value; }

        [HttpGet("/eventmanager")]
        public IEnumerable<string> Get()
        {
            Logger.LogInfo("Here is info message from the controller.");
            Logger.LogDebug("Here is debug message from the controller.");
            Logger.LogWarn("Here is warn message from the controller.");
            Logger.LogError("Here is error message from the controller.");
            return new string[] { "value1", "value2" };
        }
    }
}
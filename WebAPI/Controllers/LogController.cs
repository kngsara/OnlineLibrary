using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using WebAPI.Repository;
using Shared;

namespace WebAPI.Controllers
{


    [Route("api/[controller]")]
    [ApiController]
    public class LogController : ControllerBase
    {
        private readonly ILogRepository _logRepository;
        //LOGGER
        private readonly ILogRepository _logger;
        public LogController(ILogRepository logRepository, ILogRepository logger)
        {
            _logRepository = logRepository;
            _logger = logger;
        }

        [HttpGet("GetLogs/{n}")]
        public async Task<IActionResult> GetLogs(int n)
        {
            var result = await _logRepository.GetLogs(n);
            var logs = new Log();

            if (result.Count == 0)
            {
                logs = new Log
                {
                    CreatedTime = DateTime.Now,
                    LogLevel = 2,
                    LogMessage = "No logs found"

                };
                await _logger.CreateLog(logs);
                return NotFound("No results");
            }
            var logDTOs = new List<LogDTO>();
            foreach (var log in result)
            {

                logDTOs.Add(new LogDTO
                {
                    LogId = log.LogId,
                    CreatedTime = log.CreatedTime,
                    LogLevel = log.LogLevel,
                    LogMessage = log.LogMessage
                });
            }
            logs = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Logs fetched successfully"

            };
            await _logger.CreateLog(logs);
            return Ok(logDTOs);
        }

        [HttpGet("GetCountedLogs")]
        public async Task<IActionResult> GetLogsTotalCount()
        {
            var log = new Log();
            var result = await _logRepository.CountLogs();
            log = new Log
            {
                CreatedTime = DateTime.Now,
                LogLevel = 1,
                LogMessage = "Log counted successfully"

            };
            await _logger.CreateLog(log);
            return Ok(result);
        }
    }
}

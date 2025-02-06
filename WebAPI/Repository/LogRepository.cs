using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Shared;

namespace WebAPI.Repository
{
    public class LogRepository : ILogRepository
    {
        private readonly OnlineLibraryContext _context;

        public LogRepository(OnlineLibraryContext context)
        {
            _context = context;
        }

        public async Task<int> CountLogs()
        {
            var result = await _context.Logs.CountAsync();
            return result;
        }

        public async Task CreateLog(Log log)
        {
            await _context.Logs.AddAsync(log);
            var result = await _context.SaveChangesAsync();

        }

        public async Task<List<LogDTO>> GetLogs(int n)
        {
            var logs = await _context.Logs.OrderByDescending(l => l.CreatedTime).Take(n).ToListAsync();

            var logList = new List<LogDTO>();
            foreach (var log in logs)
            {
                logList.Add(new LogDTO
                {
                    LogId = log.LogId,
                    CreatedTime = log.CreatedTime,
                    LogLevel = log.LogLevel,
                    LogMessage = log.LogMessage
                });
            }

            return logList;
        }
    }
}

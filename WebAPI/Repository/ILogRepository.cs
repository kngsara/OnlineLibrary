using WebAPI.Models;
using Shared;

namespace WebAPI.Repository
{
    public interface ILogRepository
    {
        Task CreateLog(Log log);

        Task<List<LogDTO>> GetLogs(int n);

        Task<int> CountLogs();

    }
}

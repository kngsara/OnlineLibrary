using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class LogDTO
    {
        public int LogId { get; set; }

        public DateTime CreatedTime { get; set; }

        public int? LogLevel { get; set; }

        public string LogMessage { get; set; } = null!;
    }
}

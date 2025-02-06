using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class Response
    {
        public ResultStatus Status { get; set; }
        public string? Message { get; set; }
    }

    public enum ResultStatus
    {
        Success,
        NotFound,
        Error
    }
}

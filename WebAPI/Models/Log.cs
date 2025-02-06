using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Log
{
    public int LogId { get; set; }

    public DateTime CreatedTime { get; set; }

    public int? LogLevel { get; set; }

    public string LogMessage { get; set; } = null!;
}

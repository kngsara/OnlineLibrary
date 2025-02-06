using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class PaginatedResult
    {
        public List<BookDTO> Data{ get; set; }
        public int TotalCount { get; set; }
    }
}

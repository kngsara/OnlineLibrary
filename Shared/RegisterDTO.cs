using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class RegisterDTO
    {
        //required zbog validacije, i na front i na back
        [Required]
        public string Username { get; set; }
        [Required]
        public string Lozinka { get; set; } 
    }
}

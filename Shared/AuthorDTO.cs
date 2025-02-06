using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared
{
    public class AuthorDTO
    {
        //data transfer object
        public int AuthorId { get; set; }
        //anotacije
        [Required]
        public string FirstName { get; set; } = null!;
        [Required]
        public string LastName { get; set; } = null!;
        public string FullName 
        { 
            get { return $"{FirstName} {LastName}"; }
        }

        public virtual ICollection<BookDTO>? Books { get; set; } = new List<BookDTO>();
    }
}

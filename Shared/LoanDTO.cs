using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class LoanDTO
    {
        public int LoanId { get; set; }
        [Required]
        public int? BookId { get; set; }
        [Required]
        public int? MemberId { get; set; }

        public DateTime LoanDate { get; set; }

        public DateTime? ReturnDate { get; set; }

        public virtual BookDTO? Book { get; set; }

        public virtual MemberDTO? Member { get; set; }
    }
}
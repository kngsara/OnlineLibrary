namespace Shared
{
    public class MemberDTO
    {
        public int MemberId { get; set; }

        public string Username { get; set; } = null!;

        public string? Lozinka { get; set; } = null!;

        public DateTime JoinDate { get; set; }

        public int? RoleId { get; set; }

        public virtual ICollection<LoanDTO>? Loans { get; set; } = new List<LoanDTO>();

        public virtual RoleDTO? Role { get; set; }
    }
}
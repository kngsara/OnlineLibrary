namespace Shared
{
    public class RoleDTO
    {
        public int RoleId { get; set; }

        public string RoleName { get; set; } = null!;

        public virtual ICollection<MemberDTO> Members { get; set; } = new List<MemberDTO>();
    }
}
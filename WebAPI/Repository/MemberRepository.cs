using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Shared;

namespace WebAPI.Repository
{
    public class MemberRepository : IMemberRepository
    {
        private readonly OnlineLibraryContext _context;

        public MemberRepository(OnlineLibraryContext context)
        {
            _context = context;
        }

        public async Task<Response> Delete(int id)
        {
            var member = await _context.Members.Include(m=>m.Loans).FirstOrDefaultAsync(m=>m.MemberId==id);
            if (member == null)
            {
                return new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Member cannot be found"
                };
            }

            try
            {
                member.Loans.Clear();
                _context.Members.Remove(member);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new Response
                    {
                        Status = ResultStatus.Success,
                        Message = "Member deleted successfully"
                    };
                }
                return new Response
                {
                    Status = ResultStatus.Error,
                    Message = "Member cannot be deleted"
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<MemberDTO> Login(LoginDTO loginDTO)
        {
            try
            {
                var member = await _context.Members.FirstOrDefaultAsync
            (m => m.Username == loginDTO.Username && m.Lozinka == loginDTO.Lozinka);
                if (member == null)
                {
                    return null;
                }
                MemberDTO memberDTO = new MemberDTO
                {
                    MemberId = member.MemberId,
                    Username = loginDTO.Username,
                    Lozinka = loginDTO.Lozinka
                };
                return memberDTO;
            }
            catch (Exception ex)
            {

                return new MemberDTO(); //error
            }

        }

        public async Task<MemberDTO> ReadById(int id)
        {
            try
            {
                var member = await _context.Members.Include(r => r.Role).FirstOrDefaultAsync
            (m => m.MemberId == id);
                if (member == null)
                {
                    return null;
                }
                MemberDTO memberDTO = new MemberDTO
                {
                    MemberId = member.MemberId,
                    Username = member.Username,
                    Lozinka = member.Lozinka,
                    JoinDate = member.JoinDate,
                    Role = new RoleDTO
                    {
                        RoleName = member.Role.RoleName
                    }
                };
                return memberDTO;
            }
            catch (Exception ex)
            {

                return new MemberDTO();
            }
        }

        public async Task<Response> Register(RegisterDTO registerDTO)
        {
            //provjera je li postoji vec member
            var oldMember = await _context.Members.FirstOrDefaultAsync(m => m.Username == registerDTO.Username);
            if (oldMember != null)
            {
                return new Response
                {
                    Status = ResultStatus.Error,
                    Message = "Member with that username already exists"
                };
            }

            Member member = new Member()
            {
                Username = registerDTO.Username,
                Lozinka = registerDTO.Lozinka,
                //JoinDate = DateOnly.FromDateTime(DateTime.Now),
                JoinDate = DateTime.Now,
                Role = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "Member")
            };
            await _context.Members.AddAsync(member);
            var result = await _context.SaveChangesAsync();
            if (result>0)
            {
                return new Response
                {
                    Status = ResultStatus.Success,
                    Message = "Member added successfully"
                };
            }
            return new Response
            {
                Status = ResultStatus.Error,
                Message = "Member cannot be added"
            };
        }

        public async Task<MemberDTO> Update(MemberDTO memberDTO)
        {
            var member = await _context.Members.FindAsync(memberDTO.MemberId);
            if (member == null)
            {
                return null; //not found
            }

            member.Username = memberDTO.Username;

            var result = await _context.SaveChangesAsync();

            if(result>0)
            {
                return memberDTO;
            }
            return new MemberDTO(); //error
        }
    }
}

using Shared;

namespace WebAPI.Repository
{
    public interface IMemberRepository
    {
        Task<Response> Register(RegisterDTO registerDTO);
        Task<MemberDTO> Login(LoginDTO loginDTO);
        Task<MemberDTO> ReadById(int id);
        Task<MemberDTO> Update(MemberDTO memberDTO);
        Task<Response> Delete(int id);
    }
}

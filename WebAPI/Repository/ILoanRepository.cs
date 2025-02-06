using Shared;

namespace WebAPI.Repository
{
    public interface ILoanRepository
    {
        Task<Response> Create(LoanDTO loanDTO);
        Task<List<LoanDTO>> GetAll();
        Task<List<LoanDTO>> GetLoanByMember(int id);
        Task<Response> ReturnBook(int id);
    }
}

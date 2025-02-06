using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Shared;

namespace WebAPI.Repository
{
    public class LoanRepository : ILoanRepository
    {
        private readonly OnlineLibraryContext _context;

        public LoanRepository(OnlineLibraryContext context)
        {
            _context = context;
        }

        public async Task<Response> Create(LoanDTO loanDTO)
        {
            var book = await _context.Books.FindAsync(loanDTO.BookId);
            if (book == null)
            {
                return new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Book cannot be found"
                };
            }
            var member = await _context.Members.FindAsync(loanDTO.MemberId);
            if (member == null)
            {
                return new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Member cannot be found"
                };
            }

            Loan loan = new Loan
            {
                //LoanDate = DateOnly.FromDateTime(DateTime.Now),
                LoanDate = DateTime.Now,
                Book = book,
                Member = member,
            };

            await _context.Loans.AddAsync(loan);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new Response
                {
                    Status = ResultStatus.Success,
                    Message = "Loan created successfully"
                };

            }

            return new Response
            {
                Status = ResultStatus.Error,
                Message = "Loan cannot be created"
            };

        }

        public async Task<List<LoanDTO>> GetAll()
        {
            var loans = await _context.Loans.Include(l => l.Book).Include(l=>l.Member).ToListAsync(); 
            if(!loans.Any())
            {
                return new List<LoanDTO>();
            }

            var loansDTO = loans.Select(l => new LoanDTO
            {
                LoanId = l.LoanId,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate,
                Book = new BookDTO
                {
                    Title = l.Book?.Title
                },
                Member = new MemberDTO
                {
                    Username = l.Member?.Username
                }
            }).ToList();
        

                return loansDTO;
        }

        public async Task<List<LoanDTO>> GetLoanByMember(int id)
        {
            var loans = await _context.Loans
                .Include(l=>l.Book)
                .Include(l=>l.Member)
                .Where(l=>l.MemberId == id).ToListAsync();
            
            if (!loans.Any())
            {
                return new List<LoanDTO>();
            }

            var loansDTO = loans.Select(l => new LoanDTO
            {
                LoanId = l.LoanId,
                LoanDate = l.LoanDate,
                ReturnDate = l.ReturnDate,
                Book = new BookDTO
                {
                    Title = l.Book.Title
                },
                Member = new MemberDTO
                {
                    Username = l.Member.Username
                }
            }).ToList();

            return loansDTO;

        }

        public async Task<Response> ReturnBook(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan == null)
            {
                return new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Cannot be found"
                };
            }
            //loan.ReturnDate = DateOnly.FromDateTime(DateTime.Now);
            loan.ReturnDate = DateTime.Now;

            var result = await _context.SaveChangesAsync();

            if (result>0)
            {
                return new Response
                {
                    Status = ResultStatus.Success,
                    Message = "The book has just been returned"
                };
            }

            return new Response
            {
                Status = ResultStatus.Error,
                Message = "Error occured"
            };
        }
    }
}

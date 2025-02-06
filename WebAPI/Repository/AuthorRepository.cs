using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Shared;

namespace WebAPI.Repository
{
    public class AuthorRepository : IAuthorRepository
    {
        private readonly OnlineLibraryContext _context;
        

        public AuthorRepository(OnlineLibraryContext context)
        {
            _context = context;
        }

        //implementacija 
        public async Task<Response> Create(AuthorDTO authorDTO)
        {
            Author author = new Author
            {
                FirstName = authorDTO.FirstName,
                LastName = authorDTO.LastName,
            };

            await _context.Authors.AddAsync(author);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return new Response
                {
                    Status = ResultStatus.Success,
                    Message = "Author created successfully"
                };
            }

            return new Response
            {
                Status = ResultStatus.Error,
                Message = "Author cannot be created"
            };
        }

        public async Task<Response> Delete(int id)
        {
            
            var author = await _context.Authors.Include(a=>a.Books).FirstOrDefaultAsync(a=>a.AuthorId==id);
            
            if (author == null)
            {
                return new Response
                {
                    Status = ResultStatus.NotFound,
                    Message = "Author cannot be found"
                };
            }
            author.Books.Clear();
            _context.Authors.Remove(author);
            var result = await _context.SaveChangesAsync();

            if (result > 0) 
            {
                return new Response
                {
                    Status = ResultStatus.Success,
                    Message = "Author deleted successfully"
                };
            }
            return new Response
            {
                Status = ResultStatus.Error,
                Message = "Author cannot be deleted"
            };
        }

        public async Task<List<AuthorDTO>> GetAll()
        {
            var authors = await _context.Authors.Include(b=>b.Books).ToListAsync();
            if (!authors.Any())
            {
                return new List<AuthorDTO>();
            }

            var authorDTO = authors.Select(a => new AuthorDTO
            {
                AuthorId = a.AuthorId,
                FirstName = a.FirstName,
                LastName = a.LastName,
                Books = a.Books.Select(b => new BookDTO
                {
                    BookId = b.BookId,
                    Title = b.Title,
                    Isbn = b.Isbn,
                    PublishedYear = b.PublishedYear,
                    ShortDescription = b.ShortDescription,
                    Loans = null,
                    Authors = null,
                }).ToList()
            });
            return authorDTO.ToList();
        }

        public async Task<AuthorDTO> ReadByID(int id)
        {
            try
            {
                var author = await _context.Authors.FindAsync(id);

                if (author == null)
                {
                    return null; //ako je not found
                }
                AuthorDTO authorDTO = new AuthorDTO
                {
                    AuthorId = author.AuthorId,
                    FirstName = author.FirstName,
                    LastName = author.LastName
                };
                return authorDTO;
            }
            catch (Exception ex)
            {

                return new AuthorDTO();
            }
        }

        public async Task<AuthorDTO> Update(AuthorDTO authorDTO)
        {
            var author = await _context.Authors.FindAsync(authorDTO.AuthorId);
            if (author == null)
            {
                return null; //ako je not found
            }
            author.FirstName = authorDTO.FirstName;
            author.LastName = authorDTO.LastName;

            var result = await _context.SaveChangesAsync();
            if (result > 0)
            {
                return authorDTO;
            }
            return new AuthorDTO();
        }

    }
}

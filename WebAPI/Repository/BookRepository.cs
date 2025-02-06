using Microsoft.EntityFrameworkCore;
using WebAPI.Models;
using Shared;
using System.Linq.Expressions;

namespace WebAPI.Repository
{
    public class BookRepository : IBookRepository
    {
        private readonly OnlineLibraryContext _context;

        public BookRepository(OnlineLibraryContext context)
        {
            _context = context;
        }

        public async Task<Response> Create(BookDTO bookdto)
        {
            try
            {
                var authors = await _context.Authors.Where(a => bookdto.SelectedAuthorsId.Contains(a.AuthorId)).ToListAsync();
                
                var existingBook = await _context.Books.FirstOrDefaultAsync(b=>b.Title == bookdto.Title);
                
                if (existingBook != null)
                {
                    return new Response
                    {
                        Status = ResultStatus.Error,
                        Message = "This title is already in use"
                    };
                }

                Book book = new Book
                {
                    Title = bookdto.Title,
                    Isbn = bookdto.Isbn,
                    PublishedYear = bookdto.PublishedYear,
                    ShortDescription = bookdto.ShortDescription,
                    Authors = authors
                };

                await _context.Books.AddAsync(book);

                var result = await _context.SaveChangesAsync(); //br podatak koji se prom u bazi

                if (result > 0)
                {
                    return new Response
                    {
                        Status = ResultStatus.Success,
                        Message = "Book added succesfully"
                    };
                }

                return new Response
                {
                    Status = ResultStatus.Error,
                    Message = "Book cannot be added"
                };
            }
            catch (Exception ex)
            {

                return new Response
                {
                    Status = ResultStatus.Error,
                    Message = "Book cannot be added"
                };
            }


        }

        public async Task<Response> Delete(int id)
        {
            try
            {
                var book = await _context.Books.Include(b => b.Authors).Include(b=>b.Loans).FirstOrDefaultAsync(b => b.BookId == id);

                if (book == null)
                {
                    return new Response
                    {
                        Status = ResultStatus.NotFound,
                        Message = "Book cannot be found"

                    };
                }

                book.Authors.Clear(); //brisanje relacije
                book.Loans.Clear();

                _context.Books.Remove(book);
                var result = await _context.SaveChangesAsync();

                if (result > 0)
                {
                    return new Response
                    {
                        Status = ResultStatus.Success,
                        Message = "Book deleted successfully"
                    };
                }
                return new Response
                {
                    Status = ResultStatus.Error,
                    Message = "Book cannot be deleted"
                };
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResult> ReadAll(RequestDTO requestDTO)
        {
            try
            {
                var query = _context.Books.Include(b => b.Authors).AsQueryable();

                if (!string.IsNullOrWhiteSpace(requestDTO.SearchTerm))
                {
                    query = query.Where(b => b.Title.Contains(requestDTO.SearchTerm) ||
                    b.Authors.Any(a => a.FirstName.Contains(requestDTO.SearchTerm) ||
                    a.LastName.Contains(requestDTO.SearchTerm)));
                }

                var totalCount = await query.CountAsync();
                var skip = (requestDTO.Page - 1) * requestDTO.PageSize;

                var books = await query.Skip(skip).Take(requestDTO.PageSize).ToListAsync();


                if (!books.Any())
                {
                    return null; // not found
                }

                var paginatedData = new PaginatedResult
                {
                    Data = books.Select(book => new BookDTO
                    {
                        BookId = book.BookId,
                        Title = book.Title,
                        Isbn = book.Isbn,
                        PublishedYear = book.PublishedYear,
                        ShortDescription = book.ShortDescription
                    }).ToList(),
                    TotalCount = totalCount,
                };

                return paginatedData; //uspjeh
            }
            catch (Exception ex)
            {
                return new PaginatedResult();
            }
        }

        public async Task<BookDTO> ReadById(int id)
        {
            try
            {
                var book = await _context.Books.Include(b=>b.Authors).FirstOrDefaultAsync(b=>b.BookId==id);
                if (book == null)
                {
                    return null; //not found
                }

                List<AuthorDTO> authors = new List<AuthorDTO>();
                foreach (var author in book.Authors)
                {
                    authors.Add(new AuthorDTO
                    {
                        AuthorId = author.AuthorId,
                        FirstName = author.FirstName,
                        LastName = author.LastName
                    });
                   
                }

                BookDTO bookDTO = new BookDTO
                {
                    BookId = book.BookId,
                    Title = book.Title,
                    Isbn = book.Isbn,
                    PublishedYear = book.PublishedYear,
                    ShortDescription = book.ShortDescription,
                    Authors = authors
                };
                return bookDTO;
            }
            catch (Exception ex)
            {
                return new BookDTO(); //error
            }
        }

        public async Task<BookDTO> Update(BookDTO bookDTO)
        {
            var book = await _context.Books.Include(b => b.Authors).FirstOrDefaultAsync(b => b.BookId == bookDTO.BookId);
            if (book == null)
            {
                return null; //not found
            }

            var authors = await _context.Authors.Where(a => bookDTO.SelectedAuthorsId.Contains(a.AuthorId)).ToListAsync();

            book.Authors.Clear();

            book.Title = bookDTO.Title;
            book.PublishedYear = bookDTO.PublishedYear;
            book.Isbn = bookDTO.Isbn;
            book.ShortDescription = bookDTO.ShortDescription;
            book.Authors = authors;
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                return bookDTO;
            }

            return new BookDTO(); //error, vraca defaultnu vrijednost objekta
        }
    }
}

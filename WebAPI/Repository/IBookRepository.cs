using Shared;

namespace WebAPI.Repository
{
    public interface IBookRepository
    {
        Task<Response> Create(BookDTO bookdto);
        Task<BookDTO> Update(BookDTO bookDTO);
        Task<Response> Delete(int id);
        Task<BookDTO> ReadById(int id);
        Task<PaginatedResult> ReadAll(RequestDTO requestDTO);

    }
}

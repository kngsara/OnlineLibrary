using Shared;

namespace WebAPI.Repository
{
    public interface IAuthorRepository
    {
        //radim metode, sta zelim raditi s autorom, CRUD operacije
        //definicije metoda

        Task<Response> Create(AuthorDTO authorDTO);
        Task<AuthorDTO> ReadByID(int id);
        Task<AuthorDTO> Update(AuthorDTO authorDTO);
        Task<Response> Delete(int id);

        Task<List<AuthorDTO>> GetAll();

    }
}

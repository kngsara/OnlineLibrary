using System.ComponentModel.DataAnnotations;

namespace Shared
{
    public class BookDTO
    {
        public int BookId { get; set; }
        [Required]
        public string Title { get; set; } = null!;
        [Required]
        public string Isbn { get; set; } = null!;
        [Required]
        public int? PublishedYear { get; set; }
        [Required]
        public string? ShortDescription { get; set; }

        public virtual ICollection<LoanDTO>? Loans { get; set; } = new List<LoanDTO>();

        public virtual ICollection<AuthorDTO>? Authors { get; set; } = new List<AuthorDTO>();

        public List<int> SelectedAuthorsId { get; set; }

    }
}
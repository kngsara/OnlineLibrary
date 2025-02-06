using System;
using System.Collections.Generic;

namespace WebAPI.Models;

public partial class Book
{
    public int BookId { get; set; }

    public string Title { get; set; } = null!;

    public string Isbn { get; set; } = null!;

    public int? PublishedYear { get; set; }

    public string? ShortDescription { get; set; }

    public virtual ICollection<Loan> Loans { get; set; } = new List<Loan>();

    public virtual ICollection<Author> Authors { get; set; } = new List<Author>();
}

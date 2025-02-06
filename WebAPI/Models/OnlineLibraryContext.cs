using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace WebAPI.Models;

public partial class OnlineLibraryContext : DbContext
{
    public OnlineLibraryContext()
    {
    }

    public OnlineLibraryContext(DbContextOptions<OnlineLibraryContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Author> Authors { get; set; }

    public virtual DbSet<Book> Books { get; set; }

    public virtual DbSet<Loan> Loans { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<Member> Members { get; set; }

    public virtual DbSet<Role> Roles { get; set; }

    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

            var connectionString = config.GetConnectionString("DefaultConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Author>(entity =>
        {
            entity.HasKey(e => e.AuthorId).HasName("PK__Authors__70DAFC1422C725D4");

            entity.Property(e => e.AuthorId).HasColumnName("AuthorID");
            entity.Property(e => e.FirstName).HasMaxLength(100);
            entity.Property(e => e.LastName).HasMaxLength(100);
        });

        modelBuilder.Entity<Book>(entity =>
        {
            entity.HasKey(e => e.BookId).HasName("PK__Books__3DE0C22754F3958D");

            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.Isbn)
                .HasMaxLength(13)
                .HasColumnName("ISBN");
            entity.Property(e => e.ShortDescription).HasMaxLength(255);
            entity.Property(e => e.Title).HasMaxLength(255);

            entity.HasMany(d => d.Authors).WithMany(p => p.Books)
                .UsingEntity<Dictionary<string, object>>(
                    "BookAuthor",
                    r => r.HasOne<Author>().WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookAutho__Autho__3C69FB99"),
                    l => l.HasOne<Book>().WithMany()
                        .HasForeignKey("BookId")
                        .OnDelete(DeleteBehavior.ClientSetNull)
                        .HasConstraintName("FK__BookAutho__BookI__3B75D760"),
                    j =>
                    {
                        j.HasKey("BookId", "AuthorId").HasName("PK__BookAuth__6AED6DE6DAD361AF");
                        j.ToTable("BookAuthors");
                        j.IndexerProperty<int>("BookId").HasColumnName("BookID");
                        j.IndexerProperty<int>("AuthorId").HasColumnName("AuthorID");
                    });
        });

        modelBuilder.Entity<Loan>(entity =>
        {
            entity.HasKey(e => e.LoanId).HasName("PK__Loans__4F5AD43748F1B595");

            entity.Property(e => e.LoanId).HasColumnName("LoanID");
            entity.Property(e => e.BookId).HasColumnName("BookID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");

            entity.HasOne(d => d.Book).WithMany(p => p.Loans)
                .HasForeignKey(d => d.BookId)
                .HasConstraintName("FK__Loans__BookID__440B1D61");

            entity.HasOne(d => d.Member).WithMany(p => p.Loans)
                .HasForeignKey(d => d.MemberId)
                .HasConstraintName("FK__Loans__MemberID__44FF419A");
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.HasKey(e => e.LogId).HasName("PK__Logs__5E5499A8E7F0AF48");

            entity.Property(e => e.LogId).HasColumnName("LogID");
            entity.Property(e => e.CreatedTime).HasColumnType("datetime");
            entity.Property(e => e.LogMessage)
                .HasMaxLength(255)
                .IsUnicode(false);
        });

        modelBuilder.Entity<Member>(entity =>
        {
            entity.HasKey(e => e.MemberId).HasName("PK__Members__0CF04B3898CB8907");

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Lozinka).HasMaxLength(100);
            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.Username).HasMaxLength(100);

            entity.HasOne(d => d.Role).WithMany(p => p.Members)
                .HasForeignKey(d => d.RoleId)
                .HasConstraintName("FK__Members__RoleID__412EB0B6");
        });

        modelBuilder.Entity<Role>(entity =>
        {
            entity.HasKey(e => e.RoleId).HasName("PK__Roles__8AFACE3A495E0899");

            entity.Property(e => e.RoleId).HasColumnName("RoleID");
            entity.Property(e => e.RoleName).HasMaxLength(50);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}

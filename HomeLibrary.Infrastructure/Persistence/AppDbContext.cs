using HomeLibrary.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace HomeLibrary.Infrastructure.Persistence;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Book> Books => Set<Book>();
    public DbSet<Author> Authors => Set<Author>();

    protected override void OnModelCreating(ModelBuilder mb)
    {
        mb.Entity<Author>(e =>
        {
            e.ToTable("authors");
            e.HasKey(a => a.AuthorId);
            e.Property(a => a.AuthorId).HasColumnName("author_id");
            e.Property(a => a.FirstName).HasColumnName("first_name").HasMaxLength(100);
            e.Property(a => a.LastName).HasColumnName("last_name").HasMaxLength(100);
        });

        mb.Entity<Book>(e =>
        {
            e.ToTable("books");
            e.HasKey(b => b.BookId);
            e.Property(b => b.BookId).HasColumnName("book_id");
            e.Property(b => b.Title).HasColumnName("title").HasMaxLength(300);
            e.Property(b => b.AuthorId).HasColumnName("author_id");
            e.Property(b => b.PublishYear).HasColumnName("publish_year");
            e.Property(b => b.Publisher).HasColumnName("publisher").HasMaxLength(200);
            e.Property(b => b.ISBN).HasColumnName("isbn").HasMaxLength(20);
            e.Property(b => b.Pages).HasColumnName("pages");
            e.Property(b => b.Contents).HasColumnName("contents").HasColumnType("xml");
            e.Property(b => b.CreatedAt).HasColumnName("created_at");
            e.Property(b => b.UpdatedAt).HasColumnName("updated_at");

            e.HasOne(b => b.Author)
             .WithMany(a => a.Books)
             .HasForeignKey(b => b.AuthorId);
        });
    }
}
using Dapper;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Domain.Interfaces;
using HomeLibrary.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using System.Data;

namespace HomeLibrary.Infrastructure.Repositories;

public class BookRepository : IBookRepository
{
    private readonly AppDbContext _db;
    private readonly string _cs;

    public BookRepository(AppDbContext db, string connectionString)
    {
        _db = db;
        _cs = connectionString;
    }

    public async Task<IEnumerable<Book>> GetAllAsync(string? search, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = @"
        SELECT * FROM sp_books_select(
            @p_search_term, @p_author_id, @p_year_from, @p_year_to)";

        var rows = await conn.QueryAsync<BookRow>(new CommandDefinition(
            sql,
            new
            {
                p_search_term = search,
                p_author_id = (int?)null,
                p_year_from = (int?)null,
                p_year_to = (int?)null
            },
            cancellationToken: ct));

        return rows.Select(ToBook);
    }

    public async Task<Book?> GetByIdAsync(int id, CancellationToken ct = default)
        => await _db.Books.Include(b => b.Author)
                          .FirstOrDefaultAsync(b => b.BookId == id, ct);

    public async Task<int> CreateAsync(Book book, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = @"
        SELECT sp_books_insert(
            @p_title, @p_author_id, @p_publish_year,
            @p_publisher, @p_isbn, @p_pages,
            @p_contents::xml
        )";

        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new
            {
                p_title = book.Title,
                p_author_id = book.AuthorId,
                p_publish_year = book.PublishYear,
                p_publisher = book.Publisher,
                p_isbn = book.ISBN,
                p_pages = book.Pages,
                p_contents = book.Contents
            },
            cancellationToken: ct));
    }

    public async Task UpdateAsync(Book book, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = @"
        SELECT sp_books_update(
            @p_book_id, @p_title, @p_author_id, @p_publish_year,
            @p_publisher, @p_isbn, @p_pages,
            @p_contents::xml
        )";

        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new
            {
                p_book_id = book.BookId,
                p_title = book.Title,
                p_author_id = book.AuthorId,
                p_publish_year = book.PublishYear,
                p_publisher = book.Publisher,
                p_isbn = book.ISBN,
                p_pages = book.Pages,
                p_contents = book.Contents
            },
            cancellationToken: ct));
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = "SELECT sp_books_delete(@p_book_id)";

        await conn.ExecuteAsync(new CommandDefinition(
            sql,
            new { p_book_id = id },
            cancellationToken: ct));
    }

    public async Task<IEnumerable<Book>> SearchInContentsAsync(string keyword, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = "SELECT * FROM sp_books_search_in_contents(@p_keyword)";

        var rows = await conn.QueryAsync<BookRow>(new CommandDefinition(
            sql, new { p_keyword = keyword }, cancellationToken: ct));

        return rows.Select(ToBook);
    }

    private static Book ToBook(BookRow r)
    {
        var author = Author.Restore(r.author_id, r.author_first_name ?? "", r.author_last_name ?? "");
        return Book.Restore(
            r.book_id, r.title, r.author_id, r.publish_year,
            r.publisher, r.isbn, r.pages, null, author);
    }

    private class BookRow
    {
        public int book_id { get; set; }
        public string title { get; set; } = "";
        public int author_id { get; set; }
        public string? author_first_name { get; set; }
        public string? author_last_name { get; set; }
        public int publish_year { get; set; }
        public string? publisher { get; set; }
        public string? isbn { get; set; }
        public int? pages { get; set; }
    }
}
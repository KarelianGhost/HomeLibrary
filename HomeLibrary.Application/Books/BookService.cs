using HomeLibrary.Application.Books.Dtos;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Domain.Interfaces;

namespace HomeLibrary.Application.Books;

public class BookService
{
    private readonly IBookRepository _repo;

    public BookService(IBookRepository repo) => _repo = repo;

    public async Task<IEnumerable<BookDto>> GetAllAsync(string? search, CancellationToken ct = default)
    {
        var books = await _repo.GetAllAsync(search, ct);
        return books.Select(ToDto);
    }

    public async Task<BookDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var book = await _repo.GetByIdAsync(id, ct);
        return book is null ? null : ToDto(book);
    }

    public async Task<int> CreateAsync(
        string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages, string? contents,
        CancellationToken ct = default)
    {
        var book = new Book(title, authorId, publishYear, publisher, isbn, pages, contents);
        return await _repo.CreateAsync(book, ct);
    }

    public async Task UpdateAsync(
        int bookId, string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages, string? contents,
        CancellationToken ct = default)
    {
        var book = await _repo.GetByIdAsync(bookId, ct)
            ?? throw new InvalidOperationException($"Книга {bookId} не найдена");

        book.Update(title, authorId, publishYear, publisher, isbn, pages, contents);
        await _repo.UpdateAsync(book, ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteAsync(id, ct);

    public async Task<IEnumerable<BookDto>> SearchInContentsAsync(string keyword, CancellationToken ct = default)
    {
        var books = await _repo.SearchInContentsAsync(keyword, ct);
        return books.Select(ToDto);
    }

    private static BookDto ToDto(Book b) => new()
    {
        BookId = b.BookId,
        Title = b.Title,
        AuthorId = b.AuthorId,
        Author = b.Author?.FullName ?? "",
        PublishYear = b.PublishYear,
        Publisher = b.Publisher,
        ISBN = b.ISBN,
        Pages = b.Pages,
        Contents = b.Contents
    };
}
using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Domain.Interfaces;

public interface IBookRepository
{
    Task<IEnumerable<Book>> GetAllAsync(string? search, CancellationToken ct = default);
    Task<Book?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateAsync(Book book, CancellationToken ct = default);
    Task UpdateAsync(Book book, CancellationToken ct = default);
    Task DeleteAsync(int id, CancellationToken ct = default);
    Task<IEnumerable<Book>> SearchInContentsAsync(string keyword, CancellationToken ct = default);
}
using HomeLibrary.Domain.Entities;

namespace HomeLibrary.Domain.Interfaces;

public interface IAuthorRepository
{
    Task<IEnumerable<Author>> GetAllAsync(CancellationToken ct = default);
    Task<int> CreateAsync(Author author, CancellationToken ct = default);
}
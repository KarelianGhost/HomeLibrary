using HomeLibrary.Application.Authors.Dtos;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Domain.Interfaces;

namespace HomeLibrary.Application.Authors;

public class AuthorService
{
    private readonly IAuthorRepository _repo;
    public AuthorService(IAuthorRepository repo) => _repo = repo;

    public async Task<IEnumerable<AuthorDto>> GetAllAsync(CancellationToken ct = default)
    {
        var authors = await _repo.GetAllAsync(ct);
        return authors.Select(a => new AuthorDto
        {
            AuthorId = a.AuthorId,
            FullName = a.FullName
        });
    }

    public async Task<int> CreateAsync(string firstName, string lastName, CancellationToken ct = default)
        => await _repo.CreateAsync(new Author(firstName, lastName), ct);
}
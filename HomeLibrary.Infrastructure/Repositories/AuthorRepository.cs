using Dapper;
using HomeLibrary.Domain.Entities;
using HomeLibrary.Domain.Interfaces;
using Npgsql;

namespace HomeLibrary.Infrastructure.Repositories;

public class AuthorRepository : IAuthorRepository
{
    private readonly string _cs;

    public AuthorRepository(string connectionString)
    {
        _cs = connectionString;
    }

    public async Task<IEnumerable<Author>> GetAllAsync(CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = "SELECT * FROM sp_authors_select()";

        var rows = await conn.QueryAsync<AuthorRow>(
            new CommandDefinition(sql, cancellationToken: ct));

        return rows.Select(ToAuthor);
    }

    public async Task<int> CreateAsync(Author author, CancellationToken ct = default)
    {
        using var conn = new NpgsqlConnection(_cs);

        const string sql = "SELECT sp_authors_insert(@p_first_name, @p_last_name)";

        return await conn.ExecuteScalarAsync<int>(new CommandDefinition(
            sql,
            new
            {
                p_first_name = author.FirstName,
                p_last_name = author.LastName
            },
            cancellationToken: ct));
    }

    private static Author ToAuthor(AuthorRow row)
    {
        var parts = (row.full_name ?? "").Split(' ', 2);
        var lastName = parts.Length > 0 ? parts[0] : "";
        var firstName = parts.Length > 1 ? parts[1] : "";

        return Author.Restore(row.author_id, firstName, lastName);
    }

    private class AuthorRow
    {
        public int author_id { get; set; }
        public string full_name { get; set; } = "";
    }
}
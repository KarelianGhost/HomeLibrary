using System;
using System.Collections.Generic;

namespace HomeLibrary.Domain.Entities;

public class Author
{
    public int AuthorId { get; private set; }
    public string FirstName { get; private set; } = "";
    public string LastName { get; private set; } = "";

    private readonly List<Book> _books = new();
    public IReadOnlyCollection<Book> Books => _books;

    private Author() { }

    public Author(string firstName, string lastName)
    {
        if (string.IsNullOrWhiteSpace(firstName))
            throw new ArgumentException("Имя обязательно", nameof(firstName));
        if (string.IsNullOrWhiteSpace(lastName))
            throw new ArgumentException("Фамилия обязательна", nameof(lastName));
        FirstName = firstName;
        LastName = lastName;
    }

    public static Author Restore(int authorId, string firstName, string lastName)
    {
        var a = new Author(firstName, lastName);
        a.AuthorId = authorId;
        return a;
    }

    public string FullName => $"{LastName} {FirstName}";
}
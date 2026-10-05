using System;
using System.Collections.Generic;

namespace HomeLibrary.Domain.Entities;

public class Book
{
    public int BookId { get; private set; }
    public string Title { get; private set; } = "";
    public int AuthorId { get; private set; }
    public Author? Author { get; private set; }
    public int PublishYear { get; private set; }
    public string? Publisher { get; private set; }
    public string? ISBN { get; private set; }
    public int? Pages { get; private set; }
    public string? Contents { get; private set; }
    public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; private set; }

    private Book() { }

    public Book(
        string title, int authorId, int publishYear,
        string? publisher = null, string? isbn = null,
        int? pages = null, string? contents = null)
    {
        Validate(title, publishYear);
        Title = title;
        AuthorId = authorId;
        PublishYear = publishYear;
        Publisher = publisher;
        ISBN = isbn;
        Pages = pages;
        Contents = contents;
    }

    public static Book Restore(
        int bookId, string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages, string? contents,
        Author? author = null)
    {
        Validate(title, publishYear);
        return new Book
        {
            BookId = bookId,
            Title = title,
            AuthorId = authorId,
            PublishYear = publishYear,
            Publisher = publisher,
            ISBN = isbn,
            Pages = pages,
            Contents = contents,
            Author = author
        };
    }

    public void Update(
        string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages, string? contents)
    {
        Validate(title, publishYear);
        Title = title;
        AuthorId = authorId;
        PublishYear = publishYear;
        Publisher = publisher;
        ISBN = isbn;
        Pages = pages;
        Contents = contents;
        UpdatedAt = DateTime.UtcNow;
    }

    private static void Validate(string title, int publishYear)
    {
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Название обязательно", nameof(title));
        if (publishYear < 1000 || publishYear > 2100)
            throw new ArgumentException("Год издания некорректен", nameof(publishYear));
    }
}
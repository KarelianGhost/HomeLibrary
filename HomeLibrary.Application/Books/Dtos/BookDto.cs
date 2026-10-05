namespace HomeLibrary.Application.Books.Dtos;

public class BookDto
{
    public int BookId { get; set; }
    public string Title { get; set; } = "";
    public int AuthorId { get; set; }
    public string Author { get; set; } = "";
    public int PublishYear { get; set; }
    public string? Publisher { get; set; }
    public string? ISBN { get; set; }
    public int? Pages { get; set; }
    public string? Contents { get; set; }
}
using System.Xml.Linq;
using Ganss.Xss;
using HomeLibrary.Application.Authors;
using HomeLibrary.Application.Books;
using HomeLibrary.Application.Books.Dtos;
using Microsoft.AspNetCore.Mvc;
using HomeLibrary.Web.Helpers;

namespace HomeLibrary.Web.Controllers;

public class BooksController : Controller
{
    private readonly BookService _books;
    private readonly AuthorService _authors;
    private readonly HtmlSanitizer _sanitizer = new();

    public BooksController(BookService books, AuthorService authors)
    {
        _books = books;
        _authors = authors;
    }

    public async Task<IActionResult> Index(string? search, CancellationToken ct)
    {
        var books = await _books.GetAllAsync(search, ct);
        ViewBag.Search = search;
        return View(books);
    }

    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null) return NotFound();

        book.Contents = XmlHelper.ExtractHtml(book.Contents);

        return View(book);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.Authors = await _authors.GetAllAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(
        string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages,
        string? contentsHtml, CancellationToken ct)
    {
        var xml = BuildContentsXml(contentsHtml);

        await _books.CreateAsync(title, authorId, publishYear,
            publisher, isbn, pages, xml, ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var book = await _books.GetByIdAsync(id, ct);
        if (book is null) return NotFound();

        book.Contents = XmlHelper.ExtractHtml(book.Contents);

        ViewBag.Authors = (await _authors.GetAllAsync(ct)).ToList();
        return View(book);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int bookId, string title, int authorId, int publishYear,
        string? publisher, string? isbn, int? pages,
        string? contentsHtml, CancellationToken ct)
    {
        var xml = BuildContentsXml(contentsHtml);

        await _books.UpdateAsync(bookId, title, authorId, publishYear,
            publisher, isbn, pages, xml, ct);

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _books.DeleteAsync(id, ct);
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> SearchInContents(string keyword, CancellationToken ct)
    {
        var books = await _books.SearchInContentsAsync(keyword, ct);
        ViewBag.Keyword = keyword;
        return View("Index", books);
    }

    private string? BuildContentsXml(string? html)
    {
        if (string.IsNullOrWhiteSpace(html))
            return null;

        var clean = _sanitizer.Sanitize(html);

        var doc = new XElement("Contents",
            new XElement("Section", clean));

        return doc.ToString();
    }
}
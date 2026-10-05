using HomeLibrary.Application.Authors;
using HomeLibrary.Application.Books;
using HomeLibrary.Domain.Interfaces;
using HomeLibrary.Infrastructure.Persistence;
using HomeLibrary.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var cs = builder.Configuration.GetConnectionString("Default")!;

// EF Core
builder.Services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(cs));

// Репозитории
builder.Services.AddScoped<IBookRepository>(sp => new BookRepository(
    sp.GetRequiredService<AppDbContext>(), cs));
builder.Services.AddScoped<IAuthorRepository>(_ => new AuthorRepository(cs));

// Сервисы приложения
builder.Services.AddScoped<BookService>();
builder.Services.AddScoped<AuthorService>();

builder.Services.AddControllersWithViews();

var app = builder.Build();

// Автоприменение миграций при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    // Seed
    if (!db.Authors.Any())
    {
        var a = new HomeLibrary.Domain.Entities.Author("Лев", "Толстой");
        db.Authors.Add(a);
        db.SaveChanges();

        db.Books.Add(new HomeLibrary.Domain.Entities.Book(
            "Война и мир", a.AuthorId, 1869, "Русский вестник", null, 1300,
            "<Contents><Section><![CDATA[<h2>Том 1</h2>]]></Section></Contents>"));
        db.SaveChanges();
    }
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Books}/{action=Index}/{id?}");

app.Run();
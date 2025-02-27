using Demo.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.WebApp.Controllers;

public class HomeController(DemoDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var newAuthor = new Author
        {
            Name = "Anduin",
            IsDied = false,
        };
        context.Authors.Add(newAuthor);
        await context.SaveChangesAsync();

        var totalAuthors = await context.Authors.CountAsync();
        var newBook = new Book
        {
            AuthorId = newAuthor.Id,
            Title = "My Book"
        };
        context.Books.Add(newBook);
        await context.SaveChangesAsync();

        var totalBooks = await context.Books.CountAsync();

        return this.Ok(new
        {
            message = "Item Inserted!",
            totalAuthors,
            totalBooks
        });
    }
}

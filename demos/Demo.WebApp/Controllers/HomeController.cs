using Demo.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Demo.WebApp.Controllers;

public class HomeController(DemoDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        context.Authors.Add(new Author
        {
            Name = "Anduin",
            IsDied = false,
        });
        await context.SaveChangesAsync();

        var totalItems = await context.Authors.CountAsync();
        return this.Ok(new
        {
            message = "Item Inserted!",
            totalItems
        });
    }
}

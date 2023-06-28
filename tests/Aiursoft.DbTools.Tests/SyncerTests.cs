using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests
{
    public class Book : ISynchronizable<Book>
    {
        public int Id { get; set; }
        public string? Name { get; set; }

        public bool EqualsInDb(Book obj)
        {
            return Name == obj.Name;
        }

        public Book Map()
        {
            return new Book
            {
                Name = Name
            };
        }
    }

    public class MyDbContext : DbContext
    {
        public DbSet<Book> Books => Set<Book>();

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite($"Data Source=app.db");
    }
    
    [TestClass]
    public class EntityFrameworkExtendsTests
    {
        [TestMethod]
        public async Task TestSync()
        {
            await using var context = new MyDbContext();
            var dbSet = context.Books;

            var collection = new List<Book>
            {
                new() { Name = "Book 1" },
                new() { Name = "Book 2" },
                new() { Name = "Book 3" }
            };

            dbSet.Sync(t => true, collection);
            await context.SaveChangesAsync();
            var results = await context.Books.ToListAsync();

            Assert.AreEqual(3, results.Count);
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 1"));
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 2"));
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 3"));

            collection.RemoveAt(1);
            collection.Add(new Book { Name = "Book 4" });

            dbSet.Sync(t => true, collection);
            await context.SaveChangesAsync();
            results = await context.Books.ToListAsync();

            Assert.AreEqual(3, dbSet.Count());
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 1"));
            Assert.IsNull(results.SingleOrDefault(t => t.Name == "Book 2"));
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 3"));
            Assert.IsNotNull(results.SingleOrDefault(t => t.Name == "Book 4"));
        }
    }
}
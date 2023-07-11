using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Aiursoft.DbTools.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Aiursoft.DbTools.Tests
{
    public class Book : ISynchronizable<Book>
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;

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
        
        [InverseProperty(nameof(Chaptor.Context))]
        public IEnumerable<Chaptor> Chaptors { get; set; }
    }

    public class Chaptor
    {
        [Key]
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        
        public int ContextId { get; set; }
        [ForeignKey(nameof(ContextId))]
        public Book? Context { get; set; }
    }

    public class MyDbContext : DbContext
    {
        public MyDbContext(DbContextOptions<MyDbContext> options) : base(options)
        {
        }

        public DbSet<Book> Books => Set<Book>();
        
        public DbSet<Chaptor> Chaptors => Set<Chaptor>();
    }
    
    [TestClass]
    public class EntityFrameworkExtendsTests
    {
        [TestMethod]
        public async Task TestSync()
        {
            // Arrange
            var services = new ServiceCollection();
            services.AddLogging();
            services.AddAiurSqliteWithCache<MyDbContext>("Data Source=app.db");
            var built = services.BuildServiceProvider();
            var context = built.GetRequiredService<MyDbContext>();
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
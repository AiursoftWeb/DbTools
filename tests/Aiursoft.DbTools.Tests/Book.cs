using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.DbTools.Tests
{
    public class Book : ISynchronizable<Book>
    {
        public int Id { get; set; }

        [MaxLength(100)]
        public string Name { get; init; } = string.Empty;

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

        [InverseProperty(nameof(Chapter.Context))]
        public IEnumerable<Chapter>? Chapters { get; set; }
    }
}

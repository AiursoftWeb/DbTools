using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Demo.Entities;

public class Author
{
    [Key]
    public int Id { get; set; }
    public bool IsDied { get; set; }

    [InverseProperty(nameof(Book.Author))]
    public IEnumerable<Book> Books { get; init; } = new List<Book>();

    [StringLength(100)]
    public required string Name { get; init; }
}
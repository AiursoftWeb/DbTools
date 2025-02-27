using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;

namespace Demo.Entities;

public class Book
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public required string Title { get; init; }

    public required int AuthorId { get; init; }
    [ForeignKey(nameof(AuthorId))]
    [NotNull]
    public Author? Author { get; init; }
}
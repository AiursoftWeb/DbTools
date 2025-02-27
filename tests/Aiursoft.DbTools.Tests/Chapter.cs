using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Aiursoft.DbTools.Tests;

public class Chapter
{
    [Key]
    public int Id { get; set; }

    [MaxLength(100)]
    public string Name { get; init; } = string.Empty;

    public int ContextId { get; set; }
    [ForeignKey(nameof(ContextId))]
    public Book? Context { get; set; }
}
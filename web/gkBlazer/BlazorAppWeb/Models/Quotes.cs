using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BlazorAppWeb.Models;

public class Quotes
{
  public int Id { get; set; }

  [Required]
  [StringLength(50)]
  public string? Author { get; set; }

  [Required]
  [StringLength(500, MinimumLength = 3)]
  public string? Quote { get; set; }

  [Required]
  public DateOnly LastUsedDate { get; set; }

  [Required]
  public int Count { get; set; }

}

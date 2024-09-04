using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Quotes.Models;

public class Quote
{
  public int Id { get; set; }

  public string? QuoteText { get; set; }

  public DateOnly LastUsedDate { get; set; }

  public string? Author { get; set; }
}

using System;

namespace gkweb.api.Types;

public class Staff
{

  public Staff() { }

  public int Id { get; set; }
  public string? Title { get; set; }
  public string? Summary { get; set; }
  public string? ImageUrl { get; set; }
}
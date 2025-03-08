class Quote {
  public int Id { get; set; }
  public string Author { get; set; }
  public string QuoteText { get; set; }
  public int UsedCount { get; set; }
  public DateTime LastUsed { get; set; }
  public DateTime Created { get; set; }
}
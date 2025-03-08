
using Microsoft.Data.SqlClient;

internal static class DataManager {
  private static T SafeGet<T>(this SqlDataReader reader, string nameOfColumn) {
    var indexOfColumn = reader.GetOrdinal(nameOfColumn);
#pragma warning disable CS8603 // Possible null reference return.
    return reader.IsDBNull(indexOfColumn) ? default(T) : reader.GetFieldValue<T>(indexOfColumn);
#pragma warning restore CS8603 // Possible null reference return.
  }

  internal static async Task<Quote> RandomQuote() {
    var rtn = new Quote();
    var quoteList = await QuoteListAsync(true);
    var random = new Random();
    int index = random.Next(quoteList.Count);
    rtn = quoteList[index];
    await SaveQuoteAsync(rtn);
    return rtn;
  }

  internal static async Task<List<Quote>> QuoteListAsync(bool filter = false) {
    var rtn = new List<Quote>();
    await using var connection = sqlHelper.sqlConnection();
    await connection.OpenAsync();

    var sql = "SELECT [Id],[Author],[Quote],[UsedCount],[UsedDate],[CreateDate] FROM dbo.LiveQuotes {0};".FormatWith( !filter ? string.Empty : "WHERE [UsedCount] = (SELECT MIN([UsedCount]) FROM dbo.LiveQuotes)");
    await using var command = new SqlCommand(sql, connection);
    var reader = command.ExecuteReader();
    var quote = new Quote();
    while(reader.Read()) {
      quote = new Quote() {
        Id = SafeGet<int>(reader, "Id"),
        Author = SafeGet<string>(reader, "Author"),
        QuoteText = SafeGet<string>(reader, "Quote"),
        UsedCount = SafeGet<int>(reader, "UsedCount"), 
        LastUsed = SafeGet<DateTime>(reader, "UsedDate"), 
        Created = SafeGet<DateTime>(reader, "CreateDate")
      };
      rtn.Add(quote);
    }
    return rtn;
  }

  internal static async Task SaveQuoteAsync(Quote quote) {
    await using var connection = sqlHelper.sqlConnection();
    await connection.OpenAsync();
    var sql = string.Empty;
    if(quote.Id == 0){
      sql = "INSERT INTO dbo.LiveQuotes (Author, Quote, UsedCount) VALUES ('{0}', '{1}', 0)".FormatWith(quote.Author.FixSqlString(), quote.QuoteText.FixSqlString());
    } else {
      sql = "UPDATE dbo.LiveQuotes SET Author = '{0}', Quote = '{1}', UsedCount = {2}, UsedDate = GETDATE() WHERE Id = {3};"
        .FormatWith(quote.Author.FixSqlString(), quote.QuoteText.FixSqlString(), (quote.UsedCount + 1).ToString(), quote.Id);
    } 
    await using var command = new SqlCommand(sql, connection);
    command.ExecuteNonQuery();
  }
}
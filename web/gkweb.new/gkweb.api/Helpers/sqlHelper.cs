using Microsoft.Data.SqlClient;

internal class SqlHelper {
  public static SqlConnection GetSqlConnection()
  {
    var builder = new SqlConnectionStringBuilder
    {
      DataSource = "glorykidd.com",
      UserID = "glorykiddUser",
      Password = "pUg8NA4c2AqYJXfwhENK",
      InitialCatalog = "glorykidd",
      TrustServerCertificate = true
    };
    return new SqlConnection(builder.ConnectionString);
  }
}
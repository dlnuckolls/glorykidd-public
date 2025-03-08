using Microsoft.Data.SqlClient;

internal class sqlHelper {
  public static SqlConnection sqlConnection() {
    var builder = new SqlConnectionStringBuilder {
      DataSource = "glorykidd.com",
      UserID = "quoteUser",
      Password = "Nzj4Jxv9Dxf^mauX6Uo#",
      InitialCatalog = "Quotes",
      TrustServerCertificate = true
    };
    return new SqlConnection(builder.ConnectionString);
  }
}
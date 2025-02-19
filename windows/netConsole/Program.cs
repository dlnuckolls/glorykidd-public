using Microsoft.Data.SqlClient;

namespace netConsole
{
  class Program
  {
    static async Task Main(string[] args)
    {
      var builder = new SqlConnectionStringBuilder
      {
        DataSource = "glorykidd.com",
        UserID = "liquibaseUser",
        Password = "liquibaseUserPassword",
        InitialCatalog = "liquibaseBase",
        TrustServerCertificate = true
      };

      var connectionString = builder.ConnectionString;

      try
      {
        await using var connection = new SqlConnection(connectionString);
        Console.WriteLine("\nQuery data example:");
        Console.WriteLine("=========================================\n");

        await connection.OpenAsync();

        var sql = "SELECT name, collation_name FROM sys.databases";
        await using var command = new SqlCommand(sql, connection);
        await using var reader = await command.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
          Console.WriteLine("{0} {1}", reader.GetString(0), reader.GetString(1));
        }
      }
      catch (Exception e)
      {
        Console.WriteLine(e.ToString());
      }

      Console.WriteLine("\nDone. Press enter.");
      Console.ReadLine();
    }
  }
}
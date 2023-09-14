using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace GloryKidd.WebCore.Helpers {
  public static class SqlHelpers {
    private static readonly string ConnectionString = EncryptionHelper.Decrypt(ConfigurationManager.ConnectionStrings["directory"].ToString());

    #region SELECT
    /// <summary>
    /// Simple select statement that creates a DataTable based on the query.  
    /// </summary>
    /// <param name="query"></param>
    /// <param name="whereClause"></param>
    /// <returns>Query matching DataTable</returns>
    public static DataTable Select(string query, string whereClause = "") {
      string testQuery = "{0} {1};".FormatWith(query, whereClause);
      return GetDataTable(testQuery);
    }

    /// <summary>
    /// Create DataTable from provided tablename that contains columns from column array. Filtered by optional where clause. 
    /// </summary>
    /// <param name="tablename"></param>
    /// <param name="columns"></param>
    /// <param name="whereClause"></param>
    /// <returns></returns>
    public static DataTable SelectByColumns(string tablename, string[] columns, string whereClause = "") {
      string query = "SELECT {0} FROM {1} {2};".FormatWith(columns.JoinWith(","), tablename, whereClause);
      return GetDataTable(query);
    }

    /// <summary>
    /// Executes the SQL query and returns the first value
    /// </summary>
    /// <param name="query">The CMD text.</param>
    /// <returns>Object containing value returned from query.</returns>
    public static object SelectScalar(string query) {
      using(var conn = new SqlConnection(ConnectionString)) {
        var command = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, };
        conn.Open();
        return command.ExecuteScalar();
      }
    }
    #endregion SELECT

    #region UPDATE
    /// <summary>
    /// Executes the SQL query and returns true or false for success
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static bool Update(string query) {
      using(var conn = new SqlConnection(ConnectionString)) {
        var command = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, };
        conn.Open();
        return command.ExecuteNonQuery() > 0;
      }
    }
    /// <summary>
    /// Execute a batch of SQL queries as a transaction and returns true or false for success
    /// </summary>
    /// <param name="queries"></param>
    /// <returns></returns>
    public static bool UpdateAll(IEnumerable<string> queries) {
      using(var conn = new SqlConnection(ConnectionString)) {
        conn.Open();
        using(var trans = conn.BeginTransaction()) {
          foreach(var query in queries) {
            try {
              var command = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, Transaction = trans };
              if(!(command.ExecuteNonQuery() > 0)) { throw new ApplicationException(); }
            } catch { trans.Rollback(); return false; }
          }
          // all successfull
          trans.Commit();
          return true;
        }
      }
    }
    #endregion UPDATE

    #region INSERT
    /// <summary>
    /// Executes SQL query and returns true or false
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static bool Insert(string query) {
      return RunInsertQuery(query) > 0;
    }
    /// <summary>
    /// Executes SQL query and returns the first value
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    public static string InsertScalar(string query) {
      //query = query + ";SELECT SCOPE_IDENTITY();";
      return RunInsertQueryScalar(query);
    }
    /// <summary>
    /// Execute the SQL query and returns the number of affected rows
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private static int RunInsertQuery(string query) {
      using(var conn = new SqlConnection(ConnectionString)) {
        var insertCommand = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, };
        conn.Open();
        return insertCommand.ExecuteNonQuery();
      }
    }
    /// <summary>
    /// Executes the SQL query and returns the first value
    /// </summary>
    /// <param name="query"></param>
    /// <returns></returns>
    private static string RunInsertQueryScalar(string query) {
      using(var conn = new SqlConnection(ConnectionString)) {
        var insertCommand = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, };
        conn.Open();
        var id = insertCommand.ExecuteScalar();
        return id.ToString();
      }
    }
    #endregion INSERT

    #region DELETE

    #endregion DELETE

    #region Connection
    /// <summary>
    /// Retrieve data table from current environment database. 
    /// </summary>
    /// <param name="query"></param>
    /// <returns>DataTable</returns>
    private static DataTable GetDataTable(string query) {
      DataTable result = new DataTable();
      using(var conn = new SqlConnection(ConnectionString)) {
        var selectCommand = new SqlCommand { Connection = conn, CommandText = query, CommandType = CommandType.Text, };
        conn.Open();
        using(var reader = selectCommand.ExecuteReader()) { result.Load(reader); }
      }
      return result;
    }
    #endregion Connection

  }
}
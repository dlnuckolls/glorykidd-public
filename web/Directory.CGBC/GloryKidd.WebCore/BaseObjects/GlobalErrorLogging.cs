using GloryKidd.WebCore.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GloryKidd.WebCore.BaseObjects {
  public static class GlobalErrorLogging {
    public static void LogError(string module, Exception error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.Message.FixSqlString(), error.StackTrace.FixSqlString()));
    }

    public static void LogError(string module, string error) {
      SqlHelpers.Insert(SqlStatements.SQL_LOG_EXCEPTION.FormatWith(DateTime.Now.ConvertSqlDateTime(), module.FixSqlString(), error.FixSqlString(), string.Empty));
    }
  }
}

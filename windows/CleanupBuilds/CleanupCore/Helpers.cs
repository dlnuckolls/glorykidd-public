using System;
using System.Text;

namespace CleanupCore;

public class Helpers {
  public static List<string> runtimeLogData = new List<string>();
  public static void CheckBuildFiles(DirectoryInfo parent, string filter = "*.*")
  {
    // Get the files
    FileInfo[] files = parent.GetFiles(filter);
    // Sort by creation-time descending 
    Array.Sort(files, delegate (FileInfo f1, FileInfo f2) {
      return f2.CreationTime.CompareTo(f1.CreationTime);
    });
    if (files.Length > 2) {
      for (var f = 2; files.Length > f; f++) {
        LogMessages(string.Format("|--- {0}", files[f].Name));
        File.Delete(files[f].FullName);
      }
    }
  }

  public static string FormatRunData() {
    var rtn = new StringBuilder();
    runtimeLogData.ForEach(l => { rtn.AppendLine(l); });
    return rtn.ToString();
  }

  public static void LogAllMessages(string logFile) {
    var runlog = FormatRunData();
    File.WriteAllText(logFile, runlog);
  }

  public static void LogMessages(string message) {
    runtimeLogData.Add("Info: {0}".FormatWith(message));
  }

  public static void LogMessages(Exception ex) {
    LogErrorMessages(ex.Message);
  }

  public static void LogErrorMessages(string message) {
    runtimeLogData.Add("Info: {0}".FormatWith(message));
  }
}

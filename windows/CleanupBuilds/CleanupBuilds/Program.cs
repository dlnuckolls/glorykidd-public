using System;
using System.IO;
using System.Text;
using System.Collections.Generic;
using System.Diagnostics;

namespace CleanupBuilds {
  class Program {
    private static EventLog logger;
    private static List<string> runtimeLogData = new List<string>();

    static void Main(string[] args) {
      try {
        if (logger == null) logger = new EventLog("Application") { Source = "GK.Cleanup.Service" };
        LogMessages("Starting Run");
        var baseFolder = @"C:\backups";
        //Cleanup backups
        LogMessages("Processing Backups");
        LogMessages("|- Removing files for CGCA");
        CheckBuildFiles(new DirectoryInfo(baseFolder), "cgca*");
        LogMessages("|- Removing files for CGBC");
        CheckBuildFiles(new DirectoryInfo(baseFolder), "cgbc*");
        LogMessages("|- Removing files for TurleyRichards");
        CheckBuildFiles(new DirectoryInfo(baseFolder), "trich*");
        //Cleanup logs
        LogMessages("Processing IIS Logs");
        baseFolder = @"C:\inetpub\logs\LogFiles";
        var iisfolders = new DirectoryInfo(baseFolder).GetDirectories();
        for (int f = 0; iisfolders.Length > f; f++) {
          LogMessages("|- Removing files for {0}".FormatWith(iisfolders[f].Name));
          CheckBuildFiles(new DirectoryInfo(iisfolders[f].FullName));
        }
        LogMessages("Completed Run");
        LogAllMessages();
      } catch (Exception e) {
        LogMessages(e);
      }
    }

    static void CheckBuildFiles(DirectoryInfo parent, string filter = "*.*") {
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

    static string FormatRunData() {
      var rtn = new StringBuilder();
      runtimeLogData.ForEach(l => { rtn.AppendLine(l); });
      return rtn.ToString();
    }
    static void LogAllMessages() {
      var runlog = FormatRunData();
      if (runlog.Length <= 32000) {
        logger.WriteEntry(runlog, EventLogEntryType.Information);
      } else {
        var parts = runlog.SplitBy(32000);
        parts.Reverse();
        parts.ForEach(p => {
          logger.WriteEntry(p, EventLogEntryType.Information);
        });
      }
      Console.WriteLine("Info: {0}".FormatWith(runlog));
    }

    static void LogMessages(string message) {
      //logger.WriteEntry(message, EventLogEntryType.Information);
      runtimeLogData.Add("Info: {0}".FormatWith(message));
    }
    static void LogMessages(Exception ex) {
      LogErrorMessages(ex.Message);
    }
    static void LogErrorMessages(string message) {
      logger.WriteEntry(message, EventLogEntryType.Error);
      Console.WriteLine("Error: {0}".FormatWith(message));
    }
  }
}

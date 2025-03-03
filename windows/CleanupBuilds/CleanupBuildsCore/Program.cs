using Microsoft.Extensions.Configuration;
using System.Diagnostics;
using System.Text;

class Program {

  private static EventLog? logger;
  private static List<string> runtimeLogData = [];

  static void Main(string[] args) {
    try {
      var builder = new ConfigurationBuilder().AddJsonFile($"appsettings.json", true, true);
      var config = builder.Build();

      if(logger == null) logger = new EventLog("Application") { Source = "GK.Cleanup.Service" };
      LogMessages("Starting Run");
      //var s =
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      var baseFolder = config["AppSettings:BackupFolder"].ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
      //Cleanup backups
      LogMessages("Processing Backups");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      var appsToProcess = config["AppSettings:CleanupApps"].ToString().Split('|').ToList();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
      appsToProcess.ForEach(a => {
        var app = a.Split(':');
        LogMessages($"|- Removing files for {app[0]}");
        CheckBuildFiles(new DirectoryInfo(baseFolder), $"{app[1]}*");
      });
      //Cleanup logs
      LogMessages("Processing IIS Logs");
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      baseFolder = config["AppSettings:IISLogFolder"].ToString();
#pragma warning restore CS8602 // Dereference of a possibly null reference.
      var iisfolders = new DirectoryInfo(baseFolder).GetDirectories();
      for(int f = 0; iisfolders.Length > f; f++) {
        LogMessages("|- Removing files for {0}".FormatWith(iisfolders[f].Name));
        CheckBuildFiles(new DirectoryInfo(iisfolders[f].FullName));
      }
      LogMessages("Completed Run");
      LogAllMessages();
    } catch(Exception e) {
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
    if(files.Length > 2) {
      for(var f = 2; files.Length > f; f++) {
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
    if(runlog.Length <= 32000) {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
      logger.WriteEntry(runlog, EventLogEntryType.Information);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    } else {
      var parts = runlog.SplitBy(32000);
      parts.Reverse();
      parts.ForEach(p => {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
        logger.WriteEntry(p, EventLogEntryType.Information);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
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
#pragma warning disable CS8602 // Dereference of a possibly null reference.
    logger.WriteEntry(message, EventLogEntryType.Error);
#pragma warning restore CS8602 // Dereference of a possibly null reference.
    Console.WriteLine("Error: {0}".FormatWith(message));
  }
}

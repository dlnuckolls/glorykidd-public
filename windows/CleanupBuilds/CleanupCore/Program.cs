using CleanupCore;
using Microsoft.Extensions.Configuration;

var builder = new ConfigurationBuilder();
builder.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
IConfigurationRoot configuration = builder.Build();
var appSettings = configuration.GetSection("AppSettings");

try
{
  if(appSettings.IsNullOrEmpty()) throw new ApplicationException("Not able to read configuration details");

  Helpers.LogMessages("Starting Run");
  var baseFolder = appSettings["BackupFolder"];
  
  //Cleanup backups
  Helpers.LogMessages("Processing Backups");
  string[] itemElements = new string[2];
  appSettings["CleanupApps"].Split('|').ToList().ForEach(item =>
  {
    itemElements = [item.Split(':')[0], item.Split(':')[1]];
    Helpers.LogMessages("|- Removing files for {0}".FormatWith(itemElements[0]));
    Helpers.CheckBuildFiles(new DirectoryInfo(baseFolder), "{0}*".FormatWith(itemElements[1]));
  });

  //Cleanup logs
  Helpers.LogMessages("Processing IIS Logs");
  baseFolder = appSettings["IISLogFolder"];
  var iisfolders = new DirectoryInfo(baseFolder).GetDirectories();
  for (int f = 0; iisfolders.Length > f; f++)
  {
    Helpers.LogMessages("|- Removing files for {0}".FormatWith(iisfolders[f].Name));
    Helpers.CheckBuildFiles(new DirectoryInfo(iisfolders[f].FullName));
  }

  Helpers.LogMessages("Completed Run");
}
catch (Exception e)
{
  Helpers.LogMessages(e);
}

Helpers.LogAllMessages(appSettings["Logfile"]);

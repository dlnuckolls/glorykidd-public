using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Kindred.Common.Logging;
using System.Configuration;

namespace Kindred.Knect.ITAT.LexmarkImaging
{
	class Program 
	{
		private ILog _log;

		static int Main(string[] args)
		{
			return (new Program()).Run(args);
		}

		private Program()
		{
			_log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
		}


		public int Run(string[] args)
		{
			int rtn = (int)ReturnCode.Unknown;
			string command = System.Environment.CommandLine;

			if (args.Length == 0)
			{
				rtn = (int)ReturnCode.NoActionSpecified;
			}
			else
			{
				switch (args[0].ToUpperInvariant())
				{
					case "VALIDATEMANAGEDITEMNUMBER":
						{
							Guid systemId = Guid.Empty;
							try
							{
								systemId = new Guid(args[1]);
							}
							catch (FormatException)
							{
								_log.Error(string.Format("Invalid systemId: \"{0}\".", args[1]));
								throw;
							}
							string managedItemNumber = args[2];

							try
							{
								ItatServices.Imaging svc = new ItatServices.Imaging();
								svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
								svc.Url = ConfigurationManager.AppSettings[string.Format("{0}.{1}", Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine), svc.GetType())];

								bool numberFound = svc.ValidateManagedItemNumber(systemId, managedItemNumber);
								rtn = (numberFound ? (int)ReturnCode.Success : (int)ReturnCode.ManagedItemNumberDoesNotExist);
								_log.Debug(string.Format("ValidateManagedItem(\"{0}\", \"{1}\") returned \"{2}\".", systemId, managedItemNumber, (ReturnCode)rtn));
							}
							catch (System.Web.Services.Protocols.SoapException ex)
							{
								_log.Error(ex.Message, ex);
							}
						}
						break;
					case "STOREDOCUMENT":
						{
							Guid systemId = Guid.Empty;
							try
							{
								systemId = new Guid(args[1]);
							}
							catch (FormatException)
							{
								_log.Error(string.Format("Invalid systemId: \"{0}\".", args[1]));
								throw;
							}
							string managedItemNumber = args[2];
							string filePath = args[3];

							try
							{
								ItatServices.Imaging svc = new ItatServices.Imaging();
								svc.Credentials = System.Net.CredentialCache.DefaultCredentials;
								string urlKey = string.Format("{0}.{1}", Utility.EnvironmentHelper.GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode.Machine), svc.GetType());
								string url = ConfigurationManager.AppSettings[urlKey];
								svc.Url = url;
								string docId = svc.StoreDocument(systemId, managedItemNumber, filePath);
								if (string.IsNullOrEmpty(docId))
								{
									rtn = (int)ReturnCode.Unknown;
								}
								else
								{
									_log.Debug(string.Format("StoreDocument(\"{0}\", \"{1}\", \"{2}\") returned \"{3}\".", systemId, managedItemNumber, filePath, docId));
									rtn = (int)ReturnCode.Success;
								}
							}
							catch (System.Web.Services.Protocols.SoapException ex)
							{
								_log.Error(ex.Message, ex);
							}
						}
						break;
					default:
						rtn = (int)ReturnCode.UnknownActionSpecified;
						break;
				}
			}

			if (rtn < 0)
				_log.Error(string.Format("ERROR: {0}. Command-line: {1}", ((ReturnCode)rtn), System.Environment.CommandLine));
			else
				_log.Debug("Program completed.  Return Code = " + rtn.ToString());
			return rtn;
		}


	}
}

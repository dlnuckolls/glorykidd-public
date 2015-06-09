using System;
using System.Security.Principal;

namespace Kindred.Knect.ITAT.Utility
{

	public static class EnvironmentHelper
	{

		public static string GetEnvironment(Kindred.Common.WebServices.EnvironmentDetectionMode mode)
		{
			switch (mode)
			{
				case Kindred.Common.WebServices.EnvironmentDetectionMode.Machine:
					{
						return GetEnvironmentByMachine();
					}
				case Kindred.Common.WebServices.EnvironmentDetectionMode.User:
					{
						return GetEnvironmentByUser();
					}
			}
			throw new Exception(string.Format("Unrecognized environment detection mode ({0}).", mode));
		}


		private static string GetEnvironmentByMachine()
		{
			string hostName = System.Net.Dns.GetHostEntry("localhost").HostName.ToUpper();
			if (hostName.StartsWith("D"))
			{
				return "LOCAL";
			}
			if (hostName.IndexOf(".PROD.") > -1)
			{
				return "PROD";
			}
			if (hostName.IndexOf(".STAGE.") > -1)
			{
				return "STAGE";
			}
			if (hostName.IndexOf(".T1.") > -1)
			{
				return "T1";
			}

			throw new Exception(string.Format("Unable to determine environment from machine account ({0}).", hostName));
		}

		private static string GetEnvironmentByUser()
		{
			if (System.Net.Dns.GetHostName().ToUpper().StartsWith("D"))
			{
				return "LOCAL";
			}

			string domainName = Kindred.Common.Data.ADHelper.CurrentDomainName;
			if (!string.IsNullOrEmpty(domainName))
			{
				return domainName.ToUpper();
			}

			throw new Exception(string.Format("Unable to determine environment from user account ({0}).", WindowsIdentity.GetCurrent().Name));
		}

	}
}

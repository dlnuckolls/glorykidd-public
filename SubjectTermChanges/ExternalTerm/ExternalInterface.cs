using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using Kindred.Knect.ITAT.ITATInterfaces;
using Kindred.Knect.ITAT.Utility;
using Kindred.Common.WebServices;

namespace Kindred.Knect.ITAT.ExternalTerm
{
	public class ExternalInterface
	{
		#region private fields

		private string _name;
		private string _webServiceURL;

		#endregion


		#region properties

		public string Name
		{
			get { return _name; }
		}

		public string WebServiceURL
		{
			get { return _webServiceURL; }
		}

		#endregion


		#region constructors

		public ExternalInterface(string name, string webServiceURL)
		{
			if (string.IsNullOrEmpty(webServiceURL))
				throw new Exception("Web Service URL not specified for External Term");
			string environment = EnvironmentHelper.GetEnvironment(EnvironmentDetectionMode.Machine);
            _webServiceURL = System.Configuration.ConfigurationManager.AppSettings[environment + ".ExternalTermWebServer"] + webServiceURL.TrimStart('/');
			_name = name;
		}

		#endregion


		#region public methods

		public string[][] GetFilterCatalog()
		{
			return GetWebService().GetFilterCatalog();
		}

		public string[][] GetFilterList(string filterName, string[] allowedValues)
		{
			return GetWebService().GetFilterList(filterName, allowedValues);
		}

		public DataSet GetFilteredData(string[][] filterCriteria)
		{
			return GetWebService().GetFilteredData(filterCriteria);
		}

		public string[] GetAvailableColumns()
		{
			return GetWebService().GetAvailableColumns();
		}

		#endregion


		#region private methods

		private Utility.ItatExternalTerm.ItatExternalTerm GetWebService()
		{
			return Kindred.Common.WebServices.WebServiceFactory<Utility.ItatExternalTerm.ItatExternalTerm>.CreateService(_webServiceURL);
		}

		#endregion


	}
}

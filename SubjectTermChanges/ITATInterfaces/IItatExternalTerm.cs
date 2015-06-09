using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using System.Web.Services;

namespace Kindred.Knect.ITAT.ITATInterfaces
{

	[WebServiceBinding(Name = "IItatExternalTerm", Namespace = "http://kindredhealthcare.com")]
	public interface IItatExternalTerm
	{

		[WebMethod(Description = "Returns a list of the various filters supported by this web service, and the type of each filter (\"List\", \"Text\").")]
		string[][] GetFilterCatalog();

		[WebMethod(Description = "Returns a collection of KeyValue pairs for the given filter; could be used to populate a List control.  If \"allowedValues\" contain entries, then only those key values that appear in that list will be returned. ")]
		string[][] GetFilterList(string filterName, string[] allowedValues);

		[WebMethod(Description = "Returns a resultset of data satisfying the specifed filter criteria.")]
		DataSet GetFilteredData(string[][] filterCriteria);

		[WebMethod(Description = "Returns a list of the columns that can be returned when calling GetFilteredData.")]
		string[] GetAvailableColumns();

	}


}

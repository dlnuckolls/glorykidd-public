using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kindred.Knect.ITAT.LexmarkImaging
{
	public enum ReturnCode
	{
		NoActionSpecified = -100,
		UnknownActionSpecified = -101,
		InvalidSystemId = -102,
		MultipleManagedItemsFound = -103,
		UnableToStoreDocument = -104,
		DocumentNotFoundOnFileSystem = -105,
		UnableToDeleteDocumentFromFileSystem = -106,
		EmptyDocumentFound = -107,
		ScriptTimeout = -108,		//Note - Hard-coded in the Lexmark script
		UnableToCreateLogger = -201,
		UnknownError = -999,
		ManagedItemNumberDoesNotExist = -1,
		Unknown = 0,
		Success = 1

	}
}

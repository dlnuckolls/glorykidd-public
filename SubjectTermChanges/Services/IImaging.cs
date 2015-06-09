using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Kindred.Knect.ITAT.Services
{
	[ServiceContract]
	public interface IImaging
	{

		[OperationContract]
		bool ValidateManagedItemNumber(Guid systemId, string managedItemNumber);

		/// <summary>
		/// Stores the document located at <paramref name="filePath"/> into the document storage repository.
		/// Returns the DocumentId if the operation is successful and null otherwise.
		/// </summary>
		/// <param name="systemId"></param>
		/// <param name="managedItemNumber"></param>
		/// <param name="filePath"></param>
		/// <returns>
		/// A string containing the DocumentId for the document if the operation is successful.
		/// If the document storage operation fails, null is returned.
		/// </returns>
		[OperationContract]
		string StoreDocument(Guid systemId, string managedItemNumber, string filePath);

	}


}

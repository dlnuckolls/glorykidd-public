using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;

namespace Kindred.Knect.ITAT.Web
{


	/// <summary>
	/// Enum to indicate the status of a multi-postbask delete process:
	///   The first postback identifies if there are any conflists of deleting the item.   If not, the delete is performed.
	///    If there are conflicts, the user is prompted with a message showing the conflicts and is given the option to cancel the delete.
	///    If the user verifies that they want to proceed with the delete, the 2nd postback actually performs the delete.
	/// </summary>
	public enum DeleteStatus
	{
		None = 0,
		Prompted,
		Cancelled,
		Verified
	}


	/// <summary>
	/// Enum to indicate whether the current item is being added or edited
	/// </summary>
	public enum EditMode
	{
		None = 0,
		Add,
		Edit
	}


}

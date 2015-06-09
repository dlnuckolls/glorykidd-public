using System;
using System.Web.UI.WebControls;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemProfilePanel : System.Web.UI.UserControl
	{
		private Business.TermGroup _termGroup;

		public bool IsActiveTermGroup { get; set; }

		public Table TermsTable
		{
			get { return tblTerms; }
		}

		public Business.TermGroup TermGroup
		{
			get
			{
				return _termGroup;
			}
			set 
			{
				_termGroup = value;
				ID = ManagedItemProfilePanel.TermGroupContainerId(_termGroup);
			}
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			pnlContainer.Style["display"] = (IsActiveTermGroup ? "block" : "none");
			pnlContainer.Attributes["TermGroupId"] = TermGroup.ID.ToString();
		}

		public static string TermGroupContainerId(Business.TermGroup tg)
		{
			return string.Format("mipp_{0}", tg.Name.Replace(' ', '_'));
		}

	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Kindred.Knect.ITAT.Web
{
	public partial class ManagedItemRollbackConfirmation : BaseManagedItemPage
	{
        public enum RollbackOption
        {
            DoNotCreateOrphan,
            CreateOrphan
        }

        internal override System.Web.UI.HtmlControls.HtmlGenericControl HTMLBody()
        {
            return htmlBody;
        }

        internal override Control ResizablePanel()
        {
            return null;
        }

        protected override ManagedItemHeader HeaderControl()
        {
            return null;
        }

		protected void Page_Load(object sender, EventArgs e)
		{
            if (!IsPostBack)
            {
                btnCancel.Visible = true;
                btnOK.Visible = true;

                string qsValue = Request.QueryString[Common.Names._QS_MANAGED_ITEM_AUDIT_ID];
                if (string.IsNullOrEmpty(qsValue))
                    throw new Exception("ManagedItemAudit ID not found.");

                Guid managedItemAuditId = new Guid(qsValue);
                if (_managedItem.CouldBeOrphaned(managedItemAuditId))
                {
                    btnRetroOrphan.Visible = true;
                    btnOK.Text = "Do Not Create an Orphan";
                    btnOK.Width = Unit.Pixel(150);
                }
            }
		}

        protected void btnCancelOnCommand(object sender, CommandEventArgs e)
		{
            CloseDialog(string.Empty);
		}

        protected void btnOKOnCommand(object sender, CommandEventArgs e)
        {
            CloseDialog(RollbackOption.DoNotCreateOrphan.ToString());
        }

        protected void btnRetroOrphanOnCommand(object sender, CommandEventArgs e)
		{
            CloseDialog(RollbackOption.CreateOrphan.ToString());
        }

	}
}

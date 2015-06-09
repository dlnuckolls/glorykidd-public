using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kindred.Common.Controls;

namespace Kindred.Knect.ITAT.Web.Controls
{
    public partial class GridMoveRows : System.Web.UI.UserControl
    {


        private string _targetControl;




        public string TargetControl
        {
            get
            {
                return _targetControl;
            }
            set
            {
                _targetControl = value;
            }

        }


        

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                //Set Target Control value to use on the client side 
                
                hfTargetControl.Value = _targetControl;

            }



        }


        protected void Page_PreRender(object sender, EventArgs e)
        {
            ////GridView that is attached to this control
            GridView gv = Page.FindControl(TargetControl) as KindredGridView;

            if (gv != null)
            {
                foreach (GridViewRow gvr in gv.Rows)
                {
                    gvr.Attributes["onclick"] += "SelectRow(this);";
                    gvr.Attributes["ondblClick"] += "SelectRow(this);";
                }
            }
        }
    }
}
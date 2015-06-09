using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.ComponentModel;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
 

namespace Kindred.Knect.ITAT.Web
{
    /// <summary>
    /// Kindred.Knect.ITAT.Web.Controls.ValidateOnSave
    /// Created By RichaL02 4/3/2008
    /// Copyright 2008, Kindred Healthcare Inc
    /// 
    /// This control encapsulates the ValidateOnSave functions for terms. 
    /// </summary>
    /// Created by Larry Richardson LRR 4/3/2008
    [ToolboxData("<{0}:ValidateOnSave runat=server id=vos></{0}:ValidateOnSave>")]
    public partial class ValidateOnSave : BaseProfileControl
    {
        #region Fields



        #endregion

        #region Properties
        [DefaultValue(false)]
        public bool Validate
        {
            get
            {
                //object validateOnSave = ViewState["ValidateOnSave"];
                //if (validateOnSave != null)
                //    return (bool)validateOnSave;
                //else
                //    return false;
                return this.chkValidateOnSave.Checked;
            }
            set
            {
                ViewState["ValidateOnSave"] = value;
            }
        }


        [DefaultValue(null)]
        public Business.ITATSystem ITATSystem
        {
            get
            {
                object iTATSystem = ViewState["ITATSystem"];
                if (iTATSystem != null)
                    return (Business.ITATSystem)iTATSystem;
                else
                    return null;
            }
            set
            {
                ViewState["ITATSystem"] = value;
                List<string> list = new List<string>();
                value.Statuses.ForEach(delegate(Business.Status s) { list.Add(s.Name); });
                PossibleStatuses = list;

            }
        }

        [DefaultValue(null)]
        private List<string> PossibleStatuses
        {
            get
            {
                object possibleStatuses = ViewState["PossibleStatuses"];
                if (possibleStatuses != null)
                    return (List<string>)possibleStatuses;
                else
                    return null;
            }
            set
            {
                ViewState["PossibleStatuses"] = value;
                if (value != null && value.Count > 0)
                {
                    this.chkListValidateStatuses.DataSource = PossibleStatuses;
                    this.chkListValidateStatuses.DataBind();

                    BaseTermEditPage.BindMultiSelectList(chkListValidateStatuses, this.Term.ValidationStatuses);
                }
            }
        }

        [DefaultValue(null)]
        public List<string> SelectedStatuses
        {
            get
            {
                object selectedStatuses = ViewState["SelectedStatuses"];
                if (selectedStatuses != null)
                    return (List<string>)selectedStatuses;
                else
                    return new List<string>(); ;
            }
            set
            {
                ViewState["SelectedStatuses"] = value;
            }
        }

        [DefaultValue(null)]
        public override Business.Term Term
        {
            get
            {
                object term = ViewState["Term"];
                if (term != null)
                    return (Business.Term)term;
                else
                    return null;
            }
            set
            {
                ViewState["Term"] = value;
            }
        }



        #endregion

        #region Public Methods

		public override void UpdateTermValue(string termGroupContainerName)
        {
            SelectedStatuses.Clear();
            if (this.Validate == true)
            {
                foreach (ListItem item in this.chkListValidateStatuses.Items)
                {
                    if (item.Selected)
                        SelectedStatuses.Add(item.Value);
                }
            }
            this.Term.ValidationStatuses = SelectedStatuses;

        }

        #endregion

        #region Protected Methods

        protected override void OnInit(EventArgs e)
        {
            this.PossibleStatuses = new List<string>();
            this.SelectedStatuses = new List<string>();
            base.OnInit(e);
        }
        
        protected override void OnLoad(EventArgs e)
        {
            //LRR This is to handle ones that already have VOS checked but no ValidationStatuses

            if (IsPostBack)
            {
                UpdateTermValue(null);
            }
            else
            {
                if (this.Term.ValidateOnSave && this.Term.ValidationStatuses.Count == 0)
                {
                    this.SelectedStatuses.AddRange(this.PossibleStatuses);
                }    
            }            
            valSelectStatus.Enabled = chkValidateOnSave.Checked;
            if (valSelectStatus.Enabled)
                valSelectStatus.Validate();
            base.OnLoad(e);         
        }
        
		
		protected override void OnPreRender(EventArgs e)
        {

            SetChildControlsValues();
            base.OnPreRender(e);
            RegisterChildControlEventHandlers();

        }
        
		
		protected override void OnUnload(EventArgs e)
        {

            base.OnUnload(e);
        }


        protected override void Render(HtmlTextWriter writer)
        {
            // Obtain the HTML rendered by the instance.
            StringWriter sw = new StringWriter();
            HtmlTextWriter hw = new HtmlTextWriter(sw);
            base.Render(hw);
            string html = sw.ToString();
            // Kill the writers we don't need anymore.
            hw.Close();
            sw.Close();

			html = html.Replace("<label for=", "<label class=\"DataEntryCaption AlignLeft\" style=\"white-space:nowrap;\" for=");
            // Send the results back into the writer provided.
            writer.Write(html);
        }

        #endregion

        #region Private Methods

        private void RegisterChildControlEventHandlers()
        {
            string function = @"function toggleVisible(chk, obj) {
					var ck = document.getElementById(chk);
					if (ck)
					{
						var el = document.getElementById(obj);
						if (ck.checked) {
							el.style.display = 'block';
						}
						else {
							el.style.display = 'none';
						}
					}
				}";

            if (!this.Page.ClientScript.IsClientScriptBlockRegistered(typeof(ValidateOnSave), "toggleVisible"))
				this.Page.ClientScript.RegisterClientScriptBlock(typeof(ValidateOnSave), "toggleVisible", function, true);

			this.chkValidateOnSave.Attributes["onclick"] = string.Format("javascript:toggleVisible('{0}', '{1}');", chkValidateOnSave.ClientID,  trValidateOnStatus.ClientID);
		}


        private void SetChildControlsValues()
        {
           if (! this.chkValidateOnSave.Checked)
                this.chkValidateOnSave.Checked = this.Term.ValidateOnSave;

            this.chkValidateOnSave.Enabled = true;
            this.chkListValidateStatuses.Enabled = true;
            if (this.SelectedStatuses.Count > 0)
            {
                this.SelectedStatuses.ForEach(delegate(string item) { chkListValidateStatuses.Items.FindByText(item).Selected = true; });
            }
			this.trValidateOnStatus.Style["display"] = (chkValidateOnSave.Checked ? "block" : "none");
        }


        #endregion
           

        protected void SelectionIsValid(object sender, ServerValidateEventArgs args)
        {
             args.IsValid = false;
            if (this.chkValidateOnSave.Checked)
            {                
                foreach (ListItem item in chkListValidateStatuses.Items)
                {
                    if (item.Selected)
                        args.IsValid = true;
                }
            }
        }


    }
}

 
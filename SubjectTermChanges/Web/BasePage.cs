using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Utility;
using Kindred.Common.Security;
using System.Configuration;
using System.Web.Configuration;
using System.Collections.ObjectModel;

namespace Kindred.Knect.ITAT.Web
{

	public abstract class BasePage : System.Web.UI.Page
	{
		#region protected/private members

		internal abstract HtmlGenericControl HTMLBody();
		internal abstract Control ResizablePanel();
		protected const string APPLICATION_NAME = "ITAT";

		private const string _KH_CLOSEDIALOG = "_kh_closedialog";
		private List<string> _loadActions = new List<string>();
		private int _alertCount = 0;
		private Control _postBackControl;
		protected const string _KH_SOURCE_TERM_POSTBACK = "_KH_SOURCE_TERM_POSTBACK";


		#endregion


		#region Public Properties

		public int AlertCount
		{
			get { return _alertCount; }
		}

		#endregion


		#region event handlers

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			AddStartupScripts();
			AddNoCacheHeaders();
		}

				///// <summary>
				///// Moves the ViewState to the bottom of the page to load faster
				///// </summary>
				///// <param name="writer">The <see cref="T:System.Web.UI.HtmlTextWriter"/> that receives the page content.</param>
				///// Created by Larry Richardson LRR 4/14/2008
				//protected override void Render(System.Web.UI.HtmlTextWriter writer)
				//{
				//		// Obtain the HTML rendered by the instance.
				//		StringWriter sw = new StringWriter();
				//		HtmlTextWriter hw = new HtmlTextWriter(sw);
				//		base.Render(hw);
				//		string html = sw.ToString();

				//		// Hose the writers we don't need anymore.
				//		hw.Close();
				//		sw.Close();

				//		// Find the viewstate.  Hope M$ doesn't decide to change the case of these tags anytime soon.
				//		int start = html.IndexOf("<input type=\"hidden\" name=\"__VIEWSTATE\"");
				//		// If we find it, then move it.
				//		if (start >=0)
				//		{
				//				int end = html.IndexOf("/>", start) + 2;

				//				// Strip out the viewstate.
				//				string viewstate = html.Substring(start, end - start);
				//				html = html.Remove(start, end - start);

				//				// Find the end of the form and insert it there, since we can't put it any lower in the response stream.
				//				int FormEndStart = html.IndexOf("</form>") - 1;
				//				if (FormEndStart >= 0)
				//				{
				//						html = html.Insert(FormEndStart, viewstate);
				//				}
               
				//		}

				//		// Send the results back into the writer provided.
				//		writer.Write(html);
				//}

		#endregion



		#region generic page methods

        protected virtual bool AllowAccess(string resource)
        {
            string userLogon = Context.User.Identity.Name;
            userLogon = userLogon.Substring(userLogon.IndexOf('\\') + 1);
			KeyedCollection<string, ISecurityResource> securityResources = Kindred.Common.Security.SecurityInfo.GetUserAccessTypes(userLogon, APPLICATION_NAME, resource);
			if (securityResources.Contains(resource))
			{
				if (string.IsNullOrEmpty(securityResources[resource].AccessType))
				{
					return false;
				}
				else
				{
					return true;
				}
			}
			else
			{
				return false;
			}
        }

        protected void UnauthorizedPageAccess()
        {
            string message = "You are unauthorized to view this page.";
            Response.Redirect(String.Format("~/Exception.aspx?Message={0}&System={1}", message, APPLICATION_NAME));
        }

		private void AddNoCacheHeaders()
		{
			HtmlHead head = this.Page.Header;
			if (head == null)
				return;
			HtmlMeta hm;

			hm = new HtmlMeta();
			hm.HttpEquiv = "Expires";
			hm.Content = "0";
			head.Controls.Add(hm);

			hm = new HtmlMeta();
			hm.HttpEquiv = "Cache-Control";
			hm.Content = "no-cache";
			head.Controls.Add(hm);

			hm = new HtmlMeta();
			hm.HttpEquiv = "Pragma";
			hm.Content = "no-cache";
			head.Controls.Add(hm);
		}


		public static void ShowDialog(BasePage p, string pageName, string dialogProperties, bool refreshOnClose)
		{
			p.ShowDialog(pageName, dialogProperties, refreshOnClose);
		}

		/// <summary>
		/// This function registers script blocks to force a client-side popup dialog.
		/// </summary>
		protected virtual void ShowDialog(string pageName, string dialogProperties, bool refreshOnClose)
		{
			StringWriter scriptBlock = null;
			string scriptBlockName = String.Empty;

			if (refreshOnClose)
			{
				scriptBlockName = "_kh_RefreshPage";
				scriptBlock = new StringWriter();
				scriptBlock.WriteLine("<script type=\"text/javascript\">");
				scriptBlock.WriteLine("<!--");
				scriptBlock.WriteLine(" function _kh_RefreshPage() { ");
				scriptBlock.WriteLine(" document.forms(0).submit(); ");
				scriptBlock.WriteLine(" } ");
				scriptBlock.WriteLine("//-->");
				scriptBlock.WriteLine("</script> ");
				if (!ClientScript.IsClientScriptBlockRegistered(scriptBlockName))
					ClientScript.RegisterClientScriptBlock(this.GetType(), scriptBlockName, scriptBlock.ToString());
				scriptBlock.Close();
				_loadActions.Add("_kh_RefreshPage();");
			}

			if (!dialogProperties.EndsWith(";"))
				dialogProperties += ";";
			scriptBlockName = "_kh_ShowDialog";
			scriptBlock = new StringWriter();
			scriptBlock.WriteLine("<script type=\"text/javascript\">");
			scriptBlock.WriteLine("<!--");
			scriptBlock.WriteLine("var properties = \"{0}\";", dialogProperties);
			if (!dialogProperties.ToLower().Contains("dialogheight:"))
				scriptBlock.WriteLine("properties = properties + ' dialogHeight:' + (0.95 * screen.availHeight).toString() + 'px;'; ");
			if (!dialogProperties.ToLower().Contains("dialogwidth:"))
				scriptBlock.WriteLine("properties = properties + ' dialogWidth:' + (0.9 *  screen.availWidth).toString() + 'px;';");

			scriptBlock.WriteLine("window.showModalDialog('" + pageName + "', '', properties); ");
			scriptBlock.WriteLine("//-->");
			scriptBlock.WriteLine("</script> ");
			if (!ClientScript.IsClientScriptBlockRegistered(scriptBlockName))
				ClientScript.RegisterClientScriptBlock(this.GetType(), scriptBlockName, scriptBlock.ToString());
			scriptBlock.Close();
		}


		protected virtual void CloseDialog()
		{
			Type t = this.GetType();
			if (!Page.ClientScript.IsStartupScriptRegistered(_KH_CLOSEDIALOG))
				Page.ClientScript.RegisterStartupScript(t, _KH_CLOSEDIALOG, "window.close();", true);
		}

		protected virtual void CloseDialog(string sReturnValue)
		{
			Type t = this.GetType();
			if (!Page.ClientScript.IsStartupScriptRegistered(_KH_CLOSEDIALOG))
			{
				string sScript = string.Format("window.returnValue = '{0}';window.close();", sReturnValue);
				Page.ClientScript.RegisterStartupScript(t, _KH_CLOSEDIALOG, sScript, true);
			}
		}

		/// <summary>
		/// This function registers script blocks on the rendered page.
		/// </summary>
		protected virtual void AddStartupScripts()
		{
			string formName = GetFormName();
			string scriptName;
			StringWriter scriptBlock;

			// Add and register script block for resize action.
			if (ResizablePanel() != null)
			{
				scriptName = "_kh_doResize";
				scriptBlock = new StringWriter();
				try
				{
					scriptBlock.WriteLine();
					scriptBlock.WriteLine("<script type=\"text/javascript\">");
					scriptBlock.WriteLine("<!--");
					scriptBlock.WriteLine("function {0}() {{", scriptName);
					scriptBlock.WriteLine("	if (document.readyState != 'complete')");
					scriptBlock.WriteLine("		return false; ");
					scriptBlock.WriteLine("	var p = document.getElementById('{0}');", ResizablePanel().ClientID);
					scriptBlock.WriteLine("	var h = p.offsetHeight;");
					scriptBlock.WriteLine("  var bdy = document.getElementById('{0}');", HTMLBody().ClientID);
					scriptBlock.WriteLine("	h += bdy.clientHeight;");
					scriptBlock.WriteLine("	h -= parseInt(bdy.style.marginTop);");
					scriptBlock.WriteLine("	h -= parseInt(bdy.style.marginBottom);");
					scriptBlock.WriteLine("	for (i = 0; i < bdy.childNodes.length; i++) {");
					scriptBlock.WriteLine("		if (bdy.childNodes[i].offsetHeight) {");
					scriptBlock.WriteLine("			if (bdy.childNodes[i].tagName != 'SCRIPT') {");
					scriptBlock.WriteLine("				h -= bdy.childNodes[i].offsetHeight;");
					scriptBlock.WriteLine("			}");
					scriptBlock.WriteLine("		}");
					scriptBlock.WriteLine("	}");
					scriptBlock.WriteLine("	p.style.height = (h > 0 ? h : 1);");
					scriptBlock.WriteLine("	return true;");
					scriptBlock.WriteLine("}");

					scriptBlock.WriteLine("window.onresize = {0};", scriptName);
					RegisterLoadAction(string.Format("{0}();", scriptName));
					scriptBlock.WriteLine("//-->");
					scriptBlock.WriteLine("</script>");
					if (!ClientScript.IsStartupScriptRegistered(scriptName))
					{
						ClientScript.RegisterStartupScript(this.GetType(), scriptName, scriptBlock.ToString());
					}
				}
				finally
				{
					scriptBlock.Close();
				}
				if (HTMLBody().Attributes.CssStyle["margin"] == null)
					HTMLBody().Attributes.CssStyle["margin"] = "4px";
			}


			// Add and register script block for load actions.
			if (_loadActions != null && _loadActions.Count > 0)
			{
				scriptName = "_kh_doLoad";
				scriptBlock = new StringWriter();
				try
				{
					scriptBlock.WriteLine();
					scriptBlock.WriteLine("<script type=\"text/javascript\">");
					scriptBlock.WriteLine("<!--");
					scriptBlock.WriteLine("function {0}() {{", scriptName);
					scriptBlock.WriteLine("	if (document.readyState != 'complete')");
					scriptBlock.WriteLine("		return false; ");
					for (int i = 0, j = _loadActions.Count; i < j; i++)
					{
						scriptBlock.WriteLine("	{0}", _loadActions[i]);
					}
					scriptBlock.WriteLine("	return true;");
					scriptBlock.WriteLine("}");
					scriptBlock.WriteLine("document.onreadystatechange = {0};", scriptName);
					scriptBlock.WriteLine("//-->");
					scriptBlock.WriteLine("</script>");
					if (!ClientScript.IsStartupScriptRegistered(scriptName))
					{
						ClientScript.RegisterStartupScript(this.GetType(), scriptName, scriptBlock.ToString());
					}
				}
				finally
				{
					scriptBlock.Close();
				}
			}

		}


		/// <summary>
		/// This functions add client-side script for scrolling the supplied control into view.
		/// </summary>
		/// <param name="control">The control to scroll into view.</param>
		public void ScrollControlIntoView(Control control)
		{
			if (control != null)
			{
				RegisterLoadAction(string.Format("document.getElementById('{0}').scrollIntoView();", control.ClientID), false);
			}
		}


		internal void RegisterRedirect(string url)
		{
			RegisterLoadAction(string.Format("document.location.href='{0}';", url));
		}

		/// <summary>
		/// This function registers text to display in a alert box for the response.
		/// </summary>
		/// <param name="text">The alert text to register.</param>
		internal void RegisterAlert(string text)
		{
			_alertCount++;
            RegisterLoadAction(string.Format("alert('{0}');\n", text.Replace("'", "\\'")));
        }

		internal bool RegisterValidationAlerts(List<string> validation, string preface)
		{
			if (validation == null)
				return false;
			if (validation.Count == 0)
				return false;
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.Write(preface + "\\n\\n");
			foreach (string error in validation)
			{
				sw.Write(error);
				sw.Write("\\n");
			}
			sw.Write("\\nPlease correct the error(s) and try again.");
			RegisterAlert(sw.ToString());
			return true;
		}

        internal bool RegisterAlerts(List<string> alerts)
        {
            if (alerts == null)
                return false;
            if (alerts.Count == 0)
                return false;
            System.IO.StringWriter sw = new System.IO.StringWriter();
            if (alerts.Count == 1)
            {
                sw.Write(alerts[0]);
            }
            else
            {
                foreach (string alert in alerts)
                {
                    sw.Write(alert);
                    sw.Write("\\n");
                }
            }
            RegisterAlert(sw.ToString());
            return true;
        }

		internal void RegisterLoadAction(string action)
		{
			RegisterLoadAction(action, false);
		}
		internal void RegisterLoadAction(string action, bool isPriority)
		{
			if (!string.IsNullOrEmpty(action))
				if (isPriority)
					_loadActions.Insert(0, action);
				else
					_loadActions.Add(action);
		}


		/// <summary>
		/// This function returns the name of the form on the page.
		/// </summary>
		/// <returns>The form name.</returns>
		internal protected string GetFormName()
		{
			string rtn = "";
			for (int i = 0, j = HTMLBody().Controls.Count; i < j; i++)
			{
				if (HTMLBody().Controls[i] is HtmlForm)
				{
					rtn = HTMLBody().Controls[i].UniqueID;
					break;
				}
			}
			return rtn;
		}

		protected virtual string GetPageName()
		{
			return String.Empty;
		}

		protected virtual void GetITATSystem()
		{
		}


		protected void RegisterRetainScrollPosition(WebControl control)
		{
			string kh_hf_ScrollPosition = String.Format("_kh_hf_Scroll_{0}", control.ClientID);
			Type t = this.GetType();

			string scriptName1 = "_kh_SetScrollAction";
			System.IO.StringWriter sw1 = new System.IO.StringWriter();
			sw1.WriteLine("	function SetScrollAction(c, hf)");
			sw1.WriteLine("	{");
			sw1.WriteLine("		var hiddenField = document.getElementById(hf);");
			sw1.WriteLine("		if (!isNaN(hiddenField.value)) ");
			sw1.WriteLine("			hiddenField.value = document.getElementById(c).scrollTop; ");
			sw1.WriteLine("		document.getElementById(hf).value = document.getElementById(c).scrollTop; ");
			sw1.WriteLine("	}");
			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName1))
				ClientScript.RegisterClientScriptBlock(t, scriptName1, sw1.ToString(), true);

			string initialScrollPosition = Request.Form[kh_hf_ScrollPosition];
			if (string.IsNullOrEmpty(initialScrollPosition))
				initialScrollPosition = "0";
			ClientScript.RegisterHiddenField(kh_hf_ScrollPosition, initialScrollPosition);

			control.Attributes["onscroll"] = string.Format("javascript:SetScrollAction('{0}', '{1}');", control.ClientID, kh_hf_ScrollPosition);

			string scriptName2 = string.Format("_kh_SetInitialScrollPosition_{0}", control.ClientID);
			System.IO.StringWriter sw2 = new System.IO.StringWriter();
			sw2.WriteLine("	document.getElementById('{0}').scrollTop = {1};", control.ClientID, initialScrollPosition);
			if (!ClientScript.IsStartupScriptRegistered(t, scriptName2))
				ClientScript.RegisterStartupScript(t, scriptName2, sw2.ToString(), true);
		}


		/// <summary>
		/// This property gets the control causing the postback.
		/// </summary>
		public Control PostBackControl
		{
			get
			{
				if (_postBackControl == null)
				{
					if (IsPostBack)
					{
						// Attempt to find control using hidden __EVENTTARGET variable.
						// Works for non-button controls.
						string ctlName = Request.Form["__EVENTTARGET"];
						if (!string.IsNullOrEmpty(ctlName))
						{
							_postBackControl = FindControl(ctlName);
						} /* if */
						else
						{
							// Hidden __EVENTTARGET variable not found; find control the classic way.
							// Works for button controls.
							for (int i = 0, j = Request.Form.Keys.Count; i < j; i++)
							{
								_postBackControl = FindControl(Request.Form.Keys[i]);
								if (_postBackControl is Button)
								{
									break;
								} /* if */
								else
								{
									_postBackControl = null;
								} /* else */
							} /* for */
						} /* else */
					} /* if */
				} /* if */
				return _postBackControl;
			} /* get */
		} /* property */

		/// <summary>
		/// This property gets the control causing the postback.
		/// </summary>
		public string PostBackControlID
		{
			get
			{
				if (IsPostBack)
				{
					// Attempt to find control using hidden __EVENTTARGET variable.
					// Works for non-button controls.
					string ctlName = Request.Form["__EVENTTARGET"];
					if (!string.IsNullOrEmpty(ctlName))
					{
						return ctlName;
					}
					else
					{
						// Hidden __EVENTTARGET variable not found; find control the classic way.
						// Works for button controls.
						for (int i = 0, j = Request.Form.Keys.Count; i < j; i++)
						{
							Control postBackControl = FindControl(Request.Form.Keys[i]);
							if (postBackControl is Button)
							{
								return postBackControl.UniqueID;
							}
						}
					}
				}
				return "";
			}
		}

		/// <summary>
		/// This property gets the argument passed during the postback.
		/// </summary>
		public string PostBackArgument
		{
			get
			{
				if (IsPostBack)
				{
					string sArgument = Request.Form["__EVENTARGUMENT"];
					if (!string.IsNullOrEmpty(sArgument))
					{
						return sArgument;
					}
				}
				return "";
			}
		}

		#endregion



	}
}


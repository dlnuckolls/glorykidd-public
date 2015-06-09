using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Collections;
using Kindred.Knect.ITAT.Business;
using System.Text.RegularExpressions;

namespace Kindred.Knect.ITAT.Web
{


	public static class Helper
	{
		/// <summary>
		/// Recursively calls FindControl until it either finds a control with the requested name, or does not have any more child controls
		/// NOTE:  This will only return the FIRST descendant control that it finds with this id.
		/// </summary>
		/// <param name="root">The control from which to start the recursively</param>
		/// <param name="id">The desired control ID</param>
		/// <returns>The requested Control or null if no control with this name is found</returns>
		public static Control FindControlRecursive(Control root, string id)
		{
			if (root.ID == id)
			{
				return root;
			}
			foreach (Control c in root.Controls)
			{
				Control t = Helper.FindControlRecursive(c, id);
				if (t != null)
				{
					return t;
				}
			}
			return null;
		}


		/// <summary>
		/// This will add or modify an HTML attribute.  If the attribute does not exist, it will be added.   
		/// If the attribute exists, and it contains the value of attrValue, it is left unchanged.
		/// If the attribute exists, but does not contain the value of attrValue, attrValue is appended to the attribute's value
		/// </summary>
		public static void AddAttribute(AttributeCollection attributes, string attrKey, string attrValue)
		{
			string existingValue = attributes[attrKey];
			if (string.IsNullOrEmpty(existingValue))
			{
				attributes.Add(attrKey, attrValue);
			}
			else
			{
				if (!existingValue.Contains(attrValue))
					attributes[attrKey] += attrValue;
			}
		}



		public static TreeNodeCollection Siblings(TreeNode node)
		{
			if (node == null)
				return null;
			if (node.Parent == null)
				return null;
			return node.Parent.ChildNodes;
		}


		public static Term CreateTerm(TermType termType, bool systemTerm, bool IsManagedItem, Template template, bool isFilter)
		{
			switch (termType)
			{
				case TermType.Text:
                    return new TextTerm(systemTerm, template, isFilter);
				case TermType.Date:
                    return new DateTerm(systemTerm, template, isFilter);
				case TermType.Link:
                    return new LinkTerm(systemTerm, template, isFilter);
				case TermType.MSO:
                    return new MSOTerm(systemTerm, template, isFilter);
				case TermType.Renewal:
                    return new RenewalTerm(systemTerm, IsManagedItem, template, isFilter);
				case TermType.Facility:
                    return new FacilityTerm(systemTerm, template, isFilter);
				case TermType.PickList:
                    return new PickListTerm(systemTerm, template, isFilter);
				case TermType.ComplexList:
                    return new ComplexList(systemTerm, template, false, isFilter);
				case TermType.External:
				   throw new Exception("The CreateTerm method is not to be used when creating an external term.");
                case TermType.PlaceHolderAttachments:
                   throw new Exception("The CreateTerm method is not to be used when creating a PlaceHolderAttachments term.");
                case TermType.PlaceHolderComments:
                   throw new Exception("The CreateTerm method is not to be used when creating a PlaceHolderComments term.");
                default:
					throw new Exception(string.Format("Edit screen not implemented for TermType = {0}", termType));
			}
		}

		public static string TemplateTermEditPage(TermType termType)
		{
			switch (termType)
			{
				case TermType.Text:
					return "TermEditText.aspx";
				case TermType.Date:
					return "TermEditDate.aspx";
				case TermType.Link:
					return "TermEditLink.aspx";
				case TermType.MSO:
					return "TermEditMSO.aspx";
				case TermType.Renewal:
					return "TermEditRenewal.aspx";
				case TermType.Facility:
					return "TermEditFacility.aspx";
				case TermType.PickList:
					return "TermEditPickList.aspx";
				case TermType.ComplexList:
					return "TermEditComplexList.aspx";
				case TermType.External:
				   return "TermEditExternal.aspx";
                case TermType.PlaceHolderAttachments:
                case TermType.PlaceHolderComments:
                   return "TermEditAttachmentsAndComments.aspx";
				default:
					throw new Exception(string.Format("Edit screen not implemented for TermType = {0}", termType));
			}
		}


		public static void LoadTemplateStatusDropDown(DropDownList ddl, bool addAll, TemplateStatusType selectedValue)
		{
			ddl.Items.Clear();
			TemplateStatusType[] values = (TemplateStatusType[])Enum.GetValues(typeof(TemplateStatusType));
			foreach (TemplateStatusType value in values)
			{
				ListItem itm = new ListItem(string.Format("{0:G}", value), string.Format("{0:D}", value));
				itm.Selected = (value == selectedValue);
				ddl.Items.Add(itm);
			}
			if (addAll)
				ddl.Items.Add(new ListItem("All", "-1"));
		}


        public static void LoadRetroOptionsDropDown(DropDownList ddl, bool addAll, Retro.RetroModel selectedValue)
        {
            ddl.Items.Clear();
            Retro.RetroModel[] values = (Retro.RetroModel[])Enum.GetValues(typeof(Retro.RetroModel));


            foreach (Retro.RetroModel value in values)
            {
                if (value != Retro.RetroModel.Off)
                {
                    ListItem itm = new ListItem(string.Format("{0:G}", Utility.EnumHelper.GetDescription(value)), string.Format("{0:D}", value));
                    itm.Selected = (value == selectedValue);
                    ddl.Items.Add(itm);
                }
            }
            if (addAll)
                ddl.Items.Add(new ListItem("All", "-1"));


        }


		/// <summary>
		/// Similar to LoadTemplateStatusDropDown(ddl, addAll, includeUnknown, selectedValue), defaulting to "Inactive"
		/// </summary>
		/// <param name="ddl"></param>
		/// <param name="addAll"></param>
		public static void LoadTemplateStatusDropDown(DropDownList ddl, bool addAll)
		{
			LoadTemplateStatusDropDown(ddl, addAll, TemplateStatusType.Inactive);
		}


		//NOTE: Since the RoleType enum has the Flags attribute set, "roleTypes" can denote multiple RoleType's
		//For a given role, if ANY of the roles RoleType values match  ANY of the RoleType values specified in "roleTypes", then that role is included in the ListControl.
		private static void LoadRoles(System.Web.UI.WebControls.ListControl listControl, ITATSystem system, RoleType roleTypes)
		{
			List<Role> roles = new List<Role>();
			foreach (Role role in system.Roles)
				if ((role.RoleType & roleTypes) != RoleType.None)
					roles.Add(role);
			listControl.DataSource = roles;
			listControl.DataTextField = "Name";
			listControl.DataValueField = "Name";
			listControl.DataBind();
		}

		public static void LoadRoles(System.Web.UI.WebControls.ListControl listControl, ITATSystem system,  RoleType roleTypes, List<Role> selectedRoles)
		{
			LoadRoles(listControl, system, roleTypes);
			if (selectedRoles != null)
			{
				List<string> roleNames = selectedRoles.ConvertAll<string>(Role.StringConverter);
				foreach (ListItem item in listControl.Items)
					item.Selected = roleNames.Contains(item.Value);
			}
		}


		public static void LoadRoles(System.Web.UI.WebControls.ListControl listControl, ITATSystem system, RoleType roleTypes, List<string> selectedRoles)
		{
			LoadRoles(listControl, system, roleTypes);
			if (selectedRoles != null)
			{
				foreach (ListItem item in listControl.Items)
					item.Selected = selectedRoles.Contains(item.Value);
			}
		}



		public static void MarkListBoxMultiSelection(ListBox listBox, string targetValue)
		{
			foreach (ListItem li in listBox.Items)
				if (li.Value == targetValue)
					li.Selected = true;
		}


		public static void MarkListBoxMultiSelection(ListBox listBox, IList<string> targetTextValues)
		{
			foreach (ListItem li in listBox.Items)
				if (targetTextValues.Contains(li.Value))
					li.Selected = true;
		}

		
		public static string GetDefaultBasicTermName(Template template)
		{
			string startingTermName = "New Term";
			string termName = startingTermName;
			int counter = 1;
			while (template.BasicTermExists(termName))
			{
				termName = string.Format("{0} {1}", startingTermName, counter++);
			}
			return termName;
		}

        public static string GetDefaultWorkflowName(Template template)
        {
            string startingWorkflowName = "New Workflow";
            string workflowName = startingWorkflowName;
            int counter = 1;
            while (template.WorkflowExists(workflowName))
            {
                workflowName = string.Format("{0} {1}", startingWorkflowName, counter++);
            }
            return workflowName;
        }

		public static string GetDefaultComplexListTermName(Template template)
		{
			string startingTermName = "New Complex List";
			string termName = startingTermName;
			bool termNameExists = template.ComplexListTermNameInUse(termName, null);
			int counter = 1;
			while (termNameExists)
			{
				termName = string.Format("{0} {1}", startingTermName, counter);
				termNameExists = template.BasicTermExists(termName);
			}
			return termName;
		}

		public static string GetDefaultTermGroupName(Template template)
		{
			string startingTermGroupName = "New Tab";
			string termGroupName = startingTermGroupName;
			int counter = 1;
			while (template.TermGroupExists(termGroupName))
			{
				termGroupName = string.Format("{0} {1}", startingTermGroupName, counter++);
			}
			return termGroupName;
		}



		private static void AddTermsToolBarItem(Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd, string text)
		{
			tdd.Items.Add(new Telerik.WebControls.RadEditorUtils.ListItem(text, text));
		}

		public static void AddSpecialToolBarItems(BaseSystemPage page, Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd)
		{
            AddTermsToolBarItem(tdd, "* " + page.ITATSystem.ManagedItemName + XMLNames._SpecialTermName_Number);
            AddTermsToolBarItem(tdd, "* " + XMLNames._SpecialTermName_Status);
            AddTermsToolBarItem(tdd, "* " + XMLNames._SpecialTermName_WorkflowState);
		}

		public static void InitializeToolBarItems(BaseSystemPage page, Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd)
		{
			tdd.Items.Clear();
			tdd.DropDownHeight = Unit.Parse("240px");
			tdd.DropDownWidth = Unit.Parse("180px");
			tdd.ShowText = false;
			RegisterEditorInsertTermAction(page);
			RegisterEditorImageManagerAction(page);
		}

		public static void AddTermsToolBarItems(BaseSystemPage page, Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd, List<Term> terms)
		{
            //Filter out PlaceHolderAttachments and PlaceHolderComments.

            terms = BasicTerms.FindTermsOfTypeExcluding(terms, TermType.PlaceHolderComments | TermType.PlaceHolderAttachments);

            List<string> sortedTerms = new List<string>();

            //TODO*: In the case of terms with embedded 'pipe' symbols, if the embedded text is not of a 'fixed' nature (MSOTerm, External), then 
            //the handling of term name changes may be broken with respect to TermID's.
			foreach (Term term in terms)
			{
				switch (term.TermType)
				{
					case TermType.None:
						break;
					case TermType.MSO:
						MSOTerm msoTerm = term as MSOTerm;
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.MSOName));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.Address1Name));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.Address2Name));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.CityName));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.StateName));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.ZipName));
						sortedTerms.Add(string.Format("{0} | {1}", msoTerm.Name, msoTerm.PhoneName));
						break;
					case TermType.Renewal:
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_EffectiveDate));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_InitialTermDuration));
						if (((RenewalTerm)term).IsTypeNone)
						{
                            sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_ExpirationDate));
						}
						else
						{
                            sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_RenewalDate));
                            sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_RenewalTermDuration));
						}
						break;
					case TermType.Facility:
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_FacilityName));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_SAPID));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_FacilityType));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_Address));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_City));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_County));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_State));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_StateCode));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_Zip));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_Phone));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_Fax));
                        sortedTerms.Add(term.Name + string.Format(" | {0}", XMLNames._TPS_LegalEntityName));
						break;
					case TermType.External:
					   foreach (ExternalInterfaceSearchableField searchableField  in ((Business.ExternalTerm)term).InterfaceConfig.SearchableFields)
                           sortedTerms.Add(string.Format("{0} | {1}", term.Name, searchableField.Name));
					   break;
					case TermType.ComplexList:
					case TermType.Text:
					case TermType.Date:
					case TermType.PickList:
					case TermType.Link:
                       sortedTerms.Add(term.Name);
						break;
                    default:
                        throw new Exception(string.Format("Term type '{0}' not handled", term.TermType.ToString()));
				}
			}

            sortedTerms.Sort();

            foreach (string term in sortedTerms)
            {
                AddTermsToolBarItem(tdd, term);
            }
      
		}


		private static string TransformParagraphs(string html)
		{
			//Remove any <p></p> tag pairs, leaving the contents between the tags.   (This may be temporary code.)
			const string pattern = @"<p (.+?)>(.+?)</p>";
			string text = HttpUtility.HtmlDecode(html);
			if (!string.IsNullOrEmpty(html))
			{
				MatchCollection matches = Regex.Matches(html, pattern);
				while (matches.Count > 0)
				{
					Match match = matches[0];
					string matchedText = match.Result("$0");
					string replacementText = match.Groups[2].Value;
					html = HttpUtility.HtmlDecode(text).Replace(matchedText, replacementText);
					matches = Regex.Matches(html, pattern);
				}
			}
			return html;

		}

		public static void RegisterParagraphWrapperScript(Page page)
		{
			//  The following javascript code was downloaded from the telerik Knowledge Base:
			//                  http://www.telerik.com/community/forums/thread/b311D-cmkdk.aspx
			//  It replaces the "InsertParagraph" button logic with a more robust pragraph wrapper.
			//  If text is selected in the editor, it will wrap that text in a paragraph.  
			//  If no text is selected, it will insert a paragraph as the current cursor location.
			Type t = page.GetType();
			string scriptName = "_kh_InsertParagraphWrapper";
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                sw.WriteLine("RadEditorCommandList[\"Paragraph Wrapper\"] = function(commandName, editor, oTool) ");
                sw.WriteLine("{");
                sw.WriteLine("	var cArea = editor.GetContentArea();");
                sw.WriteLine("	var pTags = cArea.getElementsByTagName(\"p\");");
                sw.WriteLine("	var oRange = editor.Document.selection.createRange();");
                sw.WriteLine("	var text = _kh_trimRight(oRange.htmlText);");
                sw.WriteLine("	var replacementText;");
                sw.WriteLine("	if (pTags) ");
                sw.WriteLine("	{");
                sw.WriteLine("		for (var i=0; i<pTags.length; i++) ");
                sw.WriteLine("		{");
                sw.WriteLine("			var elem = pTags.item(i);");
                sw.WriteLine("			var index = elem.innerHTML.indexOf(text);");
                sw.WriteLine("			if (index>0) ");
                sw.WriteLine("			{");
                sw.WriteLine("				if ((index + text.length +1) == elem.innerHTML.length) ");
                sw.WriteLine("				{");
                sw.WriteLine("					replacementText = \"</p>\\n<p> \" + text + \"</p>\\n\";");
                sw.WriteLine("					break;");
                sw.WriteLine("				} ");
                sw.WriteLine("				else ");
                sw.WriteLine("				{");
                sw.WriteLine("					replacementText = \"</p>\\n<p> \" + text + \"</p>\\n<p>\";");
                sw.WriteLine("					break;");
                sw.WriteLine("				}");
                sw.WriteLine("			}");
                sw.WriteLine("		}");
                sw.WriteLine("	}");
                sw.WriteLine("	if (!replacementText) ");
                sw.WriteLine("	{ ");
                sw.WriteLine("		replacementText = \"\\n<p>\" + text + \"</p>\\n\"; ");
                sw.WriteLine("	} ");
                sw.WriteLine("	oRange.pasteHTML(replacementText); ");
                sw.WriteLine("	oRange.collapse(false);  ");
                sw.WriteLine("}");

                if (!page.ClientScript.IsStartupScriptRegistered(t, scriptName))
                    page.ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
            }
		}


		private static void RegisterEditorInsertTermAction(Page page)
		{
			Type t = page.GetType();
			string scriptName = "_kh_InsertTermAction";
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                sw.WriteLine("RadEditorCommandList[\"Insert Term\"] = function(commandName, editor, oTool) ");
                sw.WriteLine("{");
                sw.WriteLine("	oValue = oTool.GetSelectedValue();");
                //20070817_DEG  Bug 131 - Special case - replace the quotes
                sw.WriteLine("	oValue = oValue.replace(/\"/g,\"&quot;\");");
                //20070817_DEG  Tried this approach in order to support trailing spaces, but spaces are trimmed by the drawing tool code.
                //sw.WriteLine("	oValue = oValue.replace(/ /g,\"&#032;\");");
                sw.WriteLine("	editor.PasteHtml('<img class=\"TermImg\" src=\"TextImage.ashx?text=' + oValue + '\"/>');");
                sw.WriteLine("}");
                if (!page.ClientScript.IsStartupScriptRegistered(t, scriptName))
                    page.ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
            }
		}


		private static void RegisterEditorImageManagerAction(BaseSystemPage page)
		{
			Type t = page.GetType();
			string scriptName = "_kh_ImageManagerAction";
            using (System.IO.StringWriter sw = new System.IO.StringWriter())
            {
                //sw.WriteLine("var RadEditorClientObject = null;");
                sw.WriteLine("RadEditorCommandList[\"Image Manager\"] = function(commandName, editor, oTool) ");
                sw.WriteLine("{");
                sw.WriteLine("	var props='center:yes; help:no; resizable:yes; dialogHeight:500px; dialogWidth:540px;'; ");
                sw.WriteLine("	var retVal=window.showModalDialog('ImageSelector.aspx?system={0}', '', props); ", page.ITATSystem.ID);
                //NOTE: retVal is expected to be an object with properties named 'id' and 'name'			
                sw.WriteLine("	if (retVal) ");
                sw.WriteLine("	{");
                sw.WriteLine("		editor.PasteHtml('<img src=\"ShowImage.ashx?id=' + retVal.id + '\"/>');");
                sw.WriteLine("	}");
                sw.WriteLine("}");

                if (!page.ClientScript.IsStartupScriptRegistered(t, scriptName))
                    page.ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
            }
		}





		/// <summary>
		/// Updates a collection of Role objects based on the selected items in a list box
		/// </summary>
		/// <param name="lstRoles">List box containing the user's selections</param>
		/// <param name="roles">A collection of Roles to compare the selections against</param>
		/// <returns>Indicates whether there were changes</returns>
		public static bool UpdateList(ListControl lstRoles, List<Role> roles)
		{
			bool rtn = false;

			List<string> selectedRoles = new List<string>();
			for (int i = 0, j = lstRoles.Items.Count; i < j; i++)
				if (lstRoles.Items[i].Selected)
					selectedRoles.Add(lstRoles.Items[i].Value);

			if (!Utility.ListHelper.ListsMatch<string, Role>(selectedRoles, roles))
			{
				rtn = true;
				roles.Clear();
				for (int i = 0, j = lstRoles.Items.Count; i < j; i++)
					if (lstRoles.Items[i].Selected)
						roles.Add(new Role(lstRoles.Items[i].Value));
			}

			return rtn;
		}

		/// <summary>
		/// Updates a collection of strings based on the selected items in a list box
		/// </summary>
		/// <param name="lstRoles">List box containing the user's selections</param>
		/// <param name="roles">A collection of strings to compare the selections against</param>
		/// <returns>Indicates whether there were changes</returns>
		public  static bool UpdateList(ListControl lstRoles, List<string> roles)
		{
			bool rtn = false;

			List<string> selectedRoles = new List<string>();
			for (int i = 0, j = lstRoles.Items.Count; i < j; i++)
				if (lstRoles.Items[i].Selected)
					selectedRoles.Add(lstRoles.Items[i].Value);

			if (roles == null)
				roles = new List<string>(selectedRoles.Count);

			if (!Utility.ListHelper.ListsMatch<string, string>(selectedRoles, roles))
			{
				rtn = true;
				roles.Clear();
				for (int i = 0, j = lstRoles.Items.Count; i < j; i++)
					if (lstRoles.Items[i].Selected)
						roles.Add(lstRoles.Items[i].Value);
			}

			return rtn;
		}



        //public static void LoadListControl(ListControl ddl, IDictionary list, string textField, string valueField, string currentValue)
        //{
        //    LoadListControl(ddl, list, textField, valueField, currentValue, false, "", "");
        //}

        //public static void LoadListControl(ListControl ddl, IDictionary list, string textField, string valueField, string currentValue, bool includeDefault, string defaultText, string defaultValue)
        //{
        //    ddl.DataSource = list.Values;
        //    ddl.DataTextField = textField;
        //    ddl.DataValueField = valueField;
        //    ddl.DataBind();
        //    if (includeDefault)
        //        ddl.Items.Insert(0, new ListItem(defaultText, defaultValue));

        //    try
        //    {
        //        ddl.SelectedValue = currentValue;
        //    }
        //    catch
        //    {
        //        ddl.SelectedIndex = -1;
        //    }
        //    if (ddl.SelectedIndex == -1)
        //        if (includeDefault)
        //            ddl.SelectedIndex = 0;
        //}


		public static void LoadListControl(ListControl ddl, IList list, string textField, string valueField, string currentValue)
		{
			LoadListControl(ddl, list, textField, valueField, currentValue, false, "", "");
		}

		public static void LoadListControl(ListControl ddl, IList list, string textField, string valueField, string currentValue, bool includeDefault, string defaultText, string defaultValue)
		{
			try
			{
				ddl.DataSource = list;
				ddl.DataTextField = textField;
				ddl.DataValueField = valueField;
				ddl.DataBind();
				if (includeDefault)
					ddl.Items.Insert(0, new ListItem(defaultText, defaultValue));
				ddl.SelectedValue = currentValue;
			}
			catch
			{
				ddl.SelectedIndex = -1;
			}
			if (ddl.SelectedIndex == -1)
				if (includeDefault)
					ddl.SelectedIndex = 0;
		}


        //public static void LoadListControl(ListControl ddl, DataSet list, string textField, string valueField, string currentValue)
        //{
        //    LoadListControl(ddl, list, textField, valueField, currentValue, false, "", "");
        //}

		public static void LoadListControl(ListControl ddl, DataSet list, string textField, string valueField, string currentValue, bool includeDefault, string defaultText, string defaultValue)
		{
			try
			{
				ddl.DataSource = list;
				ddl.DataTextField = textField;
				ddl.DataValueField = valueField;
				ddl.DataBind();
				if (includeDefault)
					ddl.Items.Insert(0, new ListItem(defaultText, defaultValue));
				ddl.SelectedValue = currentValue;
			}
			catch
			{
				ddl.SelectedIndex = -1;
			}
			if (ddl.SelectedIndex == -1)
				if (includeDefault)
					ddl.SelectedIndex = 0;
		}


		#region Create Web Controls 


		public static Dictionary<string, WebControl> CreateSearchControls(List<Term> terms, bool canEditProfile, Business.SecurityHelper securityHelper)
		{
			Dictionary<string, WebControl> rtn = new Dictionary<string, WebControl>(terms.Count);
			WebControl ctl;
			foreach (Term term in terms)
			{
				switch (term.TermType)
				{
					case TermType.Text:
						ctl = Helper.CreateTextSearchControl((TextTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.Date:
						ctl = CreateDateSearchControl((DateTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.MSO:
						ctl = CreateMSOSearchControl((MSOTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.Renewal:
						ctl = CreateRenewalSearchControl((RenewalTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.Facility:
						ctl = CreateFacilitySearchControl((FacilityTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.PickList:
						ctl = CreatePickListSearchControl((PickListTerm)term, canEditProfile, securityHelper);
						break;
					case TermType.External:
					default:
						ctl = new Label();
						((Label)ctl).Text = "External Term Dummy Code";
						break;
				}
				ctl.EnableViewState = true;
				rtn.Add(term.Name, ctl);
			}
			return rtn;
		}


		internal static WebControl CreateExternalTermSearchControl(ExternalInterfaceConfig eic)
		{
			Label lbl = new Label();
			lbl.Width = Unit.Percentage(100.0);
			lbl.Text = string.Format("Search control for external interface '{0}' goes here.", eic.Name);
			return lbl;
		}


		private static TextBox CreateTextSearchControl(TextTerm textTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			TextBox txt = new TextBox();
			txt.ID = ControlID(textTerm.Name);
			switch (textTerm.Format)
			{
				case TextTermFormat.Plain:
					txt.Width = Unit.Percentage(100.0);
					break;
				case TextTermFormat.Number:
					txt.Width = Unit.Pixel(120);
					break;
				case TextTermFormat.Currency:
					txt.Width = Unit.Pixel(120);
					break;
				case TextTermFormat.SSN:
					txt.Width = Unit.Pixel(120);
					break;
				case TextTermFormat.Phone:
					txt.Width = Unit.Pixel(120);
					break;
				case TextTermFormat.PhonePlusExtension:
					txt.Width = Unit.Pixel(200);
					break;
				default:
					break;
			}
			return txt;
		}

        internal static TextBox CreateTextComplexListFieldControl(ComplexListField field)
        {
            TextBox txt = new TextBox();
            txt.ID = ControlID(field.Name);
            switch (((TextTerm)field.FilterTerm).Format)
            {
                case TextTermFormat.Plain:
                    txt.Width = Unit.Percentage(100);
                    break;
                case TextTermFormat.Number:
                    txt.Width = Unit.Pixel(120);
                    break;
                case TextTermFormat.Currency:
                    txt.Width = Unit.Pixel(120);
                    break;
                case TextTermFormat.SSN:
                    txt.Width = Unit.Pixel(120);
                    break;
                case TextTermFormat.Phone:
                    txt.Width = Unit.Pixel(120);
                    break;
                case TextTermFormat.PhonePlusExtension:
                    txt.Width = Unit.Pixel(200);
                    break;
                default:
                    break;
            }
            return txt;
        }
        internal static TextBox CreateTextComplexListFieldControl(ComplexListItemValue itemValue)
        {
            TextBox txt = new TextBox();
            txt.ID = ControlID(itemValue.FieldName);
            txt.Width = Unit.Percentage(100);
            //switch (((TermType.Text)itemValue.TermType).Format)
            //{
            //    case TextTermFormat.Plain:
            //        txt.Width = Unit.Percentage(100);
            //        break;
            //    case TextTermFormat.Number:
            //        txt.Width = Unit.Pixel(120);
            //        break;
            //    case TextTermFormat.Currency:
            //        txt.Width = Unit.Pixel(120);
            //        break;
            //    case TextTermFormat.SSN:
            //        txt.Width = Unit.Pixel(120);
            //        break;
            //    case TextTermFormat.Phone:
            //        txt.Width = Unit.Pixel(120);
            //        break;
            //    case TextTermFormat.PhonePlusExtension:
            //        txt.Width = Unit.Pixel(200);
            //        break;
            //    default:
            //        break;
            //}
            return txt;
        }

		private static Panel CreateDateSearchControl(DateTerm dateTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			Panel pnl = new Panel();

			//if current page is the Profile page, then display the MSO Control.  Otherwise (e.g. Search), show a text box.
			BaseManagedItemPage p = HttpContext.Current.CurrentHandler as BaseManagedItemPage;
			if (p == null)
			{
				pnl.Controls.Add(DateRangeControl(dateTerm.Name));
			}
			else
			{
				//Profile page
				pnl.Controls.Add(CreateDateControl(dateTerm.Name));
			}
			return pnl;
		}

        internal static Panel CreateDateComplexListFieldControl(ComplexListItemValue itemValue)
        {

            Panel pnl = new Panel();
            pnl.ID = ControlID(itemValue.FieldName) + "_pnl";

            //write textbox to contain date					
            TextBox txtDate = new TextBox();   //pnl.Controls[0]
            txtDate.Style["width"] = "114px";
            txtDate.Style["height"] = "18px";
            txtDate.Style["padding-top"] = "0";
            txtDate.MaxLength = 12;
            txtDate.ID = ControlID(itemValue.FieldName);
            txtDate.Attributes["onfocus"] = "javascript:this.select();";
            pnl.Controls.Add(txtDate);

            //write "button" image and click event that will display calendar IFRAME
            Image imgDate = new Image();    //pnl.Controls[1]
            imgDate.Style["cursor"] = "hand";
            imgDate.ID = ControlID(itemValue.FieldName) + "_img";
            imgDate.ImageUrl = "/Global/bin/Calendar/calendar.gif";
            imgDate.Height = Unit.Parse("19px");
            imgDate.Width = Unit.Parse("34px");
            imgDate.ImageAlign = ImageAlign.Top;
            imgDate.BorderWidth = Unit.Parse("0px");
            imgDate.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtDate.ClientID);
            pnl.Controls.Add(imgDate);

            return pnl;
        }
        internal static Panel CreateDateComplexListFieldControl(ComplexListField field)
        {

            Panel pnl = new Panel();
            pnl.ID = ControlID(field.Name) + "_pnl";

            //write textbox to contain date					
            TextBox txtDate = new TextBox();   //pnl.Controls[0]
            txtDate.Style["width"] = "114px";
            txtDate.Style["height"] = "18px";
            txtDate.Style["padding-top"] = "0";
            txtDate.MaxLength = 12;
            txtDate.ID = ControlID(field.Name);
            txtDate.Attributes["onfocus"] = "javascript:this.select();";
            pnl.Controls.Add(txtDate);

            //write "button" image and click event that will display calendar IFRAME
            Image imgDate = new Image();    //pnl.Controls[1]
            imgDate.Style["cursor"] = "hand";
            imgDate.ID = ControlID(field.Name) + "_img";
            imgDate.ImageUrl = "/Global/bin/Calendar/calendar.gif";
            imgDate.Height = Unit.Parse("19px");
            imgDate.Width = Unit.Parse("34px");
            imgDate.ImageAlign = ImageAlign.Top;
            imgDate.BorderWidth = Unit.Parse("0px");
            imgDate.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtDate.ClientID);
            pnl.Controls.Add(imgDate);

            return pnl;
        }


		private static Panel CreateRenewalSearchControl(RenewalTerm renewalTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			Panel pnl = new Panel();
			pnl.ID = ControlID(renewalTerm.Name);
			pnl.Controls.Add(DateRangeControl(renewalTerm.Name));
			return pnl;
		}


		private static Panel CreateMSOSearchControl(MSOTerm msoTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			Panel pnl = new Panel();
			pnl.ID = ControlID(msoTerm.Name);
			TextBox txtMso = new TextBox();
			txtMso.ID = pnl.ID + "_lst";
			txtMso.Width = Unit.Percentage(100.0);
			pnl.Controls.Add(txtMso);
			return pnl;
		}


		private static Panel CreateFacilitySearchControl(FacilityTerm facilityTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			bool canEdit = canEditProfile;
			if (facilityTerm.SystemTerm && facilityTerm.OwningFacilityIDs.Count > 0)
				canEdit = false;

			Panel pnl = new Panel();
			pnl.ID = ControlID(facilityTerm.Name);
			pnl.CssClass = "NoBorder";

			if (canEdit)
			{

				FacilityCollection facilities;
				if (facilityTerm.UseUserSecurity ?? false)
					if (facilityTerm.IncludeChildren ?? false)
						facilities = FacilityCollection.FilteredFacilityList(securityHelper.AllUserFacilities, facilityTerm);
					else
						facilities = FacilityCollection.FilteredFacilityList(securityHelper.UserFacilities, facilityTerm);
				else
					facilities = FacilityCollection.FilteredFacilityList(FacilityCollection.FacilityList(Data.Facility.CorporateFacilityId, facilityTerm.IncludeChildren ?? false), facilityTerm);
				List<Data.FacilityDataRow> sortedFacilities = facilities.SortedList(facilityTerm.SortField);

				if (facilityTerm.MultiSelect ?? false)
				{
					pnl.Height = Unit.Pixel(4 + 20 * Math.Min(4, facilities.Count));
					pnl.Width = Unit.Percentage(100.0);

					MultiSelectGrid grd = new MultiSelectGrid();
					grd.CssClass = "MSG";
					grd.ID = pnl.ID + "_lst";
					grd.CheckboxColumn = 0;
					grd.BoundColumns = "FacilityId SapFacilityId FacilityName";
					grd.ColumnWidths = "0px 60px 100%";
					//grd.DataKeyNames = new string[] { "FacilityId" };
					grd.Container = pnl.ID;
					grd.HeaderContainer = "";
					grd.AutoGenerateColumns = false;
					grd.EnableClickEvent = false;
					grd.RowHighlighting = false;
					grd.EnableDoubleClickEvent = false;
					grd.EnableHeaderClick = false;
					grd.ShowHeader = false;
					grd.DataSource = sortedFacilities;
					grd.DataBind();
					pnl.Controls.Add(grd);
				}
				else
				{
					DropDownList ddl = new DropDownList();
					ddl.ID = pnl.ID + "_lst";
					ddl.Width = Unit.Parse("100%");
					Helper.LoadListControl(ddl, sortedFacilities, "SapIdPlusName", "FacilityId", "", true, "(Select a Facility)", "0");
					pnl.Controls.Add(ddl);
				}
			}
			else
			{
				Label lbl = new Label();
				lbl.Text = string.Format("{0} - {1}", facilityTerm.DisplayValue(XMLNames._TPS_SAPID), facilityTerm.DisplayValue(XMLNames._TPS_FacilityName));
				pnl.Controls.Add(lbl);
			}
			return pnl;
		}


		private static Panel CreatePickListSearchControl(PickListTerm pickListTerm, bool canEditProfile, SecurityHelper securityHelper)
		{
			Panel pnl = new Panel();
			pnl.ID = ControlID(pickListTerm.Name);

			if (pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
			{
				CheckBox chk = new CheckBox();
				chk.BorderStyle = BorderStyle.None;
				chk.ID = pnl.ID + "_lst";    // ????
				chk.CssClass = Common.Names._STYLE_CSSCLASS_EDIT;
				chk.Text = pickListTerm.PickListItems[0].Value;
				pnl.Controls.Add(chk);
			}
			else
			{
				if (pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
				{
					pnl.Height = Unit.Pixel(4 + 20 * Math.Min(4, pickListTerm.PickListItems.Count));
					pnl.Width = Unit.Percentage(100.0);

					MultiSelectGrid grd = new MultiSelectGrid();
					grd.ID = pnl.ID + "_lst";
					grd.CheckboxColumn = 0;
					grd.DataSource = pickListTerm.PickListItems;
					grd.BoundColumns = "Value";
					//grd.DataKeyNames = new string[] { "Value" };
					grd.Container = pnl.ID;
					grd.HeaderContainer = "";
					grd.AutoGenerateColumns = false;
					grd.RowHighlighting = false;
					grd.EnableClickEvent = false;
					grd.EnableDoubleClickEvent = false;
					grd.EnableHeaderClick = false;
					grd.ShowHeader = false;
					grd.ColumnWidths = "100%";
					grd.DataBind();
					pnl.Controls.Add(grd);
				}
				else
				{
					if (pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
					{
						RadioButtonList rdolst = new RadioButtonList();
						rdolst.ID = pnl.ID + "_lst";
						rdolst.CellPadding = 0;
						rdolst.CellSpacing = 0;
						rdolst.RepeatDirection = RepeatDirection.Vertical;
						AddListItems(rdolst, pickListTerm.PickListItems);
						pnl.Controls.Add(rdolst);
					}
					else          // 3 or more item single-select list: DropDrownList
					{
						DropDownList ddl = new DropDownList();
						ddl.ID = pnl.ID + "_lst";
						ddl.Width = Unit.Percentage(100.0);
						ddl.Items.Add(new ListItem("(Select One)", ""));
						AddListItems(ddl, pickListTerm.PickListItems);
						pnl.Controls.Add(ddl);
					}
				}
			}
			return pnl;
		}


        internal static Panel CreatePickListComplexListFieldControl(ComplexListField field)
        {
            Panel pnl = new Panel();
            pnl.ID = ControlID(field.Name);
            PickListTerm pickListTerm = (PickListTerm)field.FilterTerm;
            if (pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
            {
                CheckBox chk = new CheckBox();
                chk.BorderStyle = BorderStyle.None;
                chk.ID = pnl.ID + "_lst";    // ????
                chk.CssClass = Common.Names._STYLE_CSSCLASS_EDIT;
                chk.Text = pickListTerm.PickListItems[0].Value;
                pnl.Controls.Add(chk);
            }
            else
            {
                if (pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
                {
                    pnl.Height = Unit.Pixel(4 + 20 * Math.Min(4, pickListTerm.PickListItems.Count));
                    pnl.Width = Unit.Percentage(100.0);

                    MultiSelectGrid grd = new MultiSelectGrid();
                    grd.ID = pnl.ID + "_lst";
                    grd.CheckboxColumn = 0;
                    grd.DataSource = pickListTerm.PickListItems;
                    grd.BoundColumns = "Value";
                    //grd.DataKeyNames = new string[] { "Value" };
                    grd.Container = pnl.ID;
                    grd.HeaderContainer = "";
                    grd.AutoGenerateColumns = false;
                    grd.RowHighlighting = false;
                    grd.EnableClickEvent = false;
                    grd.EnableDoubleClickEvent = false;
                    grd.EnableHeaderClick = false;
                    grd.ShowHeader = false;
                    grd.ColumnWidths = "100%";
                    grd.DataBind();
                    pnl.Controls.Add(grd);
                }
                else
                {
                    if (pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
                    {
                        RadioButtonList rdolst = new RadioButtonList();
                        rdolst.ID = pnl.ID + "_lst";
                        rdolst.CellPadding = 0;
                        rdolst.CellSpacing = 0;
                        rdolst.RepeatDirection = RepeatDirection.Vertical;
                        AddListItems(rdolst, pickListTerm.PickListItems);
                        pnl.Controls.Add(rdolst);
                    }
                    else          // 3 or more item single-select list: DropDrownList
                    {
                        DropDownList ddl = new DropDownList();
                        ddl.ID = pnl.ID + "_lst";
                        ddl.Width = Unit.Percentage(100.0);
                        ddl.Items.Add(new ListItem("(Select One)", ""));
                        AddListItems(ddl, pickListTerm.PickListItems);
                        pnl.Controls.Add(ddl);
                    }
                }
            }
            return pnl;
        }
        internal static Panel CreatePickListComplexListFieldControl(ComplexListItemValue itemValue)
        {
            Panel pnl = new Panel();
            pnl.ID = ControlID(itemValue.FieldName);
            PickListTerm pickListTerm = (PickListTerm)itemValue.Term;
            if (pickListTerm.PickListItems.Count == 1)    //1 picklist item: single check box
            {
                CheckBox chk = new CheckBox();
                chk.BorderStyle = BorderStyle.None;
                chk.ID = pnl.ID + "_lst";    // ????
                chk.CssClass = Common.Names._STYLE_CSSCLASS_EDIT;
                chk.Text = pickListTerm.PickListItems[0].Value;
                pnl.Controls.Add(chk);
            }
            else
            {
                if (pickListTerm.MultiSelect ?? false)  // multi-select picklist: MultiSelectGrid control
                {
                    pnl.Height = Unit.Pixel(4 + 20 * Math.Min(4, pickListTerm.PickListItems.Count));
                    pnl.Width = Unit.Percentage(100.0);

                    MultiSelectGrid grd = new MultiSelectGrid();
                    grd.ID = pnl.ID + "_lst";
                    grd.CheckboxColumn = 0;
                    grd.DataSource = pickListTerm.PickListItems;
                    grd.BoundColumns = "Value";
                    //grd.DataKeyNames = new string[] { "Value" };
                    grd.Container = pnl.ID;
                    grd.HeaderContainer = "";
                    grd.AutoGenerateColumns = false;
                    grd.RowHighlighting = false;
                    grd.EnableClickEvent = false;
                    grd.EnableDoubleClickEvent = false;
                    grd.EnableHeaderClick = false;
                    grd.ShowHeader = false;
                    grd.ColumnWidths = "100%";
                    grd.DataBind();
                    pnl.Controls.Add(grd);
                }
                else
                {
                    if (pickListTerm.PickListItems.Count == 2)   //2 item single-select picklist: RadioButtonList
                    {
                        RadioButtonList rdolst = new RadioButtonList();
                        rdolst.ID = pnl.ID + "_lst";
                        rdolst.CellPadding = 0;
                        rdolst.CellSpacing = 0;
                        rdolst.RepeatDirection = RepeatDirection.Vertical;
                        AddListItems(rdolst, pickListTerm.PickListItems);
                        pnl.Controls.Add(rdolst);
                    }
                    else          // 3 or more item single-select list: DropDrownList
                    {
                        DropDownList ddl = new DropDownList();
                        ddl.ID = pnl.ID + "_lst";
                        ddl.Width = Unit.Percentage(100.0);
                        ddl.Items.Add(new ListItem("(Select One)", ""));
                        AddListItems(ddl, pickListTerm.PickListItems);
                        pnl.Controls.Add(ddl);
                    }
                }
            }
            return pnl;
        }

		#endregion


		#region Helper methods when creating term controls

		private static void AddListItems(ListControl pickListControl, List<PickListItem> pickListItems)
		{
			foreach (PickListItem item in pickListItems)
			{
				ListItem listItem = new ListItem(item.Value, item.Value, true);
				pickListControl.Items.Add(listItem);
			}
		}


		// Generic textbox (to be used within other terms)
		public static TextBox CreateTextControl(string termName, int maxLength, Unit width, int rows)
		{
			TextBox txt = new TextBox();
			txt.ID = ControlID(termName);
			txt.Width = width;
			if (rows > 1)
			{
				txt.TextMode = TextBoxMode.MultiLine;
				txt.Rows = rows;
			}
			else
			{
				txt.TextMode = TextBoxMode.SingleLine;
				txt.MaxLength = maxLength;
			}
			return txt;
		}


		public static HtmlContainerControl DateRangeControl(string termName)
		{
			HtmlContainerControl pnl = new HtmlGenericControl("span");
			pnl.ID = ControlID(termName) + "_pnl";

			Label lbl1 = new Label();
			lbl1.Style["margin"] = "0 3px 0 0";
			lbl1.Text = "From: ";
			pnl.Controls.Add(lbl1);

			//write 1st textbox to contain date					
			TextBox txtDate1 = new TextBox();   //pnl.Controls[0]
			txtDate1.Style["width"] = "114px";
			txtDate1.Style["height"] = "18px";
			txtDate1.Style["padding-top"] = "0";
			txtDate1.MaxLength = 12;
			txtDate1.ID = ControlID(termName) + Common.Names._IDENTIFIER_StartDate;
			txtDate1.Attributes["onfocus"] = "javascript:this.select();";
			pnl.Controls.Add(txtDate1);

			//write 1st "button" image and click event that will display calendar IFRAME
			Image imgDate1 = new Image();    //pnl.Controls[1]
			imgDate1.Style["cursor"] = "hand";
			imgDate1.ID = ControlID(termName) + Common.Names._IDENTIFIER_StartDate + "_img";
			imgDate1.ImageUrl = "/Global/bin/Calendar/calendar.gif";
			imgDate1.Height = Unit.Parse("19px");
			imgDate1.Width = Unit.Parse("34px");
			imgDate1.ImageAlign = ImageAlign.Top;
			imgDate1.BorderWidth = Unit.Parse("0px");
			imgDate1.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtDate1.ClientID);
			pnl.Controls.Add(imgDate1);

			Label lbl2 = new Label();
			lbl2.Style["margin"] = "0 4px 0 15px";
			lbl2.Text = "To:";
			pnl.Controls.Add(lbl2);

			//write 2nd textbox to contain date					
			TextBox txtDate2 = new TextBox();   //pnl.Controls[0]
			txtDate2.Style["width"] = "114px";
			txtDate2.Style["height"] = "18px";
			txtDate2.Style["padding-top"] = "0";
			txtDate2.MaxLength = 12;
			txtDate2.ID = ControlID(termName) + Common.Names._IDENTIFIER_EndDate;
			txtDate2.Attributes["onfocus"] = "javascript:this.select();";
			pnl.Controls.Add(txtDate2);

			//write 2nd "button" image and click event that will display calendar IFRAME
			Image imgDate2 = new Image();    //pnl.Controls[1]
			imgDate2.Style["cursor"] = "hand";
			imgDate2.ID = ControlID(termName) + Common.Names._IDENTIFIER_EndDate + "_img";
			imgDate2.ImageUrl = "/Global/bin/Calendar/calendar.gif";
			imgDate2.Height = Unit.Parse("19px");
			imgDate2.Width = Unit.Parse("34px");
			imgDate2.ImageAlign = ImageAlign.Top;
			imgDate2.BorderWidth = Unit.Parse("0px");
			imgDate2.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtDate2.ClientID);
			pnl.Controls.Add(imgDate2);

			return pnl;
		}

		// Generic date control (to be used within other terms)
		public static HtmlContainerControl CreateDateControl(string termName)
		{
			HtmlContainerControl pnl =  new HtmlGenericControl("span");
			pnl.ID = ControlID(termName) + "_pnl";
			
			//write textbox to contain date					
			TextBox txtDate = new TextBox();   //pnl.Controls[0]
			txtDate.Style["width"] = "114px";
			txtDate.Style["height"] = "18px";
			txtDate.Style["padding-top"] = "0";
			txtDate.MaxLength = 12;
			txtDate.ID = ControlID(termName);
			txtDate.Attributes["onfocus"] = "javascript:this.select();";
			pnl.Controls.Add(txtDate);

			//write "button" image and click event that will display calendar IFRAME
			Image imgDate = new Image();    //pnl.Controls[1]
			imgDate.Style["cursor"] = "hand";
			imgDate.ID = ControlID(termName) + "_img";
			imgDate.ImageUrl = "/Global/bin/Calendar/calendar.gif";
			imgDate.Height = Unit.Parse("19px");
			imgDate.Width = Unit.Parse("34px");
			imgDate.ImageAlign = ImageAlign.Top;
			imgDate.BorderWidth = Unit.Parse("0px");
			imgDate.Attributes["onclick"] = string.Format("window.event.cancelBubble=true; ShowCalendar(document.all.CalFrame, window.frames.CalFrame, null, document.getElementById('{0}'), null, -600, 3600);", txtDate.ClientID);
			pnl.Controls.Add(imgDate);

			return pnl;
		}


		public static string ControlID(string termName)
		{
			return "ctl_" + termName.Replace(' ', '_');
		}

		public static string ControlID(string termName, string suffix)
		{
			return ControlID(termName) + suffix;
		}

		public static string ControlID(string termName, TermType termType)
		{
			switch (termType)
			{
				case TermType.Facility:
				case TermType.PickList:
				case TermType.MSO:
					return ControlID(termName, "_lst");
				case TermType.ComplexList:
				case TermType.Link:
				case TermType.None:
				case TermType.Text:
				case TermType.Date:
				case TermType.Renewal:
				default:
					return ControlID(termName);
			}
		}



		public static  void FormatDataCell(Control cell, bool canEdit)
		{
			WebControl webControl = cell as WebControl;
			if (webControl != null)
			{
				webControl.Attributes["class"] = (canEdit ? Common.Names._STYLE_CSSCLASS_EDIT : Common.Names._STYLE_CSSCLASS_EDITREADONLY);
				webControl.Attributes["width"] = "100%";
			}
			else
			{
				HtmlControl htmlControl = cell as HtmlControl;
				if (htmlControl != null)
				{
					htmlControl.Attributes["class"] = (canEdit ? Common.Names._STYLE_CSSCLASS_EDIT : Common.Names._STYLE_CSSCLASS_EDITREADONLY);
					htmlControl.Attributes["width"] = "100%";
				}
			}
		}

		public static void FormatCaptionCell(TableCell cell, Term term)
		{
			if (term.Runtime.HasError)
			{
				cell.CssClass = Common.Names._STYLE_CSSCLASS_PROFILE_VALIDATIONERROR;
				//string msg = term.Runtime.ErrorMessage.Replace(@"\n", @"<br />").Replace("'", "&#x0022;").Replace("\"", "&#x0027;");
				cell.Attributes.Add("onmouseover", "ShowItatToolTip(event, 'itatToolTip', '" + term.Runtime.ErrorMessage + "')");
				cell.Attributes.Add("onmouseout", "HideItatToolTip('itatToolTip');");
				cell.Attributes.Add("onmousemove", "MoveItatToolTip(event, 'itatToolTip');");
			}
			else
			{
				cell.CssClass = Common.Names._STYLE_CSSCLASS_CAPTION;
			}
			cell.Width = Unit.Pixel(150); 
			if (term.Required ?? false)
				cell.Text = string.Concat("* ", term.Name);
			else
				cell.Text = term.Name;

		}

		public static void FormatCaptionCell(TableCell cell)
		{
			cell.CssClass = Common.Names._STYLE_CSSCLASS_CAPTION;
			cell.Width = Unit.Pixel(150);
		}


		public static void FormatCaptionCell(HtmlTableCell cell)
		{
			cell.Attributes["class"] = Common.Names._STYLE_CSSCLASS_CAPTION;
			cell.Width = "150px"; 
		}

		

		#endregion


		//Creates HTML to use as an initial value when creating a new instance of HTML Text to be edited using the r.a.d.editor.
		public static string DefaultEditorHtml(Telerik.WebControls.RadEditor edt)
		{
			string defaultFontFace = edt.FontNames[0];
			int defaultFontSize = 3;
			return string.Format("<p align=\"left\"><font face=\"{0}\" size=\"3\"></font></p>", defaultFontFace, defaultFontSize);
		}


		public static string GetVersion()
		{
			System.Reflection.Assembly asm = System.Reflection.Assembly.GetExecutingAssembly();
			Version ver = asm.GetName().Version;
			return string.Format("{0}.{1}.{2}.{3}", ver.Major, ver.Minor, ver.Build, ver.Revision);
		}

        #region ComplexList Validation

        public static Dictionary<string /* Term Name */, List<string> /* Error Messages */ > GetComplexListTermTypeErrors(
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup,
            Business.ComplexList complexList,
            int? itemIndex)
        {
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > errorMessages = new Dictionary<string, List<string>>();

            if (canEditTermGroup == null || canEditTermGroup[complexList.TermGroupID])
            {
                //Use itemIndex to determine if this check is for one itemValue or all itemValues
                if (itemIndex.HasValue)
                {
                    ComplexListItem item = complexList.Items[itemIndex.Value];
                    for (int index = 0; index < item.ItemValues.Count; index++)
                    {
                        GetComplexListItemTermTypeErrors(errorMessages, item.ItemValues[index], itemIndex, complexList.Name);
                    }
                }
                else
                {
                    foreach (ComplexListItem item in complexList.Items)
                    {
                        for (int index = 0; index < item.ItemValues.Count; index++)
                        {
                            GetComplexListItemTermTypeErrors(errorMessages, item.ItemValues[index], null, null);
                        }
                    }
                }
            }
            return errorMessages;
        }

        private static string GetTabMessage(string complexListName, int index)
        {
            string tabMessage = string.Format("row {0:D}", index + 1);
            if (!string.IsNullOrEmpty(complexListName))
                tabMessage = string.Format("{0}, row {1:D}", complexListName, index + 1);
            return tabMessage;
        }

        private static void GetComplexListItemTermTypeErrors(
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > errorMessages,
            ComplexListItemValue itemValue,
            int? itemIndex,
            string complexListName)
        {
            Term term = itemValue.Term;
            string termName = itemValue.FieldName;
            term.ReportName = termName;
            term.Runtime.Clear(true, term.Required ?? false);
            itemValue.FieldFilterTerm.Runtime.Clear(true, term.Required ?? false);

            string tabMessage = null;
            if (itemIndex.HasValue)
                tabMessage = GetTabMessage(complexListName, itemIndex.Value);

            string testValueError = term.TestValue(termName, tabMessage, itemValue.FieldValue);
            if (string.IsNullOrEmpty(testValueError))
            {
                term.SetValue(itemValue.FieldValue);

                List<string> sErrors = term.CheckType(!string.IsNullOrEmpty(tabMessage), tabMessage);
                if (sErrors != null && sErrors.Count > 0)
                {
                    if (!errorMessages.ContainsKey(itemValue.FieldName))
                    {
                        itemValue.FieldFilterTerm.Runtime.HasError = true;
                        if (sErrors.Count > 1)
                            itemValue.FieldFilterTerm.Runtime.ErrorMessage = string.Join("\\n", sErrors.ToArray());
                        else
                            itemValue.FieldFilterTerm.Runtime.ErrorMessage = sErrors[0];
                        errorMessages.Add(itemValue.FieldName, sErrors);
                    }
                }
            }
            else
            {
                if (!errorMessages.ContainsKey(itemValue.FieldName))
                {
                    itemValue.FieldFilterTerm.Runtime.HasError = true;
                    itemValue.FieldFilterTerm.Runtime.ErrorMessage = testValueError;
                    errorMessages.Add(itemValue.FieldName, new List<string>(new string[] { testValueError }));
                }
            }
        }

        public static List<string> GetComplexListValidationErrors(
            Dictionary<Guid /*TermGroupID*/, bool /*CanEdit*/> canEditTermGroup,
            ManagedItemValidationType validationType,
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > allTermTypeErrors,
            List<Business.ComplexListItem> items,
            bool includeName,
            Business.SecurityModel securityModel,
            string currentStatus,
            Guid? termGroupID,
            string complexListName)
        {
            List<string> validationResultsAllTermAlert = new List<string>();

            if (canEditTermGroup == null || (termGroupID.HasValue && canEditTermGroup[termGroupID.Value]))
            {
                bool includeTab = securityModel == SecurityModel.Advanced;
                for (int itemIndex = 0; itemIndex < items.Count; itemIndex++)
                {
                    GetComplexListItemValidationErrors(validationType, allTermTypeErrors, items[itemIndex], validationResultsAllTermAlert, includeTab, itemIndex, includeName, currentStatus, complexListName);
                }
            }
            return validationResultsAllTermAlert;
        }

        public static void GetComplexListItemValidationErrors(
            ManagedItemValidationType validationType,
            Dictionary<string /* Term Name */, List<string> /* Error Messages */ > allTermTypeErrors,
            Business.ComplexListItem item,
            List<string> validationResultsAllTermAlert,
            bool includeTab,
            int itemIndex,
            bool includeName,
            string currentStatus,
            string complexListName)
        {
            string tabMessage = GetTabMessage(complexListName, itemIndex);

            foreach (ComplexListItemValue itemValue in item.ItemValues)
            {
                Term term = itemValue.Term;
                string termName = itemValue.FieldName;
                term.ReportName = termName;
                term.Runtime.Clear(true, term.Required ?? false);
                //Note - It is expected that 'itemValue.FieldFilterTerm.Runtime.Clear' has been called prior to this method...
                string testValueError = term.TestValue(termName, tabMessage, itemValue.FieldValue);
                term.SetValue(itemValue.FieldValue);

                List<string> validationResultsTermAlert = new List<string>();
                List<string> termValidate = term.Validate(includeTab, tabMessage);

                if (!string.IsNullOrEmpty(testValueError))
                {
                    if (!termValidate.Contains(testValueError))
                        termValidate.Add(testValueError);
                }

                if (validationType == ManagedItemValidationType.FullValidation || (validationType == ManagedItemValidationType.ValidateOnSave && term.ValidationStatuses.Contains(currentStatus)))
                {
                    if (termValidate.Count > 0)
                    {
                        itemValue.FieldFilterTerm.Runtime.HasError = true;
                        if (termValidate.Count > 1)
                            itemValue.FieldFilterTerm.Runtime.ErrorMessage = string.Join("\\n", termValidate.ToArray());
                        else
                            itemValue.FieldFilterTerm.Runtime.ErrorMessage = termValidate[0];
                        validationResultsTermAlert.AddRange(termValidate);
                    }
                }
                ConsolidateErrors(termName, allTermTypeErrors, validationResultsTermAlert, validationResultsAllTermAlert);
            }
        }

        private static void ConsolidateErrors(
                        string termName,
                        Dictionary<string /* Term Name */, List<string> /* Error Messages */ > allTermTypeErrors,
                        List<string> validationResultsTermAlert,
                        List<string> validationResultsAllTermAlert)
        {
            List<string> termTypeErrors = null;

            if (allTermTypeErrors != null && allTermTypeErrors.ContainsKey(termName))
                termTypeErrors = allTermTypeErrors[termName];
            if (termTypeErrors != null && termTypeErrors.Count > 0)
            {
                if (validationResultsTermAlert == null)
                    validationResultsTermAlert = new List<string>();
                foreach (string sMessage in termTypeErrors)
                {
                    if (!validationResultsTermAlert.Contains(sMessage))
                        validationResultsTermAlert.Add(sMessage);
                }
            }

            if (validationResultsTermAlert != null)
            {
                validationResultsAllTermAlert.AddRange(validationResultsTermAlert);
            }
        }
        #endregion


       
    }
}

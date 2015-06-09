using System;
using System.Data;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;

namespace Kindred.Knect.ITAT.Web
{
	public partial class TermDependencyEdit : BaseTemplatePage
	{

		#region private members

		protected Business.TermDependency _termDependency;
		protected EditMode _editMode;
		private Business.TermDependencyType _selectedDependentTermType = TermDependencyType.All;

		private const string _KH_VS_TERMDEPENDENCY = "_kh_vs_TermDependency";
		private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";

		private const string CONDITION_VALUE_SEPARATOR = "~~~";
		#endregion


		#region base class overrides

		protected override TemplateHeader HeaderControl()
		{
			return null;
		}

		internal override HtmlGenericControl HTMLBody()
		{
			return this.body;
		}

		internal override Control ResizablePanel()
		{
			return editBody;
		}

		#endregion

		#region override event handlers
		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

			if (!string.IsNullOrEmpty(ddlTermDependencyType.SelectedValue))
				_selectedDependentTermType = (Business.TermDependencyType)Enum.Parse(typeof(Business.TermDependencyType), ddlTermDependencyType.SelectedValue);

			if (IsPostBack)
			{
				ClientScript.RegisterStartupScript(this.GetType(), "_kh_startupvisibility", string.Format("ShowPopup({0})", (hfAddCondition.Value == "1").ToString().ToLower()), true);
			}
			else
			{
				GetContextData();
				LoadStaticDropDowns();

				TermDependencyType termDependencyType = TermDependencyType.None;
				switch (_termDependency.Target)
				{
					case Business.DependencyTarget.Workflow:
						((StandardHeader)header).PageTitle = "Edit Workflow Dependency";
						break;

					case Business.DependencyTarget.Term:
						((StandardHeader)header).PageTitle = "Edit Term Dependency";
						if (_termDependency.DependentTermIDs.Count > 1)
						{
							foreach (Guid dependentTermID in _termDependency.DependentTermIDs)
							{
								Term term = _template.FindTerm(dependentTermID);
								if (term is ComplexList)
								{
									termDependencyType = TermDependencyType.ComplexList;
									break;
								}
							}
							if (termDependencyType != TermDependencyType.ComplexList)
								termDependencyType = TermDependencyType.TextDatePicklist;
						}
						else
						{
							termDependencyType = TermDependencyType.All;
						}
						break;
				}

				LoadDependentTerms(true, termDependencyType);
				SetValues();
				SetTarget(termDependencyType, _editMode == EditMode.Edit);
			}
		}

		#region OnLoad

		//OnLoad, NotPostBack
		private void SetValues()
		{
			ddlQuantifier.SelectedValue = _termDependency.Quantifier.ToString();
			lstConditions.Items.Clear();
			foreach (Business.TermDependencyCondition condition in _termDependency.Conditions)
				AddConditionListItem(_template.FindTermName(condition.SourceTermID, condition.SourceTerm), condition.Oper, condition.Value1, condition.Value2);
			ddlActionEnabled.SelectedValue = _termDependency.Action.Enabled.ToString();
			ddlActionRequired.SelectedValue = _termDependency.Action.Required.ToString();
			cbActive.Checked = _termDependency.IsActive ?? false;

			try { ddlDependentTerm.SelectedValue = _termDependency.DependentTermIDs[0].ToString(); }
			catch { ddlDependentTerm.SelectedIndex = 0; }
		}

		//OnLoad, not Postback
		//ddlDependentTermOnSelectedIndexChanged
		private void SetTarget(TermDependencyType termDependencyType, bool preFill)
		{
			switch (_termDependency.Target)
			{
				case Business.DependencyTarget.Workflow:
					divActionSetActiveWorkflow.Visible = true;

					rowDDLTermDependencyType.Visible = false;
					rowDDLDependentTerm.Visible = false;
					rowListBoxDependentTerms.Visible = false;
					divActionEnabled.Visible = false;
					divActionRequired.Visible = false;
					divActionSetValue.Visible = false;
					Active.Visible = false;
					cbActive.Visible = false;

					ddlActionSetActiveWorkflow.Items.Clear();
					foreach (Business.Workflow workflow in _template.Workflows)
					{
						AddListItem(ddlActionSetActiveWorkflow, workflow.Name, workflow.ID.ToString());
					}
					if (preFill)
						ddlActionSetActiveWorkflow.SelectedValue = _termDependency.Action.SetValue;
					break;

				case Business.DependencyTarget.Term:
					Business.Term term = null;

					divActionSetActiveWorkflow.Visible = false;
					rowDDLTermDependencyType.Visible = true;
					divActionEnabled.Visible = true;
					Active.Visible = true;
					cbActive.Visible = true;

					divActionSetValue.Visible = false;      //Note - this is needed only for the case of 'All' and a Picklist is selected.

					switch (termDependencyType)
					{
						case TermDependencyType.All:
							rowDDLDependentTerm.Visible = true;
							rowListBoxDependentTerms.Visible = false;
							divActionRequired.Visible = true;

							if (preFill)
							{
								try { term = _template.FindTerm(_termDependency.DependentTermIDs[0]); }
								catch { }
							}
							else
							{
								try { term = _template.FindTerm(new Guid(ddlDependentTerm.SelectedValue)); }
								catch { }
							}

							if (term != null && _template.BasicTermExists(term.Name) && term.TermType == Business.TermType.PickList)
							{
								//PickList
								divActionSetValue.Visible = true;
								ddlActionSetValue.Items.Clear();
								AddListItem(ddlActionSetValue, Business.Term._SET_VALUE_DEFAULT, Business.Term._SET_VALUE_DEFAULT);
								foreach (Business.PickListItem item in (term as Business.PickListTerm).PickListItems)
								{
									AddListItem(ddlActionSetValue, item.Value, item.Value);
								}
								if (preFill)
								{
									try { ddlActionSetValue.SelectedValue = _termDependency.Action.SetValue; }
									catch { ddlActionSetValue.SelectedIndex = 0; }
								}
							}
							break;

						case TermDependencyType.TextDatePicklist:
							rowDDLDependentTerm.Visible = false;
							rowListBoxDependentTerms.Visible = true;
							divActionRequired.Visible = true;
							break;

						case TermDependencyType.ComplexList:
							rowDDLDependentTerm.Visible = false;
							rowListBoxDependentTerms.Visible = true;
							divActionRequired.Visible = false;
							break;

						default:
							break;
					}
					break;
			}
		}

		//OnLoad, NotPostBack
		private void GetContextData()
		{
			_template = (Business.Template)Context.Items[Common.Names._CNTXT_Template];
			_editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
			_termDependency = (Business.TermDependency)Context.Items[Common.Names._CNTXT_TermDependency];
			if (_termDependency == null)
				throw new Exception("Error retrieving Term Dependency.");
		}

		//OnLoad, NotPostBack
		private void LoadStaticDropDowns()
		{
			ddlTermDependencyType.Items.Clear();
			ddlTermDependencyType.Items.Add(new ListItem("All", Business.TermDependencyType.All.ToString()));
			ddlTermDependencyType.Items.Add(new ListItem("Text-Date-Picklist", Business.TermDependencyType.TextDatePicklist.ToString()));
			ddlTermDependencyType.Items.Add(new ListItem("ComplexList", Business.TermDependencyType.ComplexList.ToString()));

			ddlSourceTerm.Items.Add(new ListItem("(Select a Term)", ""));
			foreach (Business.Term basicTerm in BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments))
			{
				switch (basicTerm.TermType)
				{
					case Kindred.Knect.ITAT.Business.TermType.Text:
					case Kindred.Knect.ITAT.Business.TermType.Date:
					case Kindred.Knect.ITAT.Business.TermType.PickList:
						AddListItem(ddlSourceTerm, basicTerm.Name, basicTerm.ID.ToString());
						break;
					//TODO:  (MAYBE) support other term types
					default:
						break;
				}
			}

			ddlQuantifier.Items.Clear();
			ddlQuantifier.Items.Add(new ListItem("ALL conditions must be met", Business.DependencyQuantifier.All.ToString()));
			ddlQuantifier.Items.Add(new ListItem("ANY condition can be met", Business.DependencyQuantifier.Any.ToString()));

			ddlActionEnabled.Items.Clear();
			ddlActionRequired.Items.Clear();
			Business.TermDependencyActionValue[] actions = (Business.TermDependencyActionValue[])Enum.GetValues(typeof(Business.TermDependencyActionValue));
			for (int i = 0; i < actions.Length; i++)
			{
				string actionName = actions[i].ToString();
				AddListItem(ddlActionEnabled, actionName, "");
				AddListItem(ddlActionRequired, actionName, "");
			}
		}

		private void LoadDependentTerms(bool clear, Business.TermDependencyType termType)
		{
			if (clear)
			{
				ddlDependentTerm.Items.Clear();
				ddlDependentTerm.Items.Add(new ListItem("(Select a Term)", ""));
				lstbxDependentTerms.Items.Clear();
			}

			switch (termType)
			{
				case TermDependencyType.TextDatePicklist:
					rowListBoxDependentTerms.Visible = true;
					foreach (Business.Term basicTerm in BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments))
					{
						switch (basicTerm.TermType)
						{
							case Kindred.Knect.ITAT.Business.TermType.Text:
							case Kindred.Knect.ITAT.Business.TermType.Date:
							case Kindred.Knect.ITAT.Business.TermType.PickList:
								if (_termDependency.DependentTermIDs != null)
									AddListItem(lstbxDependentTerms, basicTerm.Name, basicTerm.ID.ToString(), _termDependency.DependentTermIDs.Contains(basicTerm.ID));
								else
									AddListItem(lstbxDependentTerms, basicTerm.Name, basicTerm.ID.ToString());
								break;
						}
					}
					break;

				case TermDependencyType.ComplexList:
					rowListBoxDependentTerms.Visible = true;
					foreach (Business.Term complexList in _template.ComplexLists)
					{
						if (_termDependency.DependentTermIDs != null)
							AddListItem(lstbxDependentTerms, complexList.Name, complexList.ID.ToString(), _termDependency.DependentTermIDs.Contains(complexList.ID));
						else
							AddListItem(lstbxDependentTerms, complexList.Name, complexList.ID.ToString());
					}
					break;

				case TermDependencyType.All:
					rowListBoxDependentTerms.Visible = false;
					foreach (Business.Term basicTerm in BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments))
					{
						switch (basicTerm.TermType)
						{
							case Kindred.Knect.ITAT.Business.TermType.Text:
							case Kindred.Knect.ITAT.Business.TermType.Date:
							case Kindred.Knect.ITAT.Business.TermType.PickList:
								AddListItem(ddlDependentTerm, basicTerm.Name, basicTerm.ID.ToString());
								break;
						}
					}
					foreach (Business.Term complexList in _template.ComplexLists)
					{
						AddListItem(ddlDependentTerm, complexList.Name, complexList.ID.ToString());
					}
					break;

				default:
					break;
			}
			rowDDLDependentTerm.Visible = !rowListBoxDependentTerms.Visible;
		}

		#endregion

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);
			RegisterDeleteConditionClientScript();
		}

		private void RegisterDeleteConditionClientScript()
		{
			Type t = this.GetType();
			string scriptName = "_kh_DeleteCondition";
			System.IO.StringWriter sw = new System.IO.StringWriter();
			sw.WriteLine("	function DeleteSelectedCondition() ");
			sw.WriteLine("	{");
			sw.WriteLine("		if (confirm('Are you sure you want to delete this condition?')) ");
			sw.WriteLine("		{ ");
			sw.WriteLine("			var options = document.getElementById('{0}').options; ", lstConditions.ClientID);
			sw.WriteLine("			var index = options.selectedIndex; ");
			sw.WriteLine("			if (index > -1) ");
			sw.WriteLine("				options.remove(index); ");
			sw.WriteLine("		} ");
			sw.WriteLine("	}");

			sw.WriteLine("function SetDeleteButtonEnabled() ");
			sw.WriteLine("{ ");
			sw.WriteLine("	var btn = document.getElementById('{0}'); ", btnDeleteCondition.ClientID);
			sw.WriteLine("	var lst = document.getElementById('{0}'); ", lstConditions.ClientID);
			sw.WriteLine("	btn.disabled = (lst.selectedIndex == -1);  ");
			sw.WriteLine("} ");

			sw.WriteLine("function ShowPopup(bool) ");
			sw.WriteLine("{ ");
			sw.WriteLine("	var div = document.getElementById('{0}'); ", divCondition.ClientID);
			sw.WriteLine("	var ifr = document.getElementById('ifrBlocker');  ");
			sw.WriteLine("	var hf = document.getElementById('{0}'); ", hfAddCondition.ClientID);
			sw.WriteLine("	if (bool) ");
			sw.WriteLine("	{ ");
			sw.WriteLine("		div.style.display='block'; ");
			sw.WriteLine("		ifr.style.top = div.style.top; ");
			sw.WriteLine("		ifr.style.left = div.style.left; ");
			sw.WriteLine("		ifr.style.height = div.style.height; ");
			sw.WriteLine("		ifr.style.width = div.style.width; ");
			sw.WriteLine("		ifr.style.zIndex = div.style.zIndex - 1; ");
			sw.WriteLine("		ifr.style.display='block'; ");
			sw.WriteLine("		hf.value = '1'; ");
			sw.WriteLine("	} ");
			sw.WriteLine("	else ");
			sw.WriteLine("	{ ");
			sw.WriteLine("		ifr.style.display='none'; ");
			sw.WriteLine("		div.style.display='none'; ");
			sw.WriteLine("		hf.value = '0'; ");
			sw.WriteLine("	} ");
			sw.WriteLine("} ");

			if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
				ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);

			ClientScript.RegisterStartupScript(t, "_kh_startupbuttonenabled", "SetDeleteButtonEnabled(); ", true);
		}

		#endregion

		#region button event handlers
		protected void btnDeleteConditionOnCommand(object sender, EventArgs e)
		{
			if (lstConditions.SelectedIndex > -1)
				lstConditions.Items.RemoveAt(lstConditions.SelectedIndex);
		}

		protected void btnOK_Click(object sender, EventArgs e)
		{
			SetContextDataAndReturn(true);
		}

		protected void btnCancel_Click(object sender, EventArgs e)
		{
			SetContextDataAndReturn(false);
		}

		//btnOK_Click
		//btnCancel_Click
		private void SetContextDataAndReturn(bool updateValues)
		{
			Business.DependencyTarget termDependencyTarget = _termDependency.Target;
			//If the user clicks Cancel and the EditMode == Add, then remove the newly added term from the collection
			if (_editMode == EditMode.Add && !updateValues)
			{
				_template.RemoveTermDependency(_termDependency.ID);
				_termDependency = null;
			}

			List<string> errors = ValidateForm();
			if (updateValues && (errors != null) && (errors.Count > 0))
			{
				System.IO.StringWriter sw = new System.IO.StringWriter();
				sw.Write("Unable to save the term dependency due to the following errors:\\n\\n");
				foreach (string error in errors)
				{
					sw.Write(error);
					sw.Write("\\n");
				}
				sw.Write("\\nPlease correct the error(s) and try again.");
				RegisterAlert(sw.ToString());
			}
			else
			{
				if (updateValues)
					UpdateValues();
				Context.Items[Common.Names._CNTXT_TermDependency] = _termDependency;
				Context.Items[Common.Names._CNTXT_Template] = _template;
				//IsChanged related change
				if (IsChangedInitial)
					Context.Items[Common.Names._CNTXT_IsChanged] = true;
				else if (IsChanged)
					Context.Items[Common.Names._CNTXT_IsChanged] = updateValues;
				else
					Context.Items[Common.Names._CNTXT_IsChanged] = false;

				switch (termDependencyTarget)
				{
					case Business.DependencyTarget.Workflow:
						Server.Transfer("TemplateWorkflows.aspx");
						break;
					case Business.DependencyTarget.Term:
						Server.Transfer("TemplateTermDependencies.aspx");
						break;
				}
			}
		}

		//SetContextDataAndReturn
		private void UpdateValues()
		{
			if (_termDependency == null)
				throw new NullReferenceException("_termDependency is null");

			try
			{
				_termDependency.DependentTermIDs.Clear();
				if (lstbxDependentTerms.Visible)
				{
					foreach (ListItem listItem in lstbxDependentTerms.Items)
					{
						if (listItem.Selected)
							_termDependency.DependentTermIDs.Add(new Guid(listItem.Value));
					}
				}
				else
				{
					_termDependency.DependentTermIDs.Add(new Guid(ddlDependentTerm.SelectedValue));
				}
			}
			catch (Exception)
			{
			}
			_termDependency.Quantifier = (Business.DependencyQuantifier)Enum.Parse(typeof(Business.DependencyQuantifier), ddlQuantifier.SelectedValue);

			_termDependency.Conditions.Clear();
			foreach (ListItem item in lstConditions.Items)
			{
				string[] values = item.Value.Split(new string[] { CONDITION_VALUE_SEPARATOR }, StringSplitOptions.None);
				_termDependency.Conditions.Add(new Business.TermDependencyCondition(_template, values[0], values[1], values[2], values[3]));
			}

			_termDependency.Action.Enabled = (Business.TermDependencyActionValue)Enum.Parse(typeof(Business.TermDependencyActionValue), ddlActionEnabled.SelectedValue);
			_termDependency.Action.Required = (Business.TermDependencyActionValue)Enum.Parse(typeof(Business.TermDependencyActionValue), ddlActionRequired.SelectedValue);
			if (_termDependency.Target == Kindred.Knect.ITAT.Business.DependencyTarget.Workflow)
				_termDependency.Action.SetValue = ddlActionSetActiveWorkflow.SelectedValue;
			else
			{
				if (ddlActionSetValue.Visible)
					_termDependency.Action.SetValue = ddlActionSetValue.SelectedValue;
				else
					_termDependency.Action.SetValue = null;
			}

			_termDependency.IsActive = cbActive.Checked;
		}

		protected void btnAddConditionOKOnClick(object sender, EventArgs e)
		{
			string value1 = GetValue1();
			string value2 = GetValue2();
			AddConditionListItem(ddlSourceTerm.SelectedItem.Text, ddlOperator.SelectedValue, value1, value2);
			ddlSourceTerm.SelectedIndex = 0;
		}

		//btnAddConditionOKOnClick
		private string GetValue1()
		{
			if (txtValue1.Visible)
				return txtValue1.Text;
			if (dateValue1.Visible)
				return dateValue1.Text;
			if (ddlValue1.Visible)
				return ddlValue1.SelectedValue;
			return string.Empty;
		}

		//btnAddConditionOKOnClick
		private string GetValue2()
		{
			if (txtValue2.Visible)
				return txtValue2.Text;
			if (dateValue2.Visible)
				return dateValue2.Text;
			if (ddlValue2.Visible)
				return ddlValue2.SelectedValue;
			return string.Empty;
		}

		#endregion

		#region dropdown event handlers
		protected void ddlTermDependencyTypeOnSelectedIndexChanged(object sender, EventArgs e)
		{
			Business.TermDependencyType selection = (Business.TermDependencyType)Enum.Parse(typeof(Business.TermDependencyType), ddlTermDependencyType.SelectedItem.Value);
			LoadDependentTerms(true, selection);
			SetTarget(selection, false);
		}

		protected void ddlDependentTermOnSelectedIndexChanged(object sender, EventArgs e)
		{
			SetTarget(TermDependencyType.All, false);
		}

		protected void ddlOperatorOnSelectedIndexChanged(object sender, EventArgs e)
		{
			SetValueVisibility();
		}

		protected void ddlSourceTermOnSelectedIndexChanged(object sender, EventArgs e)
		{
			if (ddlSourceTerm.SelectedIndex > 0)
			{
				ddlOperator.Visible = true;
				if (ddlSourceTerm.SelectedItem.Text == "UserRole")
				{
					ddlOperator.Items.Clear();
					ddlOperator.Items.Add(new ListItem("(Select One)", ""));
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_InRole, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NotInRole, "");
				}
				else
				{
					Business.Term term = _template.FindTerm(new Guid(ddlSourceTerm.SelectedValue));
					LoadOperatorDropDown(term);
				}
			}
			else
			{
				ddlOperator.Visible = false;
			}
			SetValueVisibility();
		}

		//ddlOperatorOnSelectedIndexChanged
		//ddlSourceTermOnSelectedIndexChanged
		private void SetValueVisibility()
		{
			//TODO: (MAYBE) support different option for different text types (currency, number, phone, etc)
			if (ddlOperator.SelectedIndex > 0 && ddlSourceTerm.SelectedIndex > 0)
			{
				Business.Term term = _template.FindTerm(new Guid(ddlSourceTerm.SelectedValue));
				switch (ddlOperator.SelectedValue)
				{
					case Business.XMLNames._TermDependencyOperator_Equals:
					case Business.XMLNames._TermDependencyOperator_GreaterThan:
					case Business.XMLNames._TermDependencyOperator_NoLessThan:
					case Business.XMLNames._TermDependencyOperator_LessThan:
					case Business.XMLNames._TermDependencyOperator_NoMoreThan:
					case Business.XMLNames._TermDependencyOperator_NotEqual:
					case Business.XMLNames._TermDependencyOperator_Contains:
					case Business.XMLNames._TermDependencyOperator_StartsWith:
					case Business.XMLNames._TermDependencyOperator_EndsWith:
						switch (term.TermType)
						{
							case Kindred.Knect.ITAT.Business.TermType.Text:
								txtValue1.Visible = true;
								txtValue2.Visible = false;
								dateValue1.Visible = false;
								dateValue2.Visible = false;
								dateImage1.Visible = false;
								dateImage2.Visible = false;
								ddlValue1.Visible = false;
								ddlValue2.Visible = false;
								break;
							case Kindred.Knect.ITAT.Business.TermType.PickList:
								txtValue1.Visible = false;
								txtValue2.Visible = false;
								dateValue1.Visible = false;
								dateValue2.Visible = false;
								dateImage1.Visible = false;
								dateImage2.Visible = false;
								ddlValue1.Visible = true;
								ddlValue2.Visible = false;
								Helper.LoadListControl(ddlValue1, ((Business.PickListTerm)term).PickListItems, "Value", "Value", "", true, "(Select a value)", "");
								break;
							case Kindred.Knect.ITAT.Business.TermType.Date:
								txtValue1.Visible = false;
								txtValue2.Visible = false;
								dateValue1.Visible = true;
								dateImage1.Visible = true;
								dateValue2.Visible = false;
								dateImage2.Visible = false;
								ddlValue1.Visible = false;
								ddlValue2.Visible = false;
								break;
							//TODO:  (MAYBE) support other term types		
						}
						lblOperatorMiddleText.Visible = false;
						break;

					case Business.XMLNames._TermDependencyOperator_Between:
						switch (term.TermType)
						{
							case Kindred.Knect.ITAT.Business.TermType.Text:
								txtValue1.Visible = true;
								txtValue2.Visible = true;
								dateValue1.Visible = false;
								dateValue2.Visible = false;
								dateImage1.Visible = false;
								dateImage2.Visible = false;
								ddlValue1.Visible = false;
								ddlValue2.Visible = false;
								break;
							case Kindred.Knect.ITAT.Business.TermType.Date:
								txtValue1.Visible = false;
								txtValue2.Visible = false;
								dateValue1.Visible = true;
								dateValue2.Visible = true;
								dateImage1.Visible = true;
								dateImage2.Visible = true;
								ddlValue1.Visible = false;
								ddlValue2.Visible = false;
								break;
							//TODO:  (MAYBE) support other term types		
						}
						lblOperatorMiddleText.Visible = (txtValue2.Visible || ddlValue2.Visible || dateValue2.Visible);
						lblOperatorMiddleText.Text = " and ";
						break;

					case Business.XMLNames._TermDependencyOperator_InRole:
					case Business.XMLNames._TermDependencyOperator_NotInRole:
						//TODO:  ???
						break;

					default:
						txtValue1.Visible = false;
						txtValue2.Visible = false;
						dateValue1.Visible = false;
						dateValue2.Visible = false;
						dateImage1.Visible = false;
						dateImage2.Visible = false;
						ddlValue1.Visible = false;
						ddlValue2.Visible = false;
						lblOperatorMiddleText.Visible = false;
						break;
				}
			}
			else
			{
				txtValue1.Visible = false;
				txtValue2.Visible = false;
				dateValue1.Visible = false;
				dateValue2.Visible = false;
				dateImage1.Visible = false;
				dateImage2.Visible = false;
				ddlValue1.Visible = false;
				ddlValue2.Visible = false;
				lblOperatorMiddleText.Visible = false;
			}
		}

		//ddlSourceTermOnSelectedIndexChanged
		private void LoadOperatorDropDown(Kindred.Knect.ITAT.Business.Term term)
		{
			ddlOperator.Items.Clear();
			ddlOperator.Items.Add(new ListItem("(Select One)", ""));
			switch (term.TermType)
			{
				case Kindred.Knect.ITAT.Business.TermType.Text:
					switch (((Business.TextTerm)term).Format)
					{
						case Business.TextTermFormat.Currency:
						case Business.TextTermFormat.Number:
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Equals, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NotEqual, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_GreaterThan, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_LessThan, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NoMoreThan, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NoLessThan, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Between, "");
							break;
						case Business.TextTermFormat.Plain:
						case Business.TextTermFormat.Phone:
						case Business.TextTermFormat.PhonePlusExtension:
						case Business.TextTermFormat.SSN:
						default:
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Equals, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NotEqual, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Contains, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_StartsWith, "");
							AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_EndsWith, "");
							break;
					}
					break;
				case Kindred.Knect.ITAT.Business.TermType.Date:
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Equals, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NotEqual, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_GreaterThan, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_LessThan, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NoMoreThan, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NoLessThan, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Between, "");
					break;
				case Kindred.Knect.ITAT.Business.TermType.PickList:
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Equals, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_NotEqual, "");
					AddListItem(ddlOperator, Business.XMLNames._TermDependencyOperator_Contains, "");
					break;
			}
		}

		#endregion

		#region misc
		//SetValues
		//btnAddConditionOKOnClick
		private void AddConditionListItem(string sourceTerm, string operatorValue, string value1, string value2)
		{
			string newValue = string.Format("{1}{0}{2}{0}{3}{0}{4}", CONDITION_VALUE_SEPARATOR, sourceTerm, operatorValue, value1, value2);
			string middleText = string.Empty;
			if (!string.IsNullOrEmpty(value1))
				value1 = string.Concat("\"", value1, "\"");
			if (!string.IsNullOrEmpty(value2))
			{
				value2 = string.Concat("\"", value2, "\"");
				middleText = "and ";
			}
			string newText = string.Format("\"{0}\" {1} {2} {3}{4}", sourceTerm, operatorValue, value1, middleText, value2);
			lstConditions.Items.Add(new ListItem(newText, newValue));
		}
		#endregion

		#region private methods

		//LoadOperatorDropDown (ddlSourceTermOnSelectedIndexChanged)
		//LoadStaticDropDowns (OnLoad, NotPostBack)
		private void AddListItem(ListControl ctl, string text, string id)
		{
			if (string.IsNullOrEmpty(id))
				ctl.Items.Add(new ListItem(text, text));
			else
				ctl.Items.Add(new ListItem(text, id));
		}

		private void AddListItem(ListControl ctl, string text, string id, bool isSelected)
		{
			ListItem listItem = null;
			if (string.IsNullOrEmpty(id))
				listItem = new ListItem(text, text);
			else
				listItem = new ListItem(text, id);
			listItem.Selected = isSelected;
			ctl.Items.Add(listItem);
		}

		private List<string> ValidateForm()
		{
			List<string> rtn = new List<string>();
			////TODO: put validation here
			// TODO:   do we need to sort _value1 and _value2, or validate that _value1 < _value2 ???

			if (ddlDependentTerm.Visible)
			{
				if (ddlDependentTerm.SelectedIndex == 0)
					rtn.Add("A Dependent Term must be selected.");
			}
			else if (lstbxDependentTerms.Visible)
			{
				int selectedCount = 0;
				foreach (ListItem item in lstbxDependentTerms.Items)
				{
					if (item.Selected)
					{
						selectedCount++;
						break;
					}
				}
				if (selectedCount == 0)
					rtn.Add("A Dependent Term must be selected.");
			}

			if (lstConditions.Items.Count == 0)
				rtn.Add("At least one condition must be created.");

			return rtn;
		}

		#endregion

		#region ViewState events

		protected override object SaveViewState()
		{
			ViewState[_KH_VS_TERMDEPENDENCY] = _termDependency;
			//ViewState[_KH_VS_TERMDEPENDENCYINDEX] = _termDependencyIndex;
			ViewState[_KH_VS_EDITMODE] = _editMode;
			return base.SaveViewState();
		}

		protected override void LoadViewState(object savedState)
		{
			base.LoadViewState(savedState);
			_termDependency = (Business.TermDependency)ViewState[_KH_VS_TERMDEPENDENCY];
			//_termDependencyIndex = (int)ViewState[_KH_VS_TERMDEPENDENCYINDEX];
			_editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
		}

		#endregion

	}
}

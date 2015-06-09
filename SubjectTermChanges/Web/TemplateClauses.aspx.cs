using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Collections.Generic;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using Kindred.Knect.ITAT.Business;
using System.Xml;

namespace Kindred.Knect.ITAT.Web
{
    public partial class TemplateClauses : BaseTemplatePage //, ICallbackEventHandler
    {

        #region private members

        private const string _KH_VSKEY_CLAUSEPATH = "_kh_ClausePath";
        private const string _KH_VS_EDITMODE = "_kh_vs_EditMode";
        private const string _KH_VS_ITATDOCUMENTID = "_kh_vs_ITATDocumentID";
        private string _clausePath;
        protected EditMode _editMode;
        private Guid _ITATDocumentID;
        private ITATDocument _document;


        #endregion


        #region base class overrides

        internal override Control ResizablePanel()
        {
            return divEditor;
        }

        internal override HtmlGenericControl HTMLBody()
        {
            return this.htmlBody;
        }

        protected override TemplateHeader HeaderControl()
        {
            if (_template.CanGenerateUserDocuments)
                return null;
            else
                return (TemplateHeader)this.templateheader;
        }

        protected override object SaveViewState()
        {
            ViewState[_KH_VSKEY_CLAUSEPATH] = _clausePath;
            ViewState[_KH_VS_EDITMODE] = _editMode;
            ViewState[_KH_VS_ITATDOCUMENTID] = _ITATDocumentID;
            return base.SaveViewState();
        }

        protected override void LoadViewState(object savedState)
        {
            base.LoadViewState(savedState);
            _clausePath = (string)ViewState[_KH_VSKEY_CLAUSEPATH];
            _editMode = (EditMode)ViewState[_KH_VS_EDITMODE];
            _ITATDocumentID = (Guid)ViewState[_KH_VS_ITATDOCUMENTID];
        }



        protected override void OnLoad(EventArgs e)
        {

            base.OnLoad(e);

            if (IsPostBack)
            {
                UpdateClause();
            }
            else
            {
                InitializeMain();
            }

        }

        private void InitializeMain()
        {
            //Changed for Multiple Documents

            if (_template.CanGenerateUserDocuments)
            {

                GetContextData();

            }
            else
            {
                //Changed for Multiple Documents
                _ITATDocumentID = _template.GetDefaultITATDocument().ITATDocumentID;
                _editMode = EditMode.Edit;
            }

            InitializeForm();
            treClauses.ExpandDepth = 1;
            _clausePath = _template.GetITATDocument(_ITATDocumentID).Clause.Name;
            SelectTreeNode(_clausePath);
            IsChanged = false;
        }

        private void GetContextData()
        {
            if (Context.Items[Common.Names._CNTXT_EditMode] != null)
                _editMode = (EditMode)Context.Items[Common.Names._CNTXT_EditMode];
            else
                _editMode = EditMode.Add;

            _template = (Business.Template)Context.Items[Common.Names._CNTXT_Template];

            if (_editMode == EditMode.Edit)
            {
                _ITATDocumentID = new Guid(Context.Items[Common.Names._CNTXT_ITATDocumentID].ToString());
            }
            else
            {
                _document = new ITATDocument();
                _template.Documents.Add(_document);
                _ITATDocumentID = _document.ITATDocumentID;
            }


        }


        protected override void OnPreRender(EventArgs e)
        {
            //Changed for Multiple Documents
            ShowHideMenuAndButtons();
            EnableUpDownButtons();
            SetDefaultDocumentVisibility();
            SuppressChangeNotification(this.treClauses);
            RegisterTreeNodeNotificationSuppression();
            RegisterRetainScrollPosition(divTreeView);
            RegisterResizeTreeDiv();
            RegisterDeleteConfirmation();
            RegisterFormValidation();
            if (edt.Visible)
            {
                Helper.RegisterParagraphWrapperScript(this);
                Telerik.WebControls.RadEditorUtils.ToolbarDropDown tdd = (Telerik.WebControls.RadEditorUtils.ToolbarDropDown)edt.Toolbars["DynamicToolbar"].Tools["Insert Term"];
                Helper.InitializeToolBarItems(this, tdd);
                Helper.AddSpecialToolBarItems(this, tdd);
                Helper.AddTermsToolBarItems(this, tdd, _template.BasicTerms);
                Helper.AddTermsToolBarItems(this, tdd, _template.ComplexLists);

            }

            if (txtName.Enabled)
                txtName.Focus();
            else
                treClauses.Focus();
            base.OnPreRender(e);

        }

        private void SetDefaultDocumentVisibility()
        {
            chkDefaultDocument.Visible = _template.CanGenerateDocument;

        }



        private void ShowHideMenuAndButtons()
        {
            if (_template.CanGenerateUserDocuments)
            {
                header.Visible = true;
                brHeader.Visible = true;
                divButtons.Visible = true;
                templateheader.Visible = false;
                divUserGeneratedDocument.Visible = true;
            }
            else
            {
                header.Visible = false;
                brHeader.Visible = false;
                divButtons.Visible = false;
                templateheader.Visible = true;
                divUserGeneratedDocument.Visible = false;
            }
        }


        protected void OnHeaderEvent(object sender, HeaderEventArgs e)
        {
            switch (e.CommandName)
            {
                //Changed for Multiple Documents 
                case Common.Names._HEADER_EVENT_Save:
                    if (!ValidateAndUpdateTemplate(true, false, true, true))
                        return;
                    IsChanged = false;
                    if (_template.IsManagedItem)
                        RegisterAlert(string.Format("{0} Documents have been saved.", this._itatSystem.ManagedItemName));
                    else
                        RegisterAlert("Template Documents have been saved.");
                    break;
                //Changed for Multiple Documents
                case Common.Names._HEADER_EVENT_Reset:
                    GetTemplate(true);
                    InitializeMain();
                    break;
                default:
                    base.HandleHeaderEvent(sender, e);
                    break;
            }
        }

        #endregion


        #region methods to load form

        private void InitializeForm()
        {
            LoadDocumentProperties();
            LoadChildNumeringSchemes();
            LoadDependentTermList();
            LoadClauseTree();
            IsChanged = false;
        }

        private void LoadDocumentProperties()
        {
            txtDocumentName.Text = _template.GetITATDocument(_ITATDocumentID).DocumentName;
            chkWorkflowEnabled.Checked = (bool)_template.GetITATDocument(_ITATDocumentID).WorkflowEnabled;
            chkDefaultDocument.Checked = (bool)_template.GetITATDocument(_ITATDocumentID).DefaultDocument;
        }


        private void EnableUpDownButtons()
        {
            TreeNode selectedNode = treClauses.SelectedNode;
            TreeNodeCollection siblings = Helper.Siblings(selectedNode);
            if (selectedNode == null || siblings == null)
            {
                SetUpDownButtonStatus(imgMoveUp, false);
                SetUpDownButtonStatus(imgMoveDown, false);
            }
            else
            {
                //if the selected node is the first of the siblings, disable the up button; otherwise enable it
                //if the selected node is the last of the siblings, disable the down button; otherwise enable it
                SetUpDownButtonStatus(imgMoveUp, siblings[0].ValuePath != selectedNode.ValuePath);
                SetUpDownButtonStatus(imgMoveDown, siblings[siblings.Count - 1].ValuePath != selectedNode.ValuePath);
            }
        }


        private void SetUpDownButtonStatus(ImageButton btn, bool isEnabled)
        {
            if (isEnabled)
            {
                btn.ImageUrl = (btn == imgMoveUp ? "~/Images/MoveUp.gif" : "~/Images/MoveDown.gif");
                btn.Style["cursor"] = "pointer";
                btn.Enabled = true;
            }
            else
            {
                btn.ImageUrl = (btn == imgMoveUp ? "~/Images/MoveUpDisabled.gif" : "~/Images/MoveDownDisabled.gif");
                btn.Style["cursor"] = "default";
                btn.Enabled = false;
            }

        }


        private void LoadClauseTree()
        {
            treClauses.Nodes.Clear();
            AddTreeNode(null, _template.GetITATDocument(_ITATDocumentID).Clause);
            treClauses.Nodes[0].Expand();
        }


        private void AddTreeNode(TreeNode parent, ITATClause clause)
        {
            TreeNode newNode;
            if (parent == null)
            {
                newNode = new TreeNode(clause.Name, clause.Name);
                treClauses.Nodes.Add(newNode);
            }
            else
            {
                newNode = new TreeNode(clause.Name, clause.Name);
                parent.ChildNodes.Add(newNode);
            }
            //newNode.NavigateUrl = string.Format("javascript:OnTreeNodeClick('{0}')", clause.Name);

            foreach (ITATClause childClause in clause.Children)
                AddTreeNode(newNode, childClause);
        }


        private void LoadChildNumeringSchemes()
        {
            ddlChildNumberingScheme.Items.Clear();
            foreach (ChildNumberingSchemeType numberingScheme in (ChildNumberingSchemeType[])Enum.GetValues(typeof(ChildNumberingSchemeType)))
                ddlChildNumberingScheme.Items.Add(new ListItem(ChildNumberingSchemeHelper.GetSchemeDisplayText(numberingScheme), numberingScheme.ToString("D")));
        }


        private void LoadDependentTermList()
        {
            ddlConditionalTerm.Items.Clear();
            ddlConditionalTerm.Items.Add(new ListItem(string.Empty, string.Empty));
            foreach (Term term in BasicTerms.FindTermsOfTypeExcluding(_template.BasicTerms, TermType.PlaceHolderAttachments | TermType.PlaceHolderComments))
                if (term.TermType == TermType.PickList)
                    ddlConditionalTerm.Items.Add(new ListItem(term.Name, term.Name));
        }


        private void DisableClauseText()
        {
            edt.Visible = false;
            edt.Html = "";
        }


        private void LoadClauseText(ITATClause clause)
        {
            edt.Visible = true;
            edt.Html = Business.Term.SubstituteTermIDs(_template, clause.Text);
        }



        private void LoadClauseProperties(ITATClause clause, bool rootNode)
        {
            txtName.Text = clause.Name;
            ddlChildNumberingScheme.SelectedValue = clause.ChildNumberingScheme.ToString("D");

            if (rootNode)
            {
                lblName.Visible = false;
                txtName.Visible = false;
                chkIndentFirstParagraph.Visible = false;
                chkIndentSubsequentParagraphs.Visible = false;
                chkHangingIndent.Visible = false;
                chkBreakParagraphs.Visible = false;
                chkPageBreakBefore.Visible = false;
                ddlConditionalTerm.Visible = false;
                spnDependsOn.Visible = false;
                fldDependsOn.Visible = false;


            }
            else
            {
                chkIndentFirstParagraph.Checked = (clause.IndentFirstParagraph ?? false);
                chkIndentSubsequentParagraphs.Checked = (clause.IndentSubsequentParagraphs ?? false);
                chkHangingIndent.Checked = (clause.HangingIndent ?? false);
                chkBreakParagraphs.Checked = (clause.BreakParagraphs ?? false);
                chkPageBreakBefore.Checked = (clause.PageBreakBefore ?? false);
                string dependsOnTermName = _template.FindTermName(clause.DependsOnTermID, clause.DependsOnTermName);
                FormatDependentClauseSection(dependsOnTermName, clause.DependsOnOperator, clause.DependsOnValue);

                txtName.Visible = true;
                lblName.Visible = true;
                chkIndentFirstParagraph.Visible = true;
                chkIndentSubsequentParagraphs.Visible = true;
                chkHangingIndent.Visible = true;
                chkBreakParagraphs.Visible = true;
                chkPageBreakBefore.Visible = true;
                ddlConditionalTerm.Visible = true;
                spnDependsOn.Visible = true;
                fldDependsOn.Visible = true;
            }

        }

        private void FormatDependentClauseSection(string dependsOnTermName, string dependsOnOperator, string dependsOnValue)
        {
            LoadDependentTermList();
            if (string.IsNullOrEmpty(dependsOnTermName))
            {
                spnDependsOn.Style["visibility"] = "hidden";
                ddlConditionalTerm.SelectedIndex = -1;
                ddlConditionalOperator.SelectedIndex = -1;
                ddlConditionalValue.SelectedIndex = -1;
            }
            else
            {
                try
                {
                    ddlConditionalTerm.SelectedValue = dependsOnTermName;
                    spnDependsOn.Style["visibility"] = "visible";
                    LoadConditionalOperatorsList();
                    LoadConditionalValuesDropdown(dependsOnTermName);
                    if (!string.IsNullOrEmpty(dependsOnOperator))
                        ddlConditionalOperator.SelectedValue = dependsOnOperator;
                    if (!string.IsNullOrEmpty(dependsOnValue))
                        ddlConditionalValue.SelectedValue = dependsOnValue;
                }
                catch (ArgumentOutOfRangeException)
                {
                    ddlConditionalTerm.SelectedIndex = -1;
                    spnDependsOn.Style["visibility"] = "hidden";
                }
            }
        }


        private void LoadConditionalOperatorsList()
        {
            ddlConditionalOperator.Items.Clear();
            ddlConditionalOperator.Items.Add(new ListItem("=", "="));
            ddlConditionalOperator.Items.Add(new ListItem("<>", "<>"));
        }


        private void LoadConditionalValuesDropdown(string conditionalTermName)
        {
            ddlConditionalValue.Items.Clear();
            if (string.IsNullOrEmpty(conditionalTermName))
                return;
            PickListTerm term = (PickListTerm)_template.FindBasicTerm(conditionalTermName, TermType.PickList);
            if (term == null)
                throw new Exception(ddlConditionalTerm + " is not a valid PickList term.");
            foreach (PickListItem pickListItem in term.PickListItems)
            {
                ddlConditionalValue.Items.Add(new ListItem(pickListItem.Value, pickListItem.Value));
            }
        }




        #endregion


        #region  event handlers

        protected void treClauses_SelectedNodeChanged(object sender, EventArgs e)
        {
            SelectTreeNode(treClauses.SelectedNode.ValuePath);
        }


        protected void ddlConditionalTerm_SelectedIndexChanged(object sender, EventArgs e)
        {
            FormatDependentClauseSection(ddlConditionalTerm.SelectedValue, string.Empty, string.Empty);
        }




        protected void imgMoveUp_OnClick(object sender, ImageClickEventArgs e)
        {
            ITATClause parentClause = ParentClause(_clausePath);
            ITATClause selectedClause = _template.GetITATDocument(_ITATDocumentID).FindClause(_clausePath, treClauses.PathSeparator);
            int selectedClauseIndex = parentClause.Children.IndexOf(selectedClause);
            parentClause.Children.Reverse(selectedClauseIndex - 1, 2);
            LoadClauseTree();
            SelectTreeNode(_clausePath);
            IsChanged = true;
        }

        protected void imgMoveDown_OnClick(object sender, ImageClickEventArgs e)
        {
            ITATClause parentClause = ParentClause(_clausePath);
            ITATClause selectedClause = _template.GetITATDocument(_ITATDocumentID).FindClause(_clausePath, treClauses.PathSeparator);
            int selectedClauseIndex = parentClause.Children.IndexOf(selectedClause);
            parentClause.Children.Reverse(selectedClauseIndex, 2);
            LoadClauseTree();
            SelectTreeNode(_clausePath);
            IsChanged = true;
        }


        protected void btnAddClause_OnCommand(object sender, CommandEventArgs e)
        {
            ITATClause parent = _template.GetITATDocument(_ITATDocumentID).FindClause(_clausePath, treClauses.PathSeparator);
            ITATClause newClause = new ITATClause();
            newClause.Name = "New Clause";
            newClause.ChildNumberingScheme = ChildNumberingSchemeType.None;
            newClause.Text = Helper.DefaultEditorHtml(edt);
            parent.Children.Add(newClause);
            LoadClauseTree();   //refresh the clause tree to pick up the new clause
            SelectTreeNode(string.Format("{0}{2}{1}", _clausePath, newClause.Name, treClauses.PathSeparator));
            IsChanged = true;
        }


        protected void btnDeleteClause_OnCommand(object sender, CommandEventArgs e)
        {
            string parentPath = ParentPath(_clausePath);
            ITATClause selectedClause = _template.GetITATDocument(_ITATDocumentID).FindClause(_clausePath, treClauses.PathSeparator);
            _template.GetITATDocument(_ITATDocumentID).FindClause(parentPath, treClauses.PathSeparator).Children.Remove(selectedClause);
            LoadClauseTree();   //refresh the clause tree to pick up the new clause
            SelectTreeNode(parentPath);
            IsChanged = true;
        }


        protected void btnOK_Command(object sender, CommandEventArgs e)
        {
            ValidateAndUpdateTemplate(true, true, true, false);
        }

        //Changed for Multiple Documents
        protected void btnCancel_Command(object sender, CommandEventArgs e)
        {
            ValidateAndUpdateTemplate(true, true, true, false);
        }



        #endregion


        #region other private methods

        private bool ValidateAndUpdateTemplate(bool updateValues, bool returnToCaller, bool validateFormRequired, bool saveToDatabase)
        {
            if (updateValues)
            {
                //Validation
                if (validateFormRequired && !ValidateForm())
                    return false;

                UpdateTemplate(saveToDatabase);
            }

            if ((_template.CanGenerateUserDocuments) && returnToCaller)
            {
                Context.Items[Common.Names._CNTXT_IsChanged] = HasDataChanged();
                Server.Transfer("TemplateDocuments.aspx");
            }

            return true;
        }

        private bool ValidateForm()
        {
            string documentName = txtDocumentName.Text.Trim();
            if (string.IsNullOrEmpty(documentName))
                RegisterAlert("Please enter a document name.");

            if (_template.CanGenerateDocument)
            {
                ITATDocument defaultDocument = _template.GetDefaultITATDocument();
                if ((defaultDocument == null || defaultDocument.DocumentName == _template.GetITATDocument(_ITATDocumentID).DocumentName) && !chkDefaultDocument.Checked)
                    RegisterAlert("You must have at least one default document.  Please check the default document check box.");
            }

            if (documentName == ITATSystem.ManagedItemName)
                RegisterAlert(string.Format("The document name cannot be the same as the system defined document name.  You must select a different document name.", documentName));


            if (_editMode == EditMode.Add)
                //Changed for Multiple Documents
                if (_template.DocumentNameExists(documentName))
                    RegisterAlert(string.Format("The document name '{0}' is already being used.   You must select a different document name.", documentName));

            if (_editMode == EditMode.Edit)
                if (txtDocumentName.Text != _template.GetITATDocument(_ITATDocumentID).DocumentName)
                    if (_template.DocumentNameExists(documentName))
                        RegisterAlert(string.Format("The document name '{0}' is already being used.   You must select a different document name.", documentName));

            if (this.AlertCount == 0)
                return true;

            return false;
        }

        private bool HasDataChanged()
        {
            if (IsChanged)
            {
                return true;
            }
            else
            {
                if (_editMode == EditMode.Edit)
                {
                    if (txtDocumentName.Text != _template.GetITATDocument(_ITATDocumentID).DocumentName)
                        return true;
                    if (chkWorkflowEnabled.Checked != _template.GetITATDocument(_ITATDocumentID).WorkflowEnabled)
                        return true;
                    if (chkDefaultDocument.Checked != _template.GetITATDocument(_ITATDocumentID).DefaultDocument)
                        return true;
                }
                else if (_editMode == EditMode.Add)
                {
                    return true;
                }
            }
            return false;
        }

        private void UpdateTemplate(bool saveToDatabase)
        {
            _template.GetITATDocument(_ITATDocumentID).WorkflowEnabled = chkWorkflowEnabled.Checked;

            //Make sure that the current default document is unchecked before choosing another

            ITATDocument doc = _template.GetDefaultITATDocument();

            if (doc != null && doc.DocumentName != txtDocumentName.Text && chkDefaultDocument.Checked)
                doc.DefaultDocument = false;


            _template.GetITATDocument(_ITATDocumentID).DefaultDocument = chkDefaultDocument.Checked;
            _template.GetITATDocument(_ITATDocumentID).DocumentName = txtDocumentName.Text;

            if (saveToDatabase)
                _template.SaveDocuments(null, false); 

            Context.Items[Common.Names._CNTXT_Template] = _template;
        }

        private string ParentPath(string valuePath)
        {
            return valuePath.Substring(0, valuePath.LastIndexOf(treClauses.PathSeparator));
        }


        private ITATClause ParentClause(string valuePath)
        {
            return _template.GetITATDocument(_ITATDocumentID).FindClause(ParentPath(valuePath), treClauses.PathSeparator);
        }

        private void UpdateClause()
        {
            ITATClause clause = _template.GetITATDocument(_ITATDocumentID).FindClause(_clausePath, treClauses.PathSeparator);
            if (clause != null)
            {
                IsChanged = IsChanged || FormChanged(clause);
                if (clause.Name != txtName.Text)
                    UpdateTreeNodeText(ref _clausePath, txtName.Text);
                clause.Name = txtName.Text;
                clause.ChildNumberingScheme = (Business.ChildNumberingSchemeType)Enum.Parse(typeof(Business.ChildNumberingSchemeType), ddlChildNumberingScheme.SelectedValue);
                clause.IndentFirstParagraph = chkIndentFirstParagraph.Checked;
                clause.IndentSubsequentParagraphs = chkIndentSubsequentParagraphs.Checked;
                clause.HangingIndent = chkHangingIndent.Checked;
                clause.BreakParagraphs = chkBreakParagraphs.Checked;
                clause.PageBreakBefore = chkPageBreakBefore.Checked;
                clause.Text = ValidatedXHTML(Business.Term.SubstituteTermNames(_template, edt.Html));
                if (!string.IsNullOrEmpty(ddlConditionalTerm.SelectedValue))
                {
                    clause.DependsOnTermID = _template.FindTerm(ddlConditionalTerm.SelectedValue).ID;
                    clause.DependsOnOperator = ddlConditionalOperator.SelectedValue;
                    clause.DependsOnValue = ddlConditionalValue.SelectedValue;
                }
                else
                {
                    clause.DependsOnTermID = Guid.Empty;
                    clause.DependsOnOperator = string.Empty;
                    clause.DependsOnValue = string.Empty;
                }
            }
        }

        private void UpdateTreeNodeText(ref string clausePath, string newName)
        {
            TreeNode node = treClauses.FindNode(clausePath);
            if (node == null)
                throw new Exception(string.Format("Unable to find the node to change the clause name.   Clause Path = \"{0}\".   New Name = \"{1}\".", clausePath, txtName.Text));
            node.Text = newName;
            node.Value = newName;
            clausePath = node.ValuePath;
        }


        private bool FormChanged(ITATClause clause)
        {
            if (clause.Name != txtName.Text)
            {
                //TODO: rename the appropriate tree node that was changed
                return true;
            }
            if (clause.ChildNumberingScheme != (Business.ChildNumberingSchemeType)Enum.Parse(typeof(Business.ChildNumberingSchemeType), ddlChildNumberingScheme.SelectedValue))
                return true;
            if ((clause.IndentFirstParagraph ?? false) != chkIndentFirstParagraph.Checked)
                return true;
            if ((clause.IndentSubsequentParagraphs ?? false) != chkIndentSubsequentParagraphs.Checked)
                return true;
            if ((clause.HangingIndent ?? false) != chkHangingIndent.Checked)
                return true;
            if ((clause.BreakParagraphs ?? false) != chkBreakParagraphs.Checked)
                return true;
            if ((clause.PageBreakBefore ?? false) != chkPageBreakBefore.Checked)
                return true;

            //in the next 3 "if" blocks, ignore the case where one string is null and the other is empty			
            if (_template.MatchTerm(clause.DependsOnTermID, clause.DependsOnTermName, ddlConditionalTerm.SelectedValue))
                return true;
            if (clause.DependsOnOperator != ddlConditionalOperator.SelectedValue)
                if (!(string.IsNullOrEmpty(clause.DependsOnOperator) && string.IsNullOrEmpty(ddlConditionalOperator.SelectedValue)))   //ignore the case where one string is null and the other is empty
                    return true;
            if (clause.DependsOnValue != ddlConditionalValue.SelectedValue)
                if (!(string.IsNullOrEmpty(clause.DependsOnValue) && string.IsNullOrEmpty(ddlConditionalValue.SelectedValue)))   //ignore the case where one string is null and the other is empty
                    return true;

            return false;
        }



        private void ExpandAncestorNodes(TreeNode node)
        {
            TreeNode parentNode = node.Parent;
            if (parentNode != null)
            {
                parentNode.Expand();
                ExpandAncestorNodes(parentNode);
            }
        }

        private void SelectTreeNode(string path)
        {
            TreeNode node = treClauses.FindNode(path);
            if (node == null)
                node = treClauses.Nodes[0];
            ExpandAncestorNodes(node);
            node.Select();

            ITATClause clause;
            if (node == treClauses.Nodes[0])
            {

                clause = _template.GetITATDocument(_ITATDocumentID).Clause;
                LoadClauseProperties(clause, true);
                DisableClauseText();
                btnDeleteClause.Enabled = false;

            }
            else
            {
                clause = _template.GetITATDocument(_ITATDocumentID).FindClause(path, treClauses.PathSeparator);
                LoadClauseProperties(clause, false);
                LoadClauseText(clause);
                btnDeleteClause.Enabled = true;
            }
            _clausePath = path;
        }



        private string ValidatedXHTML(string html)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            System.Xml.XmlDocument doc = new System.Xml.XmlDocument();
            try
            {
                doc.LoadXml(string.Concat("<root>", html, "</root>"));
                foreach (XmlNode node in doc.DocumentElement.ChildNodes)
                {
                    if (node.NodeType == XmlNodeType.Element)
                    {
                        if (node.Name == "p")
                            sb.Append(node.OuterXml);
                        else
                        {
                            sb.Append(@"<p align=""left"">");
                            sb.Append(node.OuterXml);
                            sb.Append("</p>");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating XML Document from clause text.  Error=" + ex.Message);
            }
            return sb.ToString();
        }


        #endregion


        #region client-side code


        private void RegisterDeleteConfirmation()
        {
            //Set up client-side script to prompt the user if they click the Delete button
            btnDeleteClause.OnClientClick = "javascript:return confirm('Are you sure you want to delete this clause?');";
        }


        private void RegisterFormValidation()
        {
            Type t = this.GetType();
            string scriptName = "_kh_FormValidation";

            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("var txt = document.getElementById('{0}');", txtName.ClientID);
            sw.WriteLine("if (txt) ");
            sw.WriteLine("	if (txt.value.length == 0) ");
            sw.WriteLine("	{ ");
            sw.WriteLine("		alert('The Clause Name cannot be blank.'); ");
            sw.WriteLine("		return false; ");
            sw.WriteLine("	} ");
            if (!ClientScript.IsOnSubmitStatementRegistered(scriptName))
                ClientScript.RegisterOnSubmitStatement(t, scriptName, sw.ToString());
        }



        private void RegisterResizeTreeDiv()
        {
            Type t = this.GetType();
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("	function ResizeTreeDiv(btn)");
            sw.WriteLine("	{");
            sw.WriteLine("		var makeBig = (btn.innerText == 'Enlarge');");
            sw.WriteLine("		var div = document.getElementById('{0}');", divTreeView.ClientID);
            sw.WriteLine("		if (makeBig) ");
            sw.WriteLine("		{");
            sw.WriteLine("			var h = 0;");
            sw.WriteLine("			o = document.getElementById('{0}');", tdProperties.ClientID);
            sw.WriteLine("			if (o)   h += o.offsetHeight;");
            sw.WriteLine("			o = document.getElementById('{0}');", tdTreeViewHeader.ClientID);
            sw.WriteLine("			if (o)   h -= o.offsetHeight;");
            sw.WriteLine("			var o = document.getElementById('{0}_wrapper');", edt.ClientID);
            sw.WriteLine("			if (o)   h += o.offsetHeight;");
            sw.WriteLine("			div.style.height = h.toString() + 'px';", divTreeView.ClientID);
            sw.WriteLine("			btn.innerText = 'Shrink';");
            sw.WriteLine("			ScrollSelectedNodeIntoView();");
            sw.WriteLine("		}");
            sw.WriteLine("		else ");
            sw.WriteLine("		{");
            sw.WriteLine("			div.style.height = '150px';", divTreeView.ClientID);
            sw.WriteLine("			btn.innerText = 'Enlarge';");
            sw.WriteLine("		}");
            sw.WriteLine("	}");

            sw.WriteLine("	function ScrollSelectedNodeIntoView()");
            sw.WriteLine("	{");
            sw.WriteLine("		var oData = {0}_Data;", treClauses.ClientID);
            sw.WriteLine("		if (oData) ");
            sw.WriteLine("		{	");
            sw.WriteLine("			if (oData.selectedNodeID) ");
            sw.WriteLine("			{");
            sw.WriteLine("			var name = oData.selectedNodeID.value; ");
            sw.WriteLine("			var selectedNode = document.getElementById(name); ");
            sw.WriteLine("			if(selectedNode)	    ");
            sw.WriteLine("				selectedNode.scrollIntoView(false);  	");
            sw.WriteLine("			}");
            sw.WriteLine("		}");
            sw.WriteLine("	}");

            string scriptName = "_kh_ResizeTreeDiv";
            if (!ClientScript.IsClientScriptBlockRegistered(t, scriptName))
                ClientScript.RegisterClientScriptBlock(t, scriptName, sw.ToString(), true);
        }


        private void RegisterTreeNodeNotificationSuppression()
        {

            Type t = this.GetType();
            string scriptName = "_kh_SuppressTreeNodeNotification";
            System.IO.StringWriter sw = new System.IO.StringWriter();
            sw.WriteLine("	function CancelOnBeforeUnload(elem)");
            sw.WriteLine("	{");
            sw.WriteLine("		document.body.onbeforeunload = null;");
            sw.WriteLine("	}");

            sw.WriteLine("	var o = document.getElementById('{0}');", treClauses.ClientID);
            sw.WriteLine("	if (o) ");
            sw.WriteLine("	{ ");
            sw.WriteLine("		var anchors = o.getElementsByTagName(\"a\");	 ");
            sw.WriteLine("		if (anchors) ");
            sw.WriteLine("		{ ");
            sw.WriteLine("			for (i=0; i<anchors.length; i++)	 ");
            sw.WriteLine("			{ ");
            sw.WriteLine("				anchors.item(i).onclick = CancelOnBeforeUnload; ");
            sw.WriteLine("			} ");
            sw.WriteLine("		} ");
            sw.WriteLine("	} ");

            if (!ClientScript.IsStartupScriptRegistered(t, scriptName))
                ClientScript.RegisterStartupScript(t, scriptName, sw.ToString(), true);
        }



        #endregion

    }
}

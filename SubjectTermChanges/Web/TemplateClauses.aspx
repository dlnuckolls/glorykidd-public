<%@ page language="C#" autoeventwireup="true" codebehind="TemplateClauses.aspx.cs" inherits="Kindred.Knect.ITAT.Web.TemplateClauses" title="Terms" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title></title>
	<%--<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />--%>
	<link rel="stylesheet" type="text/css" href="itat.css" />

</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server" >
		<kh:standardheader id="header" runat="server"  pagetitle="Template Document Edit" visible="false"/>
        <br runat="server" id="brHeader" visible="false"/>
        <kh:templateheader id="templateheader" runat="server" onheaderevent="OnHeaderEvent" visible="false"/>
		    <div id="divUserGeneratedDocument" runat="server" class="DataEntryCaption" style="margin: 0 0 4px 0; border: 1px solid #666;">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">
            	<tr>
                    <td class="DataEntryCaption">Document Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtDocumentName" runat="server" Width="500"/></td>
				</tr>
				<tr>
                    <td class="DataEntryCaption AlignLeft">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkWorkflowEnabled" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlign" text="Workflow Enabled" /></td>
				</tr>
				<tr>
                    <td class="DataEntryCaption AlignLeft">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkDefaultDocument" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlign" text="Default Document" /></td>
				</tr>						
            </table>
            </div>
		<div id="divTopHalf" style="height: 170px; margin: 0 0 2px 0; border: 1px solid #666;">
			<table border="0" cellpadding="0" cellspacing="0" width="100%">		
				<colgroup>
					<col width="100%" />
					<col width="280px" />
				</colgroup>	
				<tr>
					<td  style="border-right: 1px solid #666;">
						<table border="0" cellpadding="2" cellspacing="0" width="100%" height="100%">
							<tr>
								<td class="1stTblColor GridTitle AlignTop">Clauses</td>
								<td runat="server" id="tdTreeViewHeader" class="1stTblColor AlignRight"  style="width:100%; padding: 0 0 0 4px;">
									<asp:button id="btnAddClause" runat="server" text="Add" cssclass="KnectButton" width="60px" oncommand="btnAddClause_OnCommand" />
									<asp:button id="btnDeleteClause" runat="server" text="Delete" cssclass="KnectButton" width="60px"  oncommand="btnDeleteClause_OnCommand"  />
								</td>
								<td class="1stTblColor" style="padding: 3px 5px  0 5px;">
									<asp:imagebutton id="imgMoveUp" runat="server" imageurl="~/Images/MoveUp.gif"  width="11px" height="11px" alternatetext="Move Clause Up" onclick="imgMoveUp_OnClick"  /><br />
									<asp:imagebutton id="imgMoveDown" runat="server" imageurl="~/Images/MoveDown.gif"  width="11px" height="11px" alternatetext="Move Clause Down" onclick="imgMoveDown_OnClick"  />
								</td>
							</tr>
							<tr>
								<td colspan="3" style="border-top: 1px solid #666;">
									<asp:panel id="divTreeView" runat="server" style="overflow-x:auto; overflow-y: scroll; height:180px; width: 100%;">
										<asp:treeview id="treClauses" runat="server" 
													showlines="true" 
													pathseparator="|"
													showexpandcollapse="true"
													onselectednodechanged="treClauses_SelectedNodeChanged" 
													nodestyle-cssclass="TreeViewNode"
													selectednodestyle-cssclass="TreeViewNodeSelected"  
													nodewrap="false"
													populatenodesfromclient="false"  
													skiplinktext=""
													>
										</asp:treeview>
									</asp:panel>
								</td>
							</tr>
						</table>
					</td>
					<td runat="server" id="tdProperties" style="vertical-align: top;" class="1stTblColor">
						<table border="0" cellpadding="1" cellspacing="0" style="table-layout: fixed;">
							<colgroup>
								<col width="100px" />
								<col width="100%" />
							</colgroup>
							<tr>
								<td class="DataEntryCaption"><asp:Label runat="server" ID="lblName" Text="Clause Name"/></td>
								<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtName" runat="server" width="100%" /></td>
							</tr>                          
							<tr>
								<td class="DataEntryCaption">Child Numbering</td>
								<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlChildNumberingScheme" runat="server" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">&nbsp;</td>
								<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkIndentFirstParagraph" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlign" text="Indent 1st Paragraph" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">&nbsp;</td>
								<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkIndentSubsequentParagraphs" runat="server" textalign="Right" cssclass="DataEntryCaption CheckBoxAlign" text="Indent Other Paragraphs" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">&nbsp;</td>
								<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkHangingIndent" runat="server" textalign="Right" cssclass="DataEntryCaption" text="Hanging Indent" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">&nbsp;</td>
								<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkBreakParagraphs" runat="server" textalign="Right" cssclass="DataEntryCaption" text="Break Paragraphs" /></td>
							</tr>
							<tr>
								<td class="DataEntryCaption">&nbsp;</td>
								<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkPageBreakBefore" runat="server" textalign="Right" cssclass="DataEntryCaption" text="Page Break Before" /></td>
							</tr>
							<tr>
								<td colspan="2">
									<fieldset id="fldDependsOn" runat="server" class="DataEntryCaption AlignLeft" style="padding: 0 4px 0 4px;">
										<legend class="DataEntryCaption">Depends On</legend>
										<asp:dropdownlist id="ddlConditionalTerm" runat="server" enableviewstate="true" autopostback="true" onselectedindexchanged="ddlConditionalTerm_SelectedIndexChanged"  width="260px" />
										<br />
										<span id="spnDependsOn" runat="server">
											&nbsp;Show if value 
											<asp:dropdownlist id="ddlConditionalOperator" enableviewstate="true" runat="server" />
											<asp:dropdownlist id="ddlConditionalValue" enableviewstate="true" runat="server" />
										</span>
									</fieldset>
								</td>
							</tr>
						</table>
					</td>
				</tr>
			</table>
		</div>
		<asp:panel id="divEditor" runat="server" style="display: block; margin: 2px 0 0 0; border: 1px solid #666;">
			<rade:radeditor id="edt" 
							runat="server" 
							toolsfile="~/Config/TemplateClauseToolsFile.xml" 
							font-names="Times New Roman, Arial, Courier New"
							onclientload="radOnClientLoad"
							enableviewstate="true"
							width="100%" 
							focusonload="false"
							height="100%" 
							enabled="true" 
							editable="true"
							visible="false"
							saveasxhtml="true" 
							saveinfile="false" 
							newlinebr="true"
							enabletab="false" 
							enabledocking="false"
							enableclientserialize="false"  
							font-size="" 
							converttagstolower="true"
							converttoxhtml="true"
							convertfonttospan="false"  	 				  
							enableserversiderendering="true" 
							stripformattingonpaste="MSWordRemoveAll"   
							toolbarmode="Default"
							showsubmitcancelbuttons="false"
							showpreviewmode="true"
							showhtmlmode="true" 
							renderastextarea="false"		   
			/>
		</asp:panel>
		<div runat="server" id="divButtons" class="DataEntryCaption AlignCenter" style="padding: 8px 0 8px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" oncommand="btnOK_Command" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" oncommand="btnCancel_Command" />
		</div>	

	</form>
</body>
</html>

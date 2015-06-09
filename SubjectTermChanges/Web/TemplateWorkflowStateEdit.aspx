<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateWorkflowStateEdit.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateWorkflowStateEdit" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>

<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Edit State" />
			
		<div class="DataEntryCaption" style="padding: 5px 0 10px 0">
			<table border="0" cellpadding="2" cellspacing="0" width="100%">
				<tr>
					<td class="DataEntryCaption">State Name</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtName" runat="server" width="300px" /></td>
				</tr>
				<tr>
					<td class="DataEntryCaption">Status</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlStatus" runat="server" width="300px" /></td>
				</tr>
				<tr id="trTermGroup" runat="server" visible="false">
					<td class="DataEntryCaption">Tab / Complex List</td>
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermGroup" runat="server" width="300px" autopostback="true" onselectedindexchanged="ddlTermGroupSelectedIndexChanged" /></td>
				</tr>
				<tr> 
					<td colspan="2">
						<table>
							<tr>
								<td>
									<kh:roleselector id="rsEditors" runat="server" widths="100,200" caption="Editors" roletype="Security" />
								</td>
								<td>
									<kh:roleselector id="rsAttachmentRemovers" runat="server" widths="100,200" caption="Attachment Removers" roletype="Security" />
								</td>
							</tr>
							<tr>
								<td>
									<kh:roleselector id="rsViewers" runat="server" widths="100,200" caption="Viewers" roletype="Security" />
								</td>
								<td>
									<kh:roleselector id="rsScannedAttachmentRemovers" runat="server" widths="100,200" caption="Scanned Attachment Removers" roletype="Security" />
								</td>
							</tr>
						</table>
					</td>
				</tr>
				<tr>
					<td class="DataEntryCaption">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkDraft" runat="server" text="Watermark" cssclass="CheckBoxAlign" /></td>
				</tr>
				<tr>
					<td class="DataEntryCaption">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkRequiresValidation" runat="server" text="Validation Required to Enter This State" cssclass="CheckBoxAlign" /></td>
				</tr>
				<tr>
					<td class="DataEntryCaption">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkExit" runat="server" text="Is Exit" cssclass="CheckBoxAlign" /></td>
				</tr>
				<tr>
					<td class="DataEntryCaption">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkBase" runat="server" text="Is Base" cssclass="CheckBoxAlign" enabled="false" /></td>
				</tr>
			</table>
		</div>

			<div class="TemplateEditBody" id="divTermContainer">
				<div class="GridTitle" style="margin-right:-2px;">
					<table cellpadding="0" cellspacing="0">
						<tr>
							<td class="1stTblColor GridTitle AlignTop">Actions</td>
							<td runat="server" id="tdTreeViewHeader" class="1stTblColor AlignRight"  style="width:100%; padding: 0 2px 1px 5px;">
								<asp:button id="btnAddAction" runat="server" cssclass="KnectButton" width="70px" text="Add" oncommand="btnAddAction_Command"   />
							</td>	
							<td class="1stTblColor">
								<asp:imagebutton id="imgMoveUp" runat="server" imageurl="~/Images/MoveUp.gif"  width="11px" height="11px" alternatetext="Move Clause Up" onclick="imgMoveUp_OnClick"  /><br />
								<asp:imagebutton id="imgMoveDown" runat="server" imageurl="~/Images/MoveDown.gif"  width="11px" height="11px" alternatetext="Move Clause Down" onclick="imgMoveDown_OnClick"  />
							</td>
						</tr>
					</table>
				</div>
				<asp:panel id="pnlGridHeader" runat="server" visible="false" cssclass="DataEntryCaption" />
				<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" >

					<kh:kindredgridview id="grdActions" runat="server" 
									cssclass="NetTable" 
									cellpadding="2" 
									cellspacing="0" 
									onrowcreated="grd_RowCreated" 
									onrowcommand="grd_RowCommand" 
									autogeneratecolumns="False" 
									datakeynames="ButtonText" 
									rowhighlighting="false" 
									confirmondelete="False" 
									enabledoubleclickevent="false" 
									enableheaderclick="false" 
									enableclickevent="true" 
									headercontainer="pnlGridHeader" 
									headerrowsize="1" 
									container="pnlGridBody" 
									sortascending="True" 
									sortcolumn="-1"
									>
						<columns>
							<asp:boundfield datafield="ButtonText" readonly="True" headertext="Button Text">
								<headerstyle width="50%"></headerstyle>
								<itemstyle horizontalalign="left"></itemstyle>
							</asp:boundfield>
							<asp:boundfield datafield="TargetState" readonly="True" headertext="Target State">
								<headerstyle width="50%"></headerstyle>
								<itemstyle horizontalalign="left"></itemstyle>
							</asp:boundfield>
							<asp:buttonfield buttontype="Link" text="Edit" headertext="" commandname="EditRow">
								<headerstyle width="70px" />
								<itemstyle horizontalalign="center"></itemstyle>
							</asp:buttonfield>
							<asp:buttonfield buttontype="Link" text="Delete" headertext="" commandname="DeleteRow">
								<headerstyle width="70px" />
								<itemstyle horizontalalign="center"></itemstyle>
							</asp:buttonfield>
						</columns>
					</kh:kindredgridview>
				</asp:panel>
			</div>
				
		<div class="DataEntryCaption AlignCenter" style="padding: 8px 0 8px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" oncommand="btnOK_Command" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" oncommand="btnCancel_Command" />
		</div>	
		
		
		
    </form>
</body>
</html>

<%--
					<td class="DataEntryCaption">
						Editors<br /><br />
						<button id="btnClearEditors" runat="server" class="KnectButton" style="width:60px;" onclick="_kh_deselectListItems(document.all.lstEditors);">Clear</button>
					</td>
					<td class="DataEntryCaption AlignLeft">
						<asp:listbox id="lstEditors" runat="server" selectionmode="Multiple" rows="6" width="300px" />	
					</td>
--%>				

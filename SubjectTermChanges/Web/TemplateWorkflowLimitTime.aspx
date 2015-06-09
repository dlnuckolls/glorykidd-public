<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TemplateWorkflowLimitTime.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateWorkflowLimitTime" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">

<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>

<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server">

	<kh:standardheader id="header" runat="server"  pagetitle="Edit Time Limit"  />
	<div class="TemplateEditBody" id="divContainer">

		<table cellpadding="2" cellspacing="0" border="0">
			<tr>
				<td class="DataEntryCaption">Limit Approval Time</td>	
				<td class="DataEntryCaption AlignLeft"><asp:checkbox id="chkLimitTime" runat="server" textalign="Right" cssclass="DataEntryCaption  CheckboxAlignLeft" autopostback="true" oncheckedchanged="chkLimitTime_CheckedChanged" /></td>
			</tr>

			<tr>
				<td class="DataEntryCaption"># of Days from Workflow Entry</td>	
				<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtDays" runat="server" width="30px" maxlength="3" textmode="SingleLine" /></td>
			</tr>
		</table>

		<fieldset id="fldNotification" runat="server" class="DataEntryCaption AlignLeft" style="padding: 0 4px 4px 4px;">
			<legend class="DataEntryCaption">Notification</legend>
			<table cellpadding="2" cellspacing="0" border="0">
				<tr>
					<td class="DataEntryCaption AlignLeft">Subject</td>	
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtSubject" runat="server" width="300px" textmode="SingleLine" /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption AlignLeft">Recipients</td>	
					<td class="DataEntryCaption AlignLeft">
						<asp:listbox id="lstRecipients" runat="server" width="300px" rows="4" selectionmode="multiple" />
						<button id="btnClearEditors" runat="server" class="KnectButton" onclick="_kh_deselectListItems(document.all.lstRecipients);">Clear</button>
					</td>
				</tr>
				<tr id="row_ddlFilterFacility" runat="server" visible="false">
					<td class="DataEntryCaption AlignLeft">Filter By Facility</td>	
					<td class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlFilterFacility" width="300px" runat="server" /></td>
				</tr>
			</table>
			
			<div class="DataEntryCaption AlignLeft">Body</div>	
			<asp:panel id="divEditor" runat="server" style="display: block; margin: 2px 0 0 0; width: 100%; border: 1px solid #666; font-family:Times New Roman; font-size:12pt">
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
				   visible="true"
					saveasxhtml="true" 
					saveinfile="false" 
					newlinebr="true"
					 enabletab="false" 
					 font-size="" 
					converttagstolower="true"
					converttoxhtml="true"
					convertfonttospan="false"  	 				  
					 enableserversiderendering="true" 
					 stripformattingonpaste="MSWordRemoveAll"   
					 toolbarmode="Default"
					 showpreviewmode="false"
					 showsubmitcancelbuttons="false"
					 showhtmlmode="false" 
					 renderastextarea="false"		   
			/>
			<%--  	(removed)	onclientload="radOnClientLoad"  --%>
			
		</asp:panel>
		
</fieldset>

		<div class="DataEntryCaption AlignCenter" style="padding: 4px 0 4px 0;">
			<asp:button id="btnOK" runat="server"  cssclass="KnectButton" text="OK" oncommand="btnOK_Command" />
			<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" oncommand="btnCancel_Command" />
		</div>	

	</div>
	</form>
</body>
</html>

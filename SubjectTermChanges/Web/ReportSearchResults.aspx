<%@ page language="C#" autoeventwireup="true" validaterequest="false" codebehind="ReportSearchResults.aspx.cs" inherits="Kindred.Knect.ITAT.Web.ReportSearchResults" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" runat="server" class="NoScroll">
	<form id="form1" runat="server">
		<kh:standardheader id="itatHeader" runat="server" />
		
	<div class="TemplateEditBody" id="divTermContainer">

		<div class="GridTitle" style="margin:0;">
			<span style="float: left; text-align: left;">Select the terms to appear in the report:</span>
			<span style="float: right; text-align:right; margin:0; padding:0 5px 0 0;">
					<asp:imagebutton id="btnTermMoveUp" runat="server" width="11px" height="11px" alternatetext="Move Row Up" oncommand="btnSwitchRows_Command" /><br />
					<asp:imagebutton id="btnTermMoveDown" runat="server" width="11px" height="11px" alternatetext="Move Row Down" oncommand="btnSwitchRows_Command" />
			</span>
		</div>
		
		<asp:panel id="pnlGridHeader" runat="server" visible="false" />
		<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="height:280px;">
			<kh:multiselectgrid id="grdTermList" runat="server" onrowcommand="grd_RowCommand" />
		</asp:panel>
	
		<div nowrap class="AlignCenter" style="margin: 4px 16px 4px 0;">
			<span class="DataEntryCaption" style="width:150px;">Primary Sort:&nbsp;</span>
			<asp:dropdownlist id="ddlSort1" cssclass="DataEntryEdit"  runat="server" autopostback="false" />
			<span class="DataEntryCaption" style="width:150px;">&nbsp;</span>
			<br />
			<span class="DataEntryCaption" style="width:150px;">Secondary Sort:&nbsp;</span>
			<asp:dropdownlist id="ddlSort2" cssclass="DataEntryEdit"  runat="server" autopostback="false" />
			<span class="DataEntryCaption" style="width:150px;">&nbsp;</span>
		</div>
	
		<div nowrap class="AlignCenter" style="margin: 4px 16px 4px 0;">
			<span class="DataEntryCaption" style="width:150px;">&nbsp;</span>
			<asp:button id="btnContinue" cssclass="KnectButton" runat="server" text="Continue" oncommand="btnContinue_Click" />&nbsp;
			<asp:button id="btnCancel" cssclass="KnectButton" runat="server" text="Cancel" oncommand="btnCancel_Click" />
			<span class="DataEntryCaption" style="width:150px;">&nbsp;</span>
		</div>

	
	</div>

	
	</form>
</body>
</html>

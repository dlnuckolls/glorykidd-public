<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemAdd.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemAdd" %>
<%@ register assembly="RadEditor.Net2" namespace="Telerik.WebControls" tagprefix="radE" %>
<%@ register assembly="RadSpell.Net2" namespace="Telerik.WebControls" tagprefix="radS" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" title="">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server" pagetitle="" />
		<div class="Emphasis" style="margin: 2px 0 2px 0;">ALL FIELDS ARE REQUIRED</div>
		<asp:panel cssclass="TemplateEditBody" id="editBody" runat="server">
			<table border="0" cellpadding="2" cellspacing="0" width="100%" style="table-layout:fixed;">
				<colgroup>
					<col width="100px" />
					<col width="100%" />
				</colgroup>	
				<tr>
					<td class="DataEntryCaption" style="width:100px">Template:</td>
					<td class="DataEntryCaption AlignLeft" style="width:100%;"><asp:dropdownlist id="ddlTemplate" runat="server" width="100%" AutoPostBack="true"  /></td>
				</tr>
				<tr id="trFacilities" runat="server">
					<td class="DataEntryCaption" style="width:100px">Owning Facility:</td>
					<td class="DataEntryCaption AlignLeft" style="width:100%;"><asp:dropdownlist id="ddlFacility" runat="server" width="100%"  /></td>
				</tr>
				<tr>
					<td colspan="2" class="DataEntryCaption" style="padding: 3px 3px 3px 3px">
						<asp:button id="btnAdd" runat="server"  cssclass="KnectButton" text="Add" oncommand="btnAdd_Command" />
					</td>
				</tr>
			</table>
		</asp:panel>
		
		<fieldset class="fieldset" id="panelDescription" runat="server">
		<table border="0" cellpadding="8" cellspacing="0" width="100%" style="table-layout:fixed;">
			<colgroup>
				<col width="200px" />
				<col width="100%" />
			</colgroup>	
		    <tr>
		        <td class="DetailedDescriptionBold" >SELECTED TEMPLATE:<br /></td>
		        <td class="DetailedDescriptionBold" ><asp:Literal ID="litTemplateName" runat="server" /><br /></td>
		    </tr>
		    <tr id="rowWhenToUse" runat="server" >
		        <td class="DetailedDescriptionBold">When to use this template?</td>
		        <td class="DetailedDescription"><asp:literal ID="litWhenToUse" runat="server" /></td>
		    </tr>
		    <tr id="rowWhenNotToUse" runat="server" >
		        <td class="DetailedDescriptionBold">When not to use this template?</td>
		        <td class="DetailedDescription"><asp:literal ID="litWhenNotToUse" runat="server" /></td>
		    </tr>
		    <tr id="rowAdditionalInfo" runat="server" >
		        <td class="DetailedDescriptionBold">Additional Template Information:</td>
		        <td class="DetailedDescription"><asp:literal ID="litAdditionalInfo" runat="server" /></td>
		    </tr>
		</table>
		</fieldset>
	</form>
</body>
</html>

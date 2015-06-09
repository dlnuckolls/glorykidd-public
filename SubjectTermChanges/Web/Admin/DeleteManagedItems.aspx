<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DeleteManagedItems.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.DeleteManagedItems" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title>View Template</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="../itat.css" />
</head>

<body id="htmlBody" runat="server">
    <form id="form1" runat="server">
			<kh:standardheader id="itatHeader" pagetitle="Delete Managed Items" runat="server"  />
			
			<div class="TemplateEditBody" id="divContainer">

				<table width="100%" style="table-layout:fixed;">

					<tr>
						<td class="DataEntryCaption" width="120px">System:</td>
						<td class="DataEntryCaption AlignLeft" width="100%"><asp:label id="lblSystem" runat="server" /></td>
					</tr>

					<tr>
						<td class="DataEntryCaption">Delete by:</td>
						<td class="DataEntryCaption"><asp:dropdownlist id="ddlDeleteBy" runat="server"  width="100%" autopostback="true" onselectedindexchanged="ddlDeleteByOnSelectedIndexChanged"  /></td>
					</tr>

					<tr id="trCriteria" runat="server">
						<td class="DataEntryCaption"><asp:label id="lblItem" runat="server" /></td>
						<td class="DataEntryCaption">
							<asp:dropdownlist id="ddlTemplate" runat="server"  width="100%" />
							<asp:textbox id="txtManagedItemNumber" runat="server" width="100%" />
						</td>
					</tr>
					
					<tr>
						<td  class="DataEntryCaption AlignCenter" colspan="2" align="center" style="padding:10px 0 10px 0;">
							<asp:button id="btnDelete" runat="server" text="Delete" cssclass="KnectButton" oncommand="btnDeleteOnCommand"  />
							<asp:button id="btnReset" runat="server" text="Reset" cssclass="KnectButton" oncommand="btnResetOnCommand" />
						</td>
					</tr>
						
				</table>
				
			</div>


    </form>
</body>
</html>

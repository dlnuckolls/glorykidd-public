<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermDelete.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermDelete" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Delete Term" />
		<asp:panel cssclass="TemplateEditBody" id="editBody" runat="server">
			<table border="0" cellpadding="2" cellspacing="0" width="100%">
				<tr>
					<td class="DataEntryCaption AlignLeft">If you delete this term, then any <asp:label id="lblItemName1" runat="server" /> created from this template will not contain this term.   However, any existing <asp:label id="lblItemName2" runat="server" /> will continue to contain this term.<br /><br />Are you sure you want to delete this term?</td>
				</tr>
				<tr>
					<td class="DataEntryCaption AlignCenter" style="padding: 15px 0 0 0">
						<asp:button id="btnYes" runat="server"  cssclass="KnectButton" text="Yes" onclick="btnYes_Click" />
						<asp:button id="btnNo" runat="server"  cssclass="KnectButton" text="No" onclick="btnNo_Click" />
					</td>
				</tr>
			</table>
		</asp:panel>
	</form>
</body>
</html>

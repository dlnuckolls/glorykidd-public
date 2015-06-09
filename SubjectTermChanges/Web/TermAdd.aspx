<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TermAdd.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.TermAdd" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll" title="Add Term">
	<form id="form1" runat="server">
		<kh:standardheader id="header" runat="server"  pagetitle="Add Term" />
		<asp:panel cssclass="TemplateEditBody" id="editBody" runat="server">
			<table border="0" cellpadding="2" cellspacing="0" width="100%" style="table-layout:fixed;">
				<colgroup>
					<col width="100px" />
					<col width="100%" />
				</colgroup>	
				<tr>
					<td class="DataEntryCaption" style="width:100px">Term Name:</td>
					<td class="DataEntryCaption AlignLeft" style="width:100%;"><asp:textbox id="txtTermName" runat="server" width="300px"  /></td>
				</tr>
				<tr>
					<td  class="DataEntryCaption">Term Type:</td>
					<td  class="DataEntryCaption AlignLeft"><asp:dropdownlist id="ddlTermType" runat="server" width="300px" /></td>
				</tr>
				<tr>
					<td>&nbsp;</td>
					<td class="DataEntryCaption AlignLeft" style="padding: 15px 0 0 0">
						<asp:button id="btnContinue" runat="server"  cssclass="KnectButton" text="Continue" onclick="btnContinue_Click" />
						<asp:button id="btnCancel" runat="server"  cssclass="KnectButton" text="Cancel" onclick="btnCancel_Click" />
					</td>
				</tr>
			</table>
		</asp:panel>
	</form>
</body>
</html>

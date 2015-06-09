<%@ page language="C#" autoeventwireup="true" validaterequest="false"  codebehind="ReportAdd.aspx.cs" inherits="Kindred.Knect.ITAT.Web.ReportAdd" title="Untitled Page" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<form id="form1" runat="server">
		<kh:standardheader id="itatHeader" runat="server"  />
		<div class="TemplateEditBody" id="divMain">
		
			<table id="tblForm" style="width: 100%;" cellspacing="0" cellpadding="2">

				<tr>
					<td class="DataEntryCaption">Name:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtName" runat="server" width="400px" /></td>
				</tr>

				<tr>
					<td class="DataEntryCaption">Description:</td>
					<td class="DataEntryCaption AlignLeft"><asp:textbox id="txtDescription" runat="server" width="400px" /></td>
				</tr>

				<tr>
					<td style="height: 34px">&nbsp;</td>
					<td class="DataEntryCaption AlignLeft" style="padding: 15px 0 0 0; height: 34px;">
						<asp:button id="btnPromote" runat="server" text="Save" cssclass="KnectButton" oncommand="btnSave_Command" />
						<asp:button id="btnDemote" runat="server" text="Cancel" cssclass="KnectButton" oncommand="btnCancel_Command" />
					</td>
				</tr>

			</table>

		</div>
		
	
	</form>
</body>
</html>

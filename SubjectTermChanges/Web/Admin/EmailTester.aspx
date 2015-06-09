<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="EmailTester.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.EmailTester" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Email Tester</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="../itat.css" />
	<script type="text/javascript">
		function Deselect(lst)
		{
			if (lst)
			{
				for (i=0; i< lst.options.length; i++)
					lst.options(i).selected = false;

			}
		}
	</script>
</head>

<body id="body" runat="server">
    <form id="form1" runat="server">
	<kh:standardheader id="itatHeader" pagetitle="E-mail Tester" runat="server"  />
    <div>
			<table>
				
				<tr>
					<td>Recipient roles:<br /><br /><button id="btnDeselectRecipients" onclick="Deselect(document.all.lstRoles);">De-Select All</button></td>
					<td><asp:listbox id="lstRoles" runat="server" width="300px" selectionmode="multiple"  rows="15" /></td>
				</tr>
				
				<tr>
					<td>Owning Facility (optional):</td>
					<td><asp:dropdownlist id="ddlFacilities" runat="server" width="300px"    /></td>
				</tr>
				
				<tr>
					<td>Subject:</td>
					<td><asp:textbox id="txtSubject" runat="server" width="300px" /></td>
				</tr>
				
				<tr>
					<td>Body:</td>
					<td><asp:textbox id="txtBody" runat="server" rows="5" textmode="MultiLine" width="300px" /></td>
				</tr>

				<tr>
					<td colspan="2" align="center">
						<asp:button text="Send Email" id="btnSubmit" runat="server" oncommand="btnSubmit_Command" commandargument="true" />
						<asp:button text="List Recipients" id="btnListRecipients" runat="server" oncommand="btnSubmit_Command" commandargument="false" />
					</td>
				</tr>				
				
			</table>
    </div>
    
    <asp:panel id="pnlResults" runat="server" style="margin:15px 5px 5px 5px;">
		<asp:literal id="litResults" runat="server" />
    </asp:panel>
    </form>
</body>
</html>

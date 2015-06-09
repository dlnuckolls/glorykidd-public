<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ExternalTermFilter.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ExternalTermFilter" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html>
<head id="Head1" runat="server">
	<title></title>
	<base target="_self" />
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="body" runat="server"    class="NoScroll">
    <form id="form1" runat="server">
    
		<asp:table id="tblFilterControls" runat="server" cellpadding="0" cellspacing="0" style="table-layout:fixed; width:100%;">
		</asp:table>

		<asp:panel id="pnlFilterButtons"  runat="server" style="text-align:center;margin:4px 0 4px 0;">
			<asp:button id="btnGetData" text="Get Data" cssclass="KnectButton" runat="server" oncommand="btnGetDataOnCommand"  />
		</asp:panel>
	
		<asp:checkbox id="chkSelectAll" runat="server" visible="false" style="position:absolute; top: 4px; left:6px;"  />
		<asp:panel id="pnlGridHeader" runat="server"	/>
		<asp:panel id="pnlGrid" runat="server" style="border:1px solid #FFF;">
			<kh:multiselectgrid id="grd" runat="server" />
		</asp:panel>
    
		<asp:panel id="pnlButtons" runat="server" style="text-align:center; white-space:nowrap; margin: 5px 0 0 0;">
			<asp:button id="btnOK" text="OK" cssclass="KnectButton" runat="server" enabled="false" oncommand="btnOKOnCommand" />&nbsp;&nbsp;
			<asp:button id="btnCancel" text="Cancel" cssclass="KnectButton" runat="server" oncommand="btnCancelOnCommand"  />&nbsp;&nbsp;
			<asp:button id="btnSearchAgain" text="Search Again" cssclass="KnectButton" runat="server" visible="false" oncommand="btnSearchAgainOnCommand" />
		</asp:panel>
       
    </form>
</body>
</html>

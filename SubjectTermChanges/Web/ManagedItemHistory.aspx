<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemHistory.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemHistory" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>

<body id="body" runat="server" class="NoScroll" title="">
	<form id="form1" runat="server">

		<kh:manageditemheader id="header" runat="server" />

			<asp:panel id="pnlError" runat="server" visible="false" cssclass="Error">
				<asp:literal id="litError" runat="server" />
			</asp:panel>
			
			<asp:panel id="pnlGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="border:solid #333; border-width:1px 1px 1px 1px;">
				<kh:kindredgridview id="grdResults" 
							runat="server" 
							cellpadding="2"
							cellspacing="0"	
							cssclass="NetTable"
							autogeneratecolumns="False" 
							datakeynames ="MIID" 
							rowhighlighting="false" 
							confirmondelete="false" 
							enabledoubleclickevent="false" 
							enableheaderclick="false" 
							enableclickevent="false" 
							headercontainer="pnlGridHeader" 
							headerrowsize="1" 
							container="pnlGridBody" 
							sortascending="True" 
							sortcolumn="-1"
				>
					<columns>
						<asp:boundfield  datafield="LogOnID" readonly="True" headertext="UserID">
							<headerstyle width="70px"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="PersonName" readonly="True" headertext="Person">
							<headerstyle width="40%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="Status" readonly="True" headertext="Status">
							<headerstyle width="30%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="State" readonly="True" headertext="State">
							<headerstyle width="30%"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
						<asp:boundfield  datafield="DateOfChange" readonly="True" headertext="Date" dataformatstring="{0:MM/dd/yyyy hh:mm tt}" htmlencode="false">
							<headerstyle width="135px"></headerstyle>
							<itemstyle horizontalalign="left"></itemstyle>
						</asp:boundfield>
					</columns>
				</kh:kindredgridview>
			</asp:panel>

    </form>
</body>
</html>

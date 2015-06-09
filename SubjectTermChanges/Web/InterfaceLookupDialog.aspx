<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="InterfaceLookupDialog.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.InterfaceLookupDialog" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html>
<head runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>
<body id="body" runat="server" class="NoScroll">
    <form id="form1" runat="server">
				<kh:standardheader id="itatHeader" pagetitle="" runat="server" />
				<asp:panel id="pnlGridHeader" runat="server"  visible="false"/>
				<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel">
					<kh:kindredgridview id="grd" runat="server" 
										cssclass="NetTable" 
										cellpadding="2" 
										cellspacing="0" 
										onrowcommand="grd_RowCommand" 
										autogeneratecolumns="false" 
										rowhighlighting="true" 
										confirmondelete="False" 
										enabledoubleclickevent="false" 
										enableheaderclick="false" 
										enableclickevent="true" 
										headerrowsize="1" 
										container="pnlGridBody"
										headercontainer="pnlGridHeader" 
										sortascending="True" 
										sortcolumn="-1"
										>
							<columns>
							</columns>
					</kh:kindredgridview>
			</asp:panel>
  
    </form>
</body>
</html>

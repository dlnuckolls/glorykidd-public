<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemSearchResults.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemSearchResults" %>
<%@ previouspagetype virtualpath="~/ManagedItemSearch.aspx" %>

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

		<kh:standardheader id="header" runat="server" pagetitle="" />

			<asp:panel id="pnlError" runat="server" visible="false" cssclass="Error">
				<asp:literal id="litError" runat="server" />
			</asp:panel>
			
			<asp:panel id="pnlGridHeader" runat="server" visible="false" ><button id="btnPrint" class="KnectButton" onclick="javascript:CallPrint('gridResults');">Print</button></asp:panel>
			<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="border:solid #333; border-width:1px 1px 1px 1px;">
			
			<div id="gridResults" style="width:100%">
				<kh:kindredgridview id="grdResults" 
							runat="server" 
							cellpadding="2"
							cellspacing="0"	
							cssclass="NetTable"
							onrowcommand="grd_RowCommand"
							autogeneratecolumns="False" 
							datakeynames ="ManagedItemID" 
							rowhighlighting="true" 
							confirmondelete="False" 
							enabledoubleclickevent="false" 
							enableheaderclick="true" 
							enableclickevent="true" 
							headercontainer="pnlGridHeader" 
							headerrowsize="2" 
							container="pnlGridBody" 
							sortascending="True" 
							sortcolumn="-1"
				>
					<columns>
					</columns>
				</kh:kindredgridview>
				</div>
			</asp:panel>


    </form>
</body>
</html>

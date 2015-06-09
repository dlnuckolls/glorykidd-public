<%@ Page Language="C#" AutoEventWireup="true" validaterequest="false" CodeBehind="Report.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.Report" %>
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
			
			<asp:panel id="pnlGridHeader" runat="server" visible="false" />
			<asp:panel id="pnlGridBody" runat="server" cssclass="GridBodyPanel" style="border:solid #333; border-width:1px 1px 1px 1px;">
				<kh:kindredgridview id="grdResults" 
							runat="server" 
							cellpadding="2"
							cellspacing="0"	
							cssclass="NetTable"
							autogeneratecolumns="false" 
							rowhighlighting="false" 
		                    onrowcommand="grd_RowCommand"
					        confirmondelete="False" 
							enabledoubleclickevent="false" 
							enableheaderclick="true" 
							enableclickevent="false" 
							headercontainer="pnlGridHeader" 
							headerrowsize="2" 
							container="pnlGridBody" 
							sortascending="True" 
							sortcolumn="-1"
				>
<%-- 						
datakeynames ="ManagedItemID"   
onrowcommand="grd_RowCommand"
 --%>	
				</kh:kindredgridview>
			</asp:panel>
		<div class="DataEntryCaption" style="text-align:center; padding: 8px 0 8px 0;">
			<asp:button id="btnPrevious" cssclass="KnectButton" runat="server" text="Previous" oncommand="btnPrevious_Click" />&nbsp;&nbsp;
			<asp:button id="btnSave" cssclass="KnectButton" runat="server" text="Save" oncommand="btnSave_Click" />&nbsp;&nbsp;
			<asp:button id="btnCancel" cssclass="KnectButton" runat="server" text="Cancel" oncommand="btnCancel_Click" />&nbsp;&nbsp;
			<asp:button id="btnExport" cssclass="KnectButton" runat="server" text="Export to Excel" onclientclick="return SaveGrid();" oncommand="btnExport_Click"  />
		</div>


    </form>
</body>
</html>

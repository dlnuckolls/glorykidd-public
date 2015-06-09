<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemSearch.aspx.cs" validateRequest=false  Inherits="Kindred.Knect.ITAT.Web.ManagedItemSearch" %>
<!-- the validateRequest=false  was set to store the cookie for search results 3/12/08 LRR --> 
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml" >
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<base target="_self" />
</head>

<body id="body" runat="server" class="NoScroll" title="">
	<IFRAME STYLE="display:none;position:absolute;width:148;height:194;" ID="CalFrame" MARGINHEIGHT="0" MARGINWIDTH="0" NORESIZE FRAMEBORDER="0" SCROLLING="NO" SRC="/Global/bin/Calendar/calendar.htm" onblur="javascript:this.style.display='none';"></IFRAME>
	<SCRIPT Language=JavaScript src="/Global/bin/Calendar/cal_routines.js"></SCRIPT>
	<form id="form1" runat="server" defaultbutton="btnSearch" defaultfocus="txtManagedItemNumber">

		<kh:standardheader id="header" runat="server" pagetitle="" />
<div class="style3" id="print_content"> 
		<asp:panel cssclass="TemplateEditBody AutoScroll" id="pnlCriteria" runat="server">
			<table id="tblCriteria" runat="server" border="0" cellspacing="0" width="100%" style="table-layout:fixed;">

				<tr>
					<td class="ProfileCaption" width="150px"><asp:label id="lblManagedItemNumber" runat="server" /></td>
					<td class="ProfileEdit" width="100%"><asp:textbox id="txtManagedItemNumber" runat="server" maxlength="20" width="100%" enableviewstate="true" /></td>
<%--                    <td width="25px"></td>
--%>				</tr>

				<tr>
					<td colspan="2" class="ProfileCaption" width="100%" height="2px" style="background-color:#666; font-size:1px;">&nbsp;</td>
<%--                    <td width="25px"></td>
--%>				</tr>

				<tr>
					<td class="ProfileCaption" width="150px">Template</td>
					<td class="ProfileEdit" width="100%"><asp:dropdownlist id="ddlTemplate" runat="server" width="100%" enableviewstate="true" /></td>
<%--                    <td width="25px"><asp:CheckBox id="chkTemplate" checked="true" runat="server"></asp:CheckBox></td>
--%>				</tr>

				<tr>
					<td class="ProfileCaption" width="150px">Status</td>
					<td class="ProfileEdit" width="100%"><asp:dropdownlist id="ddlStatus" runat="server" width="100%" enableviewstate="true" /></td>
<%--                    <td width="25px"><asp:CheckBox id="chkStatus" checked="true" runat="server"></asp:CheckBox></td>
--%>				</tr>

				<tr>
					<td class="ProfileCaption" width="150px">Keywords</td>
					<td class="ProfileEdit" width="100%"><asp:textbox id="txtKeywords" runat="server" width="100%" enableviewstate="true" /></td>
<%--                    <td width="25px"></td>
--%>				</tr>
				

			</table>
		</asp:panel>	
		</div> 			

		<div runat="server" id="divSearchButtons" class="ProfileCaption" style="text-align:center; border: solid #333; border-width:0 1px 1px 1px; padding: 3px 3px 3px 3px">
			<asp:button id="btnSearch" runat="server" cssclass="KnectButton" text="Search" oncommand="btnSearch_Command"  onclientclick="javascript:return ShowWait(this);" />
		</div>

		<div runat="server" id="divReportButtons" class="ProfileCaption" style="text-align:center; border: solid #333; border-width:0 1px 1px 1px; padding: 3px 3px 3px 3px">
			<asp:button id="btnReportContinue" runat="server" cssclass="KnectButton" text="Continue" oncommand="btnReportContinue_Command"  onclientclick="javascript:return ShowWait(this);" />&nbsp;&nbsp;
			<asp:button id="btnReportCancel" runat="server" cssclass="KnectButton" text="Cancel" oncommand="btnReportCancel_Command" />
		</div>


	</form>
</body>

</html>

<%@ page  ValidateRequest="false" language="C#" AutoEventWireup="true"  codebehind="ManagedItemProfile.aspx.cs" inherits="Kindred.Knect.ITAT.Web.ManagedItemProfile" title="Untitled Page" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<script type="text/javascript" src="Scripts/ToolTip.js"></script>
</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<iframe style="display:none;position:absolute;width:148;height:194;" id="CalFrame" marginheight="0" marginwidth="0" noresize frameborder="0" scrolling="no" src="/Global/bin/Calendar/calendar.htm" onblur="javascript:this.style.display='none';"></iframe>
	<script type="text/javascript" src="/Global/bin/Calendar/cal_routines.js"></script>
	<form id="form1" runat="server">
		<span id="itatToolTip" class="tooltip"></span>
		<div id="divBody" runat="server">
			<asp:hiddenfield id="hfTargetState" runat="server" value="" />
			<asp:hiddenfield id="hfDoValidation" runat="server" value="0" />
			<asp:hiddenfield id="_kh_hf_PageScrollTop" runat="server" value="0" />
			<asp:hiddenfield id="_kh_hf_FocusControl" runat="server" value="" />
			<asp:hiddenfield id="_kh_hf_FocusTermGroup" runat="server" value="" />
			<asp:hiddenfield id="_kh_hf_DeletedAttachments" runat="server" value="" />
			<kh:manageditemheader id="header" runat="server" onheaderevent="OnHeaderEvent" />

			<asp:panel id="pnlHeaderTerms" runat="server" style="overflow-y:scroll; overflow-x:hidden; border:solid #333; border-width:1px 1px 0 1px;" >
				<asp:table id="tblHeaderTerms" runat="server" cellpadding="0" cellspacing="0" style="table-layout:fixed;">
				</asp:table>
			</asp:panel>
			<asp:panel id="pnlTerms" runat="server" cssclass="GridBodyPanel" style="border: 1px solid #333;">
			</asp:panel>
			
			<asp:panel id="pnlButtons" runat="server" cssclass="DataEntryCaption AlignCenter" style="padding:5px 5px 5px 5px; border:1px solid #333;"></asp:panel>
		</div>
	</form>
</body>
</html>

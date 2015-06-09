<%@ page language="C#" autoeventwireup="true" codebehind="ManagedItemComplexListEdit.aspx.cs" inherits="Kindred.Knect.ITAT.Web.ManagedItemComplexListEdit" title="Untitled Page" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
	<title>Untitled Page</title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
	<script type="text/javascript" src="Scripts/ToolTip.js"></script>
</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<IFRAME STYLE="display:none;position:absolute;width:148;height:194;z-index=100" ID="CalFrame" MARGINHEIGHT="0" MARGINWIDTH="0" NORESIZE FRAMEBORDER="0" SCROLLING="NO" SRC="/Global/bin/Calendar/calendar.htm" onblur="javascript:this.style.display='none';"></IFRAME>
	<SCRIPT Language=JavaScript src="/Global/bin/Calendar/cal_routines.js"></SCRIPT>
	<form id="form1" runat="server">
        <span id="itatToolTip" class="tooltip"></span>
        <kh:manageditemheader id="header" runat="server" onheaderevent="OnHeaderEvent" />

		<asp:panel id="pnlTerms" runat="server" cssclass="GridBodyPanel" style="border: 1px solid #333;">
			<table id="tblTerms" runat="server" border="0" cellpadding="0" cellspacing="0" style="table-layout:fixed; border-top:1px solid #666;">
            
            </table>
		</asp:panel>
		
		<asp:panel id="pnlButtons" runat="server" cssclass="DataEntryCaption AlignCenter" style="padding:5px 5px 5px 5px; border:1px solid #333;" >
		</asp:panel>
	</form>
</body>
</html>

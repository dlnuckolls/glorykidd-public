<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Message.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.MessagePage" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title></title>
		<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
		<link rel="stylesheet" type="text/css" href="itat.css" />
	</head>
	<body id="htmlBody" runat="server">
		<form id="ExceptionPage" method="post" runat="server">
			<div class="PageTitle"><asp:literal id="litSystemName" runat="server" text="ITAT" /></div>
			<p class="Error"><asp:literal runat="server" text="" id="litMessage" /></p>
			<asp:label id="lblVersion" cssclass="Version" runat="server" />
		</form>
	</body>
</html>

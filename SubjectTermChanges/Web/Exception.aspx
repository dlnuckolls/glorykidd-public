<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="Exception.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.ExceptionPage" %>

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
			<p class="Error"><asp:literal id="ExceptionMessageLiteral" runat="server" text="An unknown exception occurred." /></p>
			<p class="Error">If you feel you have reached this page in error, please, contact Desktop Support at (502) 596-2626.</p>
		</form>
	</body>
</html>

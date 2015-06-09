<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AdminClearCache.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.AdminClearCache" %>

<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >

<html xmlns="http://www.w3.org/1999/xhtml">
	<head id="Head1" runat="server">
		<title></title>
		<link rel="stylesheet" type="text/css" href="/global/css/appbody.css" />
		<link rel="stylesheet" type="text/css" href="../itat.css" />
	</head>
	<body id="htmlBody" runat="server">
		<form id="ExceptionPage" method="post" runat="server">
			<div class="PageTitle"><asp:literal id="litSystemName" runat="server" text="ITAT" /></div>
		
			<p>
				Select the system whose cache you want to clear:<br />	
				<asp:dropdownlist id="ddlSystem" runat="server"  />
			</p>
			<p><asp:button id="btnSubmit" runat="server" text="Submit" cssclass="KnectButton" /></p>
		
		</form>
	</body>
</html>

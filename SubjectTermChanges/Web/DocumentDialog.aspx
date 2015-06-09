<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="DocumentDialog.aspx.cs" Inherits="Kindred.Knect.ITAT.Web.DocumentDialog" %>
<!DOCTYPE HTML PUBLIC "-//W3C//DTD HTML 4.01 Transitional//EN" >
<html>
<head id="Head1" runat="server">
	<title></title>
	<link rel="stylesheet" type="text/css" href="/Global/css/AppBody.css" />
	<link rel="stylesheet" type="text/css" href="itat.css" />
</head>
<body id="htmlBody" class="NoScroll" runat="server">
	<table cellpadding="0" cellspacing="0" border="1" style="width:100%; height:100%">
		<tr>
			<td style="height:100%">
			  <iframe id="ifr" runat="server" style="width:100%; height:100%; cursor:wait;" src="" >
			  </iframe>
			</td>
		</tr>
		<tr>
			<td class="1stTblColor AlignCenter" style="height:30px;">
			  <button id="btnclose" class="KNectButton" onclick="javascript:window.close();">Close</button>
			</td>
		</tr>
	</table>		
</body>
</html>

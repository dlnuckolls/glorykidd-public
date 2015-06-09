<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TemplateHeader.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.TemplateHeader" %>
<div id="headerDiv" class="header">
	<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td class="banner"><asp:label  id="lblBanner" runat="server"   /></td>
			<td>&nbsp;<span id="menu" class="menu" runat="server"></span></td>	
		</tr>
		<tr>
			<td style="vertical-align:top;"><asp:label id="lblSubTitle" runat="server" text="Template Name" cssclass="subTitle" /></td>
			<td style="white-space:nowrap; text-align:right; vertical-align:top;">&nbsp;<span  id="buttons" class="buttons" runat="server" /></td>
		</tr>
	</table>
</div>

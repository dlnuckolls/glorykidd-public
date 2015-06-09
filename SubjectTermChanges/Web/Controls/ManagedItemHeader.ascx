<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="ManagedItemHeader.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.ManagedItemHeader" %>
<div id="headerDiv" class="header" runat="server">
	<table border="0" cellpadding="0" cellspacing="0" width="100%">
		<tr>
			<td colspan="2" class="banner" style="width:100%"><asp:label  id="lblBanner" runat="server" /></td>
			<td>&nbsp;<span id="menu" class="menu" runat="server" ></span></td>	
		</tr>
		<tr>
			<td style="width:30%;vertical-align:top;"><asp:label id="lblSubTitle" runat="server" text="Managed Item Name" cssclass="subTitle" /></td>
			<td><asp:label id="lblRoles" cssclass="subTitle" style="width:70%;" runat="server"></asp:label></td>
			<td style="white-space:nowrap; text-align:right; vertical-align:top;">&nbsp;<span  id="buttons" class="buttons" runat="server" /></td>
		</tr>
	</table>	
</div>

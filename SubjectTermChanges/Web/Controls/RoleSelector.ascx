<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RoleSelector.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.RoleSelector" %>

<table border="0" cellpadding="2" cellspacing="0">
	<tr>
		<td id="td1" runat="server" class="DataEntryCaption">
			<asp:label id="litCaption" runat="server" text="Caption Here" />
			<br /><br />
			<asp:button id="btnClear" runat="server" cssclass="KnectButton" style="width:60px;" text="Clear" />
		</td>
		<td id="td2" runat="server" class="DataEntryCaption AlignLeft">
			<asp:listbox id="lstRoles" runat="server" selectionmode="Multiple" rows="6" width="100%" />	
		</td>
	</tr>
</table>

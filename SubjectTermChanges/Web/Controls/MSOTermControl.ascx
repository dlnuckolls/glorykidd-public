<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="MSOTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.MSOTermControl" %>

<asp:panel id="pnlContainer" runat="server">
	<button id="btnLookup" runat="server" class="KnectButton" onclick="javascript:MSOLookup();" style="display:block; width:150px;"	>Lookup Provider</button>
	<asp:datalist id="lstMso" runat="server" repeatlayout="Flow" repeatdirection="vertical" repeatcolumns="1" cellpadding="0" cellspacing="0" width="100%" />
</asp:panel>

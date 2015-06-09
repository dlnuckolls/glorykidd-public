<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="DateTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.DateTermControl" %>

<asp:panel id="pnlContainer" runat="server">
		<asp:textbox id="txt" runat="server" width="114px"  cssclass="DateControlTextBox" maxlength="12" /><asp:image id="img" runat="server"  cssclass="DateControlImage" imagealign="top" imageurl="/Global/bin/Calendar/calendar.gif"  />
		<asp:label id="lbl" runat="server" width="250px" visible="false" />
</asp:panel>
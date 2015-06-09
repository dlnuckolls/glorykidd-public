<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PickListTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.PickListTermControl" %>

<asp:panel id="pnlContainer" runat="server">
	<asp:dropdownlist id="ddl" runat="server" />
	<asp:checkbox id="chk" runat="server" />
	<asp:radiobuttonlist id="rdolst" runat="server" />
	<asp:panel id="grdContainer" runat="server">
		<kh:multiselectgrid id="grd" runat="server" />
	</asp:panel>
	<asp:label id="lbl" runat="server" />
</asp:panel>
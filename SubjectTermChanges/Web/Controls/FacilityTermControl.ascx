<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="FacilityTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.FacilityTermControl" %>

<asp:panel id="pnlContainer" runat="server">
	<asp:dropdownlist id="ddl" runat="server" width="100%" />
	<asp:panel id="grdContainer" runat="server">
		<kh:multiselectgrid id="grd" runat="server" />
	</asp:panel>
	<asp:label id="lbl" runat="server" />
</asp:panel>
<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="RenewalTermControl.ascx.cs" Inherits="Kindred.Knect.ITAT.Web.RenewalTermControl" %>
<asp:panel id="pnlContainer" runat="server">

	<%--  Effective Date row --%>
	<div>
		<asp:label id="lblEffectiveDateCaption" runat="server" text="Effective Date" width="120px" />
		<asp:textbox id="txtEffectiveDate" runat="server" width="114px"  cssclass="DateControlTextBox" maxlength="12" /><asp:image id="imgEffectiveDate" runat="server"  cssclass="DateControlImage" imagealign="top" imageurl="/Global/bin/Calendar/calendar.gif"  />
		<asp:label id="lblEffectiveDateValue" runat="server" visible="false" />
		<asp:label id="lblRenewalCount" runat="server" visible="false" />
	</div>
	
	<%--  Initial Duration row  --%>
	<div>
		<asp:label id="lblInitialDurationCaption" runat="server" text="Initial Duration" width="120px" />
		<asp:textbox id="txtInitialDurationCount" runat="server" width="30px"  maxlength="3" /><asp:dropdownlist id="ddlInitialDurationUnits" runat="server" style="margin:0 0 0 2px;" width="135px" />
		<asp:label id="lblInitialDurationValue" runat="server" width="250px" />
	</div>
	
	<%--  Renewal Duration row  --%>
	<div id="divRenewalDuration" runat="server">
		<asp:label id="lblRenewalDurationCaption" runat="server" text="Renewal Duration" width="120px" />
		<asp:textbox id="txtRenewalDurationCount" runat="server" width="30px"  maxlength="3" /><asp:dropdownlist id="ddlRenewalDurationUnits" runat="server" style="margin:0 0 0 2px;" width="135px" />
		<asp:label id="lblRenewalDurationValue" runat="server" width="250px" visible="false" />
	</div>

	<%--  Expiration/Renewal Date row  --%>
	<asp:panel id="pnlRenewalDate" runat="server" cssclass="NoBorder">
			<asp:label id="lblRenewalDateCaption" runat="server" text="" width="120px" />
			<asp:label id="lblRenewalDateValue" runat="server" width="250px"  />
	</asp:panel>
	
	<%--  Renew Button row  --%>
	<asp:panel id="pnlRenewButton" runat="server" cssclass="NoBorder" visible="false">
			<asp:button id="btnRenew" runat="server" text="Renew" cssclass="KnectButton" forecolor="Red" onclick="btnRenewOnClick" />
	</asp:panel>

	<asp:panel id="pnlRenewButtonHidden" runat="server" cssclass="NoBorder" visible="false">
			<asp:button id="btnRenewHidden" runat="server" text="Renew Hidden Due To:" cssclass="KnectButton" style="Width:150px" forecolor="Red" />
	</asp:panel>
	
</asp:panel>


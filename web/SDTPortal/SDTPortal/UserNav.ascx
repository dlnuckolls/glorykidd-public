<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="UserNav.ascx.cs" Inherits="SDTPortal.UserNav" %>

<telerik:RadMenu ID="RadAccountMenu" CssClass="mainMenu" Font-Size="1em" Skin="Silk" ShowToggleHandle="true" Flow="Horizontal" runat="server" Width="100%" RenderMode="Lightweight" EnableRoundedCorners="true" >
  <Items>
    <telerik:RadMenuItem Text="Select Files to Upload" NavigateUrl="~/MyUploads.aspx" Width="50%" Height="38px" />
    <telerik:RadMenuItem IsSeparator="true" />
    <telerik:RadMenuItem Text="Transmission History" NavigateUrl="~/MyTransactions.aspx" Width="50%" Height="38px" />
  </Items>
</telerik:RadMenu>

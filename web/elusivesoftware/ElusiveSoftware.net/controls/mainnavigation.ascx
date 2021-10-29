<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="mainnavigation.ascx.cs" Inherits="ElusiveSoftware.net.controls.mainnavigation" %>
<telerik:RadMenu ID="RadMenu1" runat="server" RenderMode="Auto" Skin="WebBlue">
  <Items>
    <telerik:RadMenuItem Text="Home" NavigateUrl="~/" />
    <telerik:RadMenuItem IsSeparator="true" />
    <telerik:RadMenuItem Text="Portfolio">
      <Items>
        <telerik:RadMenuItem Text="Web Services" NavigateUrl="~/webservices" />
        <telerik:RadMenuItem IsSeparator="true" />
        <telerik:RadMenuItem Text="Hosting Plans" NavigateUrl="~/ourplans" />
        <telerik:RadMenuItem IsSeparator="true" />
        <telerik:RadMenuItem Text="Custom Software" NavigateUrl="~/softwareservices" />
        <telerik:RadMenuItem IsSeparator="true" />
        <telerik:RadMenuItem Text="Consulting Services" NavigateUrl="~/consulting" />
      </Items>
    </telerik:RadMenuItem>
    <telerik:RadMenuItem IsSeparator="true" />
    <telerik:RadMenuItem Text="About Us" NavigateUrl="~/contact" />
  </Items>
</telerik:RadMenu>

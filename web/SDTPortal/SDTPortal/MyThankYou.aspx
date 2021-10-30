<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyThankYou.aspx.cs" Inherits="SDTPortal.MyThankYou" %>

<%@ Register TagPrefix="uc1" TagName="usernav" Src="~/UserNav.ascx" %>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="HeaderArea" runat="server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn CssClass="apptitle">
            <h2>
              <telerik:RadLabel ID="SiteApplicationTitle" runat="server" />
            </h2>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn CssClass="apptext">
            <p>
              <telerik:RadLabel ID="SiteApplicationInstructions" runat="server" />
            </p>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>
<asp:Content runat="server" ID="ActionButtonArea" ContentPlaceHolderID="ActionButtons">
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" OnClick="rbPassword_OnClick" /></div>
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" /></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <asp:Panel ID="CompletedView" runat="server">
    <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
      <Rows>
        <telerik:LayoutRow CssClass="content">
          <Columns>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="8" SpanMd="8" SpanSm="12" HiddenXs="true">
              <uc1:usernav runat="server" ID="UserNav" />
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="2" SpanMd="12" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12" HiddenXs="true" CssClass="apptext">-- TRANSMISSON COMPLETED --<br />
              <br />
              What would you like to do next?
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </asp:Panel>
</asp:Content>

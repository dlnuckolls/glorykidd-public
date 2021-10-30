<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyAccount.aspx.cs" Inherits="SDTPortal.MyAccount" %>

<%@ Register Src="~/UserNav.ascx" TagPrefix="uc1" TagName="UserNav" %>

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
<asp:Content runat="server" ID="NavButtons" ContentPlaceHolderID="ActionButtons">
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" Enabled="False" /></div>
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" /></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <asp:Panel ID="SettingsView" runat="server">
    <div id="messageDisplayArea">
      <telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="errorMessageDisplay" />
    </div>
    <telerik:RadPageLayout runat="server" ID="RadPageLayout2" OnPreRender="RadPageLayout4_OnPreRender">
      <Rows>
        <telerik:LayoutRow CssClass="content">
          <Columns>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="8" SpanMd="8" SpanSm="12">
              <uc1:UserNav runat="server" ID="UserNav" />
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow CssClass="content">
          <Columns>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="4" SpanMd="6" SpanSm="12">
              <div style="width: 500px;">
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadTextBox ID="LoginId" runat="server" Width="100%" Label="Email Address" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" Enabled="False" /></div>
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadTextBox ID="DisplayName" runat="server" Width="100%" Label="Display Name" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" Enabled="False" /></div>
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadTextBox ID="NewPassword" TextMode="Password" runat="server" Width="100%" Label="New Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" >
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox></div>
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadTextBox ID="ConfirmPassword" TextMode="Password" runat="server" Width="100%" Label="Confirm Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" >
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox></div>
                <div style="width: 100%; padding: 5px; margin-left: 170px;">
                  <telerik:RadButton ID="RadButton3" runat="server" Skin="Silk" RenderMode="Auto" Text="Save" OnClick="RadButton3_OnClick" CssClass="css3Simple" />
                </div>
              </div>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </asp:Panel>
</asp:Content>

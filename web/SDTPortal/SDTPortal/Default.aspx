<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="SDTPortal.Default" %>

<asp:Content ID="Content0" ContentPlaceHolderID="head" runat="Server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content1" ContentPlaceHolderID="HeaderArea" runat="server">
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
          <telerik:LayoutColumn CssClass="apptext" />
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContentArea" runat="Server">
  <telerik:RadAjaxPanel runat="server">
    <telerik:RadPageLayout runat="server" ID="loginPage">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="4" SpanMd="6" SpanSm="12">
              <div style="width: 500px;">
                <div style="width: 100%; padding: 5px;"><telerik:RadTextBox ID="userName" runat="server" Width="100%" Label="Email Address" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel" >
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox></div>
                <div style="width: 100%; padding: 5px;"><telerik:RadTextBox ID="password" TextMode="Password" runat="server" Width="100%" Label="Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel" >
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox></div>
                <div style="width: 100%; padding: 5px; margin-left: 150px;"><telerik:RadButton ID="SubmitLogin" runat="server" Skin="Silk" RenderMode="Auto" Text="Login" OnClick="SubmitLogin_OnClick" CssClass="css3Simple2" />&nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="appErrorMessage" /></div>
                <div style="width: 100%; padding: 5px; margin-left: 153px;"><asp:LinkButton ID="ForgotPassword" runat="server" CssClass="forgotText" Text="Forgot&nbsp;Password" OnClick="ForgotPassword_OnClick" /></div>
              </div>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

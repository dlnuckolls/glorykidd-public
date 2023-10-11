<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="Directory.CGBC.Default" %>

<asp:Content ID="Content0" ContentPlaceHolderID="PageTitle" runat="Server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContentArea" runat="Server">
  <telerik:RadAjaxPanel runat="server">
    <telerik:RadPageLayout runat="server" ID="loginPage">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="12" HiddenSm="true">
              <div style="width: 100%; height: 200px;"></div>
            </telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="4" SpanMd="3" HiddenSm="true" HiddenXs="true" />
            <telerik:LayoutColumn Span="4" SpanMd="6" SpanSm="12" SpanXs="12">
              <telerik:RadAjaxPanel ID="UpdatePanel1" runat="server">
                <div style="min-width: 400px;">
                  <telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="errorMessageDisplay" />
                  <asp:Login ID="Login2" runat="server" Width="100%" EnableViewState="false" OnAuthenticate="Login2_Authenticate">
                    <LayoutTemplate>
                      <div style="width: 100%; padding: 5px;">
                        <telerik:RadTextBox ID="UserName" runat="server" Width="100%" Label="Email Address" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel" AutoComplete="username">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="UserName" CssClass="appErrorMessage" Display="Dynamic"
                          ErrorMessage="User Name is required." ToolTip="User Name is required." ValidationGroup="Login1"></asp:RequiredFieldValidator><br />
                      </div>
                      <div style="width: 100%; padding: 5px;">
                        <telerik:RadTextBox ID="Password" TextMode="Password" runat="server" Width="100%" Label="Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel" AutoComplete="current-password">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="PasswordRequired" runat="server" ControlToValidate="Password" CssClass="appErrorMessage" Display="Dynamic"
                          ErrorMessage="Password is required." ToolTip="Password is required." ValidationGroup="Login1"></asp:RequiredFieldValidator>
                      </div>
                      <div style="width: 100%; padding: 5px; margin-left: 100px;">
                        <telerik:RadButton ID="LoginButton" runat="server" CommandName="Login" Text="Log In" ValidationGroup="Login1" Skin="Silk" CssClass="css3Simple2"></telerik:RadButton>&nbsp;&nbsp;&nbsp;&nbsp;                        <asp:LinkButton ID="ForgotPassword" runat="server" CssClass="forgotText" Text="Forgot&nbsp;Password" OnClick="ForgotPassword_OnClick" />
                      </div>
                    </LayoutTemplate>
                  </asp:Login>
                </div>
              </telerik:RadAjaxPanel>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4" SpanMd="3" HiddenSm="true" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

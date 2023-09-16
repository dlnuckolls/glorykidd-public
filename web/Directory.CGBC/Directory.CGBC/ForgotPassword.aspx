<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="Directory.CGBC.ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxPanel runat="server">
    <telerik:RadPageLayout runat="server" ID="loginPage">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="12" HiddenSm="true"><div style="width: 100%;height: 200px;"></div></telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="3" SpanMd="3" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="4" SpanMd="4" SpanSm="12" >
              <div style="width: 500px;">
              <div style="width: 100%; padding: 5px; margin-left: 170px;"><telerik:RadLabel ID="lMessage" runat="server" Text="Enter your email address below and a<br />new password will be sent to you." /></div>
                <div style="width: 100%; padding: 5px;"><telerik:RadTextBox ID="userName" runat="server" Width="100%" Label="Email Address" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" >
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox></div>
                <div style="width: 100%; padding: 5px; margin-left: 170px;"><telerik:RadButton ID="SubmitLogin" Skin="Silk" RenderMode="Auto" runat="server" Text="Reset Password" OnClick="SubmitLogin_OnClick" CssClass="css3Simple" />&nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="appErrorMessage" /></div>
                <div style="width: 100%; padding: 5px; margin-left: 170px;"><telerik:RadButton ID="ReturnToLogin" Skin="Silk" RenderMode="Auto" runat="server" Text="Cancel" OnClick="ReturnToLogin_OnClick" CssClass="css3Simple" /></div>
              </div>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="5" SpanMd="5" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

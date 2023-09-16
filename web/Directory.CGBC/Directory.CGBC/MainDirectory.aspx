<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.Master" AutoEventWireup="true" CodeBehind="MainDirectory.aspx.cs" Inherits="Directory.CGBC.MainDirectory" %>

<asp:Content ID="PageContent1" ContentPlaceHolderID="PageTitle" runat="server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <telerik:RadCodeBlock ID="RadCodeBlock1" runat="server">
    <script type="text/javascript">
      function RowDblClick(sender, eventArgs) {
        sender.get_masterTableView().editItem(eventArgs.get_itemIndexHierarchical());
      }
      function onPopUpShowing(sender, args) {
        args.get_popUp().className += " popUpEditForm";
      }
    </script>
  </telerik:RadCodeBlock>
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
  <telerik:RadWindow RenderMode="Auto" ID="PasswordChange" runat="server" Width="800px" Height="500px" Modal="true" Style="z-index: 1001;"
    Behaviors="Close" VisibleOnPageLoad="False" OpenerElementID="rbPassword" Skin="Outlook" ReloadOnShow="True" IconUrl="~/images/logo.jpg">
    <ContentTemplate>
      <telerik:RadAjaxPanel ID="UpdatePanel1" runat="server">
        <div style="min-width: 400px;">
          <telerik:RadPageLayout runat="server" ID="PopupLayout" Width="600px">
            <Rows>
              <telerik:LayoutRow>
                <Columns>
                  <telerik:LayoutColumn CssClass="apptitle">
                    <h2>
                      <telerik:RadLabel ID="RadLabel1" Text="Change My Password" runat="server" />
                    </h2>
                  </telerik:LayoutColumn>
                </Columns>
              </telerik:LayoutRow>
              <telerik:LayoutRow>
                <Columns>
                  <telerik:LayoutColumn>
                    <div style="width: 500px;">
                      <telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="errorMessageDisplay" />
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="CurrentPassword" TextMode="Password" runat="server" Width="100%" Label="Current Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="CurrentPassword" CssClass="appErrorMessage" Display="Dynamic"
                          ErrorMessage="Current Password is Required" ToolTip="Current Password is Required" ValidationGroup="Login1" ForeColor="Red"></asp:RequiredFieldValidator><br />
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="NewPassword" TextMode="Password" runat="server" Width="100%" Label="New Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NewPassword" CssClass="appErrorMessage" Display="Dynamic"
                          ErrorMessage="New Password is Required" ToolTip="New Password is Required" ValidationGroup="Login1" ForeColor="Red"></asp:RequiredFieldValidator><br />
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="ConfirmPassword" TextMode="Password" runat="server" Width="100%" Label="Confirm Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                        <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" Display="Dynamic"
                          ErrorMessage="Confirm Password is Required" ToolTip="Confirm Password is Required" ValidationGroup="Login1" ForeColor="Red"></asp:RequiredFieldValidator><br />
                        <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="New Password and Confirmation did not match" Display="Dynamic" ControlToCompare="NewPassword"
                          ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" ValidationGroup="Login1" ForeColor="Red"></asp:CompareValidator>
                      </div>
                      <div style="width: 100%; padding: 5px; margin-left: 100px;">
                        <telerik:RadButton ID="ConfirmChangePassword" runat="server" CommandName="Submit" Text="Change Password" ValidationGroup="Login1" Skin="Silk" CssClass="css3Simple3"
                          OnClick="ConfirmChangePassword_Click">
                        </telerik:RadButton>
                      </div>
                    </div>
                  </telerik:LayoutColumn>
                </Columns>
              </telerik:LayoutRow>
            </Rows>
          </telerik:RadPageLayout>
          <%--          <asp:ChangePassword ID="ChangePassword1" runat="server" LabelStyle-CssClass="MyLabel" TextBoxStyle-CssClass="MyEnabledTextBox3"
            PasswordHintText="Please enter a password at least 7 characters long, containing a number and one special character."
            NewPasswordRegularExpression='@\"(?=.{7,})(?=(.*\d){1,})(?=(.*\W){1,})' NewPasswordRegularExpressionErrorMessage="Error: Your password must be at least 7 characters long, and contain at least one number and one special character.">
          </asp:ChangePassword>--%>
        </div>
      </telerik:RadAjaxPanel>
    </ContentTemplate>
  </telerik:RadWindow>
  <div style="width: 100%; padding: 75px 5px 25px;">
    <telerik:RadLabel ID="CurrentUser" runat="server"></telerik:RadLabel>
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" />
  </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" BackgroundPosition="Center">
    <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' style="border: 0;" />
  </telerik:RadAjaxLoadingPanel>

</asp:Content>

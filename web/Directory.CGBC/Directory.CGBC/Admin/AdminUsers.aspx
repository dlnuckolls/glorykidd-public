<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.Master" AutoEventWireup="true" CodeBehind="AdminUsers.aspx.cs" Inherits="Directory.CGBC.AdminUsers" %>

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
            <div style="width: 100%; padding: 5px;">
              <telerik:RadLabel ID="CurrentUser" runat="server" CssClass="headertitle" />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<telerik:RadButton ID="rbdirectory" runat="server" Skin="Silk" RenderMode="Auto" Text="Directory" OnClick="rbdirectory_Click" CssClass="css3SimpleAction" />
              &nbsp;&nbsp;<telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" />
              &nbsp;&nbsp;<telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" />
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
  <telerik:RadWindow RenderMode="Auto" ID="PasswordChange" runat="server" Width="800px" Height="400px" Modal="true" Style="z-index: 1001;"
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
                      <telerik:RadTextBox runat="server" ID="username" name="username" type="hidden" value="NotRequired" autocomplete="username" />
                      <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="CurrentPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="Current Password is Required<br />" ToolTip="Current Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NewPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="New Password is Required<br />" ToolTip="New Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="Confirm Password is Required<br />" ToolTip="Confirm Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="New Password and Confirmation did not match<br />" Display="Dynamic" ControlToCompare="NewPassword"
                        ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:CompareValidator>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="CurrentPassword" TextMode="Password" runat="server" Width="100%" Label="Current Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3" AutoComplete="current-password">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="NewPassword" TextMode="Password" runat="server" Width="100%" Label="New Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3" AutoComplete="new-password">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="ConfirmPassword" TextMode="Password" runat="server" Width="100%" Label="Confirm Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3" AutoComplete="new-password">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                      </div>
                      <div style="width: 100%; padding: 5px; margin-left: 100px;">
                        <telerik:RadButton ID="ConfirmChangePassword" runat="server" CommandName="Submit" Text="Change Password" ValidationGroup="Login1" Skin="Silk" CssClass="css3Simple3" OnClick="ConfirmChangePassword_Click" />
                        <telerik:RadLabel ID="SuccessLabel" Text="" runat="server" CssClass="successMessageDisplay" />
                      </div>
                    </div>
                  </telerik:LayoutColumn>
                </Columns>
              </telerik:LayoutRow>
            </Rows>
          </telerik:RadPageLayout>
        </div>
      </telerik:RadAjaxPanel>
    </ContentTemplate>
  </telerik:RadWindow>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
    <telerik:RadPageLayout runat="server" ID="PageContentArea">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="2" HiddenMd="true" HiddenSm="true" HiddenXs="true" />
            <telerik:LayoutColumn Span="8" SpanMd="12" SpanSm="12" SpanXs="12">
              <telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="errorMessageDisplay" />
              <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="UserList" AllowPaging="true" Width="100%" PagerStyle-AlwaysVisible="True" AllowSorting="true" OnItemCommand="UserList_ItemCommand"
                HorizontalAlign="Left" AutoGenerateColumns="False" CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" OnDeleteCommand="UserList_DeleteCommand"
                MasterTableView-GridLines="Horizontal" GroupingSettings-CaseSensitive="false" OnNeedDataSource="UserList_NeedDataSource" OnUpdateCommand="UserList_EditCommand" OnInsertCommand="UserList_InsertCommand">
                <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None" ClientDataKeyNames="Id" CommandItemDisplay="Bottom" InsertItemPageIndexAction="ShowItemOnFirstPage">
                  <Columns>
                    <telerik:GridDropDownColumn ShowFilterIcon="false" ListValueField="Id" ListTextField="Name" DataField="RoleId" HeaderText="Role" DataSourceID="ObjectDataSource1" />
                    <telerik:GridBoundColumn ShowFilterIcon="false" FilterControlWidth="100%" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" DataField="DisplayName" AllowFiltering="True" HeaderText="Display" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="UserName" AllowFiltering="false" HeaderText="UserName" AllowSorting="true" />
                    <telerik:GridDropDownColumn DropDownControlType="RadComboBox" ListValueField="Id" ListTextField="Name" DataField="MemberId" HeaderText="Member" DataSourceID="ObjectDataSource2" ItemStyle-Width="250px" ColumnEditorID="dropdownEditor" />
                    <telerik:GridButtonColumn ShowFilterIcon="false" HeaderText="Password" ButtonType="LinkButton" Text="Reset&nbsp;Password" CommandName="ResetPassword" ShowInEditForm="False" ItemStyle-Width="150px" />
                    <telerik:GridEditCommandColumn />
                    <telerik:GridButtonColumn ConfirmText="Delete this User?" ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ButtonType="FontIconButton" CommandName="Delete" ItemStyle-Width="40px" />
                  </Columns>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="true">
                  <Selecting AllowRowSelect="true" EnableDragToSelectRows="false" />
                </ClientSettings>
                <PagerStyle Mode="NextPrevAndNumeric" />
              </telerik:RadGrid>
              <telerik:GridDropDownListColumnEditor ID="dropdownEditor" runat="server" DataTextField="Name" DataValueField="Id">
                <DropDownStyle Width="100%" />
              </telerik:GridDropDownListColumnEditor>
              <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetAdminRoles" TypeName="Directory.CGBC.Helpers.SqlDataLoader" />
              <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="GetUserMembers" TypeName="Directory.CGBC.Helpers.SqlDataLoader" />
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="2" HiddenMd="true" HiddenSm="true" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>


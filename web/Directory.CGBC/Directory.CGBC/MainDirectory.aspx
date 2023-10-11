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
            <div style="width: 100%; padding: 5px;">
              <telerik:RadLabel ID="CurrentUser" runat="server" CssClass="headertitle" />
              &nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;<telerik:RadButton ID="SuperAdmin" runat="server" Skin="Silk" RenderMode="Auto" Text="Administration" OnClick="SuperAdmin_Click" CssClass="css3SimpleAction" />
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
                      <telerik:RadTextBox runat="server" id="username" name="username" type="hidden" value="NotRequired" autocomplete="username"/>
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
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" BackgroundPosition="Center">
    <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' style="border: 0;" />
  </telerik:RadAjaxLoadingPanel>
  <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
    <telerik:RadPageLayout runat="server" ID="PageContentArea">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="2" HiddenMd="true" HiddenSm="true" HiddenXs="true" />
            <telerik:LayoutColumn Span="8" SpanMd="12" SpanSm="12" SpanXs="12">
              <div style="width: 70%; float: left; text-align: center;">
                <telerik:RadLabel ID="RadLabel12" runat="server" Text="All Member Directory" CssClass="headertitle" />
              </div>
              <div style="width: 28%; float: right; text-align: right;">
                <telerik:RadButton ID="NewMember" runat="server" Primary="true" Skin="Silk" RenderMode="Auto" Text="New Member" OnClick="NewMember_Click" />
              </div>
              <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="MemberList" AllowPaging="true" Width="100%" PagerStyle-AlwaysVisible="True" AllowSorting="true" AllowFilteringByColumn="true"
                HorizontalAlign="Left" AutoGenerateColumns="False" CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" PageSize="50"
                MasterTableView-GridLines="Horizontal" GroupingSettings-CaseSensitive="false" OnNeedDataSource="MemberList_NeedDataSource" OnItemCommand="MemberList_ItemCommand" OnPreRender="MemberList_PreRender">
                <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None" ClientDataKeyNames="Id">
                  <Columns>
                    <telerik:GridBoundColumn ShowFilterIcon="false" FilterControlWidth="100%" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" DataField="LastName" AllowFiltering="True" HeaderText="Last Name" AllowSorting="true" />
                    <telerik:GridBoundColumn ShowFilterIcon="false" FilterControlWidth="100%" AutoPostBackOnFilter="true" CurrentFilterFunction="Contains" DataField="FirstName" AllowFiltering="True" HeaderText="First Name" AllowSorting="true" />
                    <telerik:GridBoundColumn DataField="CellPhone" AllowFiltering="false" HeaderText="Cell" AllowSorting="true" ItemStyle-Width="100px" />
                    <telerik:GridBoundColumn DataField="HomePhone" AllowFiltering="false" HeaderText="Home" AllowSorting="true" ItemStyle-Width="100px" />
                    <telerik:GridDateTimeColumn ShowFilterIcon="false" DataField="LastUpdate" AllowFiltering="False" HeaderText="Last Update" AllowSorting="true" ItemStyle-Width="100px" DataFormatString="{0:MM/dd/yyyy}" />
                    <telerik:GridButtonColumn UniqueName="EditMemberRow" ButtonType="LinkButton" Text="Edit" CommandName="EditRow" />
                  </Columns>
                  <NestedViewTemplate>
                    <telerik:RadPageLayout ID="DisplayMemberDetails" runat="server" CssClass="apptext">
                      <Rows>
                        <telerik:LayoutRow>
                          <Columns>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel2" runat="server" Text="Name:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberName" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel3" runat="server" Text="Status:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberStatus" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                          </Columns>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow>
                          <Columns>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel4" runat="server" Text="Address:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberAddress" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel8" runat="server" Text="BirthDate:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberDob" runat="server" CssClass=" labels" /><br />
                              <telerik:RadLabel ID="RadLabel10" runat="server" Text="Marriage:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberMarriage" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                          </Columns>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow>
                          <Columns>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel6" runat="server" Text="Phone:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberPhone" runat="server" CssClass=" labels" /><br />
                              <telerik:RadLabel ID="RadLabel9" runat="server" Text="Email:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberEmails" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                            <telerik:LayoutColumn Span="6">
                              <telerik:RadLabel ID="RadLabel5" runat="server" Text="Related:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberRelation" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                          </Columns>
                        </telerik:LayoutRow>
                        <telerik:LayoutRow>
                          <Columns>
                            <telerik:LayoutColumn Span="12">
                              <telerik:RadLabel ID="RadLabel7" runat="server" Text="Notes:" Font-Bold="true" CssClass="labelText labels" />
                              &nbsp;&nbsp;<telerik:RadLabel ID="MemberNotes" runat="server" CssClass=" labels" />
                            </telerik:LayoutColumn>
                          </Columns>
                        </telerik:LayoutRow>
                      </Rows>
                    </telerik:RadPageLayout>
                  </NestedViewTemplate>
                </MasterTableView>
                <ClientSettings EnableRowHoverStyle="true" EnablePostBackOnRowClick="true">
                  <Scrolling AllowScroll="True" UseStaticHeaders="True" SaveScrollPosition="true"  />
                  <Selecting AllowRowSelect="true" EnableDragToSelectRows="false" />
                </ClientSettings>
                <PagerStyle Mode="NextPrevAndNumeric" />
              </telerik:RadGrid>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="2" HiddenMd="true" HiddenSm="true" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

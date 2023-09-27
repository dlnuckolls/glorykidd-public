<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.Master" AutoEventWireup="true" CodeBehind="EditMember.aspx.cs" Inherits="Directory.CGBC.EditMember" %>

<asp:Content ID="PageContent1" ContentPlaceHolderID="PageTitle" runat="server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
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
          <telerik:LayoutColumn>
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
                      <asp:RequiredFieldValidator ID="UserNameRequired" runat="server" ControlToValidate="CurrentPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="Current Password is Required<br />" ToolTip="Current Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="NewPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="New Password is Required<br />" ToolTip="New Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" Display="Dynamic"
                        ErrorMessage="Confirm Password is Required<br />" ToolTip="Confirm Password is Required" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:RequiredFieldValidator>
                      <asp:CompareValidator ID="CompareValidator1" runat="server" ErrorMessage="New Password and Confirmation did not match<br />" Display="Dynamic" ControlToCompare="NewPassword"
                        ControlToValidate="ConfirmPassword" CssClass="appErrorMessage" ValidationGroup="Login1" ForeColor="Red" Font-Size="1em"></asp:CompareValidator>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="CurrentPassword" TextMode="Password" runat="server" Width="100%" Label="Current Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="NewPassword" TextMode="Password" runat="server" Width="100%" Label="New Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
                          <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                          <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                        </telerik:RadTextBox>
                      </div>
                      <div style="width: 100%; padding: 1px;">
                        <telerik:RadTextBox ID="ConfirmPassword" TextMode="Password" runat="server" Width="100%" Label="Confirm Password" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel3">
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
  <div style="width: 100%; padding: 25px 5px 25px;">
    <telerik:RadLabel ID="CurrentUser" runat="server"></telerik:RadLabel>
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbdirectory" runat="server" Skin="Silk" RenderMode="Auto" Text="Directory" OnClick="rbdirectory_Click" CssClass="css3SimpleAction" />
  </div>
</asp:Content>
<asp:Content ID="Content5" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" BackgroundPosition="Center">
    <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' style="border: 0;" />
  </telerik:RadAjaxLoadingPanel>
  <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
    <telerik:RadPageLayout ID="DisplayMemberDetails" runat="server" CssClass="apptext">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="6">
              <telerik:RadLabel ID="RadLabel2" runat="server" Text="Name:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;<telerik:RadDropDownList ID="rddSalutation" runat="server" Width="80px" Skin="Silk" />
              <telerik:RadTextBox ID="tMemberFirstName" runat="server" EmptyMessage="First" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" />
              <telerik:RadTextBox ID="tMemberMiddleName" runat="server" EmptyMessage="Middle" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
              <telerik:RadTextBox ID="tMemberLastName" runat="server" EmptyMessage="Last" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" />
              <telerik:RadTextBox ID="tMemberSuffix" runat="server" EmptyMessage="Suffix" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="6">
              <telerik:RadLabel ID="RadLabel3" runat="server" Text="Status:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;<telerik:RadDropDownList ID="rddMaritalStatus" runat="server" Width="200px" Skin="Silk" />
            </telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="6">
              <telerik:RadLabel ID="RadLabel4" runat="server" Text="Address:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;<telerik:RadTextBox ID="tMemberAddress1" runat="server" EmptyMessage="Address Line 1" Width="450px" Skin="Silk" CssClass="MyEnabledTextBox2" /><br />
              &nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="RadLabel12" runat="server" Text="" Font-Bold="true" CssClass="labelText labels" /><telerik:RadTextBox ID="tMemberAddress2" runat="server" EmptyMessage="Address Line 2" Skin="Silk" Width="450px" CssClass="MyEnabledTextBox2" /><br />
              &nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="RadLabel13" runat="server" Text="" Font-Bold="true" CssClass="labelText labels" /><telerik:RadTextBox ID="tMemberCity" runat="server" EmptyMessage="City" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" /><telerik:RadDropDownList ID="rddStates" runat="server" Skin="Silk" Width="180px" />
              &nbsp;<telerik:RadTextBox ID="tMemberZip" runat="server" EmptyMessage="Zip" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="6">
              <telerik:RadLabel ID="RadLabel8" runat="server" Text="BirthDate:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;<telerik:RadDatePicker ID="dpMemberBirthdate" runat="server" Skin="Silk" DateInput-EnabledStyle-CssClass="MyEmptyTextBox2" />
              <br />
              <telerik:RadLabel ID="RadLabel10" runat="server" Text="Marriage:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;<telerik:RadDatePicker ID="dpMemberMarriage" runat="server" Skin="Silk" DateInput-EnabledStyle-CssClass="MyEmptyTextBox2" />
            </telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="6">
                <div style="padding-left: 120px;">
                  <telerik:RadGrid Skin="Silk" RenderMode="Auto" ID="gMemberPhones" runat="server" Width="70%" AllowPaging="false" PagerStyle-AlwaysVisible="false"
                    HorizontalAlign="Left" AutoGenerateColumns="False" CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0"
                    MasterTableView-GridLines="Horizontal" GroupingSettings-CaseSensitive="false">
                    <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None" ClientDataKeyNames="Id" InsertItemDisplay="Top" InsertItemPageIndexAction="ShowItemOnFirstPage">
                      <Columns>
                        <telerik:GridEditCommandColumn />
                        <telerik:GridBoundColumn DataField="FormattedPhoneNumber" HeaderText="Phone" />
                        <telerik:GridDropDownColumn UniqueName="ddl1" ListTextField="PhoneType.Name" ListValueField="PhoneType.Id" HeaderText="Type"  />
                      </Columns>
                    </MasterTableView>
                    <ClientSettings EnableRowHoverStyle="true">
                      <Selecting AllowRowSelect="True" />
                    </ClientSettings>
                  </telerik:RadGrid>
                </div>
              <telerik:RadLabel ID="RadLabel9" runat="server" Text="Email:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="6">
              <telerik:RadLabel ID="RadLabel5" runat="server" Text="Related:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;
            </telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="8">
              <telerik:RadLabel ID="RadLabel7" runat="server" Text="Notes:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4">
              <telerik:RadLabel ID="RadLabel11" runat="server" Text="Saved:" Font-Bold="true" CssClass="labelText labels" />
              &nbsp;&nbsp;
            </telerik:LayoutColumn>
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

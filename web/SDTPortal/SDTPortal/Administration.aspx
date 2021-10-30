<%@ Page Title="" Language="C#" MasterPageFile="~/Administrator.Master" AutoEventWireup="true" CodeBehind="Administration.aspx.cs" Inherits="SDTPortal.Administration" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <title>Administration</title>
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
            <telerik:RadLabel ID="SiteApplicationInstructions" runat="server" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="10" SpanMd="10" HiddenSm="True" />
          <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12">
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>
<asp:Content runat="server" ID="ActionButtonArea" ContentPlaceHolderID="ActionButtons">
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" OnClick="rbPassword_OnClick" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Log Out" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadLabel ID="SuperAdmin" runat="server" CssClass="adminUserDisplay" />
  </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxPanel runat="server">
    <div id="messageDisplayArea">
      <telerik:RadLabel runat="server" ID="MessageDisplay"></telerik:RadLabel>
    </div>
    <telerik:RadTabStrip RenderMode="Lightweight" runat="server" ID="RadTabStrip1" MultiPageID="RadMultiPage1" OnTabClick="RadTabStrip1_OnTabClick" SelectedIndex="0" Skin="Silk" Align="Center">
      <Tabs>
        <telerik:RadTab Text="Clients"></telerik:RadTab>
        <telerik:RadTab Text="Upload History"></telerik:RadTab>
        <telerik:RadTab Text="Recipients"></telerik:RadTab>
        <telerik:RadTab Text="Admin Accounts"></telerik:RadTab>
        <telerik:RadTab Text="Header Messages"></telerik:RadTab>
        <telerik:RadTab Text="Server Details"></telerik:RadTab>
        <telerik:RadTab Text="Password"></telerik:RadTab>
        <telerik:RadTab Text="Logout" NavigateUrl="~/Logout.aspx"></telerik:RadTab>
      </Tabs>
    </telerik:RadTabStrip>
    <telerik:RadMultiPage runat="server" ID="RadMultiPage1" SelectedIndex="0">
      <telerik:RadPageView runat="server" ID="ClientAccounts">
        <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="ClientAccountsList" AllowPaging="true" Width="100%"
          PagerStyle-AlwaysVisible="True" AllowSorting="true" HorizontalAlign="Left" AutoGenerateColumns="False"
          CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" MasterTableView-GridLines="Horizontal"
          OnUpdateCommand="ClientAccountsList_OnUpdateCommand" OnInsertCommand="ClientAccountsList_OnInsertCommand" OnItemCommand="ClientAccountsList_OnItemCommand"
          OnDeleteCommand="Account_OnDeleteCommand" OnNeedDataSource="ClientAccountsList_OnNeedDataSource">
          <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None"
            ClientDataKeyNames="Id" CommandItemDisplay="Bottom" InsertItemPageIndexAction="ShowItemOnFirstPage">
            <Columns>
              <telerik:GridBoundColumn ShowFilterIcon="False" DataField="DisplayName" AllowFiltering="False" HeaderText="Client Name" ItemStyle-Width="175px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="UserName" AllowFiltering="False" HeaderText="Client Email Address" ItemStyle-Width="175px" />
              <telerik:GridButtonColumn ShowFilterIcon="false" HeaderText="Client Password" ButtonType="LinkButton" Text="Reset&nbsp;Password" CommandName="ResetPassword" ShowInEditForm="False" ItemStyle-Width="150px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="Notes" AllowFiltering="False" HeaderText="Notes" />
              <telerik:GridDateTimeColumn ShowFilterIcon="False" DataField="Last Access" HeaderText="Last Upload" ItemStyle-Width="100px"
                UniqueName="LastSubmission" PickerType="DatePicker" DataFormatString="{0:MM/dd/yyyy HH:mm}" ReadOnly="True" />
              <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ItemStyle-Width="32px" EditText="Edit" HeaderText="Edit" />
              <telerik:GridButtonColumn ConfirmText="Delete this entry?" ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ButtonType="FontIconButton" HeaderText="Delete"
                CommandName="Delete" ItemStyle-Width="30px" />
            </Columns>
            <CommandItemSettings AddNewRecordText="Add New Client" ShowRefreshButton="False"></CommandItemSettings>
          </MasterTableView>
          <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
          </ClientSettings>
          <PagerStyle Mode="NextPrevAndNumeric" />
        </telerik:RadGrid>
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="TransmissionHistory">
        <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="Transactions" AllowPaging="true" Width="100%" OnItemCommand="Transactions_ItemCommand" OnItemCreated="Transactions_ItemCreated"
          PagerStyle-AlwaysVisible="True" AllowSorting="true" HorizontalAlign="Left" AutoGenerateColumns="False" OnNeedDataSource="Transactions_OnNeedDataSource"
          CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" MasterTableView-GridLines="Horizontal">
          <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None" ClientDataKeyNames="Id" CommandItemDisplay="None">
            <Columns>
              <telerik:GridDateTimeColumn ShowFilterIcon="False" DataField="SubmitDate" AllowFiltering="False" HeaderText="Submission Date" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="SubjectLine" AllowFiltering="False" HeaderText="Message" ItemStyle-Width="175px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="FromUser" AllowFiltering="False" HeaderText="Submitted By" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="ToUser" AllowFiltering="False" HeaderText="Sent To" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="Attachments" AllowFiltering="False" HeaderText="Attachment(s)" />
              <telerik:GridButtonColumn ButtonType="PushButton" CommandArgument="Id" Text="Download" FilterControlAltText="Filter column column" HeaderText="View" UniqueName="viewDocs" />
            </Columns>
          </MasterTableView>
          <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
          </ClientSettings>
          <PagerStyle Mode="NextPrevAndNumeric" />
        </telerik:RadGrid>
        <telerik:RadWindow RenderMode="Lightweight" ID="modalPopup" runat="server" Width="600px" Height="250px" CenterIfModal="true" AutoSize="false" Title="Attachments"
          Modal="true" OffsetElementID="main" Skin="Outlook" Style="z-index: 100001;" VisibleOnPageLoad="false" VisibleStatusbar="false" Behaviors="Close">
          <ContentTemplate>
            <div class="listViewItemPopup">
              <p>
                <telerik:RadLabel runat="server" ID="DocumentSubjectLine" /><br />
                <asp:Literal runat="server" ID="DocumentLink" />
              </p>
            </div>
          </ContentTemplate>
        </telerik:RadWindow>
        <script type="text/javascript">
          function GetRadWindow() {
            var oWindow = null;
            if (window.radWindow) oWindow = window.radWindow;
            else if (window.frameElement.radWindow) oWindow = window.frameElement.radWindow;
            return oWindow;
          }

          function Close() {
            GetRadWindow().close();
          }
        </script>
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="EmailRecipients">
        <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="EmailContacts" AllowPaging="true" Width="100%"
          PagerStyle-AlwaysVisible="True" AllowSorting="true" HorizontalAlign="Left" AutoGenerateColumns="False" OnNeedDataSource="EmailContacts_OnNeedDataSource"
          CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" MasterTableView-GridLines="Horizontal"
          OnUpdateCommand="EmailContacts_OnUpdateCommand" OnInsertCommand="EmailContacts_OnInsertCommand" OnDeleteCommand="EmailContacts_OnDeleteCommand">
          <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None"
            ClientDataKeyNames="Id" CommandItemDisplay="Bottom" InsertItemPageIndexAction="ShowItemOnFirstPage">
            <Columns>
              <telerik:GridBoundColumn ShowFilterIcon="False" DataField="UserName" AllowFiltering="False" HeaderText="Recipient Name" ItemStyle-Width="175px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="UserEmail" AllowFiltering="False" HeaderText="Recipient Email Address" ItemStyle-Width="175px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="Notes" AllowFiltering="False" HeaderText="Notes" />
              <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ItemStyle-Width="32px" EditText="Edit" HeaderText="Edit" />
              <telerik:GridButtonColumn ConfirmText="Delete this entry?" ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ButtonType="FontIconButton" CommandName="Delete" ItemStyle-Width="30px" HeaderText="Delete" />
            </Columns>
            <CommandItemSettings AddNewRecordText="Add New Recipient" ShowRefreshButton="False"></CommandItemSettings>
          </MasterTableView>
          <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
          </ClientSettings>
          <PagerStyle Mode="NextPrevAndNumeric" />
        </telerik:RadGrid>
        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="GetContactList" TypeName="SDTPortal.SqlDatasets" />
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="AdminAccounts">
        <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="AdminAccountList" AllowPaging="true" Width="100%"
          PagerStyle-AlwaysVisible="False" AllowSorting="true" HorizontalAlign="Left" AutoGenerateColumns="False" OnNeedDataSource="AdminAccountList_OnNeedDataSource"
          CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" MasterTableView-GridLines="Horizontal"
          OnUpdateCommand="ClientAccountsList_OnUpdateCommand2" OnInsertCommand="ClientAccountsList_OnInsertCommand2" OnItemCommand="ClientAccountsList_OnItemCommand"
          OnDeleteCommand="Account_OnDeleteCommand">
          <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None"
            ClientDataKeyNames="Id" CommandItemDisplay="Bottom" InsertItemPageIndexAction="ShowItemOnFirstPage">
            <Columns>
              <telerik:GridBoundColumn ShowFilterIcon="False" DataField="DisplayName" AllowFiltering="False" HeaderText="Admin Name" ItemStyle-Width="175px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="UserName" AllowFiltering="False" HeaderText="Admin Email Address" ItemStyle-Width="175px" />
              <telerik:GridButtonColumn ShowFilterIcon="false" HeaderText="Password" ButtonType="LinkButton" Text="Reset&nbsp;Password" CommandName="ResetPassword" ShowInEditForm="False" ItemStyle-Width="80px" />
              <telerik:GridBoundColumn ShowFilterIcon="false" DataField="Notes" AllowFiltering="False" HeaderText="Notes" />
              <telerik:GridCheckBoxColumn ShowFilterIcon="false" DataField="SuperAdmin" AllowFiltering="False" HeaderText="Super" />
              <telerik:GridEditCommandColumn UniqueName="EditCommandColumn" ItemStyle-Width="32px" EditText="Edit" HeaderText="Edit" />
              <telerik:GridButtonColumn ConfirmText="Delete this entry?" ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ButtonType="FontIconButton" CommandName="Delete" ItemStyle-Width="30px" HeaderText="Delete" />
            </Columns>
            <CommandItemSettings AddNewRecordText="Add New Administrator" ShowRefreshButton="False"></CommandItemSettings>
          </MasterTableView>
          <ClientSettings EnableRowHoverStyle="true">
            <Selecting AllowRowSelect="True" />
          </ClientSettings>
          <PagerStyle Mode="NextPrevAndNumeric" />
        </telerik:RadGrid>
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="SiteDetails">
        <br />
        <telerik:RadPageLayout runat="server" ID="SiteDetailsTab" OnPreRender="SiteDetailsTab_OnPreRender">
          <Rows>
            <telerik:LayoutRow>
              <Columns>
                <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
                <telerik:LayoutColumn Span="8" SpanMd="8" SpanSm="12" HiddenXs="true">
                  <telerik:RadPageLayout runat="server" ID="UploadControl">
                    <Rows>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="SystemName" runat="server" Width="90%" MaxLength="256" Label="System Name" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="UploadMessage" TextMode="MultiLine" Rows="5" runat="server" Width="90%" MaxLength="2000" Label="Upload Page" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="TransactionMessage" TextMode="MultiLine" Rows="5" runat="server" Width="90%" MaxLength="2000" Label="History Page" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="ThankyouMessage" TextMode="MultiLine" Rows="5" runat="server" Width="90%" MaxLength="2000" Label="Thank You Page" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="SuperAdminMessage" TextMode="MultiLine" Rows="5" runat="server" Width="90%" MaxLength="2000" Label="Super Admin" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <telerik:RadTextBox ID="AdminMessage" TextMode="MultiLine" Rows="5" runat="server" Width="90%" MaxLength="2000" Label="Admin Page" CssClass="MyEnabledTextBox2" LabelCssClass="MyLabel">
                              <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                              <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                            </telerik:RadTextBox>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                      <telerik:LayoutRow CssClass="content">
                        <Columns>
                          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                            <div style="width: 100%; padding-left: 150px;">
                              <telerik:RadButton ID="RadButton1" runat="server" Skin="Silk" RenderMode="Auto" Text="Save Changes" OnClick="RadButton1_OnClick" CssClass="css3Simple" />
                            </div>
                          </telerik:LayoutColumn>
                        </Columns>
                      </telerik:LayoutRow>
                    </Rows>
                  </telerik:RadPageLayout>
                </telerik:LayoutColumn>
                <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
              </Columns>
            </telerik:LayoutRow>
          </Rows>
        </telerik:RadPageLayout>
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="EmailServerDetails">
        <br />
        <telerik:RadPageLayout runat="server" ID="EmailServerTab" OnPreRender="EmailServerTab_OnPreRender">
          <Rows>
            <telerik:LayoutRow>
              <Columns>
                <telerik:LayoutColumn Span="3" SpanMd="3" SpanSm="12" HiddenXs="true" />
                <telerik:LayoutColumn Span="6" SpanMd="6" SpanSm="12">
                  <div style="width: 600px;">
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="ServerHost" runat="server" Width="90%" MaxLength="256" Label="Server Host" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadNumericTextBox ID="ServerPort" runat="server" Width="90%" MaxLength="256" MinValue="0" ShowSpinButtons="False" NumberFormat-DecimalDigits="0" Label="Server Port" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadNumericTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px; margin-left: 150px;">
                      <telerik:RadCheckBox ID="RequireAuth" runat="server" AutoPostBack="False" Text="Require Authentication" />
                      &nbsp;&nbsp;<telerik:RadCheckBox ID="RequireSSL" runat="server" AutoPostBack="False" Text="Require TLS/SSL" />
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="SmtpUser" runat="server" Width="90%" MaxLength="256" Label="SMTP Username" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="SmtpPassword" TextMode="Password" runat="server" Width="90%" MaxLength="256" Label="SMTP Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="smtpPasswordConfirm" TextMode="Password" runat="server" Width="90%" MaxLength="256" Label="Confirm" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="FromEmail" runat="server" Width="90%" Label="From Email" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="FromUsername" runat="server" Width="90%" Label="From User" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px; margin-left: 150px;">
                      <telerik:RadButton ID="RadButton2" runat="server" Skin="Silk" RenderMode="Auto" Text="Save" OnClick="RadButton2_OnClick" CssClass="css3Simple" />
                    </div>
                  </div>
                </telerik:LayoutColumn>
                <telerik:LayoutColumn Span="3" SpanMd="3" SpanSm="12" HiddenXs="true" />
              </Columns>
            </telerik:LayoutRow>
          </Rows>
        </telerik:RadPageLayout>
      </telerik:RadPageView>
      <telerik:RadPageView runat="server" ID="AccountSettings">
        <br />
        <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
          <Rows>
            <telerik:LayoutRow>
              <Columns>
                <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
                <telerik:LayoutColumn Span="4" SpanMd="6" SpanSm="12">
                  <div style="width: 500px;">
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="LoginId" runat="server" Width="100%" Label="Email Address" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" Enabled="False" />
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="DisplayName" runat="server" Width="100%" Label="Display Name" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2" Enabled="False" />
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="NewPassword" TextMode="Password" runat="server" Width="100%" Label="New Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px;">
                      <telerik:RadTextBox ID="ConfirmPassword" TextMode="Password" runat="server" Width="100%" Label="Confirm Password" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel2">
                        <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                        <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                      </telerik:RadTextBox>
                    </div>
                    <div style="width: 100%; padding: 5px; margin-left: 170px;">
                      <telerik:RadButton ID="RadButton4" runat="server" Skin="Silk" RenderMode="Auto" Text="Save" OnClick="RadButton3_OnClick" CssClass="css3Simple" />
                    </div>
                  </div>
                </telerik:LayoutColumn>
                <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
              </Columns>
            </telerik:LayoutRow>
          </Rows>
        </telerik:RadPageLayout>
      </telerik:RadPageView>
    </telerik:RadMultiPage>
  </telerik:RadAjaxPanel>
</asp:Content>

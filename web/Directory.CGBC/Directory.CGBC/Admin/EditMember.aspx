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
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" BackgroundPosition="Center">
    <img alt="Loading..." src='<%= RadAjaxLoadingPanel.GetWebResourceUrl(Page, "Telerik.Web.UI.Skins.Default.Ajax.loading.gif") %>' style="border: 0;" />
  </telerik:RadAjaxLoadingPanel>
  <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
    <telerik:RadPageLayout runat="server" ID="PageContentArea">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="1" />
            <telerik:LayoutColumn Span="10">
              <div style="width: 60%; float: left; text-align: center;">
                <telerik:RadLabel ID="EditMemberLabel" runat="server" Text="Edit Member" CssClass="headertitle" />
              </div>
              <div style="width: 38%; float: right; text-align: right;">
                <telerik:RadButton ID="NewMember" runat="server" Primary="true" Skin="Silk" RenderMode="Auto" Text="New" OnClick="NewMember_Click" />
                &nbsp;&nbsp;<telerik:RadButton ID="UpdateMember" runat="server" Primary="true" Skin="Silk" RenderMode="Auto" Text="Save" OnClick="UpdateMember_Click" />
                &nbsp;&nbsp;<telerik:RadButton ID="CancelUpdate" runat="server" Skin="Silk" RenderMode="Auto" Text="Cancel" OnClick="rbdirectory_Click" />
              </div>
              <telerik:RadPageLayout ID="DisplayMemberDetails" runat="server" CssClass="apptext">
                <Rows>
                  <telerik:LayoutRow>
                    <Columns>
                      <telerik:LayoutColumn Span="8">
                        <div style="width: 100%;">&nbsp;</div>
                        <telerik:RadLabel ID="RadLabel2" runat="server" Text="Name:<font style='font-size: .8em;'><br />(F-M-L-S)</font>" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadDropDownList ID="rddSalutation" runat="server" Width="80px" Skin="Silk" AutoPostBack="true" />
                        <telerik:RadTextBox ID="tMemberFirstName" runat="server" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                        <telerik:RadTextBox ID="tMemberMiddleName" runat="server" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                        <telerik:RadTextBox ID="tMemberLastName" runat="server" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                        <telerik:RadTextBox ID="tMemberSuffix" runat="server" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                      </telerik:LayoutColumn>
                      <telerik:LayoutColumn Span="4">
                        <div style="width: 100%;">&nbsp;</div>
                        <telerik:RadLabel ID="RadLabel3" runat="server" Text="Status:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadDropDownList ID="rddMaritalStatus" runat="server" Width="200px" Skin="Silk" AutoPostBack="true" />
                      </telerik:LayoutColumn>
                    </Columns>
                  </telerik:LayoutRow>
                  <telerik:LayoutRow>
                    <Columns>
                      <telerik:LayoutColumn Span="8">
                        <telerik:RadLabel ID="RadLabel4" runat="server" Text="Address:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadTextBox ID="tMemberAddress1" runat="server" Width="450px" Skin="Silk" CssClass="MyEnabledTextBox2" /><br />
                        &nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="RadLabel12" runat="server" Text="" Font-Bold="true" CssClass="labelText labels" /><telerik:RadTextBox ID="tMemberAddress2" runat="server" Skin="Silk" Width="450px" CssClass="MyEnabledTextBox2" /><br />
                        &nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="RadLabel13" runat="server" Text="" Font-Bold="true" CssClass="labelText labels" /><telerik:RadTextBox ID="tMemberCity" runat="server" EmptyMessage="City" Width="200px" Skin="Silk" CssClass="MyEnabledTextBox2" /><telerik:RadDropDownList ID="rddStates" runat="server" Skin="Silk" Width="180px" />
                        &nbsp;<telerik:RadTextBox ID="tMemberZip" runat="server" EmptyMessage="Zip" Width="80px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                      </telerik:LayoutColumn>
                      <telerik:LayoutColumn Span="4">
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
                      <telerik:LayoutColumn Span="8">
                        <telerik:RadLabel ID="RadLabel6" runat="server" Text="Phone:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadMaskedTextBox Mask="(###) ###-####" ID="tMemberCell" runat="server" EmptyMessage="Cellular" Width="150px" Skin="Silk" CssClass="MyEnabledTextBox2" />&nbsp;<telerik:RadLabel ID="RadLabel14" runat="server" Text="Cell" Font-Bold="true" CssClass="labels" />&nbsp;&nbsp;&nbsp;<telerik:RadMaskedTextBox Mask="(###) ###-####" ID="tMemberHome" runat="server" EmptyMessage="Home" Width="150px" Skin="Silk" CssClass="MyEnabledTextBox2" />&nbsp;<telerik:RadLabel ID="RadLabel15" runat="server" Text="Home" Font-Bold="true" CssClass="labels" /><br />
                        <telerik:RadLabel ID="RadLabel9" runat="server" Text="Email:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadTextBox ID="tMemberEmail1" runat="server" EmptyMessage="Email Address 1" Width="325px" Skin="Silk" CssClass="MyEnabledTextBox2" />&nbsp;<telerik:RadTextBox ID="tMemberEmail2" runat="server" EmptyMessage="Email Address 2" Width="325px" Skin="Silk" CssClass="MyEnabledTextBox2" />
                      </telerik:LayoutColumn>
                      <telerik:LayoutColumn Span="4">
                        <telerik:RadLabel ID="RadLabel11" runat="server" Text="Last Update:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadLabel ID="tMemberLastUpdate" runat="server" Text="tMemberUpdate" Font-Bold="true" CssClass="labels" />
                      </telerik:LayoutColumn>
                    </Columns>
                  </telerik:LayoutRow>
                  <telerik:LayoutRow>
                    <Columns>
                      <telerik:LayoutColumn Span="6">
                        <telerik:RadLabel ID="RadLabel7" runat="server" Text="Notes:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadTextBox ID="tMemberNotes" runat="server" TextMode="MultiLine" Rows="3" Width="600px" Skin="Silk" CssClass="MyEnabledTextBox2" EmptyMessage="Member Notes" /><br />
                        &nbsp;&nbsp;&nbsp;<telerik:RadLabel ID="tMemberHistoricalNotes" runat="server" CssClass="labels" />
                        <div style="width: 100%;">&nbsp;</div>
                      </telerik:LayoutColumn>
                      <telerik:LayoutColumn Span="6">
                        <telerik:RadLabel ID="RadLabel5" runat="server" Text="Related:" Font-Bold="true" CssClass="labelText labels" />
                        &nbsp;&nbsp;<telerik:RadGrid ID="MemberRelations" RenderMode="Auto" runat="server" AllowPaging="true" GridLines="None"
                          PagerStyle-AlwaysVisible="false" AllowSorting="false" Skin="Silk" OnNeedDataSource="MemberRelations_NeedDataSource"
                          OnInsertCommand="MemberRelations_InsertCommand" GroupingSettings-CaseSensitive="False" OnDeleteCommand="MemberRelations_DeleteCommand">
                          <MasterTableView AutoGenerateColumns="False" EditMode="EditForms" DataKeyNames="Id" GridLines="None"
                            ClientDataKeyNames="Id" CommandItemDisplay="Bottom" InsertItemPageIndexAction="ShowItemOnFirstPage">
                            <Columns>
                              <telerik:GridDropDownColumn ListTextField="Name" ListValueField="Id" DataField="Id" HeaderText="Relation" DataSourceID="ObjectDataSource2" DropDownControlType="DropDownList" />
                              <telerik:GridDropDownColumn UniqueName="RelationType" ListTextField="Name" ListValueField="Id" DataField="Relationship.Id" HeaderText="Type" DataSourceID="ObjectDataSource1" DropDownControlType="DropDownList" />
                              <telerik:GridButtonColumn ConfirmText="Delete this Relationship?" ConfirmDialogType="RadWindow" ConfirmTitle="Delete" ButtonType="FontIconButton" CommandName="Delete" ItemStyle-Width="40px" />
                            </Columns>
                            <EditFormSettings EditFormType="Template">
                              <FormTemplate>
                                <telerik:RadDropDownList ID="rdRelatedMember" runat="server" Skin="Silk" DataSourceID="ObjectDataSource2" DataTextField="Name" DataValueField="Id" Width="300px" />
                                <telerik:RadDropDownList ID="rdRelationType" runat="server" Skin="Silk" DataSourceID="ObjectDataSource1" DataTextField="Name" DataValueField="Id" />
                                &nbsp;&nbsp;<telerik:RadButton ID="btnUpdate" Text='<%# (Container is GridEditFormInsertItem) ? "Save" : "Update" %>' runat="server" CommandName='<%# (Container is GridEditFormInsertItem) ? "PerformInsert" : "Update" %>' Skin="Silk" />
                                <telerik:RadButton ID="btnCancel" Text="Cancel" runat="server" CausesValidation="False" CommandName="Cancel" Skin="Silk" />
                              </FormTemplate>
                            </EditFormSettings>
                          </MasterTableView>
                        </telerik:RadGrid>
                        <div style="width: 100%;">&nbsp;</div>
                        <asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="RelationshipTypes" TypeName="Directory.CGBC.Helpers.SqlDataLoader" />
                        <asp:ObjectDataSource ID="ObjectDataSource2" runat="server" SelectMethod="AllMemberRelations" TypeName="Directory.CGBC.Helpers.SqlDataLoader" />
                      </telerik:LayoutColumn>
                    </Columns>
                  </telerik:LayoutRow>
                </Rows>
              </telerik:RadPageLayout>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="1" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.Master" AutoEventWireup="true" CodeBehind="MainDirectory.aspx.cs" Inherits="Directory.CGBC.MainDirectory" %>

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
<asp:Content runat="server" ID="ActionButtonArea" ContentPlaceHolderID="ActionButtons">
  <div style="width: 100%; padding: 75px 5px 25px;">
    <telerik:RadLabel ID="CurrentUser" runat="server"></telerik:RadLabel>
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" OnClick="rbPassword_OnClick" />
  </div>
  <div style="width: 100%; padding: 5px;">
    <telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" />
  </div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadAjaxManager ID="RadAjaxManager1" runat="server">
    <AjaxSettings>
      <telerik:AjaxSetting AjaxControlID="RadButton1">
        <UpdatedControls>
          <telerik:AjaxUpdatedControl ControlID="UploadFilesView" LoadingPanelID="RadAjaxLoadingPanel1"></telerik:AjaxUpdatedControl>
        </UpdatedControls>
      </telerik:AjaxSetting>
    </AjaxSettings>
  </telerik:RadAjaxManager>
  <asp:Panel ID="UploadFilesView" runat="server">
    <div id="messageDisplayArea">
      <telerik:RadLabel ID="lErrorMessage" runat="server" CssClass="errorMessageDisplay" />
    </div>
    <telerik:RadPageLayout runat="server" ID="UploadFilesPage">
      <Rows>
        <telerik:LayoutRow CssClass="content">
          <Columns>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="8" SpanMd="8" SpanSm="12">
              <%--<uc1:usernav runat="server" ID="UserNav" />--%>
              <div style="width: 900px;">
                <div style="width: 100%; padding: 5px; padding-right: 40px;">
                  <telerik:RadTextBox ID="SubmitUserName" Width="60%" runat="server" Label="Sender" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel4" DisabledStyle-Font-Bold="True" Enabled="False" />
                </div>
                <div style="width: 100%; padding: 5px;">
                 <%-- <telerik:RadComboBox ID="SendToUser" runat="server" CausesValidation="true" RenderMode="Auto" DataSourceID="ObjectDataSource1" DataValueField="Id" DataTextField="UserName"
                    Label="Send To" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel3" ZIndex="10000" Width="70%" OnDataBound="SendToUser_OnDataBound" />--%>
                </div>
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadTextBox ID="SubjectLine" runat="server" Width="90%" MaxLength="256" Label="Message (Optional)" CssClass="MyEnabledTextBox" LabelCssClass="MyLabel4">
                    <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
                    <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
                  </telerik:RadTextBox>
                </div>
                <div style="width: 100%; padding: 5px;">
                  <telerik:RadLabel runat="server" Text="Attach File(s)" CssClass="MyLabel4" Style="width: 200px; display: inline-block; padding-top: 5px;" />
                  <telerik:RadAsyncUpload ID="FilesToSend" runat="server" Skin="Silk" ChunkSize="1048576" MultipleFileSelection="Automatic" UploadedFilesRendering="BelowFileInput"
                    HideFileInput="True" MaxFileInputsCount="3" Width="690px" RenderMode="Auto" Localization-Select="Browse" Style="float: right;"
                    AllowedFileExtensions=".doc,.docx,.rtf,.txt,.xls,.xlsx,.pdf,.zip,.bmp,.jpg,.jpeg,.tif,.tiff,.png,.gif,.ppt,.pptx">
                    <FileFilters>
                      <telerik:FileFilter Description="Document Files (.doc;.docx;.rtf;.txt)" Extensions="doc,docx,rtf,txt" />
                      <telerik:FileFilter Description="Excel Files (.xls;.xlsx)" Extensions="xls,xlsx" />
                      <telerik:FileFilter Description="PDF (.pdf)" Extensions="pdf" />
                      <telerik:FileFilter Description="Images (.bmp;.jpg;.tif;.tiff;.png)" Extensions="bmp,jpg,jpeg,tif,tiff,png,gif" />
                      <telerik:FileFilter Description="PowerPoint (.ppt;.pptx)" Extensions="ppt,pptx" />
                      <telerik:FileFilter Description="Archive (.zip)" Extensions="zip" />
                    </FileFilters>
                  </telerik:RadAsyncUpload>
                  <telerik:RadProgressArea runat="server" ID="RadProgressArea1" Width="100%" />
                </div>
                <div style="width: 100%; padding: 5px; margin-left: 610px; margin-top: 40px;">
                  <telerik:RadButton ID="RadButton1" CausesValidation="true" runat="server" Skin="Silk" RenderMode="Auto" Text="Send Files" OnClick="RadButton1_OnClick" CssClass="css3Simple" />
                </div>
              </div>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
    <%--<asp:ObjectDataSource ID="ObjectDataSource1" runat="server" SelectMethod="GetSystemRecipients" TypeName="Directory.CGBC.SqlDatasets"></asp:ObjectDataSource>--%>
  </asp:Panel>
  <telerik:RadAjaxLoadingPanel ID="RadAjaxLoadingPanel1" runat="server" Transparency="10">
    <div class="loading">
      <span class="appErrorMessage">Your data is being encrypted and transmitted. The time required for this to be accomplished will depend on the size of the file(s) being uploaded and the speed of your Internet connection. For exceptionally LARGE files or slow Internet connections, the process may take several minutes. Please be patient.<br />
        <br />
        DO NOT CLOSE YOUR BROWSER OR NAVIGATE AWAY FROM THIS PAGE until the transmission is completed.</span>
      <br />
      <br />
      <asp:Image ID="Image1" runat="server" ImageUrl="images/loading.gif" AlternateText="loading" Height="20px" Width="100%"></asp:Image>
    </div>
  </telerik:RadAjaxLoadingPanel>
</asp:Content>

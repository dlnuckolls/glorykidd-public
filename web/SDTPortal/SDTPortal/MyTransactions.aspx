<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="MyTransactions.aspx.cs" Inherits="SDTPortal.MyTransactions" %>
<%@ Register TagPrefix="uc1" TagName="usernav" Src="~/UserNav.ascx" %>

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
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbPassword" runat="server" Skin="Silk" RenderMode="Auto" Text="Change Password" CssClass="css3SimpleAction" OnClick="rbPassword_OnClick" /></div>
  <div style="width: 100%; padding: 5px;"><telerik:RadButton ID="rbLogout" runat="server" Skin="Silk" RenderMode="Auto" Text="Logout" OnClick="rbLogout_OnClick" CssClass="css3SimpleAction" /></div>
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <telerik:RadPageLayout runat="server" ID="TransactionHistory">
    <Rows>
      <telerik:LayoutRow CssClass="content">
        <Columns>
          <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
          <telerik:LayoutColumn Span="8" SpanMd="8" SpanSm="12" HiddenXs="true" >
            <uc1:usernav runat="server" ID="UserNav" />
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="2" SpanMd="2" SpanSm="12" HiddenXs="true" />
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow CssClass="content">
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12" HiddenXs="true">
            <telerik:RadGrid Skin="Silk" RenderMode="Auto" runat="server" ID="Transactions" AllowPaging="true" Width="100%"
              PagerStyle-AlwaysVisible="True" AllowSorting="true" HorizontalAlign="Left" AutoGenerateColumns="False" OnNeedDataSource="Transactions_OnNeedDataSource"
              CellPadding="0" BorderWidth="0px" BorderStyle="None" MasterTableView-CellPadding="0" MasterTableView-CellSpacing="0" MasterTableView-GridLines="Horizontal">
              <MasterTableView AutoGenerateColumns="False" EditMode="InPlace" DataKeyNames="Id" GridLines="None" ClientDataKeyNames="Id" CommandItemDisplay="None">
                <Columns>
                  <telerik:GridDateTimeColumn ShowFilterIcon="False" DataField="SubmitDate" AllowFiltering="False" HeaderText="Submission Date" DataFormatString="{0:MM/dd/yyyy HH:mm}" />
                  <telerik:GridBoundColumn ShowFilterIcon="false" DataField="SubjectLine" AllowFiltering="False" HeaderText="Message" ItemStyle-Width="175px" />
                  <telerik:GridBoundColumn ShowFilterIcon="false" DataField="ToUser" AllowFiltering="False" HeaderText="Sent To" />
                  <telerik:GridBoundColumn ShowFilterIcon="false" DataField="Attachments" AllowFiltering="False" HeaderText="Attachment(s)" />
                </Columns>
              </MasterTableView>
              <ClientSettings EnableRowHoverStyle="true">
                <Selecting AllowRowSelect="True" />
              </ClientSettings>
              <PagerStyle Mode="NextPrevAndNumeric" />
            </telerik:RadGrid>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>

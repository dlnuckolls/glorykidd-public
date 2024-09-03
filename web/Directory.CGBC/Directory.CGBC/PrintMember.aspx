<%@ Page Title="" Language="C#" MasterPageFile="~/PageMaster.Master" AutoEventWireup="true" CodeBehind="PrintMember.aspx.cs" Inherits="Directory.CGBC.PrintMember" %>

<asp:Content ID="Content1" ContentPlaceHolderID="PageTitle" runat="server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="head" runat="server">
  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.js"></script>
  <script type="text/javascript" src="https://cdnjs.cloudflare.com/ajax/libs/pdf.js/2.2.2/pdf.worker.js"></script>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="HeaderArea" runat="server">
  <telerik:RadClientExportManager runat="server" ID="RadClientExportManager1" OnClientPdfExporting="OnClientPdfExporting" />
</asp:Content>
<asp:Content ID="Content4" ContentPlaceHolderID="MainContentArea" runat="server">
  <div style="width: 100%; text-align: center;">
    <telerik:RadButton RenderMode="Lightweight" ID="bPrint" runat="server" Skin="Silk" Text="Export Card" Primary="true" OnClientClicked="exportPDF" UseSubmitBehavior="false" AutoPostBack="false">
      <Icon PrimaryIconCssClass="rbPrint"></Icon>
    </telerik:RadButton>
  </div>
  <telerik:RadPageLayout ID="DisplayMemberDetails" runat="server" CssClass="appPrinttext">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1">
            <telerik:RadAvatar runat="server" ID="RadAvatar1" Type="Text" Text="CM" ThemeColor="Primary" Size="Large" Skin="Silk" Rounded="Large" />
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6">
            <telerik:RadLabel ID="RadLabel2" runat="server" Text="Name:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberName" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="5">
            <telerik:RadLabel ID="RadLabel3" runat="server" Text="Status:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberStatus" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" />
          <telerik:LayoutColumn Span="6">
            <telerik:RadLabel ID="RadLabel4" runat="server" Text="Address:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberAddress" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="5">
            <telerik:RadLabel ID="RadLabel8" runat="server" Text="BirthDate:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberDob" runat="server" CssClass=" labels" /><br />
            <telerik:RadLabel ID="RadLabel10" runat="server" Text="Marriage:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberMarriage" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" />
          <telerik:LayoutColumn Span="6">
            <telerik:RadLabel ID="RadLabel6" runat="server" Text="Phone:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberPhone" runat="server" CssClass=" labels" /><br />
            <telerik:RadLabel ID="RadLabel9" runat="server" Text="Email:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberEmails" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="5">
            <telerik:RadLabel ID="RadLabel5" runat="server" Text="Related:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberRelation" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" />
          <telerik:LayoutColumn Span="11">
            <telerik:RadLabel ID="RadLabel7" runat="server" Text="Notes:" Font-Bold="true" CssClass="labelText labels" />
            &nbsp;&nbsp;<telerik:RadLabel ID="MemberNotes" runat="server" CssClass=" labels" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
  <telerik:RadPdfViewer runat="server" ID="RadPdfViewer1" Height="350px" Width="825px" Scale="0.8" />
  <script>
    var $ = $telerik.$;
    function exportPDF() {
      $find('<%=RadClientExportManager1.ClientID%>').exportPDF($(".appPrinttext"));
    }
    function OnClientPdfExporting(sender, args) {
      var data = args.get_dataURI().split(',')[1];
      setData(data);
      args.set_cancel(true);
    }
    function setData(data) {
      var RadPdfViewerObject = $find("<%=RadPdfViewer1.ClientID %>");
      RadPdfViewerObject.get_kendoWidget().fromFile({ data: data });
      return false;
    }
  </script>
</asp:Content>

<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="AISWeb.Default" %>

<asp:Content ID="Content0" ContentPlaceHolderID="head" runat="Server">
  <asp:Literal ID="TitleTag" runat="server"></asp:Literal>
</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="MainContentArea" runat="Server">
  <telerik:RadAjaxPanel runat="server">
    <telerik:RadPageLayout runat="server" ID="loginPage">
      <Rows>
        <telerik:LayoutRow>
          <Columns>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
            <telerik:LayoutColumn Span="4" SpanMd="6" SpanSm="12">
              <div style="width: 500px;">
                <div style="width: 100%; padding: 5px; "><telerik:RadButton ID="AisRetirement" runat="server" Skin="Silk" RenderMode="Auto" Text="AIS Retirement (www.aisretirement.com)" OnClick="AisRetirement_OnClick" CssClass="css3SimpleAction" /></div>
                <div style="width: 100%; padding: 5px; "><telerik:RadButton ID="SdpApplication" runat="server" Skin="Silk" RenderMode="Auto" Text="Secure Document Portal" OnClick="SdpApplication_OnClick" CssClass="css3SimpleAction" /></div>
              </div>
            </telerik:LayoutColumn>
            <telerik:LayoutColumn Span="4" SpanMd="3" SpanSm="12" HiddenXs="true" />
          </Columns>
        </telerik:LayoutRow>
      </Rows>
    </telerik:RadPageLayout>
  </telerik:RadAjaxPanel>
</asp:Content>

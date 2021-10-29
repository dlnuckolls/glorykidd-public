<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ClientEmail.aspx.cs" Inherits="ElusiveSoftware.net.pages.ClientEmail" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
  <meta name="viewport" content="initial-scale=1.0, minimum-scale=1, maximum-scale=1.0, user-scalable=no" />
  <link href="/styles/base.css" rel="stylesheet" />
  <link href="/styles/default.css" rel="stylesheet" />
  <!-- Global site tag (gtag.js) - Google Analytics -->
  <script async src="https://www.googletagmanager.com/gtag/js?id=UA-18831849-20"></script>
  <script>
    window.dataLayer = window.dataLayer || [];
    function gtag() { dataLayer.push(arguments); }
    gtag('js', new Date());
    gtag('config', 'UA-18831849-20');
  </script>
  <title>Client Emails</title>
</head>
<body>
  <form id="form1" runat="server">
    <telerik:RadScriptManager runat="server" />
    <telerik:RadAjaxPanel ID="RadAjaxPanel1" runat="server">
      <asp:Panel ID="SendEmails" runat="server">
        <div class="article">
          <h2>Email Us Here!</h2>
          <telerik:RadTextBox ID="ContactName" runat="server" Width="100%" MaxLength="256" Label="Your Name" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="ContactNameValidator" runat="server" ControlToValidate="ContactName" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your name" Display="Dynamic" ForeColor="Red" />
          <telerik:RadTextBox ID="ContactEmail" runat="server" Width="100%" MaxLength="256" Label="Your Email" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="ContactEmailValidator" runat="server" ControlToValidate="ContactEmail" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your email" Display="Dynamic" ForeColor="Red" />
          <telerik:RadTextBox ID="ContactSubject" runat="server" Width="100%" MaxLength="256" Label="Subject" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <telerik:RadTextBox ID="ContactMessage" runat="server" Width="100%" Label="Message" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel" TextMode="MultiLine" Rows="8" Resize="None">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <div style="padding-left: 100px; padding-top: 10px; padding-bottom: 10px;">
            <telerik:RadCaptcha ID="RadCaptcha1" runat="server" ErrorMessage="The code you entered is not valid." ForeColor="Red"
              ValidationGroup="Group" EnableRefreshImage="true" CaptchaTextBoxCssClass="MyCaptchaTextBox" CaptchaTextBoxLabel="Please enter code">
              <CaptchaImage ImageCssClass="imageClass" />
            </telerik:RadCaptcha>
            <telerik:RadButton ID="SendMessage" runat="server" Skin="Silk" RenderMode="Auto" Text="Send Message" ValidationGroup="Group" OnClick="SendMessage_Click" CssClass="css3Simple" />
          </div>
        </div>
      </asp:Panel>
      <asp:Panel ID="Varitech" runat="server">
        <div class="article">
          <h2>Email Us Here!</h2>
          <telerik:RadTextBox ID="vName" runat="server" Width="100%" MaxLength="256" Label="Name" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator1" runat="server" ControlToValidate="vName" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your name" Display="Dynamic" ForeColor="Red" />
          <telerik:RadTextBox ID="vPhone" runat="server" Width="100%" MaxLength="15" Label="Phone" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator2" runat="server" ControlToValidate="vPhone" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your phone number" Display="Dynamic" ForeColor="Red" />
          <telerik:RadTextBox ID="vEmail" runat="server" Width="100%" MaxLength="256" Label="Email" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator3" runat="server" ControlToValidate="vEmail" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your email address" Display="Dynamic" ForeColor="Red" />
          <telerik:RadTextBox ID="vAddress" runat="server" Width="100%" MaxLength="256" Label="Address" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <asp:RequiredFieldValidator ID="RequiredFieldValidator4" runat="server" ControlToValidate="vAddress" EnableClientScript="false" ValidationGroup="Group"
            ErrorMessage="Please fill in your address" Display="Dynamic" ForeColor="Red" />
          <div style="padding-left: 100px; padding-top: 10px; padding-bottom: 10px;">
            <h3>Areas of Interest</h3>
            <telerik:RadCheckBoxList ID="vCBList" runat="server" AutoPostBack="false" Columns="3" >
              <Items>
                <telerik:ButtonListItem Text="Generator Systems" Value="YES" Selected="false" />
                <telerik:ButtonListItem Text="CNG / Compressed Natural Gas" Value="YES" Selected="false" />
                <telerik:ButtonListItem Text="Other" Value="YES" Selected="false" />
              </Items>
            </telerik:RadCheckBoxList>
          </div>
          <telerik:RadTextBox ID="vMessage" runat="server" Width="100%" Label="Message" CssClass="MyEnabledTextBox" LabelCssClass="MyContactLabel" TextMode="MultiLine" Rows="8" Resize="None">
            <HoveredStyle CssClass="MyHoveredTextBox"></HoveredStyle>
            <FocusedStyle CssClass="MyFocusedTextBox"></FocusedStyle>
          </telerik:RadTextBox>
          <div style="padding-left: 100px; padding-top: 10px; padding-bottom: 10px;">
            <telerik:RadCaptcha ID="RadCaptcha2" runat="server" ErrorMessage="The code you entered is not valid." ForeColor="Red"
              ValidationGroup="Group" EnableRefreshImage="true" CaptchaTextBoxCssClass="MyCaptchaTextBox" CaptchaTextBoxLabel="Please enter code">
              <CaptchaImage ImageCssClass="imageClass" />
            </telerik:RadCaptcha>
            <telerik:RadButton ID="RadButton1" runat="server" Skin="Silk" RenderMode="Auto" Text="Send Message" ValidationGroup="Group" OnClick="SendMessage_Click" CommandArgument="vteng" CssClass="css3Simple" />
          </div>
        </div>
      </asp:Panel>
      <asp:Panel ID="ThanksMessage" runat="server">
        <div class="homearticle">
          <h2>Thank you for your email!</h2>
        </div>
      </asp:Panel>
    </telerik:RadAjaxPanel>
  </form>
</body>
</html>

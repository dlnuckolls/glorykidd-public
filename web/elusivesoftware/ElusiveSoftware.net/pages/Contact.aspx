<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="ElusiveSoftware.net.pages.Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <asp:Literal ID="TitleTag" runat="server" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <h2>GloryKidd Technologies LLC</h2>
              <p>
                Our mission is to use workflow processes and leverage software to create practical IT solutions with the 
                highest standard of excellence to both honor God and provide unparalleled customer service for our clients. 
                Let us put you on the digital map, with a complete online presence.<br />
                <br />
                Equally as important as building the right statement for your business, is the power to deliver that message 
                to the world. GloryKidd Technologies offers a full range of services tailored specifically to meet your 
                needs and budget! We cover all the features that our customers expect and need to promote their message 
                loud and clear. Hosting plans are tailored to give you the maximum value at a budget price.
              </p>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
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
              <asp:Panel ID="ThanksMessage" runat="server">
                <div class="homearticle">
                  <h2>Thank you for your email!</h2>
                </div>
              </asp:Panel>
            </telerik:RadAjaxPanel>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>

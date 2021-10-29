<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="OurPlans.aspx.cs" Inherits="ElusiveSoftware.net.pages.OurPlans" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <asp:Literal ID="TitleTag" runat="server" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" HiddenMd="true" />
          <telerik:LayoutColumn Span="10" SpanMd="12" SpanSm="12">
            <div class="pageHeader">
              <h2>Plans to fit every project and budget</h2>
              <h3>~ See which works best for you ~<br />
                <br />
              </h3>
            </div>
            <telerik:RadPageLayout runat="server" ID="RadPageLayout3">
              <Rows>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-sitemap" aria-hidden="true"></i>
                        </div>
                        <h4>Basic Plan</h4>
                        <p>
                          Gives you all the essential features you need for a "just getting started" website or an average-size blog, personal, or small business website.<br />
                          <br />
                          Works well for smaller online shops with a reasonable number of products.<br />
                          <br />
                          You may outgrow this plan if you start attracting more than 10,000 unique visits per month on a regular basis.<br />
                          <br />
                          Includes all the Standard Features!
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-black-tie" aria-hidden="true"></i>
                        </div>
                        <h4>Professional Plan</h4>
                        <p>
                          If you need more resources and premium features in addition to the essential features then we recommend The Professional Plan.<br />
                          <br />
                          This plan is designed to accommodate either a single more heavily-trafficked website or multiple average-size websites.<br />
                          <br />
                          Popular among customers who prefer to host all their sites in one account, or want to draw a crowd with extensive social media integration.<br />
                          <br />
                          Premium Features like Customer Accessible Backups and SEO Optimizations.<br />
                          <br />
                          Includes everything in the Basic Plan.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-industry" aria-hidden="true"></i>
                        </div>
                        <h4>Ultimate Plan</h4>
                        <p>
                          If you have a really heavily-visited or resource-intensive website then The Ultimate Plan is your best option.<br />
                          <br />
                          Offers a completely different and more geeky server infrastructure with more powerful machines.<br />
                          <br />
                          Great for average to large-sized e-commerce websites because it can accommodate a much larger product line.<br />
                          <br />
                          Includes everything from the Basic and Professional plans<br />
                          <br />
                          Ultimate features like Social Media Integrations, Google Ad-Words Campaigns, and Facebook Ad Campaigns!
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                  </Columns>
                </telerik:LayoutRow>
              </Rows>
            </telerik:RadPageLayout>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="1" HiddenMd="true" />
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
            <div class="homearticle2">
            <div>
              <telerik:RadLinkButton ID="RadButton1" runat="server" Skin="Silk" Text="Contact us today about the plan you need to succeed!" NavigateUrl="~/contact"></telerik:RadLinkButton>
            </div>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>

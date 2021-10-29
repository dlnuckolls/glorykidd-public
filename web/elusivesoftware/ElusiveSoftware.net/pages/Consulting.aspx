<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Consulting.aspx.cs" Inherits="ElusiveSoftware.net.pages.Consulting" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <asp:Literal ID="TitleTag" runat="server" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
            <div class="pageHeader">
              <h2>Raise Your Team</h2>
              <h3>~ Our Professional Consulting Services can help improve you productivity ~<br />
                <br />
              </h3>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <h2>Accutrol LLC</h2>
              <p>
                Accutrol, LLC, develops software for industrial automation. When we needed to improve our system architecture skills, we turned 
                to GloryKidd Technologies for our training needs. Mr. Dave Nuckolls provided training at a retreat during which he led 
                our team through the fundamentals of architecting, demonstrating principles using one of our major projects as a tutorial. 
                The process of architecting a system, as time consuming as it is, inevitably yields a design that saves much more development 
                effort than that spent on architecture. We were very pleased. Not only did we receive outstanding training, we also came away 
                with a complete and elegant system architecture.
                <br />
                <br />
                <br />
                <em><strong>~Hamilton Woods</strong>, Principal Engineer (Hunstville AL)</em>
              </p>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <h2>Consulting Services</h2>
              <p>
                IT teams face constant pressure to focus on strategic initiatives and increase productivity while keeping costs low. We get it. 
                That’s why our professional services team is there to help you get the most out of your IT solution. Our professional team is with you 
                every step of the way, from assessing your environment and long-term objectives to designing and implementing a custom solution complete 
                with hands-on training and ongoing support.
              </p>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" HiddenMd="true" />
          <telerik:LayoutColumn Span="10" SpanMd="12" SpanSm="12">
            <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
              <Rows>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
                      <div class="pageHeader">
                        <h3>~ Our Specialties ~<br />
                          <br />
                        </h3>
                      </div>
                    </telerik:LayoutColumn>
                  </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-shield" aria-hidden="true"></i>
                        </div>
                        <h4>Security Services</h4>
                        <p>
                          Security goes deeper than your hardware, software and applications. It touches every layer of your network infrastructure and requires 
                          a holistic defense strategy that aligns people, processes and policies. We prioritize security in everything that we do, but we also 
                          specialize in security assessments and overhauls to safeguard one of your most precious assets—your data.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-sitemap" aria-hidden="true"></i>
                        </div>
                        <h4>Technology and Workflow Optimizations</h4>
                        <p>
                          Our project managers and consultants work directly with you to design and implement every facet of your IT solution. After assessing 
                          your current environment and business objectives, they’ll produce a detailed project blueprint. Partnering with your existing staff, 
                          they'll oversee the full implementation of the solution. We help you maximize the return on your IT investments by supporting the full 
                          lifecycle of your important IT initiatives, from assessment and design to deployment and ongoing management. Our professional services 
                          team helps you assess your business objectives and identify opportunities for improvements; walks you through the design process by 
                          recommending relevant technologies and services; and oversees the full deployment of your solution.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                  </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-wifi" aria-hidden="true"></i>
                        </div>
                        <h4>Network Optimizations</h4>
                        <p>
                          There's more traffic on your network than ever before, and trends such as BYOD, video conferencing and virtualized computing are only 
                          getting bigger. Before you implement any new IT solution, our team will assess the impact on your network bandwidth and recommend 
                          upgrades as needed. From next generation network overhauls to WAN optimization, our priority is to keep your data moving.
                          <br />
                          <br />
                          Adding new branches? Planning a merger or acquisition? We'll help you optimize your network for the big changes.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-building" aria-hidden="true"></i>
                        </div>
                        <h4>Managed Infrastructure</h4>
                        <p>
                          For most businesses, technology is a means to an end. The more time your team spends managing your infrastructure, the less time 
                          they have to work towards business objectives. GloryKidd can relieve some of this burden by helping you manage your most complex 
                          infrastructure, freeing you to focus on growing your business and delighting your customers. Our IT experts have the knowledge 
                          and experience to deliver a holistic solution providing global connectivity, colocation and managed cloud services tailored to 
                          your needs and goals.
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
                <telerik:RadLinkButton ID="RadButton1" runat="server" Skin="Silk" Text="Contact us today for more details!" NavigateUrl="~/contact"></telerik:RadLinkButton>
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

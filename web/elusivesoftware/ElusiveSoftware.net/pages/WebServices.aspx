<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="WebServices.aspx.cs" Inherits="ElusiveSoftware.net.pages.WebServices" %>

<asp:Content ID="Content1" ContentPlaceHolderID="head" runat="server">
  <asp:Literal ID="TitleTag" runat="server" />
  <link href="https://cdnjs.cloudflare.com/ajax/libs/font-awesome/4.7.0/css/font-awesome.css" rel="stylesheet" />
</asp:Content>
<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
            <telerik:RadPageLayout runat="server" ID="RadPageLayout2">
              <Rows>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="1" HiddenMd="true" />
                    <telerik:LayoutColumn Span="10" SpanMd="12" SpanSm="12">
                      <div class="pageHeader">
                        <h2>More than <em>just</em> another website</h2>
                        <h3>~ See what our services give you ~<br />
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
                                    <i class="fa fa-globe" aria-hidden="true"></i>
                                  </div>
                                  <h4>Domain Name Registration</h4>
                                  <p>All website design services include one year of free domain name registration. We will help you choose and acquire the right domain name to attract customers, and/or assist you with transferring your existing domain if you have one.</p>
                                </div>
                              </telerik:LayoutColumn>
                              <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                                <div class="service-pages-callout-white">
                                  <div class="service-pages-callout-icon">
                                    <i class="fa fa-cogs" aria-hidden="true"></i>
                                  </div>
                                  <h4>Custom Web Design</h4>
                                  <p>We will design your website using the state-of-the-art Content Management System (CMS), which is easy to update automatically. Best of all, with custom HTML, CSS, and JavaScript tools, it will look good, enabling you to attract and keep your audience.</p>
                                </div>
                              </telerik:LayoutColumn>
                              <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                                <div class="service-pages-callout-white">
                                  <div class="service-pages-callout-icon">
                                    <i class="fa fa-share-square-o" aria-hidden="true"></i>
                                  </div>
                                  <h4>Web Presence Consulting</h4>
                                  <p>We will analyze your existing and future web presence needs and develop a plan to help keep your web presence alive and dynamic.</p>
                                </div>
                              </telerik:LayoutColumn>
                            </Columns>
                          </telerik:LayoutRow>
                          <telerik:LayoutRow>
                            <Columns>
                              <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                                <div class="service-pages-callout-white">
                                  <div class="service-pages-callout-icon">
                                    <i class="fa fa-users" aria-hidden="true"></i>
                                  </div>
                                  <h4>Social Media Integration</h4>
                                  <p>Getting found is only the first step in marketing your site. The next step is to enable your users to share in the experience. This includes the ability to both keep up with what your doing and share the experience with all their friends!</p>
                                </div>
                              </telerik:LayoutColumn>
                              <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                                <div class="service-pages-callout-white">
                                  <div class="service-pages-callout-icon">
                                    <i class="fa fa-search-plus" aria-hidden="true"></i>
                                  </div>
                                  <h4>Search Engine Optimizaiton</h4>
                                  <p>Good design doesn’t just include effective graphics and layout. You need to have a website that ranks well on search engines like Google and Bing. My website design services include built-in SEO techniques that help customers find you again and again.</p>
                                </div>
                              </telerik:LayoutColumn>
                              <telerik:LayoutColumn Span="4" SpanMd="12" SpanSm="12">
                                <div class="service-pages-callout-white">
                                  <div class="service-pages-callout-icon">
                                    <i class="fa fa-wrench" aria-hidden="true"></i>
                                  </div>
                                  <h4>Design &amp; Revision</h4>
                                  <p>All our website design services include an additional 10 FREE hours of design and revision beyond the initial layout. This gives you an opportunity to tweak colors, fonts, and images, and fine tune your site so it’s just the way you want it. If you need additional hours, rest easy, as they’re charged at a low hourly rate.</p>
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
              </Rows>
            </telerik:RadPageLayout>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
            <div class="homearticle2">
            <div>
              <telerik:RadLinkButton ID="RadButton1" runat="server" Skin="Silk" Text="Find out how we can help you succeed!" NavigateUrl="~/ourplans"></telerik:RadLinkButton>
            </div>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <telerik:RadMediaPlayer ID="RadMediaPlayer2" runat="server" Height="360px" Width="640px" AutoPlay="false" Source="https://youtu.be/XTfznC7Vvdg" />
            <div class="homearticle">
              <p>
                Let us help you develop the online presence you need to grow your business the way you want.
                <br />
                <br />
                We will analyze your existing and future web presence needs and develop a plan to keep your web presence alive and dynamic. 
                Good design isn't just effective graphics and layout: you need a website that ranks well on search engines like Google and Bing. 
                Our website design services include built-in SEO techniques that help customers find you again and again. Getting found is only 
                the first step in marketing your site. The next step is to enable your users to share in the experience.
                <br />
                <br />
                We offer a full range of services so that we can tailor your plan to fit your needs and budget! Equally as important as building 
                the right statement for your business, is the power to deliver that message to the world. We also make available premium features 
                that can be added to any plan at extremely low costs! Use them to really jump-start your growth or build a huge customer base.
              </p>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <img src="/images/portfolio/portfolio-turley.png" alt="TurleysPlace" />
              <h2>Turley's Place</h2>
              <p>
                I first met David Nuckolls over 12 years ago and over the years, we have become not only business associates, 
                but also great friends. I have never worked with anyone quite as professional and detail oriented as David, 
                who also adds the personal touch and caters to his clients' individual needs.
                <br />
                <br />
                I am an international recording artist and seasoned producer, who has interacted with many record labels and 
                managers, but no one has been more hands-on making sure that I have everything that I need.
                <br />
                <br />
                Being a blind man in a technologically advanced world, causes lots of problems that most sighted people have 
                difficulty conceptualizing. As a small example, consider my screen reader for my computer: I was having difficulties 
                learning the system, and most likely would have given up on the computer world, but what did David do? He studied the 
                program (JAWS), and taught me the finer details of how to use it, which provided me a link to the sighted world that 
                I had never before realized. Whenever I am "stuck," David cheerfully accesses my computer via remote sharing, and patiently 
                walks me through the learning process.
                <br />
                <br />
                I mention these things because I think it is important that the reader understand how much David truly cares about his clients. 
                If you are considering David as someone to meet your needs in the computer world, I fully believe you will never find someone as 
                dedicated as he is to please you and do one hell of a great job. Open Quotation Marks
                <br />
                <br />
                <em><strong>~Turley Richards (Louisville KY)</strong> Producer, Singer, Songwriter, and Author</em>
              </p>
              <div>
                <telerik:RadLinkButton ID="RadButton4" runat="server" Skin="Silk" Text="Visit Turley's Place" NavigateUrl="https://www.turleyrichards.com" Target="_blank"></telerik:RadLinkButton>
              </div>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="12" SpanMd="12" SpanSm="12">
            <p></p>
            <p></p>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>
</asp:Content>
<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="server">
</asp:Content>

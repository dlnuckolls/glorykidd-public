<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="SoftwareServices.aspx.cs" Inherits="ElusiveSoftware.net.pages.SoftwareServices" %>

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
              <h2>Define Your Digital Strategy</h2>
              <h3>~ How can we help make you successful ~<br />
                <br />
              </h3>
            </div>
            <telerik:RadPageLayout runat="server" ID="RadPageLayout4">
              <Rows>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white2">
                        <h4>Custom Software</h4>
                        <p>Bespoke applications, plugins and tools to create maximum impact for your brand online.</p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white2">
                        <h4>Aftercare</h4>
                        <p>A custom care package tailored to your needs, to ensure you get the very best from your investment.</p>
                      </div>
                    </telerik:LayoutColumn>
                  </Columns>
                </telerik:LayoutRow>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white2">
                        <h4>Mobile Development</h4>
                        <p>Beautifully designed mobile applications and sites to meet the constraints of specific operating systems, screen sizes or devices.</p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white2">
                        <h4>Responsive Design</h4>
                        <p>Flexible frameworks that tailor the content, images, and functionality of your program to the device being used.</p>
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
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <h2>Advent Financial Systems</h2>
              <p>
                Advent and Glorykidd have been working together for more than ten years. During that time, David Nuckolls has developed multiple web and desktop 
                applications for us that are used on a daily basis. He is always responsive to our needs and is very patient when explaining how things work. 
                He has helped me personally with my attempt at learning how to read and write code so that I can do some of my own troubleshooting. Dave works 
                hard to ensure that our programs are scalable and makes valuable suggestions on what further automation can be provided to make our processing 
                more efficient. It is always a pleasure to work with him! 
                <br />
                <br />
                <br />
                <em><strong>~Jennifer Hobbs</strong>, Director of Technology (Elizabethtown KY)</em>
              </p>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <telerik:RadMediaPlayer ID="RadMediaPlayer2" runat="server" Height="360px" Width="640px" AutoPlay="false" Source="https://youtu.be/HDWxwPGkKC4" />
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="1" HiddenMd="true" />
          <telerik:LayoutColumn Span="10" SpanMd="12" SpanSm="12">
            <div class="pageHeader">
              <h3>
                <em>~ Our Development Philosophy ~</em><br />
                <br />
              </h3>
            </div>
            <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
              <Rows>
                <telerik:LayoutRow>
                  <Columns>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-bullseye" aria-hidden="true"></i>
                        </div>
                        <h4>Strategic Focus</h4>
                        <p>
                          Focused on delivering solutions that provide maximum benefits for businesses and their end-users, we go beyond simply 
                          developing products and instead work closely with our clients at a strategic level. Through a collaborative and adaptive 
                          long-term approach our dedicated team of software consultants gain an in-depth knowledge of your business and industry that 
                          guides the direction of development and ensures every decision supports your ultimate goals.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-code" aria-hidden="true"></i>
                        </div>
                        <h4>Technology Agnostic</h4>
                        <p>
                          Embracing the flexibility of a technology agnostic approach, our team is skilled in a wide range of tools, techniques and 
                          languages and work in multi-disciplined teams to maximise knowledge-sharing in a way that enables the project to quickly 
                          react to changing conditions and demands. Not constrained by technology limitations we place focus on gaining an in-depth 
                          understanding of your business and domain; with this knowledge driving all decisions we apply our technical insight to choose 
                          the tools that will help maximise speed, consistency, performance and returns throughout your project.
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
                          <i class="fa fa-tasks" aria-hidden="true"></i>
                        </div>
                        <h4>Agile Approach</h4>
                        <p>
                          Our Agile software developers are skilled in a vast range of technologies, from the cutting-edge to the more familiar tried and 
                          tested. Our Agile approach is uniquely applied across the project lifecycle to remove barriers between the consultancy and software 
                          development stages; increasing the speed of delivery while never compromising on quality.
                        </p>
                      </div>
                    </telerik:LayoutColumn>
                    <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
                      <div class="service-pages-callout-white">
                        <div class="service-pages-callout-icon">
                          <i class="fa fa-code-fork" aria-hidden="true"></i>
                        </div>
                        <h4>Development Process</h4>
                        <p>
                          Always be confident in the quality of delivery, knowing that we enforce software development "best practices" across every aspect 
                          of our processes. We employ industry-leading techniques such as automation, Test-Driven Development, Continuous Integration and 
                          Deployment-Minded Delivery to maximise the stability, security and scalability of your product. Our development team is committed 
                          to following rigorous coding standards and a series of robust architectural principles that support our stringent Quality Assurance 
                          (QA) procedures in ensuring delivery of the highest quality solutions.
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
                <telerik:RadLinkButton ID="RadButton1" runat="server" Skin="Silk" Text="Contact us today for details on how we can improve your processes!" NavigateUrl="~/contact"></telerik:RadLinkButton>
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

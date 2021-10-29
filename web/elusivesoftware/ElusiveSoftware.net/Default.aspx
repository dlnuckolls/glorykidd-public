<%@ Page Title="" Language="C#" MasterPageFile="~/MasterPage.Master" AutoEventWireup="true" CodeBehind="Default.aspx.cs" Inherits="ElusiveSoftware.net.Default" %>

<%@ Register TagPrefix="telerik" Namespace="Telerik.Web.UI" Assembly="Telerik.Web.UI" %>

<asp:Content ID="Content0" ContentPlaceHolderID="head" runat="Server">
  <asp:Literal ID="TitleTag" runat="server" />
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="ContentPlaceHolder1" runat="Server">
  <telerik:RadPageLayout runat="server" ID="RadPageLayout1">
    <Rows>
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <div class="homearticle">
              <img src="/images/gk-logo-square.svg" alt="GloryKidd" />
              <h2>
                Who are We?</h2>
              <p>GloryKidd Technologies mission is to leverage the "Best in Breed" technology to help spread the Gospel. We will 
                use our skills and talents to this end, and learn new skills to further push the envelope whenever possible. We 
                will continue to provide the highest standard of excellence to all clients, helping them to achieve their goals. 
                Overall, GloryKidd Technologies will provide the type and style of service that will reflect Almighty God in all 
                that we do and say. He is the author and source of our talents, so we just cannot do anything less!
              </p>
              <div><telerik:RadLinkButton ID="RadButton4" runat="server" Skin="Silk" Text="Ask us what we can to for you!" NavigateUrl="~/contact"></telerik:RadLinkButton></div>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="6" SpanMd="12" SpanSm="12">
            <telerik:RadMediaPlayer ID="RadMediaPlayer2" runat="server" Height="360px" Width="640px" AutoPlay="false" Source="https://youtu.be/pRZ3iggpyYw" />
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
      <telerik:LayoutRow>
        <Columns>
          <telerik:LayoutColumn Span="7" SpanMd="12" SpanSm="12">
            <div class="jumbotron">
              <h3>Custom Software Solutions
              <telerik:RadLinkButton ID="RadLinkButton1" runat="server" Skin="Silk" Text="Learn More" NavigateUrl="~/softwareservices"></telerik:RadLinkButton></h3>
              <p>
                Creative, pragmatic and technology agnostic, GloryKidd Technologies develops exciting and engaging high-performance software 
                applications, websites and products. Our award-winning service helps clients across the globe react to changing market conditions 
                and customer demands with the speed, scale and flexibility required to maximise returns.
              <br /><br />
                Extending the traditional boundaries of development, we bring together specialists across a wide range of disciplines including 
                User Experience, Content Strategy, Data Analytics, SEO, PPC and more, to deliver a tailored, comprehensive strategy that maximises 
                exposure and impact far beyond the delivery of your product. Tight integration between each phase of your project is achieved 
                through the adoption of an Agile web development framework that promotes flexibility, communication and collaboration, while maintaining 
                a shared understanding of the project’s vision across all stakeholders.
              <br /><br />
                Make sure your digital strategy delivers against user and business expectations by partnering with world-renowned technical experts to 
                provide value through every online interaction. Our Agile software developers are skilled in a vast range of technologies, from the 
                cutting-edge to the more familiar tried and tested. With decades of experience we know what’s most appropriate for your particular 
                requirements and how to derive optimum results from the technology we employ; whether enterprise-class integration or best-of-breed 
                open-source solutions.
              </p>
            </div>
          </telerik:LayoutColumn>
          <telerik:LayoutColumn Span="5" SpanMd="12" SpanSm="12">
            <div class="jumbotron">
              <h3>Web Services
              <telerik:RadLinkButton ID="RadLinkButton2" runat="server" Skin="Silk" Text="Learn More" NavigateUrl="~/webservices"></telerik:RadLinkButton></h3>
              <p>
                Every website should be as unique as the owner. We can help you develop the online presence you need to build your business the way 
                you want. We offer a complete line of packages to fit every need and budget. We provide service offerings that can increase traffic 
                to your site, better integration with Social Media, and Search Engine Optimazations. We specialize in tailoring our service to make 
                your vision a reality!
              <br /><br />
                Equally as important as building the right statement for your business, is the power to deliver that message to the world. GloryKidd 
                Technologies offers a full range of services tailored specifically to meet your needs and budget! We cover all the features that our 
                customers expect and need to promote their message loud and clear. Hosting plans are tailored to give you the maximum value on a budget price.
              <br /><br />
                We also offer a range of hosting options designed to match your budget. Our high performance servers can keep your content available 
                to the world around the clock, and we take care of the maintenance so you can rest easy and leave the worries to us.
              </p>
            </div>
          </telerik:LayoutColumn>
        </Columns>
      </telerik:LayoutRow>
    </Rows>
  </telerik:RadPageLayout>

</asp:Content>

<asp:Content ID="Content3" ContentPlaceHolderID="ContentPlaceHolder2" runat="Server">
</asp:Content>

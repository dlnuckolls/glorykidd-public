using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace ElusiveSoftware.net {
  public class Global : System.Web.HttpApplication {

    protected void Application_Start(object sender, EventArgs e) {
      RegisterRoutes(RouteTable.Routes);
    }

    private void RegisterRoutes(RouteCollection routes) {
      routes.MapPageRoute("", "clientemail", "~/pages/ClientEmail.aspx");
      routes.MapPageRoute("", "consulting", "~/pages/Consulting.aspx");
      routes.MapPageRoute("", "contact", "~/pages/Contact.aspx");
      routes.MapPageRoute("", "ourplans", "~/pages/OurPlans.aspx");
      routes.MapPageRoute("", "softwareservices", "~/pages/SoftwareServices.aspx");
      routes.MapPageRoute("", "webservices", "~/pages/WebServices.aspx");
    }

    protected void Session_Start(object sender, EventArgs e) { }

    protected void Application_BeginRequest(object sender, EventArgs e) { }

    protected void Application_AuthenticateRequest(object sender, EventArgs e) { }

    protected void Application_Error(object sender, EventArgs e) { }

    protected void Session_End(object sender, EventArgs e) { }

    protected void Application_End(object sender, EventArgs e) { }
  }
}
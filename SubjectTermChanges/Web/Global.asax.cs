using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using Kindred.Common.Logging;

namespace Kindred.Knect.ITAT.Web
{
    public class Global : System.Web.HttpApplication
    {

        protected void Application_Start(object sender, EventArgs e)
        {
            Type type = this.GetType();
            ILog log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
            log.Info(string.Format("ITAT web app started.  Version {0}.", Helper.GetVersion()));
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {

        }

        protected void Session_End(object sender, EventArgs e)
        {

        }

        protected void Application_End(object sender, EventArgs e)
        {
            ILog log = Kindred.Common.Logging.LogManager.GetLogger(this.GetType());
            log.Info("ITAT web app ended");
        }
    }
}
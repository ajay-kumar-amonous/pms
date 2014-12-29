using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace pEasyPrint
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
           // AntiForgeryConfig.SuppressIdentityHeuristicChecks = false;
            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AuthConfig.RegisterAuth();
        }

        protected void Application_Error(Object sender, EventArgs e)
        {
            Exception ex = Server.GetLastError();
            Errorslog(ex.Message,ex.StackTrace);
            
            
        }
        public void Errorslog(string Message, string TraceLocation)
        {
            // Compose a string that consists of three lines.
            string lines1 = "============================= \r\n";
            string lines2 = DateTime.Now + "\r\n";
            string lines3 = "=============================\r\n";



            string lines = "First line.\r\nSecond line.\r\nThird line.";

          // Write the string to a file.
            System.IO.StreamWriter file = new System.IO.StreamWriter(Server.MapPath("~/Content/errorfile.txt"),true);
            file.WriteLine(lines1);
            file.WriteLine(lines2);
            file.WriteLine(Message);
            file.WriteLine(TraceLocation);
            file.WriteLine(lines3);

            file.Close();

            

        }

    }
}
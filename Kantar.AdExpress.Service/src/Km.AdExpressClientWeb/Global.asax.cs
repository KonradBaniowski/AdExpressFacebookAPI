using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace Km.AdExpressClientWeb
{
    // Note: For instructions on enabling IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=301868
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            try
            {
                //CreativeConfigDataAccess.LoadPathes(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.CREATIVES_PATH_CONFIGURATION));
            }
            catch (System.Exception error)
            {
                //string body = "";
                //try
                //{
                //    BaseException err = (BaseException)error;
                //    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                //}
                //catch (System.Exception)
                //{
                //    try
                //    {
                //        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                //    }
                //    catch (System.Exception es)
                //    {
                //        throw (es);
                //    }
                //}
                //TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                //errorMail.Send("Erreur d'initialisation d'AdExpress " + (Server.MachineName), body, true, false);
                //throw (error);
            }

            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }
    }
}

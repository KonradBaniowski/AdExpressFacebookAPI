using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.FrameWork.DB.Common;

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
            LoadConfigurations();
        }

        protected static void LoadConfigurations()
        {
            try
            {
                //Initialisation des chemins d'accès aux créations
                CreativeConfigDataAccess.LoadPathes(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.CREATIVES_PATH_CONFIGURATION));
                Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));

            }
            catch (System.Exception)
            {
                //TODO : A REFAIRE

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

        }
    }
}

using Km.AdExpressClientWeb.App_Start;
using System;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.CampaignTypes;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Date;
using TNS.Alert.Domain;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
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
                MvcHandler.DisableMvcResponseHeader = true;
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

            
            // AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            AutoMapperConfig.Configure();
            LoadConfigurations();

        }

        protected void Application_PreSendRequestHeaders()
        {
            if (HttpContext.Current != null)
            {
                HttpContext.Current.Response.Headers.Remove("Server");
            }
        }

        protected static void LoadConfigurations()
        {
            try
            {
                //Initialisation des chemins d'accès aux créations
                CreativeConfigDataAccess.LoadPathes(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.CREATIVES_PATH_CONFIGURATION));
                Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.BAAL_CONFIGURATION_FILENAME));

                //Langues 
                Int64 dd = WebApplicationParameters.DefaultLanguage;
                IDataSource tt = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.session);
                // Initialisation des listes de texte
                TNS.AdExpress.AdExpressWordListLoader.LoadLists();

                // Chargement des niveaux de détail AdNetTrack
                AdNetTrackDetailLevelsDescription.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.ADNETTRACK_DETAIL_LEVEL_CONFIGURATION_FILENAME));

                //Units
                UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNITS_CONFIGURATION_FILENAME));

                //Vehicles
                VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));

                //Lists
                TNS.AdExpress.Domain.Lists.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.LISTS_CONFIGURATION_FILENAME));

                // Chargement des noms de modules et des catégories de modules               
                ActiveMediaList.Init(WebApplicationParameters.DefaultDataLanguage);

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
                IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
                LastAvailableDate.Init(date.GetLastAvailableDate());

                //Charge les niveaux d'univers
                TNS.Classification.Universe.UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));

                //Charge les styles personnalisés des niveaux d'univers
               // TNS.Classification.Universe.UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));

                //Charge la hierachie de niveau d'univers
                TNS.Classification.Universe.UniverseBranches.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));

                //Load flag list
                TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));

                //Load Global WebSite options
                ResultOptionsXL.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.RESULT_OPTIONS_CONFIGURATION_FILENAME));

                // Loading Ares Config configuration
                PluginConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
                AlertConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.ALERTE_CONFIGURATION));
                DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.Ares.Constantes.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));

                //Campaign  types
                CampaignTypesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.CAMPAIGN_TYPES_CONFIGURATION_FILENAME));

                if (WebApplicationParameters.VehiclesFormatInformation != null && WebApplicationParameters.VehiclesFormatInformation.Use)
                    VehiclesFormatList.Init(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList, WebApplicationParameters.DefaultDataLanguage);

                if (WebApplicationParameters.UsePurchaseMode)
                    PurchaseModeList.Init();


                if (WebApplicationParameters.UseSpotSubType)
                    SpotSubTypes.Init();

            }
          
            catch (System.Exception E)
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

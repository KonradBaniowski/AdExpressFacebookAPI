using System;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNSMail=TNS.FrameWork.Net.Mail;
using AnubisConstantes=TNS.AdExpress.Anubis.Constantes;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Layers;
using System.Reflection;
using TNS.AdExpressI.Date;
using TNS.AdExpress.Domain.CampaignTypes;
using System.Text;
using System.Web;


namespace AdExpress
{
    /// <summary>
    /// Description résumée de [!output SAFE_CLASS_NAME].
    /// </summary>
    public class Global : System.Web.HttpApplication
    {

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public Global()
        {
            InitializeComponent();
        }
        #endregion


        /// <summary>
        /// L'application est lancée
        /// </summary>
        /// <param name="sender">Objet Source</param>
        /// <param name="e">Arguments</param>
        protected void Application_Start(Object sender, EventArgs e)
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
                TNS.Classification.Universe.UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));

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
            }
            catch (System.Exception error)
            {
                string body = "";
                try
                {
                    BaseException err = (BaseException)error;
                    body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur" + err.GetHtmlDetail() + "</font></html>";
                }
                catch (System.Exception)
                {
                    try
                    {
                        body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>L'initialisation du server a &eacute;chou&eacute;.</font><br>Erreur(" + error.GetType().FullName + "):" + error.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + error.StackTrace.Replace("at ", "<br>at ") + "</font></html>";
                    }
                    catch (System.Exception es)
                    {
                        throw (es);
                    }
                }
                TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                errorMail.Send("Erreur d'initialisation d'AdExpress " + (Server.MachineName), body, true, false);
                throw (error);
            }
        }


        #region Autres évènements non gérés
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_Start(Object sender, EventArgs e)
        {
                if (WebApplicationParameters.VehiclesFormatInformation!=null && WebApplicationParameters.VehiclesFormatInformation.Use)
                    VehiclesFormatList.Init(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList, WebApplicationParameters.DefaultDataLanguage);

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_BeginRequest(Object sender, EventArgs e)
        {
            //string titi=Request.Path;

        }
        /// <summary>
        ///		
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_EndRequest(Object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_AuthenticateRequest(Object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_Error(Object sender, EventArgs e)
        {
            //Get exception details
            Exception ex = HttpContext.Current.Server.GetLastError();

            if (ex != null && ex.GetType() == typeof(HttpUnhandledException))
            {
                try
                {
                    string body = "<html><b><u>" + Server.MachineName + ":</u></b><br>" + "<font color=#FF0000>An unhandled Exception occurs.</font>";
                    System.Exception currentException = ex;
                    while (currentException != null)
                    {
                        body += "<br>Error(" + currentException.GetType().FullName + "):" + currentException.Message + "<br><br><b><u>Source:</u></b><font color=#008000>" + currentException.StackTrace.Replace("at ", "<br>at ") + "</font>";
                        body += "<br><hr>";
                        currentException = currentException.InnerException;
                    }
                    body += "</html>";
                    TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                    SetCharset(errorMail);
                    errorMail.SendWithoutThread(" AdExpress unhandled Exception " + (Server.MachineName), body, true, false);
                }
                catch (Exception)
                {
                    string body = " An AdExpress unhandled Exception occured. But it's not possible to get details.";
                    TNSMail.SmtpUtilities errorMail = new TNSMail.SmtpUtilities(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ErrorManager.WEBSERVER_ERROR_MAIL_FILE);
                    SetCharset(errorMail);
                    errorMail.SendWithoutThread(" AdExpress unhandled Exception " + (Server.MachineName), body, true, false);
                }
                finally
                {
                    Response.Redirect("/Public/Message.aspx?msgCode=5");
                }
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Session_End(Object sender, EventArgs e)
        {

        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Application_End(Object sender, EventArgs e)
        {

        }
        #endregion

        #region Web Form Designer generated code
        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
        }
        #endregion

        #region SetCharset
        /// <summary>
        /// Set charset
        /// </summary>
        /// <param name="errorMail">Smtp utilities</param>
        private void SetCharset(TNSMail.SmtpUtilities errorMail)
        {
            try
            {
                string charset = WebApplicationParameters.AllowedLanguages[WebApplicationParameters.DefaultLanguage].Charset;
                if (!string.IsNullOrEmpty(charset))
                {
                    Encoding encoding = null;
                    try
                    {
                        encoding = Encoding.GetEncoding(charset);
                    }
                    catch (Exception) { }
                    if (encoding != null)
                    {
                        errorMail.SubjectEncoding = encoding;
                        errorMail.BodyEncoding = encoding;
                    }
                    errorMail.CharsetTextHtml = charset;
                    errorMail.CharsetTextPlain = charset;
                }
            }
            catch (Exception) { }
        }
        #endregion
        
    }
}


using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWorks.LSConnectivity;
using System.Collections;
using TNS.LinkSystem.LinkKernel;
using TNS.FrameWork.DB.Common;
using TNS.Ares.Alerts.DAL;

using System.IO;
using System.Xml;
using TNS.FrameWork.Net.Mail;

using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;

using WebFunctions = TNS.AdExpress.Web.Functions;
using System.Threading;
using System.Net.Mail;
using TNS.Ares.StaticNavSession.DAL;
using TNS.Ares.Domain.Mail;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using TNS.Ares.Domain.LS;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.Ares.AdExpress.MailAlert.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Domain.DataBase;

using ConfigurationFile = TNS.Ares.Constantes.ConfigurationFile;
using TNS.Classification.Universe;
using System.Reflection;
using TNS.Ares.Domain.Layers;

namespace TNS.Ares.AdExpress.MailAlert
{
    /// <summary>
    /// Shell used for the alerts treatment
    /// </summary>
    public class AlertShell : Shell {

        #region Variables
        /// <summary>
        /// Data source
        /// </summary>
        private IDataSource _source;
        /// <summary>
        /// Alert DAL Dll
        /// </summary>
        private IAlertDAL _alertDAL;
        /// <summary>
        /// Static Nav Session DAL Dll
        /// </summary>
        private IStaticNavSessionDAL _navDAL;
        /// <summary>
        /// End execution Date
        /// </summary>
        private DateTime _endExecutionDateTime = DateTime.Now;
        /// <summary>
        /// Execution duration
        /// </summary>
        private TimeSpan _duration;
        /// <summary>
        /// Max available slots
        /// </summary>
        private int _maxAvailableSlots;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Name that appears in the link console</param>
        /// <param name="familyId">Id used to assemble a group of modules tasks</param>
        /// <param name="source">Data source</param>
        /// <param name="moduleDescriptionList">The list of modules that the client can treat</param>
        public AlertShell(LsClientConfiguration lsClientConfiguration)
            : base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, lsClientConfiguration.MaxAvailableSlots.ToString(), lsClientConfiguration.ModuleDescriptionList, true, true)
        {
        }
        #endregion

        #region Initialize
        /// <summary>
        /// Initialize
        /// </summary>
        protected override void InitializeShell(string pathConfiguration){
            try {

                #region WebApplicationParameters
                try {
                    new WebApplicationParameters();
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load WebApplicationParameters", e);
                }
                #endregion

                #region Product Baal List
                try {
                    Product.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Product Baal List", e);
                }
                #endregion

                #region Media Baal List
                try {
                    Media.LoadBaalLists(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.BAAL_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load Media Baal List", e);
                }
                #endregion

                #region UnitsInformation
                try {
                    // Units
                    UnitsInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNITS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UnitsInformation", e);
                }
                #endregion

                #region VehiclesInformation
                try {
                    // Vehicles
                    VehiclesInformation.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.VEHICLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load VehiclesInformation", e);
                }
                #endregion

                #region UniverseLevels
                try {
                    // Universes
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region UniverseLevelsCustomStyles
                try {
                    // Charge les styles personnalisés des niveaux d'univers
                    UniverseLevelsCustomStyles.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CUSTOM_STYLES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevelsCustomStyles", e);
                }
                #endregion

                #region UniverseBranches
                try {
                    // Charge la hierachie de niveau d'univers
                    UniverseBranches.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_BRANCHES_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseBranches", e);
                }
                #endregion

                #region UniverseLevels
                try {
                    // Charge les niveaux d'univers
                    UniverseLevels.getInstance(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.UNIVERSE_LEVELS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load UniverseLevels", e);
                }
                #endregion

                #region AllowedFlags
                try {
                    //Load flag list
                    TNS.AdExpress.Domain.AllowedFlags.Init(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + WebConstantes.ConfigurationFile.FLAGS_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load AllowedFlags", e);
                }
                #endregion

                #region ModulesList
                try {
                    new ModulesList();
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to load ModulesList", e);
                }
                #endregion

                #region Ares DataBaseConfiguration
                try {
                    // Loading DataBase configuration
                    DataBaseConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.DATABASE_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Load DataBaseConfiguration", e);
                }
                #endregion
                
                #region Get Source
                try {
                    this._source = TNS.Ares.Domain.DataBase.DataBaseConfiguration.DataBase.GetDefaultConnection(TNS.Ares.Domain.DataBaseDescription.DefaultConnectionIds.alert);
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Get dataSource", e);
                }
                #endregion

                #region Ares PluginConfiguration
                try {
                    PluginConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + ConfigurationFile.PLUGIN_CONFIGURATION_FILENAME));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Load Ares PluginConfiguration", e);
                }
                #endregion

                #region Ares Alert PluginConfiguration
                try {
                    AlertConfiguration.Load(new XmlReaderDataSource(WebApplicationParameters.CountryConfigurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.ALERTE_CONFIGURATION));
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Load AlertConfiguration : " + e);
                }
                #endregion

                #region Create Instance of Alert DAL
                try {
                    object[] parameter = new object[1];
                    parameter[0] = _source;
                    DataAccessLayer cl = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                    this._alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Create Instance Of Layer IAlertDAL ", e);
                }
                #endregion

                #region Create Instance of Static Nav Session
                try {
                    object[] parameter = new object[1];
                    parameter[0] = _source;
                    DataAccessLayer cl = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Session);
                    this._navDAL = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameter, null, null, null);
                }
                catch (Exception e) {
                    throw new ShelInitializationException("Impossible to Create Instance Of Layer IStaticNavSessionDAL ", e);
                }
                #endregion

                this._oLinkClient.AlwaysReconnect = true;
                _maxAvailableSlots = Int32.Parse(pathConfiguration);

                base.InitializeShell(pathConfiguration);

            }
            catch (Exception e) {
                this.sendEmailError("Initialization Error in Shell in Initialize()", e);
                throw new ShelInitializationException("Initialization Error in Shell in Initialize()", e);
            }
        }
        #endregion

        #region Tasks
        /// <summary>
        /// 
        /// </summary>
        /// <param name="oObj"></param>
        protected override void DoTask(object oObj) {
            TaskExecution oTaskExecution = null;

            try {
                this.Log(new LogLine("Task received", eLogCategories.Information));
                // Setting variables
                oTaskExecution = (TaskExecution)oObj;
                long alertId = -1;

                // Getting email content
                this.Log(new LogLine("Getting email content", eLogCategories.Information, "Alerts"));

                if (oTaskExecution != null) {

                    _endExecutionDateTime = DateTime.Now;
                    // Extracting alert from XMLTask property
                    this.Log(new LogLine("Extracting alert from XMLTask property", eLogCategories.Information, "Alerts"));
                    alertId = getAlertId(oTaskExecution);

                    // Task treatment
                    if (alertId != -1) {
                        this.Log(new LogLine(string.Format("Begin Alert traitment for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                        if (_oListRunningTasks.ContainsKey(alertId) == false) {

                            lock (_oObjectLocker) {
                                _oListRunningTasks.Add(alertId, oTaskExecution);
                            }

                        }

                        try {

                            // Loading alert from database
                            this.Log(new LogLine(string.Format("Loading alert from database for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            TNS.Alert.Domain.Alert alert = this._alertDAL.GetAlert((int)alertId);

                            // Updating session information
                            this.Log(new LogLine(string.Format("Updating session information for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            WebSession session = (WebSession)alert.Session;
                            DateTime FirstDayNotEnable = DateTime.Now;
                            if (session.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.JUSTIFICATIFS_PRESSE)
                                if (session.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE) {

                                    try {

                                        int oldYear = 2000;
                                        long selectedVehicle = ((LevelInformation)session.SelectionUniversMedia.FirstNode.Tag).ID;
                                        FirstDayNotEnable = WebFunctions.Dates.GetFirstDayNotEnabled(session, selectedVehicle, oldYear, this._source);
                                    }
                                    catch (Exception e) {
                                        this.Log(new LogLine(String.Format("Impossible to get vehicle from session [#{0}] for alert '{1}'", session.IdSession, alertId.ToString()), e, eLogCategories.Warning, "Alerts"));                                    }
                                }

                            // Updating session dates
                            this.Log(new LogLine(string.Format("Updating session dates for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            session.UpdateDates(FirstDayNotEnable, ((DateTime)this._navDAL.GetRow(extractParameterId(oTaskExecution))["date_creation"]));
                            // session.PeriodType = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate;

                            // Inserting alert occurrence
                            this.Log(new LogLine(string.Format("Inserting alert occurrence for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            int occId = this._alertDAL.InsertAlertOccurrenceData(DateTime.Now.AddDays(AlertConfiguration.OccurrenceInformation.DayExpiration), session.PeriodBeginningDate, session.PeriodEndDate, alert.AlertId);

                            // Creating alert content
                            this.Log(new LogLine(string.Format("Creating alert content for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));

                            string alertContent = GetMailContent(alert.Title, String.Format("{0}Private/Alerts/ShowAlert.aspx?idAlert={1}&idOcc={2}", TNS.Alert.Domain.AlertConfiguration.MailInformation.TargetHost, alert.AlertId, occId), session.SiteLanguage);

                            // Sending email
                            this.Log(new LogLine(string.Format("Sending email for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            SmtpClient client = new SmtpClient(SmtpServer);
                            MailMessage email = new MailMessage(SmtpFromAddress, alert.Recipients.Replace(";", ","));
                            email.BodyEncoding = Encoding.UTF8;
                            email.Subject = alert.Title;
                            email.Body = alertContent;
                            email.IsBodyHtml = true;
                            client.Send(email);

                            // Successfully sent the alert
                            this.Log(new LogLine(string.Format("The alert was successfully sent for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));


                            // Updating static nav session row
                            this.Log(new LogLine(string.Format("Updating static nav session row with a statut = 3 for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                            this._navDAL.UpdateStatus((int)extractParameterId(oTaskExecution), TNS.Ares.Constantes.Constantes.Result.status.sent.GetHashCode());

                            // Delete static nav session row
                            PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Alertes);
                            if (pluginInformation.DeleteRowSuccess) {
                                this.Log(new LogLine(string.Format("Delete static nav session row #{1} for alert '{0}'", alertId.ToString(), extractParameterId(oTaskExecution).ToString()), eLogCategories.Information, "Alerts"));
                                this._navDAL.DeleteRow(extractParameterId(oTaskExecution));
                            }

                            _duration = DateTime.Now.Subtract(_endExecutionDateTime);

                        }
                        catch (Exception e) {
                            // An exception was thrown during alert treatment
                            LogLine error = new LogLine("An error occurred during alert treatment", e, eLogCategories.Problem, "Alerts");
                            this.Log(error);
                            sendEmailError("An error occurred during alert treatment", e);
                            this._oLinkClient.ReleaseTaskInError(oTaskExecution, error);
                        }

                        this.Log(new LogLine(string.Format("End alert traitment for alert '{0}'", alertId.ToString()), eLogCategories.Information, "Alerts"));
                        this._oLinkClient.ReleaseTask(oTaskExecution);
                    }
                    else {
                        LogLine error = new LogLine(string.Format("Alert Id ''{0}'' is invalide in dynamic task", alertId.ToString()), eLogCategories.Information, "Alerts");
                        this.Log(error);
                        sendEmailError(string.Format("Alert Id ''{0}'' is invalide in dynamic task", alertId.ToString()));
                        this._oLinkClient.ReleaseTaskInError(oTaskExecution, error);
                    }
                    
                }
                else {
                    // The task description is null
                    this._oLinkClient.ReleaseTaskInError(oTaskExecution, new LogLine("TaskExecution object is null", eLogCategories.Problem, "Alerts"));
                    sendEmailError("TaskExecution object is null");
                }
            }
            catch (Exception e) {
                sendEmailError("Error in Alert traitment task", e);
                if(oTaskExecution!=null)
                    this._oLinkClient.ReleaseTaskInError(oTaskExecution, new LogLine("TaskExecution object is null", eLogCategories.Problem, "Alerts"));
            }
        }

        #endregion

        #region MaxAvailableSlots
        /// <summary>
        /// Max Available Slots
        /// </summary>
        /// <returns>Max available slots number</returns>
        public override int MaxAvailableSlots() {
            return (_maxAvailableSlots);
        }
        #endregion

        #region Private method
        /// <summary>
        /// Retrieving alert id  from parameter string
        /// </summary>
        /// <param name="oTaskExecution">Task parameters</param>
        /// <returns>The alert id contained in alert_id</returns>
        private long getAlertId(TaskExecution oTaskExecution)
        {
            long alertId = -1;
            StringReader sr = new StringReader(oTaskExecution.XMLTask);
            XmlTextReader reader = new XmlTextReader(sr);
            reader.Read();
            try
            {
                alertId = long.Parse(reader.GetAttribute("alert_id"));
            }
            catch (Exception e)
            {
                this._oLinkClient.ReleaseTaskInError(oTaskExecution, new LogLine("Task parameter error: no valid 'parameter_id' attribute in task definition", e, eLogCategories.Problem, "Alerts"));
            }
            return (alertId);
        }
        #endregion

        #region Email
        /// <summary>
        /// Send Mail Error
        /// </summary>
        /// <param name="body">Body</param>
        protected override void sendEmailError(string body) {
            if (this.SendEmailOnError) {
                string subject = " " + _strProductName + " (" + Environment.MachineName + ")";
                SmtpUtilities mailError = new SmtpUtilities(this.SmtpFromAddress, new ArrayList(this.ErrorEmailList.Split(';')), subject, Convertion.ToHtmlString(body), true, this.SmtpServer, this.MailPort);
                mailError.SendWithoutThread(false);
            }
        }

        /// <summary>
        /// Send Mail Error
        /// </summary>
        /// <param name="e">Exception</param>
        protected override void sendEmailError(Exception e) {

            if (this.SendEmailOnError) {

                string subject = " " + _strProductName + " (" + Environment.MachineName + ")";
                string stackTrace = string.Empty;
                string message = string.Empty;
                string body = string.Empty;

                try {
                    BaseException err = ((BaseException)((Exception)e));
                    message = err.Message;
                    stackTrace = err.GetHtmlDetail();
                }
                catch {
                    try {
                        message = e.Message;
                        stackTrace = e.StackTrace;
                    }
                    catch (System.Exception es) {
                        throw (es);
                    }
                }

                #region Message d'erreur
                body = "<html>";
                body += "<hr>";
                body += "<u>Message d'erreur:</u><br>" + message + "<br>";
                if (stackTrace != null)
                    body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                body += "</html>";
                #endregion

                SmtpUtilities mailError = new SmtpUtilities(this.SmtpFromAddress, new ArrayList(this.ErrorEmailList.Split(';')), subject, Convertion.ToHtmlString(body), true, this.SmtpServer, this.MailPort);
                mailError.SendWithoutThread(false);
            }
        }

        /// <summary>
        /// Send Mail Error
        /// </summary>
        /// <param name="e">Exception</param>
        /// <param name="message">Message</param>
        protected void sendEmailError(string message, Exception e) {

            if (this.SendEmailOnError) {

                string subject = " " + _strProductName + " (" + Environment.MachineName + ")";
                string stackTrace = string.Empty;
                string body = string.Empty;

                try {
                    BaseException err = ((BaseException)((Exception)e));
                    if (message.Trim().Length <= 0)
                        message = err.Message;
                    stackTrace = err.GetHtmlDetail();
                }
                catch {
                    try {
                        if (message.Trim().Length <= 0)
                            message = e.Message;
                        stackTrace = e.StackTrace;
                    }
                    catch (System.Exception es) {
                        throw (es);
                    }
                }

                #region Message d'erreur
                body = "<html>";
                body += "<hr>";
                body += "<u>Message d'erreur:</u><br>" + message + "<br>";
                if (stackTrace != null)
                    body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                body += "</html>";
                #endregion

                SmtpUtilities mailError = new SmtpUtilities(this.SmtpFromAddress, new ArrayList(this.ErrorEmailList.Split(';')), subject, Convertion.ToHtmlString(body), true, this.SmtpServer, this.MailPort);
                mailError.SendWithoutThread(false);
            }
        }


        #endregion

        #region Get Email Content
        /// <summary>
        /// Get Alert Mail Client Content
        /// </summary>
        /// <param name="title">Title</param>
        /// <param name="link">Link</param>
        /// <param name="language">Language</param>
        /// <returns></returns>
        private string GetMailContent(string title, string link, int language) {
            StringBuilder mailContent = new StringBuilder();

            mailContent.AppendFormat("<p>{0}</p>",GestionWeb.GetWebWord(2625, language));

            mailContent.AppendFormat("<p>{0} \"{1}\".</p>", 
                GestionWeb.GetWebWord(2626, language),
                title);

            mailContent.AppendFormat("<p>{0} <a href=\"{1}\">{2}</a>.</p>", 
                GestionWeb.GetWebWord(2627, language),
                link,
                GestionWeb.GetWebWord(2628, language));

            mailContent.AppendFormat("<p>{0}</p>", GestionWeb.GetWebWord(2629, language));

            return mailContent.ToString();
        }
        #endregion

    }
}

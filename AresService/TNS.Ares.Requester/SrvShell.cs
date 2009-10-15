using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
//using TNS.Ares.StaticNav;
//using TNS.Ares.Alerts;

//using DefaultStatic = TNS.Ares.StaticNav.DAL.Default;
//using DefaultAlert = TNS.Ares.Alerts.DAL.Default;
using TNS.LinkSystem.LinkKernel;
using System.Data;
using TNS.Ares.Domain.LS;
using AresConst = TNS.Ares.Constantes.Constantes;
using TNS.FrameWorks.LSConnectivity;
using System.Text.RegularExpressions;
using TNS.Ares.Domain;
using TNS.Ares.StaticNavSession.DAL;
using TNS.Ares.Alerts.DAL;
using TNS.Alert.Domain;
using TNS.Ares.Domain.Layers;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.Net.Mail;
using System.Collections;
using TNS.FrameWork;
using System.Xml;


namespace TNS.Ares.Requester
{
    /// <summary>
    /// Requester Shell Class
    /// </summary>
    public class SrvShell : Shell
    {
        #region Enum
        /// <summary>
        /// LS Module Id
        /// </summary>
        public enum moduleId
        {
            /// <summary>
            /// Anubis Module
            /// </summary>
            anubis = 0,
            /// <summary>
            /// Anubis Error Module
            /// </summary>
            anubisError = 2,
            /// <summary>
            /// Alerts Module
            /// </summary>
            alerts = 3,
            /// <summary>
            /// Alerts Error Module
            /// </summary>
            alertsError = 4,
            /// <summary>
            /// Crawler Module
            /// </summary>
            CrawlerRequest = 5,
            /// <summary>
            /// Garbage AdExpress
            /// </summary>
            garbageAdExpress = 10,
            /// <summary>
            /// Garbage Alertes
            /// </summary>
            garbageAlertes = 11
        }
        #endregion

        #region Variables
        /// <summary>
        /// DataSource
        /// </summary>
        IDataSource _source;
        /// <summary>
        /// DAL Layer for a static nav session
        /// </summary>
        IStaticNavSessionDAL _staticNavDAL;
        /// <summary>
        /// DAL Layer for a alert
        /// </summary>
        IAlertDAL _alertDAL;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="requesterName">Requester</param>
        /// <param name="requesterConfiguration">Resquester Configuration</param>
        /// <param name="source">Data Source</param>
        public SrvShell(LsClientName requesterName, RequesterConfiguration requesterConfiguration, IDataSource source) :
            base(requesterConfiguration.ProductName, requesterConfiguration.FamilyId, requesterConfiguration.ModuleDescriptionList)
        {

            #region variables
            object[] parameters = null;
            DataAccessLayer dataAccessLayer = null;
            #endregion

            if (source == null && requesterName != LsClientName.PixPalaceRequester)
                throw new ArgumentNullException("SrvShell constructor: Datasource cannot be null");
            this._source = source;

            if (requesterName != LsClientName.PixPalaceRequester)
            {
                #region Load plugin Session
                dataAccessLayer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Session);
                if (dataAccessLayer == null) throw new Exception("PluginConfiguration haven't PluginDataAccessLayerName 'Session'");

                parameters = new object[1];
                parameters[0] = this._source;

                this._staticNavDAL = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + dataAccessLayer.AssemblyName, dataAccessLayer.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, parameters, null, null, null);
                #endregion

                #region Load plugin Alert
                dataAccessLayer = PluginConfiguration.GetDataAccessLayer(PluginDataAccessLayerName.Alert);
                if (dataAccessLayer != null) this._alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + dataAccessLayer.AssemblyName, dataAccessLayer.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, parameters, null, null, null);
                #endregion
            }
        }

        #endregion

        #region Tasks

        #region CrawlerRequest - Generate Dynamic Task For Crawlers

        /// <summary>
        /// Generates all dynamic tasks that should be sent
        /// </summary>
        /// <param name="oTaskContext"></param>
        private void CrawlerRequestGenerateDynamicTask(TaskExecution oTaskContext)
        {
            try
            {
                // Get infos from Directory
                if (ConfigParamHelper.InputDirectories == null || ConfigParamHelper.InputDirectories.Length == 0)
                {
                    this.Log(new LogLine("No Input directory to watch is specified in configuration file !", eLogCategories.Problem, "CrawlerRequest"));
                    return;
                }

                RequesterConfiguration currentCrawlerConfiguration = RequesterConfigurations.GetAresConfiguration(LsClientName.PixPalace);
                if (currentCrawlerConfiguration == null || currentCrawlerConfiguration.ModuleDescriptionList.Count == 0)
                {
                    this.Log(new LogLine("No PixPalace services is specified in configuration file !", eLogCategories.Problem, "CrawlerRequest"));
                    return;
                }

                foreach (string directoryPath in ConfigParamHelper.InputDirectories)
                {
                    if (!Directory.Exists(directoryPath))
                    {
                        this.Log(new LogLine("Directory specified in configuration file doesn't exist : " + directoryPath, eLogCategories.Warning, "CrawlerRequest"));
                        continue;
                    }

                    IList<string> fileList = Directory.GetFiles(directoryPath, "*.xml");

                    foreach (string file in fileList)
                    {
                        if (!File.Exists(file))
                        {
                            this.Log(new LogLine("File doesn't exist : " + file, eLogCategories.Warning, "CrawlerRequest"));
                            continue;
                        }

                        // Create All CrawlerRequest with site file path                    
                        string parameters = String.Format("<task_meta family_id=\"{0}\" module_id=\"{1}\" name=\"{2} - Dynamic Task #{1}-{3}\" site_path=\"{3}\" force_crawl=\"false\" />",
                                                                        currentCrawlerConfiguration.FamilyId,
                                                                        currentCrawlerConfiguration.ModuleDescriptionList[0].ModuleID,
                                                                        currentCrawlerConfiguration.ModuleDescriptionList[0].Description,
                                                                        file);

                        this._oLinkClient.AddDynamicTask(parameters);
                    }
                }

                this._oLinkClient.ReleaseTask(oTaskContext);
            }
            catch (Exception ex)
            {
                LogLine logLine = new LogLine(ex.Message, ex, eLogCategories.Problem, "CrawlerRequest");
                this._oLinkClient.ReleaseTaskInError(oTaskContext, logLine); 
            }
        }

        #endregion

        #region GenerateDynamicTask
        /// <summary>
        /// Generates all dynamic tasks that should be sent
        /// </summary>
        /// <param name="oTaskContext"></param>
        private void GenerateDynamicTask(TaskExecution oTaskContext)
        {
            try {

                #region Variables
                DataTable data = null;
                PluginInformation pluginInformation = null;
                Int64 staticNavSessionId = -1;
                string pdfName = string.Empty;
                Int64 loginId = -1;
                Int32 pdfResultType = -1;
                #endregion

                #region Get Data
                data = this._staticNavDAL.Get(0, PluginConfiguration.PluginsName);
                #endregion

                #region Traitments
                if (data.Rows.Count > 0) {
                    this.Log(new LogLine(String.Format(" - There are {0} alert(s) to generate", data.Rows.Count), eLogCategories.Information, "Anubis"));
                    foreach (DataRow row in data.Rows) {

                        #region Init Variables
                        staticNavSessionId = Int64.Parse(row["ID_STATIC_NAV_SESSION"].ToString());
                        pdfName = row["PDF_NAME"].ToString();
                        loginId = Int64.Parse(row["ID_LOGIN"].ToString());
                        pdfResultType = Int32.Parse(row["ID_PDF_RESULT_TYPE"].ToString());
                        pluginInformation = PluginConfiguration.GetPluginInformation(pdfResultType);
                        #endregion

                        #region Build Dynamic task
                        this.Log(new LogLine(String.Format(" - Begin : Build Dynamic Task with an ID_STATIC_NAV_SESSION = '{0}'", staticNavSessionId.ToString()), eLogCategories.Information, "Anubis"));
                        string parameters = String.Format("<task_meta family_id=\"{0}\" module_id=\"{1}\" name=\"{2} - Dynamic Task #{1}-{3}\" parameter_id=\"{3}\" ",
                                                                        pluginInformation.FamilyId.ToString(),
                                                                        pluginInformation.ResultType.ToString(),
                                                                        pluginInformation.Name,
                                                                        row["ID_STATIC_NAV_SESSION"].ToString());

                        if ((AresConst.Result.type)Enum.Parse(typeof(AresConst.Result.type), row["ID_PDF_RESULT_TYPE"].ToString()) == AresConst.Result.type.alertAdExpress)
                            parameters += String.Format("alert_id=\"{0}\"", pdfName);

                        parameters += " />";

                        this._oLinkClient.AddDynamicTask(parameters);
                        this.Log(new LogLine(String.Format(" - The task for ID_STATIC_NAV_SESSION = '{0}' is correctly add", staticNavSessionId.ToString()), eLogCategories.Information, "Anubis"));

                        #endregion

                        #region  Update Statut In Static Nav Session
                        this._staticNavDAL.UpdateStatus(staticNavSessionId, AresConst.Result.status.processing.GetHashCode());
                        this.Log(new LogLine(String.Format(" - The status for ID_STATIC_NAV_SESSION = '{0}' is correctly update to 1", staticNavSessionId.ToString()), eLogCategories.Information, "Anubis"));
                        this.Log(new LogLine(String.Format(" - End : Build Dynamic Task with an ID_STATIC_NAV_SESSION = '{0}'", staticNavSessionId.ToString()), eLogCategories.Information, "Anubis"));
                        #endregion

                    }
                }
                else
                    this.Log(new LogLine(" - There is nothing to generate", eLogCategories.Information, "Anubis"));
                #endregion
            }
            catch (Exception e) {
                sendEmailError("Error in traitment of '" + _strProductName + "'", e);
                this.Log(new LogLine("Error in GenerateDynamicTask of '" + _strProductName + "'", eLogCategories.Fatal, "Anubis"));
            }
            this._oLinkClient.ReleaseTask(oTaskContext);

        }
        #endregion

        #region GenerateTaskInError

        #region GenerateAnubisTaskInError
        /// <summary>
        /// Set up a new dynamic task with the given static nav session id
        /// </summary>
        /// <param name="oTaskContext">TaskExecution object</param>
        private void GenerateAnubisTaskInError(TaskExecution oTaskContext)
        {
            try {
                Regex xmlTask = new Regex("parameter_id=\"([^\"]+)\"");
                long staticNavSession = -1;
                if (xmlTask.IsMatch(oTaskContext.XMLTask)) {
                    Match res = xmlTask.Match(oTaskContext.XMLTask);
                    staticNavSession = long.Parse(res.Groups[1].Captures[0].Value);
                }
                else
                    this.Log(new LogLine(String.Format(" - XMLTask is probably not valid ({0})", oTaskContext.XMLTask), eLogCategories.Warning, "Anubis Error"));

                if (staticNavSession != -1) {
                    try {
                        DataRow row = this._staticNavDAL.GetRow(staticNavSession);
                        PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(int.Parse(row["ID_PDF_RESULT_TYPE"].ToString()));
                        string parameters = String.Format("<task_meta family_id=\"{0}\" module_id=\"{1}\" name=\"{2} - Dynamic Task #{1}-{3}\" parameter_id=\"{3}\" ",
                                                                                pluginInformation.FamilyId.ToString(),
                                                                                pluginInformation.ResultType.ToString(),
                                                                                pluginInformation.Name,
                                                                                row["ID_STATIC_NAV_SESSION"].ToString());

                        if ((AresConst.Result.type)Enum.Parse(typeof(AresConst.Result.type), pluginInformation.ResultType.ToString()) == AresConst.Result.type.alertAdExpress)
                            parameters += String.Format("alert_id=\"{0}\"", row["PDF_NAME"]);

                        parameters += " />";

                        this._oLinkClient.AddDynamicTask(parameters);

                        this.Log(new LogLine(" - The task was successfully created!", eLogCategories.Information, "Anubis Error"));
                        this._oLinkClient.ReleaseTask(oTaskContext);
                    }
                    catch (Exception e) {
                        LogLine error = new LogLine(e.Message, e, eLogCategories.Problem, "Anubis Error");
                        this.Log(error);
                        sendEmailError("Error in GenerateAnubisTaskInError of '" + _strProductName + "' for static_nav_session_id " + staticNavSession, e);
                        this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
                    }
                }
                else {
                    LogLine error = new LogLine(" - XML task doesn't contain a parameter id (static nav session id)", new ArgumentException("XML Task Format Error"), eLogCategories.Problem, "Anubis Error");
                    this.Log(error);
                    sendEmailError("XML Alert task doesn't contain a parameter id (static nav session id)", new ArgumentException("XML Task Format Error"));
                    this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
                }
            }
            catch (Exception e) {
                LogLine error = new LogLine(e.Message, e, eLogCategories.Problem, "Anubis Error");
                this.Log(error);
                sendEmailError("Error in GenerateAnubisTaskInError of '" + _strProductName + "'", e);
                this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
            }
        }
        #endregion

        #region GenerateAlertsTaskInError
        /// <summary>
        /// Set up a new dynamic task with the given static nav session id
        /// </summary>
        /// <param name="oTaskContext">TaskExecution object</param>
        private void GenerateAlertsTaskInError(TaskExecution oTaskContext) {
            try {
                Regex xmlTask = new Regex("alert_id=\"([^\"]+)\"");
                Int64 alertId = -1;
                if (xmlTask.IsMatch(oTaskContext.XMLTask)) {
                    Match res = xmlTask.Match(oTaskContext.XMLTask);
                    alertId = Int64.Parse(res.Groups[1].Captures[0].Value);
                }
                else
                    this.Log(new LogLine(String.Format(" - XMLTask is probably not valid ({0})", oTaskContext.XMLTask), eLogCategories.Warning, "Alerts"));

                if (alertId != -1) {

                    try {
                        DateTime today = DateTime.Now;
                        Alert.Domain.Alert alert = this._alertDAL.GetAlert(int.Parse(alertId.ToString()));
                        this._staticNavDAL.InsertData(AresConst.Result.type.alertAdExpress.GetHashCode(), alert.Title, alert.AlertId, alert.CustomerId);
                        this._oLinkClient.ReleaseTask(oTaskContext);
                    }
                    catch (Exception e) {
                        LogLine error = new LogLine(e.Message, e, eLogCategories.Problem, "Alerts");
                        this.Log(error);
                        sendEmailError("Error in GenerateAlertsTaskInError of '" + _strProductName + "' for alert_id " + alertId + "", e);
                        this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
                    }
                }
                else {
                    LogLine error = new LogLine(" - XML Alert task doesn't contain a alert_id (alert id)", new ArgumentException("XML Task Format Error"), eLogCategories.Problem, "Alerts");
                    sendEmailError("XML Alert task doesn't contain a alert_id (alert id)", new ArgumentException("XML Task Format Error"));
                    this.Log(error);
                    this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
                }
            }
            catch (Exception e) {
                LogLine error = new LogLine(e.Message, e, eLogCategories.Problem, "Alerts");
                this.Log(error);
                sendEmailError("Error in GenerateAlertsTaskInError of '" + _strProductName + "'", e);
                this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
            }
        }
        #endregion

        #endregion

        #region GenerateAlerts
        /// <summary>
        /// Generates occurrences and send email alerts
        /// </summary>
        /// <param name="oTaskContext"></param>
        private void GenerateAlerts(TaskExecution oTaskContext)
        {
            try
            {
                PluginInformation pluginInformation = null;
                DateTime today = DateTime.Now;
                AlertCollection alerts = this._alertDAL.GetAlerts((today.DayOfWeek.GetHashCode() + 6) % 7 + 1, today.Day, this.GetHourExecution(oTaskContext));
                if (alerts.Count > 0) {
                    this.Log(new LogLine(String.Format(" - There are {0} alert(s) to send", alerts.Count), eLogCategories.Information, "Alerts"));
                    foreach (Alert.Domain.Alert a in alerts) {
                        this.Log(new LogLine(String.Format(" - Begin : Build alert with an alert_id = '{0}'", a.AlertId), eLogCategories.Information, "Alerts"));
                        pluginInformation = PluginConfiguration.GetPluginInformation(PluginType.Alertes);
                        if (pluginInformation != null && pluginInformation.UseExec && pluginInformation != null
                            && pluginInformation.PluginExecList.ContainsKey(DateTime.Now.DayOfWeek)) {
                            DateTime dateExec = DateTime.Now;
                            while (dateExec.DayOfWeek != pluginInformation.PluginExecList[DateTime.Now.DayOfWeek].DayOfWeekTo) {
                                dateExec = dateExec.AddDays(1);
                            }
                            dateExec = new DateTime(dateExec.Year, dateExec.Month, dateExec.Day, pluginInformation.PluginExecList[DateTime.Now.DayOfWeek].DayOfWeekHourTo.Hours, pluginInformation.PluginExecList[DateTime.Now.DayOfWeek].DayOfWeekHourTo.Minutes, pluginInformation.PluginExecList[DateTime.Now.DayOfWeek].DayOfWeekHourTo.Seconds);
                            this.Log(new LogLine(String.Format(" - Decalage : date à decaler : {0}, date d'execution : {1}", DateTime.Now.ToString("yyyyMMdd HH:mm:ss"), dateExec.ToString("yyyyMMdd HH:mm:ss")), eLogCategories.Information, "Alerts"));
                            this._staticNavDAL.InsertData(AresConst.Result.type.alertAdExpress.GetHashCode(), a.Title, a.AlertId, a.CustomerId, dateExec);
                        }
                        else {
                            this._staticNavDAL.InsertData(AresConst.Result.type.alertAdExpress.GetHashCode(), a.Title, a.AlertId, a.CustomerId);
                        }
                        this.Log(new LogLine(String.Format(" - End : Build alert with an alert_id = '{0}'", a.AlertId), eLogCategories.Information, "Alerts"));
                    }
                }
                else {
                    this.Log(new LogLine(" - There is nothing to send", eLogCategories.Information, "Alerts"));
                }
                this._oLinkClient.ReleaseTask(oTaskContext);
            }
            catch (Exception e)
            {
                LogLine error = new LogLine(e.Message, e, eLogCategories.Problem, "Alerts");
                this.Log(error);
                sendEmailError("Error in GenerateAlerts of '" + _strProductName + "'", e);
                this._oLinkClient.ReleaseTaskInError(oTaskContext, error);
            }
        }
        #endregion

        #region GenerateGarbageStaticNavTask
        /// <summary>
        /// Generates Garbage task
        /// </summary>
        /// <param name="oTaskContext"></param>
        private void GenerateGarbageStaticNavTask(TaskExecution oTaskContext)
        {
            try {

                this.Log(new LogLine(String.Format(" - Begin : Process Garbage AdExpress on {0}", _strProductName), eLogCategories.Information, "GarbageAdExpress"));

                #region Garbage Plugins
                // Deleting static nav session's data
                this.Log(new LogLine(String.Format(" - Get All row expired in static_nav_session on {0}", _strProductName), eLogCategories.Information, "GarbageAdExpress"));
                DataTable expiredStaticNav = this._staticNavDAL.GetExpired(PluginConfiguration.PluginsName);
                if (expiredStaticNav != null && expiredStaticNav.Rows != null && expiredStaticNav.Rows.Count > 0) {
                    this.Log(new LogLine(String.Format(" - They are {1} rows expired in static_nav_session on {0}", _strProductName, expiredStaticNav.Rows.Count.ToString()), eLogCategories.Information, "GarbageAdExpress"));
                    foreach (DataRow row in expiredStaticNav.Rows) {
                        try {
                            PluginInformation pluginInformation = PluginConfiguration.GetPluginInformation(Int32.Parse(row["ID_PDF_RESULT_TYPE"].ToString()));
                            Int64 loginId = Int64.Parse(row["ID_LOGIN"].ToString());
                            string pdfName = row["PDF_NAME"].ToString();
                            Int64 staticNavSessionId = Int64.Parse(row["ID_STATIC_NAV_SESSION"].ToString());
                            DeleteRowInStaticNav(staticNavSessionId, loginId, pdfName, pluginInformation);
                        }
                        catch (Exception e) {
                            // An error occurred while deleting a row.
                            // Execution doesn't stop, but we log this as a warning
                            this.Log(new LogLine(String.Format(" - An error occured while deleting static nav session row #{0} of '{1}'", row["ID_STATIC_NAV_SESSION"], _strProductName), e, eLogCategories.Problem, "GarbageAdExpress"));
                            this.sendEmailError(String.Format("An error occured while deleting static nav session row #{0} of '{1}'", row["ID_STATIC_NAV_SESSION"], _strProductName), e);
                            this._oLinkClient.ReleaseTaskInError(oTaskContext, new LogLine(String.Format(" - An error occured while deleting static nav session row #{0} of '{1}'", row["ID_STATIC_NAV_SESSION"], _strProductName), e, eLogCategories.Problem, "GarbageAdExpress"));
                        }
                    }
                    this.Log(new LogLine(String.Format(" - End : All row expired in static nav session is treated of '{0}'", _strProductName), eLogCategories.Information, "GarbageAdExpress"));
                }
                else {
                    this.Log(new LogLine(String.Format(" - End : There is nothing static nav session to delete of '{0}'", _strProductName), eLogCategories.Information, "GarbageAdExpress"));
                }
                #endregion

                this.Log(new LogLine(String.Format(" - End : Process Garbage AdExpress on {0}", _strProductName), eLogCategories.Information, "GarbageAdExpress"));

                // Releasing task
                this._oLinkClient.ReleaseTask(oTaskContext);
            }
            catch (Exception e) {
                // A fatal error occured. Logging this
                // and releasing the task in error
                this.sendEmailError("A database error occurred while trying to delete expired requests of '" + _strProductName + "'", e);
                this._oLinkClient.ReleaseTaskInError(oTaskContext, new LogLine(" - A database error occurred while trying to delete expired requests of '" + _strProductName + "'", eLogCategories.Problem, "GarbageAdExpress"));
            }
        }
        #endregion

        #region GenerateGarbageAlerteTask
        /// <summary>
        /// Generates Garbage task
        /// </summary>
        /// <param name="oTaskContext"></param>
        private void GenerateGarbageAlerteTask(TaskExecution oTaskContext) {
            try {

                #region Garbage Alertes
                if (AlertConfiguration.AlertInformation.Delete) {
                    this.Log(new LogLine(String.Format(" - Begin : Process Garbage Alert on {0}", _strProductName), eLogCategories.Information, "GarbageAlertes"));

                    this._alertDAL.DeleteExpiredAlerts();

                    this.Log(new LogLine(String.Format(" - End : Process Garbage Alert on {0}", _strProductName), eLogCategories.Information, "GarbageAlertes"));
                }
                else {
                    this.Log(new LogLine(String.Format(" - Process Garbage Alert is Disabled on {0}", _strProductName), eLogCategories.Warning, "GarbageAlertes"));
                }
                #endregion

                #region Garbage Occurrence Alertes
                if (AlertConfiguration.OccurrenceInformation.Delete) {
                this.Log(new LogLine(String.Format(" - Get All row expired in Occurrence Alert on {0}", _strProductName), eLogCategories.Information, "GarbageAlertes"));

                this._alertDAL.DeleteExpiredAlertOccurences();

                this.Log(new LogLine(String.Format(" - End : Process Garbage Occurrence Alert on {0}", _strProductName), eLogCategories.Information, "GarbageAlertes"));
                }
                else {
                    this.Log(new LogLine(String.Format(" - Process Garbage Occurrence Alert is Disabled on {0}", _strProductName), eLogCategories.Warning, "GarbageAlertes"));
                }
                #endregion

                // Releasing task
                this._oLinkClient.ReleaseTask(oTaskContext);
            }
            catch (Exception e) {
                // A fatal error occured. Logging this
                // and releasing the task in error
                this.sendEmailError("A database error occurred while trying to delete expired requests of '" + _strProductName + "'", e);
                this._oLinkClient.ReleaseTaskInError(oTaskContext, new LogLine(" - A database error occurred while trying to delete expired requests of '" + _strProductName + "'", eLogCategories.Problem, "GarbageAlertes"));
            }
        }
        #endregion
        
        #region DoTask
        /// <summary>
        /// Do Task
        /// </summary>
        /// <param name="oObj">Context of Task</param>
        protected override void DoTask(object oObj)
        {
            TaskExecution oTaskContext = (TaskExecution)oObj;
            this.Log(new LogLine("Task received"));

            if (oTaskContext != null)
            {
                switch ((moduleId)oTaskContext.ModuleID)
                {
                    case moduleId.anubis:
                        this.Log(new LogLine(" - Starting Dynamic Tasks creation...", eLogCategories.Information, "Anubis"));
                        GenerateDynamicTask(oTaskContext);
                        break;
                    case moduleId.anubisError:
                        this.Log(new LogLine(" - Retrieving Anubis task in error...", eLogCategories.Information, "Manual"));
                        GenerateAnubisTaskInError(oTaskContext);
                        break;
                    case moduleId.alerts:
                        this.Log(new LogLine(" - Generating and sending alerts", eLogCategories.Information, "Alerts"));
                        GenerateAlerts(oTaskContext);
                        break;
                    case moduleId.alertsError:
                        this.Log(new LogLine(" - Retrieving Alert task in error...", eLogCategories.Information, "Alerts"));
                        GenerateAlertsTaskInError(oTaskContext);
                        break;
                    case moduleId.CrawlerRequest:
                        this.Log(new LogLine("CrawlerRequest - Starting Dynamic Tasks creation", eLogCategories.Information, "CrawlerRequest"));
                        CrawlerRequestGenerateDynamicTask(oTaskContext);
                        break;
                    case moduleId.garbageAdExpress:
                        this.Log(new LogLine(" - Starting Garbage AdExpress process", eLogCategories.Information, "GarbageAdExpress"));
                        GenerateGarbageStaticNavTask(oTaskContext);
                        break;
                    case moduleId.garbageAlertes:
                        this.Log(new LogLine(" - Starting Garbage Alertes process", eLogCategories.Information, "GarbageAlertes"));
                        GenerateGarbageAlerteTask(oTaskContext);
                        break;

                        
                    default:
                        LogLine warning = new LogLine(" - This module id is not implemented by the service", eLogCategories.Warning);
                        this.Log(warning);
                        this._oLinkClient.ReleaseTaskInError(oTaskContext, warning);
                        break;
                }
            }
            else
                this._oLinkClient.ReleaseTaskInError(oTaskContext, new LogLine("TaskExecution object is null", eLogCategories.Problem));
        }
        #endregion

        #region KnownModules
        /// <summary>
        /// Get Know Module List
        /// </summary>
        /// <returns></returns>
        public override List<ModuleDescription> KnownModules()
        {
            return _moduleDescriptionList;
        }
        #endregion

        #endregion

        #region Private method

        #region GetHourExecution
        /// <summary>
        /// Retrieving hour Execution from parameter string
        /// </summary>
        /// <param name="oTaskExecution">Task parameters</param>
        /// <returns>The hour Execution in second</returns>
        private Int64 GetHourExecution(TaskExecution oTaskExecution) {
            Int64 hourExecution = -1;
            StringReader sr = new StringReader(oTaskExecution.XMLTask);
            XmlTextReader reader = new XmlTextReader(sr);
            reader.Read();
            try {
                hourExecution = Int64.Parse(reader.GetAttribute("hourExecution"));
            }
            catch (Exception e) {
                this.sendEmailError("Task parameter error: no valid 'parameter_id' attribute in task definition", e);
                this._oLinkClient.ReleaseTaskInError(oTaskExecution, new LogLine("Task parameter error: no valid 'parameter_id' attribute in task definition", e, eLogCategories.Problem));
            }
            return (hourExecution);
        }
        #endregion

        #region DeleteRowInStaticNav
        /// <summary>
        /// DeleteRowInStaticNav
        /// </summary>
        /// <param name="staticNavSessionId">staticNavSessionId</param>
        /// <param name="loginId">loginId</param>
        /// <param name="pdfName">pdfName</param>
        /// <param name="pluginInformation">pluginInformation</param>
        private void DeleteRowInStaticNav(Int64 staticNavSessionId, Int64 loginId, string pdfName, PluginInformation pluginInformation) {
            this.Log(new LogLine(String.Format(" - Begin : Process for static nav session row #{0} of type '{2}' of '{1}'", staticNavSessionId.ToString(), _strProductName, pluginInformation.ResultType.ToString()), eLogCategories.Information, "GarbageAdExpress"));

            string fileName = Path.Combine(pluginInformation.FilePath, Path.Combine(loginId.ToString(), pdfName) + pluginInformation.Extension);
            if (File.Exists(fileName)) {
                this.Log(new LogLine(String.Format(" - Deleting file [{2}] of static nav session row #{0} of type '{3}' of '{1}'", staticNavSessionId.ToString(), _strProductName, fileName, pluginInformation.ResultType.ToString()), eLogCategories.Information, "GarbageAdExpress"));
                File.Delete(fileName);
            }
            else {
                this.Log(new LogLine(String.Format(" - The file [{2}] don't exist of static nav session row #{0} of type '{3}' of '{1}'", staticNavSessionId.ToString(), _strProductName, fileName, pluginInformation.ResultType.ToString()), eLogCategories.Warning, "GarbageAdExpress"));
            }

            // Deleting row
            this.Log(new LogLine(String.Format(" - Deleting row in Database of static nav session row #{0} of type '{2}' of '{1}'", staticNavSessionId.ToString(), _strProductName, pluginInformation.ResultType.ToString()), eLogCategories.Information, "GarbageAdExpress"));
            this._staticNavDAL.DeleteRow(staticNavSessionId);

            this.Log(new LogLine(String.Format(" - End : Process for static nav session row #{0} of type '{2}' of '{1}'", staticNavSessionId.ToString(), _strProductName, pluginInformation.ResultType.ToString()), eLogCategories.Information, "GarbageAdExpress"));
        }
        #endregion

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
                if(stackTrace!=null)
                    body += "<u>StackTrace:</u><br>" + stackTrace.Replace("at ", "<br>at ") + "<br>";
                body += "<hr>";
                body += "</html>";
                #endregion

                SmtpUtilities mailError = new SmtpUtilities(this.SmtpFromAddress, new ArrayList(this.ErrorEmailList.Split(';')), subject, Convertion.ToHtmlString(body), true, this.SmtpServer, this.MailPort);
                mailError.SendWithoutThread(false);
            }
        }


        #endregion
    }
}

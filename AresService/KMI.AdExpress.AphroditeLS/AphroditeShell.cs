using System;
using System.Collections.Generic;
using TNS.FrameWorks.LSConnectivity;
using TNS.Ares.Domain.LS;
using System.Diagnostics;
using TNS.LinkSystem.LinkKernel;
using KMI.AdExpress.Aphrodite.Exceptions;
using Constantes = KMI.AdExpress.Aphrodite.Constantes;
using KMI.WebTeam.Utils.XmlLoader;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.Aphrodite.Domain.XmlLoaders;
using KMI.AdExpress.Aphrodite.Domain;
using AdExpressConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.FrameWork.DB.Common.Oracle;
using TNS.FrameWork.DB.BusinessFacade.Oracle;
using KMI.AdExpress.Aphrodite;
using KMI.AdExpress.Aphrodite.Domain.Layers;
using KMI.AdExpress.Aphrodite.Domain.XmlLoader;
using System.IO;
using System.Xml;

namespace KMI.AdExpress.AphroditeLS {
    /// <summary>
    /// Aphrodite service is in charge of loading data for tendencies module
    /// </summary>
    public class AphroditeShell : Shell {

        #region Variable
        /// <summary>
        /// Aphrodite service log
        /// </summary>
        EventLog _eventLogAphrodite;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lsClientConfiguration">lsClientConfiguration</param>
        /// <param name="eventLogAphrodite">Aphrodite service log</param>
        public AphroditeShell(LsClientConfiguration lsClientConfiguration, EventLog eventLogAphrodite) :
            base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, null, lsClientConfiguration.ModuleDescriptionList) {
                _eventLogAphrodite = eventLogAphrodite;
                
        }
        #endregion

        #region DoTask
        /// <summary>
        /// Dotask
        /// </summary>
        /// <param name="oObj">task Context</param>
        protected override void DoTask(object oObj) {

            TaskExecution task = null;
            Int64 indexCurrentTask = -1;
            string messageContent = string.Empty;

            Log(new LogLine("Starting Aphrodite Service"));
            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Starting Aphrodite Service");
            task = (TaskExecution)oObj;

            try {
                if (task != null) {

                    try {

                        #region Init and check current task
                        indexCurrentTask = Int64.Parse(DateTime.Now.ToString("yyyyMMddHHmmss"));

                        if (!_oListRunningTasks.ContainsKey(indexCurrentTask))
                            _oListRunningTasks.Add(indexCurrentTask, task);
                        else {
                            Log(new LogLine("Index Current Task '" + indexCurrentTask + "' is in Execution"));
                            if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Index Current Task '" + indexCurrentTask + "' is in Execution");
                            throw new AphroditeShellException("Index Current Task '" + indexCurrentTask + "' is in Execution");
                        }
                        #endregion

                        #region Start Work
                        t_OnStartWork(indexCurrentTask, "Start Traitment for Aphrodite Service");
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Start Traitment for Aphrodite Service");
                        #endregion

                        #region Main directoty initialization
                        string configurationPathDirecory = AppDomain.CurrentDomain.BaseDirectory + Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY + @"\";
                        configurationPathDirecory += MainApplicationConfigurationPathXL.LoadDirectoryName(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory + Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY + @"\" + Constantes.Application.APPLICATION_CONFIGURATION_FILE)) + @"\";
                        #endregion

                        #region Media type description loading
                        Dictionary<AdExpressConstantes.Vehicles.names, MediaTypeInformation> mediaTypesList = MediaTypeInformationXL.Load(new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.MEDIA_TYPES_CONFIGURATION_FILE));
                        #endregion

                        DataAccessLayer dal = DataAccessLayerXL.Load(new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.DAL_CONFIGURATION_FILE));

                        #region Database initialisation
                        DataBaseConfiguration dataBaseConfiguration = DataBaseConfigurationBussinessFacade.GetOne(configurationPathDirecory + Constantes.Application.DATABASE_CONFIGURATION_FILE);
                        #endregion

                        DateTime currentDay = GetTreatmentDate(task);
                        
                        ComputeDataEngine trendsData = new ComputeDataEngine(mediaTypesList, currentDay, dataBaseConfiguration);
                        trendsData.EventChange += new ComputeDataEngine.EventChangeHandler(SendLog);
                        trendsData.ErrorEventChange += new ComputeDataEngine.ErrorEventChangeHandler(SendErrorLog);
                        trendsData.Load(_eventLogAphrodite, dal);

                        #region Stop work and send notification mail
                        t_OnStopWorkerJob(indexCurrentTask, "", "", "Aphrodite Service treatment was successfully executed.");
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Aphrodite Service treatment was successfully executed.");
                        sendEmailError("<html><body><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">Aphrodite Service treatment was successfully executed.</font><br/><br/>" + messageContent + "</body></html>");
                        #endregion

                    }
                    catch (Exception e) {
                        if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error during main treatment : " + e.Message);
                        throw new AphroditeShellException("Error during main treatment", e);
                    }
                }
                else {
                    _eventLogAphrodite.WriteEntry("The task object sent by the Link Server is null.");
                    throw new AphroditeShellException("The task object sent by the Link Server is null");
                }
            }
            catch (Exception e) {
                const string MESSAGE = "An error occurred during Aphrodite Service treatment";
                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("An error occurred during Aphrodite Service treatment : " + e.Message);
                if (task != null) {
                    if (_oListRunningTasks.ContainsKey(indexCurrentTask)) {
                        t_OnError(indexCurrentTask, MESSAGE, e);
                    }
                    else {
                        _oLinkClient.ReleaseTaskInError(task, new LogLine(MESSAGE));
                        sendEmailError("An error occurred during Aphrodite Service treatment task", e);
                    }
                }
                else {
                    Log(new LogLine(MESSAGE));
                    sendEmailError("An error occurred during Aphrodite Service treatment task", e);
                }
            }

        }
        #endregion

        #region Methods

        #region Get Treatment Date
        /// <summary>
        /// Retrieving Request Type from parameter string
        /// </summary>  
        /// <param name="oTaskExecution">Task parameters</param>
        /// <returns>Request Type</returns>
        private DateTime GetTreatmentDate(TaskExecution oTaskExecution) {

            DateTime treatmentDate = DateTime.Now;

            try {

                var sr = new StringReader(oTaskExecution.XMLTask);
                var reader = new XmlTextReader(sr);
                reader.Read();

                if (reader.GetAttribute("treatmentDate") != null && reader.GetAttribute("treatmentDate").Length > 0) {
                    string date = reader.GetAttribute("treatmentDate");
                    if (date != null && date.Length == 8)
                        treatmentDate = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
                    else
                        throw new AphroditeShellException("Error while retrieving treatment date : date format not valid");
                }

            }
            catch (Exception e) {
                if (_eventLogAphrodite != null) _eventLogAphrodite.WriteEntry("Error while retrieving request type : " + e.Message);
                throw new AphroditeShellException("Error while retrieving treatment date", e);
            }

            return (treatmentDate);
        }
        #endregion

        #region Logs
        // The method that implements the
        // delegated functionality
        private void SendLog(string message, eLogCategories logCategory) {
            Log(new LogLine(message, logCategory));
        }
        // The method that implements the
        // delegated functionality
        private void SendErrorLog(string message, Exception ex, eLogCategories logCategory) {
            Log(new LogLine(message, ex, logCategory));
        }
        #endregion

        #endregion

    }
}

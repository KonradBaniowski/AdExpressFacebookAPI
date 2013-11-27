using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWorks.LSConnectivity;
using System.Diagnostics;
using TNS.Ares.Domain.LS;
using TNS.LinkSystem.LinkKernel;
using KMI.AdExpress.PSALoader.Domain;
using TNS.FrameWork.DB.Common.Oracle;
using TNS.FrameWork.DB.BusinessFacade.Oracle;
using KMI.AdExpress.PSALoader.Exceptions;
using TNS.FrameWork.DB.Common;
using KMI.AdExpress.PSALoader.DAL;
using System.Management;
using KMI.AdExpress.PSALoader.Domain.XmlLoader;
using Microsoft.Web.Administration;
using System.IO;

namespace KMI.AdExpress.PSALoader {
    /// <summary>
    /// Aphrodite service is in charge of loading data for PSA module
    /// </summary>
    public class PSALoaderShell : Shell {

        #region Variable
        /// <summary>
        /// PSA LOader configuration
        /// </summary>
        PSALoaderConfiguration _psaLoaderConfiguration;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lsClientConfiguration">lsClientConfiguration</param>
        /// <param name="eventLogAphrodite">Aphrodite service log</param>
        public PSALoaderShell(LsClientConfiguration lsClientConfiguration) :
            base(lsClientConfiguration.ProductName, lsClientConfiguration.FamilyId, lsClientConfiguration.FamilyName, null, lsClientConfiguration.ModuleDescriptionList) {
                
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

            Log(new LogLine("Starting PSA Service"));
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
                            throw new PSALoaderShellException("Index Current Task '" + indexCurrentTask + "' is in Execution");
                        }
                        #endregion

                        #region Start Work
                        t_OnStartWork(indexCurrentTask, "Start Traitment for PSA Service");
                        #endregion

                        // Main directoty initialization
                        string configurationPathDirecory = AppDomain.CurrentDomain.BaseDirectory + Constantes.Application.APPLICATION_CONFIGURATION_DIRECTORY + @"\";

                        // Database initialisation
                        DataBaseConfiguration dataBaseConfiguration = DataBaseConfigurationBussinessFacade.GetOne(configurationPathDirecory + Constantes.Application.DATABASE_CONFIGURATION_FILE);

                        // Init visual informations object
                        VisualInformations.Init(new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.VISUAL_INFORMATION));

                        _psaLoaderConfiguration = PSALoaderConfigurationXL.Load(new XmlReaderDataSource(configurationPathDirecory + Constantes.Application.PSA_LOADER_CONFIGURATION));

                        // Load Data from XML file
                        FormInformations formInformations;
                        string[] psaDataFilePaths = GetPSAFile(Constantes.Application.DATA_FILE_DIRECTORY);

                        IDataSource source = new OracleDataSource(dataBaseConfiguration.ConnectionString);
                        List<long> formIdList = PSALoaderDAL.GetFormIdList(dataBaseConfiguration.ConnectionString);

                        foreach (string psaDataFilePath in psaDataFilePaths) {
                            
                            if (psaDataFilePath.Length > 0)
                                formInformations = KMI.AdExpress.PSALoader.Domain.XmlLoader.FormInformationsXL.Load(new XmlReaderDataSource(psaDataFilePath));
                            else
                                throw new PSALoaderShellException("The PSA data file doesn't exist");

                            BackUpPSADataFile(psaDataFilePath);

                            // Insert PSA Data
                            if (formIdList != null && formIdList.Count > 0)
                                PSALoaderDAL.InsertData(dataBaseConfiguration.ConnectionString, formInformations, formIdList);
                            else
                                throw new PSALoaderShellException("Can't retrieve form id list");

                        }

                        //RecycleWebServicePool();

                        #region Stop work and send notification mail
                        t_OnStopWorkerJob(indexCurrentTask, "", "", "PSA Service treatment was successfully executed.");
                        sendEmailError("<html><body><font color=\"#4b3e5a\" size=\"2\" face=\"Arial\">PSA Service treatment was successfully executed.</font><br/><br/>" + messageContent + "</body></html>");
                        #endregion

                    }
                    catch (Exception e) {
                        throw new PSALoaderShellException("Error during PSA loader main treatment", e);
                    }
                }
                else {
                    throw new PSALoaderShellException("The task object sent by the Link Server is null");
                }
            }
            catch (Exception e) {
                const string MESSAGE = "An error occurred during PSA Service treatment";
                if (task != null) {
                    if (_oListRunningTasks.ContainsKey(indexCurrentTask)) {
                        t_OnError(indexCurrentTask, MESSAGE, e);
                    }
                    else {
                        _oLinkClient.ReleaseTaskInError(task, new LogLine(MESSAGE));
                        sendEmailError("An error occurred during PSA Service treatment task", e);
                    }
                }
                else {
                    Log(new LogLine(MESSAGE));
                    sendEmailError("An error occurred during PSA Service treatment task", e);
                }
            }

        }
        #endregion

        #region Get PSA file
        /// <summary>
        /// Get PSA file
        /// </summary>
        /// <param name="folder">Location of PSA data file</param>
        /// <returns>Path for PSA file data</returns>
        private string[] GetPSAFile(string folder) {

            string[] files = Directory.GetFiles(folder, "creation_KANTAR_PSA_*");

            return files;

        }
        #endregion

        private void BackUpPSADataFile(string filePath) {

            string fileName = Path.GetFileNameWithoutExtension(filePath);
            string destFilePath = Constantes.Application.DATA_FILE_DIRECTORY + "Historique\\" + fileName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xml";
            System.IO.File.Move(filePath, destFilePath);
        }

        #region Recycle Web Service Pool
        /// <summary>
        /// Recycle Web Service Pool
        /// </summary>
        private void RecycleWebServicePool() {
            
            try {

                    /*ServerManager manager= ServerManager.OpenRemote("frmitch-ws003");
                    var pool = manager.ApplicationPools["PsaDispacherWebService"];
                    pool.Recycle();*/

                ConnectionOptions co = new ConnectionOptions();
                /*co.Username = _psaLoaderConfiguration.Login;
                co.Password = _psaLoaderConfiguration.Password;*/
                co.Username = "tnsad\adm.guillaume.facon";
                co.Password = "Sandie.1";
                co.Impersonation = ImpersonationLevel.Impersonate;
                co.Authentication = AuthenticationLevel.PacketPrivacy;
                /*string objPath = "IISApplicationPool.Name='W3SVC/AppPools/" + _psaLoaderConfiguration.AppPool + "'";
                ManagementScope scope = new ManagementScope(@"\\" + _psaLoaderConfiguration.ServerName + @"\root\MicrosoftIISV2", co);*/

                string objPath = "IISApplicationPool.Name='W3SVC/AppPools/PsaDispacherWebService'";
                ManagementScope scope = new ManagementScope(@"\\frmitch-ws003\root\MicrosoftIISV2", co);

                using (ManagementObject mc = new ManagementObject(objPath)) {
                    mc.Scope = scope;
                    mc.Scope.Connect();
                    mc.InvokeMethod("Recycle", null, null);
                }
                
            }
            catch (System.Exception err) {
                throw new System.Exception(" Impossible to recycle the Pool", err);
            }

        }
        #endregion

    }
}

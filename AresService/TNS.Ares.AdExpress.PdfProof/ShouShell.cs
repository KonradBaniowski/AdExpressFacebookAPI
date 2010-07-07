using System;
using System.Collections.Generic;
using System.Text;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Shou;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
using TNS.Ares;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;

namespace TNS.Ares.AdExpress.PdfProof
{
    public class ShouShell : AdExpressShell
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="source">DataSource</param>
        /// <param name="confFile">Path Configuration File</param>
        public ShouShell(string productName, int familyId, List<ModuleDescription> moduleDescriptionList, string directoryName) :
            base(productName, familyId, moduleDescriptionList, directoryName)
        {
        }
        #endregion

        #region DoTask
        /// <summary>
        /// Do task
        /// </summary>
        /// <param name="oObj">task Execution</param>
        protected override void DoTask(object oObj)
        {
            TaskExecution task = (TaskExecution)oObj;
            try {
                if (task != null) {
                    // Extracting parameter id from task
                    long staticNavSession = extractParameterId(task);
                    if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                        _oListRunningTasks.Add(staticNavSession, task);
                        // Preparing treatment
                        TreatementSystem t = new TreatementSystem();
                        t.OnStartWork += new TNS.Ares.StartWork(t_OnStartWork);
                        t.OnError += new TNS.Ares.Error(t_OnError);
                        t.OnStopWorkerJob += new TNS.Ares.StopWorkerJob(t_OnStopWorkerJob);
                        t.OnSendReport += new TNS.Ares.SendReport(t_OnSendReport);
                        t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessageAlert);
                        t.Treatement(this._confFile, this._source, staticNavSession);
                }
                else {
                    this._oLinkClient.ReleaseTaskInError(task, new LogLine("TaskExecution object is null", eLogCategories.Problem));
                }
            }
            catch (Exception e) {
                this.sendEmailError("Build Task Error in Shell in DoTask(object oObj)", e);
                this._oLinkClient.ReleaseTaskInError(task, new LogLine("Build Task Error in Shell in DoTask(object oObj)"));
            }

        }
        #endregion

    }
}

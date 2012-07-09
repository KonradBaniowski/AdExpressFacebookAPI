using System;
using System.Collections.Generic;
using System.Text;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Aton;
using System.Collections;
//using TNS.AdExpress.Anubis.BusinessFacade.Result;
using TNS.Ares;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
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

namespace TNS.Ares.AdExpress.PdfCadrage
{
    public class AtonShell : AdExpressShell
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="source">DataSource</param>
        /// <param name="confFile">Path Configuration File</param>
        public AtonShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
            base(lsClientConfiguration, directoryName)
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
            long staticNavSessionId = -1;
            try {
                if (task != null) {
                    // Extracting parameter id from task
                    long staticNavSession = extractParameterId(task);
                    if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                        _oListRunningTasks.Add(staticNavSession, task);
                        // Preparing treatment
                        TreatementSystem t = new TreatementSystem();
                        t.EvtStartWork += t_OnStartWork;
                        t.EvtError += t_OnError;
                        t.EvtStopWorkerJob += t_OnStopWorkerJob;
                        t.EvtSendReport += t_OnSendReport;
                        t.EvtMessageAlert += t_OnMessage;
                        t.Treatement(_confFile, _source, staticNavSession);
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

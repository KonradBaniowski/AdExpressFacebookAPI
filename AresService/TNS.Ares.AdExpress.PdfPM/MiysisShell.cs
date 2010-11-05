using System;
using System.Collections.Generic;
using System.Text;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Miysis;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
using TNS.Ares;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.Ares.Domain.DataBase;
using TNS.Ares.Domain.LS;
using TNS.Ares.Constantes;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Net.Mail;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using System.IO;
using TNS.Ares.AdExpress.Exceptions;

namespace TNS.Ares.AdExpress.PdfPM
{
    public class MiysisShell : AdExpressShell
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="source">DataSource</param>
        /// <param name="confFile">Path Configuration File</param>
        public MiysisShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
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
            long staticNavSession = -1;
            TreatementSystem t = null;
            try {
                if (task == null) throw new ShellException("TaskExecution object is null");
                // Extracting parameter id from task
                staticNavSession = extractParameterId(task);
                if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                    _oListRunningTasks.Add(staticNavSession, task);
                // Preparing treatment
                t = new TreatementSystem();
                t.OnStartWork += new TNS.Ares.StartWork(t_OnStartWork);
                t.OnError += new TNS.Ares.Error(t_OnError);
                t.OnStopWorkerJob += new TNS.Ares.StopWorkerJob(t_OnStopWorkerJob);
                t.OnSendReport += new TNS.Ares.SendReport(t_OnSendReport);
                t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessage);
                t.Treatement(this._confFile, this._source, staticNavSession);
            }
            catch (Exception e) {
                if (staticNavSession > 0) {
                    t_OnError(staticNavSession, "Build Task Error in Shell in DoTask(object oObj) for id '" + staticNavSession.ToString() + "'", e);
                }
            }
        }
        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.LinkSystem.LinkKernel;
using TNS.Ares.AdExpress.Exceptions;
using TNS.AdExpress.Anubis.Selket;
using TNS.Ares.Domain.LS;

namespace TNS.Ares.AdExpress.PdfVpFile
{
    public class SelketShell : AdExpressShell
    {
         #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="productName">Product Name</param>
        /// <param name="familyId">Family Id</param>
        /// <param name="source">DataSource</param>
        /// <param name="confFile">Path Configuration File</param>
        public SelketShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
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
                t.EvtStartWork += t_OnStartWork;
                t.EvtError += t_OnError;
                t.EvtStopWorkerJob += t_OnStopWorkerJob;
                t.EvtSendReport += t_OnSendReport;
                t.EvtMessageAlert += t_OnMessage;
                t.Treatement(_confFile, _source, staticNavSession);
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

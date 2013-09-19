﻿using System;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Apis;
using TNS.Ares.Domain.LS;
using TNS.Ares.AdExpress.Exceptions;

namespace TNS.Ares.AdExpress.PdfCelebrities {
    /// <summary>
    /// Celebrities Shell
    /// </summary>
    public class CelebritiesShell : AdExpressShell {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lsClientConfiguration">lsClientConfiguration</param>
        /// <param name="directoryName">directoryName</param>
        public CelebritiesShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
            base(lsClientConfiguration, directoryName) {
        }
        #endregion

        #region DoTask
        /// <summary>
        /// Do task
        /// </summary>
        /// <param name="oObj">task Execution</param>
        protected override void DoTask(object oObj) {

            var task = (TaskExecution)oObj;
            long staticNavSession = -1;
            try
            {
                if (task == null) throw new ShellException("TaskExecution object is null");
                // Extracting parameter id from task
                staticNavSession = extractParameterId(task);
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
            catch (Exception e)
            {
                if (staticNavSession > 0)
                {
                    t_OnError(staticNavSession, "Build Task Error in Shell in DoTask(object oObj) for id '" + staticNavSession.ToString() + "'", e);
                }
            }
        }
        #endregion

    }
}
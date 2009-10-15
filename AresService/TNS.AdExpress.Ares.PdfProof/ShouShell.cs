using System;
using System.Collections.Generic;
using System.Text;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Shou;
using System.Collections;
using TNS.FrameWork.DB.Common;
using TNS.FrameWorks.LSConnectivity;
using TNS.Ares;

namespace TNS.AdExpress.Ares.PdfProof
{
    public class ShouShell : Shell
    {
        #region Variables

        private string _confFile;
        private IDataSource _source;

        #endregion

        public ShouShell(string productName, int familyId, string confFile, IDataSource source)
            : base(productName, familyId, null, 30000)
        {
            this._confFile = confFile;
            this._source = source;
        }

        protected override void DoTask(object oObj)
        {
            TaskExecution oTaskContext = (TaskExecution)oObj;
            long staticNavSessionId = -1;

            if (oTaskContext != null)
            {
                staticNavSessionId = getStaticNavSessionId(oTaskContext);
                if (staticNavSessionId != -1)
                {
                    lock (_oObjectLocker)
                    {
                        _oListRunningTasks.Add(staticNavSessionId, oTaskContext);
                    }

                    TreatementSystem t = new TreatementSystem();
                    t.OnStartWork += new TNS.Ares.StartWork(t_OnStartWork);
                    t.OnError += new TNS.Ares.Error(t_OnError);
                    t.OnStopWorkerJob += new TNS.Ares.StopWorkerJob(t_OnStopWorkerJob);
                    t.OnSendReport += new TNS.Ares.SendReport(t_OnSendReport);
                    t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessageAlert);
                    t.Treatement(this._confFile, this._source, staticNavSessionId);
                }
            }
            else
                this._oLinkClient.ReleaseTaskInError(oTaskContext, new LogLine("TaskExecution object is null", eLogCategories.Problem));

        }

        protected override void oLinkClient_evtStateChanged(eClientState eState, Exception oError)
        {
            base.oLinkClient_evtStateChanged(eState, oError);
            if (eState == eClientState.Connected && this._oMonitorServer != null)
                this._oMonitorServer.Log(new LogLine("Shou request treatment is ready to receive requests", eLogCategories.Information));
        }


        public override List<ModuleDescription> KnownModules()
        {
            return new List<ModuleDescription>()
            {
                new ModuleDescription("Shou request treatment", 9)
            };
        }

        #region Plugin event callbacks

        protected void t_OnSendReport(string reportTitle, TimeSpan duration, DateTime endExecutionDateTime, string reportCore, ArrayList mailList, ArrayList errorList, string from, string mailServer, int mailPort, long navSessionId)
        {
            ReportingSystem reportingSystem = new ReportingSystem(reportTitle, duration, endExecutionDateTime, reportCore, mailList, errorList, from, mailServer, mailPort, navSessionId);
            try
            {
                reportingSystem.SendReport(reportingSystem.SetReport());
            }
            catch (System.Exception ex)
            {
                this._oLinkClient.ReleaseTaskInError(_oListRunningTasks[navSessionId], new LogLine(ex.Message, ex, eLogCategories.Warning));
            }
        }

        #endregion
    }
}

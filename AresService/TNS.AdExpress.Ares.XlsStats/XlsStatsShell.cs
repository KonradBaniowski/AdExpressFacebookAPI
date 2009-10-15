using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWorks.LSConnectivity;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Bastet;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Ares.XlsStats
{
    public class XlsStatsShell : Shell
    {
        private IDataSource _source;
        private string _confFile;

        public XlsStatsShell(string productName, int familyId, IDataSource source, string confFile) :
            base(productName, familyId)
        {
            this._confFile = confFile;
            this._source = source;
        }

        protected override void DoTask(object oObj)
        {
            TaskExecution task = (TaskExecution)oObj;

            if (task != null)
            {
                // Extracting parameter id from task
                long staticNavSession = extractParameterId(task);
                if (_oListRunningTasks.ContainsKey(staticNavSession) == false)
                    _oListRunningTasks.Add(staticNavSession, task);

                // Preparing treatment
                TreatementSystem t = new TreatementSystem();
                t.OnStartWork += new TNS.Ares.StartWork(t_OnStartWork);
                t.OnError += new TNS.Ares.Error(t_OnError);
                t.OnStopWorkerJob += new TNS.Ares.StopWorkerJob(t_OnStopWorkerJob);
                t.OnMessageAlert += new TNS.Ares.MessageAlert(t_OnMessageAlert);
                t.Treatement(this._confFile, this._source, staticNavSession);
            }
            else
                this._oLinkClient.ReleaseTaskInError(task, new LogLine("Link system failure: the given task definition is not correct"));
        }

        public override List<ModuleDescription> KnownModules()
        {
            return new List<ModuleDescription>
            {
                new ModuleDescription("Ares XlsStats Treatment", 2)
            };
        }
    }
}

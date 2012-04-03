using System;
using System.IO;
using TNS.LinkSystem.LinkKernel;
using TNS.AdExpress.Anubis.Hotep;
using TNS.Ares.Domain.LS;
using System.Reflection;

namespace TNS.Ares.AdExpress.PdfSectorielle
{
    public class HotepShell : AdExpressShell
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="lsClientConfiguration">Ls ClientConfiguration</param>
        /// <param name="directoryName">Directory Name</param>
        public HotepShell(LsClientConfiguration lsClientConfiguration, string directoryName) :
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
                        PluginInformation pInf = PluginConfiguration.GetPluginInformation(PluginType.Hotep);
                        if (pInf.AssemblyName == null) throw (new ArgumentNullException("AssemblyName layer is null for the Indicator result"));
                     TreatementSystem t = (TreatementSystem)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, pInf.AssemblyName), pInf.Class_, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null);
                     t.EvtStartWork += t_OnStartWork;
                        t.EvtError += t_OnError;
                        t.EvtStopWorkerJob += t_OnStopWorkerJob;
                        t.EvtSendReport += t_OnSendReport;
                        t.EvtMessageAlert += t_OnMessage;
                        t.Treatement(_confFile, _source, staticNavSession);
                }
                else {
                    _oLinkClient.ReleaseTaskInError(task, new LogLine("TaskExecution object is null", eLogCategories.Problem));
                }
            }
            catch (Exception e) {
                sendEmailError("Build Task Error in Shell in DoTask(object oObj)", e);
                _oLinkClient.ReleaseTaskInError(task, new LogLine("Build Task Error in Shell in DoTask(object oObj)"));
            }

        }
        #endregion

    }
}

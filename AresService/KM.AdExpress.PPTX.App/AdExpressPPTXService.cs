using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace KM.AdExpress.PPTX.App
{
    partial class AdExpressPPTXService : ServiceBase
    {
        public AdExpressPPTXService()
        {
            InitializeComponent();
        }

        #region Variables
        /// <summary>
        /// Is Stopping
        /// </summary>
        private bool _isStopping;
        /// <summary>
        /// Current Thread.
        /// </summary>
        private Thread _currentThread;
        /// <summary>
        /// Current Shell.
        /// </summary>
        private Engine _currentShell;

        #endregion


        #region Public Methods
        public void StartManual(string[] args)
        {
            OnStart(null);
        }

        public void StopManual() { OnStop(); }

        public void StartService() { OnStart(null); }

        public void StopService() { OnStop(); }

        #endregion

        protected override void OnStart(string[] args)
        {
            _currentThread = new Thread(Run);
            _currentThread.Start();
        }

        protected override void OnStop()
        {
            if (_currentShell != null && !_isStopping)
            {
                _isStopping = true;
                _currentThread.Join(10000);
                _currentShell.Dispose();
                _currentShell = null;
                _isStopping = false;
            }
        }

        #region Run
        /// <summary>
        /// Run
        /// </summary>
        protected virtual void Run()
        {

            try
            {
                _currentShell = new Engine();

                while (!_isStopping)
                    Thread.Sleep(500);
            }
            catch
            {
                OnStop();
            }
        }
        #endregion
    }
}

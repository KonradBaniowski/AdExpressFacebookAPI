using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.ServiceProcess;
using System.Text;

namespace TNS.Ares.AdExpress.XlsCadrageSrv
{
    public partial class XlsCadrageService : ServiceBase
    {
        public XlsCadrageService()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
        }

        protected override void OnStop()
        {
        }
    }
}

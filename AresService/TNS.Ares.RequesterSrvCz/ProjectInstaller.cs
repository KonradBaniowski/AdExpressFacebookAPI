using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;
using System.Linq;


namespace TNS.Ares.RequesterSrvCz
{
    [RunInstaller(true)]
    public partial class serviceInstallerAdExpress : Installer
    {
        public serviceInstallerAdExpress()
        {
            InitializeComponent();
        }
    }
}

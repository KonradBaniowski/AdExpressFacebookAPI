using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration.Install;


namespace TNS.Ares.AdExpress.PdfChronoSrv {
    [RunInstaller(true)]
    public partial class ProjectInstaller : Installer {
        public ProjectInstaller() {
            InitializeComponent();
        }
    }
}

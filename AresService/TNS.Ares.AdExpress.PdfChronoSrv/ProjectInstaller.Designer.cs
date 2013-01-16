namespace TNS.Ares.AdExpress.PdfChronoSrv {
    partial class ProjectInstaller {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing) {
            if (disposing && (components != null)) {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {
            this.serviceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.aresPdfChronoServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // serviceProcessInstaller
            // 
            this.serviceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.serviceProcessInstaller.Password = null;
            this.serviceProcessInstaller.Username = null;
            // 
            // aresPdfChronoServiceInstaller
            // 
            this.aresPdfChronoServiceInstaller.Description = "Generates All result of Chronopress in Pdf";
            this.aresPdfChronoServiceInstaller.DisplayName = "Ares Pdf Chrono Service (Appm)";
            this.aresPdfChronoServiceInstaller.ServiceName = "Ares Pdf Chrono Service (Appm)";
            this.aresPdfChronoServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.serviceProcessInstaller,
            this.aresPdfChronoServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller serviceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller aresPdfChronoServiceInstaller;
    }
}
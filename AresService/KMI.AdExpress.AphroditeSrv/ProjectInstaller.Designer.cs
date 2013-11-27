namespace KMI.AdExpress.AphroditeSrv {
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
            this.aphroditeServiceInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.aphroditeService = new System.ServiceProcess.ServiceInstaller();
            // 
            // aphroditeServiceInstaller
            // 
            this.aphroditeServiceInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.aphroditeServiceInstaller.Password = null;
            this.aphroditeServiceInstaller.Username = null;
            // 
            // aphroditeService
            // 
            this.aphroditeService.ServiceName = "Aphrodite Service";
            this.aphroditeService.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.aphroditeServiceInstaller,
            this.aphroditeService});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller aphroditeServiceInstaller;
        private System.ServiceProcess.ServiceInstaller aphroditeService;
    }
}
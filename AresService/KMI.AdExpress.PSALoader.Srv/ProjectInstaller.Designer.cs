namespace KMI.AdExpress.PSALoader.Srv {
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
            this.PSALoaderProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.PSALoaderInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // PSALoaderProcessInstaller
            // 
            this.PSALoaderProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.PSALoaderProcessInstaller.Password = null;
            this.PSALoaderProcessInstaller.Username = null;
            // 
            // PSALoaderInstaller
            // 
            this.PSALoaderInstaller.Description = "PSA Data Loader";
            this.PSALoaderInstaller.ServiceName = "PSALoader";
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.PSALoaderProcessInstaller,
            this.PSALoaderInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller PSALoaderProcessInstaller;
        private System.ServiceProcess.ServiceInstaller PSALoaderInstaller;
    }
}
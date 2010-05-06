namespace TNS.Ares.AdExpress.XlsCadrageSrv
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Nettoyage des ressources utilisées.
        /// </summary>
        /// <param name="disposing">true si les ressources managées doivent être supprimées ; sinon, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Code généré par le Concepteur de composants

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.aresXlsCadrageProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.aresXlsCadrageInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // aresXlsCadrageProcessInstaller
            // 
            this.aresXlsCadrageProcessInstaller.Password = null;
            this.aresXlsCadrageProcessInstaller.Username = null;
            // 
            // aresXlsCadrageInstaller
            // 
            this.aresXlsCadrageInstaller.Description = "AdExpress Xls Cadrage Chronopresse generation service";
            this.aresXlsCadrageInstaller.DisplayName = "Ares XlsCadrage Service (Amset)";
            this.aresXlsCadrageInstaller.ServiceName = "Ares XlsCadrage Service (Amset)";
            this.aresXlsCadrageInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.aresXlsCadrageProcessInstaller,
            this.aresXlsCadrageInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller aresXlsCadrageProcessInstaller;
        private System.ServiceProcess.ServiceInstaller aresXlsCadrageInstaller;
    }
}
namespace KMI.AdExpress.AphroditeSrv {
    partial class AphroditeService {

        #region Variables
        /// <summary> 
        /// Variable nécessaire au concepteur.
        /// </summary>
        private System.ComponentModel.IContainer components = null;
        /// <summary>
        /// Event log Aphrodite
        /// </summary>
        private System.Diagnostics.EventLog eventLogAphrodite;
        #endregion

        #region Nettoyage des ressources utilisées
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
        #endregion

        #region Code généré par le Concepteur de composants
        /// <summary> 
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas 
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent() {

            components = new System.ComponentModel.Container();

            this.eventLogAphrodite = new System.Diagnostics.EventLog();
            
            if (!System.Diagnostics.EventLog.SourceExists("AphroditeLog"))
                System.Diagnostics.EventLog.CreateEventSource("AphroditeLog", "AphroditeLog");
            
            this.eventLogAphrodite.Source = "AphroditeLog";
            // the event log source by which 
            //the application is registered on the computer
            this.eventLogAphrodite.Log = "AphroditeLog";

            ((System.ComponentModel.ISupportInitialize)(this.eventLogAphrodite)).BeginInit();

            // Aphrodite Service
            this.ServiceName = "Aphrodite Service";
            this.AutoLog = true;

            ((System.ComponentModel.ISupportInitialize)(this.eventLogAphrodite)).EndInit();
        }
        #endregion
    }
}

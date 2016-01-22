namespace TNS.AdExpress.Rolex.Loader.Domain
{
    /// <summary>
    /// Classe qui contient la configuration de Rolex
    /// </summary>
   public class RolexConfiguration
    {
        #region Accesseurs

        /// <summary>
        /// Obtient ou défini le serveur des mails de résultats
        /// </summary>
        public string CustomerMailServer { get; set; }

        /// <summary>
        /// Obtient ou défini le port du serveur des mails des résultats
        /// </summary>
        public int CustomerMailPort { get; set; }

        /// <summary>
        /// Obtient ou défini le mail d'envoi des résultats 
        /// </summary>
        public string CustomerMailFrom { get; set; }

        /// <summary>
        /// Obtient ou défini le chemin du fichier de configuration
        /// </summary>
        public string ConfigurationReportFilePath { get; set; }

        /// <summary>
        /// Obtient ou défini le nom du serveur
        /// </summary>
        public string Server { get; set; }

        public string Recipients
        {
            get;
            set;
        }

        /// <summary>
        /// Obtient ou défini le login
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// Obtient ou défini le mot de passe
        /// </summary>
        public string Password { get; set; }


        /// <summary>
        /// Get /Set Connection String
        /// </summary>
        public string ConnectionString { get; set; }
    
        /// <summary>
        /// Get  /Set Data access provider
        /// </summary>
        public string ProviderDataAccess { get; set; }

       /// <summary>
       /// Visual path
       /// </summary>
        public string ViualPath { get; set; }

        #endregion
    }
}

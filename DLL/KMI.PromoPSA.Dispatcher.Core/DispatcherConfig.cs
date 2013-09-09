using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Dispatcher.Core
{
   public  class DispatcherConfig
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
       

        public ArrayList Recipients
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

        #endregion
    }
}

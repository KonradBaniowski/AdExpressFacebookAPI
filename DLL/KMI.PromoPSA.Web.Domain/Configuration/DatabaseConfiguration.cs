using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Web.Domain.Configuration
{
 public   class DatabaseConfiguration
    {
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

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebServiceSetAdScopeLogin.Constantes
{
    #region Gestion d'erreur
    /// <summary>
    /// Constantes de gestion d'erreur
    /// </summary>
    public class ErrorManager
    {

        #region Constantes
       
        /// <summary>
        /// Chemin du fichier de configuration du mail des erreurs du serveur
        /// </summary>
        public const string WEBSERVER_ERROR_MAIL_FILE = @"ErrorMail.xml";
       
        #endregion

      


    }
    #endregion

    #region Fichier configuration
   
    /// <summary>
    /// Constantes de gestion d'erreur
    /// </summary>
    public class ConfigFile
    {

        #region Constantes
       
     
        /// <summary>
        /// Database CONFIGURATION FILE NAME
        /// </summary>
        public const string CONFIGURATION_DATABASE_FILENAME = "Database.xml";
        #endregion

        #region Constante
        /// <summary>
        /// Name of the configuration directory
        /// </summary>
        public const string CONFIGURATION_DIRECTORY_NAME = "Configuration";
        #endregion


    }
 
    #endregion
}
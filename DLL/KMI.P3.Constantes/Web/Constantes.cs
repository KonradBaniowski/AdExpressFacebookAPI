using System;
using System.Collections.Generic;
using System.Text;

namespace KMI.P3.Constantes.Web
{
    #region Gestion d'erreur
    /// <summary>
    /// Constantes de gestion d'erreur
    /// </summary>
    public class ErrorManager
    {

        #region Constantes
        /// <summary>
        /// Chemin du fichier de configuration du mail des erreurs client
        /// </summary>
        public const string CUSTOMER_ERROR_MAIL_FILE = @"CustomerErrorMail.xml";
        /// <summary>
        /// Chemin du fichier de configuration du mail des erreurs du serveur
        /// </summary>
        public const string WEBSERVER_ERROR_MAIL_FILE = @"ErrorMail.xml";
        #endregion

    }
    #endregion

    #region URL
    /// <summary>
    /// Constantes de gestion d'erreur
    /// </summary>
    public class URL
    {

        #region Constantes
        /// <summary>
        /// Home page url
        /// </summary>
        public const string HOME_URL = @"Home.aspx";
        /// <summary>
        /// AdExpress page url
        /// </summary>
        public const string ADEXPRESS_URL = @"http://assouan/Private/P3.aspx";
        /// <summary>
        /// Adscope page url
        /// </summary>
        public const string ADSCOPE_URL = @"http://www.adscope.fr/site/identification_FR.jsp"; 
        #endregion

    }
    #endregion

    #region Core
    /// <summary>
    /// Constantes de relatives aux noyau d'EasyMusic
    /// </summary>
    public class Core
    {
        /// <summary>
        /// Clés des paramètres enregistrés dans une session cliente
        /// </summary>
        public enum SessionParamters
        {
            /// <summary>
            /// Data Source
            /// </summary>
            dataSource = 1,
            /// <summary>
            /// Session Id
            /// </summary>
            sessionId = 2,
            /// <summary>
            /// Site language
            /// </summary>
            siteLanguage = 3,
            /// <summary>
            /// Beginning Date
            /// </summary>
            beginningDate = 4,                                
            /// <summary>
            /// Login Id
            /// </summary>
            loginId = 5,
            /// <summary>
            /// Right Data Source
            /// </summary>
            rightDataSource = 6,                   
            /// <summary>
            /// Customer navigator information
            /// </summary>
            browser = 7,
            /// <summary>
            /// Customer navigator version information
            /// </summary>
            browserVersion = 8,
            /// <summary>
            /// Customer navigator user agent information
            /// </summary>
            userAgent = 9,
            /// <summary>
            /// Customer Os
            /// </summary>
            customerOs = 10,
            /// <summary>
            /// Customer Ip
            /// </summary>
            customerIp = 11,
            /// <summary>
            /// last url set un WebPage (P3 base page) use for customer erro
            /// </summary>
            lastWebPage = 12,
            /// <summary>
            /// Server Name
            /// </summary>
            serverName = 13,
            /// <summary>
            /// Modification date
            /// </summary>
            modificationDate =14
          
        }
    }
    #endregion
}

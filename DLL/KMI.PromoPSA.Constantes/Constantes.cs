using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace KMI.PromoPSA.Constantes
{
    public class Constantes
    {
        public static long NO_USER_VALUE = 0;

        public static long ACTIVATION_CODE_TO_CODIFY = 40;

        public static long ACTIVATION_CODE_INACTIVE = 50;

        public static long ACTIVATION_CODE_CODIFIED = 20;

        public static long ACTIVATION_CODE_REJECTED = 30;

        public static long ACTIVATION_CODE_WAITING = 10;

        public static long ACTIVATION_CODE_VALIDATED = 0;

    }

    #region WebSession
    /// <summary>
    /// Constantes de session
    /// </summary>
    public class WebSession
    {
        /// <summary>
        /// Langue d'affichage du site
        /// </summary>
        public const string WEB_LANGUAGE_ID = "WebLanguageId";
        /// <summary>
        /// Session
        /// </summary>
        public const string WEB_SESSION = "WebSession";
    }
    /// <summary>
    /// Constantes de relatives aux noyau d'AdExpress 3.0 
    /// </summary>
    public class Core
    {
        /// <summary>
        /// Clés des paramètres enregistrés dans une session cliente
        /// </summary>
        public enum SessionParamters
        {
            /// <summary>
            /// Clé d'accès à l'ordre de tri
            /// </summary>
            sortOrderKey = 16,
            /// <summary>
            /// Clé d'accès à la clé identifiant la colonne à trier.
            /// </summary>
            sortKeyKey = 17
        }

    }
    #endregion

    #region Configuration
    /// <summary>
    /// Configuration Class
    /// </summary>
    public class Configuration
    {
        /// <summary>
        /// Path directory of configuration files
        /// </summary>
        public const string DIRECTORY_CONFIGURATION = @"Configuration\";
        /// <summary>
        /// Parameter file name
        /// </summary>
        public const string FILE_PARAMETER = @"Parameter.xml";
        /// <summary>
        /// AdvertCollection file name
        /// </summary>
        public const string FILE_ADVERTCOLLECTION = @"AdvertCollection.xsd";
        /// <summary>
        /// Database file name
        /// </summary>
        public const string FILE_DATABASE = @"Database.xml";
        /// <summary>
        /// Vehicles file name
        /// </summary>
        public const string FILE_VEHICLES = @"Vehicles.xml";
        /// <summary>
        /// Vehicles file name
        /// </summary>
        public const string FILE_MEDIA_STATUTS = @"MediaStatuts.xml";
        /// <summary>
        /// Error mail file name
        /// </summary>
        public const string FILE_ERROR_MAIL = @"ErrorMail.xml";
        /// <summary>
        /// Fichier de configuration des Css
        /// </summary>
        public const string FILE_CSS_CONFIGURATION = @"CssConfiguration.xml";
        /// <summary>
        /// Web Languages configuration file
        /// </summary>
        public const string FILE_WEBLANGUAGES_CONFIGURATION = @"WebLanguages.xml";
        /// <summary>
        /// Web themes configuration file
        /// </summary>
        public const string FILE_WEBTHEMES_CONFIGURATION = @"WebThemes.xml";
        /// <summary>
        /// Web parameters configuration file
        /// </summary>
        public const string FILE_WEBPARAMETERS_CONFIGURATION = @"WebParameters.xml";
        /// <summary>
        /// Menu configuration file
        /// </summary>
        public const string MENU_CONFIGURATION = @"Menu.xml";
        /// <summary>
        /// WebServices configuration file
        /// </summary>
        public const string WEBSERVICE_CONFIGURATION = @"WebServices.xml";
        /// <summary>
        /// Option Advert configuration file
        /// </summary>
        public const string OPTION_ADVERT_CONFIGURATION = @"OptionAdvert.xml";
        /// <summary>
        /// Option Advert configuration file
        /// </summary>
        public const string VEHICLES_CONFIGURATION = @"Vehicles.xml";
        /// <summary>
        /// Option Advert configuration file
        /// </summary>
        public const string MEDIA_STATUS_INFORMATION_CONFIGURATION = @"MediaStatusInformations.xml";
        /// <summary>
        /// Option Advert configuration file
        /// </summary>
        public const string STATUS_INFORMATION_CONFIGURATION = @"StatusInformations.xml";
        /// <summary>
        /// WebServices configuration file
        /// </summary>
        public const string AJAX_CONFIGURATION = @"AjaxParameter.xml";
        /// <summary>
        /// Data Access Layer file name
        /// </summary>
        public const string FILE_DATA_ACCESS_LAYERS = @"DataAccessLayers.xml";
        /// <summary>
        /// Maximum value of width thumbnail
        /// </summary>
        public const int THUMBNAIL_MAX_WIDTH = 145;
        /// <summary>
        /// Maximum value of height thumbnail
        /// </summary>
        public const int THUMBNAIL_MAX_HEIGHT = 85;
        /// <summary>
        /// Maximum de ligne pour la requete de recherche produit
        /// </summary>
        public const int PRODUCT_MAX_ROWS = 500;
        /// <summary>
        /// Maximum de ligne pour l'affichage de liens
        /// </summary>
        public const int NB_ADVERT_PAGE_URL_LIST_MAX = 2;
        /// <summary>
        /// Maximum de ligne pour l'affichage de liens
        /// </summary>
        public const int NB_ADVERT_BY_PAGE = 15;
        /// <summary>
        /// Maximum de ligne pour l'affichage de liens
        /// </summary>
        public const int NB_PRODUCT_DETAIL_BY_PAGE = 50;
        /// <summary>
        /// Valeur du nom de fichier de la page de destination lorsque le fichier n'existe pas
        /// </summary>
        public const string PAGE_DEST_NO_FILE = "no_file.jpg";
        /// <summary>
        /// Nombre de jour visualiser dans la page de verification
        /// </summary>
        public const int CHECKING_NB_DAY_VIEW = 8;
    }
    #endregion

    public class Db
    {
        public const String ADEXPRESS_SCHEMA = "ADEXPR03";

        public const String PROMO_SCHEMA = "PROMO03";
    }
}

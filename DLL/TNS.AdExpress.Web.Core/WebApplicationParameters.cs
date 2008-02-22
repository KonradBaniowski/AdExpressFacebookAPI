#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Web.Core.DataAccess;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core {

    /// <summary>
    /// Web Application Parameters
    /// </summary>
    public class WebApplicationParameters {

        #region variables
        /// <summary>
        /// Default Language
        /// </summary>
        protected static int _defaultLanguage;
        /// <summary>
        /// Allowed languages List 
        /// </summary>
        protected static Dictionary<Int64,WebLanguage> _allowedLanguages;
        /// <summary>
        /// Themes List 
        /// </summary>
        /// <remarks>The key is the site language</remarks>
        protected static Dictionary<Int64,WebTheme> _themes;


        #endregion
        
        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static WebApplicationParameters() {
            //Initialization
            _defaultLanguage=WebLanguagesDataAccess.LoadDefaultLanguage(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_PATH));
            _allowedLanguages=WebLanguagesDataAccess.LoadLanguages(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBLANGUAGES_CONFIGURATION_PATH));
            _themes=WebThemesDataAccess.LoadThemes(new XmlReaderDataSource(AppDomain.CurrentDomain.BaseDirectory+TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBTHEMES_CONFIGURATION_PATH));
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Default Language
        /// </summary>
        public static int DefaultLanguage {
            get { return _defaultLanguage; }
        }
        /// <summary>
        /// Get allowed languages List 
        /// </summary>
        public static Dictionary<Int64,WebLanguage> AllowedLanguages {
            get { return _allowedLanguages; }
        }

        /// <summary>
        /// Get themes List 
        /// </summary>
        public static Dictionary<Int64,WebTheme> Themes {
            get { return _themes; }
        }

        #endregion

    }
}

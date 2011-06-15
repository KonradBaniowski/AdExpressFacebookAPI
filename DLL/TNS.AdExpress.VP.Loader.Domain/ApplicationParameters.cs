#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Core;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Constantes.Web;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.ModulesDescritpion;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.DB.Constantes;
using System.IO;

namespace TNS.AdExpress.Domain.Web {

    /// <summary>
    /// Web Application Parameters
    /// </summary>
    public class ApplicationParameters {

        #region variables
        /// <summary>
        /// Database description
        /// </summary>
        private static DataBase _dataBase;        	
		/// <summary>
		/// Collections of layers which can be called in all web site
		/// <example></example>
		/// </summary>
		protected static Dictionary<WebConstantes.Layers.Id, TNS.AdExpress.Domain.Layers.CoreLayer> _coreLayers = null;
        #endregion
        
        #region Contructeur
        /// <summary>
        /// Constructor
        /// </summary>
        static ApplicationParameters() {
        }
        #endregion

        #region Accessors
        /// <summary>
        /// Get Database description
        /// </summary>
        public static DataBase DataBaseDescription{
            get { return _dataBase; }
        }
		/// <summary>
		/// Get Collections of layers which can be called in all web site
		/// </summary>
		public static Dictionary<WebConstantes.Layers.Id, Domain.Layers.CoreLayer> CoreLayers {
			get { return _coreLayers; }
		}
        #endregion

        #region Public Methods
        /// <summary>
        /// Initialize
        /// </summary>
        public static void Initialize(string configurationDirectoryRoot) {
            string countryCode = WebParamtersXL.LoadDirectoryName(new XmlReaderDataSource(configurationDirectoryRoot + TNS.AdExpress.Constantes.Web.ConfigurationFile.WEBPARAMETERS_CONFIGURATION_FILENAME));
            string countryConfigurationDirectoryRoot = Path.Combine(configurationDirectoryRoot, countryCode);

            _dataBase = new DataBase(new XmlReaderDataSource(Path.Combine(countryConfigurationDirectoryRoot, TNS.AdExpress.Constantes.Web.ConfigurationFile.DATABASE_CONFIGURATION_FILENAME)));
            _coreLayers = CoreLayersXL.Load(new XmlReaderDataSource(Path.Combine(countryConfigurationDirectoryRoot, TNS.AdExpress.Constantes.Web.ConfigurationFile.CORE_LAYERS_CONFIGURATION_FILENAME)));
        }
        #endregion

    }
}

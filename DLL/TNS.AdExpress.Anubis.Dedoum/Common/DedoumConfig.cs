using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Dedoum.DataAccess;

namespace TNS.AdExpress.Anubis.Dedoum.Common
{
    /// <summary>
    /// Static class containing every cofiguration elements for the Dedoum Config Plug-in
    /// </summary>
    public class DedoumConfig
    {
        #region Attributes

        #region Mail Properties
        /// <summary>
        /// Web Server
        /// </summary>
        private string _webServer = string.Empty;
        /// <summary>
        /// Serveur de mail d'envoie des résultats
        /// </summary>
        private string _customerMailServer;
        /// <summary>
        /// Port du serveur de mail d'envoie des résultats
        /// </summary>
        private int _customerMailPort = 25;
        /// <summary>
        /// Mail d'envoie des résultats
        /// </summary>
        private string _customerMailFrom;
        /// <summary>
        /// Sujet des mails de résultats
        /// </summary>
        private string _customerMailSubject;
        /// <summary>
        /// Base path for storing creatives files
        /// </summary>
        private string _creativesPath = "";
        /// <summary>
        /// Base path for creatives source
        /// </summary>
        private string _creativesSourcePath = "";

        private Dictionary<long, string> _columnsCongifs = new Dictionary<long, string>();
        #endregion

        #endregion

        #region Accesseurs

        #region Mail properties
      
        /// <summary>
        /// Obtient le serveur web de résultats
        /// </summary>
        public string WebServer
        {
            get { return _webServer; }
            set { _webServer = value; }
        }
        /// <summary>
        /// Obtient le serveur des mails de résultats
        /// </summary>
        public string CustomerMailServer
        {
            get { return _customerMailServer; }
            set { _customerMailServer = value; }
        }
        /// <summary>
        /// Obtient le port du serveur des mails des résultats
        /// </summary>
        public int CustomerMailPort
        {
            get { return _customerMailPort; }
            set { _customerMailPort = value; }
        }
        /// <summary>
        /// Obtient le mail d'envoie des résultats 
        /// </summary>
        public string CustomerMailFrom
        {
            get { return _customerMailFrom; }
            set { _customerMailFrom = value; }
        }
        /// <summary>
        /// Obtient le sujet des mails de résultat
        /// </summary>
        public string CustomerMailSubject
        {
            get { return _customerMailSubject; }
            set { _customerMailSubject = value; }
        }
        /// <summary>
        /// Get / Set the base path for creatives file
        /// </summary>
        public string CreativesPath
        {
            get { return _creativesPath; }
            set { _creativesPath = value; }
        }
        /// <summary>
        /// Get / Set the base path for creatives source
        /// </summary>
        public string CreativesSourcePath
        {
            get { return _creativesSourcePath; }
            set { _creativesSourcePath = value; }
        }
        /// <summary>
        /// GET SET columns configuration
        /// </summary>
        public Dictionary<long, string> ColumnsCongifs
        {
            get { return _columnsCongifs; }
            set { _columnsCongifs = value; }
        }

        #endregion


        #endregion

        	#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="dataSource">Source de données</param>
        public DedoumConfig(IDataSource dataSource)
        {
			try{
                DedoumConfigDataAccess.Load(dataSource, this);
			}
			catch(System.Exception e){
				throw(e);
			}
		}
		#endregion

    }
}

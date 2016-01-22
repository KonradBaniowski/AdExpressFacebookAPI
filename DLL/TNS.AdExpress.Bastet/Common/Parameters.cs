#region Informations
// Auteur: B. Masson, D.Mussuma
// Date de création: 16/11/2005
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.IO;
using System.Data;
using System.Runtime.Serialization.Formatters.Binary;
using Oracle.DataAccess.Client;
using Oracle.DataAccess.Types;
using TNS.AdExpress.Constantes.DB;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.Ares.StaticNavSession.DAL;
using TNS.AdExpress.Bastet.Web;


namespace TNS.AdExpress.Bastet.Common{
	/// <summary>
	/// La classe Parameters comprend tous les paramètres nécessaires à la génération des statistiques d'AdExpress.
	/// </summary>
	[System.Serializable]
	public class Parameters {
		
		#region variables
		/// <summary>
		/// Source de données
		/// </summary>
		protected IDataSource _source = null;
		/// <summary>
		/// Identifiant du login qui fait la demande
		/// </summary>
		protected Int64 _loginId;
		/// <summary>
		/// Date de début
		/// </summary>
		protected DateTime _periodBeginningDate;
		/// <summary>
		/// Date de fin
		/// </summary>
        protected DateTime _periodEndDate;
		/// <summary>
		/// Logins clients
		/// </summary>
		protected string _logins;
		/// <summary>
		/// Emails destinataires
		/// </summary>
		protected ArrayList _emailsRecipient = new ArrayList();
		/// <summary>
		/// Nom du fichier Excel
		/// </summary>
		protected string _exportExcelFileName;
        /// <summary>
        /// SiteLanguage
        /// </summary>
        protected int _siteLanguage = 33;
		#endregion

		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public Parameters(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="loginId">Identifiant du login qui fait la demande</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <param name="loginsList">Logins clients</param>
		/// <param name="emails">Emails destinataires</param>
        public Parameters(IDataSource source, Int64 loginId, DateTime dateBegin, DateTime dateEnd, string loginsList, ArrayList emails, int siteLanguage) {
			_source = source;
			_loginId = loginId;
			_periodBeginningDate = dateBegin;
			_periodEndDate = dateEnd;
			_logins = loginsList;
			_emailsRecipient = emails;
			_exportExcelFileName = "Tracking_"+dateBegin+"-"+dateEnd;
            _siteLanguage = siteLanguage;
		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou défini la date de début
		/// </summary>
        public DateTime PeriodBeginningDate {
			get{return _periodBeginningDate;}
			set{_periodBeginningDate = value;}
		}

		/// <summary>
		/// Obtient ou défini la date de fin
		/// </summary>
        public DateTime PeriodEndDate {
			get{return _periodEndDate;}
			set{_periodEndDate = value;}
		}

		/// <summary>
		/// Obtient ou défini la liste des logins
		/// </summary>
		public string Logins{
			get{return _logins;}
			set{_logins = value;}
		}

		/// <summary>
		/// Obtient ou défini la liste des emails destinataires
		/// </summary>
		public ArrayList EmailsRecipient{
			get{return _emailsRecipient;}
			set{_emailsRecipient = value;}
		}

		/// <summary>
		/// Obtient ou défini la source de données
		/// </summary>
		public IDataSource Source{
			get{return _source;}
			set{_source = value;}
		}

        /// <summary>
        /// Obtient ou défini la langue su site
        /// </summary>
        public int SiteLanguage {
            get { return _siteLanguage; }
            set { _siteLanguage = value; }
        }
		#endregion

		#region Blob
		
		#region Chargement et sauvegarde des sessions statiques
		/// <summary>
		/// Sauvegarde des paramètres d'un résultat PDF à partir de la base de données
		/// </summary>
		/// <param name="webSession">Session client à sauvegarder</param>
		/// <param name="resultType">Type de résultat</param>
		/// <returns>Identifiant correspondant à la session sauvegardée</returns>
		public Int64 Save(){


            TNS.Ares.Domain.Layers.DataAccessLayer dataAccessLayer = TNS.Ares.Domain.LS.PluginConfiguration.GetDataAccessLayer(TNS.Ares.Domain.LS.PluginDataAccessLayerName.Session);

            object[] parameters = new object[1];
            parameters[0] = _source;
            IStaticNavSessionDAL staticNavSessionDAL = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + dataAccessLayer.AssemblyName, dataAccessLayer.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, parameters, null, null, null);

            MemoryStream ms=null;
			BinaryFormatter bf=null;
            byte[] binaryData=null;
            Int64 idStaticNavSession = -1;

            try {
                ms = new MemoryStream();
                bf = new BinaryFormatter();
                bf.Serialize(ms, this);
                binaryData = new byte[ms.GetBuffer().Length];
                binaryData = ms.GetBuffer();
                try {
                    idStaticNavSession = staticNavSessionDAL.InsertData(binaryData, _loginId, TNS.Ares.Domain.LS.PluginConfiguration.GetPluginInformation(TNS.Ares.Domain.LS.PluginType.Bastet).ResultType, _exportExcelFileName);
                }
                catch (Exception er) {
                    throw (new ParametersException("WebSession.Save() : Echec de l'insertion de l'objet dans la base de donnée", er));
                }
            }
            catch (System.Exception e) {
                // Fermeture des structures
                try {
                    if (ms != null) ms.Close();
                    if (bf != null) bf = null;
                }
                catch (System.Exception et) {
                    throw (new ParametersException("Parameter.Save() : Impossible de libérer les ressources après échec de la méthode", et));
                }
                throw (new ParametersException("WebSession.Save() : Echec de la sauvegarde de l'objet dans la base de donnée", e));
            }

            
			
			return(idStaticNavSession);
		}

        /// <summary>
        /// Méthode pour la récupération et la "deserialization" d'un objet WebSession à partir du champ BLOB de la table des static_nav_sessions
        /// </summary>
        /// <returns>Retourne l'objet récupéré ou null si il y a eu un problème non géré</returns>
        /// <param name="idStaticNavSession">Identifiant de la session sauvegardée</param>
        public static Object Load(Int64 idStaticNavSession, IDataSource dataSource) {

            TNS.Ares.Domain.Layers.DataAccessLayer dataAccessLayer = TNS.Ares.Domain.LS.PluginConfiguration.GetDataAccessLayer(TNS.Ares.Domain.LS.PluginDataAccessLayerName.Session);

            object[] parameters = new object[1];
            parameters[0] = dataSource;
            IStaticNavSessionDAL staticNavSessionDAL = (IStaticNavSessionDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + dataAccessLayer.AssemblyName, dataAccessLayer.Class, false, System.Reflection.BindingFlags.CreateInstance | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, parameters, null, null);
            return staticNavSessionDAL.LoadData(idStaticNavSession);

        }
		#endregion

		#endregion


	}
}

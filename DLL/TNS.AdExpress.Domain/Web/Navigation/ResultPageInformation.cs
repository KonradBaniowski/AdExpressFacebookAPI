#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon 11/08/2005 Nom de variables 
//	Y. Rkaina 25/08/2006 remoteMediaPlanPdfUrl
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	/// Classe contenant divers information sur une page de resultat comme son url ou son code de traduction
	/// </summary>
	public class ResultPageInformation:PageInformation{

		#region Variables
		/// <summary>
		/// Identifiant du résultat
		/// </summary>
		protected Int64 _resultId;
		/// <summary>
		/// Lien vers la page Excel des données brutes
		/// </summary>
		protected string _rawExcelUrl="";
		/// <summary>
		/// Lien vers la page Excel
		/// </summary>
		protected string _printExcelUrl="";
		/// <summary>
		/// Lien vers la page Excel Bis (inversion ligne/colonne)
		/// </summary>
		protected string _printBisExcelUrl="";
		/// <summary>
		/// Lien vers la page Excel pour l'affichage des unités (Plan Média)
		/// </summary>
		protected string _valueExcelUrl="";
		/// <summary>
		/// Lien vers la page d'export Jpeg
		/// </summary>
		protected string _exportJpegUrl="";
		/// <summary>
		/// Lien vers la pop-up de sauvegarde d'un pdf via Anubis
		/// </summary>
		protected string _remotePdfUrl="";
		/// <summary>
		/// Lien vers la pop-up de sauvegarde d'un résultat en pdf via Anubis
		/// </summary>
		protected string _remoteResultPdfUrl="";
		/// <summary>
		/// Lien vers la pop-up de sauvegarde d'un fichier texte via Anubis
		/// </summary>
		protected string _remoteTextUrl="";
		/// <summary>
		/// Lien vers la pop-up de sauvegarde d'un fichier excel via Anubis
		/// </summary>
		protected string _remoteExcelUrl="";
		/// <summary>
		/// Liste des types de détail sélection à afficher dans les rappels de la sélection
		/// </summary>
		protected ArrayList _detailSelectionItemsType=new ArrayList();
        /// <summary>
        /// Allowed units list
        /// </summary>
        protected List<CustomerSessions.Unit> _allowedUnitsList = new List<CustomerSessions.Unit>();
        /// <summary>
        /// Allowed media list
        /// </summary>
        protected MediaItemsList _allowedMediaUniverse;		
        /// <summary>
        /// Parent Module
        /// </summary>
        protected Module _parentModule;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'objet</param>
		/// <param name="url">Url de la page</param>
		/// <param name="idWebText">Identifiant du texte</param>
		/// <param name="resultId">Identifiant du resultat (MAU01)</param>
		/// <param name="helpUrl">Url de la page d'aide</param>
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant à la page</param>
		public ResultPageInformation(int id,Int64 resultId, string url, Int64 idWebText,string helpUrl,Int64 menuTextId):base(id,url,idWebText,helpUrl,menuTextId){
			_resultId=resultId;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'objet</param>
		/// <param name="url">Url de la page</param>
		/// <param name="idWebText">Identifiant du texte</param>
		/// <param name="rawExcelUrl">Lien vers la page Excel des données brutes</param>
		/// <param name="printExcelUrl">Lien vers la page Excel</param>
		/// <param name="exportJpegUrl">Lien vers l'export JPEG</param>
		/// <param name="printBisExcelUrl">Lien vers l'export de 2ième type Excel</param>
		/// <param name="remotePdfUrl">Lien pour la génération de pdf par anubis</param>
		/// <param name="_remoteResultPdfUrl">Lien pour la génération de pdf par anubis (Plan Media avec Visuels)</param>
		/// <param name="resultId">Identifiant du resultat (MAU01)</param>
		/// <param name="valueExcel">Lien vers la page excel pour l'affichage des unités</param>
		/// <param name="remoteTextUrl">Lien pour la génération du  fichier texte par anubis</param>
		/// <param name="remoteExcelUrl">Lien pour la génération du  fichier excel par anubis</param>
		/// <param name="helpUrl">Url de la page d'aide</param>
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant à la page</param>
        ///<param name="functionName">Nom de la fonction script à utiliser</param>
		public ResultPageInformation(int id,Int64 resultId, string url, Int64 idWebText, string rawExcelUrl, string printExcelUrl, string printBisExcelUrl, string exportJpegUrl,string remotePdfUrl,string remoteResultPdfUrl,string valueExcel,string remoteTextUrl,string remoteExcelUrl,string helpUrl,Int64 menuTextId):this(id,resultId,url,idWebText,helpUrl,menuTextId){
			if(rawExcelUrl!=null)_rawExcelUrl = rawExcelUrl;
			if(printExcelUrl!=null)_printExcelUrl = printExcelUrl;
			if(printBisExcelUrl!=null)_printBisExcelUrl = printBisExcelUrl;
			if(exportJpegUrl!=null)_exportJpegUrl = exportJpegUrl;
			if(remotePdfUrl!=null)_remotePdfUrl = remotePdfUrl;
			if(remoteResultPdfUrl!=null)_remoteResultPdfUrl = remoteResultPdfUrl;
			if(remoteTextUrl!=null)_remoteTextUrl = remoteTextUrl;
			if(remoteExcelUrl!=null)_remoteExcelUrl = remoteExcelUrl;
			if(valueExcel!=null)_valueExcelUrl = valueExcel;
		}
		#endregion

		#region Accesseur
        /// <summary>
        /// Set parent module
        /// </summary>
        public Module ParentModule {
            set { _parentModule=value; }
        }

        /// <summary>
        /// Get if allowed Media universe is null
        /// </summary>
        public bool IsNullAllowedMediaUniverse {
            get {
                if(_allowedMediaUniverse==null) return (true);
                return (false);
            }
        }

        /// <summary>
        /// Get/Set Allowed Media Universe
        /// </summary>
        public MediaItemsList AllowedMediaUniverse {
            get {
				//if(_allowedMediaUniverse==null) {
				//    if(_parentModule==null) throw (new NullReferenceException("Parent module has to be initialized"));
				//    return (_parentModule.AllowedMediaUniverse);
				//}
				SetMediaUniverse();
                return _allowedMediaUniverse;
            }
            set { _allowedMediaUniverse=value; }
        }
		
		/// <summary>
		/// Obtient l'identifiant du MAU01 du résultat
		/// </summary>
		public Int64 ResultID{
			get{return _resultId;}
		}

		/// <summary>
		/// Obtient l'url de la page Excel des données brutes
		/// </summary>
		public string RawExcelUrl{
			get{return _rawExcelUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page Excel
		/// </summary>
		public string PrintExcelUrl{
			get{return _printExcelUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page Excel Bis
		/// </summary>
		public string PrintBisExcelUrl{
			get{return _printBisExcelUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page Excel avec les unités (Plan média)
		/// </summary>
		public string ValueExcelUrl{
			get{return _valueExcelUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page export Jpeg
		/// </summary>
		public string ExportJpegUrl{
			get{return _exportJpegUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de génération d'un Pdf Via Anubis
		/// </summary>
		public string RemotePdfUrl{
			get{return _remotePdfUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de génération d'un résultat en Pdf Via Anubis
		/// </summary>
		public string RemoteResultPdfUrl{
			get{return _remoteResultPdfUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de génération d'un fichier Texte Via Anubis
		/// </summary>
		public string RemoteTextUrl
		{
			get{return _remoteTextUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de génération d'un fichier excel Via Anubis
		/// </summary>
		public string RemoteExceltUrl {
			get{return _remoteExcelUrl;}
		}


		/// <summary>
		/// Obtient et défini la liste des types d'éléments à montrer dans le rappel de la sélection
		/// </summary>
		public ArrayList DetailSelectionItemsType
		{
			get{return _detailSelectionItemsType;}
			set{_detailSelectionItemsType=value;}
		}

        /// <summary>
        /// Get / Set allowed units enum list
        /// </summary>
        public List<CustomerSessions.Unit> AllowedUnitEnumList {
            get { return _allowedUnitsList; }
            set { _allowedUnitsList = value; }
        }
		#endregion

		#region Méthodes Externe
        /// <summary>
        /// Initialize allowed media universe with a default instance
        /// </summary>
        public void InitAllowedMediaUniverse() {
            _allowedMediaUniverse=new MediaItemsList();
        }

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel des données brutes
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayRawExcelPage(){
			if(_rawExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayPrintExcelPage(){
			if(_printExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel Bis
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayPrintBisExcelPage(){
			if(_printBisExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel des unités
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayValueExcelPage(){
			if(_valueExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page d'export Jpeg
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayExportJpegPage(){
			if(_exportJpegUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page la page de génération d'un Pdf via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayRemotePdfPage(){
			if(_remotePdfUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page la page de génération d'un résultat Pdf via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayRemoteResultPdfPage(){
			if(_remoteResultPdfUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers  la page de génération d'un fichier texte via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayRemoteTextPage()
		{
			if(_remoteTextUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers  la page de génération d'un fichier excel via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalité doit être montrée, false sinon</returns>
		public bool CanDisplayRemoteExcelPage() {
			if(_remoteExcelUrl.Length==0) return (false);
			return (true);
		}


        /// <summary>
        /// Get commun Unit list
        /// </summary>
        /// <param name="databaseVehiclesList">Database vehicle List</param>
        /// <returns>commun Unit list</returns>
        public List<CustomerSessions.Unit> GetValidUnitsEnum(List<Int64> databaseVehiclesList) {
            return (GetValidUnitsEnum(VehiclesInformation.DatabaseIdToEnumListList(databaseVehiclesList)));       
        }

        /// <summary>
        /// Get commun Unit list
        /// </summary>
        /// <param name="vehiclesList">Vehicle List</param>
        /// <returns>commun Unit list</returns>
        public List<CustomerSessions.Unit> GetValidUnitsEnum(List<Vehicles.names> vehiclesList) {
            List<CustomerSessions.Unit> validUnitsList = new List<CustomerSessions.Unit>();
            List<CustomerSessions.Unit> vehicleUnitList=VehiclesInformation.GetCommunUnitList(vehiclesList);

            foreach (CustomerSessions.Unit currentUnit in _allowedUnitsList) {

                if(vehicleUnitList.Contains(currentUnit))
                    validUnitsList.Add(currentUnit);

            }
            return (validUnitsList);
        }

        /// <summary>
        /// Get commun UnitInformation list
        /// </summary>
        /// <param name="databaseVehiclesList">Database vehicle List</param>
        /// <returns>commun UnitInformation list</returns>
        public List<UnitInformation> GetValidUnits(List<Int64> databaseVehiclesList) {
            return (GetValidUnits(VehiclesInformation.DatabaseIdToEnumListList(databaseVehiclesList)));
        }



        /// <summary>
        /// Get commun UnitInformation list
        /// </summary>
        /// <param name="vehiclesList">Vehicle List</param>
        /// <returns>commun UnitInformation list</returns>
        public List<UnitInformation> GetValidUnits(List<Vehicles.names> vehiclesList) {
            List<UnitInformation> validUnitsList = new List<UnitInformation>();
            List<CustomerSessions.Unit> vehicleUnitList = VehiclesInformation.GetCommunUnitList(vehiclesList);

            foreach (CustomerSessions.Unit currentUnit in _allowedUnitsList) {
                
                if (vehicleUnitList.Contains(currentUnit))
                    validUnitsList.Add(UnitsInformation.Get(currentUnit));

            }
            return (validUnitsList);
        }
		/// <summary>
		/// Detrmine if result page if valid according media universe selection
		/// </summary>
		/// <param name="selectedMediaUniverse">Selected media universe</param>
		/// <returns></returns>
		public bool IsValidResultPage(MediaItemsList selectedMediaUniverse) {
			bool isValidPage = true;
			int nbValidItems = 0;
			MediaItemsList allowedMediaUniverse = AllowedMediaUniverse;
			if (selectedMediaUniverse != null && selectedMediaUniverse.VehicleList != null && selectedMediaUniverse.VehicleList.Length>0 ) {				
				if (allowedMediaUniverse != null) {
					//Determine if valid vehicles exist for current result
					List<Int64> vehicleList = allowedMediaUniverse.GetVehicles();
					if (vehicleList != null && vehicleList.Count > 0) {
						List<Int64> selectedVehicles = selectedMediaUniverse.GetVehicles();
						nbValidItems = GetNbValidItems(selectedVehicles, vehicleList);
					}
					//Determine if valid categories exist for current result
					List<Int64> categoryList = allowedMediaUniverse.GetCategories();
					if (nbValidItems ==0 && categoryList != null && categoryList.Count > 0) {
						List<Int64> selectedCategories = selectedMediaUniverse.GetCategories();
						nbValidItems = GetNbValidItems(selectedCategories, categoryList);
					}
					//Determine if valid medias exist for current result
					List<Int64> mediaList = allowedMediaUniverse.GetMedias();
					if (nbValidItems == 0 && mediaList != null && mediaList.Count > 0) {
						List<Int64> selectedMedias = selectedMediaUniverse.GetMedias();
						nbValidItems = GetNbValidItems(selectedMedias, mediaList);
					}
					if(nbValidItems==0)isValidPage = false;
				}
			}
			return isValidPage;
		}

		

		#endregion

		#region SQL Generator
		/// <summary>
		/// Get media universe sql conditions
		/// </summary>
		/// <param name="startWithAnd">Determine if sql condition start with "and"</param>
		/// <returns>Sql conditions</returns>
		public string GetAllowedMediaUniverseSql(bool startWithAnd) {
			string sql = "";
			MediaItemsList mediaList = AllowedMediaUniverse;

			if (mediaList != null) {
				sql = AllowedMediaUniverse.GetVehicleListSQL(startWithAnd);
				sql += AllowedMediaUniverse.GetCategoryListSQL(startWithAnd);
				sql += AllowedMediaUniverse.GetMediaListSQL(startWithAnd);
			}
			return sql;
		}
		/// <summary>
		/// Get media universe sql conditions without prefix
		/// </summary>
		/// <param name="startWithAnd">Determine if sql condition start with "and"</param>
		/// <returns>Sql conditions</returns>
		public string GetAllowedMediaUniverseSqlWithOutPrefix(bool startWithAnd) {
			return GetAllowedMediaUniverseSql("", startWithAnd);
		}
		/// <summary>
		/// Get media universe sql conditions
		/// </summary>
		/// <param name="prefix">prefix</param>		
		/// <param name="startWithAnd">Determine if sql condition start with "and"</param>
		/// <returns>Sql conditions</returns>
		public string GetAllowedMediaUniverseSql(string prefix, bool startWithAnd) {
			return GetAllowedMediaUniverseSql(prefix, prefix, prefix, startWithAnd);
		}
		/// <summary>
		/// Get media universe sql conditions
		/// </summary>
		/// <param name="vehiclePrefix">Vehicle prefix</param>
		/// <param name="categoryPrefix">Category prefix</param>
		/// <param name="mediaPrefix">Media prefix</param>
		/// <param name="startWithAnd">Determine if sql condition start with "and"</param>
		/// <returns>Sql conditions</returns>
		public string GetAllowedMediaUniverseSql(string vehiclePrefix, string categoryPrefix, string mediaPrefix, bool startWithAnd) {
			string sql = "";
			MediaItemsList mediaList = AllowedMediaUniverse;

			if (mediaList != null) {
				sql = AllowedMediaUniverse.GetVehicleListSQL(startWithAnd, vehiclePrefix);
				sql += AllowedMediaUniverse.GetCategoryListSQL(startWithAnd, categoryPrefix);
				sql += AllowedMediaUniverse.GetMediaListSQL(startWithAnd, mediaPrefix);
			}
			return sql;

		}

		#endregion

		#region Internal methods
		/// <summary>
		/// Define media universe
		/// </summary>
		protected void SetMediaUniverse() {
			string res = "";
			//Set vehicle list
			if (_allowedMediaUniverse == null) {
				if (_parentModule == null) throw (new NullReferenceException("Parent module has to be initialized"));
				_allowedMediaUniverse = _parentModule.AllowedMediaUniverse;
			}
			else {
				string vehicles = _allowedMediaUniverse.VehicleList;
				if (vehicles != null && vehicles.Length > 0) {
					List<Int64> vehicleList = new List<Int64>(Array.ConvertAll<string, Int64>(vehicles.Split(','), (Converter<string, long>)delegate(string s) { return Convert.ToInt64(s); }));
					for (int i = 0; i < vehicleList.Count; i++) {
						if (VehiclesInformation.Contains(vehicleList[i])) {
							res += vehicleList[i].ToString() + ",";
						}
					}
					if (res.Length > 0) res = res.Substring(0, res.Length - 1);
					_allowedMediaUniverse.VehicleList = res;
				}
			}
		}

		/// <summary>
		/// Get Nb valid items for current result page
		/// </summary>
		/// <param name="selectedItems">Selected Items</param>
		/// <param name="allowedUniverseItems">Allowed universe Items in configurations file or Baal</param>
		/// <returns>Nb valid items for current result page</returns>
		protected virtual int GetNbValidItems(List<Int64> selectedItems, List<Int64> allowedUniverseItems) {
			int nbValidItems = 0;
			if (selectedItems != null && selectedItems.Count > 0) {
				for (int i = 0; i < selectedItems.Count; i++) {
					if (allowedUniverseItems.Contains(selectedItems[i])) nbValidItems++;
				}
			}
			return nbValidItems;
		}
		#endregion

	}
}

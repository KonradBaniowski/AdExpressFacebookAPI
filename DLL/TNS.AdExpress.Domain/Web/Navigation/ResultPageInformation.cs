#region Informations
// Auteur: g. Facon
// Date de cr�ation:
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
		/// Identifiant du r�sultat
		/// </summary>
		protected Int64 _resultId;
		/// <summary>
		/// Lien vers la page Excel des donn�es brutes
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
		/// Lien vers la page Excel pour l'affichage des unit�s (Plan M�dia)
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
		/// Lien vers la pop-up de sauvegarde d'un r�sultat en pdf via Anubis
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
		/// Liste des types de d�tail s�lection � afficher dans les rappels de la s�lection
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
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant � la page</param>
		public ResultPageInformation(int id,Int64 resultId, string url, Int64 idWebText,string helpUrl,Int64 menuTextId):base(id,url,idWebText,helpUrl,menuTextId){
			_resultId=resultId;
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="id">identifiant de l'objet</param>
		/// <param name="url">Url de la page</param>
		/// <param name="idWebText">Identifiant du texte</param>
		/// <param name="rawExcelUrl">Lien vers la page Excel des donn�es brutes</param>
		/// <param name="printExcelUrl">Lien vers la page Excel</param>
		/// <param name="exportJpegUrl">Lien vers l'export JPEG</param>
		/// <param name="printBisExcelUrl">Lien vers l'export de 2i�me type Excel</param>
		/// <param name="remotePdfUrl">Lien pour la g�n�ration de pdf par anubis</param>
		/// <param name="_remoteResultPdfUrl">Lien pour la g�n�ration de pdf par anubis (Plan Media avec Visuels)</param>
		/// <param name="resultId">Identifiant du resultat (MAU01)</param>
		/// <param name="valueExcel">Lien vers la page excel pour l'affichage des unit�s</param>
		/// <param name="remoteTextUrl">Lien pour la g�n�ration du  fichier texte par anubis</param>
		/// <param name="remoteExcelUrl">Lien pour la g�n�ration du  fichier excel par anubis</param>
		/// <param name="helpUrl">Url de la page d'aide</param>
		///<param name="menuTextId">Identifiant du texte dans le menu correspondant � la page</param>
        ///<param name="functionName">Nom de la fonction script � utiliser</param>
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
                if(_allowedMediaUniverse==null) {
                    if(_parentModule==null) throw (new NullReferenceException("Parent module has to be initialized"));
                    return (_parentModule.AllowedMediaUniverse);
                }
                return _allowedMediaUniverse;
            }
            set { _allowedMediaUniverse=value; }
        }

		/// <summary>
		/// Obtient l'identifiant du MAU01 du r�sultat
		/// </summary>
		public Int64 ResultID{
			get{return _resultId;}
		}

		/// <summary>
		/// Obtient l'url de la page Excel des donn�es brutes
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
		/// Obtient l'url de la page Excel avec les unit�s (Plan m�dia)
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
		/// Obtient l'url de la page de g�n�ration d'un Pdf Via Anubis
		/// </summary>
		public string RemotePdfUrl{
			get{return _remotePdfUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de g�n�ration d'un r�sultat en Pdf Via Anubis
		/// </summary>
		public string RemoteResultPdfUrl{
			get{return _remoteResultPdfUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de g�n�ration d'un fichier Texte Via Anubis
		/// </summary>
		public string RemoteTextUrl
		{
			get{return _remoteTextUrl;}
		}

		/// <summary>
		/// Obtient l'url de la page de g�n�ration d'un fichier excel Via Anubis
		/// </summary>
		public string RemoteExceltUrl {
			get{return _remoteExcelUrl;}
		}


		/// <summary>
		/// Obtient et d�fini la liste des types d'�l�ments � montrer dans le rappel de la s�lection
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

		#region M�thodes Externe
        /// <summary>
        /// Initialize allowed media universe with a default instance
        /// </summary>
        public void InitAllowedMediaUniverse() {
            _allowedMediaUniverse=new MediaItemsList();
        }

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel des donn�es brutes
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayRawExcelPage(){
			if(_rawExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayPrintExcelPage(){
			if(_printExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel Bis
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayPrintBisExcelPage(){
			if(_printBisExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page Excel des unit�s
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayValueExcelPage(){
			if(_valueExcelUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page d'export Jpeg
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayExportJpegPage(){
			if(_exportJpegUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page la page de g�n�ration d'un Pdf via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayRemotePdfPage(){
			if(_remotePdfUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers la page la page de g�n�ration d'un r�sultat Pdf via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayRemoteResultPdfPage(){
			if(_remoteResultPdfUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers  la page de g�n�ration d'un fichier texte via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
		public bool CanDisplayRemoteTextPage()
		{
			if(_remoteTextUrl.Length==0) return (false);
			return (true);
		}

		/// <summary>
		/// Il existe ou pas un lien vers  la page de g�n�ration d'un fichier excel via Anubis
		/// </summary>
		/// <returns>True si la fonctionnalit� doit �tre montr�e, false sinon</returns>
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

		#endregion



	}
}

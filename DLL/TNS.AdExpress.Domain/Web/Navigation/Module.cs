#region Informations
// Auteur: g. Facon
// Date de création:
// Date de modification:
//	G. Facon    11/08/2005      Nom de variables
//  G. Facon    27/08/2008      Add Allowed Media universe
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Classification;
using DBConstantes = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpress.Domain.Web.Navigation {
	/// <summary>
	///  Classe utilisée dans l'affichage des textes des modules
	/// </summary>
	public class Module:ModuleItem{
		
		#region Variable
        /// <summary>
        /// Allowed Media universe
        /// </summary>
        protected MediaItemsList _allowedMediaUniverse;		
        /// <summary>
        /// Country rules Layer
        /// </summary>
        protected RulesLayer _rulesLayer;
        /// <summary>
        /// Country data access Layer
        /// </summary>
        protected DataAccessLayer _dataAccessLayer;
		/// <summary>
		/// URL de la prochaine page
		/// </summary>
		protected string _urlNextPage="";
		///<summary>
		/// Information sur les pages de résultats qui concernent le module
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.ResultPageInformation</associates>
		///  <label>_resultsPages</label>
		protected System.Collections.ArrayList _resultsPages = new ArrayList();
		///<summary>
		/// Information sur les pages de sélectionsz qui concernent le module
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.SelectionPageInformation</associates>
		///  <label>_selectionsPages</label>
		protected System.Collections.ArrayList _selectionsPages = new ArrayList();
		///<summary>
		/// Information sur les pages d'options qui concerne un module
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.Navigation.OptionalPageInformation</associates>
		///  <label>_optionalsPages</label>
		protected System.Collections.ArrayList _optionalsPages=new ArrayList();

		/// <summary>
		/// Liens vers les autres modules à partir du module courant
		/// </summary>
		protected ArrayList _bridges = new ArrayList();
		/// <summary>
		/// Type du module
		/// </summary>
		protected TNS.AdExpress.Constantes.Web.Module.Type _moduleType;
		///<summary>
		/// Niveaux de détails orientés support par défaut
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_defaultMediaDetailLevels</label>
		protected ArrayList _defaultMediaDetailLevels=new ArrayList();
		///<summary>
		/// Niveaux de détails orientés produit par défaut
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_defaultProductDetailLevels</label>
		protected ArrayList _defaultProductDetailLevels=new ArrayList();
		///<summary>
		/// Eléments orienté media de niveau de détail autorisés
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_allowedMediaDetailLevelItems</label>
		protected ArrayList _allowedMediaDetailLevelItems=new ArrayList();
		///<summary>
		/// Eléments orientés produit de niveau de détail autorisés
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		///  <label>_allowedProductDetailLevelItems</label>
		protected ArrayList _allowedProductDetailLevelItems=new ArrayList();
        ///<summary>
        /// Eléments de niveau de détail colonne autorisés
        /// </summary>
        ///  <link>aggregation</link>
        ///  <supplierCardinality>0..*</supplierCardinality>
        ///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
        ///  <label>_allowedColumnDetailLevelItems</label>
        protected ArrayList _allowedColumnDetailLevelItems = new ArrayList();
		/// <summary>
		/// Identifiant de la catégorie du module
		/// </summary>
		protected Int64 _moduleCategoryId;
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur représentant un module
		/// </summary>
		/// <param name="id">identifiant du module</param>
		/// <param name="idWebText">Code traduction du texte du module</param>
		/// <param name="descriptionWebTextId">Identifiant de la description de l'item</param>
		/// <param name="moduleCategoryId">Identifiant de la catégorie de module</param>
		/// <param name="urlNextPage">Adresse URL de la page suivante</param>
		/// <param name="moduleTypeValue">Type de module (alert, analyse, recap ...)</param>
        public Module(Int64 id, Int64 idWebText, Int64 descriptionWebTextId, Int64 moduleCategoryId, string urlNextPage, int moduleTypeValue, string descriptionImageName) : base(id, idWebText, descriptionWebTextId, descriptionImageName) {
			_urlNextPage=urlNextPage;
			_idWebText=idWebText;
			_moduleType=(TNS.AdExpress.Constantes.Web.Module.Type)moduleTypeValue;
			_moduleCategoryId=moduleCategoryId;
		}
		#endregion

		#region Accesseurs
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
				SetMediaUniverse();
                return _allowedMediaUniverse;
            }
            set { _allowedMediaUniverse=value; }
        }
		
        /// <summary>
        /// Get/Set Rules Layer
        /// </summary>
        public RulesLayer CountryRulesLayer {
            get { return _rulesLayer; }
            set { _rulesLayer=value; }
        }
        /// <summary>
        /// Get/Set Rules Layer
        /// </summary>
        public DataAccessLayer CountryDataAccessLayer {
            get { return _dataAccessLayer; }
            set { _dataAccessLayer=value; }
        }
		/// <summary>
		/// Obtient l'URL de la prochaine page
		/// </summary>
		public string UrlNextPage {
			get{return _urlNextPage;}
		}

		/// <summary>
		/// Obtient le type de module
		/// </summary>
		public TNS.AdExpress.Constantes.Web.Module.Type ModuleType {
			get{return _moduleType;}
		}

		/// <summary>
		/// Get l'ensemble des informations concerntant les pages du module (url, code traduction...)
		/// </summary>
		private ArrayList ResultsPages {
			get{return _resultsPages;}
		}

		/// <summary>
		/// Get l'ensemble des informations concerntant les pages du module (url, code traduction...)
		/// </summary>
		public ArrayList SelectionsPages {
			get{return _selectionsPages;}
		}

		/// <summary>
		/// Get l'ensemble des informations concerntant les pages du module (url, code traduction...)
		/// </summary>
		public ArrayList OptionalsPages {
			get{return _optionalsPages;}
		}

		/// <summary>
		/// Get les modules atteignables à partir du module courant
		/// </summary>
		public ArrayList Bridges {
			get{return _bridges;}
		}

		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///<summary>Obtient les eléments orienté support de niveau de détail autorisés</summary>
		public ArrayList AllowedMediaDetailLevelItems {
			get{return (_allowedMediaDetailLevelItems);}
		}

		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///<summary>Obtient les eléments orienté produit de niveau de détail autorisés</summary>
		public ArrayList AllowedProductDetailLevelItems {
			get{return (_allowedProductDetailLevelItems);}
		}

        ///<author>Y. R'kaina</author>
        ///  <since>11/12/2007</since>
        ///<summary>Obtient les eléments de niveau de détail colonne autorisés</summary>
        public ArrayList AllowedColumnDetailLevelItems {
            get { return (_allowedColumnDetailLevelItems); }
        }

		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///<summary>Obtient les niveaux de détails orientés support par défaut</summary>
		public ArrayList DefaultMediaDetailLevels {
			get{return (_defaultMediaDetailLevels);}
		}

		///<author>G. Facon</author>
		///  <since>28/03/2006</since>
		///<summary>Obtient les niveaux de détails orientés produit par défaut</summary>
		public ArrayList DefaultProductDetailLevels {
			get{return (_defaultProductDetailLevels);}
		}
		#endregion

		#region Méthode Externe

        /// <summary>
        /// Initialize allowed media universe with a default instance
        /// </summary>
        public void InitAllowedMediaUniverse() {
            _allowedMediaUniverse=new MediaItemsList();
        }

		#region Gestion des pages de résultat
		///<author>G. Facon</author>
		///  <since>01/08/2006</since>
		///<summary>
		///Ajout à la liste des résultats les informations d'un page
		///</summary>
		public void AddResultPageInformation(ResultPageInformation resultPageInformation){
			_resultsPages.Add(resultPageInformation);
		}

		///<author>G. Facon</author>
		///  <since>01/08/2006</since>
		///<summary>
		///Obtient la liste ordonnée des informations des pages de résultat
		///</summary>
		///<remarks>
		///Ne doit pas être utilisée pour accéder à un résultat particulier
		///<seealso cref="TNS.AdExpress.Web.Core.Navigation.Module.GetResultPageInformation"/>
		///</remarks>
		///<returns>Liste ordonnée des informations des pages de résultat</returns>
		public ArrayList GetResultPageInformationsList(){
			return(_resultsPages);
		}

		///<author>G. Facon</author>
		///  <since>01/08/2006</since>
		///<summary>
		///Obtient les inforamtions de la page de résultat correspondant au currentTab
		///Afin de trouver les informations de la page correspondant au "currentTab" la méthode parcourt
		///la liste _resultsPages.
		///</summary>  
		///<remarks>
		///Si la méthode ne trouve pas les informations corresondantes, elle renvoie null
		///</remarks>
		///<param name="currentTab">Identifiant du resultat (currentTab)</param>
		///<returns>Inforamtions de la page de résultat</returns>
		public ResultPageInformation GetResultPageInformation(long currentTab) {
			try{
				foreach(ResultPageInformation currentPage in _resultsPages){
					if(currentPage.Id==currentTab)
						return(currentPage);
				}
				return(null);
			}
			catch(System.Exception err){
				throw(new ModuleException("Impossible d'obtenir les informations correspondant au résultat "+currentTab,err));
			}
		}

		/// <summary>
		/// Obtient l'identifant MAU01 du résultat corresondant au resultat sélectionné
		/// Afin de trouver les informations de la page correspondant au "currentTab" la méthode parcourt
		/// la liste _resultsPages.
		/// </summary>
		///<remarks>
		///Si la méthode ne trouve pas les information corresondant à ce couple elle renvoie null
		///</remarks>
		/// <param name="currentTab">Identifiant de l'onglet affiché</param>
		/// <returns>Identifiant du résultat dans le MAU01</returns>
		public Int64 GetResultId( int currentTab ){
			try{
				foreach(ResultPageInformation currentPage in _resultsPages){
					if(currentPage.Id==currentTab)
						return(currentPage.ResultID);
				}
				return(-1);
			}
			catch(System.Exception err){
				throw(new ModuleException("Impossible d'obtenir le resultId de "+currentTab,err));
			}
			
		}
		#endregion

		/// <summary>
		/// Détermine l'url suivante
		/// </summary>
		/// <param name="currentUrl">Url Courrante</param>
		/// <returns>Url</returns>
		public string FindNextUrl(string currentUrl){
			int i=0;
			foreach(SelectionPageInformation currentPage in _selectionsPages){
				if((currentPage.Url==currentUrl)||(IsSubSection(currentPage,currentUrl))){
					break;
				}
				i++;
			}
			i++;
			// La page suivante est une sélection
			if (i<_selectionsPages.Count)return(((SelectionPageInformation)_selectionsPages[i]).Url);
				// C'est une page de résultat
			else return(((ResultPageInformation)_resultsPages[0]).Url);
		}

		///<author>G. Facon</author>
		///  <since>31/07/2006</since>
		///<summary>
		///Obtient les informations de la page ayant pour url "url"
		///Afin de trouver la page correspondant à url la méthode parcourt les 3 listes d'éléments:
		/// - _selectionsPages
		/// - _resultsPages
		/// - _optionalsPages
		/// Comme il peut exister plusieurs résultat qui ont la même page, la méthode utilise currentTab (résultat courrant)
		/// afin de déterminer le résultat sélectionné par le client.
		///</summary>
		///<remarks>
		///Si la méthode ne trouve pas les informations corresondantes à ce couple elle renvoie -1
		///</remarks>
		///<param name="url">Url de la page</param>
		///<param name="currentTab">id le page de résultat sélectionnée par le client</param>
		public PageInformation GetPageInformation( string url, Int64 currentTab ){
			try{
				foreach(PageInformation currentPage in _selectionsPages){
					if(currentPage.Url==url)return(currentPage); 
					else {
						if((((SelectionPageInformation)currentPage).HtSubSelectionPageInformation!=null)&&(((SelectionPageInformation)currentPage).HtSubSelectionPageInformation.Count>0)){
							IDictionaryEnumerator myEnumerator = ((SelectionPageInformation)currentPage).HtSubSelectionPageInformation.GetEnumerator();
							while(myEnumerator.MoveNext()){
								if(((PageInformation)myEnumerator.Value).Url==url)return((PageInformation)myEnumerator.Value); 
							}
						}
					}
				}
				foreach(PageInformation currentPage in _resultsPages){
					if(currentPage.Url==url && Convert.ToInt64(currentPage.Id)==currentTab)return(currentPage);
				}
				foreach(PageInformation currentPage in _optionalsPages){
					if(currentPage.Url==url)return(currentPage);
				}
				return(null);
			}
			catch(System.Exception err){
				throw(new ModuleException("Impossible d'obtenir les inforamtions de la page "+url+","+currentTab.ToString(),err));
			}
		
		} 
		/// <summary>
		/// Permet de récupérer l'url de la page (sub selection) cible
		/// </summary>
		/// <param name="subSectionId">L'index de la page sub section courante</param>
		/// <param name="url">L'url de la page courante</param>
		/// <param name="isSelectionPage">Pour savoir si la page courante est une page de selection ou une sub selection</param>
		/// <returns>L'url de la page sub section cible</returns>
		public string GetSubSectionURL(int subSectionId,string url,bool isSelectionPage) {
			if(isSelectionPage) {
				foreach(SelectionPageInformation currentPage in _selectionsPages) {
					if(currentPage.Url==url)
						if((currentPage.HtSubSelectionPageInformation!=null)&&(currentPage.HtSubSelectionPageInformation.Count>0)) {
							return(((SubSelectionPageInformation)currentPage.HtSubSelectionPageInformation[subSectionId]).Url); 
						}
 				}
			}
			else{
				foreach(SelectionPageInformation currentPage in _selectionsPages) {
					if((currentPage.HtSubSelectionPageInformation!=null)&&(currentPage.HtSubSelectionPageInformation.Count>0)) {
						IDictionaryEnumerator myEnumerator = currentPage.HtSubSelectionPageInformation.GetEnumerator();
						while(myEnumerator.MoveNext()) {
							if(((PageInformation)myEnumerator.Value).Url==url)
								return(((SubSelectionPageInformation)currentPage.HtSubSelectionPageInformation[subSectionId]).Url); 
						}
					}
				}
			}
			return "";
		}
		/// <summary>
		/// Permet de recuperer l'url suivante pour une optionalSelection
		/// </summary>
		/// <param name="url">L'url de la page optionnelle courante</param>
		/// <returns>L'url suivante</returns>
		public string GetOptionalNextUrl(string url){
			foreach(OptionalPageInformation currentPage in _optionalsPages) {
				if(currentPage.Url==url)
					return (currentPage.OptionalNextUrl);
			}
			return "";
		}
		#endregion

		#region Méthode Interne
		/// <summary>
		/// Permet de savoir si la page courante est une sub section
		/// </summary>
		/// <param name="selectionPage">La page de selection courante</param>
		/// <param name="currentUrl">L'url courante</param>
		/// <returns>vrai ou faux</returns>
		private bool IsSubSection(SelectionPageInformation selectionPage, string currentUrl){
			if((selectionPage.HtSubSelectionPageInformation!=null)&&(selectionPage.HtSubSelectionPageInformation.Count>0)) {
				IDictionaryEnumerator myEnumerator = selectionPage.HtSubSelectionPageInformation.GetEnumerator();
				while(myEnumerator.MoveNext()) {
					if(((PageInformation)myEnumerator.Value).Url==currentUrl) {
						return(true);
					}
				}
			}
			return(false);

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
			return GetAllowedMediaUniverseSql(prefix,prefix, prefix,startWithAnd);
		}
		/// <summary>
		/// Get media universe sql conditions
		/// </summary>
		/// <param name="vehiclePrefix">Vehicle prefix</param>
		/// <param name="categoryPrefix">Category prefix</param>
		/// <param name="mediaPrefix">Media prefix</param>
		/// <param name="startWithAnd">Determine if sql condition start with "and"</param>
		/// <returns>Sql conditions</returns>
		public string GetAllowedMediaUniverseSql(string vehiclePrefix,string categoryPrefix,string mediaPrefix,bool startWithAnd) {
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

		#region protected methods

		#region SetMediaUniverse
		/// <summary>
		/// Define media universe
		/// </summary>
		protected void SetMediaUniverse() {
			string[] list;
			string res = "";
			//Set vehicle list
			if (_allowedMediaUniverse == null) {
				MediaItemsList mediaItemsList = new MediaItemsList();
				mediaItemsList.VehicleList = VehiclesInformation.GetDatabaseIds();
				_allowedMediaUniverse = mediaItemsList;
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
		#endregion
		
		#endregion

		/// <summary>
		/// Get valid result page
		/// </summary>
		/// <remarks>Valid results pages depends of universe selection , like vehicle selected</remarks>
		/// <param name="selectedMediaUniverse">selected Media Universe</param>
		/// <returns></returns>
		public List<ResultPageInformation> GetValidResultsPage(MediaItemsList selectedMediaUniverse) {
			List<ResultPageInformation> res = new List<ResultPageInformation>();

			if (_resultsPages != null && _resultsPages.Count > 0) {
				for (int i = 0; i < _resultsPages.Count; i++) {
					if (((ResultPageInformation)_resultsPages[i]).IsValidResultPage(selectedMediaUniverse)) {
						res.Add((ResultPageInformation)_resultsPages[i]);
					}
				}
			}
			return res;
		}

	}
}

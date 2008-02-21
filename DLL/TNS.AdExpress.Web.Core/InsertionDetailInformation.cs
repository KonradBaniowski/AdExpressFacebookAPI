#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Création: 16/05/2006
// Modification:
#endregion

using System;
using System.Collections;

using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core{
	/// <summary>
	/// Description résumée de InsertionDetailInformation.
	/// </summary>
	public class InsertionDetailInformation{
		#region Variables
		/// <summary>
		/// Les listes des modules autorisées par  liste de colonnes
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.GenericColumnItemInformation</associates>
		protected static Hashtable _allowedModulesByDetailColums;
		/// <summary>
		/// Niveaux de détails orientés support par défaut
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		protected static Hashtable _defaultMediaDetailLevels;
		///<summary>
		/// Eléments orienté media de niveau de détail autorisés
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		protected static Hashtable _allowedMediaDetailLevelItems;
		/// <summary>
		/// Liste des détails de colonnes a charger
		/// </summary>
		protected static Hashtable  _detailColumnsList;

		#endregion

	

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		 static InsertionDetailInformation(){			
			_allowedModulesByDetailColums=new Hashtable();
			_detailColumnsList=new Hashtable();
			_defaultMediaDetailLevels=new Hashtable();
			_allowedMediaDetailLevelItems=new Hashtable();
		}
		#endregion

		#region Initialisation de la classe
		/// <summary>
		/// Initialise la classe
		/// </summary>
		/// <param name="source">source de données</param>
		public static void Init(IDataSource source){
			DataAccess.InsertionDetailInformationDataAccess.Load(source,_allowedModulesByDetailColums,_detailColumnsList,_defaultMediaDetailLevels,_allowedMediaDetailLevelItems);
		}
		#endregion

		/// <summary>
		/// Retourne la liste des colonnes associés au média
		/// </summary>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <param name="idModule">Identifiant du module</param>
		/// <returns>Liste des colonnes</returns>
		public static ArrayList GetDetailColumns(Int64 idVehicle,Int64 idModule){
			try{
				Hashtable listAllowedModules = (Hashtable)_allowedModulesByDetailColums[idVehicle];
				Int64 idDetailColumn = (Int64)listAllowedModules[idModule];
				return (ArrayList)_detailColumnsList[idDetailColumn]; 
				
			}
			catch(System.Exception){
				return(null);
			}
		}

		/// <summary>
		/// Retourne la liste des niveaux de détail média par défaut pour un média
		/// </summary>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <returns>Liste des colonnes</returns>
		public static ArrayList GetDefaultMediaDetailLevels(Int64 idVehicle){
			try{
				return (ArrayList)_defaultMediaDetailLevels[idVehicle]; 
			}
			catch(System.Exception){
				return(null);
			}						
		}

		/// <summary>
		/// Retourne la liste des niveaux de détail média autorisés pour un média
		/// </summary>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <returns>Liste des colonnes</returns>
		public static ArrayList GetAllowedMediaDetailLevelItems(Int64 idVehicle){
			try{
				return (ArrayList)_allowedMediaDetailLevelItems[idVehicle]; 
			}
			catch(System.Exception){
				return(null);
			}				 			
		}
	}
}

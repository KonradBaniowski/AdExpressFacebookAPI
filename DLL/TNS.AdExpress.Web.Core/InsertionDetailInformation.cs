#region Informations
// Auteur: D. Mussuma, Y. Rkaina
// Cr�ation: 16/05/2006
// Modification:
#endregion

using System;
using System.Collections;

using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Web.Core{
	/// <summary>
	/// Description r�sum�e de InsertionDetailInformation.
	/// </summary>
	public class InsertionDetailInformation{
		#region Variables
		/// <summary>
		/// Les listes des modules autoris�es par  liste de colonnes
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.GenericColumnItemInformation</associates>
		protected static Hashtable _allowedModulesByDetailColums;
		/// <summary>
		/// Niveaux de d�tails orient�s support par d�faut
		/// </summary>
		///<link>aggregation</link>
		/// <supplierCardinality>0..*</supplierCardinality>
		/// <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		protected static Hashtable _defaultMediaDetailLevels;
		///<summary>
		/// El�ments orient� media de niveau de d�tail autoris�s
		/// </summary>
		///  <link>aggregation</link>
		///  <supplierCardinality>0..*</supplierCardinality>
		///  <associates>TNS.AdExpress.Web.Core.DetailLevelItemInformation</associates>
		protected static Hashtable _allowedMediaDetailLevelItems;
		/// <summary>
		/// Liste des d�tails de colonnes a charger
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
		/// <param name="source">source de donn�es</param>
		public static void Init(IDataSource source){
			DataAccess.InsertionDetailInformationDataAccess.Load(source,_allowedModulesByDetailColums,_detailColumnsList,_defaultMediaDetailLevels,_allowedMediaDetailLevelItems);
		}
		#endregion

		/// <summary>
		/// Retourne la liste des colonnes associ�s au m�dia
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
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
		/// Retourne la liste des niveaux de d�tail m�dia par d�faut pour un m�dia
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
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
		/// Retourne la liste des niveaux de d�tail m�dia autoris�s pour un m�dia
		/// </summary>
		/// <param name="idVehicle">Identifiant du m�dia</param>
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

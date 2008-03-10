#region Informations
// Auteur:  
// Date de création: 
// Date de modification: 15/06/2004 (G. Facon)
// G. Facon		11/08/2005 New Exception Management and property name
#endregion

using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using WebExceptions=TNS.AdExpress.Web.Core.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.DB;
/*
namespace TNS.AdExpress.Web.Core{


	///<summary>
	/// Classe pour gérer les droits Web
	/// </summary>
	///  <url>element://model:project::AdExpressDiagram/design:view:::etrnivz05f2fg2q_v</url>
	[System.Serializable]
	public class WebRight:TNS.AdExpress.Web.Core.DataAccess.WebRightDataAccess{
	
		#region variables
		/// <summary>
		/// hashtable : clé idFlag
		/// valeur : Flag
		/// </summary>
		protected Hashtable listFlags;
		#endregion

		#region Constructeur


		///<summary>
		/// Constructeur
		/// </summary>
		///  <url>element://model:project::TNS.AdExpress.Web/design:view:::mle9oy52bw9ww7p_v</url>
		///  <url>element://model:project::TNS.AdExpress.Web/design:view:::y1gc2wqpqsuxdyr_v</url>
		public WebRight(string login, string password,IDataSource source):base(login,password,source) {
			try{
				listFlags=LoadFlagRight(source);
			}
			catch(System.Exception err){
				throw(new WebExceptions.WebRightRulesException("Impossible de construire la liste des flag",err)); 
			}
		}

		#endregion

		#region Accesseurs
		/// <summary>
		/// Récupère le tableau avec les champs suivants
		/// 1ère colonne : idGroupModule 2ème colonne : idModule
		/// </summary>
		/// <returns>dtModule</returns>
		public DataTable ModuleList(){
			return moduleListDB(); 
		}
		
		/// <summary>
		/// Obtient la hashTable avec la liste des modules accessibles
		/// par l'utilisateur
		/// </summary>
		/// <returns></returns>
		public Hashtable HtModulesList{
			get {
				if(htModulesList==null || htModulesList.Count==0){
					moduleListDB(); 
				}
				return htModulesList;
			}
		}
	
		/// <summary>
		/// Obtient la liste des flags
		/// </summary>
		public Hashtable FlagsList{
			get{return listFlags;}
		}
		#endregion 
		
		#region Accès à un module
		/// <summary>
		/// Retourne l'objet module 
		/// </summary>
		/// <param name="idModule">idModule</param>
		/// <returns>Module</returns>
		public Module GetModule(Int64 idModule){
			if(htModulesList[idModule]!=null) {
				return  (Module)htModulesList[idModule];
			}
			else{return null;}
		}
	
		/// <summary>
		/// Obtient le nom du flag à partir de son identifiant
		/// </summary>
		/// <param name="idFlag">Identifiant du flag</param>
		/// <returns>nom du flag</returns>
		public string GetFlag(Int64 idFlag){
			if(listFlags[idFlag]!=null){
				return listFlags[idFlag].ToString();
			}else{
				return null;
			}
		}	
		#endregion

        #region Accès aux Créations d'un media
        /// <summary>
        /// Indique si le client a le droit de voir les créations d'un media
        /// </summary>
        /// <param name="vehicleId">Identifiant du media</param>
        /// <returns>True, s'il a le droit, false sinon</returns>
        public bool ShowCreatives(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names vehicleId) {
            switch (vehicleId) {
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                    return (FlagsList[Flags.ID_PRESS_CREATION_ACCESS_FLAG] != null);                    
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.radio:
                    return (FlagsList[Flags.ID_RADIO_CREATION_ACCESS_FLAG] != null);                             
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                    return (FlagsList[Flags.ID_TV_CREATION_ACCESS_FLAG] != null);                   
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internet:
                    return (FlagsList[Flags.ID_DETAIL_INTERNET_ACCESS_FLAG] != null);                    
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor:
                    return (FlagsList[Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG] != null);                    
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing:
                    return (FlagsList[Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG] != null);                    
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others:
                    return (FlagsList[Flags.ID_OTHERS_CREATION_ACCESS_FLAG] != null);                    
                case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                    return (FlagsList[Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG] != null);                    
                default:
                    return (false);
            }            
        }
        #endregion

    }
}
*/
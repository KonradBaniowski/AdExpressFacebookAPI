#region Informations
// Auteur: D. Mussuma
// Date de création: 21/07/2006
// Date de modification: 
//		
#endregion

using System;
using System.Data;
using System.Text;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst=TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.FrameWork.DB.Common;
namespace TNS.AdExpress.Web.DataAccess.Selections.Medias
{
	/// <summary>
	/// Description résumée de TitleListDataAccess.
	/// </summary>
	public class TitleListDataAccess
	{
		#region Titre
		/// <summary>
		/// Retourne la liste des Titres avec les supports associés
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Dataset avec 4 colonnes : id_title, title, id_media, media</returns>
		public static DataSet DetailTitleListDataAccess(WebSession webSession){
			
			#region Variables
		
			bool premier=true;			
			StringBuilder sql=new StringBuilder(1000);
            string activeMediaList = string.Empty;

			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.TITLE_PREFIXE+".id_title,"+DBConstantes.Tables.TITLE_PREFIXE+".title , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".title "+DBConstantes.Tables.TITLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".id_title="+DBConstantes.Tables.MEDIA_PREFIXE+".id_title");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet) {
                activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                if(activeMediaList.Length>0)
                    sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media in (" + activeMediaList + ")");
            }

			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0) {
				if(beginByAnd) sql.Append(" and");
				sql.Append(" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			// Droits en exclusion
			
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" ");
				sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category not in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryException]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media not in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaException]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.TITLE_PREFIXE+".title , "+DBConstantes.Tables.TITLE_PREFIXE+".id_title,"+DBConstantes.Tables.MEDIA_PREFIXE+".media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".id_media ");



			#endregion

			#region Execution de la requête
			try{
				 return webSession.Source.Fill(sql.ToString());				
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.TitleListDataAccessException("Impossible de charger la liste des titres avec les supports associés",err));
			}
			#endregion
			
		
		
		}

		/// <summary>
		/// Retourne la liste des Titres avec les supports associés ayant une partie du mot keyWord 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="keyWord">Mot clef</param>
		/// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
		/// <returns>Données</returns>
		public static DataSet keyWordTitleListDataAccess(WebSession webSession, string keyWord, string listMedia){
		
			#region Variables
			bool premier=true;			
			StringBuilder sql=new StringBuilder(1000);
            string activeMediaList = string.Empty;
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.TITLE_PREFIXE+".id_title,"+DBConstantes.Tables.TITLE_PREFIXE+".title , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".title "+DBConstantes.Tables.TITLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.TITLE_PREFIXE+".id_title="+DBConstantes.Tables.MEDIA_PREFIXE+".id_title");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Mot clé et médias déjà sélectionnés
			sql.Append("  and (title like UPPER('%"+keyWord+"%') ");
			sql.Append(" or media like UPPER('%"+keyWord+"%') ");
			if(listMedia.Length>0) {
				sql.Append(" or "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+listMedia+") ");
			}
			sql.Append(" ) ");
			
						
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet) {
                activeMediaList = TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
                if(activeMediaList.Length>0)
                    sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media in (" + activeMediaList + ")");
            }
		
			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0) {
				if(beginByAnd) sql.Append(" and");
				sql.Append(" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ");
				premier=false;
			}
			// Category
			//			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess).Length>0)
			//			{
			//				if(beginByAnd) sql.Append(" and");
			//				sql.Append(" (("+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess)+") ");
			//				premier=false;
			//			}
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			// Droits en exclusion
			//vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" ");
				sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category not in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryException]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media not in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaException]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.TITLE_PREFIXE+".title , "+DBConstantes.Tables.TITLE_PREFIXE+".id_title,"+DBConstantes.Tables.MEDIA_PREFIXE+".media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".id_media  ");



			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());				
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.TitleListDataAccessException("Impossible de charger la liste des titres avec les supports associés",err));
			}
			#endregion
		
			
		}
		#endregion
	}
}

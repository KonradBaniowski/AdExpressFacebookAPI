#region Informations
// Auteur: D. Mussuma
// Date de cr�ation: 22/01/2007
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
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpress.Web.DataAccess.Selections.Medias
{
	/// <summary>
	/// Charge la liste des pays.
	/// </summary>
	public class CountryListDataAccess
	{
		#region Pays
		/// <summary>
		/// Retourne la liste des pays avec les supports associ�s
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Dataset avec 4 colonnes : id_country, title, id_media, media</returns>
		public static DataSet DetailCountryListDataAccess(WebSession webSession){
			
			#region Variables	
		
			bool premier=true;			
			StringBuilder sql=new StringBuilder(1000);

			#endregion

			#region Requ�te
			sql.Append("Select distinct "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country,"+DBConstantes.Tables.COUNTRY_PREFIXE+".country , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".country "+DBConstantes.Tables.COUNTRY_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country="+DBConstantes.Tables.MEDIA_PREFIXE+".id_country");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

			//Condition univers des m�dias AdExpress en acc�s
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0) {
				if(beginByAnd) sql.Append(" and");
                sql.Append(" ((" + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.VEHICLE_PREFIXE + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess], true) + " ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.MEDIA_PREFIXE + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ");
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
                sql.Append(SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.VEHICLE_PREFIXE + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleException], false) + " ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.MEDIA_PREFIXE + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.COUNTRY_PREFIXE+".country , "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country,"+DBConstantes.Tables.MEDIA_PREFIXE+".media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".id_media ");



			#endregion

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());				
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.CountryListDataAccessException("Impossible de charger la liste des pays avec les supports associ�s",err));
			}
			#endregion
			
		
		
		}

		/// <summary>
		/// Retourne la liste des Pays avec les supports associ�s ayant une partie du mot keyWord 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="keyWord">Mot clef</param>
		/// <param name="listMedia">Liste des supports (media) d�j� s�lectionn�s</param>
		/// <returns>Donn�es</returns>
		public static DataSet keyWordCountryListDataAccess(WebSession webSession, string keyWord, string listMedia){
		
			#region Variables		
			bool premier=true;			
			StringBuilder sql=new StringBuilder(1000);
			#endregion

			#region Requ�te
			sql.Append("Select distinct "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country,"+DBConstantes.Tables.COUNTRY_PREFIXE+".country , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".country "+DBConstantes.Tables.COUNTRY_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country="+DBConstantes.Tables.MEDIA_PREFIXE+".id_country");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Mot cl� et m�dias d�j� s�lectionn�s
			sql.Append("  and (country like UPPER('%"+keyWord+"%') ");
			sql.Append(" or media like UPPER('%"+keyWord+"%') ");
			if(listMedia.Length>0) {
				sql.Append(" or "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+listMedia+") ");
			}
			sql.Append(" ) ");
			
						
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

	
			//Condition univers des m�dias AdExpress en acc�s
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0) {
				if(beginByAnd) sql.Append(" and");
                sql.Append(" ((" + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.VEHICLE_PREFIXE + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess], true) + " ");
				premier=false;
			}
			
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess], true) + " ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) {
				if(!premier) sql.Append(" or");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.MEDIA_PREFIXE + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess], true) + " ");
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
                sql.Append(SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.VEHICLE_PREFIXE + ".id_vehicle", webSession.CustomerLogin[CustomerRightConstante.type.vehicleException], false) + " ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category", webSession.CustomerLogin[CustomerRightConstante.type.categoryException], false) + " ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0) {
				if(!premier) sql.Append(" and");
				else {
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
                sql.Append(" " + SQLGenerator.GetInClauseMagicMethod(DBConstantes.Tables.MEDIA_PREFIXE + ".id_media", webSession.CustomerLogin[CustomerRightConstante.type.mediaException], false) + " ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.COUNTRY_PREFIXE+".country , "+DBConstantes.Tables.COUNTRY_PREFIXE+".id_country,"+DBConstantes.Tables.MEDIA_PREFIXE+".media ,"+DBConstantes.Tables.MEDIA_PREFIXE+".id_media  ");



			#endregion

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());				
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.CountryListDataAccessException("Impossible de charger la liste des pays avec les supports associ�s",err));
			}
			#endregion
		
			
		}
		#endregion
	}
}
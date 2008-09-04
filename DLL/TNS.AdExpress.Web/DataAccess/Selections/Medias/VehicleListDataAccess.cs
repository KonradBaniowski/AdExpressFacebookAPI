#region Informations
// Auteur: G. Facon
// Date de création: 11/05/2004
// Date de modification: 17/05/2004
//		29/10/2004 (G Facon intégration de la notion d'univers pour les alertes et recap)
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

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using WebNavigation=TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;

namespace TNS.AdExpress.Web.DataAccess.Selections.Medias{
	/// <summary>
	/// Charge la liste des Vehicles que peut sélectionner un client
	/// </summary>
	public class VehicleListDataAccess{
		
		#region Variables
		/// <summary>
		/// Liste des Vehicles que peut sélectionner un client
		/// </summary>
		protected DataTable _list;
		string sql = "";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		public VehicleListDataAccess(WebSession webSession){

			
				WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
				string sql="";
				bool isRecap = false;
				
				try {
			
					if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
					|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
						isRecap = true;

				sql = GetSqlQuery(webSession);
				
			}
			catch (System.Exception err) {
				throw (new VehicleListDataAccessException("Impossible to  build vehicle data access sql query", err));
			}
		

			#region Execution de la requête
			try{
				DataSet ds = GetSource(webSession).Fill(sql);
				if (!isRecap) {
					ds.Tables[0].Columns[0].ColumnName = "idVehicle";
					ds.Tables[0].Columns[1].ColumnName = "vehicle";
				}
				_list = ds.Tables[0];
			}
			catch(System.Exception err){
				throw (new VehicleListDataAccessException("Impossible to execute query",err));
			}
			#endregion			

		}
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient la liste des vehicles visible par le client
		/// </summary>
		public DataTable List{
			get{return(_list);}
		}
		#endregion

		#region Centre d'intéret
		/// <summary>
		/// Retourne la liste des centres d'intérêts avec les supports d'un utilisateur
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <returns>Dataset avec 4 colonnes : id_interest_center, interest_center, id_media, media</returns>
		public static DataSet DetailInterestCenterListDataAccess(WebSession webSession){
			
			#region Variables 
            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
			bool premier=true;
			DataSet dsListAdvertiser=null;
			StringBuilder sql=new StringBuilder(500);
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center,"+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center="+DBConstantes.Tables.MEDIA_PREFIXE+".id_interest_center");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");
			
            //Liste des supports actifs pour Internet
            if(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID)+ ")"); 

			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0)
			{
				if(beginByAnd) sql.Append(" and");
				sql.Append(" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0)
			{
				if(!premier) sql.Append(" or");
				else
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) 
			{
				if(!premier) sql.Append(" or");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			// Droits en exclusion
			
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" ");
				sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category not in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryException]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media not in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaException]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".media ");



			#endregion

			#region Execution de la requête
			try{
				dsListAdvertiser=webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName="dsListAdvertiser";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur",err));
			}
			#endregion			
		
			return dsListAdvertiser;
		}

		/// <summary>
		/// Retourne la liste des centres d'intérêts avec les supports d'un utilisateur ayant une partie du mot keyWord 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="keyWord">Mot clef</param>
		/// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
		/// <returns>Données</returns>
		public static DataSet keyWordInterestCenterListDataAccess(WebSession webSession, string keyWord, string listMedia){
		
			#region Variables
            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
			bool premier=true;
			DataSet dsListAdvertiser=null;
			StringBuilder sql=new StringBuilder(500);
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center,"+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center="+DBConstantes.Tables.MEDIA_PREFIXE+".id_interest_center");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Mot clé et médias déjà sélectionnés
			sql.Append("  and (interest_center like UPPER('%"+keyWord+"%') ");
			sql.Append(" or media like UPPER('%"+keyWord+"%') ");
			if(listMedia.Length>0)
			{
				sql.Append(" or "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+listMedia+") ");
			}
			sql.Append(" ) ");
			
						
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")"); 

			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));


			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0)
			{
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
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0)
			{
				if(!premier) sql.Append(" or");
				else
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) 
			{
				if(!premier) sql.Append(" or");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" ((");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			// Droits en exclusion
			//vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" ");
				sql.Append(DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category not in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryException]+") ");
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append(" (");
				}
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media not in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaException]+") ");
				premier=false;
			}
			if(!premier) sql.Append(" )");

			sql.Append(" order by "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".media ");



			#endregion

			#region Execution de la requête
			try{
				dsListAdvertiser=webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName="dsListMedia";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des centres d'intérêts avec les supports d'un utilisateur ayant une partie du mot keyWord",err));
			}
			#endregion		
		
			return dsListAdvertiser;
		}
		#endregion

		#region Régie
		/// <summary>
		/// Retourne la liste des régies avec les supports d'un utilisateur
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <returns>dataset avec 4 colonnes : id_media_seller, media_seller, id_media, media</returns>
		public static DataSet DetailMediaSellerListDataAccess(WebSession webSession){

			#region Variables
            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
			bool premier=true;
			DataSet dsListAdvertiser=null;
			StringBuilder sql=new StringBuilder(500);
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_media_seller,"+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_seller "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_media_seller="+DBConstantes.Tables.MEDIA_PREFIXE+".id_media_seller");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")"); 


			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession)) 
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			//vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0)
			{
				if(beginByAnd) sql.Append(" and");
				sql.Append(" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0)
			{
				if(!premier) sql.Append(" or");
				else
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
			//			// Category
//			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess).Length>0) {
//				if(beginByAnd) sql.Append(" and");
//				sql.Append(" (("+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess)+") ");
//				premier=false;
//			}
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
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
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

			sql.Append(" order by "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller , "+DBConstantes.Tables.MEDIA_PREFIXE+".media ");



			#endregion

			#region Execution de la requête
			try{
				dsListAdvertiser=webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName="dsListAdvertiser";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des régies avec les supports d'un utilisateur",err));
			}
			#endregion			
		
			return dsListAdvertiser;
		}
	
		/// <summary>
		/// Retourne la liste des régies avec les supports d'un utilisateur ayant une partie du mot keyWord 
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="keyWord">Mot clef</param>
		/// <param name="listMedia">Liste des supports (media) déjà sélectionnés</param>
		/// <returns>Données</returns>
		public static DataSet keyWordMediaSellerListDataAccess(WebSession webSession, string keyWord, string listMedia) {
		
			#region Variables
            OracleConnection connection=(OracleConnection)webSession.Source.GetSource();
			bool premier=true;
			DataSet dsListAdvertiser=null;
			StringBuilder sql=new StringBuilder(500);
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_media_seller,"+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_seller "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_media_seller="+DBConstantes.Tables.MEDIA_PREFIXE+".id_media_seller");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			// Mot clé et médias déjà sélectionnés
			sql.Append("  and (media_seller like UPPER('%"+keyWord+"%') ");
			sql.Append(" or media like UPPER('%"+keyWord+"%') ");
			if(listMedia.Length>0) {
				sql.Append(" or "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+listMedia+") ");
			}
			sql.Append(" ) ");
			
						
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");

            //Liste des supports actifs pour Internet
            if (((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID == (long)VehicleClassificationCst.internet)
                sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media in (" + TNS.AdExpress.Web.Core.ActiveMediaList.GetActiveMediaList(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID) + ")"); 

			//Condition univers des médias AdExpress en accès
			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession))  
				sql.Append(WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true));

			
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0)
			{
				if(beginByAnd) sql.Append(" and");
				sql.Append(" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ");
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0)
			{
				if(!premier) sql.Append(" or");
				else
				{
					if(beginByAnd) sql.Append(" and");
					sql.Append("((");
				}
				sql.Append(" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ");
				premier=false;
			}
// Category
//			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess).Length>0) {
//				if(beginByAnd) sql.Append(" and");
//				sql.Append(" (("+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess)+") ");
//				premier=false;
//			}
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
			//Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0)
			{
				if(!premier) sql.Append(" and");
				else 
				{
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

			sql.Append(" order by "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller , "+DBConstantes.Tables.MEDIA_PREFIXE+".media ");



			#endregion

			#region Execution de la requête
			try{
				dsListAdvertiser=webSession.Source.Fill(sql.ToString());
				dsListAdvertiser.Tables[0].TableName="dsListMedia";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des régies avec les supports d'un utilisateur ayant une partie du mot keyWord",err));
			}
			#endregion
			
		
			return dsListAdvertiser;
		}
		#endregion

		#region Media/Centre d'intérêts/supports
		/// <summary>
		/// Donne la liste des Media/Centre d'intérêts/supports en fonctions des droits de l'utilisateur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>dataset avec 6 colonnes : id_vehicle,vehicle,id_interest_center, interest_center, id_media, media</returns>
		public static DataSet VehicleInterestCenterMediaListDataAccess(WebSession webSession){		
			return VehicleInterestCenterMediaListDataAccess(webSession,"","","");
		}

		/// <summary>
		/// Donne la liste des Media/Centre d'intérêts/supports en fonctions des droits de l'utilisateur
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="listVehicle">liste des média (vehicle) déjà sélectionnés</param>
		/// <returns>dataset avec 6 colonnes : id_vehicle,vehicle,id_interest_center, interest_center, id_media, media</returns>
		public static DataSet VehicleInterestCenterMediaListDataAccess(WebSession webSession,string listVehicle){		
			return VehicleInterestCenterMediaListDataAccess(webSession,listVehicle,"","");
		}
		/// Donne la liste des Media/Centre d'intérêts/supports en fonctions des droits de l'utilisateur
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="listInterestCenter">liste des centres d'intérêts déjà sélectionnés</param>
		/// <param name="listMedia">liste des supports (media) déjà sélectionnés</param>	
		/// <returns>dataset avec 6 colonnes : id_vehicle,vehicle,id_interest_center, interest_center, id_media, media</returns>
		public static DataSet VehicleInterestCenterMediaListDataAccess(WebSession webSession, string listInterestCenter, string listMedia) {
			return VehicleInterestCenterMediaListDataAccess(webSession, "", listInterestCenter, listMedia);
		}
		/// <summary>
		/// Donne la liste des Media/Centre d'intérêts/supports en fonctions des droits de l'utilisateur
		/// </summary>
		/// <param name="webSession">Session du client</param>	
		/// <param name="listVehicle">liste des média (vehicle) déjà sélectionnés</param>
		/// <param name="listInterestCenter">liste des centres d'intérêts déjà sélectionnés</param>
		/// <param name="listMedia">liste des supports (media) déjà sélectionnés</param>	
		/// <returns>dataset avec 6 colonnes : id_vehicle,vehicle,id_interest_center, interest_center, id_media, media</returns>
		public static DataSet VehicleInterestCenterMediaListDataAccess(WebSession webSession,string listVehicle,string listInterestCenter,string listMedia) {
			
			#region Variables
			StringBuilder sql=new StringBuilder(500);
			bool beginByAnd = true;
			string sql2="";
			DataSet ds;
			#endregion

			WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			Table vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
			Table categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);
			Table interestCenterTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter);
			Table basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);
			Table mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);

			#region Requête
			sql.Append("Select distinct " + vehicleTable.Prefix + ".id_vehicle," + vehicleTable.Prefix + ".vehicle," + interestCenterTable.Prefix + ".id_interest_center," + interestCenterTable.Prefix + ".interest_center , " + mediaTable.Prefix + ".id_media , " + mediaTable.Prefix + ".media");
			sql.Append(" from " + vehicleTable.SqlWithPrefix + "," + interestCenterTable.SqlWithPrefix + "," + mediaTable.SqlWithPrefix + " , " + categoryTable.SqlWithPrefix + ", " + basicMediaTable.SqlWithPrefix + " ");
			sql.Append(" where");
			// Langue
			sql.Append(" " + vehicleTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString());
			sql.Append(" and " + interestCenterTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString());
			sql.Append(" and " + mediaTable.Prefix+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+ basicMediaTable.Prefix + ".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+ categoryTable.Prefix +".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and " + vehicleTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + interestCenterTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + mediaTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + categoryTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and " + basicMediaTable.Prefix + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle");
			sql.Append(" and " + interestCenterTable.Prefix + ".id_interest_center=" + DBConstantes.Tables.MEDIA_PREFIXE + ".id_interest_center");
			sql.Append(" and " + categoryTable.Prefix + ".id_category=" + basicMediaTable.Prefix + ".id_category");
			sql.Append(" and " + basicMediaTable.Prefix + ".id_basic_media=" + mediaTable.Prefix + ".id_basic_media");
			
			#region Media selection
			bool premier=true;
			// vehicle 
			if (listVehicle != null && listVehicle.Length > 0) {
				if(beginByAnd) sql2+=" and ";
				sql2+=" "+vehicleTable.Prefix+".id_vehicle in (  "+listVehicle+" ) ";				
			}
			// interest Center 
			if (listInterestCenter != null && listInterestCenter.Length > 0) {
				if(beginByAnd){ sql2+=" and ";					
					sql2+=" ( ";
				}
				sql2+=" "+interestCenterTable.Prefix+".id_interest_center in ( "+listInterestCenter+" ) ";
				premier=false;
			}
			// media 
			if (listMedia != null && listMedia.Length>0) {
				if(!premier) sql2+=" or ";
				else{
					if(beginByAnd) sql2+=" and ";
					sql2+=" ( ";
				}
				sql2+=" "+mediaTable.Prefix+".id_media in ("+listMedia+" ) ";
				premier=false;
			}

			if(!premier) sql2+=" )";

			if(sql2.Length>0)sql.Append(sql2);
			#endregion

			#region Media universe
			//Media universe
			if (module != null)
				sql.Append(module.GetAllowedMediaUniverseSql(vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, true));
			#endregion
		

			#region Media rights
			sql.Append(WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession, vehicleTable.Prefix, categoryTable.Prefix, mediaTable.Prefix, true));
			#endregion			
			
			sql.Append(" order by  "+vehicleTable.Prefix+".vehicle , "+interestCenterTable.Prefix+".interest_center , "+mediaTable.Prefix+".media ");

			#endregion

			#region Execution query
			try{
				ds = webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw (new VehicleListDataAccessException("Impossible to exectue query", err));
			}
			#endregion			

			return(ds);	
		}
		#endregion

		#region Liste des centre d'interet en fonction des droits du client
		/// <summary>
		///Donne une liste de centre d'interet en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Liste des centre d'interetpour autorisées au client.</returns>
		public static DataSet InterestCenterList(WebSession webSession){
			return InterestCenterList(webSession,"");
		}

		/// <summary>
		///Donne une liste de centre d'interet en fonction des droits du client. 
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idInterestCenter">identifiant(s) de(s) centre d'interet(s)</param>
		/// <returns>Liste des centre d'interetautorisées au client.</returns>
		public static DataSet InterestCenterList(WebSession webSession,string idInterestCenter){			
			DataSet dsListAdvertiser=InterestCenterList(webSession,idInterestCenter,"");
			return dsListAdvertiser;
		}

		/// <summary>
		///Donne une liste de centre d'interet en fonction des droits du client
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idInterestCenter">identifiant(s) de(s) centre d'interet(s)</param>
		/// <param name="idMedia">identifiant(s) de(s) support(s) sélectionné(s)</param>
		/// <returns>Liste des centre d'interet autorisées au client.</returns>
		public static DataSet InterestCenterList(WebSession webSession,string idInterestCenter,string idMedia) {
			
			#region Variables
			OracleConnection connection=(OracleConnection)webSession.CustomerLogin.Source.GetSource();
			//bool premier=true;
			DataSet dsListAdvertiser=null;
			StringBuilder sql=new StringBuilder(500);
			#endregion

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center,"+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center  ");
			//+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from  "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.DataLanguage.ToString());
			// Activation
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);	
			// Jointure
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle");
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center="+DBConstantes.Tables.MEDIA_PREFIXE+".id_interest_center");
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_category");
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media="+DBConstantes.Tables.MEDIA_PREFIXE+".id_basic_media");
			//sélection média 
			if(WebFunctions.CheckedText.IsStringEmpty(idInterestCenter) || WebFunctions.CheckedText.IsStringEmpty(idMedia) ) {
				sql.Append("  and  ( ");
				if(idInterestCenter.Length>0)
					sql.Append(" "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".Id_interest_center in ("+idInterestCenter+")");	
				if(idInterestCenter.Length>0 && idMedia.Length>0)sql.Append(" or ");
				if(idMedia.Length>0)
				sql.Append(" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+idMedia+")");	
				sql.Append(" ) ");
			}
			
			// Vehicle
			sql.Append(" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID+"");
			
			#region droits média
			sql.Append(WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession,DBConstantes.Tables.VEHICLE_PREFIXE,DBConstantes.Tables.CATEGORY_PREFIXE,DBConstantes.Tables.MEDIA_PREFIXE,true));
			#endregion

			//Order by
			sql.Append(" order by "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center  ");

			#endregion

			#region Execution de la requête
			try{
				dsListAdvertiser=webSession.Source.Fill(sql.ToString());
				if(dsListAdvertiser!=null)dsListAdvertiser.Tables[0].TableName="dsListAdvertiser";
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste de centre d'interet en fonction des droits du client",err));
			}
			#endregion
		
			return dsListAdvertiser;
		}

		/// <summary>
		/// retourne la liste de centres d'interet
		/// </summary>
		/// <param name="webSession"> session client</param>
		/// <returns>Liste des centre d'interet sélectionnées</returns>
		public static DataSet getListInterestCenterSelected(WebSession webSession){
			string listInterestCenter =webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.interestCenterAccess);		
			string listMedia =webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.mediaAccess);													
			return InterestCenterList(webSession,listInterestCenter,listMedia);			
		}
		#endregion					

		#region Recap sql
		/// <summary>
		/// Get source
		/// </summary>
		/// <param name="webSession">Web session</param>
		/// <returns>IDataSource</returns>
		protected IDataSource GetSource(WebSession webSession) {
			switch (webSession.CurrentModule) {
				case TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR :
				case TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE :
					string nlsSort = "";
				bool isUTF8 = false;
				if (WebApplicationParameters.AllowedLanguages.ContainsKey(long.Parse(webSession.DataLanguage.ToString()))) {
					nlsSort = WebApplicationParameters.AllowedLanguages[long.Parse(webSession.DataLanguage.ToString())].NlsSort;
				} 
                return WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis,nlsSort);
			default: return webSession.Source;
			}
		}
		/// <summary>
		/// Get recap media conditions
		/// </summary>
		/// <param name="webSession">Web session</param>
		/// <returns>string sql</returns>
		protected string GetRecapMediaConditions(WebSession webSession, Table vehicleTable, Table categoryTable, Table mediaTable ) {
			string sql = "";

			#region media rigths conditions
			sql += WebFunctions.SQLGenerator.getAccessVehicleList(webSession, vehicleTable.Prefix, true);
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
                && !webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
            {
                sql += " and " + categoryTable.Prefix+ ".id_category not in (68) ";
            }
			#endregion

			return sql;
		}

		#endregion

		#region GetMediaRights
		/// <summary>
		/// Get media rights
		/// </summary>
		/// <param name="webSession">web session</param>
		/// <param name="categoryTable">category Table</param>
		/// <param name="mediaTable">media Table</param>
		/// <param name="vehicleTable">vehicle Table</param>
		/// <returns>string sql</returns>
		protected string GetMediaRights(WebSession webSession,Table vehicleTable, Table categoryTable, Table mediaTable) {
			string sql = "";
			#region Droits en accès
			bool premier = true;
			bool beginByAnd = true;
			// le bloc doit il commencer par AND
			// Vehicle
			if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length > 0) {
				if (beginByAnd) sql += " and";
				sql += " ((" + vehicleTable.Prefix + ".id_vehicle in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess] + ") ";
				premier = false;
			}
			// Category
			if (webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + categoryTable.Prefix + ".id_category in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess] + ") ";
				premier = false;
			}
			// Media
			if (webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length > 0) {
				if (!premier) sql += " or";
				else {
					if (beginByAnd) sql += " and";
					sql += " ((";
				}
				sql += " " + mediaTable.Prefix + ".id_media in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess] + ") ";
				premier = false;
			}
			if (!premier) sql += " )";

			// Droits en exclusion
			// Vehicle
			if (webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + vehicleTable.Prefix + ".id_vehicle not in (" + webSession.CustomerLogin[CustomerRightConstante.type.vehicleException] + ") ";
				premier = false;
			}
			// Category
			if (webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + categoryTable.Prefix + ".id_category not in (" + webSession.CustomerLogin[CustomerRightConstante.type.categoryException] + ") ";
				premier = false;
			}
			// Media
			if (webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length > 0) {
				if (!premier) sql += " and";
				else {
					if (beginByAnd) sql += " and";
					sql += " (";
				}
				sql += " " + mediaTable.Prefix + ".id_media not in (" + webSession.CustomerLogin[CustomerRightConstante.type.mediaException] + ") ";
				premier = false;
			}
			if (!premier) sql += " )";
			#endregion

			return sql;
		}
		#endregion

		#region GetTables

		protected void GetTables(WebSession webSession, Table vehicleTable, Table categoryTable, Table basicMediaTable, Table mediaTable) {
			if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
				|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE) {
				vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
				categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
				mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
			}
			else {
				vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
				categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);
				basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);
				mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);
			}
		}
		#endregion

		#region GetSqlQuery
		/// <summary>
		/// Get sql query
		/// </summary>
		/// <param name="webSession">Client Session </param>
		/// <returns>Sql query</returns>
		protected string GetSqlQuery(WebSession webSession) {
			string sql = "";
			#region Variables
			bool premier=true;
			#endregion

			
			bool isRecap = false;

			WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
			Table vehicleTable = null, categoryTable = null, basicMediaTable = null, mediaTable = null;

			if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
					|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
				isRecap = true;

			if (isRecap) {
				vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
				categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
				mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
			}
			else {
				vehicleTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.vehicle);
				categoryTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.category);
				basicMediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.basicMedia);
				mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.media);
			}

			#region Requête
			
				
			sql = "Select distinct " + vehicleTable.Prefix + ".id_vehicle," + vehicleTable.Prefix + ".vehicle ";
			if (isRecap) sql += ", " + categoryTable.Prefix + ".id_category," + categoryTable.Prefix + ".category, " + mediaTable.Prefix + ".id_media ," + mediaTable.Prefix + ".media";
			sql += " from " + vehicleTable.SqlWithPrefix + ",";
			sql += categoryTable.SqlWithPrefix + ",";
			if(!isRecap) sql += basicMediaTable.SqlWithPrefix + ",";
			sql += mediaTable.SqlWithPrefix + " ";
			sql += " where";
			// Langue
			sql += " " + vehicleTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
			sql += " and " + categoryTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
			if (!isRecap) sql += " and " + basicMediaTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
			sql += " and " + mediaTable.Prefix + ".id_language=" + webSession.DataLanguage.ToString();
			// Activation
			sql += " and " + vehicleTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;
			sql += " and " + categoryTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;
			if (!isRecap) sql += " and " + basicMediaTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;
			sql += " and " + mediaTable.Prefix + ".activation<" + DBConstantes.ActivationValues.UNACTIVATED;

			// Jointure
			if (isRecap) {
				sql += " and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle";
				sql += " and " + categoryTable.Prefix + ".id_category=" + mediaTable.Prefix + ".id_category";
			}
			else {
				sql += " and " + vehicleTable.Prefix + ".id_vehicle=" + categoryTable.Prefix + ".id_vehicle";
				sql += " and " + categoryTable.Prefix + ".id_category=" + basicMediaTable.Prefix + ".id_category";
				sql += " and " + basicMediaTable.Prefix + ".id_basic_media=" + mediaTable.Prefix + ".id_basic_media";
			}
			#endregion

			#region Media universe
			//Media universe
			if (module != null)
				sql += module.GetAllowedMediaUniverseSql(vehicleTable.Prefix,categoryTable.Prefix,mediaTable.Prefix, true);
			#endregion

			#region Media Rights
			if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
				|| webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
				sql += GetRecapMediaConditions(webSession, vehicleTable, categoryTable, mediaTable);
			else
				sql += GetMediaRights(webSession, vehicleTable, categoryTable, mediaTable);
			#endregion

			sql += " order by " + vehicleTable.Prefix + ".vehicle,"+vehicleTable.Prefix + ".id_vehicle";
			if (isRecap) sql += ", " + categoryTable.Prefix + ".category," + categoryTable.Prefix + ".id_category, " + mediaTable.Prefix + ".media ," + mediaTable.Prefix + ".id_media";
			return sql;
		}

		#endregion

	}
}

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
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;

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
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="webSession">Session du client</param>
		public VehicleListDataAccess(WebSession webSession){

			#region Variables
			bool premier=true;
			#endregion

			#region Requête
			string sql="Select distinct "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle,"+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle ";
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+",";
			sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+",";
			sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+",";
			sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" ";
			sql+=" where";
			// Langue
			sql+=" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			// Activation
			sql+=" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;	
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;	
			sql+=" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;	
			sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;	

			// Jointure
			sql+=" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle=ct.id_vehicle";
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category=bm.id_category";
			sql+=" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_basic_media=md.id_basic_media";

			//Univers des véhicles AdExpress en accès

			//condition added to exclude outdoor from tendances
			if(webSession.CurrentModule==WebConstantes.Module.Name.TENDACES)
			{
				sql+=" and vh.id_vehicle not in(8)";
			}
			#region ancienne version
//			if(WebFunctions.Modules.IsSponsorShipTVModule(webSession) )
//				sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID,true);
//			else 
			//sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(WebConstantes.AdExpressUniverse.ALERTE_MEDIA_LIST_ID,true);
			#endregion
			sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,true);
			
			#region Droits en accès
			premier=true;
			bool beginByAnd=true;
			// le bloc doit il commencer par AND
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess].Length>0)
			{
				if(beginByAnd) sql+=" and";
				sql+=" (("+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleAccess]+") ";
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess].Length>0)
			{
				if(!premier) sql+=" or";
				else 
				{
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryAccess]+") ";
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess].Length>0) 
			{
				if(!premier) sql+=" or";
				else 
				{
					if(beginByAnd) sql+=" and";
					sql+=" ((";
				}
				sql+=" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaAccess]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";

			// Droits en exclusion
			// Vehicle
			if(webSession.CustomerLogin[CustomerRightConstante.type.vehicleException].Length>0)
			{
				if(!premier) sql+=" and";
				else 
				{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle not in ("+webSession.CustomerLogin[CustomerRightConstante.type.vehicleException]+") ";
				premier=false;
			}
			// Category
			if(webSession.CustomerLogin[CustomerRightConstante.type.categoryException].Length>0)
			{
				if(!premier) sql+=" and";
				else 
				{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category not in ("+webSession.CustomerLogin[CustomerRightConstante.type.categoryException]+") ";
				premier=false;
			}
			// Media
			if(webSession.CustomerLogin[CustomerRightConstante.type.mediaException].Length>0)
			{
				if(!premier) sql+=" and";
				else 
				{
					if(beginByAnd) sql+=" and";
					sql+=" (";
				}
				sql+=" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media not in ("+webSession.CustomerLogin[CustomerRightConstante.type.mediaException]+") ";
				premier=false;
			}
			if(!premier) sql+=" )";
			#endregion

			sql+=" order by vh.vehicle";

			#endregion

			#region Execution de la requête
			try{
				DataSet ds = webSession.Source.Fill(sql.ToString());
				ds.Tables[0].Columns[0].ColumnName="idVehicle";
				ds.Tables[0].Columns[1].ColumnName="vehicle";
				_list = ds.Tables[0];
			}
			catch(System.Exception err){
				throw (new VehicleListDataAccessException("Impossible de charger la liste des vehicles visible par le client",err));
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
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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

			#region Requête
			sql.Append("Select distinct "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle,"+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center,"+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media , "+DBConstantes.Tables.MEDIA_PREFIXE+".media");
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DBConstantes.Tables.MEDIA_PREFIXE+" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DBConstantes.Tables.CATEGORY_PREFIXE+", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+" ");
			sql.Append(" where");
			// Langue
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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
			
			#region sélection média
			bool premier=true;
			// vehicle (media)
			if(WebFunctions.CheckedText.IsStringEmpty(listVehicle)){
				if(beginByAnd) sql2+=" and ";
				sql2+=" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle in (  "+listVehicle+" ) ";				
			}
			// interest Center (centre d'intérêt)
			if(WebFunctions.CheckedText.IsStringEmpty(listInterestCenter)){
				if(beginByAnd){ sql2+=" and ";					
					sql2+=" ( ";
				}
				sql2+=" "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_interest_center in ( "+listInterestCenter+" ) ";
				premier=false;
			}
			// media (support)
			if(WebFunctions.CheckedText.IsStringEmpty(listMedia)){
				if(!premier) sql2+=" or ";
				else{
					if(beginByAnd) sql2+=" and ";
					sql2+=" ( ";
				}
				sql2+=" "+DBConstantes.Tables.MEDIA_PREFIXE+".id_media in ("+listMedia+" ) ";
				premier=false;
			}

			if(!premier) sql2+=" )";

			if(sql2.Length>0)sql.Append(sql2);
			#endregion

			#region droits média
			sql.Append(WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession,DBConstantes.Tables.VEHICLE_PREFIXE,DBConstantes.Tables.CATEGORY_PREFIXE,DBConstantes.Tables.MEDIA_PREFIXE,true));
			#endregion			
			
			sql.Append(" order by  "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle , "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".interest_center , "+DBConstantes.Tables.MEDIA_PREFIXE+".media ");

			#endregion

			#region Execution de la requête
			try{
				ds = webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw (new VehicleListDataAccessException("Impossible de charger la liste des Media/Centre d'intérêts/supports en fonctions des droits d'un utilisateur",err));
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
			sql.Append(" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.INTEREST_CENTER_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.BASIC_MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
			sql.Append(" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString());
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
				
		#region Choix media pour Analyse Sectorielle
		/// <summary>
		/// Dataset retournant la liste des média, catégories, support
		/// en fonction des droits d'un utilisateur
		/// </summary>
		/// <param name="webSession">webSession</param>
		/// <returns>dataset</returns>
		public static DataSet VehicleCatMediaListDataAccess(WebSession webSession){

			#region Construction de la requête
			string sql="";
			string listCategory="";

			sql+=" select "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle,"+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category,"+DBConstantes.Tables.CATEGORY_PREFIXE+".category,"+DBConstantes.Tables.MEDIA_PREFIXE+".id_media,"+DBConstantes.Tables.MEDIA_PREFIXE+".media";
			sql+=" from vehicle "+DBConstantes.Tables.VEHICLE_PREFIXE+", category "+DBConstantes.Tables.CATEGORY_PREFIXE+",media "+DBConstantes.Tables.MEDIA_PREFIXE+"";
			sql+=" Where";
			// Langue
			sql+=" "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".id_language="+webSession.SiteLanguage.ToString();
			// Activation
			sql+=" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".activation<"+DBConstantes.ActivationValues.DEAD;
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".activation<"+DBConstantes.ActivationValues.DEAD;
			sql+=" and "+DBConstantes.Tables.MEDIA_PREFIXE+".activation<"+DBConstantes.ActivationValues.DEAD;
			// jointure
			sql+=" and "+DBConstantes.Tables.VEHICLE_PREFIXE+".id_vehicle="+DBConstantes.Tables.CATEGORY_PREFIXE+".id_vehicle";
			sql+=" and "+DBConstantes.Tables.CATEGORY_PREFIXE+".id_category="+DBConstantes.Tables.MEDIA_PREFIXE+".id_category";			
			
			// Sélection des médias (on ne devrait plus en avoir besoin grâce au filtre des média
			
			//Univers des véhicles AdExpress en accès
			sql+=WebFunctions.SQLGenerator.getAdExpressUniverseCondition(webSession,WebConstantes.AdExpressUniverse.RECAP_MEDIA_LIST_ID,true);			

			#region Nomenclature Produit (droits)
			//Droits en accès
			//sql+=WebFunctions.SQLGenerator.getClassificationCustomerRecapMediaRight(webSession,true);
			#endregion
			
			#region Droits médias spécifique au media
			sql+=WebFunctions.SQLGenerator.getAccessVehicleList(webSession,DBConstantes.Tables.VEHICLE_PREFIXE,true);

			#endregion


			// Tri
			sql+=" order by "+DBConstantes.Tables.VEHICLE_PREFIXE+".vehicle,"+DBConstantes.Tables.CATEGORY_PREFIXE+".category,"+DBConstantes.Tables.MEDIA_PREFIXE+".media";

			#endregion
			
			#region Execution de la requête
			try{
                IDataSource source=WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis); 
				return(source.Fill(sql.ToString()));
			}
			catch(System.Exception err){
				throw (new TNS.AdExpress.Web.Exceptions.VehicleListDataAccessException("Impossible de charger la liste des régies avec les supports d'un utilisateur ayant une partie du mot keyWord",err));
			}
			#endregion
		}
		#endregion

	}
}

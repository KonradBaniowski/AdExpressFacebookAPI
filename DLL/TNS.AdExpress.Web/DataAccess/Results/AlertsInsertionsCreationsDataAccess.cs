#region Informations
// Auteur: D. V. Mussuma	
// Date de création: 14/02/2007
// Date de modification:
#endregion

#region Using
using System;
using System.Text;
using System.Collections;
using System.Collections.Specialized;
using System.Data;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.FrameWork.DB.Common;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using WebDataAccess=TNS.AdExpress.Web.DataAccess;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using ConstantesCustomer=TNS.AdExpress.Constantes.Customer;
using WebFunctions = TNS.AdExpress.Web.Functions;
using  ConstantesFrameWork=TNS.AdExpress.Constantes.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Classification;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Extrait le détail des insertions publicitaires dans un support pour les Alertes mail
	/// </summary>
	public class AlertsInsertionsCreationsDataAccess{

		#region Méthodes publiques
		/// <summary>
		/// Extrait le détail des insertions publicitaires dans un média 
		///		Extraction de la table attaquée et des champs de sélection à partir du vehicle
		///		Construction et exécution de la requête
		///		
		/// </summary>
		/// <param name="dataSource">Source de données</param>		
		/// <param name="dateBegin">Date de début de l'étude</param>
		/// <param name="dateEnd">Date de fin de l'étude</param>
		/// <param name="idMedia">Identifiant du support </param>
		/// <param name="idProduct">Identifiant du produit </param>
		/// <param name="idVehicle">Identifiant du média (Vehicle) sélectionné par le client</param>
		/// <param name="siteLanguage">Language du site</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand on ne sait pas quelle table attaquer, quels champs sélectionner ou quand une erreur Oracle s'est produite
		/// </exception>
		/// <returns>DataSet contenant le résultat de la requête</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		- <code>private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail)</code> : obtient les champs de la requête.
		///		- <code>private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle,WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails preformatedMediaDetail)</code> : obtient les tables de données pour la requête
		///		- <code>private static void GetJoinConditions(WebSession webSession,StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd)</code> : obtient les jointures de la requête
		///		- <code> public static string TNS.AdExpress.Web.DataAccess.GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion,bool isMediaDetail,string prefixeMediaPlanTable)</code> : obtient liodre de tri des résultats
		/// </remarks>
		public static DataSet GetData(IDataSource dataSource,string idMedia,string idProduct,string dateBegin,string dateEnd,Int64 idVehicle,string siteLanguage){

				
			#region Variables
			string fields = "";
//			string list="";
//			bool premier = true;

			StringBuilder sql= new StringBuilder(5000);

			#endregion

			#region Construction de la requête
				try{

					fields = GetFields(VehiclesInformation.DatabaseIdToEnum(idVehicle), DbTables.WEB_PLAN_PREFIXE);
					// Sélection de la nomenclature Support
					sql.Append("select " + fields);

					// Tables TODO ADNETTRACK
					sql.Append(" from ");
					sql.Append(GetTables(VehiclesInformation.DatabaseIdToEnum(idVehicle)));

					// Conditions de jointure
					sql.Append(" Where ");
					GetJoinConditions(sql, VehiclesInformation.DatabaseIdToEnum(idVehicle), DbTables.WEB_PLAN_PREFIXE, false, siteLanguage);

					//Sélection produit
					sql.Append(" and  "+DbTables.WEB_PLAN_PREFIXE+".id_product="+idProduct);

					//Sélection support
					sql.Append(" and  "+DbTables.WEB_PLAN_PREFIXE+".id_media="+idMedia);

					// Période
					sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".date_media_num>="+dateBegin);
					sql.Append(" and "+DbTables.WEB_PLAN_PREFIXE+".date_media_num<="+dateEnd);

					// Ordre
					sql.Append(" order by " + GetInsertionsOrder(VehiclesInformation.DatabaseIdToEnum(idVehicle),DbTables.WEB_PLAN_PREFIXE));

				}catch(System.Exception err){
					throw(new AlertsInsertionsCreationsDataAccessException("Impossible de construire la requête",err));
				}
			#endregion

			#region Execution de la requête
			try{
				return dataSource.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new AlertsInsertionsCreationsDataAccessException ("Impossible de charger pour les insertions: "+sql,err));
			}
			#endregion
		}

		#region identification des média (vehicles) à traiter
		
		/// <summary>
		/// Obtient le média (vehicle) correspondant au support sélectionné.
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="idMedia">Identifiant support</param>
		/// <param name="siteLanguage">Language du site</param>
		/// <returns>Média (vehicle)</returns>
		public static DataSet GetIdVehicle(IDataSource dataSource,string idMedia,string siteLanguage){

			StringBuilder sql=new StringBuilder(500);			
            //DataSet ds = null;
			
			#region Construction de la requête
			
			sql.Append(" select distinct "+DbTables.VEHICLE_PREFIXE+".id_vehicle,vehicle from ");
			sql.Append(" "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.VEHICLE+" "+DbTables.VEHICLE_PREFIXE);			
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.CATEGORY+" "+DbTables.CATEGORY_PREFIXE);						
			sql.Append(" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".basic_media "+DbTables.BASIC_MEDIA_PREFIXE);
			sql.Append(" , "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DbTables.MEDIA_PREFIXE);
			
			
			sql.Append(" Where ");	
		
			//Langages
			sql.Append("  "+DbTables.VEHICLE_PREFIXE+".id_language="+siteLanguage);
			sql.Append(" and  "+DbTables.CATEGORY_PREFIXE+".id_language="+siteLanguage);			
			sql.Append(" and "+DbTables.MEDIA_PREFIXE+".id_language="+siteLanguage);			
			sql.Append(" and "+DbTables.BASIC_MEDIA_PREFIXE+".id_language="+siteLanguage);
		

			//Jointures
			sql.Append("  and "+DbTables.VEHICLE_PREFIXE+".id_vehicle ="+DbTables.CATEGORY_PREFIXE+".id_vehicle");	
			sql.Append("  and "+DbTables.BASIC_MEDIA_PREFIXE+".id_basic_media ="+DbTables.MEDIA_PREFIXE+".id_basic_media");
			sql.Append("  and "+DbTables.BASIC_MEDIA_PREFIXE+".id_category ="+DbTables.CATEGORY_PREFIXE+".id_category");
			sql.Append("  and "+DbTables.MEDIA_PREFIXE+".id_media= "+idMedia);
									
			//Activation
			sql.Append(" and "+DbTables.VEHICLE_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);			
			sql.Append(" and "+DbTables.CATEGORY_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);						
			sql.Append(" and "+DbTables.MEDIA_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DbTables.BASIC_MEDIA_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
			

			sql.Append(" order by vehicle");
			#endregion

			#region Execution de la requête
			try{
				return  dataSource.Fill(sql.ToString());
				
			}
			catch(System.Exception err){
				throw(new AlertsInsertionsCreationsDataAccessException ("Impossible d'identifier le média : "+sql,err));
			}
			#endregion

		}
		#endregion

		#region Detail produit
		/// <summary>
		/// Obtient les informations du produit
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="siteLanguage">Langue du site</param>
		/// <returns>informations du produit</returns>
		public static DataSet GetProductInformations(IDataSource dataSource,string idProduct,string siteLanguage){

			System.Text.StringBuilder sql = new System.Text.StringBuilder(1000);

			sql.Append ("  SELECT ap.product,ap.group_,apa.advertiser ");
			sql.Append ("  FROM "+Constantes.DB.Schema.ADEXPRESS_SCHEMA+".ALL_PRODUCT ap , "+Constantes.DB.Schema.ADEXPRESS_SCHEMA+".ALL_PRODUCT_ADVERTISER  apa ");				
			sql.Append ("  WHERE ap.id_product="+idProduct);  
			sql.Append ("  and apa.id_product = ap.id_product ");
			sql.Append ("  and ap.id_language="+siteLanguage);
			
			#region Execution de la requête
			try{
				return dataSource.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new AlertsInsertionsCreationsDataAccessException ("Impossible de recupérer les informations du produit "+sql,err));
			}
			#endregion
		}
		#endregion
	
		#endregion

		#region Méthodes privées
		
		#region Méthodes pour requêtes avec gestion des colonnes génériques
		/// <summary>
		/// Obetient les champs de la requête
		/// </summary>
		/// <param name="webSession"></param>
		/// <returns></returns>
		private static string GetSqlFields(WebSession webSession){
			string sql="";
			IList tempDetailLevelSqlFields=null, tempInsertionColumnsSqlFields=null;
			
			if(webSession.DetailLevel !=null && webSession.DetailLevel.GetSqlFields().Length>0){
				tempDetailLevelSqlFields = webSession.DetailLevel.GetSqlFields().Split(',');
				sql="  "+webSession.DetailLevel.GetSqlFields();			
			}
			if(webSession.GenericInsertionColumns !=null && webSession.GenericInsertionColumns.GetSqlFields().Length>0){
				
				if(tempDetailLevelSqlFields!=null && tempDetailLevelSqlFields.Count>0){
					tempInsertionColumnsSqlFields = webSession.GenericInsertionColumns.GetSqlFields().Split(',');
					for(int i=0;i<tempInsertionColumnsSqlFields.Count;i++){

						if(!tempDetailLevelSqlFields.Contains(tempInsertionColumnsSqlFields[i].ToString()))
							sql+=" ,"+tempInsertionColumnsSqlFields[i].ToString();
					}
				}
				else sql = webSession.GenericInsertionColumns.GetSqlFields();
			}
			
			return sql;
		}

		/// <summary>
		/// Obetient les tables de la requête
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>Tables</returns>
		private static string GetSqlTables(WebSession webSession){
			string sql="";
			IList tempDetailLevelSqlTables=null, tempInsertionColumnsSqlTables=null;
			
			if(webSession.DetailLevel !=null && webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length>0){
				tempDetailLevelSqlTables = webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
				sql="  "+webSession.DetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);			
			}
			if(webSession.GenericInsertionColumns !=null && webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Length>0){
				
				if(tempDetailLevelSqlTables!=null && tempDetailLevelSqlTables.Count>0){
					tempInsertionColumnsSqlTables = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA).Split(',');
					for(int i=0;i<tempInsertionColumnsSqlTables.Count;i++){

						if(!tempDetailLevelSqlTables.Contains(tempInsertionColumnsSqlTables[i].ToString()))
							sql+=" ,"+tempInsertionColumnsSqlTables[i].ToString();
					}
				}
				else sql = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
			}
			
			return sql;
		}

		
		#endregion

		#region Champs de la requête  
		/// <summary>
		/// Donne les champs à traiter pour le détail des insertions.
		/// </summary>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <param name="prefixeMediaPlanTable">prfixe table média (vehicle)</param>
		/// <returns>Chaine contenant les champs à traiter</returns>
		private static string GetFields(DBClassificationConstantes.Vehicles.names idVehicle,string prefixeMediaPlanTable){
			string sql="";
					
			
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					
					
					sql="  "+prefixeMediaPlanTable+".date_media_Num"
						+", "+prefixeMediaPlanTable+".media_paging"
						+", group_"
						+", advertiser"
						+", product"
						+", format"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()
						+", color"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", location"
						+", "+prefixeMediaPlanTable+".visual "
						+", "+prefixeMediaPlanTable+".id_advertisement"
						+", "+DbTables.APPLICATION_MEDIA_PREFIXE+".disponibility_visual"
						+", "+DbTables.APPLICATION_MEDIA_PREFIXE+".activation"
						+", "+prefixeMediaPlanTable+".id_slogan"
						+", vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media ,interest_center,media_seller,"+prefixeMediaPlanTable+".id_slogan,date_cover_num";
					return sql;

                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:       
					
					sql=" "+prefixeMediaPlanTable+".date_media_num"
						+", "+prefixeMediaPlanTable+".id_top_diffusion"
						+", "+prefixeMediaPlanTable+".associated_file"
						+", advertiser"
						+", product"
						+", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
						+", "+prefixeMediaPlanTable+".rank"
						+", "+prefixeMediaPlanTable+".duration_commercial_break"
						+", "+prefixeMediaPlanTable+".number_spot_com_break"
						+", "+prefixeMediaPlanTable+".rank_wap"
						+", "+prefixeMediaPlanTable+".duration_com_break_wap"
						+", "+prefixeMediaPlanTable+".number_spot_com_break_wap"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", "+prefixeMediaPlanTable+".id_cobranding_advertiser"
						+", "+prefixeMediaPlanTable+".id_slogan"
						+", vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media ,interest_center,media_seller,"+prefixeMediaPlanTable+".id_slogan";
					return sql;

                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
				case DBClassificationConstantes.Vehicles.names.others:										
					sql=" "+prefixeMediaPlanTable+".date_media_num"
						+", "+prefixeMediaPlanTable+".top_diffusion"
						+", "+prefixeMediaPlanTable+".associated_file"
						+", advertiser"
						+", product"
						+", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.duration].Id.ToString()
						+", "+prefixeMediaPlanTable+".id_rank"
						+", "+prefixeMediaPlanTable+".duration_commercial_break"
						+", "+prefixeMediaPlanTable+".number_message_commercial_brea"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
						+", "+prefixeMediaPlanTable+".id_commercial_break"
						+", "+prefixeMediaPlanTable+".id_slogan"
						+", vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media ,interest_center,media_seller,"+prefixeMediaPlanTable+".id_slogan";
					return sql;

				case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:	
					
					sql=" "+prefixeMediaPlanTable+".date_media_num"						
						+", advertiser"
						+", product"
						+", group_"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField + " as " + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].Id.ToString()
						+", "+prefixeMediaPlanTable+".type_board"
						+", "+prefixeMediaPlanTable+".type_sale"
						+", "+prefixeMediaPlanTable+".poster_network"
						+", "+DbTables.AGGLOMERATION_PREFIXE+".agglomeration"
                        + ", " + prefixeMediaPlanTable + "." + UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField + " as " + UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString()
					    +", vehicle,"+prefixeMediaPlanTable+".id_category,category,"+prefixeMediaPlanTable+".id_media,media ,interest_center,media_seller,"+prefixeMediaPlanTable+".id_slogan";
					return sql;
				case DBClassificationConstantes.Vehicles.names.adnettrack:
					sql=" distinct "+prefixeMediaPlanTable+".hashcode,";
					sql+=" "+prefixeMediaPlanTable+".acces_fichier,";
					sql+=" "+prefixeMediaPlanTable+".dimension,";
					sql+=" "+prefixeMediaPlanTable+".id_format,";
					sql+=" "+prefixeMediaPlanTable+".id_format,";
					sql+=" "+prefixeMediaPlanTable+".url,";
					sql+=" advertiser";
					sql+=" product";
					return sql;
				default:
					throw new Exceptions.MediaCreationDataAccessException("GetFields(DBClassificationConstantes.Vehicles.names idVehicle,WebSession webSesssion) :: Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		
		/// <summary>
		///Obtient les tables correspondants au détail media demandée par le client. 
		/// </summary>
		/// <param name="idVehicle">Identifiant du média (vehicle)</param>			
		/// <returns>Chaîne contenant les tables</returns>
		private static string GetTables(DBClassificationConstantes.Vehicles.names idVehicle){
			string sql="";
			string tableName="";
			
			tableName = SQLGenerator.GetVehicleTableNameForDetailResult(idVehicle,TNS.AdExpress.Constantes.Web.Module.Type.analysis, false);

			sql+=GetMediaTables();		
			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GROUP_+ "  "+DbTables.GROUP_PREFIXE;
			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser   "+DbTables.ADVERTISER_PREFIXE;
			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product   "+DbTables.PRODUCT_PREFIXE;		

			sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DbTables.WEB_PLAN_PREFIXE;
			
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
					sql+="," +DBConstantes.Schema.ADEXPRESS_SCHEMA+".agglomeration  "+DbTables.AGGLOMERATION_PREFIXE;
					break;
				case DBClassificationConstantes.Vehicles.names.internationalPress:
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.COLOR+" "+DbTables.COLOR_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.LOCATION+" "+DbTables.LOCATION_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.DATA_LOCATION+" "+DbTables.DATA_LOCATION_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.FORMAT+" "+DbTables.FORMAT_PREFIXE;
					sql+=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" "+DbTables.APPLICATION_MEDIA_PREFIXE;
					break;
			}
						
			return sql;
		}

	
		/// <summary>
		/// Obtient les tables médias correspondants au détail media demandée par le client. 
		/// </summary>
		/// <returns>Chaîne contenant les tables médias</returns>
		public static string GetMediaTables(){
			string sql="";
			
			//Vehicles							
			sql+=DBConstantes.Schema.ADEXPRESS_SCHEMA+".vehicle "+DbTables.VEHICLE_PREFIXE;								
			
			//Categories
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".category "+DbTables.CATEGORY_PREFIXE;					
					

			// Media							
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media "+DbTables.MEDIA_PREFIXE;												

			// Interest center
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".interest_center "+DbTables.INTEREST_CENTER_PREFIXE;					


			// Régie
			sql+=","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media_seller "+DbTables.MEDIA_SELLER_PREFIXE;										

	
			return(sql);
		}

		#endregion

		#region Jointures 
		
		/// <summary>
		/// Obtient les jointures à utiliser lors d'un détail media
		/// </summary>
		/// <param name="sql">requete sql</param>
		/// <param name="idVehicle">Identifiant média (vehicle)</param>
		/// <param name="dataTablePrefixe">prefixe table média</param>
		/// <param name="beginByAnd">Vrai si la condition doit commencée par And</param>
		/// <param name="siteLanguage">Language du site</param>
		/// <returns>requete sql</returns>
		private static void GetJoinConditions(StringBuilder sql,DBClassificationConstantes.Vehicles.names idVehicle,string dataTablePrefixe,bool beginByAnd,string siteLanguage){			

			if(beginByAnd) sql.Append(" and ");
			sql.Append(" "+DbTables.PRODUCT_PREFIXE+".id_product="+dataTablePrefixe+".id_product ");
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".id_advertiser="+dataTablePrefixe+".id_advertiser ");
			sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_group_="+dataTablePrefixe+".id_group_ ");			
			sql.Append(" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle");
			
			//jointures média
			sql.Append(GetMediaJoinConditions(dataTablePrefixe,true,siteLanguage));

            if (idVehicle == DBClassificationConstantes.Vehicles.names.press || idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress || idVehicle == DBClassificationConstantes.Vehicles.names.newspaper || idVehicle == DBClassificationConstantes.Vehicles.names.magazine)
            {
				sql.Append(" and ("+DbTables.APPLICATION_MEDIA_PREFIXE+".id_media(+) = "+dataTablePrefixe+".id_media ");
				sql.Append(" and "+DbTables.APPLICATION_MEDIA_PREFIXE+".date_debut(+) = "+dataTablePrefixe+".date_media_num ");
				sql.Append(" and "+DbTables.APPLICATION_MEDIA_PREFIXE+".id_project(+) = "+ CstProject.ADEXPRESS_ID +") ");
			
				sql.Append(" and "+DbTables.LOCATION_PREFIXE+".id_location (+)="+DbTables.DATA_LOCATION_PREFIXE+".id_location ");
				sql.Append(" and "+DbTables.LOCATION_PREFIXE+".id_language (+)="+siteLanguage);
				
				sql.Append(" and "+DbTables.DATA_LOCATION_PREFIXE+".id_advertisement (+)="+dataTablePrefixe+".id_advertisement ");
				sql.Append(" and "+DbTables.DATA_LOCATION_PREFIXE+".id_media (+)="+dataTablePrefixe+".id_media ");
                sql.Append(" and " + DbTables.DATA_LOCATION_PREFIXE + ".date_media_num (+)=" + dataTablePrefixe + ".date_media_num ");
				

				sql.Append(" and "+DbTables.COLOR_PREFIXE+".id_color (+)="+dataTablePrefixe+".id_color ");
				sql.Append(" and "+DbTables.COLOR_PREFIXE+".id_language (+)="+siteLanguage);
			
				sql.Append(" and "+DbTables.FORMAT_PREFIXE+".id_format (+)="+dataTablePrefixe+".id_format ");
				sql.Append(" and "+DbTables.FORMAT_PREFIXE+".id_language (+)="+siteLanguage);
			
			}

			// agglomeration for outdoor 
            if (idVehicle == DBClassificationConstantes.Vehicles.names.outdoor || idVehicle == DBClassificationConstantes.Vehicles.names.instore) {
				sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".id_agglomeration (+)="+dataTablePrefixe+".id_agglomeration ");				
				sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".id_language(+)="+siteLanguage);
				sql.Append(" and "+DbTables.AGGLOMERATION_PREFIXE+".activation (+)< "+ DBConstantes.ActivationValues.UNACTIVATED);
			}

			//langages		
			sql.Append(" and "+DbTables.GROUP_PREFIXE+".id_language="+siteLanguage);
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".id_language="+siteLanguage);
			sql.Append(" and "+DbTables.PRODUCT_PREFIXE+".id_language="+siteLanguage);						

			//Activation			
			sql.Append(" and "+DbTables.GROUP_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DbTables.ADVERTISER_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and "+DbTables.PRODUCT_PREFIXE+".activation < "+ DBConstantes.ActivationValues.UNACTIVATED);							

		}

		/// <summary>
		/// Obtient les jointures média à utiliser lors d'un détail media
		/// </summary>
		/// <param name="siteLanguage">Language du site</param>
		/// <param name="dataTablePrefixe">Prefixe de la table de résultat</param>
		/// <param name="beginByAnd">Vrai si la condition doit commencée par And</param>
		/// <returns>Chaîne contenant les tables</returns>
		private static string GetMediaJoinConditions(string dataTablePrefixe,bool beginByAnd,string siteLanguage){
			string tmp="";

			//Vehicles			
			if(beginByAnd)tmp+=" and ";
			tmp+="  "+DbTables.VEHICLE_PREFIXE+".id_language="+siteLanguage;
			tmp+=" and "+DbTables.VEHICLE_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.VEHICLE_PREFIXE+".id_vehicle="+dataTablePrefixe+".id_vehicle ";
					
			
			//Categories
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_language="+siteLanguage;
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.CATEGORY_PREFIXE+".id_category="+dataTablePrefixe+".id_category ";
			
			// Media			
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_language="+siteLanguage;
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.MEDIA_PREFIXE+".id_media="+dataTablePrefixe+".id_media ";
				
			
			// Interest center

			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_language="+siteLanguage;
			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.INTEREST_CENTER_PREFIXE+".id_interest_center="+dataTablePrefixe+".id_interest_center ";


			// Régie
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_language="+siteLanguage;
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED;
			tmp+=" and "+DbTables.MEDIA_SELLER_PREFIXE+".id_media_seller="+dataTablePrefixe+".id_media_seller ";

			return(tmp);
		}
		#endregion		

		#region Ordre

		/// <summary>
		/// Donne l'ordre de tri des enregistrements extraits
		/// </summary>
		/// <param name="idVehicle">Identifiant du vehicle</param>
		/// <param name="prefixeMediaPlanTable">prefixe table média</param>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaCreationDataAccessException">
		/// Lancée quand le cas du vehicle spécifié n'est pas traité
		/// </exception>
		/// <returns>Chaine contenant les champs de tris</returns>
		public static string GetInsertionsOrder(DBClassificationConstantes.Vehicles.names idVehicle,string prefixeMediaPlanTable){
			string sql="";
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
				case DBClassificationConstantes.Vehicles.names.internationalPress:																
					sql+=" vehicle ,category, media ,"+prefixeMediaPlanTable+".id_slogan"																
						+", "+prefixeMediaPlanTable+".date_media_Num, "+prefixeMediaPlanTable+".id_advertisement"
						+", "+prefixeMediaPlanTable+".media_paging, location";
					return sql;

                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:    								
					sql+=" vehicle ,category, media ,"+prefixeMediaPlanTable+".id_slogan"	
					+","+TNS.AdExpress.Web.DataAccess.Functions.GetRadioInsertionsOrder(0)+" asc"
					+", wp.id_top_diffusion, wp.id_cobranding_advertiser";				
					return sql;

                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
				case DBClassificationConstantes.Vehicles.names.others:										
					sql+=" vehicle ,category, media ,"+prefixeMediaPlanTable+".id_slogan"
					+","+TNS.AdExpress.Web.DataAccess.Functions.GetTvInsertionsOrder(0)+" asc"					
						+", wp.id_commercial_break, wp.id_rank";										
					return sql;

				case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:	
					sql+=" vehicle ,category, media ,"+prefixeMediaPlanTable+".id_slogan"
                    + "," + TNS.AdExpress.Web.DataAccess.Functions.GetOutDoorInsertionsOrder(0) + " asc, wp." + UnitsInformation.List[WebConstantes.CustomerSessions.Unit.numberBoard].DatabaseField;
					return sql;

				default:
					throw new AlertsInsertionsCreationsDataAccessException(" GetInsertionsOrder : Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		#endregion

		#endregion
	}
}

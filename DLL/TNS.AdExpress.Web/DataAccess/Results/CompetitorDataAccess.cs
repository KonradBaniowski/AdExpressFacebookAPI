#region Information
// Auteur: Guillaume Facon
// Créé le: 13/09/2004
// Modifiée le:
//	13/09/2004	G. Facon
//	22/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
//	24/11/2006	G. Facon		Intégration des niveaux génériques
#endregion

#region Using
using System;
using System.Text;
using System.Data;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Level;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{


	/// <summary>
	/// Extraction des données d'analyse concurrentielles
	/// </summary>
	public class CompetitorDataAccess {

        #region GetData
        /// <summary>
		/// Charge les données pour créer une analyse concurrentielle d'UN vehicle
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">Média à traiter</param>
		/// <param name="beginingDate">Date de début</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Données de l'alerte concurrentielle</returns>
		public static DataSet GetData(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, string beginingDate, string endDate){

			#region Constantes
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Variables
			string productFieldName=string.Empty;
			string orderFieldName=string.Empty;
			string groupByFieldName=string.Empty;
			string productJoinCondition=string.Empty;
			string dataTableName=string.Empty;
			string productTableName=string.Empty;
			string mediaAgencyTable=string.Empty;
			string mediaAgencyJoins=string.Empty;


			string dateField="";
			
			string dataTableNameForGad="";
			string dataFieldsForGad="";
			string dataJointForGad="";
			string unitField="";
			string productsRights="";
			string mediaRights="";
			string listExcludeProduct="";
			string sql="";
			string list="";
			int positionUnivers=1;
			string mediaList="";
			bool premier;
			string dataJointForInsert="";
			#endregion

			#region Construction de la requête
			try{
				// Table de données
				WebConstantes.Module.Type moduleType=(ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
				switch(moduleType){
					case WebConstantes.Module.Type.alert:
						dataTableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName,moduleType);
						break;
					case WebConstantes.Module.Type.analysis:
						dataTableName=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
						break;
					default:
						throw(new CompetitorDataAccessException("Impossible de déterminer le type de module pour trouver la table à utiliser"));
				}
				
				// Obtient les tables de la nomenclature
				productTableName=webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				if(productTableName!=null &&productTableName.Length>0)productTableName=","+productTableName;
				// Obtient les champs de la nomenclature
				productFieldName=webSession.GenericProductDetailLevel.GetSqlFields();
				// Obtient l'ordre des champs
				orderFieldName=webSession.GenericProductDetailLevel.GetSqlOrderFields();
				// obtient la clause group by
				groupByFieldName=webSession.GenericProductDetailLevel.GetSqlGroupByFields();
				// Obtient les jointures pour la nomenclature
				//mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
				productJoinCondition=webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage,DbTables.WEB_PLAN_PREFIXE);
				// Unités
				unitField = WebFunctions.SQLGenerator.GetUnitFieldName(webSession);
				// Droits produit
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				// Droits media
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				// Dates
				dateField=DATA_TABLE_PREFIXE+"."+WebFunctions.SQLGenerator.GetDateFieldName(webSession);
				// Liste des produits à exclure
				listExcludeProduct=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				//option encarts (pour la presse)
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName  || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)){
					try{
						dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+WebFunctions.SQLGenerator.GetTablesForGad(webSession)+" "+DBConstantes.Tables.GAD_PREFIXE;
						dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
						dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
					}
					catch(WebExceptions.SQLGeneratorException){;}
				}
				//Agence_media
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency)||webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)){
					mediaAgencyTable=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
					mediaAgencyJoins="And "+DATA_TABLE_PREFIXE+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
					mediaAgencyJoins+="And "+DATA_TABLE_PREFIXE+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
				}
			}
			catch(Exception e){
				throw(new CompetitorDataAccessException("Impossible d'initialiser les paramètres de la requêtes"+e.Message));
			}



			/*try{
				dataTableName=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
				detailProductTablesNames=WebFunctions.SQLGenerator.getTablesForProductDetail(webSession);
				detailProductFields=WebFunctions.SQLGenerator.getFieldsForProductDetail(webSession,DATA_TABLE_PREFIXE);
				detailProductJoints=WebFunctions.SQLGenerator.getJointAndLanguageForProductDetail(webSession,DATA_TABLE_PREFIXE,vehicleName);
				unitField = WebFunctions.SQLGenerator.getUnitFieldNameForAnalysisResult(webSession.Unit);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				detailProductOrderBy=WebFunctions.SQLGenerator.getOrderByForProductDetail(webSession,DATA_TABLE_PREFIXE);
				dateField=DATA_TABLE_PREFIXE+"."+WebFunctions.SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
				listExcludeProduct=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				//option encarts (pour la presse)
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					dataJointForInsert=WebFunctions.SQLGenerator.getJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
				if(webSession.CustomerLogin.FlagsList.ContainsKey((long)CustormerConstantes.DB.Flag.id.gad.GetHashCode())){
					try{
						dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+WebFunctions.SQLGenerator.getTablesForGad(webSession)+" "+DBConstantes.Tables.GAD_PREFIXE;
						dataFieldsForGad=", "+WebFunctions.SQLGenerator.getFieldsAddressForGad();
						dataJointForGad="and "+WebFunctions.SQLGenerator.getJointForGad(DATA_TABLE_PREFIXE);
					}
					catch(WebExceptions.SQLGeneratorException){;}
				}
			}
			catch(Exception e){
				throw(new CompetitorAnalysisDataAccessException("Impossible d'initialiser les paramètres de la requêtes",e));
			}*/

			sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+productFieldName+dataFieldsForGad+", sum("+unitField+") as unit";
			sql+=" from "+mediaAgencyTable+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE+productTableName+" "+dataTableNameForGad;
			// Période
			sql+=" where "+dateField+" >="+beginingDate;
			sql+=" and "+dateField+" <="+endDate;
			// Jointures Produits
			sql+=" "+productJoinCondition;
			sql+=" "+dataJointForGad;
			sql+=" "+mediaAgencyJoins;
			//Jointures encart
			if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
				sql+=" "+dataJointForInsert;

			#region Sélection de Médias
			while(webSession.CompetitorUniversMedia[positionUnivers]!=null){
				mediaList+=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[positionUnivers],CustormerConstantes.Right.type.mediaAccess)+",";
				positionUnivers++;
			}
			if (mediaList.Length>0)sql+=" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList.Substring(0,mediaList.Length-1)+")";
			#endregion
		
			#region Ancienne Sélection de Produits
			//// Sélection en accès
			//premier=true;
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
			//if(list.Length>0){
			//    sql+=" and ((wp.id_sector in ("+list+") ";
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorAccess);
			//if(list.Length>0){
			//    if(!premier) sql+=" or";
			//    else sql+=" and ((";
			//    sql+=" wp.id_subsector in ("+list+") ";
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
			//if(list.Length>0){
			//    if(!premier) sql+=" or";
			//    else sql+=" and ((";
			//    sql+=" wp.id_group_ in ("+list+") ";
			//    premier=false;
			//}

			//if(!premier) sql+=" )";
			
			//// Sélection en Exception
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorException);
			//if(list.Length>0){
			//    if(premier) sql+=" and (";
			//    else sql+=" and";
			//    sql+=" wp.id_sector not in ("+list+") ";
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorException);
			//if(list.Length>0){
			//    if(premier) sql+=" and (";
			//    else sql+=" and";
			//    sql+=" wp.id_subsector not in ("+list+") ";
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupException);
			//if(list.Length>0){
			//    if(premier) sql+=" and (";
			//    else sql+=" and";
			//    sql+=" wp.id_group_ not in ("+list+") ";
			//    premier=false;
			//}
			//if(!premier) sql+=" )";
			#endregion

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DATA_TABLE_PREFIXE, true);

			// Produit exclus
			sql+=listExcludeProduct;

			// Droits des Médias
			// Droits des Produits
			sql+=" "+productsRights;
			sql+=" "+mediaRights;
			// Group by
			sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+groupByFieldName+dataFieldsForGad;
			// Order by
			sql+=" order by "+orderFieldName+","+DATA_TABLE_PREFIXE+".id_media ";
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new CompetitorDataAccessException ("Impossible de charger les données de l'alerte concurrentielle"+sql,err));
			}

			#endregion

        }
        #endregion

        #region Synthèse
        /// <summary>
		/// Charge les données de synthèse pour créer une analyse concurrentielle d'UN vehicle
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">Média à traiter</param>
		/// <param name="beginingDate">Date de début</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Données de l'alerte concurrentielle</returns>
		public static DataTable GetSynthesisData(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, string beginingDate, string endDate){
			
			#region Constantes
			//préfixe table à utiliser
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Variables
			string dataTableName="";
			//			string dataTableNameForGad="";
			//			string dataFieldsForGad="";
			//			string dataJointForGad="";
			//			string detailProductTablesNames="";
			//			string detailProductFields="";
			//			string detailProductJoints="";
//			string detailProductOrderBy="";
			string listExcludeProduct="";
			string unitField="";
			string productsRights="";
			string mediaRights="";			
			string list="";
			int positionUnivers=1;
			string mediaList="";			
			bool premier;
			string dataJointForInsert="";
			StringBuilder sql = new StringBuilder(3000);
				string dateField="";
			#endregion

			#region Construction de la requête
			try{
				// Table de données
				WebConstantes.Module.Type moduleType=(ModulesList.GetModule(webSession.CurrentModule)).ModuleType;
				switch(moduleType){
					case WebConstantes.Module.Type.alert:
						dataTableName=WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName,moduleType);
						break;
					case WebConstantes.Module.Type.analysis:
						dataTableName=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
						break;
					default:
						throw(new CompetitorDataAccessException("Impossible de déterminer le type de module pour trouver la table à utiliser"));
				}
				// Unités
				unitField = WebFunctions.SQLGenerator.GetUnitFieldName(webSession);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				listExcludeProduct=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				// Dates
				dateField=DATA_TABLE_PREFIXE+"."+WebFunctions.SQLGenerator.GetDateFieldName(webSession);
				//option encarts (pour la presse)
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					dataJointForInsert=WebFunctions.SQLGenerator.GetJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);				
			}
			catch(Exception e){
				throw(new CompetitorDataAccessException("Impossible d'initialiser les paramètres de la requêtes",e));
			}
			//Select
			sql.Append("  select "+DATA_TABLE_PREFIXE+".id_sector,"+DATA_TABLE_PREFIXE+".id_subsector, "+DATA_TABLE_PREFIXE+".id_group_");	
			sql.Append(", "+DATA_TABLE_PREFIXE+".id_advertiser,"+DATA_TABLE_PREFIXE+".id_brand, "+DATA_TABLE_PREFIXE+".id_product");
			sql.Append(","+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY,"+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY");
			sql.Append(","+DATA_TABLE_PREFIXE+".id_media, sum("+unitField+") as unit");
			//From
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE);
			sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);
			//Where
			// Période
			sql.Append(" where "+dateField+" >="+beginingDate);
			sql.Append(" and "+dateField+" <="+endDate);
				
			//Jointures encart
			if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
				sql.Append(" "+dataJointForInsert);

			//Jointures groupe agences/agences		
			sql.Append(" and "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product(+)="+DATA_TABLE_PREFIXE+".id_product ");
			sql.Append(" and "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_language(+)="+DBConstantes.Language.FRENCH);
			// Vehicle
			sql.Append(" and "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle(+)="+vehicleName.GetHashCode());	

			#region Sélection de Médias
			while(webSession.CompetitorUniversMedia[positionUnivers]!=null){
				mediaList+=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[positionUnivers],CustormerConstantes.Right.type.mediaAccess)+",";
				positionUnivers++;
			}
			if (mediaList.Length>0)sql.Append(" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList.Substring(0,mediaList.Length-1)+") ");
			#endregion
		
			#region Ancienne Sélection de Produits
			//// Sélection en accès
			//premier=true;
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorAccess);
			//if(list.Length>0){
			//    sql.Append(" and ((wp.id_sector in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_subsector in ("+list+") ");
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupAccess);
			//if(list.Length>0){
			//    if(!premier) sql.Append(" or");
			//    else sql.Append(" and ((");
			//    sql.Append(" wp.id_group_ in ("+list+") ");
			//    premier=false;
			//}

			//if(!premier) sql.Append(" )");
			
			//// Sélection en Exception
			//// Sector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.sectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_sector not in ("+list+") ");
			//    premier=false;
			//}
			//// SubSector
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.subSectorException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_subsector not in ("+list+") ");
			//    premier=false;
			//}
			//// Group
			//list=webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.groupException);
			//if(list.Length>0){
			//    if(premier) sql.Append(" and (");
			//    else sql.Append(" and");
			//    sql.Append(" wp.id_group_ not in ("+list+") ");
			//    premier=false;
			//}
			//if(!premier)sql.Append(" )");
			#endregion

			// Sélection de Produits
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DATA_TABLE_PREFIXE, true));

			// Produit exclus
			sql.Append(listExcludeProduct);			

			// Droits des Médias
			sql.Append(mediaRights);	
			// Droits des Produits
			sql.Append(" "+productsRights);
			// Group by			
			sql.Append("  group by "+DATA_TABLE_PREFIXE+".id_sector,"+DATA_TABLE_PREFIXE+".id_subsector, "+DATA_TABLE_PREFIXE+".id_group_");	
			sql.Append(", "+DATA_TABLE_PREFIXE+".id_advertiser,"+DATA_TABLE_PREFIXE+".id_brand, "+DATA_TABLE_PREFIXE+".id_product");
			sql.Append(", "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY,"+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY");
			sql.Append(", "+DATA_TABLE_PREFIXE+".id_media");
			// Order by
			sql.Append("  order by "+DATA_TABLE_PREFIXE+".id_sector,"+DATA_TABLE_PREFIXE+".id_subsector, "+DATA_TABLE_PREFIXE+".id_group_");	
			sql.Append(", "+DATA_TABLE_PREFIXE+".id_advertiser,"+DATA_TABLE_PREFIXE+".id_brand, "+DATA_TABLE_PREFIXE+".id_product");
			sql.Append(","+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_GROUP_ADVERTISING_AGENCY,"+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".ID_ADVERTISING_AGENCY");
			sql.Append(","+DATA_TABLE_PREFIXE+".id_media");
			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString()).Tables[0];
			}
			catch(System.Exception err){
				throw(new CompetitorDataAccessException("Impossible de charger les données de l'alerte concurrentielle: "+sql,err));
			}
			#endregion
		}
		#endregion

        #region GetSynthesisRequest
        /// <summary>
        /// Construction de la requête pour le charge les données de synthèse pour créer une analyse concurrentielle d'UN vehicle
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehicleName">Média à traiter</param>
        /// <param name="type">Type de la table à utiliser</param>
        /// <returns>Code SQL</returns>
        public static string GetSynthesisRequest(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName, DBConstantes.TableType.Type type) {

            #region Constantes
            //préfixe table à utiliser
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            string dataTableName = "";
            string listExcludeProduct = "";
            string unitField = "";
            string productsRights = "";
            string mediaRights = "";
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";
            StringBuilder sql = new StringBuilder(3000);
            string dateField = "";
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            #endregion

            #region Construction de la requête
            try {
                // Table de données
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        dataTableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.alert);
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        dataTableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.analysis);
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dataTableName = DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                        break;
                    default:
                        throw (new CompetitorDataAccessException("Impossible de déterminer le type de la table pour trouver la table à utiliser"));
                }
                // Unités
                unitField = WebFunctions.SQLGenerator.GetUnitFieldName(webSession, type);
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_TABLE_PREFIXE, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
                listExcludeProduct = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                // Dates
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                    case DBConstantes.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
                //option encarts (pour la presse)
                if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName)
                    dataJointForInsert = WebFunctions.SQLGenerator.getJointForInsertDetail(webSession, DATA_TABLE_PREFIXE, type);
            }
            catch (Exception e) {
                throw (new CompetitorDataAccessException("Impossible d'initialiser les paramètres de la requêtes", e));
            }
            //Select
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.Append("  select " + DATA_TABLE_PREFIXE + ".id_sector," + DATA_TABLE_PREFIXE + ".id_subsector, " + DATA_TABLE_PREFIXE + ".id_group_");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_advertiser," + DATA_TABLE_PREFIXE + ".id_brand, " + DATA_TABLE_PREFIXE + ".id_product");
                sql.Append("," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY");
                sql.Append("," + DATA_TABLE_PREFIXE + ".id_media, " + dateField + " as date_num, sum(" + unitField + ") as unit");
            }
            else {
                sql.Append("  select " + DATA_TABLE_PREFIXE + ".id_sector," + DATA_TABLE_PREFIXE + ".id_subsector, " + DATA_TABLE_PREFIXE + ".id_group_");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_advertiser," + DATA_TABLE_PREFIXE + ".id_brand, " + DATA_TABLE_PREFIXE + ".id_product");
                sql.Append("," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY");
                sql.Append("," + DATA_TABLE_PREFIXE + ".id_media, sum(" + unitField + ") as unit");
            }
            //From
            sql.Append(" from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + dataTableName + " " + DATA_TABLE_PREFIXE);
            sql.Append("," + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE);
            //Where

            // Période
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql.Append(" where " + dateField + " >=" + customerPeriod.StartDate);
                    sql.Append(" and " + dateField + " <=" + customerPeriod.EndDate);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                        sql.Append(" where " + dateField + " >=" + customerPeriod.StartDate);
                        sql.Append(" and " + dateField + " <=" + customerPeriod.EndDate);
                    }
                    else if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql.Append(" where ((" + dateField + " >=" + customerPeriod.PeriodDayBegin[0]);
                        sql.Append(" and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0]);
                        sql.Append(" ) or (" + dateField + " >=" + customerPeriod.PeriodDayBegin[1]);
                        sql.Append(" and " + dateField + " <=" + customerPeriod.PeriodDayEnd[1]);
                        sql.Append("))");
                    }
                    else {
                        sql.Append(" where " + dateField + " >=" + customerPeriod.PeriodDayBegin[0]);
                        sql.Append(" and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0]);
                    }
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql.Append(" where " + dateField + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6));
                    sql.Append(" and " + dateField + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6));
                    break;
            }

            //Jointures encart
            if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName)
                sql.Append(" " + dataJointForInsert);

            //Jointures groupe agences/agences		
            sql.Append(" and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product(+)=" + DATA_TABLE_PREFIXE + ".id_product ");
            sql.Append(" and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_language(+)=" + DBConstantes.Language.FRENCH);
            // Vehicle
            sql.Append(" and " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle(+)=" + vehicleName.GetHashCode());

            #region Sélection de Médias
            while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }
            if (mediaList.Length > 0) sql.Append(" and " + DATA_TABLE_PREFIXE + ".id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ") ");
            #endregion

            #region Sélection de Produits
            // Sélection en accès
            premier = true;
            // Sector
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.sectorAccess);
            if (list.Length > 0) {
                sql.Append(" and ((wp.id_sector in (" + list + ") ");
                premier = false;
            }
            // SubSector
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.subSectorAccess);
            if (list.Length > 0) {
                if (!premier) sql.Append(" or");
                else sql.Append(" and ((");
                sql.Append(" wp.id_subsector in (" + list + ") ");
                premier = false;
            }
            // Group
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.groupAccess);
            if (list.Length > 0) {
                if (!premier) sql.Append(" or");
                else sql.Append(" and ((");
                sql.Append(" wp.id_group_ in (" + list + ") ");
                premier = false;
            }

            if (!premier) sql.Append(" )");

            // Sélection en Exception
            // Sector
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.sectorException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.Append(" wp.id_sector not in (" + list + ") ");
                premier = false;
            }
            // SubSector
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.subSectorException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.Append(" wp.id_subsector not in (" + list + ") ");
                premier = false;
            }
            // Group
            list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.groupException);
            if (list.Length > 0) {
                if (premier) sql.Append(" and (");
                else sql.Append(" and");
                sql.Append(" wp.id_group_ not in (" + list + ") ");
                premier = false;
            }
            if (!premier) sql.Append(" )");
            #endregion

            // Produit exclus
            sql.Append(listExcludeProduct);

            // Droits des Médias
            sql.Append(mediaRights);
            // Droits des Produits
            sql.Append(" " + productsRights);
            // Group by		
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql.Append("  group by " + DATA_TABLE_PREFIXE + ".id_sector," + DATA_TABLE_PREFIXE + ".id_subsector, " + DATA_TABLE_PREFIXE + ".id_group_");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_advertiser," + DATA_TABLE_PREFIXE + ".id_brand, " + DATA_TABLE_PREFIXE + ".id_product");
                sql.Append(", " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_media, " + dateField);
            }
            else {
                sql.Append("  group by " + DATA_TABLE_PREFIXE + ".id_sector," + DATA_TABLE_PREFIXE + ".id_subsector, " + DATA_TABLE_PREFIXE + ".id_group_");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_advertiser," + DATA_TABLE_PREFIXE + ".id_brand, " + DATA_TABLE_PREFIXE + ".id_product");
                sql.Append(", " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_GROUP_ADVERTISING_AGENCY," + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".ID_ADVERTISING_AGENCY");
                sql.Append(", " + DATA_TABLE_PREFIXE + ".id_media ");
            }
            #endregion

            #region Execution de la requête
            try {
                return sql.ToString();
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de construire la requête pour le chargement des données de l'alerte concurrentielle: " + sql, err));
            }
            #endregion
        }
        #endregion

        #region GetGenericSynthesisData
        /// <summary>
		/// Charge les données de synthèse pour créer une analyse concurrentielle d'UN vehicle
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">Média à traiter</param>
		/// <returns>Données de l'alerte concurrentielle</returns>
        public static DataTable GetGenericSynthesisData(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName) {

            #region Constantes
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            #endregion

            #region Construction de la requête
            try {

                if (customerPeriod.Is4M) {
                    sql4M = GetSynthesisRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle4M);
                    sql4M += "  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product";
                    sql4M += ", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media";
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql = GetSynthesisRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle);
                    sql += "  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product";
                    sql += ", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media";
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle = GetSynthesisRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle);
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan = GetSynthesisRequest(webSession, vehicleName, DBConstantes.TableType.Type.webPlan);
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sql = "";

                        sql += "  select id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product";
                        sql += ", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media, sum(unit) as unit";
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";
                        sql += "  group by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product";
                        sql += ", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media";

                    }

                    sql += "  order by id_sector, id_subsector, id_group_, id_advertiser, id_brand, id_product";
                    sql += ", ID_GROUP_ADVERTISING_AGENCY, ID_ADVERTISING_AGENCY, id_media";
                }
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de construire la requête pour la synthèse du module concurrentiel" + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return webSession.Source.Fill(sql.ToString()).Tables[0];
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger les données pour la synthèse du module concurrentiel" + sql, err));
            }

            #endregion
        
        }
        #endregion

        #region GetRequest
        /// <summary>
        /// Construit la requête pour charge les données pour créer une analyse concurrentielle d'UN vehicle
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehicleName">Média à traiter</param>
        /// <param name="type">Type de la table à utiliser</param>
        /// <returns>Requête de l'alerte concurrentielle</returns>
        public static string GetRequest(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName, DBConstantes.TableType.Type type) {

            #region Constantes
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            string productFieldName = string.Empty;
            string orderFieldName = string.Empty;
            string groupByFieldName = string.Empty;
            string productJoinCondition = string.Empty;
            string dataTableName = string.Empty;
            string productTableName = string.Empty;
            string mediaAgencyTable = string.Empty;
            string mediaAgencyJoins = string.Empty;

            string dateField = "";

            string dataTableNameForGad = "";
            string dataFieldsForGad = "";
            string dataJointForGad = "";
            string unitField = "";
            string productsRights = "";
            string mediaRights = "";
            string listExcludeProduct = "";
            string sql = "";
            string list = "";
            int positionUnivers = 1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";

            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requête
            try {
                // Table de données
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        dataTableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.alert);
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        dataTableName = WebFunctions.SQLGenerator.GetVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.analysis);
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dataTableName = DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                        break;
                    default:
                        throw (new CompetitorDataAccessException("Impossible de déterminer le type de la table pour trouver la table à utiliser"));
                }

                // Obtient les tables de la nomenclature
                productTableName = webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
                if (productTableName != null && productTableName.Length > 0) productTableName = "," + productTableName;
                // Obtient les champs de la nomenclature
                productFieldName = webSession.GenericProductDetailLevel.GetSqlFields();
                // obtient la clause group by
                groupByFieldName = webSession.GenericProductDetailLevel.GetSqlGroupByFields();
                // Obtient les jointures pour la nomenclature
                //mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
                productJoinCondition = webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage, DbTables.WEB_PLAN_PREFIXE);
                // Unités
                unitField = WebFunctions.SQLGenerator.GetUnitFieldName(webSession, type);
                // Droits produit
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_TABLE_PREFIXE, true);
                // Droits media
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
                // Dates
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                    case DBConstantes.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
                // Liste des produits à exclure
                listExcludeProduct = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                //option encarts (pour la presse)
                if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName)
                    dataJointForInsert = WebFunctions.SQLGenerator.getJointForInsertDetail(webSession, DATA_TABLE_PREFIXE, type);
                if (webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser)) {
                    try {
                        dataTableNameForGad = ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + WebFunctions.SQLGenerator.GetTablesForGad(webSession) + " " + DBConstantes.Tables.GAD_PREFIXE;
                        dataFieldsForGad = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad();
                        dataJointForGad = "and " + WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
                    }
                    catch (WebExceptions.SQLGeneratorException) { ;}
                }
                //Agence_media
                if (webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.groupMediaAgency) || webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.agency)) {
                    mediaAgencyTable = DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + webSession.MediaAgencyFileYear + " " + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ",";
                    mediaAgencyJoins = "And " + DATA_TABLE_PREFIXE + ".id_product=" + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_product ";
                    mediaAgencyJoins += "And " + DATA_TABLE_PREFIXE + ".id_vehicle=" + DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE + ".id_vehicle ";
                }
            }
            catch (Exception e) {
                throw (new CompetitorDataAccessException("Impossible d'initialiser les paramètres de la requêtes" + e.Message));
            }

            try {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql += " select " + DATA_TABLE_PREFIXE + ".id_media, " + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + " as columnDetailLevel, " + productFieldName + dataFieldsForGad + ", " + dateField + " as date_num, sum(" + unitField + ") as unit";
                else
                    sql += " select " + DATA_TABLE_PREFIXE + ".id_media, " + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + " as columnDetailLevel, " + productFieldName + dataFieldsForGad + ", sum(" + unitField + ") as unit";
                sql += " from " + mediaAgencyTable + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + dataTableName + " " + DATA_TABLE_PREFIXE + productTableName + " " + dataTableNameForGad;
                // Période
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        sql += " where " + dateField + " >=" + customerPeriod.StartDate;
                        sql += " and " + dateField + " <=" + customerPeriod.EndDate;
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                            sql += " where " + dateField + " >=" + customerPeriod.StartDate;
                            sql += " and " + dateField + " <=" + customerPeriod.EndDate;
                        }
                        else if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                            sql += " where ((" + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
                            sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
                            sql += " ) or (" + dateField + " >=" + customerPeriod.PeriodDayBegin[1];
                            sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[1];
                            sql += "))";
                        }
                        else {
                            sql += " where " + dateField + " >=" + customerPeriod.PeriodDayBegin[0];
                            sql += " and " + dateField + " <=" + customerPeriod.PeriodDayEnd[0];
                        }
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        sql += " where " + dateField + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
                        sql += " and " + dateField + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
                        break;
                }
                // Jointures Produits
                sql += " " + productJoinCondition;
                sql += " " + dataJointForGad;
                sql += " " + mediaAgencyJoins;
                //Jointures encart
                if (DBClassificationConstantes.Vehicles.names.press == vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress == vehicleName)
                    sql += " " + dataJointForInsert;

                #region Sélection de Médias
                while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                    mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                    positionUnivers++;
                }
                if (mediaList.Length > 0) sql += " and " + DATA_TABLE_PREFIXE + ".id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")";
                #endregion

                #region Ancienne Sélection de Produits
				//// Sélection en accès
				//premier = true;
				//// Sector
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.sectorAccess);
				//if (list.Length > 0) {
				//    sql += " and ((wp.id_sector in (" + list + ") ";
				//    premier = false;
				//}
				//// SubSector
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.subSectorAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_subsector in (" + list + ") ";
				//    premier = false;
				//}
				//// Group
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.groupAccess);
				//if (list.Length > 0) {
				//    if (!premier) sql += " or";
				//    else sql += " and ((";
				//    sql += " wp.id_group_ in (" + list + ") ";
				//    premier = false;
				//}

				//if (!premier) sql += " )";

				//// Sélection en Exception
				//// Sector
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.sectorException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_sector not in (" + list + ") ";
				//    premier = false;
				//}
				//// SubSector
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.subSectorException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_subsector not in (" + list + ") ";
				//    premier = false;
				//}
				//// Group
				//list = webSession.GetSelection(webSession.CurrentUniversProduct, CustomerRightConstante.type.groupException);
				//if (list.Length > 0) {
				//    if (premier) sql += " and (";
				//    else sql += " and";
				//    sql += " wp.id_group_ not in (" + list + ") ";
				//    premier = false;
				//}
				//if (!premier) sql += " )";
                #endregion

				// Sélection de Produits
				if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
					sql += webSession.PrincipalProductUniverses[0].GetSqlConditions(DATA_TABLE_PREFIXE, true);

                // Produit exclus
                sql += listExcludeProduct;

                // Droits des Médias
                // Droits des Produits
                sql += " " + productsRights;
                sql += " " + mediaRights;
                // Group by
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql += " group by " + DATA_TABLE_PREFIXE + ".id_media, " + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + ", " + groupByFieldName + dataFieldsForGad + ", " + dateField;
                else
                    sql += " group by " + DATA_TABLE_PREFIXE + ".id_media, " + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + ", " + groupByFieldName + dataFieldsForGad;

                return (sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de contruire la requête pour la chargement des données pour le module concurrentiel" + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetGenericData
        /// <summary>
        /// Charge les données pour créer une analyse concurrentielle d'UN vehicle
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="vehicleName">Média à traiter</param>
        /// <returns>Données de l'alerte concurrentielle</returns>
        public static DataSet GetGenericData(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName) {

            #region Constantes
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requête
            try {
                orderFieldName = webSession.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = webSession.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = webSession.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

                if (customerPeriod.Is4M) {
                    sql4M = GetRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle4M);
                    sql4M += " order by " + orderFieldName + "," + DATA_TABLE_PREFIXE + ".id_media ";
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql = GetRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle);
                    sql += " order by " + orderFieldName + "," + DATA_TABLE_PREFIXE + ".id_media ";
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle = GetRequest(webSession, vehicleName, DBConstantes.TableType.Type.dataVehicle);
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan = GetRequest(webSession, vehicleName, DBConstantes.TableType.Type.webPlan);
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        productFieldNameWithoutTablePrefix = webSession.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                        if (webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                            dataFieldsForGadWithoutTablePrefix = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad("");
                        sql = "";
                        sql += " select id_media, columnDetailLevel, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + ", sum(unit) as unit";
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";
                        sql += " group by id_media, columnDetailLevel, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
                    }

                    sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";
                }
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de construire la requête pour le module concurrentiel" + sql, err));
            }
            #endregion
                
            #region Execution de la requête
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger les données pour le module concurrentiel" + sql, err));
            }

            #endregion

        }
        #endregion

		#region GetNbParutionData
		/// <summary>
		/// Obtient le nombre de parution par média
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="vehicleName">Nom du media</param>
		/// <returns>Nombre de parution par média</returns>
		public static DataSet GetNbParutionData(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName) { //, string beginingDate, string endDate, string beginingDateN1, string endDateN1) {
			string sql = "";

			CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;


			#region Construction de la requete

			try {

				sql += "  select id_media, columnDetailLevel, count(distinct date_num ) as NbParution ";
				sql += "  from ( ";

				//Get sub query 
				sql += GetNbParutionRequest(webSession, vehicleName, customerPeriod.StartDate, customerPeriod.EndDate);

				sql += "  )  group by id_media, columnDetailLevel ";
				sql += "  order by columnDetailLevel, id_media";

			}
			catch (System.Exception err) {
				throw (new CompetitorDataAccessException("Impossible de construire la requête pour le nombre de parutions par supports " + sql, err));
			}
			#endregion

			#region Execution de la requête
			try {
				return webSession.Source.Fill(sql.ToString());
			}
			catch (System.Exception err) {
				throw (new CompetitorDataAccessException("Impossible de charger les données du module présenets absents", err));
			}
			#endregion
		}
		#endregion

		#region GetNbParutionRequest
		/// <summary>
		/// Get sql query of parution number by media
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="vehicleName">vehicle Name</param>
		/// <param name="beginingDate">begining Date</param>
		/// <param name="endDate">end Date</param>
		/// <returns>sql query</returns>
		private static string GetNbParutionRequest(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName, string beginingDate, string endDate) {

			#region Constantes
			const string DATA_TABLE_PREFIXE = "wp";
			#endregion

			#region Variables
			string sql = "";
			string orderFieldName = string.Empty;
			string groupByFieldName = string.Empty;			
			int positionUnivers = 1;
			string mediaList = "";
			
			#endregion

			#region Construction de la requête
			try {
				DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];

				sql += " select " + DATA_TABLE_PREFIXE + ".id_media, " + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + " as columnDetailLevel, " + DATA_TABLE_PREFIXE + ".date_media_num as date_num";
				sql += " from  " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".data_press " + DATA_TABLE_PREFIXE;
				sql += " where " + DATA_TABLE_PREFIXE + ".date_media_num >=" + beginingDate + " and " + DATA_TABLE_PREFIXE + ".date_media_num <=" + endDate;

				#region Sélection de Médias
				while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
					mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
					positionUnivers++;
				}
				if (mediaList.Length > 0) sql += " and " + DATA_TABLE_PREFIXE + ".id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")";
				#endregion

			
				// Group by
				sql += " group by " + DATA_TABLE_PREFIXE + ".id_media," + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + ", date_media_num ";

			}
			catch (System.Exception err) {
				throw (new DynamicDataAccessException("Impossible de construire la sous requête pour le nombre de parutions par supports " + sql, err));
			}
			#endregion

			return sql;
		}
		#endregion

        #region GetMediaColumnDetailLevelList
        /// <summary>
        /// Charge les données pour recuperer une liste des supports pour le niveau de détail colonne
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Liste des supports</returns>
        public static DataSet GetMediaColumnDetailLevelList(WebSession webSession) {

            #region Variables
            string sql = "";
            string mediaList = "";
            string prefixe = "";
            int positionUnivers = 1;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requête
            sql += " select id_media, ";
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) prefixe = DBConstantes.Tables.CATEGORY_PREFIXE + ".";
            sql += prefixe + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + " as columnDetailLevel ";
            sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DBConstantes.Tables.MEDIA_PREFIXE;
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".basic_media " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + DBConstantes.Tables.CATEGORY_PREFIXE;
            }

            #region Sélection de Médias
            while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }

            sql += " where id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")";
            sql += " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
            #endregion

            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_basic_media = " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_basic_media";
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_category = " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category";
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
                sql += " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
            }

            #endregion

            #region Execution de la requête
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger la liste des supports pour le niveau de détail colonne" + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetColumnDetailLevelList
        /// <summary>
        /// Charge les données pour recuperer une liste des supports pour le niveau de détail colonne
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Liste des supports</returns>
        public static DataSet GetColumnDetailLevelList(WebSession webSession) {

            #region Variables
            string sql = "";
            string mediaList = "";
            string prefixe = "";
            int positionUnivers = 1;
            DetailLevelItemInformation columnDetailLevel = (DetailLevelItemInformation)webSession.GenericColumnDetailLevel.Levels[0];
            #endregion

            #region Construction de la requête
            sql += "select distinct ";
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) prefixe = DBConstantes.Tables.CATEGORY_PREFIXE + ".";
            sql += prefixe + columnDetailLevel.GetSqlFieldIdWithoutTablePrefix() + " as columnDetailLevel ";
            sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".media " + DBConstantes.Tables.MEDIA_PREFIXE;
            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".basic_media " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE;
                sql += ", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".category " + DBConstantes.Tables.CATEGORY_PREFIXE;
            }

            #region Sélection de Médias
            while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                positionUnivers++;
            }

            sql += " where id_media in (" + mediaList.Substring(0, mediaList.Length - 1) + ")";
            sql += " and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
            #endregion

            if (columnDetailLevel.Id == DetailLevelItemInformation.Levels.category) {
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_basic_media = " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_basic_media";
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_category = " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category";
                sql += " and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
                sql += " and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language = " + TNS.AdExpress.Constantes.DB.Language.FRENCH;
            }

            #endregion

            #region Execution de la requête
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger la liste des supports pour le niveau de détail colonne" + sql, err));
            }
            #endregion

        }
        #endregion

    }
}

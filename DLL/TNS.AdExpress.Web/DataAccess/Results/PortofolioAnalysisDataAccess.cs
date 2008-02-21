#region Informations
// Auteur: A. Obermeyer & D. V. Mussuma
// Date de cr�ation: 28/12/2004
// Date de modification: 28/12/2004
//	23/08/2005	G. Facon		Solution temporaire pour les IDataSource
//	10/11/2005	D. V. Mussuma	Utilisation de IDataSource depuis WebSession
//	25/11/2005	B.Masson		webSession.Source
#endregion

#region Using
using System;
using System.Data;
using System.Text;
using System.Collections;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using WebFunctions=TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CstProject = TNS.AdExpress.Constantes.Project;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{

	/// <summary>
	/// Classe d'acc�s aux donn�es pour le portefeuille d'un support
	/// </summary>
	public class PortofolioAnalysisDataAccess{

		#region D�tail du Portefeuille d'un m�dia
		/// <summary>
		/// Charge les donn�es pour cr�er le d�tail du portefeuille d'un Media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">M�dia � traiter</param>
		/// <param name="beginingDate">Date de d�but</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Donn�es du portefeuille d'un Media</returns>
		public static DataSet GetData(WebSession webSession,DBClassificationConstantes.Vehicles.names vehicleName, string beginingDate, string endDate){
			
			#region Constantes
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Variables
			string dateField="";
			string dataTableName="";
			string dataTableNameForGad="";
			string dataFieldsForGad="";
			string dataJointForGad="";
			string dataJointForInsert="";
			string detailProductTablesNames="";
			string detailProductFields="";
			string detailProductJoints="";
			string detailProductOrderBy="";
			string unitFields="";
			string productsRights="";
			string sql="";
			string list="";
			// int positionUnivers=1;
			string mediaList="";
			bool premier;
			string mediaRights="";
			string listProductHap="";
			//DataSet ds=null;
			string mediaAgencyTable=string.Empty;
			string mediaAgencyJoins=string.Empty;
			#endregion

			#region Construction de la requ�te
			try{
				dataTableName=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod);
				detailProductTablesNames=webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				detailProductFields=webSession.GenericProductDetailLevel.GetSqlFields();
				detailProductJoints=webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage,DATA_TABLE_PREFIXE);
				unitFields = WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(vehicleName,DATA_TABLE_PREFIXE);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				detailProductOrderBy=webSession.GenericProductDetailLevel.GetSqlOrderFields();
				dateField=DATA_TABLE_PREFIXE+"."+WebFunctions.SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				listProductHap=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				//option encarts (pour la presse)
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					dataJointForInsert=WebFunctions.SQLGenerator.getJointForInsertDetail(webSession,DATA_TABLE_PREFIXE);
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.advertiser)){
					try{
						dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
						dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
						dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
					}
					catch(WebExceptions.SQLGeneratorException){;}
				}
				//Agence_media
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.groupMediaAgency)||webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.agency)){
					mediaAgencyTable=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
					mediaAgencyJoins="And "+DATA_TABLE_PREFIXE+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
					mediaAgencyJoins+="And "+DATA_TABLE_PREFIXE+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
				}

			}
			catch(System.Exception err){
				throw(new PortofolioAnalysisDataAccessException("Impossible d'initialiser les param�tres de la requ�tes",err));
			}
			if(WebFunctions.CheckedText.IsStringEmpty(dataTableName.ToString().Trim())){
				sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+","+unitFields;
				sql+=" from "+mediaAgencyTable+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE;
				if (detailProductTablesNames.Length > 0)
					sql+=", "+detailProductTablesNames;
				sql+=" "+dataTableNameForGad;
				// P�riode
				sql+=" where "+dateField+" >="+beginingDate;
				sql+=" and "+dateField+" <="+endDate;
				// Jointures Produits
				sql+=" "+detailProductJoints;
				sql+=" "+dataJointForGad;
				sql+=" "+mediaAgencyJoins;
				//Jointures encart
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					sql+=" "+dataJointForInsert;

				#region S�lection de M�dias
				mediaList+=webSession.GetSelection((TreeNode) webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);						
				if (mediaList.Length>0)sql+=" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList+")";
				#endregion

                #region S�lection de Produits
                sql += GetProductSelection(webSession, DATA_TABLE_PREFIXE);
                #endregion

                #region S�lection de Produits (Old)
                //// S�lection en acc�s
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
			
                //// S�lection en Exception
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
				
				// Droits des Produits
				sql+=" "+productsRights;
				sql+=listProductHap;
				// Droits des M�dias
				sql+=mediaRights;				
				
				// Group by
				sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad;
				// Order by
				sql+=" order by "+detailProductOrderBy+","+DATA_TABLE_PREFIXE+".id_media";
			}
			#endregion
            
			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaAgencyException ("Impossible de charger la liste des ann�es disponible pour les agences media: "+sql,err));
			}
			#endregion

		}
		#endregion

		#region Synth�se
		/// <summary>
		/// Recup�re le total investissement, le nombre de pub, la dur�e des spot
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns></returns>
		public static DataSet GetDataForAnalysisPortofolio(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
		
			#region Construction de la requ�te
			
			#region Constantes
			//pr�fixe table � utiliser
			const string DATA_TABLE_PREFIXE="wp";
			#endregion			

			//DataSet ds=null;
			string table=WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod) ;
			string product=TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetProductData(webSession);
			string productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
			string mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
			//liste des produit hap
			string listProductHap=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
			string date=WebFunctions.SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
			string period=WebFunctions.SQLGenerator.getMaxDateForPortofolio(webSession.DetailPeriod);

			string sql = null;
			if(DBClassificationConstantes.Vehicles.names.internet.GetHashCode() == idVehicle)
				sql="select sum(totalunite) as investment";
			else sql="select sum (totalpages) as page,sum(totalduree) duree,sum(totalinsert) as insertion,sum(totalunite) as investment,sum(totalmmc)";
            if (DBClassificationConstantes.Vehicles.names.directMarketing.GetHashCode() == idVehicle)
                sql += ",sum(totalvolume) as volume";
			sql+=","+period;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+table+" wp ";
			sql+=" where id_media="+idMedia+"";		
			sql+=" and "+date+">="+dateBegin.Substring(0,6)+" ";						
			sql+=" and "+date+"<="+dateEnd.Substring(0,6)+"";
			sql+=product;
			sql+=productsRights;
			sql+=mediaRights;
			sql+=listProductHap;
			#endregion

			#region Execution de la requ�te
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new MediaAgencyException ("Impossible de charger la liste des ann�es disponible pour les agences media: "+sql,err));
			}
			#endregion

		}

		/// <summary>
		/// R�cup�re le nombre de produits et d'annonceurs dans un support
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de d�but</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>object [] en 0 le nombre de produit en 1 le nombre d'annonceurs</returns>
		public static object [] NumberProductAdvertiser(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
	
			#region Constantes
			//pr�fixe table � utiliser
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Variables
			object [] tab=new object[4];
			string sql="";
			string tableName="";
			int compteur=0;
			string productsRights="";
			string product="";
			string mediaRights="";
			string listProductHap="";
			string date="";
			#endregion
			
			try{
				tableName = WebFunctions.SQLGenerator.getTableNameForAnalysisResult(webSession.DetailPeriod) ;
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				product=TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetProductData(webSession);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				listProductHap=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				date=WebFunctions.SQLGenerator.getDateFieldNameForAnalysisResult(webSession.DetailPeriod);
			}
			catch(System.Exception err){
				throw(new PortofolioAnalysisDataAccessException("Impossible de contruire la requ�te",err));
			}

			#region Execution de la requ�te
			OracleConnection connection=new OracleConnection(webSession.CustomerLogin.OracleConnectionString);
			OracleCommand sqlCommand=null;
			OracleDataAdapter sqlAdapter=null;
			OracleDataReader sqlOracleDataReader=null;
			
			#region Ouverture de la base de donn�es
			bool DBToClosed=false;
			// On teste si la base est d�j� ouverte
			if (connection.State==System.Data.ConnectionState.Closed){
				DBToClosed=true;
				try{
					connection.Open();
				}
				catch(System.Exception et){
					throw(new PortofolioAnalysisDataAccessException("Impossible d'ouvrir la base de donn�es:"+et.Message));
				}
			}
			#endregion		
			
			try{
				for(int i=0 ;i<=1;i++){					
					sql="select count(rowid) as rowid ";					
					sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" wp" ;
					sql+=" where id_media="+idMedia+"";							
					sql+=" and  "+date+">= "+dateBegin.Substring(0,6)+" ";					
					sql+=" and  "+date+"<= "+dateEnd.Substring(0,6)+" ";
					sql+=product;
					sql+=productsRights;
					sql+=mediaRights;
					sql+=listProductHap;
					if(i==0){
						sql+=" group by id_product ";
					}
					else{
						sql+=" group by id_advertiser ";
					}
					sqlCommand=new OracleCommand(sql,connection);
					sqlAdapter=new OracleDataAdapter(sqlCommand);
					sqlOracleDataReader=sqlCommand.ExecuteReader();
				
					while(sqlOracleDataReader.Read()){
					
						compteur++;
					}
					tab[i]=compteur;
					compteur=0;
				}			
			}
			#region Traitement d'erreur du chargement des donn�es
			catch(System.Exception ex){
				try{
					// Fermeture de la base de donn�es
					if (sqlAdapter!=null){
						sqlAdapter.Dispose();
					}
					if(sqlCommand!=null) sqlCommand.Dispose();
					if (DBToClosed) connection.Close();
				}
				catch(System.Exception et){
					throw(new PortofolioAnalysisDataAccessException ("Impossible de fermer la base de donn�es, lors de la gestion d'erreur "+ex.Message+" : "+et.Message));
				}
				throw(new PortofolioAnalysisDataAccessException ("Impossible de charger les donn�es d'un portefeuille:"+sql+" "+ex.Message));
			}
			#endregion		

			#endregion	

			#region Fermeture de la base de donn�es
			try{
				// Fermeture de la base de donn�es
				if (sqlAdapter!=null){
					sqlAdapter.Dispose();
				}
				if(sqlCommand!=null)sqlCommand.Dispose();
				if (DBToClosed) connection.Close();
			}
			catch(System.Exception et){
				throw(new PortofolioAnalysisDataAccessException("Impossible de fermer la base de donn�es :"+et.Message));
			}
			#endregion		

			return tab;
		}
		#endregion

        #region GetRequest
        /// <summary>
		/// Construction de la requ�te pour le chargement des donn�es pour cr�er le d�tail du portefeuille d'un Media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">M�dia � traiter</param>
        /// <param name="type">Type de la table � utiliser</param>
		/// <returns>Donn�es du portefeuille d'un Media</returns>
        public static string GetRequest(WebSession webSession, DBClassificationConstantes.Vehicles.names vehicleName, DBConstantes.TableType.Type type) {
        
            #region Constantes
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

            #region Variables
			string dateField="";
			string dataTableName="";
			string dataTableNameForGad="";
			string dataFieldsForGad="";
			string dataJointForGad="";
			string dataJointForInsert="";
			string detailProductTablesNames="";
			string detailProductFields="";
			string detailProductJoints="";
			string detailProductOrderBy="";
			string unitFields="";
			string productsRights="";
			string sql="";
			string list="";
			string mediaList="";
			bool premier;
			string mediaRights="";
			string listProductHap="";
			string mediaAgencyTable=string.Empty;
			string mediaAgencyJoins=string.Empty;
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
			#endregion

            #region Construction de la requ�te
			try{
                // Table de donn�es
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        dataTableName = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.alert);
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        dataTableName = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult(vehicleName, WebConstantes.Module.Type.analysis);
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dataTableName = DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                        break;
                    default:
                        throw (new CompetitorDataAccessException("Impossible de d�terminer le type de la table pour trouver la table � utiliser"));
                }
				detailProductTablesNames=webSession.GenericProductDetailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				detailProductFields=webSession.GenericProductDetailLevel.GetSqlFields();
				detailProductJoints=webSession.GenericProductDetailLevel.GetSqlJoins(webSession.SiteLanguage,DATA_TABLE_PREFIXE);
                unitFields = WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(vehicleName, type, DATA_TABLE_PREFIXE);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				detailProductOrderBy=webSession.GenericProductDetailLevel.GetSqlOrderFields();
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                    case DBConstantes.TableType.Type.dataVehicle:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        dateField = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				listProductHap=WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				//option encarts (pour la presse)
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					dataJointForInsert=WebFunctions.SQLGenerator.getJointForInsertDetail(webSession,DATA_TABLE_PREFIXE,type);
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.advertiser)){
					try{
						dataTableNameForGad=", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.GAD+" "+DBConstantes.Tables.GAD_PREFIXE;
						dataFieldsForGad=", "+WebFunctions.SQLGenerator.GetFieldsAddressForGad();
						dataJointForGad="and "+WebFunctions.SQLGenerator.GetJointForGad(DATA_TABLE_PREFIXE);
					}
					catch(WebExceptions.SQLGeneratorException){;}
				}
				//Agence_media
				if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.groupMediaAgency)||webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.agency)){
					mediaAgencyTable=DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+webSession.MediaAgencyFileYear+" "+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+",";
					mediaAgencyJoins="And "+DATA_TABLE_PREFIXE+".id_product="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_product ";
					mediaAgencyJoins+="And "+DATA_TABLE_PREFIXE+".id_vehicle="+DBConstantes.Views.PRODUCT_GROUP_ADVER_AGENCY_PREFIXE+".id_vehicle ";
				}

			}
			catch(System.Exception err){
				throw(new PortofolioAnalysisDataAccessException("Impossible d'initialiser les param�tres de la requ�tes",err));
			}
			if(WebFunctions.CheckedText.IsStringEmpty(dataTableName.ToString().Trim())){
                if(customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
				    sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+", "+dateField+" as date_num, "+unitFields;
                else
                    sql+=" select "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+","+unitFields;
				sql+=" from "+mediaAgencyTable+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+dataTableName+" "+DATA_TABLE_PREFIXE;
				if (detailProductTablesNames.Length > 0)
					sql+=", "+detailProductTablesNames;
				sql+=" "+dataTableNameForGad;
                // P�riode
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
				sql+=" "+detailProductJoints;
				sql+=" "+dataJointForGad;
				sql+=" "+mediaAgencyJoins;
				//Jointures encart
				if(DBClassificationConstantes.Vehicles.names.press==vehicleName || DBClassificationConstantes.Vehicles.names.internationalPress==vehicleName)
					sql+=" "+dataJointForInsert;

				#region S�lection de M�dias
				mediaList+=webSession.GetSelection((TreeNode) webSession.ReferenceUniversMedia,CustormerConstantes.Right.type.mediaAccess);						
				if (mediaList.Length>0)sql+=" and "+DATA_TABLE_PREFIXE+".id_media in ("+mediaList+")";
				#endregion

                #region S�lection de Produits
                sql += GetProductSelection(webSession, DATA_TABLE_PREFIXE);
                #endregion

				#region S�lection de Produits (Old)
                //// S�lection en acc�s
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
			
                //// S�lection en Exception
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
				
				// Droits des Produits
				sql+=" "+productsRights;
				sql+=listProductHap;
				// Droits des M�dias
				sql+=mediaRights;				
				
				// Group by
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
				    sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad+", "+dateField;
                else
                    sql+=" group by "+DATA_TABLE_PREFIXE+".id_media, "+detailProductFields+dataFieldsForGad;
			}
			#endregion

            #region Execution de la requ�te
            try {
                return sql.ToString();
            }
            catch (System.Exception err) {
                throw (new MediaAgencyException("Impossible de charger la liste des ann�es disponible pour les agences media: " + sql, err));
            }
            #endregion

        }    
        #endregion

        #region GetGenericData
		/// <summary>
		/// Charge les donn�es pour cr�er le d�tail du portefeuille d'un Media
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleName">M�dia � traiter</param>
		/// <returns>Donn�es du portefeuille d'un Media</returns>
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
            #endregion

            #region Construction de la requ�te
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
                        if(webSession.GenericProductDetailLevel.ContainDetailLevelItem(TNS.AdExpress.Web.Core.DetailLevelItemInformation.Levels.advertiser))
                            dataFieldsForGadWithoutTablePrefix = ", " + WebFunctions.SQLGenerator.GetFieldsAddressForGad("");
                        sql = "";
                        sql += " select id_media, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix +", "+ WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult(vehicleName,Portofolio.DETAIL_PORTOFOLIO);
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";
                        sql += " group by id_media, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
                    }

                    sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";
                }
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de construire la requ�te pour le d�tail du Portefeuille d'un m�dia" + sql, err));
            }
            #endregion

            #region Execution de la requ�te
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger les donn�es pour le d�tail du Portefeuille d'un m�dia" + sql, err));
            }

            #endregion
        
        }
        #endregion

        #region GetGenericSynthesis

        #region GetRequestForAnalysisPortofolio
        /// <summary>
        /// Construction de la requ�te pour recup�rer le total investissement, le nombre de pub, la dur�e des spot
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">identifiant vehicle</param>
        /// <param name="idMedia">identifiant media</param>
        /// <param name="type">Type de la table � utiliser</param>
        /// <returns></returns>
        public static string GetRequestForAnalysisPortofolio(WebSession webSession, Int64 idVehicle, Int64 idMedia, DBConstantes.TableType.Type type) {

            #region Construction de la requ�te

            #region Constantes
            //pr�fixe table � utiliser
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;

            string table = string.Empty;
            // Table de donn�es
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    table = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)idVehicle, WebConstantes.Module.Type.alert);
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    table = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)idVehicle, WebConstantes.Module.Type.analysis);
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    table = DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                    break;
                default:
                    throw (new CompetitorDataAccessException("Impossible de d�terminer le type de la table pour trouver la table � utiliser"));
            }
            string product = TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetProductData(webSession);
            string productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_TABLE_PREFIXE, true);
            string mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
            //liste des produit hap
            string listProductHap = WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
            string date = string.Empty;
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    date = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    date = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                    break;
            }

            string sql = null;
            
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                case DBConstantes.TableType.Type.dataVehicle:
                    sql += "select " + WebFunctions.SQLGenerator.getUnitFieldsNameForAnalysisPortofolio((DBClassificationConstantes.Vehicles.names)idVehicle, DBConstantes.TableType.Type.dataVehicle);
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql += "select " + WebFunctions.SQLGenerator.getUnitFieldsNameForAnalysisPortofolio((DBClassificationConstantes.Vehicles.names)idVehicle, DBConstantes.TableType.Type.webPlan);
                    break;
            }
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql += ", " + date + " as date_num ";
            }
            sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + table + " wp ";
            sql += " where id_media=" + idMedia + "";
            // P�riode
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql += " and " + date + " >=" + customerPeriod.StartDate;
                    sql += " and " + date + " <=" + customerPeriod.EndDate;
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                        sql += " and " + date + " >=" + customerPeriod.StartDate;
                        sql += " and " + date + " <=" + customerPeriod.EndDate;
                    }
                    else if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql += " and ((" + date + " >=" + customerPeriod.PeriodDayBegin[0];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
                        sql += " ) or (" + date + " >=" + customerPeriod.PeriodDayBegin[1];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[1];
                        sql += "))";
                    }
                    else {
                        sql += " and " + date + " >=" + customerPeriod.PeriodDayBegin[0];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
                    }
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql += " and " + date + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
                    sql += " and " + date + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
                    break;
            }

            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                sql += " group by " + date;
            }
            #endregion

            return sql.ToString();

        }
        #endregion

        #region GetGenericDataForAnalysisPortofolio
        // <summary>
		/// Recup�re le total investissement, le nombre de pub, la dur�e des spot
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <returns></returns>
        public static DataSet GetGenericDataForAnalysisPortofolio(WebSession webSession, Int64 idVehicle, Int64 idMedia) {

            #region Variables
            string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            #endregion

            #region Construction de la requ�te
            try {

                if (customerPeriod.Is4M) {
                    sql4M = GetRequestForAnalysisPortofolio(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle4M);
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                    sql = GetRequestForAnalysisPortofolio(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle);
                }
                else {

                    if (customerPeriod.IsDataVehicle) {
                        sqlDataVehicle = GetRequestForAnalysisPortofolio(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle);
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan) {
                        sqlWebPlan = GetRequestForAnalysisPortofolio(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.webPlan);
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                        sql = "";
                        sql += " select " + WebFunctions.SQLGenerator.getUnitFieldsNameForMediaDetailResult((DBClassificationConstantes.Vehicles.names)idVehicle, Portofolio.SYNTHESIS);
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";
                    }

                }
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de construire la requ�te pour recup�rer le total investissement, le nombre de pub, la dur�e des spot" + sql, err));
            }
            #endregion

            #region Execution de la requ�te
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new CompetitorDataAccessException("Impossible de charger les donn�es pour recup�rer le total investissement, le nombre de pub, la dur�e des spot" + sql, err));
            }
            #endregion

        }
        #endregion

        #region GetRequestForNumberProductAdvertiser
        /// <summary>
        /// R�cup�re le nombre de produits et d'annonceurs dans un support
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">identifiant vehicle</param>
        /// <param name="idMedia">identifiant media</param>
        /// <param name="type">Type de la table � utiliser</param>
        /// <param name="index">It�ration</param>
        /// <returns>object [] en 0 le nombre de produit en 1 le nombre d'annonceurs</returns>
        public static string GetRequestForNumberProductAdvertiser(WebSession webSession, Int64 idVehicle, Int64 idMedia, DBConstantes.TableType.Type type, int index) {

            #region Constantes
            //pr�fixe table � utiliser
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            string sql = "";
            string tableName = "";
            string productsRights = "";
            string product = "";
            string mediaRights = "";
            string listProductHap = "";
            string date = "";
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            #endregion

            try {
                // Table de donn�es
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        tableName = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)idVehicle, WebConstantes.Module.Type.alert);
                        break;
                    case DBConstantes.TableType.Type.dataVehicle:
                        tableName = WebFunctions.SQLGenerator.getVehicleTableNameForDetailResult((DBClassificationConstantes.Vehicles.names)idVehicle, WebConstantes.Module.Type.analysis);
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        tableName = DBConstantes.Tables.WEB_PLAN_MEDIA_MONTH;
                        break;
                    default:
                        throw (new CompetitorDataAccessException("Impossible de d�terminer le type de la table pour trouver la table � utiliser"));
                }
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_TABLE_PREFIXE, true);
                product = TNS.AdExpress.Web.DataAccess.Results.PortofolioDataAccess.GetProductData(webSession);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
                listProductHap = WebFunctions.SQLGenerator.getAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                switch (type) {
                    case DBConstantes.TableType.Type.dataVehicle4M:
                    case DBConstantes.TableType.Type.dataVehicle:
                        date = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.DATE_MEDIA_NUM;
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        date = DATA_TABLE_PREFIXE + "." + DBConstantes.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioAnalysisDataAccessException("Impossible de contruire la requ�te", err));
            }

            #region Construction de la requ�te
            if (index == 0) {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql = " select id_product, " + date + " as date_num, count(rowid) as nbLines";
                else
                    sql = " select id_product, count(rowid) as nbLines";
            }
            else {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql = " select id_advertiser, " + date + " as date_num, count(rowid) as nbLines";
                else
                    sql = " select id_advertiser, count(rowid) as nbLines";
            }
            sql += " from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " wp";
            sql += " where id_media=" + idMedia + "";
            // P�riode
            switch (type) {
                case DBConstantes.TableType.Type.dataVehicle4M:
                    sql += " and " + date + " >=" + customerPeriod.StartDate;
                    sql += " and " + date + " <=" + customerPeriod.EndDate;
                    break;
                case DBConstantes.TableType.Type.dataVehicle:
                    if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 0) {
                        sql += " and " + date + " >=" + customerPeriod.StartDate;
                        sql += " and " + date + " <=" + customerPeriod.EndDate;
                    }
                    else if (webSession.CustomerPeriodSelected.PeriodDayBegin.Count == 2) {
                        sql += " and ((" + date + " >=" + customerPeriod.PeriodDayBegin[0];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
                        sql += " ) or (" + date + " >=" + customerPeriod.PeriodDayBegin[1];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[1];
                        sql += "))";
                    }
                    else {
                        sql += " and " + date + " >=" + customerPeriod.PeriodDayBegin[0];
                        sql += " and " + date + " <=" + customerPeriod.PeriodDayEnd[0];
                    }
                    break;
                case DBConstantes.TableType.Type.webPlan:
                    sql += " and " + date + " >=" + customerPeriod.PeriodMonthBegin[0].ToString().Substring(0, 6);
                    sql += " and " + date + " <=" + customerPeriod.PeriodMonthEnd[0].ToString().Substring(0, 6);
                    break;
            }
            sql += product;
            sql += productsRights;
            sql += mediaRights;
            sql += listProductHap;
            if (index == 0) {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql += " group by id_product, " + date;
                else
                    sql += " group by id_product ";
            }
            else {
                if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    sql += " group by id_advertiser, " + date;
                else
                    sql += " group by id_advertiser ";
            }
            #endregion

            return sql;
        }
        #endregion

        #region GetGenericDataForNumberProductAdvertiser
        /// <summary>
		/// R�cup�re le nombre de produits et d'annonceurs dans un support
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <returns>object [] en 0 le nombre de produit en 1 le nombre d'annonceurs</returns>
        public static object[] NumberProductAdvertiser(WebSession webSession, Int64 idVehicle, Int64 idMedia) {

            #region Variables
            string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            CustomerPeriod customerPeriod = webSession.CustomerPeriodSelected;
            object[] tab = new object[4];
            int compteur = 0;
            #endregion

            #region Execution de la requ�te
            OracleConnection connection = new OracleConnection(webSession.CustomerLogin.OracleConnectionString);
            OracleCommand sqlCommand = null;
            OracleDataAdapter sqlAdapter = null;
            OracleDataReader sqlOracleDataReader = null;

            #region Ouverture de la base de donn�es
            bool DBToClosed = false;
            // On teste si la base est d�j� ouverte
            if (connection.State == System.Data.ConnectionState.Closed) {
                DBToClosed = true;
                try {
                    connection.Open();
                }
                catch (System.Exception et) {
                    throw (new PortofolioAnalysisDataAccessException("Impossible d'ouvrir la base de donn�es:" + et.Message));
                }
            }
            #endregion		

            try {

                for (int i = 0; i <= 1; i++) {

                    if (customerPeriod.Is4M) {
                        sql4M = GetRequestForNumberProductAdvertiser(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle4M, i);
                        sql = sql4M;
                    }
                    else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan) {
                        sql = GetRequestForNumberProductAdvertiser(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle, i);
                    }
                    else {

                        if (customerPeriod.IsDataVehicle) {
                            sqlDataVehicle = GetRequestForNumberProductAdvertiser(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.dataVehicle, i);
                            sql = sqlDataVehicle;
                        }

                        if (customerPeriod.IsWebPlan) {
                            sqlWebPlan = GetRequestForNumberProductAdvertiser(webSession, idVehicle, idMedia, DBConstantes.TableType.Type.webPlan, i);
                            sql = sqlWebPlan;
                        }

                        if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan) {
                            sql = "";
                            if (i == 0)
                                sql += " select id_product, sum(nbLines) as nbLines";
                            else
                                sql += " select id_advertiser, sum(nbLines) as nbLines";
                            sql += " from (";
                            sql += sqlDataVehicle;
                            sql += " UNION ";
                            sql += sqlWebPlan;
                            sql += " ) ";
                            if (i == 0) {
                                sql += " group by id_product ";
                            }
                            else {
                                sql += " group by id_advertiser ";
                            }
                        }

                    }

                    sqlCommand = new OracleCommand(sql, connection);
                    sqlAdapter = new OracleDataAdapter(sqlCommand);
                    sqlOracleDataReader = sqlCommand.ExecuteReader();

                    while (sqlOracleDataReader.Read()) {

                        compteur++;
                    }
                    tab[i] = compteur;
                    compteur = 0;
                }
            }

            #region Traitement d'erreur du chargement des donn�es
            catch (System.Exception ex) {
                try {
                    // Fermeture de la base de donn�es
                    if (sqlAdapter != null) {
                        sqlAdapter.Dispose();
                    }
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et) {
                    throw (new PortofolioAnalysisDataAccessException("Impossible de fermer la base de donn�es, lors de la gestion d'erreur " + ex.Message + " : " + et.Message));
                }
                throw (new PortofolioAnalysisDataAccessException("Impossible de charger les donn�es d'un portefeuille:" + sql + " " + ex.Message));
            }
			#endregion		

            #endregion

            #region Fermeture de la base de donn�es
            try {
                // Fermeture de la base de donn�es
                if (sqlAdapter != null) {
                    sqlAdapter.Dispose();
                }
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
            }
            catch (System.Exception et) {
                throw (new PortofolioAnalysisDataAccessException("Impossible de fermer la base de donn�es :" + et.Message));
            }
            #endregion

            return tab;
        }	
        #endregion

        #endregion

        #region M�thodes internes
        /// <summary>
        /// Get product selection
        /// </summary>
        /// <remarks>
        /// Must beginning by AND
        /// </remarks>
        /// <param name="webSession">Client session</param>
        /// <param name="dataTablePrefixe">data table prefixe</param>
        /// <returns>product selection to add as condition into a sql query</returns>
        private static string GetProductSelection(WebSession webSession, string dataTablePrefixe) {
            string sql = "";
            if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
                sql = webSession.PrincipalProductUniverses[0].GetSqlConditions(dataTablePrefixe, true);
            return sql;
        }
        #endregion

    }
}

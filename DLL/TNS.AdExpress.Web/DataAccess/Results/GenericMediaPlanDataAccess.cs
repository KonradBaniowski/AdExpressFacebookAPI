#region Informations
// Auteur: G. Facon
// Date de cr�ation: 30/03/2006
// Date de modification: 
#endregion

#region Using
using System;
using System.Data;
using System.Collections;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using DbSchemas=TNS.AdExpress.Constantes.DB.Schema;
using DbTables=TNS.AdExpress.Constantes.DB.Tables;
using TNS.FrameWork.DB.Common;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using DBTableFieldsName = TNS.AdExpress.Constantes.DB;
using WebCommon=TNS.AdExpress.Web.Common;
using TNS.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results{
	/// <summary>
	/// Acc�s donn�es plan media
	/// 	/// </summary>
	public class GenericMediaPlanDataAccess{

		/// <summary>
		/// Obtient les donn�es d'un calendrier d'action AdNetTrack
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du vehicle si cela est n�cessaire</param>
		/// <param name="beginningDate">Date de d�but</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Donn�es</returns>
		public static DataSet GetAdNetTrackData(WebSession webSession,Int64 vehicleId, string beginningDate,string endDate){
			string additionalConditions="";
			switch(webSession.AdNetTrackSelection.SelectionType){
				case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.advertiser:
					additionalConditions=" AND "+DbTables.WEB_PLAN_PREFIXE+".id_advertiser="+webSession.AdNetTrackSelection.Id.ToString()+" ";
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.product:
					additionalConditions=" AND "+DbTables.WEB_PLAN_PREFIXE+".id_product="+webSession.AdNetTrackSelection.Id.ToString()+" ";
					break;
				case TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.visual:
					additionalConditions=" AND "+DbTables.WEB_PLAN_PREFIXE+".hashcode="+webSession.AdNetTrackSelection.Id.ToString()+" ";
					break;
				default:
					throw(new NotSupportedException("AdNetTrack Selection Type not supported"));
			}

			#region Execution de la requ�te
			try{
				return(webSession.Source.Fill(GetData(webSession,vehicleId,beginningDate,endDate,additionalConditions,webSession.GenericAdNetTrackDetailLevel)));
			}
			catch(System.Exception err){ 
				throw(new WebExceptions.MediaPlanDataAccessException ("Impossible de charger le plan media",err));
			}
			#endregion
		
		}
		/// <summary>
		/// Obtient les donn�es d'un calendrier d'action pour une analyse plan m�dia
		/// </summary>
		/// <remarks>A utiliser pour les analyses</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Donn�es</returns>
		public static DataSet GetDataWithMediaDetailLevel(WebSession webSession){
			#region Execution de la requ�te
			try{
				return(webSession.Source.Fill(GetData(webSession,-1,webSession.PeriodBeginningDate,webSession.PeriodEndDate)));
			}
			catch(System.Exception err){ 
				throw(new WebExceptions.MediaPlanDataAccessException ("Impossible de charger le plan media",err));
			}
			#endregion
		}

		/// <summary>
		/// Obtient les donn�es d'un calendrier d'action pour une analyse plan m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du vehicle si cela est n�cessaire</param>
		/// <param name="beginningDate">Date de d�but</param>
		/// <param name="endDate">Date de fin</param>
		/// <returns>Donn�es</returns>
		public static string GetData(WebSession webSession,Int64 vehicleId, string beginningDate,string endDate) {
			return(GetData(webSession,vehicleId, beginningDate,endDate,"",webSession.GenericMediaDetailLevel));
		}
		/// <summary>
		/// Obtient les donn�es d'un calendrier d'action pour une analyse plan m�dia
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du vehicle si cela est n�cessaire</param>
		/// <param name="beginningDate">Date de d�but</param>
		/// <param name="endDate">Date de fin</param>
		/// <param name="additionalConditions">Conditions addionnelles</param>
		/// <param name="detailLevel">Generic detail level to use for the request</param>
		/// <returns>Donn�es</returns>
		/// <remarks>
		/// Les conditions addtionnelles doivent commencer par le mot cl� AND
		/// </remarks>
		public static string GetData(WebSession webSession,Int64 vehicleId, string beginningDate,string endDate,string additionalConditions,GenericDetailLevel detailLevel){

			#region Variables
			string list="";
			//bool premier=true;
			string tableName=null;
			string mediaTableName=null;
			string dateFieldName=null;
			string mediaFieldName=null;
			string mediaPeriodicity=null;
			string orderFieldName=null;
			string unitFieldName=null;
			string mediaJoinCondition=null;
			string groupByFieldName=null;
			#endregion
			
			#region Construction de la requ�te
			try{

				// Obtient le nom de la table de donn�es
				tableName=GetMediaTableName(webSession,vehicleId);
				// Obtient les tables de la nomenclature
				//mediaTableName=GetMediaTable(webSession.PreformatedMediaDetail);
				mediaTableName=detailLevel.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
				if(mediaTableName.Length>0)mediaTableName+=",";
				// Obtient le champs unit�
				dateFieldName=GetDateFieldName(webSession);
				// cas particulier outdoor en alerte
				if((DBClassificationConstantes.Vehicles.names)vehicleId==DBClassificationConstantes.Vehicles.names.outdoor && webSession.Unit== TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion && WebConstantes.CustomerSessions.Period.DisplayLevel.dayly==webSession.DetailPeriod)
					unitFieldName=Fields.NUMBER_BOARD;
				else
					unitFieldName=GetUnitFieldName(webSession);
				//SQL Pour la p�riodicit�
				mediaPeriodicity=GetPeriodicity(webSession.DetailPeriod,vehicleId);
				// Obtient les champs de la nomenclature
				//mediaFieldName=GetMediaFields(webSession.PreformatedMediaDetail);
				mediaFieldName=detailLevel.GetSqlFields();
				// Obtient l'ordre des champs
				//orderFieldName=GetOrderMediaFields(webSession.PreformatedMediaDetail);
				orderFieldName=detailLevel.GetSqlOrderFields();
				// obtient la clause group by
				groupByFieldName=detailLevel.GetSqlGroupByFields();
				// Obtient les jointures pour la nomenclature
				//mediaJoinCondition=GetMediaJoinConditions(webSession,DbTables.WEB_PLAN_PREFIXE,false);
				mediaJoinCondition=detailLevel.GetSqlJoins(webSession.SiteLanguage,DbTables.WEB_PLAN_PREFIXE);
			}
			catch(System.Exception err){
				throw(new WebExceptions.MediaPlanDataAccessException("Impossible de construire la requ�te",err));	
			}

			string sql="";
			// S�lection de la nomenclature Support
			sql+="select "+mediaFieldName+" ";
			// S�lection de la date
			sql+=", "+dateFieldName+" as date_num,";
			//sql+=",max(wp.id_periodicity) as id_periodicity";
			sql+=mediaPeriodicity+",";
			// S�lection de l'unit� sauf pour AdNetTrack
			if((DBClassificationConstantes.Vehicles.names)vehicleId==DBClassificationConstantes.Vehicles.names.adnettrack)
				sql+="sum(OCCURRENCE) as unit";
			else
				sql+="sum("+unitFieldName+") as unit";
			// Tables
			sql+=" from "+mediaTableName+tableName+" wp,"+DbSchemas.ADEXPRESS_SCHEMA+"."+DbTables.PERIODICITY+" "+DbTables.PERIODICITY_PREFIXE+" ";
			//Conditions media
			sql+="where 0=0 "+mediaJoinCondition+"";
			// Jointure P�riodicit�
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".activation<"+DBConstantes.ActivationValues.UNACTIVATED;
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_language=33";
			sql+=" and "+DbTables.PERIODICITY_PREFIXE+".id_periodicity=wp.id_periodicity";
			// P�riode
			sql+=" and "+dateFieldName+">="+beginningDate;
			sql+=" and "+dateFieldName+"<="+endDate;

			// Conditions additionnelles
			if(additionalConditions.Length>0){
				sql+=" "+additionalConditions+" ";
			}

			// Sous s�lection de version
			string slogans=webSession.SloganIdList;
			// Zoom sur une version
			if(webSession.SloganIdZoom>0 && webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA){
				sql+=" and wp.id_slogan ="+webSession.SloganIdZoom+" ";
			}
			else{
				// affiner les version
				if(slogans.Length>0 && webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA){
					sql+=" and wp.id_slogan in("+slogans+") ";
				}
			}
			
			// Gestion des s�lections et des droits

			#region Nomenclature Produit (droits)
			//Droits en acc�s
			sql+=SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			// Produit � exclure en radio
			sql+=SQLGenerator.getAdExpressProductUniverseCondition(TNS.AdExpress.Constantes.Web.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DbTables.WEB_PLAN_PREFIXE,true,false);
			#endregion

			#region Nomenclature Produit (Niveau de d�tail)  
			// Niveau de produit
			sql+=SQLGenerator.getLevelProduct(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			#endregion

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et s�lection) 
		
			#region S�lection
			sql+=GetProductSelection(webSession,DbTables.WEB_PLAN_PREFIXE);			
			#endregion

			#endregion

			#region Nomenclature Media (droits et s�lection)

			#region Droits
			// On ne tient pas compte des droits vehicle pour les plans media AdNetTrack
			if((DBClassificationConstantes.Vehicles.names)vehicleId==DBClassificationConstantes.Vehicles.names.adnettrack)
				sql+=SQLGenerator.GetAdNetTrackMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			else
				sql+=SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.WEB_PLAN_PREFIXE,true);
			#endregion

			#region S�lection
			if(webSession.DetailPeriod!=WebConstantes.CustomerSessions.Period.DisplayLevel.dayly){
				list=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerRightConstante.type.vehicleAccess);
				if(list.Length>0) sql+=" and (("+DbTables.WEB_PLAN_PREFIXE+".id_vehicle in ("+list+"))) ";
			}
			else
				sql+=" and ((wp.id_vehicle= "+vehicleId+")) ";
			#endregion

			#endregion
			
			// Ordre
			sql+="Group by "+groupByFieldName+" ";
			// et la date
			sql+=", "+dateFieldName+" ";

			// Ordre
			if(UseOrder(webSession.DetailPeriod,vehicleId)){
				sql+="Order by "+orderFieldName+" ";
				// et la date
				sql+=", "+dateFieldName+" ";
			}

			#endregion

			return(sql);		

		}

		/// <summary>
		/// Charge les donn�es pour cr�er un plan m�dia pour chaque vehicle s�lectionn� dans la session
		/// sur la p�riode sp�cifi�e
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="beginningDate">date de d�but de p�riode au format YYYYMMDD</param>
		/// <param name="endDate">date de fin de p�riode au formazt YYYYMMDD</param>
		/// <returns>Dataset contenant les donn�es rapatri�es de la base de donn�es pour les vehicule s�lectionn� dans la session</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lanc�e en cas d'erreur sur un vehicle
		/// </exception>
		/// <remarks>
		/// Utilise les m�thodes:
		///		public string TNS.AdExpress.Web.Core.Sessions.WebSession.GetSelection(TreeNode univers, TNS.AdExpress.Constantes.Customer.Right.type level)
		/// </remarks>
		public static DataSet GetPluriMediaDataSetWithMediaDetailLevel(WebSession webSession, string beginningDate, string endDate){
			bool first=true;
			string[] listVehicles = webSession.GetSelection(webSession.SelectionUniversMedia, CustomerRightConstante.type.vehicleAccess).Split(new char[]{','});
			DataSet ds = new DataSet();
			string sql="select "+webSession.GenericMediaDetailLevel.GetSqlFieldsWithoutTablePrefix()+",date_num,"+DbSchemas.ADEXPRESS_SCHEMA+".RECUP_ID_PERIOD(max(value_second))as id_periodicity,sum(unit) as unit from (";
			for(int i=0; i< listVehicles.Length; i++){
				try{
					if(!first)sql+= " union ";
					else first=false;
					sql+="("+GetData(webSession,Int64.Parse(listVehicles[i]),beginningDate,endDate)+")";
				}
				catch(System.Exception err){
					throw new WebExceptions.MediaPlanDataAccessException("MediaPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate",err);
				}
			}
			sql+=") ";
			sql+="group by "+webSession.GenericMediaDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix()+",date_num ";
			sql+="order by "+webSession.GenericMediaDetailLevel.GetSqlOrderFieldsWithoutTablePrefix()+", date_num ";
			
			#region Execution de la requ�te
				try{
					return webSession.Source.Fill(sql.ToString());
				}
				catch(System.Exception err){
					throw(new WebExceptions.MediaPlanDataAccessException ("Impossible de charger le plan media: "+sql,err));
				}
			#endregion
		}


		/// <summary>
		/// Obtient les donn�es pour cr�er un plan m�dia plurim�dia � partir  des donn�es 
		/// de la session d'un client
		/// </summary>
		/// <remarks>A utiliser pour les alertes</remarks>
		/// <param name="webSession">Session du client</param>
		/// <returns>Donn�es charg�es</returns>
		/// <exception cref="TNS.AdExpress.Web.Exceptions.MediaPlanAlertDataAccessException">
		/// Lanc�e en cas d'erreur
		/// </exception>
		public static DataSet GetPluriMediaDataSetWithMediaDetailLevel(WebSession webSession){
			try{
				return GetPluriMediaDataSetWithMediaDetailLevel(webSession, webSession.PeriodBeginningDate, webSession.PeriodEndDate);
			}
			catch(System.Exception err){
				throw new WebExceptions.MediaPlanDataAccessException("MedaiPlanAlertDataAccess.getPluriMediaDataset(WebSession webSession, string beginningDate, string endDate)",err);
			}
		}


		#region M�thode internes nouvelle formule
		/*** Nouvelle gestion des d�tails support ***/

		#region Donne la s�lection produit
		
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

		#region Donne les champs � utiliser pour l'unit� dans la requ�te
		/// <summary>
		/// Indique le champ � utiliser pour l'unit� dans la requ�te
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Le champ correspondant au type d'unit�</returns>
		private static string GetUnitFieldName(WebSession webSession){
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
				switch(webSession.Unit){
					case WebConstantes.CustomerSessions.Unit.euro:
					case WebConstantes.CustomerSessions.Unit.kEuro:
						return(Fields.WEB_PLAN_MEDIA_MONTH_EURO_FIELD);
					case WebConstantes.CustomerSessions.Unit.mmPerCol:
						return(Fields.WEB_PLAN_MEDIA_MONTH_MMC_FIELD);
					case WebConstantes.CustomerSessions.Unit.pages:
						return(Fields.WEB_PLAN_MEDIA_MONTH_PAGES_FIELD);
					case WebConstantes.CustomerSessions.Unit.insertion:
					case WebConstantes.CustomerSessions.Unit.numberBoard:
						return(Fields.WEB_PLAN_MEDIA_MONTH_INSERT_FIELD);
					case WebConstantes.CustomerSessions.Unit.spot:
						return(Fields.WEB_PLAN_MEDIA_MONTH_INSERT_FIELD);
					case WebConstantes.CustomerSessions.Unit.duration:
						return(Fields.WEB_PLAN_MEDIA_MONTH_DUREE_FIELD);
                    case WebConstantes.CustomerSessions.Unit.volume:
                        if (webSession.CustomerLogin.GetFlag(DBConstantes.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                            return(Fields.WEB_PLAN_MEDIA_MONTH_VOLUME_FIELD);
                        else
                            return(Fields.WEB_PLAN_MEDIA_MONTH_EURO_FIELD);
					default:
						throw(new WebExceptions.SQLGeneratorException("Le d�tails unit� s�lectionn� est incorrect pour le choix du champ"));
				}
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
				switch(webSession.Unit){
					case WebConstantes.CustomerSessions.Unit.euro:
					case WebConstantes.CustomerSessions.Unit.kEuro:
						return(Fields.EXPENDITURE_EURO);
					case WebConstantes.CustomerSessions.Unit.mmPerCol:
						return(Fields.AREA_MMC);
					case WebConstantes.CustomerSessions.Unit.pages:
						return(Fields.AREA_PAGE);
					case WebConstantes.CustomerSessions.Unit.spot:
					case WebConstantes.CustomerSessions.Unit.insertion:
						return(Fields.INSERTION);
					case WebConstantes.CustomerSessions.Unit.numberBoard:
						return(Fields.NUMBER_BOARD);
					case WebConstantes.CustomerSessions.Unit.duration:
						return(Fields.DURATION);
                    case WebConstantes.CustomerSessions.Unit.volume:
                        if (webSession.CustomerLogin.GetFlag(DBConstantes.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                            return(Fields.VOLUME);
                        else
                            return(Fields.EXPENDITURE_EURO);
					default:
						throw(new WebExceptions.SQLGeneratorException("Le d�tails unit� s�lectionn� est incorrect pour le choix du champ"));
				}
				default:
					throw(new WebExceptions.SQLGeneratorException("Le d�tails P�riode s�lectionn� est incorrect pour le choix des unit�s"));

			}
		}
		#endregion

		#region Date
		/// <summary>
		/// Indique le champ � utilis�e pour la date dans la requ�te
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <returns>Le champ correspondant au type de p�riode</returns>
		private static string GetDateFieldName(WebSession webSession){
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					return(Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(Fields.WEB_PLAN_MEDIA_WEEK_DATE_FIELD);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					return("date_media_num");
				default:
					throw(new WebExceptions.MediaPlanDataAccessException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
			}
		}
		#endregion

		#region P�riodicit�
		/// <summary>
		/// Indique le champ � utilis�e pour la date dans la requ�te
		/// </summary>
		///<param name="detailPeriod">Detail period type</param>
		///<param name="vehicleId">Vehicle Id</param>
		/// <returns>Le champ correspondant au type de p�riode</returns>
		private static string GetPeriodicity(WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriod,Int64 vehicleId){
			if(vehicleId==(Int64)DBClassificationConstantes.Vehicles.names.adnettrack)
				return(" "+DbSchemas.ADEXPRESS_SCHEMA+".RECUP_ID_PERIOD(max(value_second))as id_periodicity ");
			switch(detailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(" "+DbSchemas.ADEXPRESS_SCHEMA+".RECUP_ID_PERIOD(max(value_second))as id_periodicity ");
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					return(" max("+DbTables.PERIODICITY_PREFIXE+".value_second) as value_second ");
				default:
					throw(new WebExceptions.MediaPlanDataAccessException("Le d�tails p�riode s�lectionn� est incorrect pour le choix du champ"));
			}
		}
		#endregion

		/// <summary>
		/// Indique la table � utilis�e pour la requ�te
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="vehicleId">Identifiant du media</param>
		/// <returns>La table correspondant au type de p�riode</returns>
		private static string GetMediaTableName(WebSession webSession,Int64 vehicleId){
			switch(webSession.DetailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
					return(DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+Tables.WEB_PLAN_MEDIA_MONTH);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+Tables.WEB_PLAN_MEDIA_WEEK);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
					switch(currentModuleDescription.ModuleType){
						case WebConstantes.Module.Type.alert:
							switch((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
								case DBClassificationConstantes.Vehicles.names.press:								
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_PRESS;
								case DBClassificationConstantes.Vehicles.names.internationalPress:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_PRESS_INTER;
								case DBClassificationConstantes.Vehicles.names.radio:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_RADIO;
								case DBClassificationConstantes.Vehicles.names.tv:
								case DBClassificationConstantes.Vehicles.names.others:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_TV;
								case DBClassificationConstantes.Vehicles.names.outdoor:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_OUTDOOR;
								case DBClassificationConstantes.Vehicles.names.adnettrack:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_ADNETTRACK;
								case DBClassificationConstantes.Vehicles.names.internet :
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_INTERNET;
                                case DBClassificationConstantes.Vehicles.names.directMarketing:
                                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.ALERT_DATA_MARKETING_DIRECT;
								default:
									throw(new WebExceptions.SQLGeneratorException("Impossible de d�terminer la table media � utiliser"));
							}
						case WebConstantes.Module.Type.analysis:
							switch((DBClassificationConstantes.Vehicles.names)Convert.ToInt32(vehicleId.ToString())){
								case DBClassificationConstantes.Vehicles.names.press:								
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS;
								case DBClassificationConstantes.Vehicles.names.internationalPress:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_PRESS_INTER;
								case DBClassificationConstantes.Vehicles.names.radio:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_RADIO;
								case DBClassificationConstantes.Vehicles.names.tv:
								case DBClassificationConstantes.Vehicles.names.others:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_TV;
								case DBClassificationConstantes.Vehicles.names.outdoor:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_OUTDOOR;
								case DBClassificationConstantes.Vehicles.names.adnettrack:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_ADNETTRACK;
								case DBClassificationConstantes.Vehicles.names.internet:
									return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_INTERNET;
                                case DBClassificationConstantes.Vehicles.names.directMarketing:
                                    return DBTableFieldsName.Schema.ADEXPRESS_SCHEMA+"."+DBTableFieldsName.Tables.DATA_MARKETING_DIRECT;
								default:
									throw(new WebExceptions.SQLGeneratorException("Impossible de d�terminer la table media � utiliser"));
							}
						default:
							throw(new WebExceptions.SQLGeneratorException("Impossible de d�terminer le type du module pour d�terminer la table � utiliser"));
					}
				default:
					throw(new WebExceptions.SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
			}
		}

		/// <summary>
		/// Indique la table � utilis�e pour la requ�te
		/// </summary>
		///<param name="detailPeriod">Detail period type</param>
		///<param name="vehicleId">Vehicle Id</param>
		/// <returns>La table correspondant au type de p�riode</returns>
		private static bool UseOrder(WebConstantes.CustomerSessions.Period.DisplayLevel detailPeriod,Int64 vehicleId){
			if(vehicleId==(Int64)DBClassificationConstantes.Vehicles.names.adnettrack)return(true);
			switch(detailPeriod){
				case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
				case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
					return(true);
				case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
					return(false);
				default:
					throw(new WebExceptions.SQLGeneratorException("Le d�tails p�riode s�lectionn� est incorrect pour le choix de la table"));
			}
		}

		/// <summary>
		/// Get field names for SQL Order code
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <returns>field names</returns>
		private static string GetFieldOrderForDataView(WebSession webSession){
			string sql=webSession.GenericMediaDetailLevel.GetSqlOrderFields().ToUpper();
			string tmp="";
			const string POINT_SEARCH=".";
			string[] fields=sql.Split(',');
			foreach(string currentfield in fields){
				tmp+=currentfield.Substring(currentfield.IndexOf(POINT_SEARCH)+1,currentfield.Length-(currentfield.IndexOf(POINT_SEARCH)+1))+",";
			}
			if(tmp.Length>0)tmp=tmp.Substring(0,tmp.Length-1);
			return(tmp);
		}
		#endregion

	}
}


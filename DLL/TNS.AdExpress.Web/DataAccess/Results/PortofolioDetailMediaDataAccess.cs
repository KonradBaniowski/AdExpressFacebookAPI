#region Informations
// Auteur: D. V. Mussuma
// Date de création: 09/12/2005
// Date de modification: 
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
using DbTables = TNS.AdExpress.Constantes.DB.Tables;
using TNS.AdExpress.Constantes.FrameWork.Results;
using WebDataAccess = TNS.AdExpress.Web.DataAccess;
using TNS.AdExpress.Web.Functions;
using TNS.FrameWork.DB.Common;
#endregion

namespace TNS.AdExpress.Web.DataAccess.Results
{
	/// <summary>
	/// Détail du portefeuille d'un support
	/// </summary>
	public class PortofolioDetailMediaDataAccess
	{
		#region Détail Support
		/// <summary>
		/// Récupère la liste des publicités pour un média donné
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>		
		/// <returns>liste des publicités pour un média donné</returns>
		public static DataSet GetDetailMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){
			return GetDetailMedia(webSession,idVehicle,idMedia,dateBegin,dateEnd,"",false);
		}

		/// <summary>
		/// Récupère la liste des publicités pour un média donné
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="code_ecran">code ecran</param>
		/// <param name="allPeriod">vrai si le détail des insertions concerne toute la période sélectionnée</param>
		/// <returns>liste des publicités pour un média donné</returns>
		public static DataSet GetDetailMedia(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd,string code_ecran,bool allPeriod){
		
			#region Constantes
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Variables
			string selectFields="";
			string tableName ="";
			string listProductHap="";
			string product="";
			string productsRights="";
			string mediaRights="";
			string orderby="";
			string mediaAgencyYear="";
			#endregion

			dateBegin = WebFunctions.Dates.getPeriodBeginningDate(dateBegin, webSession.PeriodType).ToString("yyyyMMdd");
			dateEnd = WebFunctions.Dates.getPeriodEndDate(dateEnd, webSession.PeriodType).ToString("yyyyMMdd");

			#region Construction de la requête

			try{
				DataTable dt=TNS.AdExpress.Web.DataAccess.Results.MediaAgencyDataAccess.GetListYear(webSession).Tables[0];
				
				if(dt!=null && dt.Rows.Count>0){
					//On récupère la dernière année des agences médias
					mediaAgencyYear = dt.Rows[0]["year"].ToString();
				}

				selectFields=GetFieldsDetailMedia((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),mediaAgencyYear);
//				tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForAlertDetailResult((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				product=GetProductData(webSession);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
				orderby=GetOrderByDetailMedia((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()),allPeriod,false);
				
			
			}
			catch(System.Exception err){
				throw(new PortofolioDataAccessException("Impossible de construire la requête",err));
			}
			
			StringBuilder sql=new StringBuilder(500);

			sql.Append("select "+selectFields);
			sql.Append(" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DATA_TABLE_PREFIXE+"");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".advertiser ad");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product pr");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".sector se");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".group_ gr");
			if(mediaAgencyYear.Length>0)
//			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".product_group_adver_agency pgaa ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"." + DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY + mediaAgencyYear+" pgaa ");
			sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".media me ");
			// A changer pour inter si le nom de la table est différent
			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_PRESS)==0){
				sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+".color co ");
				sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".format fo ");
				sql.Append(", "+DBConstantes.Schema.ADEXPRESS_SCHEMA+".location lo ");
				sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"." + DBConstantes.Tables.DATA_LOCATION + " dl ");
				sql.Append(","+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+DBConstantes.Tables.APPLICATION_MEDIA+" am ");
			}
			
			// Conditions
			sql.Append(" where wp.id_media="+idMedia+" ");
			//sql.Append(" and wp.insertion=1 ");
			//	sql.Append(" and wp.id_language_data_i="+webSession.SiteLanguage+" ");
			sql.Append(" and wp.date_media_num>="+dateBegin+" ");
			sql.Append(" and wp.date_media_num<="+dateEnd+" ");

			// A changer pour inter si le nom de la table est différent
			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_PRESS)==0){
				sql.Append(" and (am.id_media(+) = wp.id_media ");
				//	sql.Append(" and am.id_language_data_i(+) = wp.id_language_data_i ");
                sql.Append(" and am.date_debut(+) = wp.date_media_num ");
				sql.Append(" and am.id_project(+) = "+ CstProject.ADEXPRESS_ID +") ");
				sql.Append(" and co.id_color(+)=wp.id_color ");
				sql.Append(" and  fo.id_format(+)=wp.id_format ");
				sql.Append(" and lo.id_location(+)=dl.id_location ");
				sql.Append(" and dl.id_media(+)=wp.id_media ");
                sql.Append(" and dl.date_media_num(+) =wp.date_media_num ");
				sql.Append(" and dl.id_advertisement (+)=wp.id_advertisement ");
				//	sql.Append(" and dl.id_language_data_i(+)="+webSession.SiteLanguage+" ");

				sql.Append(" and co.id_language="+webSession.SiteLanguage+" ");
				sql.Append(" and fo.id_language="+webSession.SiteLanguage+" ");
				sql.Append(" and lo.id_language (+)="+webSession.SiteLanguage+" ");

				sql.Append(" and co.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
				sql.Append(" and fo.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
				sql.Append(" and lo.activation(+)<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");
				sql.Append(" and dl.activation(+)<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"");
			}


			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_RADIO)==0 && WebFunctions.CheckedText.IsStringEmpty(code_ecran)){
				sql.Append("and commercial_break="+code_ecran+"");
			}

			if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_TV)==0 && WebFunctions.CheckedText.IsStringEmpty(code_ecran)){ 
				sql.Append(" and id_commercial_break="+code_ecran+"");
			}

			sql.Append(" and ad.id_advertiser=wp.id_advertiser ");
			sql.Append(" and pr.id_product=wp.id_product ");
			sql.Append(" and se.id_sector=wp.id_sector ");
			sql.Append(" and gr.id_group_=wp.id_group_ ");			
			sql.Append(" and  me.id_media=wp.id_media ");


			sql.Append(" and ad.id_language="+webSession.SiteLanguage+" ");
			sql.Append(" and pr.id_language="+webSession.SiteLanguage+" ");
			sql.Append(" and se.id_language="+webSession.SiteLanguage+" ");
			sql.Append(" and gr.id_language="+webSession.SiteLanguage+" ");
			sql.Append(" and me.id_language="+webSession.SiteLanguage+" ");
			

			sql.Append(" and ad.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"");
			sql.Append(" and pr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+" ");			
			sql.Append(" and se.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"");
			sql.Append(" and gr.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"");
			sql.Append(" and me.activation<"+TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED+"");

			// Agence média
			if(mediaAgencyYear.Length>0){
				sql.Append(" and pgaa.id_product(+)=wp.id_product ");
				sql.Append(" and pgaa.id_language(+)="+webSession.SiteLanguage+" ");
				sql.Append(" and pgaa.id_vehicle(+)="+idVehicle+" ");
			}
			
			#region Droits
			//liste des produit hap
			sql.Append(listProductHap);
			sql.Append(product);
			sql.Append(productsRights);
			sql.Append(mediaRights);

			#endregion

			// Order by
				
			sql.Append(orderby);

			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.PortofolioDetailMediaDataAccessException("Impossible de charger des données pour les nouveauté: "+sql,err));
			}
			#endregion

		}
	
		/// <summary>
		/// Liste des codes ecrans pour la télévision et la radion
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">Identifiant vehicle</param>
		/// <param name="idMedia">identifiant media</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <returns>Liste des codes ecrans</returns>
		public static DataSet GetCommercialBreakForTvRadio(WebSession webSession,Int64 idVehicle,Int64 idMedia,string dateBegin,string dateEnd){

			#region Constantes
			const string DATA_TABLE_PREFIXE="wp";
			#endregion

			#region Varaibles
			string selectFields="";
			string tableName ="";
			string groupByFields="";
			string listProductHap="";
			string product="";
			string productsRights="";
			string mediaRights="";
			string sql="";
			#endregion

			#region Construction de la requête

			try{
				selectFields=GetFieldsDetailMediaForTvRadio((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
//				tableName = GetTableData((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				tableName = WebFunctions.SQLGenerator.GetVehicleTableNameForAlertDetailResult((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				groupByFields= GetGroupByDetailMediaForTvRadio((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));
				listProductHap=WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID,DATA_TABLE_PREFIXE,true,false);
				product=GetProductData(webSession);
				productsRights=WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DATA_TABLE_PREFIXE,true);
				mediaRights=WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DATA_TABLE_PREFIXE,true);
			}
			catch(System.Exception err){
				throw(new PortofolioDataAccessException("Impossible de construire la requête",err));
			}

			sql+="select "+selectFields;
			sql+=" from "+DBConstantes.Schema.ADEXPRESS_SCHEMA+"."+tableName+" "+DATA_TABLE_PREFIXE+" ";
			sql+=" where id_media="+idMedia+"  ";
			//		sql+=" and id_language_data_i="+webSession.SiteLanguage+" ";
			sql+=" and date_media_num>="+dateBegin+" ";
			sql+=" and date_media_num<="+dateEnd+" ";
			sql+=" and insertion=1 ";
			sql+=listProductHap;
			sql+=product;
			sql+=productsRights;
			sql+=mediaRights;
			sql+=groupByFields;


			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new WebExceptions.PortofolioDetailMediaDataAccessException("Impossible de charger des données pour les nouveauté: "+sql,err));
			}
			#endregion
		
		}
		#endregion

        #region Generic Détail Support
        /// <summary>
        /// Récupère la liste des publicités pour un média donné
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">identifiant vehicle</param>
        /// <param name="idMedia">identifiant média</param>
        /// <param name="dateBegin">date de début</param>
        /// <param name="dateEnd">date de fin</param>		
        /// <returns>liste des publicités pour un média donné</returns>
        public static DataSet GetGenericDetailMedia(WebSession webSession, Int64 idVehicle, Int64 idMedia, string mediaAgencyYear, string dateBegin, string dateEnd) {
            return GetGenericDetailMedia(webSession, idVehicle, idMedia, mediaAgencyYear, dateBegin, dateEnd, "", false);
        }

        /// <summary>
		/// Récupère la liste des publicités pour un média donné
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idMedia">identifiant média</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="code_ecran">code ecran</param>
		/// <param name="allPeriod">vrai si le détail des insertions concerne toute la période sélectionnée</param>
		/// <returns>liste des publicités pour un média donné</returns>
        public static DataSet GetGenericDetailMedia(WebSession webSession, Int64 idVehicle, Int64 idMedia, string mediaAgencyYear, string dateBegin, string dateEnd, string code_ecran, bool allPeriod) {

            #region Constantes
            const string DATA_TABLE_PREFIXE = "wp";
            #endregion

            #region Variables
            StringBuilder sql = new StringBuilder(5000);
            string sqlFields = "";
            string sqlConstraintFields = "";
            string sqlTables = "";
            string sqlConstraintTables = "";
            string listProductHap = "";
            string product = "";
            string productsRights = "";
            string mediaRights = "";
            string orderby = "";
            #endregion

            try {

                dateBegin = WebFunctions.Dates.getPeriodBeginningDate(dateBegin, webSession.PeriodType).ToString("yyyyMMdd");
                dateEnd = WebFunctions.Dates.getPeriodEndDate(dateEnd, webSession.PeriodType).ToString("yyyyMMdd");

                //Select
                sql.Append(" select distinct");
                sqlFields = webSession.GenericInsertionColumns.GetSqlFields(null);
                if (sqlFields.Length > 0) {
                    sql.Append(" " + sqlFields);
                }

                if (mediaAgencyYear.Length > 0) sql.Append(" , advertising_agency");

                if (idVehicle == DBClassificationConstantes.Vehicles.names.press.GetHashCode() || idVehicle == DBClassificationConstantes.Vehicles.names.internationalPress.GetHashCode())
                    sql.Append(" , date_cover_num");

                sqlConstraintFields = webSession.GenericInsertionColumns.GetSqlConstraintFields();
                if (sqlConstraintFields.Length > 0)
                    sql.Append(" , " + sqlConstraintFields);//Champs pour la gestion des contraintes métiers
                
                //Tables
                string tableName = "";
                tableName = SQLGenerator.GetVehicleTableNameForAlertDetailResult((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()));

                sql.Append(" from ");
                sql.Append(" " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + tableName + " " + DbTables.WEB_PLAN_PREFIXE);
                if (mediaAgencyYear.Length > 0)
                    sql.Append(", " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.PRODUCT_GROUP_ADV_AGENCY + mediaAgencyYear + " pgaa ");
                sqlTables = webSession.GenericInsertionColumns.GetSqlTables(DBConstantes.Schema.ADEXPRESS_SCHEMA, null);
                if (sqlTables.Length > 0) {
                    sql.Append(" ," + sqlTables);
                }
                sqlConstraintTables = webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);
                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Conditions de jointure
                sql.Append(" Where ");

                // Conditions
                sql.Append(" wp.id_media=" + idMedia + " ");
                sql.Append(" and wp.date_media_num>=" + dateBegin + " ");
                sql.Append(" and wp.date_media_num<=" + dateEnd + " ");

                if (webSession.GenericInsertionColumns.GetSqlJoins(webSession.SiteLanguage, DbTables.WEB_PLAN_PREFIXE, null).Length > 0)
                    sql.Append("  " + webSession.GenericInsertionColumns.GetSqlJoins(webSession.SiteLanguage, DbTables.WEB_PLAN_PREFIXE, null));
                sql.Append("  " + webSession.GenericInsertionColumns.GetSqlContraintJoins());

                if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_RADIO) == 0 && WebFunctions.CheckedText.IsStringEmpty(code_ecran)) {
                    sql.Append("and commercial_break=" + code_ecran + "");
                }

                if (tableName.CompareTo(DBConstantes.Tables.ALERT_DATA_TV) == 0 && WebFunctions.CheckedText.IsStringEmpty(code_ecran)) {
                    sql.Append(" and id_commercial_break=" + code_ecran + "");
                }

                listProductHap = WebFunctions.SQLGenerator.GetAdExpressProductUniverseCondition(WebConstantes.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, DATA_TABLE_PREFIXE, true, false);
                product = GetProductData(webSession);
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_TABLE_PREFIXE, true);
                mediaRights = WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_TABLE_PREFIXE, true);
                orderby = GetOrderByDetailMedia((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()), allPeriod,true);

                // Agence média
                if (mediaAgencyYear.Length > 0) {
                    sql.Append(" and pgaa.id_product(+)=wp.id_product ");
                    sql.Append(" and pgaa.id_language(+)=" + webSession.SiteLanguage + " ");
                    sql.Append(" and pgaa.id_vehicle(+)=" + idVehicle + " ");
                }

                #region Droits
                //liste des produit hap
                sql.Append(listProductHap);
                sql.Append(product);
                sql.Append(productsRights);
                sql.Append(mediaRights);

                //Droit detail spot à spot TNT
                if ((DBClassificationConstantes.Vehicles.names)int.Parse(idVehicle.ToString()) == DBClassificationConstantes.Vehicles.names.tv
                    && webSession.CustomerLogin.GetFlag(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG) == null)
                    sql.Append(" and " + DATA_TABLE_PREFIXE + ".id_category != " + DBConstantes.Category.ID_DIGITAL_TV + "  ");

                #endregion

                // Order by
                sql.Append(orderby);

            }
            catch (System.Exception err) {
                throw (new PortofolioDetailMediaDataAccessException("Impossible de construire la requête", err));
            }

            #region Execution de la requête
            try {
                return webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err) {
                throw (new PortofolioDetailMediaDataAccessException("Impossible de charger pour le détail media: " + sql, err));
            }
            #endregion

        }
        #endregion

        #region Méthodes Internes
        /// <summary>
		/// Champs pour le detail support
		/// </summary>
		/// <param name="idVehicle">Identifiant du media (vehicle)</param>
		/// <returns>SQL</returns>
		public static string GetFieldsDetailMediaForTvRadio(DBClassificationConstantes.Vehicles.names idVehicle){		
			string sql="";
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.radio:
					sql+=" sum(insertion) as insertion ";
					sql+=",commercial_break as code_ecran";
					sql+=",sum(expenditure_euro) value ";
					sql+=" , date_media_num  ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					sql+=" sum(insertion) as insertion ";
					sql+=",id_commercial_break as code_ecran";
					sql+=",sum(expenditure_euro) value ";
					sql+=",date_media_num  ";
					return sql;
				default:
					throw new Exceptions.PortofolioDetailMediaDataAccessException("getDetailMedia(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		/// <summary>
		/// Group by
		/// </summary>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <returns>string conteant le group by</returns>
		public static string GetGroupByDetailMediaForTvRadio(DBClassificationConstantes.Vehicles.names idVehicle){
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.radio:					
					return "group by date_media_num ,commercial_break order by commercial_break";
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					return "group by date_media_num ,id_commercial_break order by id_commercial_break";
				default:
					throw new Exceptions.PortofolioDetailMediaDataAccessException("getDetailMedia(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		/// <summary>
		/// Génère le order by pour le détail portefeuille
		/// </summary>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="allPeriod">vrai si le détail des insertions concerne toute la période sélectionnée</param>
		/// <returns>SQL</returns>
		public static string GetOrderByDetailMedia(DBClassificationConstantes.Vehicles.names idVehicle,bool allPeriod,bool generic){
			string sql="";
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
//					if(allPeriod) sql+=" order by wp.date_media_num,wp.Id_type_page, ChampPage";
//					else
//					sql+=" order by wp.Id_type_page, ChampPage";	
                    if (allPeriod) sql += " order by wp.date_media_num,wp.Id_type_page, " + (generic ? "media_paging," : "ChampPage,wp.") + "id_product,wp.id_advertisement";
					else
                        sql += " order by wp.Id_type_page, " + (generic ? "media_paging," : "ChampPage,.wp") + "id_product,wp.id_advertisement";	
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					if(allPeriod)sql+="order by wp.date_media_num,wp.id_top_diffusion";
					else sql+="order by wp.id_top_diffusion";
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:								
					// Top diffusion
					if(allPeriod)
					sql+="order by wp.date_media_num,wp.top_diffusion ";
					else sql+="order by wp.top_diffusion ";
					return sql;	
				case DBClassificationConstantes.Vehicles.names.others:					
					//Règle : ordre par date, code écran pour le média Autres
					if(allPeriod)
						sql+="order by wp.date_media_num,wp.id_commercial_break ";
					else sql+="order by wp.id_commercial_break";
					return sql;			
				default:
					throw new Exceptions.PortofolioDetailMediaDataAccessException("getDetailMedia(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}		
		}

		/// <summary>
		/// Champs
		/// </summary>
		/// <param name="idVehicle">identifiant vehicle</param>
        /// <param name="mediaAgencyYear">Media Agency year</param>
		/// <returns>SQL</returns>
		public static string GetFieldsDetailMedia(DBClassificationConstantes.Vehicles.names idVehicle,string mediaAgencyYear){		
			string sql="";
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					sql+="  distinct advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					sql+=" ,area_page";
					sql+=" ,area_mmc";			  
					sql+=" ,format";
					sql+=" , wp.expenditure_euro";
					sql+=" ,wp.date_media_num";
					sql+=" ,wp.media_paging";	
					sql+=" ,rank_sector";
					sql+=" ,rank_group_";
					sql+=" ,rank_media";
					sql+=" ,color";
					sql+=" ,id_type_page";
					sql+=" ,location";
					sql+=" ,wp.id_advertisement";
					sql+=" ,wp.visual ";
					sql+=" , am.disponibility_visual";
					sql+=" ,wp.date_media_num ";
					if(mediaAgencyYear.Length>0)sql+=" , advertising_agency";
					sql+=" , media ";
                    sql += " ,LPAD(RTRIM(wp.Media_paging,' '),10,'0') as ChampPage ";
					sql+=" ,wp.date_cover_num ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					sql+="  distinct advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					sql+=" ,wp.duration ";
					sql+=", wp.duration_commercial_break";
					sql+=", wp.number_spot_com_break";
					sql+=", wp.rank_wap";
					sql+=", wp.duration_com_break_wap";
					sql+=", wp.number_spot_com_break_wap";
					sql+=", wp.expenditure_euro";
					sql+=", wp.id_cobranding_advertiser";
					sql+=",wp.id_top_diffusion as top_diffusion";
					sql+=",wp.commercial_break  ecran";
					sql+=",wp.expenditure_30s_euro";
					sql+=",wp.rank ";
					sql+=", wp.date_media_num as dateOfWeek "; 
					sql+=", wp.associated_file as fileData ";
					if(mediaAgencyYear.Length>0)sql+=" , advertising_agency";
					sql+=" , media ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					sql+="  distinct advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					// Top diffusion
					sql+=",wp.top_diffusion as top_diffusion";
					// durée
					sql+=" ,wp.duration ";
					// Code ecran
					sql+=",wp.id_commercial_break  ecran";
					// Position
					sql+=", wp.id_rank as rank";
					// durée écran
					sql+=", wp.duration_commercial_break";
					// Spots ecran
					sql+=",wp.number_message_commercial_brea ";
					// prix
					sql+=", wp.expenditure_euro";
					// prix du 30 sec
					sql+=",wp.expenditure_30s_euro";
					// Date
					sql+=", wp.date_media_num as dateOfWeek "; 
					// Fichier associé
					sql+=", wp.associated_file as fileData ";
					// Agence média
					if(mediaAgencyYear.Length>0)sql+=" , advertising_agency";
					sql+=" , media ";
					return sql;
				default:
					throw new Exceptions.PortofolioDetailMediaDataAccessException("getDetailMedia(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}	
		}


		/// <summary>
		/// Récupère la liste produit de référence
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <returns>la liste produit de référence</returns>
		internal static string GetProductData(WebSession webSession){
			bool premier;
			string list;
			string sql="";

			#region Ancienne version Sélection de Produits
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

			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql = webSession.PrincipalProductUniverses[0].GetSqlConditions("wp", true);

			return sql;
		}


		private static string GetDetailMediaColumnToOrder(DBClassificationConstantes.Vehicles.names idVehicle){
			string sql="";		
			switch(idVehicle){
				case DBClassificationConstantes.Vehicles.names.press:
				case DBClassificationConstantes.Vehicles.names.internationalPress:
					sql+="advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					sql+=" ,area_page";
					sql+=" ,area_mmc";			  
					sql+=" ,format";
					sql+=" , wp.expenditure_euro";
					sql+=" ,wp.date_media_num";
					sql+=" ,wp.media_paging";	
					sql+=" ,rank_sector";
					sql+=" ,rank_group_";
					sql+=" ,rank_media";
					sql+=" ,color";
					sql+=" ,id_type_page";
					sql+=" ,location";
					sql+=" ,wp.id_advertisement";
					sql+=" ,wp.visual ";
					sql+=" , am.disponibility_visual";
					sql+=" ,wp.date_media_num ";
					sql+=" , advertising_agency";
					sql+=" , media ";
					sql+=" ,LPAD(RTRIM(wp.Media_paging,' '),10,'0') as ChampPage ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.radio:
					sql+="   advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					sql+=" ,wp.duration ";
					sql+=", wp.duration_commercial_break";
					sql+=", wp.number_spot_com_break";
					sql+=", wp.rank_wap";
					sql+=", wp.duration_com_break_wap";
					sql+=", wp.number_spot_com_break_wap";
					sql+=", wp.expenditure_euro";
					sql+=", wp.id_cobranding_advertiser";
					sql+=",wp.id_top_diffusion as top_diffusion";
					sql+=",wp.commercial_break  ecran";
					sql+=",wp.expenditure_30s_euro";
					sql+=",wp.rank ";
					sql+=", wp.date_media_num as dateOfWeek "; 
					sql+=", wp.associated_file as fileData ";
					sql+=" , advertising_agency";
					sql+=" , media ";
					return sql;
				case DBClassificationConstantes.Vehicles.names.tv:
				case DBClassificationConstantes.Vehicles.names.others:
					sql+="advertiser";
					sql+=",product,sector,group_ ";
					sql+=", wp.id_product";
					// Top diffusion
					sql+=",wp.top_diffusion as top_diffusion";
					// durée
					sql+=" ,wp.duration ";
					// Code ecran
					sql+=",wp.id_commercial_break  ecran";
					// Position
					sql+=", wp.id_rank as rank";
					// durée écran
					sql+=", wp.duration_commercial_break";
					// Spots ecran
					sql+=",wp.number_message_commercial_brea ";
					// prix
					sql+=", wp.expenditure_euro";
					// prix du 30 sec
					sql+=",wp.expenditure_30s_euro";
					// Date
					sql+=", wp.date_media_num as dateOfWeek "; 
					// Fichier associé
					sql+=", wp.associated_file as fileData ";
					// Agence média
					sql+=" , advertising_agency";
					sql+=" , media ";
					return sql;
				default:
					throw new Exceptions.PortofolioDataAccessException("getDetailMedia(DBClassificationConstantes.Vehicles.value idMedia)-->Le cas de ce média n'est pas gérer. Pas de table correspondante.");
			}
		}

		private static string GetDetailPressColumnToOrder(int columnIndex){
			switch(columnIndex){
				case 0 :
					return "advertiser";
				case 1 :
					return "product";
				case 2 :
					return "sector";
				case 4 :
					return "group_";
//				case DBClassificationConstantes.Vehicles.names.internationalPress:
//					
//					sql+=" ,area_page";
//					sql+=" ,area_mmc";			  
//					sql+=" ,format";
//					sql+=" , wp.expenditure_euro";
//					sql+=" ,wp.date_media_num";
//					sql+=" ,wp.media_paging";	
//					sql+=" ,rank_sector";
//					sql+=" ,rank_group_";
//					sql+=" ,rank_media";
//					sql+=" ,color";
//					sql+=" ,id_type_page";
//					sql+=" ,location";
//					sql+=" ,wp.id_advertisement";
//					sql+=" ,wp.visual ";
//					sql+=" , am.disponibility_visual";
//					sql+=" ,wp.date_media_num ";
//					sql+=" , advertising_agency";
//					sql+=" , media ";
//					sql+=" ,LPAD(RTRIM(wp.Media_paging,' '),10,'0') as ChampPage ";
//					return sql;
				default : return "advertiser";
			}
		}

        /// <summary>
        /// Determine if a media belongs to a categroy
        /// </summary>
        /// <returns></returns>
        public static bool IsMediaBelongToCategory(WebSession webSession, long idMedia, long idCategory, int siteLanguage)
        {
            StringBuilder sql = new StringBuilder(1000);
            DataTable dt = null;

            sql.Append(" select  " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media," + DBConstantes.Tables.MEDIA_PREFIXE + ".media," + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category ");
            sql.Append(" from  " + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".CATEGORY  " + DBConstantes.Tables.CATEGORY_PREFIXE + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".MEDIA " + DBConstantes.Tables.MEDIA_PREFIXE);
            sql.Append(" " + "," + DBConstantes.Schema.ADEXPRESS_SCHEMA + ".BASIC_MEDIA " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + " ");
            sql.Append(" where ");
            sql.Append(" " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_basic_media =" + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_basic_media ");
            sql.Append(" and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_category ");
            sql.Append(" and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_category=" + idCategory);
            sql.Append(" and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".id_language=" + siteLanguage);
            sql.Append(" and " + DBConstantes.Tables.CATEGORY_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language=" + siteLanguage);
            sql.Append(" and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".id_language=" + siteLanguage);
            sql.Append(" and " + DBConstantes.Tables.BASIC_MEDIA_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media = " + idMedia);
            sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation<" + TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED + " ");
            sql.Append("  order by media ");

            #region Execution de la requête
            try
            {
                dt = webSession.Source.Fill(sql.ToString()).Tables[0];

                if (dt != null && !dt.Equals(System.DBNull.Value) && dt.Rows.Count > 0) return true;
                else return false;
            }
            catch (System.Exception err)
            {
                throw (new WebExceptions.PortofolioDataAccessException("Impossible de déterminer si le média appartient à la categorie: ", err));
            }
            #endregion
        }
		#endregion
	}
}

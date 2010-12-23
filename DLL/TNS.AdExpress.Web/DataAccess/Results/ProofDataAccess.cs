
#region Information
/*
Author : D. Mussuma
Creation : 10/01/2006
Modifications :
*/
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
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Units;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;

#endregion

namespace TNS.AdExpress.Web.DataAccess.Results
{
	/// <summary>
	/// Classe pour les requêtes nécessaires au module des justificatifs
	/// </summary>
	public class ProofDataAccess
	{
		#region GetData
		/// <summary>
		/// Obtient la liste des insertions de l'univers sélectionné
		/// </summary>
		/// <param name="webSession"> Session client</param>
		/// <param name="dateBegin">Date de début</param>
		/// <param name="dateEnd">Date de fin</param>
		/// <returns>Tableau de données des insertions</returns>
		public static DataSet GetData(WebSession webSession , string dateBegin, string dateEnd){
			
			#region variables
			StringBuilder sql = new StringBuilder(5000);			
			
			
			#endregion

			#region Construction de la requête
			

			// Sélection de la nomenclature
			sql.Append(" select ");
			GetSqlFields(sql);
			

			// Tables
			sql.Append(" from ");
			GetSqlTables(sql);
						
			
			//Conditions 
			sql.Append(" where  ");
			GetSqlJoins(sql,webSession);
						
			// Période
			sql.Append(" and " + DbTables.DATA_PRESS_PREFIXE + ".date_media_num>="+dateBegin);
            sql.Append(" and " + DbTables.DATA_PRESS_PREFIXE + ".date_media_num<=" + dateEnd);

			// Affiner les version
			string slogans = webSession.SloganIdList;			
			if(slogans.Length>0){
				sql.Append(" and "+DbTables.DATA_PRESS_PREFIXE+".id_slogan in("+slogans+") ");
			}
			
			
			// Gestion des sélections et des droits

			#region Nomenclature Produit (droits)

			//Droits en accès
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession,DbTables.DATA_PRESS_PREFIXE,true));
			
			#endregion			

			#region Nomenclature Annonceurs (droits(Ne pas faire pour l'instant) et sélection) 
			

			//Sélection produit
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.DATA_PRESS_PREFIXE, true));


			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.DATA_PRESS_PREFIXE,true));
			#endregion

			//#region Sélection
			//sql.Append(" and (( " + DbTables.DATA_PRESS_PREFIXE + ".id_vehicle = " + DBClassificationConstantes.Vehicles.names.press.GetHashCode() + ")) ");
			//#endregion

			//Media Univers
			sql.Append(GetMediaUniverse(webSession));

			#endregion

			// Group by
			sql.Append(" Group by ");
			GetSqlGroupByFields(sql);

			// Ordre
			sql.Append(" Order by ");
			GetSqlOrderByFields(sql);

			#endregion

			#region Execution de la requête
			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch(System.Exception err){
				throw(new ProofDataAccessException ("Impossible de charger les données des justificatifs presse "+sql.ToString(),err));
			}

			#endregion
		}
		#endregion

		#region GetProofFileData
		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère les données de la fiche justificative
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="date">Date</param>
		/// <param name="page">Numéro de la page</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetProofFileData( WebSession webSession, string idMedia, string idProduct, string dateParution, string page){ //IDataSource dataSource
			
			StringBuilder sql = new StringBuilder(3000);
			Table pressTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.dataPress);
			Table mediaTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.media);
			Table advertiserTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.advertiser);
			Table productTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.product);
			Table groupTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.group);
			Table colorTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.color);
			Table formatTable = WebApplicationParameters.DataBaseDescription.GetTable(TNS.AdExpress.Domain.DataBaseDescription.TableIds.format);			
			
			string DATA_PRESS_PREFIXE = pressTable.Prefix;

			#region Select
			sql.Append("select "+ DATA_PRESS_PREFIXE +".id_media, "+ mediaTable.Prefix +".media, ");
            sql.Append(DATA_PRESS_PREFIXE + ".date_media_num, " + DATA_PRESS_PREFIXE + ".date_cover_num, ");
			sql.Append(advertiserTable.Prefix +".advertiser, ");
			sql.Append(productTable.Prefix +".product, "+ groupTable.Prefix +".group_, ");
			sql.AppendFormat("{0}.media_paging, {0}.{1} as {2}, "
                , DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());
			sql.AppendFormat("{0}.{1} as {2}, {3}.color, "
                , DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.mmPerCol].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.mmPerCol].Id.ToString()
                , colorTable.Prefix);
			sql.Append(formatTable.Prefix +".format, "+ DATA_PRESS_PREFIXE +".rank_sector, ");
			sql.Append(DATA_PRESS_PREFIXE +".rank_group_, "+ DATA_PRESS_PREFIXE +".rank_media, ");
			sql.Append(DATA_PRESS_PREFIXE +".id_advertisement, ");
			sql.AppendFormat("{0}.visual, {0}.{1} as {2}"
                , DATA_PRESS_PREFIXE
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
			#endregion

			#region From
			sql.Append(" from " + pressTable.SqlWithPrefix + ",");
			sql.Append(mediaTable.SqlWithPrefix+ ",");
			sql.Append(advertiserTable.SqlWithPrefix + ",");
			sql.Append(productTable.SqlWithPrefix+ ",");
			sql.Append(groupTable.SqlWithPrefix + ",");
			sql.Append(colorTable.SqlWithPrefix + ",");
			sql.Append(formatTable.SqlWithPrefix + " ");
			#endregion

			#region Where
			sql.Append(" where "+ DATA_PRESS_PREFIXE +".id_media = "+ idMedia);
			sql.Append(" and "+ DATA_PRESS_PREFIXE +".id_product = "+ idProduct);

            sql.Append(" and " + DATA_PRESS_PREFIXE + ".date_media_num = " + dateParution);			
			
			sql.Append(" and rtrim(ltrim("+ DATA_PRESS_PREFIXE +".media_paging)) like '"+ page +"'");
			sql.Append(" and " + mediaTable.Prefix + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and "+ advertiserTable.Prefix +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ productTable.Prefix +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ groupTable.Prefix+".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ colorTable.Prefix +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+formatTable.Prefix +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_media = " + mediaTable.Prefix + ".id_media ");
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_advertiser = " + advertiserTable.Prefix + ".id_advertiser ");
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_product = " + productTable.Prefix + ".id_product ");
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_group_ = " + groupTable.Prefix + ".id_group_ ");
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_color = " + colorTable.Prefix + ".id_color ");
			sql.Append(" and " + DATA_PRESS_PREFIXE + ".id_format = " + formatTable.Prefix + ".id_format ");
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DATA_PRESS_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DATA_PRESS_PREFIXE, true));

			#endregion

			try{
				return webSession.Source.Fill(sql.ToString());
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetProofFileData : An error occured when accessing to the database ",exc);
			}
		}
		#endregion

		#region GetDataLocation
		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère le descriptif pour la fiche justificative
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="idAdvertisement">Identifiant du paramètre</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="date">Date</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetDataLocation(WebSession webSession, string idAdvertisement, string idMedia, string date){

			StringBuilder sql = new StringBuilder(500);

			sql.Append("select "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_location, "+ DBConstantes.Tables.LOCATION_PREFIXE +".location ");
			sql.Append("from "+ DBConstantes.Schema.ADEXPRESS_SCHEMA +"."+ DBConstantes.Tables.DATA_LOCATION +" "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA +"."+ DBConstantes.Tables.LOCATION +" "+ DBConstantes.Tables.LOCATION_PREFIXE +" ");
			sql.Append("where "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_media = "+ idMedia +" ");
			sql.Append("and "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_advertisement = "+ idAdvertisement +" ");
			sql.Append("and "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".date_media_num = "+ date +" ");
			//sql.Append("and "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_language_data_i = "+ webSession.DataLanguage +" ");
			sql.Append("and "+ DBConstantes.Tables.LOCATION_PREFIXE +".id_language = "+ webSession.DataLanguage +" ");
			sql.Append("and "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_location = "+ DBConstantes.Tables.LOCATION_PREFIXE +".id_location ");

			try{
				DataSet ds = webSession.Source.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetDataLocation:An error occured when accessing to the database ",exc);
			}
		}
		#endregion

		#region Nombre de pages
		/// <summary>
		/// Récupère le nombre de page d'un magazine
		/// </summary>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="date">Date du media</param>
		/// <returns>DataSet contenant les données</returns>
		public static int GetDataPages(WebSession webSession, string idMedia, string date){

			StringBuilder sql = new StringBuilder(500);
			int nbPage=0;

			sql.Append("select id_media, number_page_media  from "+ DBConstantes.Schema.ADEXPRESS_SCHEMA +"."+ DBConstantes.Tables.ALARM_MEDIA +" ");
			sql.Append("where id_media = "+ idMedia +" and id_language_i = "+ webSession.DataLanguage +" and date_alarm = "+ date +" ");

			try{
				DataSet ds = webSession.Source.Fill(sql.ToString());
				if (ds.Tables[0].Rows.Count>0) 
					nbPage = int.Parse(ds.Tables[0].Rows[0].ItemArray.GetValue(1).ToString());
				return nbPage;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetDataPages:An error occured when accessing to the database ",exc);
			}
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Obtient les champs de la requêtes
		/// </summary>
		/// <param name="sql">Chaine de caractères de la requete</param>
		private static void GetSqlFields(StringBuilder sql){

			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media_seller,"+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media,"+DBConstantes.Tables.MEDIA_PREFIXE+".media,");
			sql.Append(DBConstantes.Tables.FORMAT_PREFIXE + ".id_format,"+DBConstantes.Tables.FORMAT_PREFIXE+".format,");
			sql.Append(DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_advertisement,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_cover_num,");
			sql.AppendFormat("{0}.{1} as {2},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].Id.ToString());
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
			sql.AppendFormat("{0}.{1} as {2},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString());
            sql.AppendFormat("{0}.{1} as {2} "
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString());
		}
		
		/// <summary>
		/// Obtient les tables de la requête
		/// </summary>
		/// <param name="sql">Chaine de caractères de la requete</param>
		private static void GetSqlTables(StringBuilder sql){
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DbTables.DATA_PRESS+" "+DbTables.DATA_PRESS_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MEDIA_SELLER + " " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MEDIA + " " + DBConstantes.Tables.MEDIA_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + " " + DBConstantes.Tables.FORMAT_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.PRODUCT + " " + DBConstantes.Tables.PRODUCT_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_LOCATION + " " + DBConstantes.Tables.DATA_LOCATION_PREFIXE+ ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.LOCATION + " " + DBConstantes.Tables.LOCATION_PREFIXE);
		
		}

		/// <summary>
		/// Obtient les jointures de la requête
		/// </summary>
		/// <param name="sql">Chaine de caractères de la requete</param>
		/// <param name="webSession">Session du client</param>
		private static void GetSqlJoins(StringBuilder sql,WebSession webSession){
			
			sql.Append("  " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE + ".id_media_seller = " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media_seller" );
			sql.Append(" and " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media = " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media" );
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".id_format (+)= " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_format" );
			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".id_language (+)= " + webSession.DataLanguage);
			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".activation (+)< " + DBConstantes.ActivationValues.UNACTIVATED);
		
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product = " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_product" );
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language = " + webSession.DataLanguage);
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBConstantes.Tables.LOCATION_PREFIXE + ".id_location (+)= " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".id_location" );
			sql.Append(" and " + DBConstantes.Tables.LOCATION_PREFIXE + ".id_language (+)= " + webSession.DataLanguage);
			sql.Append(" and " + DBConstantes.Tables.LOCATION_PREFIXE + ".activation (+)< " + DBConstantes.ActivationValues.UNACTIVATED);
			sql.Append(" and " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".id_media (+)= " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media ");
			sql.Append(" and " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".id_advertisement (+)= " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_advertisement ");
			sql.Append(" and " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".date_media_num (+)= " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num ");
			sql.Append(" and " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".activation (+)< " + DBConstantes.ActivationValues.UNACTIVATED);
		}

		/// <summary>
		/// Obtient les regroupement de la requête
		/// </summary>
		/// <param name="sql">Chaine de caractères de la requete</param>
		private static void GetSqlGroupByFields(StringBuilder sql){

			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media_seller,"+DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media,"+DBConstantes.Tables.MEDIA_PREFIXE+".media,");
			sql.Append(DBConstantes.Tables.FORMAT_PREFIXE + ".id_format,"+DBConstantes.Tables.FORMAT_PREFIXE+".format,");
			sql.Append(DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product,"+DBConstantes.Tables.PRODUCT_PREFIXE+".product,");		
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_advertisement,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_cover_num,");
			sql.AppendFormat("{0}.{1},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField);
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
            sql.AppendFormat("{0}.{1},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField);
            sql.AppendFormat("{0}.{1} "
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField);
		}

		/// <summary>
		/// Obtient les tri de la requête
		/// </summary>
		/// <param name="sql">Chaine de caractères de la requete</param>
		private static void GetSqlOrderByFields(StringBuilder sql){

			sql.Append(DBConstantes.Tables.MEDIA_SELLER_PREFIXE+".media_seller,"+DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media_seller,");
			sql.Append(DBConstantes.Tables.MEDIA_PREFIXE+".media,"+DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media,");					
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num,");
            sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_cover_num,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_advertisement,");
			sql.Append(DBConstantes.Tables.PRODUCT_PREFIXE+".product,"+DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product,");		
			sql.Append(DBConstantes.Tables.FORMAT_PREFIXE+".format,"+DBConstantes.Tables.FORMAT_PREFIXE + ".id_format,");
            sql.AppendFormat("{0}.{1},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[UnitsInformation.DefaultCurrency].DatabaseField);
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
            sql.AppendFormat("{0}.{1},"
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].DatabaseField);
            sql.AppendFormat("{0}.{1} "
                , DBConstantes.Tables.DATA_PRESS_PREFIXE
                , UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].DatabaseField);
		}

		/// <summary>
		/// Get media Universe
		/// </summary>
		/// <param name="webSession">Web Session</param>
		/// <returns>string sql</returns>
		protected static string GetMediaUniverse(WebSession webSession) {
			string sql = "";
				WebNavigation.Module module = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
				sql = module.GetResultPageInformation(webSession.CurrentTab).GetAllowedMediaUniverseSql(DBConstantes.Tables.DATA_PRESS_PREFIXE, true);
			return sql;
		}
		#endregion
	}
}

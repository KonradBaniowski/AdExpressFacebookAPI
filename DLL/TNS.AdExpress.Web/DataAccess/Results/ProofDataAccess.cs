
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
		
			#region Ancienne version Sélection produit
			//sql.Append(WebFunctions.SQLGenerator.GetAnalyseCustomerProductSelection(webSession,DbTables.DATA_PRESS_PREFIXE,DbTables.DATA_PRESS_PREFIXE,DbTables.DATA_PRESS_PREFIXE,true));			
			#endregion

			//Sélection produit
			if (webSession.PrincipalProductUniverses != null && webSession.PrincipalProductUniverses.Count > 0)
				sql.Append(webSession.PrincipalProductUniverses[0].GetSqlConditions(DbTables.DATA_PRESS_PREFIXE, true));


			#endregion

			#region Nomenclature Media (droits et sélection)

			#region Droits
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession,DbTables.DATA_PRESS_PREFIXE,true));
			#endregion

            #region Sélection
            sql.Append(" and (( " + DbTables.DATA_PRESS_PREFIXE + ".id_vehicle = " + DBClassificationConstantes.Vehicles.names.press.GetHashCode() + ")) ");
            #endregion

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

			#region Select
			sql.Append("select "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_media, "+ DBConstantes.Tables.MEDIA_PREFIXE +".media, ");
            sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num, " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_cover_num, ");
			sql.Append(DBConstantes.Tables.ADVERTISER_PREFIXE +".advertiser, ");
			sql.Append(DBConstantes.Tables.PRODUCT_PREFIXE +".product, "+ DBConstantes.Tables.GROUP_PREFIXE +".group_, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE +".media_paging, "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".area_page, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE +".area_mmc, "+ DBConstantes.Tables.COLOR_PREFIXE +".color, ");
			sql.Append(DBConstantes.Tables.FORMAT_PREFIXE +".format, "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".rank_sector, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE +".rank_group_, "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".rank_media, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_advertisement, ");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE +".visual, "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".expenditure_euro ");
			#endregion

			#region From
			sql.Append(" from " + DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.DATA_PRESS + " " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.MEDIA + " " + DBConstantes.Tables.MEDIA_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.ADVERTISER + " " + DBConstantes.Tables.ADVERTISER_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.PRODUCT + " " + DBConstantes.Tables.PRODUCT_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.GROUP_ + " " + DBConstantes.Tables.GROUP_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.COLOR + " " + DBConstantes.Tables.COLOR_PREFIXE + ",");
			sql.Append(DBConstantes.Schema.ADEXPRESS_SCHEMA + "." + DBConstantes.Tables.FORMAT + " " + DBConstantes.Tables.FORMAT_PREFIXE + " ");
			#endregion

			#region Where
			sql.Append(" where "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_media = "+ idMedia);
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_product = "+ idProduct);

            sql.Append(" and " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".date_media_num = " + dateParution);			
			
			sql.Append(" and rtrim(ltrim("+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".media_paging)) like '"+ page +"'");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_language_data_i = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.MEDIA_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.ADVERTISER_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.PRODUCT_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.GROUP_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.COLOR_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.FORMAT_PREFIXE +".id_language = "+ webSession.SiteLanguage);
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_media = "+ DBConstantes.Tables.MEDIA_PREFIXE +".id_media ");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_advertiser = "+ DBConstantes.Tables.ADVERTISER_PREFIXE +".id_advertiser ");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_product = "+ DBConstantes.Tables.PRODUCT_PREFIXE +".id_product ");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_group_ = "+ DBConstantes.Tables.GROUP_PREFIXE +".id_group_ ");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_color = "+ DBConstantes.Tables.COLOR_PREFIXE +".id_color ");
			sql.Append(" and "+ DBConstantes.Tables.DATA_PRESS_PREFIXE +".id_format = "+ DBConstantes.Tables.FORMAT_PREFIXE +".id_format ");
			//media rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBConstantes.Tables.DATA_PRESS_PREFIXE, true));
			//product rights
			sql.Append(WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(webSession, DBConstantes.Tables.DATA_PRESS_PREFIXE, true));

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
			sql.Append("and "+ DBConstantes.Tables.DATA_LOCATION_PREFIXE +".id_language_data_i = "+ webSession.SiteLanguage +" ");
			sql.Append("and "+ DBConstantes.Tables.LOCATION_PREFIXE +".id_language = "+ webSession.SiteLanguage +" ");
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
			sql.Append("where id_media = "+ idMedia +" and id_language_i = "+ webSession.SiteLanguage +" and date_alarm = "+ date +" ");

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
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".expenditure_euro as euro,");			
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".area_page as pages,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".insertion");
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
			sql.Append(" and " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE + ".id_language = " + webSession.SiteLanguage);
			sql.Append(" and " + DBConstantes.Tables.MEDIA_SELLER_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);
			
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_media = " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_media" );
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".id_language = " + webSession.SiteLanguage);
			sql.Append(" and " + DBConstantes.Tables.MEDIA_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".id_format (+)= " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_format" );
			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".id_language (+)= " + webSession.SiteLanguage);
			sql.Append(" and " + DBConstantes.Tables.FORMAT_PREFIXE + ".activation (+)< " + DBConstantes.ActivationValues.UNACTIVATED);
		
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_product = " + DBConstantes.Tables.DATA_PRESS_PREFIXE + ".id_product" );
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".id_language = " + webSession.SiteLanguage);
			sql.Append(" and " + DBConstantes.Tables.PRODUCT_PREFIXE + ".activation < " + DBConstantes.ActivationValues.UNACTIVATED);

			sql.Append(" and " + DBConstantes.Tables.LOCATION_PREFIXE + ".id_location (+)= " + DBConstantes.Tables.DATA_LOCATION_PREFIXE + ".id_location" );
			sql.Append(" and " + DBConstantes.Tables.LOCATION_PREFIXE + ".id_language (+)= " + webSession.SiteLanguage);
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
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".expenditure_euro,");			
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".area_page,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".insertion");
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
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE + ".expenditure_euro,");			
			sql.Append(DBConstantes.Tables.LOCATION_PREFIXE + ".location,");			
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".media_paging,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".area_page,");
			sql.Append(DBConstantes.Tables.DATA_PRESS_PREFIXE+ ".insertion");
		}
		#endregion
	}
}

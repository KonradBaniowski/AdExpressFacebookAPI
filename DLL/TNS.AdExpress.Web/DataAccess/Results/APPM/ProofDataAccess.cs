#region Information
/*
Author : B.Masson
Creation : 25/08/2005
Last Modifications : 
	26/08/2005 par B.Masson
*/
#endregion

using System;
using System.Data;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;

using DBCst = TNS.AdExpress.Constantes.DB;
using Cst = TNS.AdExpress.Constantes;

namespace TNS.AdExpress.Web.DataAccess.Results.APPM{
	/// <summary>
	/// Classe pour les requêtes nécessaires à la fiche justificative
	/// </summary>
	public class ProofDataAccess{
		
		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère les données de la fiche justificative
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="date">Date</param>
		/// <param name="page">Numéro de la page</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetData(IDataSource dataSource, WebSession webSession, Int64 idMedia, Int64 idProduct, int date, string page){
			
			StringBuilder sql = new StringBuilder(3000);

			#region Select
			sql.Append("select "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media, "+ DBCst.Tables.MEDIA_PREFIXE +".media, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_media_num, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_cover_num,"); 
			sql.Append(DBCst.Tables.ADVERTISER_PREFIXE +".advertiser, ");
			sql.Append(DBCst.Tables.PRODUCT_PREFIXE +".product, "+ DBCst.Tables.GROUP_PREFIXE +".group_, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".media_paging, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".area_page, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".area_mmc, "+ DBCst.Tables.COLOR_PREFIXE +".color, ");
			sql.Append(DBCst.Tables.FORMAT_PREFIXE +".format, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".rank_sector, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".rank_group_, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".rank_media, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_inset, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_advertisement, ");
			sql.Append(DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".visual, "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".expenditure_euro ");
			#endregion

			#region From
			sql.Append(" from " + DBCst.Schema.APPM_SCHEMA + "." + DBCst.Tables.DATA_PRESS_APPM + " " + DBCst.Tables.DATA_PRESS_APPM_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.MEDIA + " " + DBCst.Tables.MEDIA_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.ADVERTISER + " " + DBCst.Tables.ADVERTISER_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.PRODUCT + " " + DBCst.Tables.PRODUCT_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.GROUP_ + " " + DBCst.Tables.GROUP_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.COLOR + " " + DBCst.Tables.COLOR_PREFIXE + ",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA + "." + DBCst.Tables.FORMAT + " " + DBCst.Tables.FORMAT_PREFIXE + " ");
			#endregion

			#region Where
			sql.Append(" where "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media = "+ idMedia);
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_product = "+ idProduct);

			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_media_num = "+ date);
			//sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".date_parution_num = "+ date);			
			
			sql.Append(" and rtrim(ltrim("+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".media_paging)) like '"+ page +"'");
			//sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_language_data_i = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.MEDIA_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.ADVERTISER_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.PRODUCT_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.GROUP_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.COLOR_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.FORMAT_PREFIXE +".id_language = "+ webSession.DataLanguage);
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_media = "+ DBCst.Tables.MEDIA_PREFIXE +".id_media ");
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_advertiser = "+ DBCst.Tables.ADVERTISER_PREFIXE +".id_advertiser ");
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_product = "+ DBCst.Tables.PRODUCT_PREFIXE +".id_product ");
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_group_ = "+ DBCst.Tables.GROUP_PREFIXE +".id_group_ ");
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_color = "+ DBCst.Tables.COLOR_PREFIXE +".id_color ");
			sql.Append(" and "+ DBCst.Tables.DATA_PRESS_APPM_PREFIXE +".id_format = "+ DBCst.Tables.FORMAT_PREFIXE +".id_format ");
			//media rights
			sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));
			//product rights
			sql.Append(SQLGenerator.getAnalyseCustomerProductRight(webSession, DBCst.Tables.DATA_PRESS_APPM_PREFIXE, true));

			#endregion

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetData:An error occured when accessing to the database ",exc);
			}
		}

		#region Nombre de pages
		/// <summary>
		/// Récupère le nombre de page d'un magazine
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="date">Date du media</param>
		/// <returns>DataSet contenant les données</returns>
		public static int GetDataPages(IDataSource dataSource, WebSession webSession, Int64 idMedia, int date){

			StringBuilder sql = new StringBuilder(500);
			int nbPage=0;

			sql.Append("select id_media, number_page_media  from "+ DBCst.Schema.ADEXPRESS_SCHEMA +"."+ DBCst.Tables.ALARM_MEDIA +" ");
			sql.Append("where id_media = "+ idMedia +" and id_language_i = "+ webSession.DataLanguage +" and date_alarm = "+ date +" ");

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				if (ds.Tables[0].Rows.Count>0) 
					nbPage = int.Parse(ds.Tables[0].Rows[0].ItemArray.GetValue(1).ToString());
				return nbPage;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetData:An error occured when accessing to the database ",exc);
			}
		}
		#endregion

		#region Descriptif
		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère le descriptif ENCART pour la fiche justificative
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idInset">Identifiant du paramètre</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetDataLocation(IDataSource dataSource, WebSession webSession, Int64 idInset){

			StringBuilder sql = new StringBuilder(500);

			sql.Append("select id_inset,inset from "+ DBCst.Schema.ADEXPRESS_SCHEMA +"."+ DBCst.Tables.INSERT +" ");
			sql.Append("where id_inset = "+ idInset +" and id_language = "+ webSession.DataLanguage);

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetData:An error occured when accessing to the database ",exc);
			}
		}

		/// <summary>
		/// Méthode pour l'execution de la requête qui récupère le descriptif pour la fiche justificative
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idAdvertisement">Identifiant du paramètre</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="date">Date</param>
		/// <returns>DataSet contenant les données</returns>
		public static DataSet GetDataLocation(IDataSource dataSource, WebSession webSession, Int64 idAdvertisement, Int64 idMedia, int date){

			StringBuilder sql = new StringBuilder(500);

			sql.Append("select "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".id_location, "+ DBCst.Tables.LOCATION_PREFIXE +".location ");
			sql.Append("from "+ DBCst.Schema.ADEXPRESS_SCHEMA +"."+ DBCst.Tables.DATA_LOCATION +" "+ DBCst.Tables.DATA_LOCATION_PREFIXE +",");
			sql.Append(DBCst.Schema.ADEXPRESS_SCHEMA +"."+ DBCst.Tables.LOCATION +" "+ DBCst.Tables.LOCATION_PREFIXE +" ");
			sql.Append("where "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".id_media = "+ idMedia +" ");
			sql.Append("and "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".id_advertisement = "+ idAdvertisement +" ");
			sql.Append("and "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".date_media_num = "+ date +" ");
			//sql.Append("and "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".id_language_data_i = "+ webSession.DataLanguage +" ");
			sql.Append("and "+ DBCst.Tables.LOCATION_PREFIXE +".id_language = "+ webSession.DataLanguage +" ");
			sql.Append("and "+ DBCst.Tables.DATA_LOCATION_PREFIXE +".id_location = "+ DBCst.Tables.LOCATION_PREFIXE +".id_location ");

			try{
				DataSet ds = dataSource.Fill(sql.ToString());
				return ds;
			}
			catch( System.Exception exc){
				throw new ProofDataAccessException("GetData:An error occured when accessing to the database ",exc);
			}
		}
		#endregion

	}
}

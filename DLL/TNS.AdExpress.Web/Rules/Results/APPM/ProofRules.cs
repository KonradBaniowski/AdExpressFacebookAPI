#region Information
/*
Author ; B.Masson
Creation : 25/08/2005
Last Modification : 
	26/08/2005 par B.Masson
*/
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Exceptions;
using DataAccesFct = TNS.AdExpress.Web.DataAccess; 

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Classe des rules nécessaires à la fiche justificative
	/// </summary>
	public class ProofRules{
		
		/// <summary>
		/// Méthode pour le traitement des données d'une fiche justificative
		/// </summary>
		/// <param name="dataSource">DataSource</param>
		/// <param name="webSession">Session</param>
		/// <param name="idMedia">Identifiant du media</param>
		/// <param name="idProduct">Identifiant du produit</param>
		/// <param name="date">Date</param>
		/// <param name="page">Numéro de la page</param>
		/// <returns>DataTable contenant les données</returns>
		public static DataTable GetData(IDataSource dataSource, WebSession webSession, Int64 idMedia, Int64 idProduct, int date, string page){

			#region Variables
			DataTable data = null;
			DataTable dataLocation = null;
			DataTable dtResult = null;
			DataRow dr = null;
			string dateResult = "";
			int nbMediaPage = 0;
			#endregion

			try{

				#region Get Data
				data = DataAccesFct.Results.APPM.ProofDataAccess.GetData(dataSource, webSession , idMedia, idProduct, date, page).Tables[0];
				#endregion

				dtResult = new DataTable();
				dtResult.Columns.Add("idMedia", System.Type.GetType("System.String"));
				dtResult.Columns.Add("media", System.Type.GetType("System.String"));
				dtResult.Columns.Add("date", System.Type.GetType("System.DateTime"));
				dtResult.Columns.Add("dateCover", System.Type.GetType("System.DateTime"));
				dtResult.Columns.Add("advertiser", System.Type.GetType("System.String"));
				dtResult.Columns.Add("product", System.Type.GetType("System.String"));
				dtResult.Columns.Add("group_", System.Type.GetType("System.String"));
				dtResult.Columns.Add("media_paging", System.Type.GetType("System.String"));
				dtResult.Columns.Add("area_page", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("area_mmc", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("color", System.Type.GetType("System.String"));
				dtResult.Columns.Add("format", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_sector", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_group_", System.Type.GetType("System.String"));
				dtResult.Columns.Add("rank_media", System.Type.GetType("System.String"));
				dtResult.Columns.Add("id_inset", System.Type.GetType("System.String"));
				dtResult.Columns.Add("id_advertisement", System.Type.GetType("System.String"));
				dtResult.Columns.Add("visual", System.Type.GetType("System.String"));
				dtResult.Columns.Add("expenditure_euro", System.Type.GetType("System.Decimal"));
				dtResult.Columns.Add("location", System.Type.GetType("System.String"));
				dtResult.Columns.Add("number_page_media", System.Type.GetType("System.Decimal"));

				if (data.Rows.Count>0){
					foreach(DataRow currentRow in data.Rows){
						dr = dtResult.NewRow();
						dtResult.Rows.Add(dr);
						dr["idMedia"]			= currentRow["id_media"].ToString();
						dr["media"]				= currentRow["media"].ToString();
						dr["advertiser"]		= currentRow["advertiser"].ToString();
						dr["product"]			= currentRow["product"].ToString();
						dr["group_"]			= currentRow["group_"].ToString();
						dr["media_paging"]		= currentRow["media_paging"].ToString();
						dr["area_page"]			= Decimal.Parse(currentRow["area_page"].ToString()) / 1000;
						dr["area_mmc"]			= Decimal.Parse(currentRow["area_mmc"].ToString());
						dr["color"]				= currentRow["color"].ToString();
						dr["format"]			= currentRow["format"].ToString();
						dr["rank_sector"]		= currentRow["rank_sector"].ToString();
						dr["rank_group_"]		= currentRow["rank_group_"].ToString();
						dr["rank_media"]		= currentRow["rank_media"].ToString();
						dr["visual"]			= currentRow["visual"].ToString();
						dr["expenditure_euro"]	= Decimal.Parse(currentRow["expenditure_euro"].ToString());

						#region Date
						if (currentRow["date_cover_num"].ToString().Length > 0) {
							dateResult = currentRow["date_cover_num"].ToString();
							dr["dateCover"] = new DateTime(int.Parse(dateResult.Substring(0, 4)), int.Parse(dateResult.Substring(4, 2)), int.Parse(dateResult.Substring(6, 2)));
						}
						dateResult = currentRow["date_media_num"].ToString();
						dr["date"] = new DateTime(int.Parse(dateResult.Substring(0,4)), int.Parse(dateResult.Substring(4,2)), int.Parse(dateResult.Substring(6,2)));
						#endregion
						
						#region Récupération du descriptif
						dr["id_inset"] = currentRow["id_inset"].ToString();
						dr["id_advertisement"] = currentRow["id_advertisement"].ToString();
						if (currentRow["id_inset"].ToString().Length > 0){
							dataLocation = DataAccesFct.Results.APPM.ProofDataAccess.GetDataLocation(dataSource, webSession, Int64.Parse(currentRow["id_inset"].ToString())).Tables[0];
						}
						else{
							dataLocation = DataAccesFct.Results.APPM.ProofDataAccess.GetDataLocation(dataSource, webSession, Int64.Parse(currentRow["id_advertisement"].ToString()), idMedia, date).Tables[0];
						}
						string location="";
						foreach(DataRow row in dataLocation.Rows){
							location += row.ItemArray.GetValue(1).ToString()+"<br>";
						}
						dr["location"] = location;
						#endregion

						#region Récupération du nombre de pages du media
						//nbMediaPage = DataAccesFct.Results.APPM.ProofDataAccess.GetDataPages(dataSource, webSession, idMedia, date);
						nbMediaPage = DataAccesFct.Results.APPM.ProofDataAccess.GetDataPages(dataSource, webSession, idMedia, int.Parse(currentRow["date_media_num"].ToString()));
						dr["number_page_media"]	= decimal.Parse(nbMediaPage.ToString());
						#endregion

					}
				}

			}
			catch(System.Exception err){
				throw(new ProofRulesException("Error while formatting the data for APPM Proof ",err));
			}

			return dtResult;
		}

	}
}

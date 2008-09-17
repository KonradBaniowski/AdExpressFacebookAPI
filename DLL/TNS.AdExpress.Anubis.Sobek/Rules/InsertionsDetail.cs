#region Information
/*
Author : D. Mussuma, Y. Rkaina
Creation : 23/05/2006
Last Modifications : 
*/
#endregion

using System;
using System.Collections;
using System.Data;
using System.Text;

using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Anubis.Sobek.Rules
{
	/// <summary>
	/// Description résumée de InsertionsDetail.
	/// </summary>
	public class InsertionsDetail
	{
		/// <summary>
		/// Process data from database so as to get the desired information
		/// </summary>
		/// <remarks>Able to process one or more media</remarks>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idWave">Study wave</param>
		/// <returns>DataSet ready to be displayed</returns>
		public static DataTable GetData(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idWave){
			try{ 

				DataTable data = DataAccess.InsertionsDetail.GetData(dataSource, webSession , dateBegin, dateEnd, idBaseTarget,idWave);

			
				#region Variables
				Int64 idOldMedia = -1;
				Int64 idOldAdvertisement = -1;
				Int64 idOldDate = -1;
				DataRow nRow = null;
			
				#endregion

				DataTable dtResult = new DataTable();
				dtResult.Columns.Add("idMedia", System.Type.GetType("System.String"));
				dtResult.Columns.Add("media", System.Type.GetType("System.String"));
				dtResult.Columns.Add("idProduct", System.Type.GetType("System.String"));
				dtResult.Columns.Add("product", System.Type.GetType("System.String"));
				dtResult.Columns.Add("idAdvertiser", System.Type.GetType("System.String"));
				dtResult.Columns.Add("advertiser", System.Type.GetType("System.String"));
				dtResult.Columns.Add("date", System.Type.GetType("System.String"));
				dtResult.Columns.Add("dateParution", System.Type.GetType("System.String")); 
				dtResult.Columns.Add("format", System.Type.GetType("System.String"));
				dtResult.Columns.Add("location", System.Type.GetType("System.String"));
				dtResult.Columns.Add("budget"); //"System.Decimal" 
			
			


				if (data.Rows.Count>0){

					string date = "";

					#region en-tête
					nRow = dtResult.NewRow();
					dtResult.Rows.Add(nRow);
					nRow["idMedia"] = GestionWeb.GetWebWord(1915,webSession.SiteLanguage);
					nRow["media"] = GestionWeb.GetWebWord(971,webSession.SiteLanguage);
					nRow["idProduct"] = GestionWeb.GetWebWord(1914,webSession.SiteLanguage);
					nRow["product"] = GestionWeb.GetWebWord(858,webSession.SiteLanguage);
					nRow["idAdvertiser"] = GestionWeb.GetWebWord(1916,webSession.SiteLanguage);
					nRow["advertiser"] = GestionWeb.GetWebWord(857,webSession.SiteLanguage);
					nRow["dateParution"] = GestionWeb.GetWebWord(895,webSession.SiteLanguage);
					nRow["format"] = GestionWeb.GetWebWord(1420,webSession.SiteLanguage);
					nRow["location"] = GestionWeb.GetWebWord(1732,webSession.SiteLanguage);
					nRow["budget"] = GestionWeb.GetWebWord(1712,webSession.SiteLanguage);
					#endregion

					foreach(DataRow row in data.Rows){
							

						//Si nouvelle insertion
						if(int.Parse(row["id_advertisement"].ToString()) != idOldAdvertisement || int.Parse(row["date_media_num"].ToString()) != idOldDate || (int.Parse(row["id_media"].ToString()) != idOldMedia)){
							idOldAdvertisement = int.Parse(row["id_advertisement"].ToString());
							idOldDate = int.Parse(row["date_media_num"].ToString());
							idOldMedia = int.Parse(row["id_media"].ToString());
							nRow = dtResult.NewRow();
							dtResult.Rows.Add(nRow);
							nRow["idMedia"] = row["id_media"].ToString();
							nRow["media"] = row["media"].ToString();
							nRow["idProduct"] = row["id_product"].ToString();
							nRow["product"] = row["product"].ToString();
							nRow["idAdvertiser"] = row["id_advertiser"].ToString();
							nRow["advertiser"] = row["advertiser"].ToString();
							if (row["date_media_num"].ToString().Length > 0) {
								date = DateString.YYYYMMDDToDD_MM_YYYY(row["date_media_num"].ToString(), webSession.SiteLanguage);
								nRow["dateParution"] = date;
							}
							date = DateString.YYYYMMDDToDD_MM_YYYY(row["date_media_num"].ToString(),webSession.SiteLanguage);
							nRow["date"] = date;
							nRow["dateParution"] = date;
							nRow["format"] = row["format"].ToString();
							nRow["location"] = row["location"].ToString();
							nRow["budget"] = Decimal.Parse(row["euro"].ToString());						
						}
						else{
							nRow["location"] = nRow["location"].ToString() + " , " + row["location"].ToString();
						}
					}							
				}
				return dtResult;
			}
			catch( System.Exception exc){
				throw new Exceptions.InsertionsDetailException("GetData::An error occured when creatind  the DataTable result ",exc);
			}

		}

	}
}

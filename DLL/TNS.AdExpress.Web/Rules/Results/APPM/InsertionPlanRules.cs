#region Infomration
/*
Author ; G. RAGNEAU
Creation : 27/07/2005
Last Modification : 
	24/08/2005 par B.Masson > Ajout de la colonne "idProduct" dans le DataTable dtResult
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

using DataAccesFct = TNS.AdExpress.Web.DataAccess; 

namespace TNS.AdExpress.Web.Rules.Results.APPM {

	/// <summary>
	/// Process data for the APPM module, insertion popUp
	/// </summary>
	public class InsertionPlanRules {

		/// <summary>
		/// Process data from database so as to get the desired information
		/// </summary>
		/// <remarks>Able to process one or more media</remarks>
		/// <param name="dataSource">Data Source</param>
		/// <param name="webSession">User Session</param>
		/// <param name="dateBegin">Period beginning</param>
		/// <param name="dateEnd">Period end</param>
		/// <param name="idBaseTarget">Default target</param>
		/// <param name="idMedia">Media to detail</param>
		/// <param name="idWave">Study wave</param>
		/// <returns>DataSet ready to be displayed</returns>
		public static DataTable GetData(IDataSource dataSource, WebSession webSession , int dateBegin, int dateEnd, Int64 idBaseTarget, Int64 idMedia, Int64 idWave){


			DataTable data = DataAccesFct.Results.APPM.InsertPlanDataAccess.GetData(dataSource, webSession , dateBegin, dateEnd, idBaseTarget, idMedia, idWave).Tables[0];

			
			#region Variables
			Int64 idOldMedia = -1;
			Int64 idOldAdvertisement = -1;
			Int64 idOldDate = -1;
			Decimal mdBudg = 0;
			DataRow nRow = null;
			DataRow oldMediaRow = null;
			#endregion

			DataTable dtResult = new DataTable();
			dtResult.Columns.Add("rowType", System.Type.GetType("System.String"));
			dtResult.Columns.Add("idMedia", System.Type.GetType("System.String"));
			dtResult.Columns.Add("label", System.Type.GetType("System.String"));
			dtResult.Columns.Add("date", System.Type.GetType("System.DateTime"));
			dtResult.Columns.Add("dateCover", System.Type.GetType("System.DateTime"));
			dtResult.Columns.Add("format", System.Type.GetType("System.String"));
			dtResult.Columns.Add("location", System.Type.GetType("System.String"));
			dtResult.Columns.Add("budget", System.Type.GetType("System.Decimal"));
			dtResult.Columns.Add("PDM", System.Type.GetType("System.Decimal"));
			dtResult.Columns.Add("idProduct", System.Type.GetType("System.String"));
			dtResult.Columns.Add("mediaPaging", System.Type.GetType("System.String"));

			if (data.Rows.Count>0){

				string date = "";

				foreach(DataRow row in data.Rows){
		
					//new media ?
					if(int.Parse(row["id_media"].ToString()) != idOldMedia){
						idOldMedia = int.Parse(row["id_media"].ToString());
						oldMediaRow = nRow = dtResult.NewRow();
						dtResult.Rows.Add(nRow);
						nRow["rowType"] = Right.type.mediaAccess.ToString();
						nRow["idMedia"] = row["id_media"].ToString();
						nRow["label"] = row["media"].ToString();
						nRow["date"] = DBNull.Value;
						nRow["format"] = "";
						nRow["location"] = "";
						nRow["pdm"] = 100;
						nRow["budget"] = 0.0;
						nRow["idProduct"] = row["id_product"].ToString();
						nRow["mediaPaging"] = row["media_paging"].ToString();
					}

					//new insertion?
					if(int.Parse(row["id_advertisement"].ToString()) != idOldAdvertisement || int.Parse(row["date_media_num"].ToString()) != idOldDate){
						idOldAdvertisement = int.Parse(row["id_advertisement"].ToString());
						idOldDate = int.Parse(row["date_media_num"].ToString());
						nRow = dtResult.NewRow();
						dtResult.Rows.Add(nRow);
						nRow["rowType"] = Right.type.productAccess.ToString();
						nRow["idMedia"] = row["id_media"].ToString();
						nRow["label"] = row["product"].ToString();
						if (row["date_cover_num"].ToString().Length > 0) {
							date = row["date_cover_num"].ToString();
							nRow["dateCover"] = new DateTime(int.Parse(date.Substring(0, 4)), int.Parse(date.Substring(4, 2)), int.Parse(date.Substring(6, 2)));
						}
						date = row["date_media_num"].ToString();
						nRow["date"] = new DateTime(int.Parse(date.Substring(0,4)), int.Parse(date.Substring(4,2)), int.Parse(date.Substring(6,2)));
						nRow["format"] = row["format"].ToString();
						nRow["location"] = row["location"].ToString();
						nRow["budget"] = Decimal.Parse(row["euro"].ToString());
						//nRow["pdm"] = 100 * Decimal.Parse(nRow["budget"].ToString()) / mdBudg;
						nRow["idProduct"] = row["id_product"].ToString();
						nRow["mediaPaging"] = row["media_paging"].ToString();
						oldMediaRow["budget"] = Decimal.Parse(oldMediaRow["budget"].ToString()) + Decimal.Parse(row["euro"].ToString());
					}
					else{
						nRow["location"] = nRow["location"].ToString() + " , " + row["location"].ToString();
					}
				}

				//calcul des PDM
				idOldMedia = -1;

				foreach(DataRow row in dtResult.Rows){
					//new Media?
					if(int.Parse(row["idMedia"].ToString()) != idOldMedia){
						mdBudg = Decimal.Parse(row["budget"].ToString());
						idOldMedia = int.Parse(row["idMedia"].ToString());
					}
					else{
						//new insertion
						row["pdm"] = 100 * Decimal.Parse(row["budget"].ToString()) / mdBudg;
					}
				}

			}
			return dtResult;

		}

	}
}

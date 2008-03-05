#region Informations
// Auteur: D. V. Mussuma
// Date de création: 9/08/2005 
//Date de modification :
#endregion
using System;
using System.Collections;
using System.Data;
using TNS.AdExpress.Web.DataAccess.Results.APPM;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Exceptions;
using WebConstantes=TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Rules.Results.APPM
{
	/// <summary>
	/// Classe de traitements des données des types d'emplacements du plan.
	/// </summary>
	public class LocationPlanTypesRules
	{	
		/// <summary>
		/// Traitement des données des types d'emplacement du plan
		/// </summary>
		/// <param name="webSession">session du client</param>
		/// <param name="dataSource">source de données</param>
		/// <param name="dateBegin">date de début</param>
		/// <param name="dateEnd">date de fin</param>
		/// <param name="baseTarget">cible de base</param>
		/// <param name="additionalTarget">cible supplémentaire</param>
		/// <returns>données des types d'emplacement du plan traitées</returns>
		public static DataTable GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget) {
			
			#region variables
			DataTable data=null;
			DataTable dtLocationPlanTypes=null;
			DataRow resultRow=null;
//			int i=0;
//			Int64 oldIdLocation=0;
			#endregion

			try{

				//Obtention des données
				data=LocationPlanTypesDataAccess.GetData(webSession,dataSource,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];
			
				//Traitement des données
				if(data!=null && data.Rows.Count>0) {
					dtLocationPlanTypes = new DataTable();
					dtLocationPlanTypes.Columns.Add("id_location");
					dtLocationPlanTypes.Columns.Add("location");
					dtLocationPlanTypes.Columns.Add("unitBaseTarget");
					dtLocationPlanTypes.Columns.Add("repartionBaseTarget");
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						dtLocationPlanTypes.Columns.Add("unitAdditionalTarget");
						dtLocationPlanTypes.Columns.Add("repartionAdditionalTarget");
					}
					#region old version
//					//Total
//					resultRow=dtLocationPlanTypes.NewRow();
//					resultRow["location"]=GestionWeb.GetWebWord(1742,webSession.SiteLanguage);
//					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages)
//					resultRow["unitBaseTarget"]=Math.Round(double.Parse(data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()).ToString())/(double)1000,2);
//					else resultRow["unitBaseTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString());
//					resultRow["repartionBaseTarget"]="100";
//					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
//						resultRow["unitAdditionalTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+additionalTarget.ToString());
//						resultRow["repartionAdditionalTarget"]="100";
//					}
//					dtLocationPlanTypes.Rows.Add(resultRow);
					#endregion

					#region Version corrigée


					string idKey = "-1";
					ArrayList idOldKey = new ArrayList();
					ArrayList idOldLocation = new ArrayList();

					//Total
					resultRow=dtLocationPlanTypes.NewRow();
					resultRow["location"]=GestionWeb.GetWebWord(1742,webSession.SiteLanguage);
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages)
						resultRow["unitBaseTarget"]=(decimal)0;
					else resultRow["unitBaseTarget"]=(decimal)0;
					resultRow["repartionBaseTarget"]="100";
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
						resultRow["unitAdditionalTarget"]=(decimal)0;
						resultRow["repartionAdditionalTarget"]="100";
					}
					dtLocationPlanTypes.Rows.Add(resultRow);
					//unité par emplacement
					foreach(DataRow dr in data.Rows){
						idKey=dr["id_media"].ToString()+dr["date_media_num"].ToString()+dr["id_advertisement"].ToString();//insertion
						if(!idOldKey.Contains(idKey) && !idOldLocation.Contains(dr["id_location"].ToString())){
							resultRow=dtLocationPlanTypes.NewRow();
							resultRow["location"]=dr["location"].ToString();
							if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages){
								resultRow["unitBaseTarget"]=Math.Round(decimal.Parse(data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()+" AND id_location="+dr["id_location"].ToString()).ToString())/(decimal)1000,2);								
							}
							else {
								resultRow["unitBaseTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()+" AND id_location="+dr["id_location"].ToString());								
							}
							dtLocationPlanTypes.Rows[0]["unitBaseTarget"] = decimal.Parse(dtLocationPlanTypes.Rows[0]["unitBaseTarget"].ToString()) + decimal.Parse(data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()+" AND id_location="+dr["id_location"].ToString()).ToString());//total
							if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
								resultRow["unitAdditionalTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+additionalTarget.ToString()+" AND id_location="+dr["id_location"].ToString());
								dtLocationPlanTypes.Rows[0]["unitAdditionalTarget"] =decimal.Parse(dtLocationPlanTypes.Rows[0]["unitAdditionalTarget"].ToString()) + decimal.Parse(data.Compute("sum("+webSession.Unit+")","id_target="+additionalTarget.ToString()+" AND id_location="+dr["id_location"].ToString()).ToString());//total
							}
							dtLocationPlanTypes.Rows.Add(resultRow);
							idOldKey.Add(dr["id_media"].ToString()+dr["date_media_num"].ToString()+dr["id_advertisement"].ToString());
							idOldLocation.Add(dr["id_location"].ToString());
						}						
					}
					//Pages au 1000ème
					if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages)
						dtLocationPlanTypes.Rows[0]["unitBaseTarget"] = Math.Round(decimal.Parse(dtLocationPlanTypes.Rows[0]["unitBaseTarget"].ToString())/(decimal)1000,2);						
					
					//Repartition par emplacement
					foreach(DataRow dr in dtLocationPlanTypes.Rows){
						dr["repartionBaseTarget"]=Math.Round((decimal.Parse(dr["unitBaseTarget"].ToString())*(decimal)100)/decimal.Parse(dtLocationPlanTypes.Rows[0]["unitBaseTarget"].ToString()),2);
						if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp)
						dr["repartionAdditionalTarget"]=Math.Round((decimal.Parse(dr["unitAdditionalTarget"].ToString())*(decimal)100)/decimal.Parse(dtLocationPlanTypes.Rows[0]["unitAdditionalTarget"].ToString()),2);
					}
					
					#endregion
					
					#region old version
					//Repartition par emplacement
//					foreach(DataRow dr in data.Rows){
//						if(oldIdLocation!=Int64.Parse(dr["id_location"].ToString())){
//							resultRow=dtLocationPlanTypes.NewRow();
//							resultRow["location"]=dr["location"].ToString();
//							if(webSession.Unit==WebConstantes.CustomerSessions.Unit.pages)
//							resultRow["unitBaseTarget"]=Math.Round(double.Parse(data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()+" AND id_location="+dr["id_location"].ToString()).ToString())/(double)1000,2);
//							else resultRow["unitBaseTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+baseTarget.ToString()+" AND id_location="+dr["id_location"].ToString());
//							resultRow["repartionBaseTarget"]=Math.Round((double.Parse(resultRow["unitBaseTarget"].ToString())*(double)100)/double.Parse(dtLocationPlanTypes.Rows[0]["unitBaseTarget"].ToString()),2);
//							if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
//								resultRow["unitAdditionalTarget"]=data.Compute("sum("+webSession.Unit+")","id_target="+additionalTarget.ToString()+" AND id_location="+dr["id_location"].ToString());
//								resultRow["repartionAdditionalTarget"]=Math.Round((double.Parse(resultRow["unitAdditionalTarget"].ToString())*(double)100)/double.Parse(dtLocationPlanTypes.Rows[0]["unitAdditionalTarget"].ToString()),2);
//							}
//							dtLocationPlanTypes.Rows.Add(resultRow);
//						}
//						oldIdLocation = Int64.Parse(dr["id_location"].ToString());
//					}	
					#endregion		
				}
			}catch(Exception ex){
				throw new LocationPlanTypesRulesException(" GetData(WebSession webSession,IDataSource dataSource,int dateBegin, int dateEnd,Int64 baseTarget,Int64 additionalTarget) : Impossible de traiter les types d'emplacements du plan",ex);
			}
			return  dtLocationPlanTypes;
		}
		
	}
}

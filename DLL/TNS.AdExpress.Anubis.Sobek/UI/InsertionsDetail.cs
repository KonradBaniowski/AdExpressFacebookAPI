using System;
using System.Text;
using System.IO;
using System.Data;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebFunctions=TNS.AdExpress.Web.Functions; 
using CsteCustomer = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Sobek.Common;

namespace TNS.AdExpress.Anubis.Sobek.UI
{
	/// <summary>
	/// Crée le fichier Texte contanant les insertions de l'étude APPM
	/// </summary>
	public class InsertionsDetail 
	{
		/// <summary>
		/// Crée le fichier texte du détail des insertions de l'APPM
		/// </summary>
		/// <param name="dataSource">Source de données</param>
		/// <param name="config">configuration</param>	
		/// <param name="webSession">Session  du client</param>
		/// <param name="textFilePath">Chemin fichier texte</param>
		/// <param name="sepChar">Seéparateur</param>
		public static void CreateSobekTextFile(IDataSource dataSource, SobekConfig config,WebSession webSession,string textFilePath,string sepChar){
			
			#region Paramétrage des dates
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
			#endregion

			#region targets
			//base target
            Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CsteCustomer.Right.type.aepmWaveAccess));									
			#endregion

			//Obtetention du tableau de résultals des insertions
			DataTable dt = Rules.InsertionsDetail.GetData(dataSource,webSession,dateBegin,dateEnd,idBaseTarget,idWave);

			if(!dt.Equals(System.DBNull.Value) && dt.Rows.Count>0){
				StreamWriter writer=null;
				try { 
					// Instanciation du StreamWriter avec passage du nom du fichier 
					writer = new StreamWriter(textFilePath);
					string sep="";	
					string startDate="";
					string universeDate="";
					string endDate="";

					StringBuilder builder = new StringBuilder();

					//Date de l'univers interrogé
					startDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage);
					endDate=WebFunctions.Dates.DateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage);
					universeDate = GestionWeb.GetWebWord(1541,webSession.SiteLanguage)+" : " +startDate;
					if(!startDate.Equals(endDate)){
						universeDate +=" - "+ endDate; 
					}
					writer.WriteLine(universeDate);
					
					//En - tête des colonnes

					//Pour chaque insertion
					foreach(DataRow row in dt.Rows){
						sep="";
						builder = new StringBuilder();
						
						foreach(DataColumn col in dt.Columns){
							if(!col.ColumnName.Equals("date")){
								if(col.ColumnName.Equals("dateParution")){
									if (row["dateParution"]!= DBNull.Value){
										builder.Append(sep).Append(row["dateParution"]);
									}
									else{
										if (row["date"] != DBNull.Value){
											builder.Append(sep).Append(row["date"]);
										}
										else{
											builder.Append(sep).Append("");
										}
									}
								}
								else builder.Append(sep).Append(row[col.ColumnName]);
								sep=sepChar;
							}
						}
						writer.WriteLine(builder.ToString());
					}

					// Fermeture du StreamWriter  
					writer.Close(); 
					builder=null;
				}
				catch( System.Exception ex){
					throw new Exceptions.InsertionsDetailException("CreateSobekTextFile::An error occured when creating insertion detail text File ",ex);
				}
				finally{
					if(writer!=null)
					writer.Close(); 
				}
			}
		}
	}
}

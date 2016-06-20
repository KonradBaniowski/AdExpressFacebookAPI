#region Informations
// Auteur: Y. R'kaina 
// Date de création: 12/01/2007 
#endregion

using System;
using System.Data;
using System.Windows.Forms;
using System.Collections;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;
using WebExceptions=TNS.AdExpress.Web.Exceptions;
using SQLFunctions=TNS.AdExpress.Web.DataAccess;
using WebFnc = TNS.AdExpress.Web.Functions;
using DBConstantes=TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Units;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Rules.Results.APPM{
	/// <summary>
	/// Provides formatting rules for Sector Data Synthesis
	/// </summary>
	public class SectorDataSynthesisRules{
	
		#region Constantes
		/// <summary>
		/// Index de la première colonne du tableau
		/// </summary>
		private const int FIRST_COLUMN_INDEX = 1;
		/// <summary>
		/// Index de la deuxième colonne du tableau
		/// </summary>
		private const int SECOND_COLUMN_INDEX =2;
		/// <summary>
		/// Nombre de lignes dans le tableau
		/// </summary>
		private const int LINES_NUMBER = 14;
		/// <summary>
		/// Nombre de colonnes dans le tableau
		/// </summary>
		private const int COLUMNS_NUMBER = 2;
		#endregion

		#region Sector Data synthesis 
		/// <summary>
		/// Formats the date for sector Data synthesis 
		/// </summary>
		/// <param name="webSession">clients session</param>
		/// <param name="dateBegin">Beginning Date</param>
		/// <param name="dateEnd">Ending Date</param>
		/// <param name="baseTarget">Base target</param>
		/// <param name="additionalTarget">supplementary target</param>
		/// <returns>HTML string for the synthesis table</returns>		
		public static ResultTable GetSynthesisFormattedTable(WebSession webSession,int dateBegin,int dateEnd,Int64 baseTarget,Int64 additionalTarget){
			
			#region variables
			DataTable synthesisTable=null;
			string startDate=string.Empty;
			string targetSelected=string.Empty;
			string targetBase=string.Empty;
			string endDate=string.Empty;
			string oldGroup=string.Empty;
			string groups=string.Empty;
			string groupIds=string.Empty;
			Int64 budget=0;
			int insertions=0;
			int numberOfAdvertiser=0, numberOfBrand=0, numberOfProduct=0, numberOfMedia=0;
			double totalGRP=0;
			double baseTargetGRP=0;
			double additionalTargetGRP=0;
			double additionalTargetCost=0;
			double pages=0;
			ResultTable resultTable=null;
			bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            string numberFormat = "{0:max0}", affinityFormat = "{0:max0}", cGrpFormat = "{0:max0}";
			#endregion	

			try{

				#region get Data
				synthesisTable=TNS.AdExpress.Web.DataAccess.Results.APPM.SectorDataSynthesisDataAccess.GetData(webSession,dateBegin,dateEnd,baseTarget,additionalTarget).Tables[0];
				#endregion

				if(synthesisTable!=null && synthesisTable.Rows.Count>0){

					#region Construction de resultTable
                    int nbLines = LINES_NUMBER;
                    int nbCol = COLUMNS_NUMBER;
                    int lineIndex = 0;
					resultTable = new ResultTable(nbLines,nbCol);
					#endregion
					
					#region date formatting
					//Getting the date in the format yyyyMMdd
					int dateBeginning = int.Parse(WebFnc.Dates.GetPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
					int dateEnding = int.Parse(WebFnc.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
					//Formatting the date in the format e.g 12/12/2005
					if(dateBeginning.ToString().Length>0)
						startDate = WebFnc.Dates.DateToString(new DateTime(int.Parse(dateBeginning.ToString().Substring(0,4)),int.Parse(dateBeginning.ToString().Substring(4,2)),int.Parse(dateBeginning.ToString().Substring(6,2))),webSession.SiteLanguage);						
					if(dateEnding.ToString().Length>0)
						endDate = WebFnc.Dates.DateToString(new DateTime(int.Parse(dateEnding.ToString().Substring(0, 4)), int.Parse(dateEnding.ToString().Substring(4, 2)), int.Parse(dateEnding.ToString().Substring(6, 2))), webSession.SiteLanguage);												
					#endregion															
									
					#region construction of synthesis hashtable
					
					#region traversing the table
					foreach(DataRow dr in synthesisTable.Rows){
						//Values for the base target
						if(Convert.ToInt64(dr["id_target"])==baseTarget){
							baseTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
                            budget += Convert.ToInt64(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.euro].Id.ToString()].ToString());
							targetBase=dr["target"].ToString();
                            insertions += Convert.ToInt32(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.insertion].Id.ToString()].ToString());
                            pages += Convert.ToDouble(dr[UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].Id.ToString()]);
						}
							//values for the supplementary target
						else{
							additionalTargetGRP+=Convert.ToDouble(dr["totalgrp"].ToString());
							targetSelected=dr["target"].ToString();	
						}
						totalGRP+=Convert.ToDouble(dr["totalgrp"].ToString());					
					}					
					#endregion
					
					#region getting number of advertiser
					DataTable advertiserTable=SQLFunctions.Functions.SelectDistinct("advertiser",synthesisTable,"id_advertiser");
					if(advertiserTable!=null)
						numberOfAdvertiser=advertiserTable.Rows.Count;
					#endregion

					#region getting number of brand
					DataTable brandTable=SQLFunctions.Functions.SelectDistinct("brand",synthesisTable,"id_brand");
					if(brandTable!=null)
						numberOfBrand=brandTable.Rows.Count;
					#endregion

					#region getting number of product
					if (showProduct) {
						DataTable productTable = SQLFunctions.Functions.SelectDistinct("product", synthesisTable, "id_product");
						if (productTable != null)
							numberOfProduct = productTable.Rows.Count;
					}
					#endregion

					#region getting number of media
					DataTable mediaTable=SQLFunctions.Functions.SelectDistinct("media",synthesisTable,"id_media");
					if(mediaTable!=null)
						numberOfMedia=mediaTable.Rows.Count;
					#endregion

					// Période d'analyse
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" : ");					
					resultTable[lineIndex,SECOND_COLUMN_INDEX]=new CellLabel(startDate+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+endDate);

					//budget (euros)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2075,webSession.SiteLanguage)+" : ");
					CellEuro cE = new CellEuro(budget);
					cE.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.euro).StringFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cE;

					//nombre de titre
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2108,webSession.SiteLanguage)+" : ");
					CellNumber cN = new CellNumber(numberOfMedia);
					cN.StringFormat = numberFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN;

					//nombre d'insertions
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(144,webSession.SiteLanguage)+" : ");
					CellInsertion cI = new CellInsertion(insertions);
					cI.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.insertion).StringFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cI;
					
					//nombre de pages utilisés
					pages=Math.Round(pages,3);
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" : ");
					CellPage cP = new CellPage(pages);
					cP.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.pages).StringFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cP;
					
					//nombre d'annonceurs
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2073,webSession.SiteLanguage)+" : ");
					CellNumber cN1 = new CellNumber(numberOfAdvertiser);
					cN1.StringFormat = numberFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN1;
					
					//nombre de marques
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2074,webSession.SiteLanguage)+" : ");
					CellNumber cN2 = new CellNumber(numberOfBrand);
					cN2.StringFormat = numberFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN2;
					
					//nombre de produits
					if (showProduct) {
						lineIndex = resultTable.AddNewLine(LineType.level2);
						resultTable[lineIndex, FIRST_COLUMN_INDEX] = new CellLabel(GestionWeb.GetWebWord(1393, webSession.SiteLanguage) + " : ");
						CellNumber cN3 = new CellNumber(numberOfProduct);
						cN3.StringFormat = numberFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cN3;
					}
					//nombre de GRP(cible selectionnée)
					additionalTargetGRP=Math.Round(additionalTargetGRP,3);
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" : ");
					CellGRP cGrp = new CellGRP(additionalTargetGRP);
					cGrp.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.grp).StringFormat;
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp;
					
					//nombre de GRP(cible 15 ans et +)
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1673,webSession.SiteLanguage) + " " +  targetBase + " : ");
					CellGRP cGrp1 = new CellGRP(Math.Round(baseTargetGRP, 3));
					cGrp1.StringFormat = UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.grp).StringFormat; 
					resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp1;
					
					//Affinité GRP vs cible 15 ans à +
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2076,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					if(baseTargetGRP>0){
						CellAffinity cAf = new CellAffinity(Math.Round((additionalTargetGRP / baseTargetGRP) * 100, 3));
						cAf.StringFormat = affinityFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cAf;
					}
					else{
						CellAffinity cAf1 = new CellAffinity(0.0);
						cAf1.StringFormat = affinityFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cAf1;
					}
					// Coût GRP(cible selectionnée)
					additionalTargetCost=Math.Round(additionalTargetCost,3);
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" : ");
					if(additionalTargetGRP>0){
						additionalTargetCost=Math.Round(budget/additionalTargetGRP,3);
						CellCGRP cGrp2 = new CellCGRP(additionalTargetCost);
						cGrp2.StringFormat = cGrpFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp2;
					}
					else{
						CellCGRP cGrp3 = new CellCGRP(0.0);
						cGrp3.StringFormat = cGrpFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp3;
					}
					// Coût GRP(cible 15 et +)
					lineIndex = resultTable.AddNewLine(LineType.level1);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(1675,webSession.SiteLanguage) + " " +  targetBase + " : ");
					if(baseTargetGRP>0){
						CellCGRP cGrp4 = new CellCGRP(Math.Round(budget / baseTargetGRP, 3));
						cGrp4.StringFormat = cGrpFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp4;
					}
					else{
						CellCGRP cGrp5 = new CellCGRP(0.0);
						cGrp5.StringFormat = cGrpFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cGrp5;
					}
					//Affinité coût GRP vs cible 15 ans à +
					lineIndex = resultTable.AddNewLine(LineType.level2);
					resultTable[lineIndex,FIRST_COLUMN_INDEX]=new CellLabel(GestionWeb.GetWebWord(2077,webSession.SiteLanguage)+" vs "+targetBase+" : ");
					if(additionalTargetGRP>0 && baseTargetGRP>0){
						CellAffinity cAf2 = new CellAffinity(Math.Round(((budget / additionalTargetGRP) / (budget / baseTargetGRP)) * 100, 3));
						cAf2.StringFormat = affinityFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cAf2;
					}
					else{
						CellAffinity cAf3 = new CellAffinity(0.0);
						cAf3.StringFormat = affinityFormat;
						resultTable[lineIndex, SECOND_COLUMN_INDEX] = cAf3;
					}
					#endregion
				}
			}
			catch(System.Exception err){
				throw(new WebExceptions.SectorDataSynthesisRulesException("Error while formatting the data for Sector Data Synthesis ",err));
			}

			return resultTable;
		}
		#endregion

	}
}

#region Informations
// Auteur: Y. Rkaina
// Date de création: 02-Juin.-2006 11:19:12
// Date de modification:
#endregion

using System;
using System.IO;
using Aspose.Cells;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Units;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de PDVPlan.
	/// </summary>
	public class PDVPlan
	{

        #region Variables Theme Name
        private static string _rowTitle = "PDVPlanRowTitle";
        private static string _rowTitleFirstCol = "PDVPlanRowTitleFirstCol";
        private static string _rowTotal = "PDVPlanRowTotal";
        private static string _rowTotalFirstCol = "PDVPlanRowTotalFirstCol";
        private static string _rowReferent = "PDVPlanRowReferent";
        private static string _rowReferentFirstCol = "PDVPlanRowReferentFirstCol";
        private static string _rowPdv = "PDVPlanRowPDV";
        private static string _rowPdvFirstCol = "PDVPlanRowPDVFirstCol";
        private static string _rowDefault = "PDVPlanRowDefault";
        private static string _rowDefaultFirstCol = "PDVPlanRowDefaultFirstCol";
        #endregion

		#region Analyse des parts de voix
		/// <summary>
		/// Analyse des parts de voix
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {
			
			#region targets
			//base target
            Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CsteCustomer.Right.type.aepmWaveAccess));									
			#endregion

			// Données resultats
			DataTable PDVPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PDVPlanRules.GetData(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
		
			if((PDVPlanData!=null) && PDVPlanData.Rows.Count>0) {

                #region Variables
                int nbMaxRowByPage=42;
				int s=1;
				int cellRow = 9;
				int startIndex=cellRow;	
				int header=1;
				int upperLeftColumn=9;
				Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
				Cells cells = sheet.Cells;
				object[] oArray ;										
				Range range =null;
				string vPageBreaks="";
				double columnWidth=0,indexLogo=0,index;
				bool verif=true;
                string excelPatternNameMax0 = "max0";
                string excelPatternNameMax3 = "max3";
                string excelPatternNamePercentage = "percentage";
                #endregion

                #region insertion des résultats dans feuille excel

                //En-tête du tableau  			
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleFirstCol), null, startIndex - 1, 1, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].WebTextId, webSession.SiteLanguage), startIndex - 1, 2, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage), startIndex - 1, 3, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].WebTextId, webSession.SiteLanguage), startIndex - 1, 4, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1679, webSession.SiteLanguage), startIndex - 1, 5, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitle), GestionWeb.GetWebWord(1735, webSession.SiteLanguage), startIndex - 1, 6, 1);
			
				//Insertion des résultats
				cellRow++;

				oArray = new object[6] ;
				foreach(DataRow row in PDVPlanData.Rows){
					if(row["products"].Equals("PDV")){
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdvFirstCol), row["products"], cellRow - 1, 1, 1);
                        cells[cellRow - 1, 1].Style.HorizontalAlignment = TextAlignmentType.Left;
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdv), Convert.ToDouble(row["euros"])/100, cellRow - 1, 2, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdv), Convert.ToDouble(row["pages"])/100, cellRow - 1, 3, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdv), Convert.ToDouble(row["insertions"])/100, cellRow - 1, 4, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdv), Convert.ToDouble(row["GRP"])/100, cellRow - 1, 5, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowPdv), Convert.ToDouble(row["GRPBaseTarget"]) / 100, cellRow - 1, 6, 1);
                        for (int i = 2; i <= 6; i++) {
                            cells[cellRow - 1, i].Style.HorizontalAlignment = TextAlignmentType.Right;
                            cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNamePercentage); // Number = 10;
                        }
					}
					else{
                        string currentStyle = string.Empty;
                        string currentStyleFirstCol = string.Empty;
                        if (row["products"].Equals("Total")) {
                            currentStyle = _rowTotal;
                            currentStyleFirstCol = _rowTotalFirstCol;
                        }
                        else if (row["products"].Equals("Univers de référence")) {
                            currentStyle = _rowReferent;
                            currentStyleFirstCol = _rowReferentFirstCol;
                        }
                        else {
                            currentStyle = _rowDefault;
                            currentStyleFirstCol = _rowDefaultFirstCol;
                        }

                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyleFirstCol), row["products"], cellRow - 1, 1, 1);
                        cells[cellRow - 1, 1].Style.HorizontalAlignment = TextAlignmentType.Left;
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["euros"].ToString(), WebConstantes.CustomerSessions.Unit.euro, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 2, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["pages"].ToString(), WebConstantes.CustomerSessions.Unit.pages, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 3, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["insertions"].ToString(), WebConstantes.CustomerSessions.Unit.insertion, false,WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 4, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["GRP"].ToString(), WebConstantes.CustomerSessions.Unit.grp, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 5, 1);
                        SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(currentStyle), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["GRPBaseTarget"].ToString(), WebConstantes.CustomerSessions.Unit.grp, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 6, 1);
                        for (int i = 2; i <= 6; i++) {
                            cells[cellRow - 1, i].Style.HorizontalAlignment = TextAlignmentType.Right;
                            if ((i != 2) && (i != 4))
                                cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3);// "# ### ##0.0##";
                            else
                                cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
                        }
					}		 

					cellRow++;
                }
                #region Ajustement de la taile des cellules en fonction du contenu
                for (int c=1;c<=6;c++){
					sheet.AutoFitColumn(c);
				}		
				#endregion

                #region Mise en Page
                //Mise en page de la feuille excel

				for(index=0;index<20;index++){
					columnWidth += cells.GetColumnWidth((byte)index);
					if((columnWidth<125)&&verif)
						indexLogo++;
					else
						verif=false;
				}

				upperLeftColumn=(int)indexLogo;
				vPageBreaks = cells[cellRow,(int)indexLogo+1].Name;
                SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1728, webSession.SiteLanguage), PDVPlanData.Rows.Count + 9, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
                #endregion

                #endregion
            }		

		}
		#endregion


	}
}

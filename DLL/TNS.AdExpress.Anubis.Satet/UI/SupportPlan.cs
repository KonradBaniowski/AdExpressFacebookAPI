#region Informations
// Auteur: Y. Rkaina
// Date de création: 31-May.-2006 15:10:13
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
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de SupportPlan.
	/// </summary>
	public class SupportPlan
	{

		#region Analyse par titre
		/// <summary>
		/// Affinités
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
		
			#region Données resultats
			DataTable supportPlanData=TNS.AdExpress.Web.Rules.Results.APPM.SupportPlanRules.GetData(dataSource,webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idWave);
			#endregion	

			if((supportPlanData!=null) && supportPlanData.Rows.Count>0) {

                #region Variables
                int nbMaxRowByPage=42;
				int s=1;
				int cellRow = 5;
				int startIndex=cellRow;	
				int header=1;
				int upperLeftColumn=8;
				Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
				Cells cells = sheet.Cells;
				string vPageBreaks="";
				string GRP1Length="", GRP2Length="";
				double columnWidth=0,indexLogo=0,index;
				bool verif=true;
				string PDM="";
                string excelPatternNameMax0 = "max0";
                string excelPatternNamePercentage = "percentage";
                #endregion

                #region Variables Theme Name
                string headerTableCellFirstLine = "SupportPlanHeaderTableCellFirstLine";
                string headerTableCellSecondLine = "SupportPlanHeaderTableCellSecondLine";
                string contentTableCellMediaTotal = "SupportPlanContentTableCellMediaTotal";
                string contentTableCellMediaCategory = "SupportPlanContentTableCellMediaCategory";
                string contentTableCellMediaDefault = "SupportPlanContentTableCellMediaDefault";
                #endregion

                #region En-tête du tableau
                cells.Merge(cellRow-1,0,2,1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellFirstLine), GestionWeb.GetWebWord(1141, webSession.SiteLanguage), cellRow - 1, 0, 1);
                SatetFunctions.WorkSheet.CellsStyle(excel,cells, style.GetTag(headerTableCellFirstLine), null, cellRow, 0,0, false);
				cells.Merge(cellRow-1,1,1,2);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellFirstLine), GestionWeb.GetWebWord(1682, webSession.SiteLanguage), cellRow - 1, 1, 1);
                SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(headerTableCellFirstLine), null, cellRow-1, 2,2, true);
				cells.Merge(cellRow-1,3,1,3);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellFirstLine), GestionWeb.GetWebWord(1713, webSession.SiteLanguage) + " " + supportPlanData.Columns["GRP1"].Caption, cellRow - 1, 3, 1);
				GRP1Length = GestionWeb.GetWebWord(1713,webSession.SiteLanguage)+" "+supportPlanData.Columns["GRP1"].Caption;
                SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(headerTableCellFirstLine), null, cellRow - 1, 4,5, true);
				cells.Merge(cellRow-1,6,1,3);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellFirstLine), GestionWeb.GetWebWord(1713, webSession.SiteLanguage) + " " + supportPlanData.Columns["GRP2"].Caption, cellRow - 1, 6, 1);
				GRP2Length = GestionWeb.GetWebWord(1713,webSession.SiteLanguage)+" "+supportPlanData.Columns["GRP2"].Caption;
                SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(headerTableCellFirstLine), null, cellRow - 1, 7, 8, true);

				//SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex-1,0,8,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);

				cellRow++;

                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(1712, webSession.SiteLanguage), cellRow - 1, 1, 1);

				//SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex,0,0,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);

                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(806, webSession.SiteLanguage), cellRow - 1, 2, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(1679, webSession.SiteLanguage), cellRow - 1, 3, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(806, webSession.SiteLanguage), cellRow - 1, 4, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(1685, webSession.SiteLanguage), cellRow - 1, 5, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(1679, webSession.SiteLanguage), cellRow - 1, 6, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(806, webSession.SiteLanguage), cellRow - 1, 7, 1);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(headerTableCellSecondLine), GestionWeb.GetWebWord(1685, webSession.SiteLanguage), cellRow - 1, 8, 1);

				//SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex,1,8,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,true);
				#endregion

				//Insertion des résultats
				cellRow++;

				#region Insertion des résultats
				foreach(DataRow row in supportPlanData.Rows){
					SatetFunctions.WorkSheet.PutCellValue(cells,row["label"],cellRow-1,0,1);

                    if (row[0].ToString() == CsteCustomer.Right.type.categoryAccess.ToString()) {
                        SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(contentTableCellMediaCategory), null, cellRow - 1, 0, 8, false);
					}
					else if(row["label"].Equals("Total")){
                        SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(contentTableCellMediaTotal), null, cellRow - 1, 0, 8, false);
					}
					else{
                        SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(contentTableCellMediaDefault), null, cellRow - 1, 0, 8, false);
						cells[cellRow-1,0].Style.IndentLevel=1;
					}
				
					for(int i = 3; i < row.ItemArray.Length; i++){
                        if (row.ItemArray[i] != DBNull.Value) {
                            if (row.Table.Columns[i].ColumnName.IndexOf("budget") > -1) {
                                SatetFunctions.WorkSheet.PutCellValue(cells, Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()), 2).ToString(), WebConstantes.CustomerSessions.Unit.euro, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, i - 2, 1);
                                cells[cellRow - 1, i - 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].Format); //"# ### ##0";
                            }
                            else if (row.Table.Columns[i].ColumnName.IndexOf("GRP") > -1 && !(row.Table.Columns[i].ColumnName.IndexOf("C/GRP") > -1)) {
                                if (row.ItemArray[i].ToString().Equals("0"))
                                    SatetFunctions.WorkSheet.PutCellValue(cells, 0, cellRow - 1, i - 2, 1);
                                else
                                    SatetFunctions.WorkSheet.PutCellValue(cells, Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()), 2).ToString(), WebConstantes.CustomerSessions.Unit.grp, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, i - 2, 1);
                                cells[cellRow - 1, i - 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.grp].Format); //"# ### ##0.0##";
                            }
                            else if (row.Table.Columns[i].ColumnName.IndexOf("PDM") > -1) {
                                PDM = WebFunctions.Units.ConvertUnitValueAndPdmToString(Math.Round(Decimal.Parse(row.ItemArray[i].ToString()), 2).ToString(), WebConstantes.CustomerSessions.Unit.euro, true, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo);
                                SatetFunctions.WorkSheet.PutCellValue(cells, Math.Round(Decimal.Parse(PDM), 2), cellRow - 1, i - 2, 1);
                                cells[cellRow - 1, i - 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNamePercentage); //10;
                            }
                            else {
                                SatetFunctions.WorkSheet.PutCellValue(cells, Math.Round(Decimal.Parse(row.ItemArray[i].ToString()), 0), cellRow - 1, i - 2, 1);
                                cells[cellRow - 1, i - 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
                            }
                            if ((i - 2) >= 1)
                                cells[cellRow - 1, i - 2].Style.HorizontalAlignment = TextAlignmentType.Right;
                        }
					}
					cellRow++;
				}
				#endregion

                #region Ajustement de la taile des cellules en fonction du contenu
				for(int c=0;c<=8;c++){
					sheet.AutoFitColumn(c,5,50);
					cells.SetColumnWidth((byte)c,cells.GetColumnWidth((byte)c)+2);
					cells[4,c].Style.IsTextWrapped = true;
					cells[4,c].Style.VerticalAlignment = TextAlignmentType.Top;
					sheet.AutoFitRow(4,3,5);
					cells.GetRowHeight((byte)4);
				}		

				if(GRP1Length.Length>GRP2Length.Length)
					cells.SetRowHeight((byte)4,12*IndexFitColumn(GRP1Length.Length));
				else
					cells.SetRowHeight((byte)4,12*IndexFitColumn(GRP2Length.Length));

				for(index=0;index<20;index++){
					columnWidth += cells.GetColumnWidth((byte)index);
					if((columnWidth<125)&&verif)
						indexLogo++;
					else
						verif=false;
                }
                #endregion

                #region Mise en page
                upperLeftColumn =(int)indexLogo;
				vPageBreaks = cells[cellRow,(int)indexLogo+1].Name;
                SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1680, webSession.SiteLanguage), supportPlanData.Rows.Count + 5, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
                #endregion
            }
		}
		#endregion

		#region méthode interne
		private static int IndexFitColumn(int GRPLength){
			int indexFitColumn;
			indexFitColumn = (int)Math.Ceiling((double)GRPLength / 23);
			return(indexFitColumn+1);
		}
		#endregion
	}
}

#region Informations
// Auteur: Y. R'kaina
// Date de création: 07/02/2007
// Date de modification:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Drawing;
using Aspose.Cells;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using APPMRules = TNS.AdExpress.Web.Rules.Results.APPM;
using AmsetFunctions=TNS.AdExpress.Anubis.Amset.Functions;
using AmsetExceptions=TNS.AdExpress.Anubis.Amset.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.Customer;
using CustomerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Description résumée de Average.
	/// </summary>
	public class Average{

		#region Constantes
		/// <summary>
		/// Index de la prmière colonne de la table
		/// </summary>
		private const int FIRST_TABLE_COLUMN=1;
		/// <summary>
		/// Index de la deuxième colonne de la table
		/// </summary>
		private const int SECOND_TABLE_COLUMN=2;
		/// <summary>
		/// Index de la troisième colonne de la table
		/// </summary>
		private const int THIRD_TABLE_COLUMN=3;
		/// <summary>
		/// Index de la quatrième colonne de la table
		/// </summary>
		private const int FOURTH_TABLE_COLUMN=4;
		/// <summary>
		/// Index de la prmière colonne de la feuille excel
		/// </summary>
		private const int FIRST_SHEET_COLUMN=1;
		/// <summary>
		/// Index de la deuxième colonne de la feuille excel
		/// </summary>
		private const int SECOND_SHEET_COLUMN=2;
		/// <summary>
		/// Index de la troisième colonne de la feuille excel
		/// </summary>
		private const int THIRD_SHEET_COLUMN=3;
		/// <summary>
		/// Index de la quatrième colonne de la feuille excel
		/// </summary>
		private const int FOURTH_SHEET_COLUMN=4;
		/// <summary>
		/// Index de la ligne blanche
		/// </summary>
		private const int WHITE_LINE=4;
		/// <summary>
		/// Index de la ligne des pages
		/// </summary>
		private const int PAGES_INDEX=3;
		#endregion

        #region Variables Theme Name
        private static string _rowLine1 = "AverageLine1";
        private static string _rowLine2 = "AverageLine2";
        #endregion

		#region Average
		/// <summary>
		/// Average
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {
		
			try{
				string format = "";
				#region Paramétrage des dates
				//Formatting date to be used in the tabs which use APPM Press table
				int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
				Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CustomerConstantes.Right.type.aepmBaseTargetAccess));
				//additional target
				Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CustomerConstantes.Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
				Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CustomerConstantes.Right.type.aepmWaveAccess));									
				#endregion

				ResultTable resultTable = APPMRules.SectorDataAverageRules.GetAverageFormattedTable(webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);					

				if(resultTable.LinesNumber>0){

					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;
					int cellRow = 5, cellColumn=1;
                    string tagName = string.Empty;
					double tempValue=0;

					cellRow = AmsetFunctions.WorkSheet.RenderHeader(excel,sheet,cells,style,resultTable.NewHeaders.Root,cellRow,cellColumn);
				
					#region Lignes du tableau
					int changeColor=0;
					for(int i=0; i<resultTable.LinesNumber;i++){
				
						if(i!=WHITE_LINE){
							if((changeColor%2)==0)
                                tagName = _rowLine1;
                            else
                                tagName = _rowLine2;
							changeColor++;

							AmsetFunctions.WorkSheet.CellsStyle(excel,cells,style.GetTag(tagName),null,cellRow,FIRST_SHEET_COLUMN,FOURTH_SHEET_COLUMN,false);

                            AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellLabel)resultTable[i, FIRST_TABLE_COLUMN]).Label, cellRow, FIRST_SHEET_COLUMN,SECOND_SHEET_COLUMN);
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value;

                            AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), tempValue, cellRow, SECOND_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                            format = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, SECOND_TABLE_COLUMN]).StringFormat);
							cells[cellRow, SECOND_SHEET_COLUMN].Style.Custom = format;
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Value;

                            AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), tempValue, cellRow, THIRD_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                            cells[cellRow, THIRD_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, THIRD_TABLE_COLUMN]).StringFormat); 
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Value;

                            AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), tempValue, cellRow, FOURTH_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                            cells[cellRow, FOURTH_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, FOURTH_TABLE_COLUMN]).StringFormat);//.Style;*
                           
							cellRow++;
						}
					}
					#endregion				
				
					for(int c=FIRST_SHEET_COLUMN;c<=FOURTH_SHEET_COLUMN;c++){
						if(c==FIRST_SHEET_COLUMN)
							cells.SetColumnWidth((byte)c,42);
						else
							cells.SetColumnWidth((byte)c,12);
					}
				
					AmsetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(2081,webSession.SiteLanguage),10,webSession.SiteLanguage,style);
				}
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the average page.",e));			
			}
		}
		#endregion
	}
}

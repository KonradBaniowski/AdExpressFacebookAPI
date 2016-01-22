#region Informations
// Auteur: Y. R'kaina
// Date de création: 08/02/2007
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
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Description résumée de Affinities.
	/// </summary>
	public class Affinities{

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
		/// Index de la quatrième colonne de la table
		/// </summary>
		private const int FIFTH_TABLE_COLUMN=5;
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
		/// Index de la quatrième colonne de la feuille excel
		/// </summary>
		private const int FIFTH_SHEET_COLUMN=5;
		#endregion

        #region Variables Theme Name
        private static string _rowDefault = "AffinitiesRowDefault";
        private static string _rowTotal = "AffinitiesRowTotal";
        #endregion

		#region Affinities
		/// <summary>
		/// Affinities
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {

			try{
		
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

				ResultTable resultTable = APPMRules.SectorDataAffintiesRules.GetData(webSession,webSession.Source,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave);					

				if(resultTable.LinesNumber>0){
			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;
					int cellRow = 5, cellColumn=1;
                    string tagName = string.Empty;

					cellRow = AmsetFunctions.WorkSheet.RenderHeader(excel,sheet,cells,style,resultTable.NewHeaders.Root,cellRow,cellColumn);

					#region Lignes du tableau
					for(int i=0; i<resultTable.LinesNumber;i++){

                        if (i == 0)
                            tagName = _rowTotal;
                        else
                            tagName = _rowDefault;

                        AmsetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(tagName), null, cellRow, FIRST_SHEET_COLUMN, FIFTH_SHEET_COLUMN, false);
                        AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellLabel)resultTable[i, FIRST_TABLE_COLUMN]).Label, cellRow, FIRST_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                        AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellUnit)resultTable[i, SECOND_TABLE_COLUMN]).Value, cellRow, SECOND_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                        cells[cellRow, SECOND_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, SECOND_TABLE_COLUMN]).StringFormat);//.Style;
                        AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellUnit)resultTable[i, THIRD_TABLE_COLUMN]).Value, cellRow, THIRD_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                        cells[cellRow, THIRD_TABLE_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, THIRD_TABLE_COLUMN]).StringFormat);//.Style;
                        AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellUnit)resultTable[i, FOURTH_TABLE_COLUMN]).Value, cellRow, FOURTH_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                        cells[cellRow, FOURTH_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, FOURTH_TABLE_COLUMN]).StringFormat);//.Style;
                        AmsetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(tagName), ((CellUnit)resultTable[i, FIFTH_TABLE_COLUMN]).Value, cellRow, FIFTH_SHEET_COLUMN, SECOND_SHEET_COLUMN);
                        cells[cellRow, FIFTH_TABLE_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, FIFTH_TABLE_COLUMN]).StringFormat);//.Style;

						cellRow++;
					}
					#endregion

					cells.SetColumnWidth((byte)FIRST_SHEET_COLUMN,36);
					cells.SetColumnWidth((byte)SECOND_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)THIRD_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)FOURTH_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)FIFTH_SHEET_COLUMN,12);
                    AmsetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1687, webSession.SiteLanguage), (int)resultTable.LinesNumber, 42, 10, "", "1", webSession.SiteLanguage, style);
				}
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the affinities page.",e));
			}
		}
		#endregion

	}
}

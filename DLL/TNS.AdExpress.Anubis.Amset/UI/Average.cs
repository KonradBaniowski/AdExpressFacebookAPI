#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 07/02/2007
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
	/// Description r�sum�e de Average.
	/// </summary>
	public class Average{

		#region Constantes
		/// <summary>
		/// Index de la prmi�re colonne de la table
		/// </summary>
		private const int FIRST_TABLE_COLUMN=1;
		/// <summary>
		/// Index de la deuxi�me colonne de la table
		/// </summary>
		private const int SECOND_TABLE_COLUMN=2;
		/// <summary>
		/// Index de la troisi�me colonne de la table
		/// </summary>
		private const int THIRD_TABLE_COLUMN=3;
		/// <summary>
		/// Index de la quatri�me colonne de la table
		/// </summary>
		private const int FOURTH_TABLE_COLUMN=4;
		/// <summary>
		/// Index de la prmi�re colonne de la feuille excel
		/// </summary>
		private const int FIRST_SHEET_COLUMN=1;
		/// <summary>
		/// Index de la deuxi�me colonne de la feuille excel
		/// </summary>
		private const int SECOND_SHEET_COLUMN=2;
		/// <summary>
		/// Index de la troisi�me colonne de la feuille excel
		/// </summary>
		private const int THIRD_SHEET_COLUMN=3;
		/// <summary>
		/// Index de la quatri�me colonne de la feuille excel
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

		#region Average
		/// <summary>
		/// Average
		/// </summary>
		internal static void SetExcelSheet(Workbook excel,WebSession webSession,IDataSource dataSource){
		
			try{
				string format = "";
				#region Param�trage des dates
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
					Color foreGroundColor, fontColor;
					double tempValue=0;

					cellRow = AmsetFunctions.WorkSheet.RenderHeader(sheet,cells,resultTable.NewHeaders.Root,cellRow,cellColumn);
				
					#region Lignes du tableau
					int changeColor=0;
					for(int i=0; i<resultTable.LinesNumber;i++){
				
						if(i!=WHITE_LINE){
							fontColor=Color.Black;
							if((changeColor%2)==0)
								foreGroundColor=Color.FromArgb(233,230,239);
							else
								foreGroundColor=Color.FromArgb(208,200,218);
							changeColor++;

							AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow,FIRST_SHEET_COLUMN,FOURTH_SHEET_COLUMN,true,fontColor,foreGroundColor,Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,false);

							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellLabel)resultTable[i,FIRST_TABLE_COLUMN]).Label,cellRow,FIRST_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value;
							
							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,tempValue,cellRow,SECOND_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
							format = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, SECOND_TABLE_COLUMN]).StringFormat);
							cells[cellRow, SECOND_SHEET_COLUMN].Style.Custom = format;
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Value;
							
							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,tempValue,cellRow,THIRD_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
							cells[cellRow, THIRD_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i, THIRD_TABLE_COLUMN]).StringFormat); 
							
							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Value;
							
							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,tempValue,cellRow,FOURTH_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
							cells[cellRow,FOURTH_SHEET_COLUMN].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPatternFromStringFormat(((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).StringFormat);//.Style;
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
				
					AmsetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(2081,webSession.SiteLanguage),10,webSession.SiteLanguage);
				}
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the average page.",e));			
			}
		}
		#endregion
	}
}

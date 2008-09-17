#region Informations
// Auteur: Y. R'kaina
// Date de création: 07/02/2007
// Date de modification:
#endregion

using System;
using System.Data;
using System.Collections;
using System.Drawing;
using Aspose.Excel;

using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using APPMRules = TNS.AdExpress.Web.Rules.Results.APPM;
using AmsetFunctions=TNS.AdExpress.Anubis.Amset.Functions;
using AmsetExceptions=TNS.AdExpress.Anubis.Amset.Exceptions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Description résumée de Synthesis.
	/// </summary>
	public class Synthesis{

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
		/// Index de la prmière colonne de la feuille excel
		/// </summary>
		private const int FIRST_SHEET_COLUMN=1;
		/// <summary>
		/// Index de la deuxième colonne de la feuille excel
		/// </summary>
		private const int SECOND_SHEET_COLUMN=2;
		/// <summary>
		/// Index de la ligne des pages
		/// </summary>
		private const int PAGES_INDEX=4;
		#endregion

		#region Synthesis
		/// <summary>
		/// Synthesis
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){
		
			try{

				#region Paramétrage des dates
				//Formatting date to be used in the tabs which use APPM Press table
				int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
				int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));
				#endregion

				#region targets
				//base target
				Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmBaseTargetAccess));
				//additional target
				Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmTargetAccess));									
				#endregion

				#region Wave
				Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess));									
				#endregion

				ResultTable resultTable = APPMRules.SectorDataSynthesisRules.GetSynthesisFormattedTable(webSession,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);					

				if(resultTable.LinesNumber>0){

					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;
					int cellRow = 6;
					Color foreGroundColor, fontColor;
					double tempValue=0;

					#region Title
					AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(2114,webSession.SiteLanguage),cellRow-1,FIRST_SHEET_COLUMN,false,8,2);
					AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,FIRST_SHEET_COLUMN,SECOND_SHEET_COLUMN,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);
					cells.Merge(cellRow-1,FIRST_SHEET_COLUMN,1,2);
					#endregion

					#region Lignes du tableau
					for(int i=0; i<resultTable.LinesNumber;i++){
				
						fontColor=Color.Black;
						if((i%2)==0)
							foreGroundColor=Color.FromArgb(233,230,239);
						else
							foreGroundColor=Color.FromArgb(208,200,218);

						AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow,FIRST_SHEET_COLUMN,SECOND_SHEET_COLUMN,true,fontColor,foreGroundColor,Color.White,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,8,false);

						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellLabel)resultTable[i,FIRST_TABLE_COLUMN]).Label,cellRow,FIRST_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						if(i==0)
							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellLabel)resultTable[i,SECOND_TABLE_COLUMN]).Label,cellRow,SECOND_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						else{

							if(i==PAGES_INDEX)
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value/1000;
							else
								tempValue=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value;

							AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,tempValue,cellRow,SECOND_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
							cells[cellRow,SECOND_SHEET_COLUMN].Style.Custom=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Style;
						}
						cellRow++;
					}
					#endregion

					for(int c=FIRST_SHEET_COLUMN;c<=SECOND_SHEET_COLUMN;c++){
						if(c==FIRST_SHEET_COLUMN)
							cells.SetColumnWidth((byte)c,44);
						else
							cells.SetColumnWidth((byte)c,24);
					}	

					AmsetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(2114,webSession.SiteLanguage),9);

				}
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the synthesis page.",e));
			}
		}
		#endregion

	}
}

#region Informations
// Auteur: Y. R'kaina
// Date de cr�ation: 08/02/2007
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
using WebConstantes=TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpress.Anubis.Amset.UI{
	/// <summary>
	/// Description r�sum�e de Affinities.
	/// </summary>
	public class Affinities{

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
		/// Index de la quatri�me colonne de la table
		/// </summary>
		private const int FIFTH_TABLE_COLUMN=5;
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
		/// Index de la quatri�me colonne de la feuille excel
		/// </summary>
		private const int FIFTH_SHEET_COLUMN=5;
		#endregion

		#region Affinities
		/// <summary>
		/// Affinities
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){

			try{
		
				#region Param�trage des dates
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

				ResultTable resultTable = APPMRules.SectorDataAffintiesRules.GetData(webSession,webSession.Source,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave);					

				if(resultTable.LinesNumber>0){
			
					Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
					Cells cells = sheet.Cells;
					int cellRow = 5, cellColumn=1;
					Color foreGroundColor;

					cellRow = AmsetFunctions.WorkSheet.RenderHeader(sheet,cells,resultTable.NewHeaders.Root,cellRow,cellColumn);

					#region Lignes du tableau
					for(int i=0; i<resultTable.LinesNumber;i++){
				
						if(i==0)
							foreGroundColor=Color.White;
						else
							foreGroundColor=Color.FromArgb(177,163,193);

						AmsetFunctions.WorkSheet.CellsStyle(cells,null,cellRow,FIRST_SHEET_COLUMN,FIFTH_SHEET_COLUMN,true,Color.Black,foreGroundColor,Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,false);
						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellLabel)resultTable[i,FIRST_TABLE_COLUMN]).Label,cellRow,FIRST_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Value,cellRow,SECOND_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						cells[cellRow,SECOND_SHEET_COLUMN].Style.Custom=((CellUnit)resultTable[i,SECOND_TABLE_COLUMN]).Style;
						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Value,cellRow,THIRD_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						cells[cellRow,THIRD_TABLE_COLUMN].Style.Custom=((CellUnit)resultTable[i,THIRD_TABLE_COLUMN]).Style;
						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Value,cellRow,FOURTH_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						cells[cellRow,FOURTH_SHEET_COLUMN].Style.Custom=((CellUnit)resultTable[i,FOURTH_TABLE_COLUMN]).Style;
						AmsetFunctions.WorkSheet.PutCellValue(sheet,cells,((CellUnit)resultTable[i,FIFTH_TABLE_COLUMN]).Value,cellRow,FIFTH_SHEET_COLUMN,false,8,FIRST_SHEET_COLUMN);
						cells[cellRow,FIFTH_TABLE_COLUMN].Style.Custom=((CellUnit)resultTable[i,FIFTH_TABLE_COLUMN]).Style;

						cellRow++;
					}
					#endregion

					cells.SetColumnWidth((byte)FIRST_SHEET_COLUMN,36);
					cells.SetColumnWidth((byte)SECOND_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)THIRD_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)FOURTH_SHEET_COLUMN,12);
					cells.SetColumnWidth((byte)FIFTH_SHEET_COLUMN,12);
					AmsetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1687,webSession.SiteLanguage),(int)resultTable.LinesNumber,42,10,"","1");
				}
			}
			catch(Exception e){
				throw(new AmsetExceptions.AmsetExcelSystemException("Unable to build the affinities page.",e));
			}
		}
		#endregion

	}
}
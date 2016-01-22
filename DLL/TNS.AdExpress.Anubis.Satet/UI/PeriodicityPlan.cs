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
using TNS.FrameWork;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de PeriodicityPlan.
	/// </summary>
	public class PeriodicityPlan
	{
        #region Variables Theme Name
        private static string _rowTitleFirstRow = "PeriodicityPlanRowTitleFirstRow";
        private static string _rowTitleFirstRowFirstCol = "PeriodicityPlanRowTitleFirstRowFirstCol";
        private static string _rowTitleSecondRow = "PeriodicityPlanRowTitleSecondRow";
        private static string _rowTitleSecondRowFirstCol = "PeriodicityPlanRowTitleSecondRowFirstCol";
        private static string _rowTotal = "PeriodicityPlanRowTotal";
        private static string _rowTotalFirstCol = "PeriodicityPlanRowTotalFirstCol";
        private static string _rowDefault = "PeriodicityPlanRowDefault";
        private static string _rowDefaultFirstCol = "PeriodicityPlanRowDefaultFirstCol";
        #endregion

		#region Analyse par périodicité
		/// <summary>
		/// Analyse par périodicité
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
			DataTable PeriodicityPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PeriodicityPlanRules.PeriodicityPlan(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
		
			if((PeriodicityPlanData!=null) && PeriodicityPlanData.Rows.Count>0){
				
				#region insertion des résultats dans feuille excel
				//En-tête du tableau  			
			
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					GRPTreatment(excel,webSession,PeriodicityPlanData,style);
				}
				else 
					SimpleTreatment(excel,webSession,PeriodicityPlanData,style);
				#endregion
			}
		}
		#endregion

		#region Méthodes internes
		/// <summary>
		/// Traitement des résultats en GRP
		/// </summary>
		/// <param name="excel">excel</param>
		/// <param name="periodicityPlanData">source de données</param>
        private static void GRPTreatment(Workbook excel, WebSession webSession, DataTable periodicityPlanData, TNS.FrameWork.WebTheme.Style style) {
			int s=1;
			int nbMaxRowByPage=42;		
			int upperLeftColumn=9;
			string vPageBreaks="";
			int cellRow = 9;
			int header=1;
			int startIndex=cellRow;	
			Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
			Cells cells = sheet.Cells;
			string unitName="";
			double columnWidth=0,indexLogo=0,index;
			string GRP1Length="", GRP2Length="";
			bool verif=true;
            string excelPatternNameMax3 = "max3";
			
			#region en-tête
			switch (webSession.Unit){
				case WebConstantes.CustomerSessions.Unit.euro:  
					unitName= GestionWeb.GetWebWord(1669,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.kEuro:
					unitName= GestionWeb.GetWebWord(1790,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.insertion:  
					unitName= GestionWeb.GetWebWord(940,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.grp:  
					unitName= GestionWeb.GetWebWord(573,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.pages:
                    unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage));
					break;
				default : break;
			}
            cells.Merge(cellRow-1,1,2,1);
			SatetFunctions.WorkSheet.PutCellValue(excel,sheet,cells,style.GetTag(_rowTitleFirstRowFirstCol),GestionWeb.GetWebWord(1774,webSession.SiteLanguage),cellRow-1,1,2);
            SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_rowTitleSecondRowFirstCol),null, cellRow, 1,1, false);
			cells.Merge(cellRow-1,2,1,2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells,style.GetTag(_rowTitleFirstRow), GestionWeb.GetWebWord(1679, webSession.SiteLanguage) + " (" + periodicityPlanData.Rows[0]["baseTarget"] + ")", cellRow - 1, 2, 2);
			GRP1Length = GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +periodicityPlanData.Rows[0]["baseTarget"]+")";
			cells.Merge(cellRow-1,4,1,2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleFirstRow), GestionWeb.GetWebWord(1679, webSession.SiteLanguage) + " (" + periodicityPlanData.Rows[0]["additionalTarget"] + ")", cellRow - 1, 4, 2);
			GRP2Length = GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +periodicityPlanData.Rows[0]["additionalTarget"]+")";
		
			cellRow++;

            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleSecondRow), unitName, cellRow - 1, 2, 2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleSecondRow), GestionWeb.GetWebWord(264, webSession.SiteLanguage), cellRow - 1, 3, 2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleSecondRow), unitName, cellRow - 1, 4, 2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleSecondRow), GestionWeb.GetWebWord(264, webSession.SiteLanguage), cellRow - 1, 5, 2);
		
			#endregion

           cellRow++;

			#region Total
           SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotalFirstCol), GestionWeb.GetWebWord(1401, webSession.SiteLanguage), cellRow - 1, 1, 2);
           cells[cellRow - 1, 1].Style.HorizontalAlignment = TextAlignmentType.Left;
           SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[0]["totalBaseTargetUnit"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 2, 2);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
            cells[cellRow - 1, 2].Style.HorizontalAlignment = TextAlignmentType.Right;
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), 1, cellRow - 1, 3, 2);
			cells[cellRow-1,3].Style.Number = 9;
            cells[cellRow - 1, 3].Style.HorizontalAlignment = TextAlignmentType.Right;
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[0]["totalAdditionalTargetUnit"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 4, 2);
            cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
            cells[cellRow - 1, 4].Style.HorizontalAlignment = TextAlignmentType.Right;
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), 1, cellRow - 1, 5, 2);
			cells[cellRow-1,5].Style.Number = 9;
            cells[cellRow - 1, 5].Style.HorizontalAlignment = TextAlignmentType.Right;
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,5,true,Color.Black,Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			#endregion

			cellRow++;

			#region Lignes du tableau
			for(int i=1; i<periodicityPlanData.Rows.Count-1;i++){
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefaultFirstCol), periodicityPlanData.Rows[i]["periodicity"], cellRow - 1, 1, 2);
                cells[cellRow - 1, 1].Style.HorizontalAlignment = TextAlignmentType.Left;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[i]["unitBase"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 2, 2);
                cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
                cells[cellRow - 1, 2].Style.HorizontalAlignment = TextAlignmentType.Right;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(periodicityPlanData.Rows[i]["distributionBase"]) / 100, cellRow - 1, 3, 2);
				cells[cellRow-1,3].Style.Number = 10;
                cells[cellRow - 1, 3].Style.HorizontalAlignment = TextAlignmentType.Right;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[i]["unitSelected"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo)), cellRow - 1, 4, 2);
                cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
                cells[cellRow - 1, 4].Style.HorizontalAlignment = TextAlignmentType.Right;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(periodicityPlanData.Rows[i]["distributionSelected"]) / 100, cellRow - 1, 5, 2);
				cells[cellRow-1,5].Style.Number = 10;
                cells[cellRow - 1, 5].Style.HorizontalAlignment = TextAlignmentType.Right;
				cellRow++;
			}
			#endregion

			//Ajustement de la taile des cellules en fonction du contenu
			sheet.AutoFitColumn(1);
			for(int c=1;c<=5;c++){
				sheet.AutoFitColumn(c,9,50);
				cells.SetColumnWidth((byte)c,cells.GetColumnWidth((byte)c)+5);
				cells[8,c].Style.IsTextWrapped = true;
				cells[8,c].Style.VerticalAlignment = TextAlignmentType.Top;
			}	
		

			if(GRP1Length.Length>GRP2Length.Length)
				cells.SetRowHeight((byte)8,12*IndexFitColumn(GRP1Length.Length));
			else
				cells.SetRowHeight((byte)8,12*IndexFitColumn(GRP2Length.Length));


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
            SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1665, webSession.SiteLanguage), periodicityPlanData.Rows.Count, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);

		}
		/// <summary>
		/// Traitement normale des résultats 
		/// </summary>
		/// <param name="excel">excel</param>
		/// <param name="periodicityPlanData">source de données</param>
        private static void SimpleTreatment(Workbook excel, WebSession webSession, DataTable periodicityPlanData, TNS.FrameWork.WebTheme.Style style) {
			int s=1;
			int nbMaxRowByPage=42;		
			int upperLeftColumn=9;
			string vPageBreaks="";
			int cellRow = 9;
			int startIndex=cellRow;	
			int header=1;
			Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
			Cells cells = sheet.Cells;
			string unitName="";
			double columnWidth=0,indexLogo=0,index;
			bool verif=true;
            string excelPatternNameMax0 = "max0";
			
			#region en-tête
			switch (webSession.Unit){
				case WebConstantes.CustomerSessions.Unit.euro:  
					unitName= GestionWeb.GetWebWord(1669,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.kEuro:
					unitName= GestionWeb.GetWebWord(1790,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.insertion:  
					unitName= GestionWeb.GetWebWord(940,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.grp:  
					unitName= GestionWeb.GetWebWord(573,webSession.SiteLanguage);
					break;
				case WebConstantes.CustomerSessions.Unit.pages:
                    unitName = Convertion.ToHtmlString(GestionWeb.GetWebWord(UnitsInformation.List[WebConstantes.CustomerSessions.Unit.pages].WebTextId, webSession.SiteLanguage));
					break;
				default : break;
			}

			SatetFunctions.WorkSheet.PutCellValue(excel,sheet,cells,style.GetTag(_rowTitleFirstRowFirstCol),GestionWeb.GetWebWord(1774,webSession.SiteLanguage),cellRow-1,3,4);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleFirstRow), unitName, cellRow - 1, 4, 4);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTitleFirstRow), GestionWeb.GetWebWord(264, webSession.SiteLanguage), cellRow - 1, 5, 4);
			#endregion

			cellRow++;
			
			#region Total
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotalFirstCol), GestionWeb.GetWebWord(1401, webSession.SiteLanguage), cellRow - 1, 3, 4);
            cells[cellRow - 1, 3].Style.HorizontalAlignment = TextAlignmentType.Left;
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[0]["totalBaseTargetUnit"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo), WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo), cellRow - 1, 4, 4);
            cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
            cells[cellRow - 1, 4].Style.HorizontalAlignment = TextAlignmentType.Right;
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowTotal), 1, cellRow - 1, 5, 4);
			cells[cellRow-1,5].Style.Number = 9;
            cells[cellRow - 1, 5].Style.HorizontalAlignment = TextAlignmentType.Right;
			#endregion

			cellRow++;

			#region Lignes du tableau
			for(int i=1; i<periodicityPlanData.Rows.Count-1;i++){
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefaultFirstCol), periodicityPlanData.Rows[i]["periodicity"], cellRow - 1, 3, 4);
                cells[cellRow - 1, 3].Style.HorizontalAlignment = TextAlignmentType.Left;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(periodicityPlanData.Rows[i]["unitBase"]).ToString(), webSession.Unit, false, WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo), WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo), cellRow - 1, 4, 4);
                cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
                cells[cellRow - 1, 4].Style.HorizontalAlignment = TextAlignmentType.Right;
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_rowDefault), Convert.ToDouble(periodicityPlanData.Rows[i]["distributionBase"]) / 100, cellRow - 1, 5, 4);
				cells[cellRow-1,5].Style.Number = 10;
                cells[cellRow - 1, 5].Style.HorizontalAlignment = TextAlignmentType.Right;
				cellRow++;
			}
			#endregion

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=3;c<=5;c++){
				sheet.AutoFitColumn(c);
			}	
		
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
            SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1665, webSession.SiteLanguage), periodicityPlanData.Rows.Count + 8, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
		}

		private static int IndexFitColumn(int GRPLength){
			int indexFitColumn;
			indexFitColumn = (int)Math.Ceiling((double)GRPLength / 30);
			return(indexFitColumn+1);
		}
		#endregion
	}
}

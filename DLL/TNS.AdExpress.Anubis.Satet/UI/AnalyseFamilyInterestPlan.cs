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
using TNS.AdExpress.Domain.Units;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de AnalyseFamilyInterestPlan.
	/// </summary>
	public class AnalyseFamilyInterestPlan
	{

		#region Analyse par famille de presse
		/// <summary>
		/// Analyse par famille de presse
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource) {
			
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
			DataTable AnalyseFamilyInterestPlanData=TNS.AdExpress.Web.Rules.Results.APPM.AnalyseFamilyInterestPlanRules.InterestFamilyPlan(webSession,dataSource,idWave,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget);
			
			if((AnalyseFamilyInterestPlanData!=null) && AnalyseFamilyInterestPlanData.Rows.Count>0){
                           
				#region insertion des résultats dans feuille excel
				//En-tête du tableau  			
			
				if(webSession.Unit==WebConstantes.CustomerSessions.Unit.grp){
					GRPTreatment(excel,webSession,AnalyseFamilyInterestPlanData);
				}
				else 
					SimpleTreatment(excel,webSession,AnalyseFamilyInterestPlanData);
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
        private static void GRPTreatment(Workbook excel, WebSession webSession, DataTable analyseFamilyInterestPlanData) {
			int s=1;
			int nbMaxRowByPage=42;		
			int upperLeftColumn=8;
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
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1774,webSession.SiteLanguage),cellRow-1,1,false,Color.White,8,2);
			cells.Merge(cellRow-1,2,1,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +analyseFamilyInterestPlanData.Rows[0]["baseTarget"]+")",cellRow-1,2,false,Color.White,8,2);
			GRP1Length = GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +analyseFamilyInterestPlanData.Rows[0]["baseTarget"]+")";
			cells.Merge(cellRow-1,4,1,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +analyseFamilyInterestPlanData.Rows[0]["additionalTarget"]+")",cellRow-1,4,false,Color.White,8,2);
			GRP2Length = GestionWeb.GetWebWord(1679,webSession.SiteLanguage)+ " (" +analyseFamilyInterestPlanData.Rows[0]["additionalTarget"]+")";
			SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex-1,1,5,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);

			cellRow++;

			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,unitName,cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(264,webSession.SiteLanguage),cellRow-1,3,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,unitName,cellRow-1,4,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(264,webSession.SiteLanguage),cellRow-1,5,false,Color.White,8,2);
			cells[cellRow-1,1].Style.ForegroundColor =  Color.FromArgb(100,72,131);
            cells[cellRow - 1, 1].Style.Pattern = BackgroundType.Solid;
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,5,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,8,false);
			#endregion

			cellRow++;

			#region Total
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[0]["totalBaseTargetUnit"]).ToString(),webSession.Unit,false)),cellRow-1,2,false,Color.White,8,2);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,1,cellRow-1,3,false,Color.White,8,2);
			cells[cellRow-1,3].Style.Number = 9;
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[0]["totalAdditionalTargetUnit"]).ToString(),webSession.Unit,false)),cellRow-1,4,false,Color.White,8,2);
            cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,1,cellRow-1,5,false,Color.White,8,2);
			cells[cellRow-1,5].Style.Number = 9;
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,5,true,Color.Black,Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			#endregion

			cellRow++;

			#region Lignes du tableau
			for(int i=1; i<analyseFamilyInterestPlanData.Rows.Count-1;i++){
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,analyseFamilyInterestPlanData.Rows[i]["InterestFamily"],cellRow-1,1,false,Color.White,8,2);
				cells[cellRow-1,1].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,1].Style.Borders[BorderType.TopBorder].Color = Color.White;
				
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["unitBase"]).ToString(),webSession.Unit,false)),cellRow-1,2,false,Color.White,8,2);
				cells[cellRow-1,2].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,2].Style.Borders[BorderType.TopBorder].Color = Color.White;
                cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
				
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["distributionBase"])/100,cellRow-1,3,false,Color.White,8,2);
				cells[cellRow-1,3].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,3].Style.Borders[BorderType.TopBorder].Color = Color.White;
				cells[cellRow-1,3].Style.Number = 10;
				
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["unitSelected"]).ToString(),webSession.Unit,false)),cellRow-1,4,false,Color.White,8,2);
				cells[cellRow-1,4].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,4].Style.Borders[BorderType.TopBorder].Color = Color.White;
                cells[cellRow - 1, 4].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
				
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["distributionSelected"])/100,cellRow-1,5,false,Color.White,8,2);
				cells[cellRow-1,5].Style.Borders[BorderType.TopBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,5].Style.Borders[BorderType.TopBorder].Color = Color.White;
				cells[cellRow-1,5].Style.Number = 10;

				//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,5,true,Color.Black,Color.FromArgb(177,163,193),Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,5,true,Color.Black,Color.FromArgb(177,163,193),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,false);
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
			SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1740,webSession.SiteLanguage),analyseFamilyInterestPlanData.Rows.Count,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());

		}
		/// <summary>
		/// Traitement normale des résultats 
		/// </summary>
		/// <param name="excel">excel</param>
		/// <param name="periodicityPlanData">source de données</param>
        private static void SimpleTreatment(Workbook excel, WebSession webSession, DataTable analyseFamilyInterestPlanData) {
			int s=1;
			int nbMaxRowByPage=42;		
			int upperLeftColumn=8;
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

			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1777,webSession.SiteLanguage),cellRow-1,2,false,Color.White,8,3);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,unitName,cellRow-1,3,false,Color.White,8,3);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(264,webSession.SiteLanguage),cellRow-1,4,false,Color.White,8,3);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex-1,2,4,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);
			#endregion

			cellRow++;
			
			#region Total
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1401,webSession.SiteLanguage),cellRow-1,2,false,Color.White,8,3);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[0]["totalBaseTargetUnit"]).ToString(),webSession.Unit,false)),cellRow-1,3,false,Color.White,8,3);
            cells[cellRow - 1, 3].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,1,cellRow-1,4,false,Color.White,8,3);
			cells[cellRow-1,4].Style.Number = 9;
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,4,true,Color.Black,Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
			#endregion

			cellRow++;

			#region Lignes du tableau
			for(int i=1; i<analyseFamilyInterestPlanData.Rows.Count-1;i++){
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,analyseFamilyInterestPlanData.Rows[i]["InterestFamily"],cellRow-1,2,false,Color.White,8,3);
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["unitBase"]).ToString(),webSession.Unit,false)),cellRow-1,3,false,Color.White,8,3);
                cells[cellRow - 1, 3].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(analyseFamilyInterestPlanData.Rows[i]["distributionBase"])/100,cellRow-1,4,false,Color.White,8,3);
				cells[cellRow-1,4].Style.Number = 10;
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,2,4,true,Color.Black,Color.FromArgb(177,163,193),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,CellBorderType.None,8,false);
				cellRow++;
			}
			#endregion

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=2;c<=4;c++){
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
			SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1740,webSession.SiteLanguage),analyseFamilyInterestPlanData.Rows.Count+8,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
		}

		private static int IndexFitColumn(int GRPLength){
			int indexFitColumn;
			indexFitColumn=(int)System.Math.Ceiling((double)GRPLength/30);
			return(indexFitColumn+1);
		}
		#endregion

	}
}

#region Information
//Authors: Y. Rkaina
//Date of Creation: 05/06/2006
//Date of modification:
#endregion

using System;
using System.IO;
using Aspose.Excel;
using System.Drawing;
using System.Data;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de MediaPlan.
	/// </summary>
	public class MediaPlan
	{

		#region Calendrier d'actions
		/// <summary>
		/// Calendrier d'actions
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){
			
			object[,] tab=null;
			int FIRST_PERIOD_INDEX=0;
			int nbColYear=0;
			int prevYear=0;
			int nbPeriodInYear=0;
			int prevnbPeriodInYear=2;
			int startColumn=2;
			int header=1;
			int nbTotalColumn=0;
			string currentCategoryName=string.Empty;
			string prevYearString=string.Empty;
			bool premier=true;
			int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
			int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));			

			#region targets
			//base target
			Int64 idBaseTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmBaseTargetAccess));
			//additional target
			Int64 idAdditionalTarget=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget,Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
			Int64 idWave=Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave,Right.type.aepmWaveAccess));									
			#endregion
		
			#region Données resultats
			tab=TNS.AdExpress.Web.Rules.Results.APPM.MediaPlanRules.GetData(webSession,dataSource,dateBegin,dateEnd,idBaseTarget,idAdditionalTarget,webSession.DetailPeriod);
			#endregion	

			if((tab!=null) && tab.GetLength(0)>0){

				int nbMaxRowByPage=42;
				int s=1;
				int cellRow = 5;
				int startIndex=cellRow;	
				int upperLeftColumn=10;
				Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
				Cells cells = sheet.Cells;
				string vPageBreaks="";
				double columnWidth=0,indexLogo=0,index;
				bool verif=true;

				#region En-tête du tableau  
			
				int nbColTab=tab.GetLength(1),j,i,k;

				#region number of years
				nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
				if (nbColYear>0) nbColYear++;
				FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAPPM.FIRST_PEDIOD_INDEX;			
				#endregion

				#region Colunm labels
		
				cells.Merge(startIndex-1,1,2,0);
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(804,webSession.SiteLanguage),cellRow-1,1,false,Color.White,9,2);
				cells[cellRow-1,1].Style.ForegroundColor = Color.FromArgb(100,72,131);
				cells[cellRow,1].Style.ForegroundColor = Color.FromArgb(100,72,131);
				cells[cellRow-1,1].Style.Font.Color = Color.White;
				cells[cellRow-1,1].Style.Font.IsBold = true;
				cells[cellRow-1,1].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow-1,1].Style.Borders[BorderType.RightBorder].Color = Color.White;
				cells[cellRow,1].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
				cells[cellRow,1].Style.Borders[BorderType.RightBorder].Color = Color.White;
				cellRow++;

				#region Years and months/weeks
				prevYear=int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4));
				for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
					if(prevYear!=int.Parse(tab[0,j].ToString().Substring(0,4))){
						cells.Merge(startIndex-1,prevnbPeriodInYear,1,nbPeriodInYear);
						if(nbPeriodInYear<3){
							prevYearString=" ";
							SatetFunctions.WorkSheet.PutCellValue(sheet,cells,"",startIndex-1,prevnbPeriodInYear,false,Color.Black,9,2);
						}
						else{
							prevYearString=prevYear.ToString();							
							SatetFunctions.WorkSheet.PutCellValue(sheet,cells,int.Parse(prevYearString),startIndex-1,prevnbPeriodInYear,false,Color.Black,9,2);
						}
						
						cells[startIndex-1,prevnbPeriodInYear].Style.ForegroundColor = Color.FromArgb(100,72,131);
						cells[startIndex-1,prevnbPeriodInYear].Style.Font.Color = Color.White;
						cells[startIndex-1,prevnbPeriodInYear].Style.Font.IsBold = true;
						cells[startIndex-1,prevnbPeriodInYear].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
						cells[startIndex-1,prevnbPeriodInYear].Style.Borders[BorderType.LeftBorder].Color = Color.White;
						for (k=0;k<=nbPeriodInYear;k++)
							cells[startIndex-1,k+prevnbPeriodInYear].Style.Number = 1;
					
						//HTML2+="<td colspan="+nbPeriodInYear+" class=\"pmannee\">"+prevYearString+"</td>";
						prevnbPeriodInYear+=nbPeriodInYear;
						nbPeriodInYear=0;
						prevYear=int.Parse(tab[0,j].ToString().Substring(0,4));
					}
					if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.monthly){								
						SatetFunctions.WorkSheet.PutCellValue(sheet,cells,TNS.FrameWork.Date.MonthString.Get(int.Parse(tab[0,j].ToString().Substring(4,2)),webSession.SiteLanguage,1),cellRow-1,startColumn,false,Color.Black,8,2);
						SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(177,163,193),Color.FromArgb(100,72,131),Color.White,false);
						startColumn++;
						nbTotalColumn=startColumn;
					}
					else{	
						SatetFunctions.WorkSheet.PutCellValue(sheet,cells,int.Parse(tab[0,j].ToString().Substring(4,2)),cellRow-1,startColumn,false,Color.Black,8,2);
						SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(177,163,193),Color.FromArgb(100,72,131),Color.White,false);
						cells[cellRow-1,startColumn].Style.Number = 1;
						startColumn++;
						nbTotalColumn=startColumn;
					}
				
					nbPeriodInYear++;
				}

				cells.Merge(startIndex-1,prevnbPeriodInYear,1,nbPeriodInYear);
				if(nbPeriodInYear<3){
					prevYearString=" ";
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,"",startIndex-1,prevnbPeriodInYear,false,Color.Black,9,2);
				}
				else{ 
					prevYearString=prevYear.ToString();
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,int.Parse(prevYearString),startIndex-1,prevnbPeriodInYear,false,Color.Black,9,2);
				}
							
				cells[startIndex-1,prevnbPeriodInYear].Style.ForegroundColor = Color.FromArgb(100,72,131);
				cells[startIndex-1,prevnbPeriodInYear].Style.Font.Color = Color.White;
				cells[startIndex-1,prevnbPeriodInYear].Style.Font.IsBold = true;
				cells[startIndex-1,prevnbPeriodInYear].Style.Borders[BorderType.LeftBorder].LineStyle = CellBorderType.Thin;
				cells[startIndex-1,prevnbPeriodInYear].Style.Borders[BorderType.LeftBorder].Color = Color.White;
				for (k=0;k<nbPeriodInYear;k++)
					cells[startIndex-1,k+prevnbPeriodInYear].Style.Number = 1;
			
				cellRow++;

				#endregion					
					
				#endregion		
			
				#region Traversing the table
				startColumn=2;

				for(i=1;i<tab.GetLength(0);i++){
					for(j=1;j<tab.GetLength(1);j++){
						switch(j){
								
							case FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]!=null && (Int64)tab[i,FrameWorkResultConstantes.MediaPlanAPPM.ID_CATEGORY_COUMN_INDEX]==-1){
										SatetFunctions.WorkSheet.PutCellValue(sheet,cells,tab[i,j],cellRow-1,1,false,Color.Black,8,2);
										cells[cellRow-1,1].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
										cells[cellRow-1,1].Style.Borders[BorderType.RightBorder].Color = Color.White;
										cells[cellRow-1,1].Style.Font.IsBold = true;
									}
									else{ 
										SatetFunctions.WorkSheet.PutCellValue(sheet,cells,tab[i,j],cellRow-1,1,false,Color.Black,8,2);
										SetCellStyle(excel,sheet,cells,cellRow-1,1,Color.FromArgb(177,163,193),Color.White,Color.White,true);
									}
									j=j+3;
								}
								break;

							case FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX:
								if(tab[i,j]!=null){
									if(premier){
										SatetFunctions.WorkSheet.PutCellValue(sheet,cells,tab[i,j],cellRow-1,1,false,Color.Black,8,2);
										SetCellStyle(excel,sheet,cells,cellRow-1,1,Color.FromArgb(225,224,218),Color.White,Color.White,false);
										premier=false;
									}
									else{
										SatetFunctions.WorkSheet.PutCellValue(sheet,cells,tab[i,j],cellRow-1,1,false,Color.Black,8,2);
										SetCellStyle(excel,sheet,cells,cellRow-1,1,Color.FromArgb(208,200,218),Color.White,Color.White,false);
										premier=true;
									}
									j=j+1;
								}
								break;
							case FrameWorkResultConstantes.MediaPlanAPPM.ID_MEDIA_COLUMN_INDEX:									
							case FrameWorkResultConstantes.MediaPlanAPPM.PERIODICITY_COLUMN_INDEX:
								break;
							default:
	
								if(tab[i,j]==null && (tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null )){
									SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),false);
									startColumn++;
									break;
								}
								if((tab[i,FrameWorkResultConstantes.MediaPlanAPPM.MEDIA_COLUMN_INDEX]!=null || tab[i,FrameWorkResultConstantes.MediaPlanAPPM.CATEGORY_COLUMN_INDEX]!=null) && (tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlan.graphicItemType))){
									switch((FrameWorkResultConstantes.MediaPlan.graphicItemType)tab[i,j]){
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.present:{
											SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(100,72,131),Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),false);
											startColumn++;
											break;
										}
										case FrameWorkResultConstantes.MediaPlan.graphicItemType.extended:{
											SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(162,125,203),Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),false);
											startColumn++;
											break;
										}
										default:{
											SetCellStyle(excel,sheet,cells,cellRow-1,startColumn,Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),Color.FromArgb(177,163,193),false);
											startColumn++;
											break;
										}
									}
								}
								break;
						}
					}
					cellRow++;
					startColumn=2;
				}
				#endregion			

				#endregion
 
				//Ajustement de la taile des cellules en fonction du contenu
				sheet.AutoFitColumn(1);
				for(int c=2;c<=nbTotalColumn;c++){
					cells.SetColumnWidth((byte)c,2);
				}	
	
				if(webSession.DetailPeriod==WebConstantes.CustomerSessions.Period.DisplayLevel.monthly){
					for(index=0;index<30;index++){
						columnWidth += cells.GetColumnWidth((byte)index);
						if((columnWidth<124)&&verif)
							indexLogo++;
						else
							verif=false;
					}
					upperLeftColumn=(int)indexLogo-1;
					vPageBreaks = cells[cellRow,(int)indexLogo].Name;
					SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1773,webSession.SiteLanguage),tab.GetLength(0)+3,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
				}
				else{
					if (nbTotalColumn>44){
						upperLeftColumn=nbTotalColumn-3;
						vPageBreaks = cells[cellRow,nbTotalColumn-3].Name;}
					else{
						for(index=0;index<30;index++){
							columnWidth += cells.GetColumnWidth((byte)index);
							if((columnWidth<124)&&verif)
								indexLogo++;
							else
								verif=false;
						}
						upperLeftColumn=(int)indexLogo-1;
						vPageBreaks = cells[cellRow,(int)indexLogo].Name;
					}

					SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1773,webSession.SiteLanguage),ref s,upperLeftColumn,header.ToString());
				}
			}
		}
		#endregion

		#region méthode interne
		private static void SetCellStyle(Excel excel, Worksheet sheet,Cells cells, int cellRow, int cellColumn, Color foregroundColor, Color rightBorderColor, Color bottomBorderColor, bool isBold){
			cells[cellRow,cellColumn].Style.ForegroundColor = foregroundColor;
			cells[cellRow,cellColumn].Style.Borders[BorderType.RightBorder].LineStyle = CellBorderType.Thin;
			cells[cellRow,cellColumn].Style.Borders[BorderType.RightBorder].Color = rightBorderColor;
			cells[cellRow,cellColumn].Style.Borders[BorderType.BottomBorder].LineStyle = CellBorderType.Thin;
			cells[cellRow,cellColumn].Style.Borders[BorderType.BottomBorder].Color = bottomBorderColor;
			cells[cellRow,cellColumn].Style.Font.IsBold = isBold;
		}
		#endregion

	}
}

///////////////////////////////////////////////////////////
//  SatetExcel.cs
//  Implementation of the Class Excel
//  Created on:      29-May.-2006 14:51:12
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////


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

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de Affinities.
	/// </summary>
	public class Affinities
	{

		#region Affinités
		/// <summary>
		/// Affinités
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
			DataTable affinitiesData=TNS.AdExpress.Web.Rules.Results.APPM.AffintiesRules.GetData(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idWave);					
		  
			if((affinitiesData!=null) && affinitiesData.Rows.Count>0){

			int nbMaxRowByPage=42; 
			int s=1;
			int cellRow = 5;
			int startIndex=cellRow;	
			int header=1;
			int upperLeftColumn=12;
			Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
			Cells cells = sheet.Cells;
			object[] oArray ;										
			Range range =null;
			string vPageBreaks="";
			double columnWidth=0,indexLogo=0,index;
			bool verif=true;
            string excelPatternNameMax0 = "max0";
            string excelPatternNameMax3 = "max3";
			
			#region insertion des résultats dans feuille excel

			//En-tête du tableau  			
			oArray = new object[] {GestionWeb.GetWebWord(1708,webSession.SiteLanguage),GestionWeb.GetWebWord(1679,webSession.SiteLanguage),GestionWeb.GetWebWord(1686,webSession.SiteLanguage),GestionWeb.GetWebWord(1685,webSession.SiteLanguage),GestionWeb.GetWebWord(1686,webSession.SiteLanguage)}; 					
			range = cells.CreateRange("A"+startIndex,"E"+startIndex);
			cells.ImportObjectArray(oArray,range.FirstRow,range.FirstColumn,false);									
			range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.White);
			range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.White);
			range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.White);
			range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.White);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex-1,0,4,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);				
			
			//Insertion des résultats
			cellRow++;

			oArray = new object[5] ;
			foreach(DataRow row in affinitiesData.Rows) {
				
				oArray[0]=row["target"];
				oArray[1]=Convert.ToDouble(row["totalGRP"]);
				oArray[2]=Convert.ToDouble(row["GRPAffinities"]);
				oArray[3]=Convert.ToDouble(row["cgrp"]);
				oArray[4]=Convert.ToDouble(row["cgrpAffinities"]);
				range = cells.CreateRange("A"+cellRow,"E"+cellRow);
				cells.ImportObjectArray(oArray,range.FirstRow,range.FirstColumn,false);									
				range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.White);				 

				for(int i=1;i<=4;i++)
					if(i==1)
                        cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
					else
                        cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
				
				if(Convert.ToInt64(row["id_target"])==idBaseTarget){					
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,0,4,true,Color.Black,Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
				}
				else{					
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,0,4,true,Color.Black,Color.FromArgb(208,200,218),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.Thin,8,false);
				}
					
				cellRow++;
			}

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=0;c<=4;c++){
				sheet.AutoFitColumn(c);
				cells[1,c].Style.HorizontalAlignment = TextAlignmentType.Center;
			}		
			#endregion

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
			SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1687,webSession.SiteLanguage),affinitiesData.Rows.Count+4,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
			}

		}
		#endregion

	}
}

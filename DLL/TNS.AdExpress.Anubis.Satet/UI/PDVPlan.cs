#region Informations
// Auteur: Y. Rkaina
// Date de création: 02-Juin.-2006 11:19:12
// Date de modification:
#endregion

using System;
using System.IO;
using Aspose.Excel;
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
	/// Description résumée de PDVPlan.
	/// </summary>
	public class PDVPlan
	{

		#region Analyse des parts de voix
		/// <summary>
		/// Analyse des parts de voix
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){
			
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
			DataTable PDVPlanData=TNS.AdExpress.Web.Rules.Results.APPM.PDVPlanRules.GetData(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,false);
		
			if((PDVPlanData!=null) && PDVPlanData.Rows.Count>0){

				int nbMaxRowByPage=42;
				int s=1;
				int cellRow = 9;
				int startIndex=cellRow;	
				int header=1;
				int upperLeftColumn=9;
				Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
				Cells cells = sheet.Cells;
				object[] oArray ;										
				Range range =null;
				string vPageBreaks="";
				double columnWidth=0,indexLogo=0,index;
				bool verif=true;
                string excelPatternNameMax0 = "max0";
                string excelPatternNameMax3 = "max3";
                string excelPatternNamePercentage = "percentage";
	
				#region insertion des résultats dans feuille excel

				//En-tête du tableau  			
				oArray = new object[] {GestionWeb.GetWebWord(938,webSession.SiteLanguage),GestionWeb.GetWebWord(943,webSession.SiteLanguage),GestionWeb.GetWebWord(940,webSession.SiteLanguage),GestionWeb.GetWebWord(1679,webSession.SiteLanguage),GestionWeb.GetWebWord(1735,webSession.SiteLanguage)}; 					
				range = cells.CreateRange("C"+startIndex,"G"+startIndex);
				cells.ImportObjectArray(oArray,range.FirstRow,range.FirstColumn,false);									
				range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.White);
				range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.White);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,startIndex-1,1,6,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,true);				
			
				//Insertion des résultats
				cellRow++;

				oArray = new object[6] ;
				foreach(DataRow row in PDVPlanData.Rows){
					if(row["products"].Equals("PDV")){
						oArray[0]=row["products"];
						oArray[1]=Convert.ToDouble(row["euros"])/100;
						oArray[2]=Convert.ToDouble(row["pages"])/100;
						oArray[3]=Convert.ToDouble(row["insertions"])/100;
						oArray[4]=Convert.ToDouble(row["GRP"])/100;
						oArray[5]=Convert.ToDouble(row["GRPBaseTarget"])/100;
						SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,6,true,Color.Black,Color.FromArgb(208,200,218),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
						for(int i=2;i<=6;i++)
                            cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNamePercentage); // Number = 10;
					}
					else{
						oArray[0]=row["products"];
						oArray[1]=Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["euros"].ToString(),WebConstantes.CustomerSessions.Unit.euro,false));
						oArray[2]=Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["pages"].ToString(),WebConstantes.CustomerSessions.Unit.pages,false));
						oArray[3]=Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["insertions"].ToString(),WebConstantes.CustomerSessions.Unit.insertion,false));
						oArray[4]=Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["GRP"].ToString(),WebConstantes.CustomerSessions.Unit.grp,false));
						oArray[5]=Convert.ToDouble(WebFunctions.Units.ConvertUnitValueAndPdmToString(row["GRPBaseTarget"].ToString(),WebConstantes.CustomerSessions.Unit.grp,false));
						if(!oArray[0].Equals("Total") && !oArray[0].Equals("Univers de référence"))
							SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,6,true,Color.Black,Color.FromArgb(177,163,193),Color.White,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
						else
							SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,6,true,Color.Black,Color.White,Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,8,false);
						for(int i=2;i<=6;i++)
							if((i!=2)&&(i!=4))
								cells[cellRow-1,i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3);// "# ### ##0.0##";
							else
                                cells[cellRow - 1, i].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
					}
					range = cells.CreateRange("B"+cellRow,"G"+cellRow);
					cells.ImportObjectArray(oArray,range.FirstRow,range.FirstColumn,false);									
					range.SetOutlineBorder(BorderType.RightBorder,CellBorderType.Thin,Color.White);
					range.SetOutlineBorder(BorderType.TopBorder,CellBorderType.Thin,Color.White);
					range.SetOutlineBorder(BorderType.BottomBorder,CellBorderType.Thin,Color.White);
					range.SetOutlineBorder(BorderType.LeftBorder,CellBorderType.Thin,Color.White);				 

					cellRow++;
				}

				//Ajustement de la taile des cellules en fonction du contenu
				for(int c=1;c<=6;c++){
					sheet.AutoFitColumn(c);
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
				SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1728,webSession.SiteLanguage),PDVPlanData.Rows.Count+9,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
			}		

		}
		#endregion


	}
}

#region Informations
// Auteur: Y. Rkaina
// Date de cr�ation: 02-Juin.-2006 11:19:12
// Date de modification:
#endregion


using System;
using System.IO;
using Aspose.Excel;
using System.Drawing;
using System.Data;
using System.Collections;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using TNS.AdExpress.Constantes.Customer;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description r�sum�e de Synthesis.
	/// </summary>
	public class Synthesis
	{

		#region Synth�se
		/// <summary>
		/// Synth�se
		/// </summary>
		internal static void SetExcelSheet(Excel excel,WebSession webSession,IDataSource dataSource){

			#region variables
			bool mediaAgencyAccess=false;
			#endregion

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not
			if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null)
				mediaAgencyAccess=true;
			#endregion

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
			Int64 idProduct=0;
			string idProductString = webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.productAccess);
			if(WebFunctions.CheckedText.IsStringEmpty(idProductString)){
				idProduct=Int64.Parse(idProductString);
			}
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

			#region get Data
			Hashtable synthesisData=TNS.AdExpress.Web.Rules.Results.APPM.SynthesisRules.GetData(webSession,dataSource,int.Parse(webSession.PeriodBeginningDate),int.Parse(webSession.PeriodEndDate),idBaseTarget,idAdditionalTarget,idProduct,mediaAgencyAccess);
			#endregion

			if((synthesisData!=null)&&(synthesisData.Count>0)){

			int nbMaxRowByPage=42;
			int s=1;
			int cellRow = 5;
			int startIndex=cellRow;	
			int header=1;
			int upperLeftColumn=5;
			Worksheet sheet = excel.Worksheets[excel.Worksheets.Add()];
			Cells cells = sheet.Cells;
			string vPageBreaks="";
			double columnWidth=0,indexLogo=0,index;
			bool verif=true;

			#region Tableau
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1666,webSession.SiteLanguage),cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.White,Color.FromArgb(100,72,131),Color.White,CellBorderType.None,CellBorderType.None,CellBorderType.None,CellBorderType.None,12,true);
			cells.Merge(cellRow-1,1,0,2);
			cellRow++;

			if(idProduct!=0){
				//Nom du Produit
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1418,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			    SatetFunctions.WorkSheet.PutCellValue(sheet,cells,synthesisData["product"],cellRow-1,2,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
				cellRow++;
				//Nom de l'announceur
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1667,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,synthesisData["advertiser"],cellRow-1,2,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
				cellRow++;
				
				if(mediaAgencyAccess && synthesisData["agency"].ToString().Length>0){
					//Nom de l'agence Media
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(731,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,synthesisData["agency"],cellRow-1,2,false,Color.White,8,2);
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
					cells[cellRow-1,2].Style.IndentLevel = 2;
					cellRow++;
				}
			}

			//P�riod d'analyse
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(381,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,synthesisData["dateBegin"]+" "+GestionWeb.GetWebWord(125,webSession.SiteLanguage)+" "+synthesisData["dateEnd"],cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Budget brut (euros)
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1669,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToInt64(synthesisData["budget"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0";
			cells[cellRow-1,2].Style.HorizontalAlignment = TextAlignmentType.Left;
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Nombre d'insertions
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1398,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToInt32(synthesisData["insertions"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Nombre des pages
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1385,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(synthesisData["pages"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Nombre de supports utilis�s
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1670,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToInt32(synthesisData["supports"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;

			//Secteur de r�f�rence
			//if the competitor univers is not selected we print the groups of the products selected
			if(webSession.CompetitorUniversAdvertiser.Count<2){
				string[] groups=synthesisData["group"].ToString().Split(',');
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1668,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
				Array.Sort(groups);
				foreach(string gr in groups){
					SatetFunctions.WorkSheet.PutCellValue(sheet,cells,gr,cellRow-1,2,false,Color.White,8,2);
					SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.None,10,false);
					cells[cellRow-1,2].Style.IndentLevel = 2;
					cellRow++;
				}
			    SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-2,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			}
            
			if(synthesisData["PDV"].ToString()!=""){
				//Part de voix de la campagne
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1671,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
				SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(synthesisData["PDV"])/100,cellRow-1,2,false,Color.White,8,2);
				SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
				cells[cellRow-1,2].Style.Number = 10;
				cellRow++;
			}

			//cible selectionn�e
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1672,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,synthesisData["targetSelected"],cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			// nombre de GRP
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Math.Round(Convert.ToDouble(synthesisData["GRPNumber"]),2),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			// nombre de GRP 15 et +
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1673,webSession.SiteLanguage)+" "+synthesisData["baseTarget"]+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Math.Round(Convert.ToDouble(synthesisData["GRPNumberBase"]),2),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Indice GRP vs cible 15 ans � +																				   
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1674,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Math.Round(Convert.ToDouble(synthesisData["IndiceGRP"]),2),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			// Co�t GRP(cible selectionn�e)					
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(synthesisData["GRPCost"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			// Co�t GRP(cible 15 ans et +)					
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1675,webSession.SiteLanguage)+ " " +  synthesisData["baseTarget"]+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Convert.ToDouble(synthesisData["GRPCostBase"]),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;
			//Indice co�t GRP vs cible 15 ans � +
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,GestionWeb.GetWebWord(1676,webSession.SiteLanguage)+" vs "+synthesisData["baseTarget"]+" :",cellRow-1,1,false,Color.White,8,2);
			SatetFunctions.WorkSheet.PutCellValue(sheet,cells,Math.Round(Convert.ToDouble(synthesisData["IndiceGRPCost"]),2),cellRow-1,2,false,Color.White,8,2);
			SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.Custom = "# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
			cellRow++;

			#endregion

			//Ajustement de la taile des cellules en fonction du contenu
			for(int c=1;c<=2;c++){
				sheet.AutoFitColumn(c,5,50);
				cells.SetColumnWidth((byte)c,cells.GetColumnWidth((byte)c)+10);
			}		

			for(index=0;index<20;index++){
				columnWidth += cells.GetColumnWidth((byte)index);
				if((columnWidth<125)&&verif)
					indexLogo++;
				else
					verif=false;
			}

			upperLeftColumn=(int)indexLogo;
			vPageBreaks = cells[cellRow,(int)indexLogo+1].Name;
			cells.Merge(startIndex-1,1,0,2);
				SatetFunctions.WorkSheet.PageSettings(sheet,GestionWeb.GetWebWord(1664,webSession.SiteLanguage),cellRow-7,nbMaxRowByPage,ref s,upperLeftColumn,vPageBreaks,header.ToString());
			}
		}
		#endregion

	}
}
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
using System.Collections;

using TNS.AdExpress.Anubis.Satet;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.DB;
using SatetExceptions=TNS.AdExpress.Anubis.Satet.Exceptions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using SatetFunctions=TNS.AdExpress.Anubis.Satet.Functions;
using RulesResultsAPPM=TNS.AdExpress.Web.Rules.Results.APPM;
using CsteCustomer=TNS.AdExpress.Constantes.Customer;
using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
using TNS.FrameWork.DB.Common;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Anubis.Satet.UI
{
	/// <summary>
	/// Description résumée de Synthesis.
	/// </summary>
	public class Synthesis
	{
        #region Variables Theme Name
        private static string _title = "SynthesisTitle";
        private static string _productTitle = "SynthesisProductTitle";
        private static string _productValue = "SynthesisProductValue";
        private static string _annonceurTitle = "SynthesisAnnonceurTitle";
        private static string _annonceurValue = "SynthesisAnnonceurValue";
        private static string _agenceMediaTitle = "SynthesisAgenceMediaTitle";
        private static string _agenceMediaValue = "SynthesisAgenceMediaValue";
        private static string _periodeTitle = "SynthesisPeriodeTitle";
        private static string _periodeValue = "SynthesisPeriodeValue";
        private static string _budgetTitle = "SynthesisBudgetTitle";
        private static string _budgetValue = "SynthesisBudgetValue";
        private static string _insertionTitle = "SynthesisInsertionTitle";
        private static string _insertionValue = "SynthesisInsertionValue";
        private static string _pageTitle = "SynthesisPageTitle";
        private static string _pageValue = "SynthesisPageValue";
        private static string _supportTitle = "SynthesisSupportTitle";
        private static string _supportValue = "SynthesisSupportValue";
        private static string _referenceTitle = "SynthesisReferenceTitle";
        private static string _referenceValue = "SynthesisReferenceValue";
        private static string _referenceEnd = "SynthesisReferenceValueEnd";
        private static string _pdvTitle = "SynthesisPdvTitle";
        private static string _pdvValue = "SynthesisPdvValue";
        private static string _targetSelectedTitle = "SynthesistargetSelectedTitle";
        private static string _targetSelectedValue = "SynthesistargetSelectedValue";
        private static string _grpTitle = "SynthesisGrpTitle";
        private static string _grpValue = "SynthesisGrpValue";
        private static string _grp15PlusTitle = "SynthesisGrp15PlusTitle";
        private static string _grp15PlusValue = "SynthesisGrp15PlusValue";
        private static string _grpVsTargetTitle = "SynthesisGrpVsTargetTitle";
        private static string _grpVsTargetValue = "SynthesisGrpVsTargetValue";
        private static string _coutGrpTitle = "SynthesisCoutGrpTitle";
        private static string _coutGrpValue = "SynthesisCoutGrpValue";
        private static string _coutGrp15PlusTitle = "SynthesisCoutGrp15PlusTitle";
        private static string _coutGrp15PlusValue = "SynthesisCoutGrp15PlusValue";
        private static string _coutGrpVsTargetTitle = "SynthesisCoutGrpVsTargetTitle";
        private static string _coutGrpVsTargetValue = "SynthesisCoutGrpVsTargetValue";
        #endregion

        #region Synthèse
        /// <summary>
		/// Synthèse
		/// </summary>
        internal static void SetExcelSheet(Workbook excel, WebSession webSession, IDataSource dataSource, TNS.FrameWork.WebTheme.Style style) {

			#region variables
			bool mediaAgencyAccess=false;
            bool showProduct = webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG);
            string idProductString = "";
            string excelPatternNameMax0 = "max0";
            string excelPatternNameMax3 = "max3";
            string excelPatternNamePercentage = "percentage";
			#endregion

			#region Media Agency rights
			//To check if the user has a right to view the media agency or not
			//mediaAgencyAccess flag is used in the rest of the classes which indicates whethere the user has access 
			//to media agency or not
			//if(webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY]!=null)
            if (webSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY))
				mediaAgencyAccess=true;
			#endregion

			#region IdProduct
			//this is the id of the product selected from the products dropdownlist. 0 id refers to the whole univers i.e. if no prodcut is
			//selected its by default the whole univers and is represeted by product id 0.
            Int64 idProduct = 0;
            if (showProduct) {
                //string idProductString = webSession.GetSelection(webSession.CurrentUniversProduct,CustomerRightConstante.type.productAccess);
                if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0) && webSession.SecondaryProductUniverses[0].Contains(0))
                    idProductString = webSession.SecondaryProductUniverses[0].GetGroup(0).GetAsString(TNSClassificationLevels.PRODUCT);
                if (WebFunctions.CheckedText.IsStringEmpty(idProductString)) {
                    idProduct = Int64.Parse(idProductString);
                }
            }
			#endregion

			#region targets
			//base target
            Int64 idBaseTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmBaseTargetAccess));
			//additional target
            Int64 idAdditionalTarget = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMTarget, CsteCustomer.Right.type.aepmTargetAccess));									
			#endregion

			#region Wave
            Int64 idWave = Int64.Parse(webSession.GetSelection(webSession.SelectionUniversAEPMWave, CsteCustomer.Right.type.aepmWaveAccess));									
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
			SatetFunctions.WorkSheet.PutCellValue(excel,sheet,cells,style.GetTag(_title),GestionWeb.GetWebWord(1666,webSession.SiteLanguage),cellRow-1,1,2);
            SatetFunctions.WorkSheet.CellsStyle(excel,cells,style.GetTag(_title),null,cellRow-1,2,2,true);
			cells.Merge(cellRow-1,1,1,2);
			cellRow++;

			if(idProduct!=0){
				//Nom du Produit
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells,style.GetTag(_productTitle), GestionWeb.GetWebWord(1418, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_productValue), synthesisData["product"], cellRow - 1, 2, 2);
				//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
                
				cellRow++;
				//Nom de l'announceur
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells,style.GetTag(_annonceurTitle), GestionWeb.GetWebWord(1667, webSession.SiteLanguage) + " :", cellRow - 1, 1, 2);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_annonceurValue), synthesisData["advertiser"], cellRow - 1, 2, 2);
				//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
                
				cellRow++;
				
				if(mediaAgencyAccess && synthesisData["agency"].ToString().Length>0){
					//Nom de l'agence Media
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_agenceMediaTitle), GestionWeb.GetWebWord(731, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_agenceMediaValue), synthesisData["agency"], cellRow - 1, 2,  2);
					//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
					cells[cellRow-1,2].Style.IndentLevel = 2;
                    
					cellRow++;
				}
			}

			//Périod d'analyse
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_periodeTitle), GestionWeb.GetWebWord(381, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_periodeValue), synthesisData["dateBegin"] + " " + GestionWeb.GetWebWord(125, webSession.SiteLanguage) + " " + synthesisData["dateEnd"], cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Budget brut (euros)
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_budgetTitle), GestionWeb.GetWebWord(1669, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_budgetValue), Convert.ToInt64(synthesisData["budget"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.euro].Format);// "# ### ##0";
			cells[cellRow-1,2].Style.HorizontalAlignment = TextAlignmentType.Left;
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Nombre d'insertions
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_insertionTitle), GestionWeb.GetWebWord(1398, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_insertionValue), Convert.ToInt32(synthesisData["insertions"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.insertion].Format); //"# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Nombre des pages
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_pageTitle), GestionWeb.GetWebWord(1385, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_pageValue), Convert.ToDouble(synthesisData["pages"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(UnitsInformation.List[TNS.AdExpress.Constantes.Web.CustomerSessions.Unit.pages].Format); //"# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Nombre de supports utilisés
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_supportTitle), GestionWeb.GetWebWord(1670, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_supportValue), Convert.ToInt32(synthesisData["supports"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;

			//Secteur de référence
			//if the competitor univers is not selected we print the groups of the products selected
			if(webSession.CompetitorUniversAdvertiser.Count<2){
				string[] groups=synthesisData["group"].ToString().Split(',');
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_referenceTitle), GestionWeb.GetWebWord(1668, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
				Array.Sort(groups);
				foreach(string gr in groups){
                    SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_referenceValue), gr, cellRow - 1, 2,  2);
                    SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_referenceValue), null, cellRow - 1, 2, 2, false);
                    SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_referenceTitle), null, cellRow - 1, 1, 1, false);
					cells[cellRow-1,2].Style.IndentLevel = 2;
                    
					cellRow++;
				}
                SatetFunctions.WorkSheet.CellsStyle(excel, cells, style.GetTag(_referenceEnd), null, cellRow - 2, 1, 2, false);
			}
            
			if(synthesisData["PDV"].ToString()!=""){
				//Part de voix de la campagne
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_pdvTitle), GestionWeb.GetWebWord(1671, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
                SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_pdvValue), Convert.ToDouble(synthesisData["PDV"]) / 100, cellRow - 1, 2,  2);
				//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
				cells[cellRow-1,2].Style.IndentLevel = 2;
                cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNamePercentage);
                
				cellRow++;
			}

			//cible selectionnée
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_targetSelectedTitle), GestionWeb.GetWebWord(1672, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_targetSelectedValue), synthesisData["targetSelected"], cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			// nombre de GRP
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grpTitle), GestionWeb.GetWebWord(1673, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grpValue), Math.Round(Convert.ToDouble(synthesisData["GRPNumber"]), 2), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			// nombre de GRP 15 et +
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grp15PlusTitle), GestionWeb.GetWebWord(1673, webSession.SiteLanguage) + " " + synthesisData["baseTarget"] + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grp15PlusValue), Math.Round(Convert.ToDouble(synthesisData["GRPNumberBase"]), 2), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Indice GRP vs cible 15 ans à +																				   
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grpVsTargetTitle), GestionWeb.GetWebWord(1674, webSession.SiteLanguage) + " vs " + synthesisData["baseTarget"] + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_grpVsTargetValue), Math.Round(Convert.ToDouble(synthesisData["IndiceGRP"]), 2), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			// Coût GRP(cible selectionnée)					
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrpTitle), GestionWeb.GetWebWord(1675, webSession.SiteLanguage) + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrpValue), Convert.ToDouble(synthesisData["GRPCost"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			// Coût GRP(cible 15 ans et +)					
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrp15PlusTitle), GestionWeb.GetWebWord(1675, webSession.SiteLanguage) + " " + synthesisData["baseTarget"] + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrp15PlusValue), Convert.ToDouble(synthesisData["GRPCostBase"]), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.White,Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax0); //"# ### ##0";
			cells[cellRow-1,2].Style.IndentLevel = 2;
            
			cellRow++;
			//Indice coût GRP vs cible 15 ans à +
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrpVsTargetTitle), GestionWeb.GetWebWord(1676, webSession.SiteLanguage) + " vs " + synthesisData["baseTarget"] + " :", cellRow - 1, 1,  2);
            SatetFunctions.WorkSheet.PutCellValue(excel, sheet, cells, style.GetTag(_coutGrpVsTargetValue), Math.Round(Convert.ToDouble(synthesisData["IndiceGRPCost"]), 2), cellRow - 1, 2,  2);
			//SatetFunctions.WorkSheet.CellsStyle(cells,null,cellRow-1,1,2,true,Color.FromArgb(107,89,139),Color.FromArgb(233,230,239),Color.FromArgb(100,72,131),CellBorderType.Thin,CellBorderType.Thin,CellBorderType.None,CellBorderType.Thin,10,false);
            cells[cellRow - 1, 2].Style.Custom = WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].CultureInfo.GetExcelFormatPattern(excelPatternNameMax3); //"# ### ##0.0##";
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
			cells.Merge(startIndex-1,1,1,2);
            SatetFunctions.WorkSheet.PageSettings(sheet, GestionWeb.GetWebWord(1664, webSession.SiteLanguage), cellRow - 7, nbMaxRowByPage, ref s, upperLeftColumn, vPageBreaks, header.ToString(), style);
			}
		}
		#endregion

	}
}

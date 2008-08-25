//#region Informations
//// Auteur: A. Obermeyer 
//// Date de création : 11/10/2004 
//// Date de modification : 11/10/2004 
////		10/05/2005	K.Shehzad	Chagement d'en tête Excel
////		12/08/2005	G. Facon	Nom de fonction
//#endregion

//using System;
//using System.Data;
//using System.Web.UI;
//using System.Web.UI.WebControls;
//using System.Collections;
//using System.Windows.Forms;

//using TNS.AdExpress.Domain.Translation;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Web.DataAccess.Results;
//using FrameWorkConstantes=TNS.AdExpress.Constantes.FrameWork;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using TNS.AdExpress.Constantes.Web;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using TNS.FrameWork;
//using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// Utiliser pour le palmares 
//    /// </summary>
//    public class IndicatorPalmaresUI{
	
//        #region Sortie HTML
//        /// <summary>
//        /// Affichage d'un tableau
//        /// </summary>
//        /// <param name="webSession">Session</param>		
//        /// <param name="tableType"></param>
//        /// <param name="typeYear">Année courante, année N-1</param>
//        /// <param name="excel">Excel</param>
//        /// <param name="itemType">type de champ</param>
//        /// <returns>Code HTML</returns>
//        public static string GetPalmaresIndicatorUI(WebSession webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected typeYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType tableType, string itemType,bool excel){

//            #region Variables

//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            bool firstLine=true;
//            string styleClassTitle="";
//            string styleClassNumber="";
//            string competitor="";
//            string total="";
//            string PeriodDate="";	
//            #region variables tri
////			string imageSortAsc="/Images/Common/fl_tri_croi1.gif";
////			string imageSortDesc="/Images/Common/fl_tri_decroi1.gif";
////			string AscSortLink="";
////			string DescSortLink="";
//            //bool asc=true;
//            #endregion
////			int[] indexArr = null;
////			int[] tempIndexArr = null;
////			int beginIndex=0;
////			int endIndex=0;
//            //string itemToSort=TNS.AdExpress.Constantes.Web.SortedItems.Euro;
			
//            #endregion

//            #region Constantes
			
//            const string P2="p2";
//            // Pour les libellés
//            const string L1="p7";
//            const string competitorStyle="p14";
//            const string competitorExcelStyle="p142";
//            const string referenceStyle="p15";
//            // Pour les chiffres
//            const string L3="p9";
//            const string competitorStyleNB="p141";
//            const string competitorExcelStyleNB="p143";

//            const string referenceStyleNB="p151";			
//            // Total
//            const string totalTitle="pmtotal";
//            const string totalNb="pmtotalnb";
//            //Pas de données
//            //const string  noData="acl4";

//            #endregion

			
//            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(webSession, webSession.PeriodEndDate);
			
//            if (int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate))
//                throw new NoDataException();

//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(absolutEndPeriod, webSession.PeriodType);

			
			
//            object[,] tab=TNS.AdExpress.Web.Rules.Results.IndicatorPalmaresRules.GetFormattedTable(webSession,typeYear,tableType);
//            if(typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                PeriodDate=PeriodBeginningDate.Date.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");
//            }
//            else{
//                PeriodDate=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
//            }

//            if(tab.GetLongLength(0)==1 || double.Parse(tab[0,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString())==0){
				
//                if( tableType==FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser){
//                    t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
//                    t.Append("<tr align=\"center\" class=\"txtViolet11Bold\"><td>");
//                    t.Append(GestionWeb.GetWebWord(177,webSession.SiteLanguage));
//                    t.Append("</td></tr></table>");
//                }
//                return t.ToString();

//            }
//            //Traitement lien de tri			
//            //if(webSession.Sort==1 && WebFunctions.CheckedText.IsStringEmpty(itemType))itemToSort=itemType;
//            //else itemToSort=TNS.AdExpress.Constantes.Web.SortedItems.Euro;

//            t.Append("<table class=\"whiteBackGround\" border=0 cellpadding=0 cellspacing=0 width=\"100%\">");
		
//            #region 1ere ligne
//            t.Append("\r\n\t<tr height=\"30px\" >");
//            if(tableType==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.ElementType.product){
//                t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1314,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
//            }
//            else{
//                t.Append("<td class=\""+P2+"\" nowrap valign=\"top\"  align=\"center\">"+GestionWeb.GetWebWord(1184,webSession.SiteLanguage)+"<br>"+PeriodDate+"</td>");
//            }
//            //libellé et flèche tri euro
//            //generateLink(webSession,ref imageSortAsc,ref imageSortDesc,ref AscSortLink,ref DescSortLink,TNS.AdExpress.Constantes.Web.SortedItems.Euro,itemType,P2);			
//            //t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1170,webSession.SiteLanguage)+AscSortLink+DescSortLink+"</td>");
//            t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1170,webSession.SiteLanguage)+"</td>");
//            //libellé et flèche tri SOV
////			generateLink(webSession,ref imageSortAsc,ref imageSortDesc,ref AscSortLink,ref DescSortLink,TNS.AdExpress.Constantes.Web.SortedItems.SOV,itemType,P2);			
//            t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1186,webSession.SiteLanguage)+"</td>");
//            t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1171,webSession.SiteLanguage)+"</td>");
//            //libellé et flèche tri Rang
////			generateLink(webSession,ref imageSortAsc,ref imageSortDesc,ref AscSortLink,ref DescSortLink,TNS.AdExpress.Constantes.Web.SortedItems.Rank,itemType,P2);			
//            t.Append("<td class=\""+P2+"\" nowrap valign=\"top\">"+GestionWeb.GetWebWord(1172,webSession.SiteLanguage)+"</td>");
//            if(typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                if(webSession.ComparativeStudy){
//                    t.Append("<td class=\""+P2+"\" nowrap valign=\"top\" align=\"center\">"+GestionWeb.GetWebWord(1173,webSession.SiteLanguage)+"</td>");
//                }
//            }
//            t.Append("</tr>");
////			if(webSession.Sort==1 && WebFunctions.CheckedText.IsStringEmpty(itemType))itemToSort=itemType;
////			else itemToSort=TNS.AdExpress.Constantes.Web.SortedItems.Euro;
//            #endregion

//            #region  Tri du tableau
////			//Ordre de tri
////			asc=(webSession.SortOrder.Equals("asc"))?true:false;
////													  
////			//index des éléments triés
////			endIndex=int.Parse(tab.GetLongLength(0).ToString())-(int)1;
////			switch(itemType){
////				case SortedItems.Euro:					
////					indexArr =  WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N,asc,SortedItems.Euro,beginIndex,endIndex);
////					break;
////				case TNS.AdExpress.Constantes.Web.SortedItems.SOV :
////					indexArr =  WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.SOV,asc,SortedItems.SOV,beginIndex,endIndex);
////					break;
////				case SortedItems.SOV_Cumul :
////					indexArr = WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.CUMUL_SOV,asc,SortedItems.SOV_Cumul,beginIndex,endIndex);
////					break;
////				case SortedItems.Evolution :
////					indexArr =  WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.EVOLUTION,asc,SortedItems.Evolution,beginIndex,endIndex);
////					break;
////				case SortedItems.Rank :
////					beginIndex=1;
////					indexArr =  WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.RANK,asc,SortedItems.Rank,beginIndex,endIndex);
////					break;
////				default:
////					indexArr =  WebFunctions.Sort.Sort2D_Array(tab,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N,asc,SortedItems.Euro,beginIndex,endIndex);
////					break;
////			}
////			
////			//Si trie ascendant, on place le total comme premier indice du tableau
////			if(indexArr!=null && webSession.SortOrder.Equals("asc") && !itemType.Equals(SortedItems.Rank)){
////				tempIndexArr = new int[indexArr.Length];
////				tempIndexArr[0]=indexArr[indexArr.Length-1];
////				for(int j=0;j<indexArr.Length-1;j++){
////					tempIndexArr[j+1]=indexArr[j];
////				}
////				indexArr = new int[indexArr.Length];
////				tempIndexArr.CopyTo(indexArr,0);
////			}
////			else{
////				//Cas tri Rang
////				tempIndexArr = new int[indexArr.Length+1];
////				tempIndexArr[0]=0;
////				for(int j=0;j<indexArr.Length-1;j++){
////					tempIndexArr[j+1]=int.Parse(indexArr[j].ToString())+1;
////				}
////				indexArr = new int[indexArr.Length+1];
////				tempIndexArr.CopyTo(indexArr,0);
////			}
//            #endregion

//            for(int i=0;i<tab.GetLongLength(0) && i<11 ;i++){
//            //foreach(int i in indexArr){
								
//                #region Style css
//                if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==2){
//                    styleClassTitle=competitorStyle;
//                    styleClassNumber=competitorStyleNB;
//                    if(excel){
//                        styleClassTitle=competitorExcelStyle;
//                        styleClassNumber=competitorExcelStyleNB;
//                    }
//                }
//                else if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]!=null && (int)tab[i,FrameWorkConstantes.Results.PalmaresRecap.COMPETITOR]==1){
//                    styleClassTitle=referenceStyle;
//                    styleClassNumber=referenceStyleNB;
//                }
//                else{
//                    styleClassTitle=L1;
//                    styleClassNumber=L3;
//                }
//                #endregion
								
//                if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()!="0"){
//                    t.Append("\r\n\t<tr>");
//                    if (firstLine){
//                        if(webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.universTotal){
//                            t.Append("<td class=\""+totalTitle+"\" nowrap>"+GestionWeb.GetWebWord(1188,webSession.SiteLanguage)+"</td>");
//                        }					
//                        else if(webSession.ComparaisonCriterion==TNS.AdExpress.Constantes.Web.CustomerSessions.ComparisonCriterion.sectorTotal){
//                            t.Append("<td class=\""+totalTitle+"\" nowrap>"+GestionWeb.GetWebWord(1189,webSession.SiteLanguage)+"</td>");
//                        }
//                        else{
//                            t.Append("<td class=\""+totalTitle+"\" nowrap>"+GestionWeb.GetWebWord(1190,webSession.SiteLanguage)+"</td>");
//                        }
//                        firstLine=false;
//                        styleClassNumber=totalNb;
//                    }
//                    else{
//                        if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT]!=null)t.Append("<td class=\""+styleClassTitle+"\"  nowrap>"+tab[i,FrameWorkConstantes.Results.PalmaresRecap.PRODUCT].ToString()+"</td>");
//                        else t.Append("<td class=\""+styleClassTitle+"\"  nowrap>&nbsp;</td>");
//                    }

//                    #region Total N
//                    if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()=="0"){
////						total="-";
//                        total="&nbsp;";
//                    }else{						
////						total=double.Parse(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString()).ToString("### ### ###");
//                        total = WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkConstantes.Results.PalmaresRecap.TOTAL_N].ToString(),webSession.Unit);
//                    }
//                    t.Append("<td class=\""+styleClassNumber+"\">"+total+"</td>");
//                    #endregion

//                    #region SOV
//                    if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.SOV].ToString()=="0"){
////						total="-";
//                        total="&nbsp;";
//                    }else{
//                        total=double.Parse(tab[i,FrameWorkConstantes.Results.PalmaresRecap.SOV].ToString()).ToString("0.##");
//                    }

//                    t.Append("<td class=\""+styleClassNumber+"\">"+total+"</td>");
//                    #endregion

//                    #region Cumul SOV
//                    t.Append("<td class=\""+styleClassNumber+"\">"+double.Parse(tab[i,FrameWorkConstantes.Results.PalmaresRecap.CUMUL_SOV].ToString()).ToString("0.##")+"</td>");
//                    #endregion

//                    #region Rang
//                    if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.RANK]!=null){
//                        t.Append("<td class=\""+styleClassNumber+"\" "+competitor+">"+tab[i,FrameWorkConstantes.Results.PalmaresRecap.RANK].ToString()+"</td>");
//                    }
//                    else{
//                        t.Append("<td class=\""+styleClassNumber+"\">&nbsp;</td>");
//                    }
//                    #endregion
				
//                    #region Progression rang
//                    if(typeYear==FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear){
//                        if(webSession.ComparativeStudy){
//                            if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PROGRESS_RANK]!=null){
							
//                                if(tab[i,FrameWorkConstantes.Results.PalmaresRecap.PROGRESS_RANK].ToString()=="0"){
//                                    total="";
//                                }else{
//                                    total=tab[i,FrameWorkConstantes.Results.PalmaresRecap.PROGRESS_RANK].ToString();
//                                }
							
//                                t.Append("<td class=\""+styleClassNumber+"\">"+total+"</td>");
//                            }else{
//                                total="";
//                                t.Append("<td class=\""+styleClassNumber+"\">"+total+"</td>");
//                            }
//                        }
//                    }
//                    #endregion

//                    t.Append("</tr>");
//                }
				
//            }
			
//            t.Append("</table>");

//            return t.ToString();

//        }
		
//        /// <summary>
//        /// Affichage des quatres tableaux
//        /// </summary>		
//        /// <param name="webSession">Session client</param>
//        /// <param name="excel">True si excel</param>
//        /// <param name="itemType">type de l'élément à trier</param>
//        /// <returns>tableaux palmares</returns>
//        public static string GetAllPalmaresIndicatorUI(WebSession webSession,string itemType,bool excel){
		
//            #region Variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            string productCurrentYearTable="";
//            string productPreviousYearTable="";
//            string advertiserCurrentYearTable="";
//            string advertiserPreviousYearTable="";

//            //const string purple="mediumPurple1";



//            #endregion
			
//                productCurrentYearTable=GetPalmaresIndicatorUI(webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType.product,itemType,excel);
//                advertiserCurrentYearTable=GetPalmaresIndicatorUI(webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.currentYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser,itemType,excel);

//                if(webSession.ComparativeStudy){				
//                    productPreviousYearTable=GetPalmaresIndicatorUI(webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.previousYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType.product,itemType,excel);
//                    advertiserPreviousYearTable=GetPalmaresIndicatorUI(webSession,FrameWorkConstantes.Results.PalmaresRecap.typeYearSelected.previousYear,FrameWorkConstantes.Results.PalmaresRecap.ElementType.advertiser,itemType,excel);
//                }
			
//                t.Append("<table >");	
			
		
//            #region Annonceur
//            t.Append("<tr >");
//            t.Append("<td valign=\"top\">");
//            t.Append(advertiserCurrentYearTable);
//            t.Append("</td>");
//            if(webSession.ComparativeStudy){
//                t.Append("<td>&nbsp;</td>");
//                t.Append("<td valign=\"top\" >");
//                t.Append(advertiserPreviousYearTable);
//                t.Append("</td>");
//            }
//            t.Append("</tr>");
//            #endregion
			
//            t.Append("<tr><td></td></tr>");

//            #region Produit
//            t.Append("<tr>");
//            t.Append("<td valign=\"top\"  >");
//            t.Append(productCurrentYearTable);
//            t.Append("</td>");
//            if(webSession.ComparativeStudy){
//                t.Append("<td>&nbsp;</td>");
//                t.Append("<td valign=\"top\" >");
//                t.Append(productPreviousYearTable);
//                t.Append("</td>");
//            }
//            t.Append("</tr>");
//            #endregion

//            t.Append("</table>");

//            return t.ToString();

//        }

//        #endregion

//        #region Sortie Excel
//        /// <summary>
//        /// Génère le code pour la sortie Excel
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="itemType">type d'élément à trier</param>
//        /// <returns>HTML pour Excel</returns>
//        public static string GetAllPalmaresIndicatorExcelUI(WebSession webSession,string itemType){
			
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

//            #region Rappel des paramètres
//            // Paramètres du tableau
//            string temp="";
//            if(webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.PALMARES)
//            {
//                                temp=GestionWeb.GetWebWord(1165,webSession.SiteLanguage);
//            }
//            else if(webSession.CurrentTab==TNS.AdExpress.Constantes.FrameWork.Results.PalmaresRecap.SEASONALITY){
//                                temp=GestionWeb.GetWebWord(1139,webSession.SiteLanguage);
//            }
//            t.Append(ExcelFunction.GetLogo(webSession));
//            t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,temp));

//            #endregion

//            t.Append(GetAllPalmaresIndicatorUI(webSession,itemType,true));
//            t.Append(ExcelFunction.GetFooter(webSession));
//            return Convertion.ToHtmlString(t.ToString());
			
//        }


//        #endregion

//        #region méthodes internes
//        ///// <summary>
//        ///// Génère le lien qui pointe sur les valuers à trier
//        ///// </summary>
//        ///// <param name="webSession">session du client</param>
//        ///// <param name="imageSortAsc">image du tri croissant</param>
//        ///// <param name="imageSortDesc">image du tri decroissant</param>
//        ///// <param name="AscSortLink">lien tri croissant</param>
//        ///// <param name="DescSortLink">lien tri decroissant</param>
//        ///// <param name="itemToSort">élément à trier</param>
//        ///// <param name="itemType">type de l'élément  à trier</param>
//        ///// <param name="P2">classe de style</param>
//        //private static void GenerateLink(WebSession webSession,ref string imageSortAsc,ref string imageSortDesc,ref string AscSortLink,ref string DescSortLink,string itemToSort,string itemType,string P2){
//        //    //Traitement lien de tri						
//        //    if(webSession.SortOrder.Equals(CustomerSessions.SortOrder.CROISSANT.ToString())){	
//        //        if(itemToSort.Equals(itemType)){
//        //            imageSortAsc="/Images/Common/fl_tri_croi1_in.gif";
//        //            AscSortLink="&nbsp;<img src="+imageSortAsc+" border=0>";					
//        //        }
//        //        else {
//        //            imageSortAsc="/Images/Common/fl_tri_croi1.gif";
//        //            AscSortLink="&nbsp;<a class=\""+P2+"\" nowrap href=\""+webSession.LastReachedResultUrl+"?idSession="+webSession.IdSession+"&sortOrder="+CustomerSessions.SortOrder.CROISSANT.ToString()+"&itemType="+itemToSort+"\"><img src="+imageSortAsc+" border=0></a>";
//        //        }
				
//        //        DescSortLink="&nbsp;<a class=\""+P2+"\" nowrap href=\""+webSession.LastReachedResultUrl+"?idSession="+webSession.IdSession+"&sortOrder="+CustomerSessions.SortOrder.DECROISSANT.ToString()+"&itemType="+itemToSort+"\"><img src="+imageSortDesc+" border=0></a>";
//        //    }else{
//        //        if(itemToSort.Equals(itemType)){
//        //            imageSortDesc="/Images/Common/fl_tri_decroi1_in.gif";
//        //            DescSortLink="&nbsp;<img src="+imageSortDesc+" border=0>";					
//        //        }
//        //        else{ imageSortDesc="/Images/Common/fl_tri_decroi1.gif";
//        //            DescSortLink="&nbsp;<a class=\""+P2+"\" nowrap href=\""+webSession.LastReachedResultUrl+"?idSession="+webSession.IdSession+"&sortOrder="+CustomerSessions.SortOrder.DECROISSANT.ToString()+"&itemType="+itemToSort+"\"><img src="+imageSortDesc+" border=0></a>";
//        //        }
//        //        AscSortLink="&nbsp;<a class=\""+P2+"\" nowrap href=\""+webSession.LastReachedResultUrl+"?idSession="+webSession.IdSession+"&sortOrder="+CustomerSessions.SortOrder.CROISSANT.ToString()+"&itemType="+itemToSort+"\"><img src="+imageSortAsc+" border=0></a>";								
//        //    }
					
//        //}
//        #endregion
//    }
//}

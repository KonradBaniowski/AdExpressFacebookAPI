//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de cr�ation: 2/11/2004  
//// Date de modification: 2/11/2004 
////		10/05/2005	K.Shehzad	Chagement d'en t�te Excel
////		12/08/2005	G. Facon	Nom de fonction
//#endregion

//using System.Web;

//using System;
//using System.Collections;
//using System.Web.UI;
//using TNS.AdExpress.Constantes.FrameWork.Results;
//using CstWeb = TNS.AdExpress.Constantes.Web;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using WebFunctions=TNS.AdExpress.Web.Functions;
//using TNS.FrameWork.Date;
//using WebRules=TNS.AdExpress.Web.Rules.Results;
//using DateFunctions = TNS.FrameWork.Date;
//using ConstResults=TNS.AdExpress.Constantes.FrameWork.Results;
//using CustomerRightConstante=TNS.AdExpress.Constantes.Customer.Right;
//using System.Windows.Forms;
//using System.Data;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using TNS.FrameWork;
//using TNS.Classification.Universe;
//using TNS.AdExpress.Classification;

//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// IHM: Tableau de la strat�gie m�dia des indicateurs
//    /// </summary>
//    public class IndicatorMediaStrategyUI{

//        #region sortie HTML (web)
//        /// <summary>
//        /// Cr�e le code HTML pour afficher le tableau de la r�partition m�dia sur le total de la p�riode 
//        /// contenant les �l�ments ci-apr�s :
//        /// en ligne :
//        /// -le total famille (en option uniquement en s�lection de groupe/vari�t�s) ou le
//        /// total march� (en option)
//        /// -les �l�ments de r�f�rences
//        /// -les �l�ments concurrents 
//        /// en colonne :
//        /// -Les investissements de la p�riode N
//        /// -une PDM (part de march� ) exprimant la r�partition media en %
//        /// -une �volution N vs N-1 en % (uniquement dans le cas d'une �tude comparative)
//        /// -le 1er annonceur en Keuros uniquement  pour les lignes total produits �ventuels
//        /// -la 1ere r�f�rence en Keuros uniquement  pour les lignes total produits �ventuels
//        /// Sur la dimension support le tableau est d�clin� de la fa�on suivante :
//        /// -si plusieurs media, le tableua sera d�clin� par media
//        /// -si un seul media, le tableau sera d�clin� par media, cat�gorie et supports
//        /// </summary>				
//        /// <param name="page">Page qui affiche les stat�gies m�dia</param>
//        /// <param name="tab">tableau des r�sultats</param>	
//        /// <param name="webSession">Session du client</param>
//        /// <param name="excel">bool�en pour sortie html ou excel</param>	
//        /// <returns>Code HTML</returns>		
//        public static string GetIndicatorMediaStrategyHtmlUI(Page page,object[,] tab,WebSession webSession,bool excel){	
			
//            #region Pas de donn�es � afficher
//            if(tab.GetLength(0)==0 ){
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"</div>");
//            }			
//            #endregion

//            #region variables
//            string image="/I/p.gif";
//            //string spacer="";
//            //string switchParent="";
//            string classCss="";		
//            string classCss2="p7";
//            string VehicleAccessList =((LevelInformation)webSession.CurrentUniversMedia.FirstNode.Tag).ID.ToString();				
//            string AdvertiserAccessList="";			
//            string CompetitorAdvertiserAccessList="",tempListAsString="";			
//            ArrayList AdvertiserAccessListArr=null;
//            ArrayList CompetitorAdvertiserAccessListArr=null;
//            NomenclatureElementsGroup nomenclatureElementsGroup = null;

//            #endregion
			
//            #region Constantes			
//            const string L1="acl1";		
//            const string L2="asl5"; 
//            const string L2nb="asl5nb";
//            string L3="asl5b"; 
//            //const string P2="p2";
//            //const string spacerVeh="";
//            const string spacerCat="&nbsp;&nbsp;&nbsp;";
//            const string spacerMed="&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";

//             //string competitorExcelStyle="p142";
//            // string competitorExcelStyleNB="p143";
//            #endregion
			
//            #region identiant annonceurs r�f�rence et concurrents

//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(1)) {
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[1].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0){ 
//                        CompetitorAdvertiserAccessList = tempListAsString;
//                        CompetitorAdvertiserAccessListArr = new ArrayList(CompetitorAdvertiserAccessList.Split(','));
//                    }
//                }
//            }
//            if (webSession.SecondaryProductUniverses.Count > 0 && webSession.SecondaryProductUniverses.ContainsKey(0)) {
//                nomenclatureElementsGroup = webSession.SecondaryProductUniverses[0].GetGroup(0);
//                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0) {
//                    tempListAsString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
//                    if (tempListAsString != null && tempListAsString.Length > 0){
//                        AdvertiserAccessList = tempListAsString;
//                        AdvertiserAccessListArr = new ArrayList(AdvertiserAccessList.Split(','));
//                    }
//                }
//            }
			
//            #endregion

//            #region p�riode �tudi�e
//            //fin p�riode �tudi�e
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
//            #endregion

//            System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
//            //Debut tableau
//            t.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >");
//            #region ligne lib�ll�s
//            t.Append("<tr>");										
//            t.Append("<td nowrap  colspan=2 class=\"p2\">");
//            t.Append("&nbsp;");															
//            t.Append("</td>");
//            t.Append("<!--<td nowrap  class=\"p2\">");
//            t.Append("&nbsp;");															
//            t.Append("</td>-->");
//            //Colonne separation 
//            if(!excel){
//                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//            }
//            //Colonne lib�ll� m�dia
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1246,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"</td>");				
			
//            //colonne lib�ll� annonceur ou totaux produits
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");				
//            //colonne evolution
//            if(webSession.ComparativeStudy)t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1168,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"/"+(PeriodEndDate.Year-1)+"</td>");				
//            //Colonne separation 
//            if(!excel){
//                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//            }
//            //Colonne lib�ll� 1er annonceur
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1154,webSession.SiteLanguage)+"</td>");				
//            //colonne investissement 1er annonceur sur p�riode N et par m�dia ( m�dia ou cat�gorie ou support)
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1246,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"</td>");				
//            //Colonne separation 
//            if(!excel){
//                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//            }
//            //colonne lib�ll� 1ere r�f�rence
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1155,webSession.SiteLanguage)+"</td>");				
//            //colonne investissement 1ere r�f�rence sur p�riode N et par m�dia  ( m�dia  ou cat�gorie ou support)
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1246,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"</td>");				
//            t.Append("</tr>");	
//            #endregion
//            for(int i=0;i<tab.GetLength(0);i++){
				
//                #region lignes total media
//                classCss=L1;
//                if(webSession.ComparativeStudy){
//                    if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                        if(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString())>(double)0.0)image="/I/g.gif";
//                        else if (double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString())<(double)0.0)image="/I/r.gif";
//                        else if (double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString())==(double)0.0)image="/I/o.gif";							
//                    }
//                }

//                #region ligne total march� ou famille
//                //ligne total march� ou famille	pour plurimedia																									
//                if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]!=null){							
//                    t.Append("<tr>");	
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    t.Append(GestionWeb.GetWebWord(210,webSession.SiteLanguage).ToString());															
//                    t.Append("</td>");		
							
//                    t.Append("<td class=\""+classCss+"\"  nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]!=null)
//                        t.Append(GestionWeb.GetWebWord(1190,webSession.SiteLanguage).ToString());
//                    else if(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]!=null)
//                        t.Append(GestionWeb.GetWebWord(1189,webSession.SiteLanguage).ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\"  align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else if(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td nowrap class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    if(webSession.ComparativeStudy){
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                            if(!excel)
//                                t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                            else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                        }
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                    }
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\"  nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("</tr>");	
//                    //switchParent="veh";
//                }
									
//                //Ligne total march� ou famille par media (vehicle)
//                if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]!=null){
//                    t.Append("<tr>");
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX]!=null)t.Append(tab[i,ConstResults.MediaStrategy.LABEL_VEHICLE_COLUMN_INDEX].ToString());															
//                    t.Append("</td>");							
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]!=null)
//                        t.Append(GestionWeb.GetWebWord(1190,webSession.SiteLanguage).ToString());
//                    else if(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]!=null)
//                        t.Append(GestionWeb.GetWebWord(1189,webSession.SiteLanguage).ToString());
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
							
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_VEHICLE_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else if(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_VEHICLE_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    if(webSession.ComparativeStudy){
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                            if(!excel)
//                                t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                            else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                        }
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                    }
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());						
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");							
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("</tr>");
//                    //switchParent="veh";
//                }							
//                #endregion

//                #region ligne total univers
//                //ligne total univers pour plurimedia																										
//                if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]!=null){							
//                    t.Append("<tr>");
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    t.Append("&nbsp;");															
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    t.Append(GestionWeb.GetWebWord(1188,webSession.SiteLanguage).ToString());
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else if( tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX].ToString());							
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_VEHICLE_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));							
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////						t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    if(webSession.ComparativeStudy){
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                            if(!excel)
//                                t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                            else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                        }
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                    }
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());						
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    //Colonne separation 
//                    if(!excel){
//                        t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                    }
//                    t.Append("<td class=\""+classCss+"\" nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                        t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                    else t.Append("&nbsp;");
//                    t.Append("</td>");
//                    t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                    if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////						t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                        t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                    else t.Append("");
//                    t.Append("</td>");						
//                    t.Append("</tr>");	
//                    //switchParent="veh";
//                }																										
//                #endregion
//                #endregion
				
//                if(WebRules.IndicatorMediaStrategyRules.TreatCategory(webSession)){
//                    if(excel){							
//                        //competitorExcelStyleNB="p143xls";
//                        classCss="p142xls";					
//                    }
//                    else classCss=L2;

//                    #region lignes totaux par cat�gorie
//                    // lignes total march� ou famille pour chaque cat�gorie s�lectionn�e
				
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null){							
//                        t.Append("<tr>");										
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX]!=null)
//                            t.Append(spacerCat + tab[i,ConstResults.MediaStrategy.LABEL_CATEGORY_COLUMN_INDEX].ToString());															
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");						
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null )
//                            t.Append( GestionWeb.GetWebWord(1190,webSession.SiteLanguage).ToString());
//                        else if (tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null )
//                            t.Append( GestionWeb.GetWebWord(1189,webSession.SiteLanguage).ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2nb;
//                        }
						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_CATEGORY_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else if (tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX]!=null)
////							t.Append( double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_CATEGORY_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(webSession.ComparativeStudy){
//                            t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                            if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                                if(!excel)
//                                    t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                                else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                            }
//                            else t.Append("&nbsp;");
//                            t.Append("</td>");
//                        }
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2;
//                        }
						
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(!excel)classCss=L2nb;						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2;
//                        }
						
//                        t.Append("<td class=\""+classCss+"\"   nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(!excel)classCss=L2nb;						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("</tr>");	
//                        //switchParent="cat";
//                    }
//                    // lignes total univers pour chaque cat�gorie s�lectionn�e
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null){
//                        t.Append("<tr>");										
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append("&nbsp;");															
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append(GestionWeb.GetWebWord(1188,webSession.SiteLanguage).ToString());				
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2nb;
//                        }
						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##"));
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_CATEGORY_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(webSession.ComparativeStudy){
//                            t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                            if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                                if(!excel)
//                                    t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                                else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                            }
//                            else t.Append("&nbsp;");
//                            t.Append("</td>");
//                        }
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2;
//                        }						
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(!excel)classCss=L2nb;						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                            classCss=L2;
//                        }						
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(!excel)classCss=L2nb;						
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("</tr>");
//                        //switchParent="cat";
//                    }				
//                    #endregion
//                }
				
//                if(WebRules.IndicatorMediaStrategyRules.TreatMedia(webSession)){
//                    if(excel)L3="asl5bxls";
//                    classCss=L3;
//                    #region lignes totaux par support(s)
//                    // lignes total march� ou famille pour chaque support s�lectionn�e								
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null){							
//                        t.Append("<tr>");										
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX]!=null)
//                            t.Append(spacerMed +tab[i,ConstResults.MediaStrategy.LABEL_MEDIA_COLUMN_INDEX].ToString());																						
//                        t.Append("</td>");						
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null )
//                            t.Append( GestionWeb.GetWebWord(1190,webSession.SiteLanguage).ToString());
//                        else if (tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null )
//                            t.Append(GestionWeb.GetWebWord(1189,webSession.SiteLanguage).ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_MARKET_MEDIA_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else if (tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_SECTOR_MEDIA_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(webSession.ComparativeStudy){
//                            t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                            if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                                if(!excel)
//                                    t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                                else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                            }
//                            else t.Append("&nbsp;");
//                            t.Append("</td>");
//                        }
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("</tr>");	
//                        //switchParent="med";
//                    }
//                    // lignes total univers pour chaque support s�lectionn�e
//                    if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null){
//                        t.Append("<tr>");										
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append("&nbsp;");															
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append(GestionWeb.GetWebWord(1188,webSession.SiteLanguage).ToString());				
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_UNIV_MEDIA_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////							t.Append( double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(webSession.ComparativeStudy){
//                            t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                            if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                                if(!excel)
//                                    t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                                else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                            }
//                            else t.Append("&nbsp;");
//                            t.Append("</td>");
//                        }
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_ADVERT_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX]!=null)
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_FIRST_REF_COLUMN_INDEX].ToString());
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" align=right nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX]!=null)
////							t.Append(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString());
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.INVEST_FIRST_REF_COLUMN_INDEX].ToString(),webSession.Unit,false));
//                        else t.Append("&nbsp;");
//                        t.Append("</td >");
//                        t.Append("</tr>");
//                        //switchParent="med";
//                    }				
//                    #endregion
//                }

//                #region personnalisation �l�ments de r�f�rence et concurrents
//                if(tab[i,ConstResults.MediaStrategy.ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null && AdvertiserAccessListArr!=null && AdvertiserAccessListArr.Contains(tab[i,ConstResults.MediaStrategy.ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString().Trim())){
//                    classCss2="p15"; 
//                    classCss="p151";

//                }
//                if(tab[i,ConstResults.MediaStrategy.ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX] != null && CompetitorAdvertiserAccessListArr!=null && CompetitorAdvertiserAccessListArr.Contains(tab[i,ConstResults.MediaStrategy.ID_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString().Trim())){													
//                    if(excel){										
//                        classCss2="p142";
//                        classCss="p143";
//                    }else{
//                        classCss2="p14"; 
//                        classCss="p141";
//                    }
//                }
					
				
//                #endregion

//                #region lignes annonceurs de r�f�rences ou concurrents	

//                //tab[i,ConstResults.MediaStrategy.ID_VEHICLE_COLUMN_INDEX]!=null &&
//                if(tab[i,ConstResults.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null || tab[i,ConstResults.MediaStrategy.TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null ){							
//                    if( ( tab[i,ConstResults.MediaStrategy.ID_CATEGORY_COLUMN_INDEX]==null && tab[i,ConstResults.MediaStrategy.ID_MEDIA_COLUMN_INDEX]==null)
//                        || (tab[i,ConstResults.MediaStrategy.ID_CATEGORY_COLUMN_INDEX]!=null && tab[i,ConstResults.MediaStrategy.ID_MEDIA_COLUMN_INDEX]==null && ( (webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory) || (webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia) ) )
//                        || (tab[i,ConstResults.MediaStrategy.ID_CATEGORY_COLUMN_INDEX]!=null && tab[i,ConstResults.MediaStrategy.ID_MEDIA_COLUMN_INDEX]!=null && (webSession.PreformatedMediaDetail==CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia))
//                        ){
//                        t.Append("<tr>");										
//                        t.Append("<td nowrap>");
//                        t.Append("&nbsp;");															
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss2+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX]!=null )
//                            t.Append(tab[i,ConstResults.MediaStrategy.LABEL_REF_OR_COMPETITOR_ADVERT_COLUMN_INDEX].ToString());					
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if (tab[i,ConstResults.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##") );
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false) );
//                        else if(tab[i,ConstResults.MediaStrategy.TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##") );
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.TOTAL_REF_OR_COMPETITOR_ADVERT_INVEST_COLUMN_INDEX].ToString(),webSession.Unit,false) );
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        if(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX]!=null)
////							t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL);
//                            t.Append(WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,ConstResults.MediaStrategy.PDM_COLUMN_INDEX].ToString(),webSession.Unit,true)+ConstResults.Novelty.PERCENT_SYMBOL);
//                        else t.Append("&nbsp;");
//                        t.Append("</td>");
//                        if(webSession.ComparativeStudy){
//                            t.Append("<td class=\""+classCss+"\" nowrap>");
//                            if(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX]!=null){
//                                if(!excel)
//                                    t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"&nbsp;<img src="+image+">");
//                                else t.Append(double.Parse(tab[i,ConstResults.MediaStrategy.EVOL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ##0.##")+ConstResults.Novelty.PERCENT_SYMBOL+"");
//                            }
//                            else t.Append("&nbsp;");
//                            t.Append("</td>");
//                        }
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append("&nbsp;");	
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" nowra>");
//                        t.Append("&nbsp;");
//                        t.Append("</td>");
//                        //Colonne separation 
//                        if(!excel){
//                            t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//                        }
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("<td class=\""+classCss+"\" nowrap>");
//                        t.Append("&nbsp;");
//                        t.Append("</td>");
//                        t.Append("</tr>");
//                    }
//                }
//                #endregion				
//            }
//            //fin tableau
//            t.Append("</table>");
//            return (t.ToString());
//        }
//            #endregion 
			
//        #region Sortie Excel
//        /// <summary>
//        /// Sortie Excel du tableau de r�sultats
//        /// Cette proc�dure appelle celle qui g�n�re le code HTML :
//        /// <code>t.Append(getIndicatorMediaStrategyHtmlUI(page,tab,webSession,true))</code>
//        /// Puis formatte le r�sultat dans un formulaire excel.
//        /// </summary>
//        /// <param name="page">page qui affiche les r�sultats</param>
//        /// <param name="tab">tabelau de r�sultats</param>
//        /// <param name="webSession">session client</param>
//        /// <param name="excel">bool�en pour sortie html ou excel</param>
//        /// <returns></returns>
//        public static string GetIndicatorMediaStrategyExcelUI(Page page,object[,] tab,WebSession webSession,bool excel){	
		
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

//            #region Rappel des param�tres
//            t.Append(ExcelFunction.GetLogo(webSession));
//            t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,GestionWeb.GetWebWord(1254,webSession.SiteLanguage)));
			

//            #endregion

//            t.Append(GetIndicatorMediaStrategyHtmlUI(page,tab,webSession,true));
//            t.Append(ExcelFunction.GetFooter(webSession));
//            return Convertion.ToHtmlString(t.ToString());
		
//        }

//        #endregion	
//    }



//}

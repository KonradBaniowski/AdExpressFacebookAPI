//#region Informations
//// Auteur: D. V. Mussuma 
//// Date de création: 2/11/2004  
//// Date de modification: 2/11/2004 
////		10/05/2005	K.Shehzad	Chagement d'en tête Excel
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
//    /// IHM: Tableau de la stratégie média des indicateurs
//    /// </summary>
//    public class IndicatorMediaStrategyUI{

//        #region sortie HTML (web)
//        /// <summary>
//        /// Crée le code HTML pour afficher le tableau de la répartition média sur le total de la période 
//        /// contenant les éléments ci-après :
//        /// en ligne :
//        /// -le total famille (en option uniquement en sélection de groupe/variétés) ou le
//        /// total marché (en option)
//        /// -les éléments de références
//        /// -les éléments concurrents 
//        /// en colonne :
//        /// -Les investissements de la période N
//        /// -une PDM (part de marché ) exprimant la répartition media en %
//        /// -une évolution N vs N-1 en % (uniquement dans le cas d'une étude comparative)
//        /// -le 1er annonceur en Keuros uniquement  pour les lignes total produits éventuels
//        /// -la 1ere référence en Keuros uniquement  pour les lignes total produits éventuels
//        /// Sur la dimension support le tableau est décliné de la façon suivante :
//        /// -si plusieurs media, le tableua sera décliné par media
//        /// -si un seul media, le tableau sera décliné par media, catégorie et supports
//        /// </summary>				
//        /// <param name="page">Page qui affiche les statégies média</param>
//        /// <param name="tab">tableau des résultats</param>	
//        /// <param name="webSession">Session du client</param>
//        /// <param name="excel">booléen pour sortie html ou excel</param>	
//        /// <returns>Code HTML</returns>		
//        public static string GetIndicatorMediaStrategyHtmlUI(Page page,object[,] tab,WebSession webSession,bool excel){	
			
//            #region Pas de données à afficher
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
			
//            #region identiant annonceurs référence et concurrents

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

//            #region période étudiée
//            //fin période étudiée
//            DateTime PeriodEndDate = WebFunctions.Dates.GetPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType);
//            #endregion

//            System.Text.StringBuilder t=new System.Text.StringBuilder(10000);
//            //Debut tableau
//            t.Append("<table border=\"0\" cellpadding=\"0\" cellspacing=\"0\" >");
//            #region ligne libéllés
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
//            //Colonne libéllé média
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1246,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"</td>");				
			
//            //colonne libéllé annonceur ou totaux produits
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");				
//            //colonne evolution
//            if(webSession.ComparativeStudy)t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1168,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"/"+(PeriodEndDate.Year-1)+"</td>");				
//            //Colonne separation 
//            if(!excel){
//                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//            }
//            //Colonne libéllé 1er annonceur
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1154,webSession.SiteLanguage)+"</td>");				
//            //colonne investissement 1er annonceur sur période N et par média ( média ou catégorie ou support)
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1246,webSession.SiteLanguage)+"&nbsp;"+PeriodEndDate.Year+"</td>");				
//            //Colonne separation 
//            if(!excel){
//                t.Append("<td class=\"violetBackGround whiteRightLeftBorder\"><img width=1px></td>");
//            }
//            //colonne libéllé 1ere référence
//            t.Append("<td  nowrap  class=\"p2\" >"+GestionWeb.GetWebWord(1155,webSession.SiteLanguage)+"</td>");				
//            //colonne investissement 1ere référence sur période N et par média  ( média  ou catégorie ou support)
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

//                #region ligne total marché ou famille
//                //ligne total marché ou famille	pour plurimedia																									
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
									
//                //Ligne total marché ou famille par media (vehicle)
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

//                    #region lignes totaux par catégorie
//                    // lignes total marché ou famille pour chaque catégorie sélectionnée
				
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
//                    // lignes total univers pour chaque catégorie sélectionnée
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
//                    // lignes total marché ou famille pour chaque support sélectionnée								
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
//                    // lignes total univers pour chaque support sélectionnée
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

//                #region personnalisation éléments de référence et concurrents
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

//                #region lignes annonceurs de références ou concurrents	

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
//        /// Sortie Excel du tableau de résultats
//        /// Cette procédure appelle celle qui génère le code HTML :
//        /// <code>t.Append(getIndicatorMediaStrategyHtmlUI(page,tab,webSession,true))</code>
//        /// Puis formatte le résultat dans un formulaire excel.
//        /// </summary>
//        /// <param name="page">page qui affiche les résultats</param>
//        /// <param name="tab">tabelau de résultats</param>
//        /// <param name="webSession">session client</param>
//        /// <param name="excel">booléen pour sortie html ou excel</param>
//        /// <returns></returns>
//        public static string GetIndicatorMediaStrategyExcelUI(Page page,object[,] tab,WebSession webSession,bool excel){	
		
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);

//            #region Rappel des paramètres
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

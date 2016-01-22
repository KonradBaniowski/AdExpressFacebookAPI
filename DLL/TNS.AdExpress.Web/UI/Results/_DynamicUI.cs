//#region Informations
//// Auteur: G. Facon 
//// Date de création: 09/11/2004 
//// Date de modification: 
////	15/11/2004						Ajout sortie Excel 
////	03/05/2005		K. Shehzad		Changement pour rappel des paramètres pour en tête d'Excel 
////	22/06/2005		B. Masson		Excel des données brutes
////	12/08/2005		G. Facon		Gestion des Exceptions
////	12/10/2005		D. V. Mussuma	Gestion des semaines comparatives
//#endregion


//using System;
//using System.Web.UI;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
//using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.DynamicAnalysis;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebFunctions = TNS.AdExpress.Web.Functions;
//using TNS.AdExpress.Domain.Web.Navigation;
//using PeriodDetailConstantes = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using TNS.FrameWork;
//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// Construit l'interface utilisateur pour l'analyse dynamique
//    /// </summary>
//    public class DynamicUI{
		
//        #region Constantes
//        /// <summary>
//        /// Niveau 1
//        /// </summary>
//        const int productLevel1=1;
//        /// <summary>
//        /// Niveau 2
//        /// </summary>
//        const int productLevel2=2;
//        /// <summary>
//        /// Niveau 3
//        /// </summary>
//        const int productLevel3=3;
//        #endregion

//        #region Sortie HTML
//        /// <summary>
//        /// Génère le code html pour l'alerte concurentielle
//        /// </summary>
//        /// <param name="page">Page qui appele le résultat</param>
//        /// <param name="tab">Tableau de résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Source html du tableau de résultat</returns>
//        internal static string GetHtml(Page page,object[,] tab,WebSession webSession){
		
//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            string classCss="";
//            //string creation="";
//            int oldProduct=-1;
//            //int level=-1;
//            bool endBalise=false;
//            bool oneLevel=true;
//            bool total=false;
//            Int64 idCreation=-1;
//            string totalUnit="";
//            string firstBaliseA="";
//            string endBaliseA="";
//            string tmp="";
//            // Période d'étude comparative
//            string PeriodDateN="";
//            // Période d'étude comparative
//            string PeriodDateN1="";
//            string mediaplan="";			
//            #endregion
			
//            #region Constantes
//            const string L1="acl1";
//            const string L2="acl2";
//            const string L3="acl3";
//            const string P2="p2";
//            #endregion		
			
//            #region Script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openGad")){
//                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openGad", TNS.AdExpress.Web.Functions.Script.OpenGad());
//            }
//            //plan média
//            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());			
//            #endregion
			
//            #region Calcul des périodes
//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            PeriodDateN=PeriodBeginningDate.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");
			
//            PeriodDateN1=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
			
//            // Gestion des semaines
//            if(webSession.DetailPeriod==PeriodDetailConstantes.weekly){			
//                string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,PeriodDetailConstantes.weekly);
//                string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,PeriodDetailConstantes.weekly);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//                TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//                TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//                if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//                }
//                else{
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//                }

//                if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//                }
//                else{
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//                }

//                if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//                    PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//                }else{				
//                    PeriodDateN1=firstPeriod.FirstDay.Date.ToString("dd/MM/yyyy")+"-"+endPeriod.LastDay.Date.ToString("dd/MM/yyyy");				
//                }
//            }
			
//            #endregion
			
//            #region HTML
//            t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;

//            t.Append("\r\n\t<tr height=\"20px\">");
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//            //plan média
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff rowspan=2  nowrap>"+GestionWeb.GetWebWord(150,webSession.SiteLanguage)+"</td>");
//            t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            // Periode N
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+PeriodDateN+"</td>");
//            t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            // Periode N-1
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+PeriodDateN1+"</td>");
//            t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            // Evol
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
//            t.Append("</tr>");

//            // Ligne des support
//            t.Append("\r\n\t<tr height=\"20px\">");
//            //t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
//            //t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");

//            for(i=ResultConstantes.FIRST_MEDIA_INDEX;i<nbColTab;i++){
//                if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                    t.Append("<td nowrap class=\""+P2+"\" bgcolor=#ffffff>"+tab[i,2]+"</td>");	
//                }else{				
//                    t.Append("<td><img src=\"/I/gsep.gif\" width=5 height=100%></td>");
//                    t.Append("<td class=\""+P2+"\" nowrap>"+tab[i,2]+"</td>");
//                    oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                }
//            }
//            t.Append("</tr>");

//            oldProduct=-1;
//            i=ResultConstantes.FIRST_LINE_RESULT_INDEX+3;

//            #region On regarde qu'il y ait au moins deux niveaux 
//            try{ 
//                while(i<nbline && oneLevel==true){
//                    if(	tab[ResultConstantes.LABELL2_INDEX,i]==null || tab[ResultConstantes.LABELL2_INDEX,i].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                        oneLevel=true;
//                        i++;
//                    }
//                    else{
//                        oneLevel=false;
//                    }
//                }			
//            }
//            catch(Exception){oneLevel=true;}
//            #endregion

//            for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
//                for(i=0;i<nbColTab;i++){	
//                    #region switch
//                    switch(i){				
//                        #region Label 1
//                        case  ResultConstantes.LABELL1_INDEX:						
							
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                //level=ResultConstantes.IDL1_INDEX;
//                                if(!oneLevel)classCss=L1;
//                                else classCss=L2;

//                                if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                    tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL1_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                    firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
//                                    endBaliseA="</a>"; 
//                                }else{
//                                    firstBaliseA="";
//                                    endBaliseA=""; 		
//                                }
//                                if((long)tab[ResultConstantes.FIRST_COLUMN_T0_SHOW_INDEX,j]==ResultConstantes.TOTAL_IDENTIFICATION){
//                                    total=true;
//                                    t.Append("\r\n\t<tr align=\"right\" bgcolor=#ffffff height=\"20px\">\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[ResultConstantes.LABELL1_INDEX,j]+endBaliseA+"</td>");
//                                    //lien plan média
//                                    if(total)t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");									
//                                    i=ResultConstantes.FIRST_MEDIA_INDEX-1;	
//                                }
//                                else{
//                                    total=false;
//                                    idCreation= (Int64)tab[ResultConstantes.IDL1_INDEX,j];
//                                    //lien plan média
//                                    if(WebFunctions.ProductDetailLevel.ShowMediaPlanL1(webSession))mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','1');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    else mediaplan="";
//                                    t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[ResultConstantes.LABELL1_INDEX,j]+endBaliseA+"</td>");
//                                    //lien plan média
//                                    if(total)t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");
//                                    else t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
//                                    i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                                }
//                            }else if(tab[i,j]!=null){
//                                if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(j<nbline){										
//                                        i=nbColTab;
//                                    }
//                                }
//                            }							
//                            break;
//                        #endregion

//                        #region Label 2
//                        case ResultConstantes.LABELL2_INDEX:
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                classCss=L2;
//                                if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                    tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL2_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                    firstBaliseA="<a class=\""+classCss+"\" href=\""+tmp+"\"> > ";
//                                    endBaliseA="</a>"; 
//                                }else{
//                                    firstBaliseA="";
//                                    endBaliseA=""; 		
//                                }
//                            //	level=ResultConstantes.IDL2_INDEX;
//                                total=false;
//                                oldProduct=-1;
								
//                                idCreation= (Int64)tab[ResultConstantes.IDL2_INDEX,j];
//                                //lien plan média
//                                if(WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession))mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','2');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                else mediaplan="";
//                                t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA height=\"20px\">\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[ResultConstantes.LABELL2_INDEX,j]+endBaliseA+"</td>");
//                                //	t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA height=\"20px\">\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+tab[ResultConstantes.LABELL2_INDEX,j]+"</td>");
//                                //lien plan média
//                                if(total)t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");
//                                else t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
//                                i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                            }else if(tab[i,j]!=null){
//                                if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(j<nbline){										
//                                        i=nbColTab;
//                                    }
//                                }
//                            }								
//                            break;
//                        #endregion

//                        #region Label 3
//                        case ResultConstantes.LABELL3_INDEX:
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                    classCss=L2;
//                                    tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL1_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                    firstBaliseA="<a  class=\""+classCss+"\" href=\""+tmp+"\"> > ";
//                                    endBaliseA="</a>"; 
//                                }else{
//                                    firstBaliseA="";
//                                    endBaliseA=""; 		
//                                }
//                                //level=ResultConstantes.IDL3_INDEX;
//                                total=false;
//                                oldProduct=-1;
//                                classCss=L3;
//                                idCreation= (Int64)tab[ResultConstantes.IDL3_INDEX,j];
//                                //lien plan média
//                                mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','3');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                t.Append("\r\n\t<tr align=\"right\" height=\"20px\" bgcolor=#E1E0DA onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[ResultConstantes.LABELL3_INDEX,j]+endBaliseA+"</td>");
//                                //lien plan média
//                                if(total)t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");
//                                else t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
//                                i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                            }else if(tab[i,j]!=null){
//                                if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(j<nbline){										
//                                        i=nbColTab;
//                                    }
//                                }
//                            }							
//                            break;
//                        #endregion

//                        #region Données des supports
//                        case ResultConstantes.FIRST_MEDIA_INDEX:							
//                            for(i=ResultConstantes.FIRST_MEDIA_INDEX; i<nbColTab;i++){
//                                if(tab[i,j]!=null){
//                                    endBalise=true;
//                                    total=false;
//                                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==ResultConstantes.EVOL_UNIVERSE_POSITION)
//                                    {
										
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,true);
//                                        if(totalUnit=="+Infini")
//                                        {
//                                            totalUnit="+ <img src=/I/g.gif>";
//                                        }
//                                        else if(totalUnit=="-100,00")
//                                        {
//                                            totalUnit+=" <img src=/I/r.gif>";
//                                        }
//                                        else if(double.Parse(totalUnit)>0)
//                                        {
//                                            totalUnit+=" <img src=/I/g.gif>";
//                                        }
//                                        else if(double.Parse(totalUnit)<0)
//                                        {
//                                            totalUnit+=" <img src=/I/r.gif>";
//                                        }
//                                        else if(totalUnit=="Non Numérique")
//                                        {
//                                            totalUnit="";
//                                        }
//                                        else
//                                        {
//                                            totalUnit+=" <img src=/I/o.gif>";
//                                        }
//                                    }
//                                    else
//                                    {
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                    }
//                                    if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" ||totalUnit.Trim()==",000" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="&nbsp;";
//                                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                                        t.Append("\r\n\t<td class=\""+classCss+"\">"+totalUnit+"</td>");
////										if(totalUnit.Length>0){
////											resultExist=true;
////										}
																				
//                                    }else{
//                                        //t.Append("\r\n\t<td class=\""+classCss+"\" style=\"BORDER-LEFT: #ffffff solid;\" >"+tab[i,j]+"</td>");
//                                        t.Append("<td><img src=\"/I/gsep.gif\" width=5 height=100%></td><td class=\""+classCss+"\">"+totalUnit+"</td>");
//                                        oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                                    }
//                                }
//                            }	
//                            if(endBalise){
//                                t.Append("</tr>");
//                                endBalise=false;
//                            }
//                            break;
//                        #endregion
//                    }
//                    #endregion
					
//                }
//            }
//            t.Append("</table>");			
//            #endregion
		
//            return t.ToString();
//        }

		
//        /// <summary>
//        /// Génère le code html pour la synthèse de l'analyse dynamique
//        /// </summary>
//        /// <param name="page">Page qui appele le résultat</param>
//        /// <param name="tab">Tableau de résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="excel">Vrai si sortie au format excel</param>
//        /// <returns>Source html du tableau de résultat</returns>
//        internal static string GetSynthesisHtml(Page page,object[,] tab,WebSession webSession,bool excel){
			
//            if(tab==null || tab.GetLength(0)==0 || tab.GetLength(1)==0){
//                #region Pas de données à afficher				
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"</div>");					
//                #endregion
//            }

//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            const string P2="p2";
//            const string L1="acl3";
//            string unit="";
//            string classCss=L1;
//            if(excel)classCss="acl31";

//            #region Calcul des périodes
//            // Période d'étude comparative
//            string PeriodDateN="";
//            // Période d'étude comparative
//            string PeriodDateN1="";
//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            PeriodDateN=WebFunctions.Dates.dateToString(PeriodBeginningDate,webSession.SiteLanguage)+"-"+WebFunctions.Dates.dateToString(PeriodEndDate,webSession.SiteLanguage);			
//            PeriodDateN1=WebFunctions.Dates.dateToString(PeriodBeginningDate.Date.AddYears(-1),webSession.SiteLanguage)+"-"+WebFunctions.Dates.dateToString(PeriodEndDate.Date.AddYears(-1),webSession.SiteLanguage);
			
//            // Gestion des semaines
//            if(webSession.DetailPeriod==PeriodDetailConstantes.weekly){			
//                string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,PeriodDetailConstantes.weekly);
//                string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,PeriodDetailConstantes.weekly);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//                TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//                TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//                if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//                }
//                else{
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//                }

//                if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//                }
//                else{
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//                }

//                if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//                    PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//                }else{				
//                    PeriodDateN1=WebFunctions.Dates.dateToString(firstPeriod.FirstDay,webSession.SiteLanguage)+"-"+WebFunctions.Dates.dateToString(endPeriod.LastDay,webSession.SiteLanguage);				
//                }
//            }
			
//            #endregion

//            t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");
			
//            #region En-tête du tableau
//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Fidèles
//            t.Append("<td  bgcolor=#ffffff nowrap rowspan=3 valign=\"middle\">&nbsp;</td>");
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=3 valign=\"middle\">"+GestionWeb.GetWebWord(1241,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en baisse
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff colspan=3  nowrap>"+GestionWeb.GetWebWord(1242,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en développement
//            t.Append("<td  class=\""+P2+"\" nowrap  colspan=3>"+GestionWeb.GetWebWord(1243,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Gagnés
//            t.Append("<td  class=\""+P2+"\" nowrap colspan=3>"+GestionWeb.GetWebWord(1244,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Perdus
//            t.Append("<td  class=\""+P2+"\" nowrap colspan=3>"+GestionWeb.GetWebWord(1245,webSession.SiteLanguage)+"</td>");			
//            t.Append("</tr>");

//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Fidèles Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Fidèles Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en baisse Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Fidèles en baisse Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en développement Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Fidèles en développement Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Gagnés Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Gagnés Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Perdus Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Perdus Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");						
//            t.Append("</tr>");

//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Fidèles N
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN+"</td>");
//            //Fidèles N-1 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap valign=\"middle\">"+PeriodDateN1+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en baisse N
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN+"</td>");
//            //Fidèles en baisse N-1
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN1+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Fidèles en développement N
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN+"</td>");
//            //Fidèles en développement N-1 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN1+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Gagnés N
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN+"</td>");
//            //Gagnés N-1 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN1+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Perdus N
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN+"</td>");
//            //Perdus N-1 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+PeriodDateN1+"</td>");						
//            t.Append("</tr>");
//            #endregion

//            #region Affichage des résultats
//            for(int i=0;i<tab.GetLength(0);i++){
//                if(i!=ResultConstantes.BRANDS_LINE_INDEX && i!=ResultConstantes.AGENCY_LINE_INDEX && i!=ResultConstantes.AGENCIES_GROUPS_LINE_INDEX ||(i==ResultConstantes.BRANDS_LINE_INDEX && webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)!=null)
//                    || ((i==ResultConstantes.AGENCY_LINE_INDEX || i==ResultConstantes.AGENCIES_GROUPS_LINE_INDEX) && webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_MEDIA_AGENCY)!=null)
//                    ){
//                    t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >");

//                    for(int j=0;j<tab.GetLength(1);j++){	
//                        //Colonne libellé produits
//                        switch(j){
//                            case ResultConstantes.PRODUCTS_LABEL_COLUMN_INDEX :
//                                if(tab[i,j]!=null)t.Append("\r\n\t\t<td align=\"left\" class=\""+P2+"\" nowrap>"+tab[i,j].ToString()+"</td>");
//                                else t.Append("\r\n\t\t<td align=\"left\" class=\""+P2+"\" nowrap>&nbsp;</td>");
//                                break;
//                                //Colonne nombre de produits						
//                            case ResultConstantes.NUMBER_LOYAL_COLUMN_INDEX:
//                            case ResultConstantes.NUMBER_LOYAL_DECLINE_COLUMN_INDEX:
//                            case ResultConstantes.NUMBER_LOYAL_RISE_COLUMN_INDEX:						
//                            case ResultConstantes.NUMBER_WON_COLUMN_INDEX:
//                            case ResultConstantes.NUMBER_LOST_COLUMN_INDEX:
//                                if(tab[i,j]!=null)t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ###")+"</td>");											 
//                                else t.Append("\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;</td>");
//                                break;
//                            case ResultConstantes.N_LOST_COLUMN_INDEX:
//                            case ResultConstantes.N_LOYAL_COLUMN_INDEX: 
//                            case ResultConstantes.N_LOYAL_DECLINE_COLUMN_INDEX: 
//                            case ResultConstantes.N_LOYAL_RISE_COLUMN_INDEX: 
//                            case ResultConstantes.N_WON_COLUMN_INDEX: 
//                            case ResultConstantes.N1_LOST_COLUMN_INDEX:
//                            case ResultConstantes.N1_LOYAL_COLUMN_INDEX: 
//                            case ResultConstantes.N1_LOYAL_DECLINE_COLUMN_INDEX: 
//                            case ResultConstantes.N1_LOYAL_RISE_COLUMN_INDEX: 
//                            case ResultConstantes.N1_WON_COLUMN_INDEX: 
//                                if(tab[i,j]!=null)unit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,false);						
//                                else unit="";
//                                if(unit.Trim().Length==0 || unit.Trim()==",00" ||unit.Trim()==",000" || unit.Trim()=="0,00" || unit=="Non Numérique" || unit=="00 H 00 M 00 S")unit="";							
//                                t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+unit+"</td>");									
//                                break;							
//                            default :
//                                break;
//                        }

//                        switch(j){
//                            case ResultConstantes.N1_LOYAL_COLUMN_INDEX: 
//                            case ResultConstantes.N1_LOYAL_DECLINE_COLUMN_INDEX: 
//                            case ResultConstantes.N1_LOYAL_RISE_COLUMN_INDEX: 
//                            case ResultConstantes.N1_WON_COLUMN_INDEX: 								
//                                if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");		
//                                break;							
//                            default :
//                                break;
//                        }

//                    }
//                    t.Append("</tr>");
//                }
//            }

//            #endregion

//            t.Append("</table>");

//            #region Debug: voir le tableau			
//            #if(DEBUG)
////				int i,j;
////				string HTML="<html><table border=1><tr>";
////				for(i=0;i<tab.GetLength(0);i++){
////					for(j=0;j<tab.GetLength(1);j++){
////						if(tab[i,j]!=null)HTML+="<td>"+tab[i,j].ToString()+"</td>";
////						else HTML+="<td>&nbsp;</td>";
////					}
////					HTML+="</tr><tr>";
////				}
////				HTML+="</tr></table></html>";
////			return HTML;
//            #endif
//            #endregion

//            return t.ToString();
//        }
//        #endregion

//        #region Sortie Excel
//        /// <summary>
//        /// Génère le code html pour l'alerte concurentielle
//        /// </summary>
//        /// <param name="tab">Tableau de résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Source html du tableau de résultat</returns>
//        internal static string GetExcel(object[,] tab,WebSession webSession){

//            #region Constantes
//        //	const string L1="acl1";
//        //	const string L2="acl2";
//        //	const string L3="acl3";
//            const string P2="p2";
//            #endregion

//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            string classCss="";
//            int oldProduct=-1;
//            bool endBalise=false;
//            string totalUnit="";
//            string tmp="";
//            // Période d'étude comparative
//            string PeriodDateN="";
//            // Période d'étude comparative
//            string PeriodDateN1="";
//            #endregion

//            #region Calcul des périodes
//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            PeriodDateN=PeriodBeginningDate.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");
			
//            PeriodDateN1=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
			
//            // Gestion des semaines
//            if(webSession.DetailPeriod==PeriodDetailConstantes.weekly){
//                string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,PeriodDetailConstantes.weekly);
//                string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,PeriodDetailConstantes.weekly);
//                //TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
				
//                TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//                TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//                TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//                if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//                }
//                else{
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//                }

//                if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//                }
//                else{
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//                }

//                if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//                    PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//                }else{				
//                    PeriodDateN1=firstPeriod.FirstDay.Date.ToString("dd/MM/yyyy")+"-"+endPeriod.LastDay.Date.ToString("dd/MM/yyyy");				
//                }
//            }
			
//            #endregion

//            #region Rappel des paramètres
//            // Paramètres du tableau
//            t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,true,true,""));
////			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(464,webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage)+" - "
////				+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			// Type de resultat
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(1493,webSession.SiteLanguage)+" : "+stringTypeResult(webSession)+"</td>");
////			t.Append("</tr>");			
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(1313,webSession.SiteLanguage)+" "+GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.XLSUnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+" "+stringLevelProduct(webSession)+"</td>");
////			t.Append("</tr>");
////
////			t.Append("<tr>");
////			t.Append("<td>&nbsp;</td>");
////			t.Append("</tr>");
////
////
////			t.Append("</table>");
//            #endregion

//            #region Excel
//            t.Append("<table  border=0 cellpadding=0 cellspacing=0 >");

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;

//            t.Append("\r\n\t<tr height=\"20px\">");
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+PeriodDateN+"</td>");
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+PeriodDateN1+"</td>");
//            t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
//            t.Append("</tr>");

//            // Première ligne
//            t.Append("\r\n\t<tr height=\"40px\">");
//            //t.Append("<td class=\"p2\" bgcolor=#ffffff>"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
//            //t.Append("<td class=\"p2\" bgcolor=#ffffff>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");

//            for(i=ResultConstantes.FIRST_MEDIA_INDEX;i<nbColTab;i++){
//                if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                    t.Append("<td class=\"p2\" bgcolor=#ffffff>"+tab[i,2]+"</td>");	
//                }else{
//                    t.Append("<td  style=\"BORDER-LEFT: #ffffff solid\" class=\"p2\" >"+tab[i,2]+"</td>");
//                    oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                }
//            }
//            t.Append("</tr>");

//            oldProduct=-1;
//            for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
//                for(i=0;i<nbColTab;i++){

//                    #region switch
//                    switch(i){	
					
//                        #region Label 1
//                        case  ResultConstantes.LABELL1_INDEX:						
							
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                if((long)tab[ResultConstantes.FIRST_COLUMN_T0_SHOW_INDEX,j]==ResultConstantes.TOTAL_IDENTIFICATION){
//                                    classCss="acl11";
//                                    t.Append("\r\n\t<tr height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">"+tab[ResultConstantes.LABELL1_INDEX,j]+"</td>");
//                                    i=ResultConstantes.FIRST_MEDIA_INDEX-1;	
//                                }
//                                else{
//                                    classCss="acl11";							
//                                    t.Append("\r\n\t<tr  height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">"+tab[ResultConstantes.LABELL1_INDEX,j]+"</td>");
//                                    i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                                }	
//                            }else if(tab[i,j]!=null){
//                                if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(j<nbline){										
//                                        i=nbColTab;
//                                    }
//                                }
//                            }				 
//                            break;
//                        #endregion

//                        #region Label 2
//                        case ResultConstantes.LABELL2_INDEX:
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                classCss="acl21";
//                                t.Append("\r\n\t<tr  height=\"20px\" >\r\n\t\t<td class=\""+classCss+"\">&nbsp;&nbsp;&nbsp;"+tab[ResultConstantes.LABELL2_INDEX,j]+"</td>");
//                                i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                            }else if(tab[i,j]!=null){
//                                if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(j<nbline){										
//                                        i=nbColTab;
//                                    }
//                                }
//                            }	
//                            break;
//                        #endregion

//                        #region Label 3
//                        case ResultConstantes.LABELL3_INDEX:
//                            if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                classCss="acl31";
//                                t.Append("\r\n\t<tr  height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[ResultConstantes.LABELL3_INDEX,j]+"</td>");
//                                i=ResultConstantes.FIRST_MEDIA_INDEX-1;
//                            }
//                            break;
//                        #endregion

//                        #region Données des supports
//                        case ResultConstantes.FIRST_MEDIA_INDEX:
//                            for(i=ResultConstantes.FIRST_MEDIA_INDEX; i<nbColTab;i++){
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    endBalise=true;
//                                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==ResultConstantes.EVOL_UNIVERSE_POSITION)
//                                    {										
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,true);
//                                        if(totalUnit=="+Infini")
//                                        {
//                                            totalUnit="+";
//                                        }
//                                        else if(totalUnit=="-100,00")
//                                        {
//                                            totalUnit+="";
//                                        }										
//                                        else if(totalUnit=="Non Numérique")
//                                        {
//                                            totalUnit="";
//                                        }																				
//                                    }
//                                    else
//                                    {
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                    }
//                                    if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
//                                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                                        t.Append("\r\n\t<td class=\""+classCss+"\" style=\"BORDER-LEFT: #ffffff solid; Text-align:right\">"+totalUnit+"</td>");
																				
//                                    }else{
//                                        t.Append("\r\n\t<td class=\""+classCss+"\" style=\"BORDER-LEFT: #ffffff solid; Text-align:right\" >"+totalUnit+"</td>");
//                                        oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                                    }
//                                }
//                            }	
//                            if(endBalise){
//                                t.Append("</tr>");
//                                endBalise=false;
//                            }
//                            break;
//                        #endregion
//                    }	
//                    #endregion

//                }
//            }
//            t.Append("</table>");
//            #endregion

//            return t.ToString();
//        }

//        /// <summary>
//        /// Génère le code excel pour la synthèse de l'analyse dynamique
//        /// </summary>
//        /// <param name="page">Page qui appele le résultat</param>
//        /// <param name="tab">Tableau de résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Source html du tableau de résultat</returns>
//        internal static string GetSynthesisExcel(Page page,object[,] tab,WebSession webSession){
//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);			
//            #endregion

//            #region Rappel des paramètres
//            // Paramètres du tableau
//            t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,true,true,""));
//            #endregion

//            #region Résultat
//            t.Append(Convertion.ToHtmlString(GetSynthesisHtml(page,tab,webSession,true)));
//            #endregion

//            return t.ToString();
//        }
//        #endregion

//        #region Sortie Excel des données brutes
//        /// <summary>
//        /// Génère le code html pour l'alerte concurentielle
//        /// </summary>
//        /// <param name="tab">Tableau de résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Source html du tableau de résultat</returns>
//        internal static string GetRawExcel(object[,] tab,WebSession webSession){
			
//            #region Constantes
//            //	const string L1="acl1";
//            //	const string L2="acl2";
//            //	const string L3="acl3";
//            //	const string P2="p2";
//            #endregion

//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            //string classCss="";
//            int oldProduct=-1;
//            //bool endBalise=false;
//            string totalUnit="";
//            string tmp="";
//            // Période d'étude comparative
//            string PeriodDateN="";
//            // Période d'étude comparative
//            string PeriodDateN1="";
//            #endregion

//            #region Calcul des périodes
//            DateTime PeriodBeginningDate = WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType);
//            DateTime PeriodEndDate = WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType);
//            PeriodDateN=PeriodBeginningDate.ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.ToString("dd/MM/yyyy");
			
//            PeriodDateN1=PeriodBeginningDate.Date.AddYears(-1).ToString("dd/MM/yyyy")+"-"+PeriodEndDate.Date.AddYears(-1).ToString("dd/MM/yyyy");
			
//            // Gestion des semaines
//            if(webSession.DetailPeriod==PeriodDetailConstantes.weekly){
//                string beginningPeriodN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodBeginningDate,PeriodDetailConstantes.weekly);
//                string endPeriodEndN1 = WebFunctions.Dates.GetPreviousYearDate(webSession.PeriodEndDate,PeriodDetailConstantes.weekly);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek testWeek=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),1);

//                TNS.FrameWork.Date.AtomicPeriodWeek testWeekBeginPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),1);
				
//                TNS.FrameWork.Date.AtomicPeriodWeek endPeriod;
//                TNS.FrameWork.Date.AtomicPeriodWeek firstPeriod;
//                if(testWeek.NumberWeekInYear<int.Parse(endPeriodEndN1.Substring(4,2))){
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),testWeek.NumberWeekInYear);
//                }
//                else{
//                    endPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(endPeriodEndN1.Substring(0,4)),int.Parse(endPeriodEndN1.Substring(4,2)));
//                }

//                if(testWeekBeginPeriod.NumberWeekInYear<int.Parse(beginningPeriodN1.Substring(4,2))){
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek((int.Parse(beginningPeriodN1.Substring(0,4))+1),01);
//                }
//                else{
//                    firstPeriod=new TNS.FrameWork.Date.AtomicPeriodWeek(int.Parse(beginningPeriodN1.Substring(0,4)),int.Parse(beginningPeriodN1.Substring(4,2)));
					
//                }

//                if(int.Parse(endPeriodEndN1.Substring(4,2))==53 &&  int.Parse(beginningPeriodN1.Substring(4,2))==53){
//                    PeriodDateN1=GestionWeb.GetWebWord(1530,webSession.SiteLanguage);
//                }else{				
//                    PeriodDateN1=firstPeriod.FirstDay.Date.ToString("dd/MM/yyyy")+"-"+endPeriod.LastDay.Date.ToString("dd/MM/yyyy");				
//                }
//            }
			
//            #endregion

//            #region Nombre de niveaux
//            int nbLevels=WebFunctions.ProductDetailLevel.GetLevelNumber(webSession);
//            string levels = WebFunctions.ProductDetailLevel.LevelProductToExcelString(webSession);
//            string[] levelsTab = levels.Split('/');
//            #endregion

//            #region Rappel des paramètres
//            t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,true,true,""));
//            #endregion

//            #region Excel
//            t.Append("<table border=1 cellpadding=0 cellspacing=0 >");

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;

//            #region Première ligne
//            try{
//                t.Append("<tr>");
//                t.Append("<td colspan="+nbLevels+"></td>");
//                t.Append("<td colspan="+nbMedia+" >"+PeriodDateN+"</td>");
//                t.Append("<td colspan="+nbMedia+" >"+PeriodDateN1+"</td>");
//                t.Append("<td colspan="+nbMedia+" >"+GestionWeb.GetWebWord(1212,webSession.SiteLanguage)+"</td>");
//                t.Append("</tr>");
//                t.Append("<tr>");
//                for(int k=0;k<nbLevels;k++){
//                    t.Append("<td>"+levelsTab[k].ToString()+"</td>");
//                }
//                for(i=ResultConstantes.FIRST_MEDIA_INDEX;i<nbColTab;i++){
//                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                        t.Append("<td>"+tab[i,2]+"</td>");	
//                    }else{
//                        t.Append("<td>"+tab[i,2]+"</td>");
//                        oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                    }
//                }
//                t.Append("</tr>");
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.DynamicUIException("Impossible de contruire la première ligne du tableau dans le fichier Excel des données brutes",err));
//            }
//            #endregion

//            #region Résultat
//            oldProduct=-1;
//            try{

////				for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
////					t.Append("<tr>");
////					for(i=0;i<nbColTab;i++){
////						t.Append("<td>"+tab[i,j]+"</td>");
////					}
////					t.Append("</tr>");
////				}

//                string oldTextLevel1="";
//                string oldTextLevel2="";
//                string oldTextLevel3="";
//                for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
//                    if(tab[0,j]==null || (tab[0,j]!=null && tab[0,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine))){
//                        t.Append("<tr>");
//                        for(i=0;i<nbColTab;i++){
//                            switch(i){	
//                                case  ResultConstantes.LABELL1_INDEX:
//                                    if(tab[i,j]!=null){
//                                        t.Append("<td>"+tab[ResultConstantes.LABELL1_INDEX,j]+"</td>");
//                                        oldTextLevel1 = tab[ResultConstantes.LABELL1_INDEX,j].ToString();
//                                        oldTextLevel2=null;
//                                    }
//                                    else t.Append("<td>"+oldTextLevel1+"</td>");
//                                    break;

//                                case  ResultConstantes.LABELL2_INDEX:
//                                    if(tab[i,j]!=null ){
//                                        t.Append("<td>"+tab[ResultConstantes.LABELL2_INDEX,j]+"</td>");
//                                        oldTextLevel2 = tab[ResultConstantes.LABELL2_INDEX,j].ToString();
//                                    }
//                                    else{
//                                        if(oldTextLevel2!=null){
//                                            t.Append("<td>"+oldTextLevel2+"</td>");
//                                        }
//                                        else{ 
//                                            // Libellé TOTAL
//                                            if(nbLevels>1)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
//                                        }
//                                    }
//                                    break;

//                                case  ResultConstantes.LABELL3_INDEX:
//                                    if(tab[i,j]!=null ){
//                                        t.Append("<td>"+tab[ResultConstantes.LABELL3_INDEX,j]+"</td>");
//                                        oldTextLevel3 = tab[ResultConstantes.LABELL3_INDEX,j].ToString();
//                                    }
//                                    else {
//                                        // Libellé TOTAL
//                                        if(nbLevels>2)t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
//                                    }
//                                    break;

//                                case ResultConstantes.FIRST_MEDIA_INDEX:
//                                    for(i=ResultConstantes.FIRST_MEDIA_INDEX; i<nbColTab;i++){
//                                        if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==ResultConstantes.EVOL_UNIVERSE_POSITION) {										
//                                            totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,true);
//                                            if(totalUnit=="+Infini") {
//                                                //totalUnit="+";
//                                                totalUnit="0";
//                                            }
//                                            else if(totalUnit=="-100,00") {
//                                                //totalUnit+="";
//                                                totalUnit+="0";
//                                            }										
//                                            else if(totalUnit=="Non Numérique") {
//                                                //totalUnit="";
//                                                totalUnit="0";
//                                            }																				
//                                        }
//                                        else {
//                                            totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                        }
//                                        if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="0";
//                                        if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                                            t.Append("<td>"+totalUnit+"</td>");
																			
//                                        }else{
//                                            t.Append("<td>"+totalUnit+"</td>");
//                                            oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                                        }
//                                    }	
//                                    break;
//                            }
//                        }
//                        t.Append("</tr>");
//                    }
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.DynamicUIException("Impossible de construire le tableau de résultat dans le fichier Excel des données brutes",err));
//            }
//            #endregion

//            t.Append("</table>");
//            #endregion

//            return t.ToString();
//        }
//        #endregion
		
//    }
//}

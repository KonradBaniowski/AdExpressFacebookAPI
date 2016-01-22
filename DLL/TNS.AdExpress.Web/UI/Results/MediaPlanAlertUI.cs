//#region Informations
//// Auteur: G. Facon 
//// Date de création: 07/06/2004  
//// Date de modification:
////	22/07/2004  G. Ragneau
////	29/04/2005	G. Ragneau
////	10/05/2005	K.Shehzad	Chagement d'en tête Excel
////	12/08/2005	G. Facon	Nom de variables
//#endregion

//using System.Web;
//using System;
//using System.Web.UI;
//using System.Globalization;
//using TNS.AdExpress.Constantes.FrameWork.Results;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using TNS.FrameWork;
//using TNS.FrameWork.Date;
//using WebConstantes = TNS.AdExpress.Constantes.Web;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using WebFunctions = TNS.AdExpress.Web.Functions;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using TNS.AdExpress.Web.Functions;
//using TNS.AdExpress.Web.Core;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Domain.Web;

//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// Construit l'interface utilisateur pour les alertes plan media ou les zooms plan media
//    /// </summary>
//    public class MediaPlanAlertUI{

//        #region Sortie HTML (Web)

//        #region détail support date de la session
//        /// <summary>
//        /// Génère le code HTML pour afficher un calendrier d'action détaillé en jour entre les dates de la session.
//        /// Elles se base sur un tableau contenant les données
//        /// tab vide : message "Pas de données" + bouton retour
//        /// tab non vide:
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
//        ///		Génération du code HTML des entêtes de colonne
//        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <param name="page">Page qui affiche le Plan Média</param>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code Généré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
//        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
//        /// </remarks>
//        public static string GetMediaPlanAlertWithMediaDetailLevelHtmlUI(Page page,object[,] tab,WebSession webSession){
		
//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0){
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
//                    +"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
//            }
//            #endregion

//            #region Variables
//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            //int nbColYear = 0;
//            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. 
//            int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ

//            string classe;
//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            //MAJ GR : FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            //fin MAJ
//            string currentCategoryName="tit";
//            string totalUnit="";
//            int colorItemIndex=1;
//            int colorNumberToUse=0;
//            int sloganIndex=-1;
//            //webSession.SloganColors.Clear();
//            try{webSession.SloganColors.Add((Int64)0,"pc0");}catch(System.Exception){}
//            string presentClass="p4";
//            string extendedClass="p5";
//            string stringItem="&nbsp;";

//            #endregion

//            #region On calcule la taille de la colonne Total
//            int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
//            int nbSpace=(nbtot-1)/3;
//            int nbCharTotal=nbtot+nbSpace-5;
//            if(nbCharTotal<5)nbCharTotal=0;
//            #endregion
			
//            #region script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openCreation", WebFunctions.Script.OpenCreation());
//            #endregion

//            #region Début du tableau
//            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//            t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            for(int h=0;h<nbCharTotal;h++){
//                t.Append("&nbsp;");
//            }
//            t.Append("</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
//                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+tab[0,k].ToString()+"</td>");
//            }
//            //fin MAJ
//            #endregion

//            #region Période
//            string dateHtml="<tr>";
//            string headerHtml="";
//            //DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
//            DateTime currentDay =DateString.YYYYMMDDToDateTime((string)tab[0,FIRST_PERIOD_INDEX]);
//                //new DateTime(int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(0,4)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(4,2)),int.Parse(tab[0,FIRST_PERIOD_INDEX].ToString().Substring(6,2)));
//            int previousMonth = currentDay.Month;
//            currentDay = currentDay.AddDays(-1);
//            int nbPeriodInMonth=0;
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                currentDay = currentDay.AddDays(1);
//                if (currentDay.Month!=previousMonth){
//                    if (nbPeriodInMonth>=8){
//                        headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
//                            +Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
//                            +"</td>";
//                    }
//                    else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
//                             +"&nbsp"
//                             +"</td>";
//                    nbPeriodInMonth=0;
//                    previousMonth = currentDay.Month;
//                }
//                nbPeriodInMonth++;
//                dateHtml+="<td class=\"pp\">&nbsp;"+currentDay.ToString("dd") +"&nbsp;</td>";
//            }
//            if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
//                                       +Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
//                                       +"</td>";
//            else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"pt\" align=center>"
//                     +"&nbsp"
//                     +"</td>";
//            dateHtml += "</tr>";

//            string dayClass="";
//            dateHtml+="\r\n\t<tr>";
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)));
//                if(dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ){
//                    dayClass="pdw";
//                }
//                else{
//                    dayClass="pd";
//                }	
//                CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters((new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)))), cultureInfo,1) + "</td>";
//            }
//            dateHtml+="\r\n\t</tr>";

//            headerHtml += "</tr>";
//            t.Append(headerHtml);
//            t.Append(dateHtml);
//            #endregion

//            #region Calendrier d'actions
//            i=0;
//            try{		
//                sloganIndex=GetSloganIdIndex(webSession);
//                for(i=1;i<nbline;i++){

//                    #region Gestion des couleurs des accroches
//                    if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null){
//                        if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])){
//                            colorNumberToUse=(colorItemIndex%30)+1;
//                            webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString());
//                            colorItemIndex++;
//                        }
//                        presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
//                        extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
//                        stringItem="x";
//                    }
//                    else{
//                        presentClass="p4";
//                        extendedClass="p5";
//                        stringItem="&nbsp;";
//                    }
//                    #endregion

//                    for(j=0;j<nbColTab;j++){
//                        switch(j){
//                            #region Level 1
//                            case DetailledMediaPlan.L1_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    //vérification que la catégorie n est pas les chaines thematiques
									
//                                    if(i==DetailledMediaPlan.TOTAL_LINE_INDEX) classe="L0";
//                                    else{

//                                        classe="L1";
//                                        if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]!=null)
//                                            tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+",-1,-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        else
//                                            tmpHtml="";
//                                    }
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\""+classe+"nb\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+10+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+10+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                #endregion

//                            #region Level 2
//                            case DetailledMediaPlan.L2_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    //vérification que la catégorie n est pas les chaines thematiques
//                                    if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+",-1,-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]+"</td><td class=\"L2nb\">"+totalUnit+"</td><td class=\"L2nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L2\">"+tmpHtml+"</td>");
//                                    currentCategoryName=tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\"L2nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+9+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 3
//                            case DetailledMediaPlan.L3_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    //vérification que la catégorie n est pas les chaines thematiques
//                                    if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+",-1,-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]+"</td><td class=\"L3nb\">"+totalUnit+"</td><td class=\"L3nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L3\">"+tmpHtml+"</td>");
//                                    currentCategoryName=tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX].ToString();
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\"L3nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+8+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 4
//                            case DetailledMediaPlan.L4_COLUMN_INDEX: 
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
//                                if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null)
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]+"</td><td class=\"L4nb\">"+totalUnit+"</td><td class=\"L4nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L4\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L4_ID_COLUMN_INDEX]+",-1,1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                else
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L4\">&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]+"</td><td class=\"L4nb\">"+totalUnit+"</td><td class=\"L4nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L4\"></td>");
//                                //MAJ GR : totaux par années si nécessaire
//                                for(k=1; k<=nbColYear; k++) {
//                                    t.Append("<td class=\"L4nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");
//                                }
//                                //fin MAJ
//                                j=j+7+nbColYear;//nbColYear ajouter GF
//                                break;
//                            #endregion
//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(MediaPlanItem)){
//                                    switch(((MediaPlanItem)tab[i,j]).GraphicItemType){
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
//                                            t.Append("<td class=\""+presentClass+"\">"+stringItem+"</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
//                                            t.Append("<td class=\""+extendedClass+"\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    t.Append("</tr>");
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.MediaPlanUIException("erreur i="+i+",j="+j,err));
//            }
//            //t.Append("</table></td></tr></table>");			
//            t.Append("</table>");
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #region Avec détail support zoom
//        /// <summary>
//        /// Génère le code HTML pour afficher un calendrier d'action détaillé en jour sur UNE période.
//        /// Elles se base sur un tableau contenant les données
//        /// tab vide : message "Pas de données" + bouton retour
//        /// tab non vide:
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
//        ///		Génération du code HTML des entêtes de colonne
//        ///		Génération du code HTML des périodes (période suivante, période courante, période suivante, intitulés d'une unité de période
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <param name="page">Page qui affiche le Plan Média</param>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
//        /// <param name="url">Lien vers la pge elle-même. Permet de gérer les flèches "Période suivante", "Période précédente"</param>
//        /// <returns>Code Généré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
//        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
//        /// </remarks>
//        public static string GetMediaPlanAlertWithMediaDetailLevelHtmlUI(Page page,object[,] tab,WebSession webSession, string zoomDate, string url){
			
//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0){
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
//                    +"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
//            }
//            #endregion

//            #region Variables

//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            int nbColYear = 0;
//            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ
			
//            string classe;
//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            //fin MAJ
//            string currentCategoryName="tit";
//            //string classCss="";
//            string totalUnit="";

//            #endregion

//            #region On calcule la taille de la colonne Total
//            int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
//            int nbSpace=(nbtot-1)/3;
//            int nbCharTotal=nbtot+nbSpace-5;
//            if(nbCharTotal<5)nbCharTotal=0;
//            #endregion
			
//            #region script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openCreation", WebFunctions.Script.OpenCreation());
//            #endregion

//            #region debut Tableau
//            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"250px\" class=\"pt\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//            t.Append("\r\n\t\t<td rowspan=3 class=\"pt\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            for(int h=0;h<nbCharTotal;h++){
//                t.Append("&nbsp;");
//            }
//            t.Append("</td>");
			
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");

//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
//                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"pt\">"+tab[0,k].ToString()+"</td>");
//                for(int h=0;h<nbCharTotal+5;h++){
//                    t.Append("&nbsp;");
//                }
//                t.Append("</td>");
//            }
//            //fin MAJ

//            #endregion

//            #region Période précédente
//            if (webSession.PeriodBeginningDate != zoomDate) {
//                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
//                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//                }
//                else{
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyLeft\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//                    tmp.SubWeek(1);
//                    if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//                    t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//                }
//            }
//            else
//                t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
//            #endregion

//            #region Entête de période
//            t.Append("\r\n\t\t\t\t\t\t<td colspan=\""+(nbColTab-FIRST_PERIOD_INDEX-2)+"\" class=\"pa\" align=center>"
//                +Dates.getPeriodTxt(webSession, zoomDate)
//                +"</td>");
//            #endregion

//            #region Période suivante
//            if (webSession.PeriodEndDate != zoomDate){
//                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
//                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//                }
//                else{
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//                    tmp.Increment();
//                    if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//                    t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//                }
//            }
//            else
//                t.Append("\r\n\t\t\t\t\t\t<td class=\"pyRight\">&nbsp;</td>");
//            t.Append("</tr><tr>");
//            #endregion

//            #region Périodes
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){

//                t.Append("<td class=\"pp\">&nbsp;"+((string)tab[0,j]).Substring(6,2)+"&nbsp;</td>");
//            }

//            string dayClass="";
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            t.Append("\r\n\t</tr><tr>");
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = new DateTime(int.Parse(tab[0, j].ToString().Substring(0, 4)), int.Parse(tab[0, j].ToString().Substring(4, 2)), int.Parse(tab[0, j].ToString().Substring(6, 2)));
//                if (dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ) {
//                    dayClass="pdw";
//                }
//                else{
//                    dayClass="pd";
//                }	
//                t.Append("<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>");
//            }
//            t.Append("\r\n\t</tr>");
//            #endregion

//            #region Calendrier d'action
//            i=0;
//            try{
//                for(i=1;i<nbline;i++){
//                    for(j=0;j<nbColTab;j++){
//                        switch(j){

//                            #region Level 1
//                            case DetailledMediaPlan.L1_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string  tmpHtml="";
//                                    if(i==DetailledMediaPlan.TOTAL_LINE_INDEX) classe="L0";
//                                    else{
//                                        classe="L1";
//                                        if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]!=null)
//                                            tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+",-1,-1,-1,-1,1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        else
//                                            tmpHtml="";
//                                    }									
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+10+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+10+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 2
//                            case DetailledMediaPlan.L2_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+",-1,-1,-1,1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L2\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]+"</td><td class=\"L2nb\">"+totalUnit+"</td><td class=\"L2nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L2\">"+tmpHtml+"</td>");
//                                    currentCategoryName=tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX].ToString();
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\"L2nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+9+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 3
//                            case DetailledMediaPlan.L3_COLUMN_INDEX:
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
//                                if (tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX] != null)
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]+"</td><td class=\"L3nb\">"+totalUnit+"</td><td class=\"L3nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L3\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L1_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L2_ID_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.DetailledMediaPlan.L3_ID_COLUMN_INDEX]+",-1,-1,1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                else
//                                    t.Append("\r\n\t<tr>\r\n\t\t<td class=\"L3\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]+"</td><td class=\"L3nb\">"+totalUnit+"</td><td class=\"L3nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"L3\"></td>");
//                                //MAJ GR : totaux par années si nécessaire
//                                for(k=1; k<=nbColYear; k++){
//                                    t.Append("<td class=\"L3nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
//                                }
//                                //fin MAJ

//                                //aller aux colonnes du calendrier d'action
//                                j=j+8;
//                                break;
//                            #endregion
//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(MediaPlanItem)){
//                                    switch(((MediaPlanItem)tab[i,j]).GraphicItemType){
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
//                                            t.Append("<td class=\"p4\">&nbsp;</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
//                                            t.Append("<td class=\"p5\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    t.Append("</tr>");
//                }
//            }
//            catch(System.Exception e){
//                throw(new System.Exception("erreur i="+i+",j="+j,e));
//            }	
//            t.Append("</table>");
			
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #region Sans détail support zoom
//        /// <summary>
//        /// Génère le code HTML pour afficher un calendrier d'action détaillé en jour sur UNE période.
//        /// Elles se base sur un tableau contenant les données
//        /// tab vide : message "Pas de données" + bouton retour
//        /// tab non vide:
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
//        ///		Génération du code HTML des entêtes de colonne
//        ///		Génération du code HTML des périodes (période suivante, période courante, période suivante, intitulés d'une unité de période
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <param name="page">Page qui affiche le Plan Média</param>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="zoomDate">Période à prendre en compte (un mois ou une semaine)</param>
//        /// <param name="url">Lien vers la pge elle-même. Permet de gérer les flèches "Période suivante", "Période précédente"</param>
//        /// <returns>Code Généré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
//        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
//        /// </remarks>
//        public static string GetMediaPlanAlertUI(Page page,object[,] tab,WebSession webSession, string zoomDate, string url){
			
//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0){
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
//                    +"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
//            }
//            #endregion

//            #region Variables

//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            int nbColYear = 0;
//            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ
			
//            string classe;
//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            //fin MAJ
//            bool premier=true;
//            string currentCategoryName="tit";
//            //string classCss="";
//            const string PLAN_MEDIA_1_CLASSE="p6";
//            const string PLAN_MEDIA_2_CLASSE="p7";
//            const string PLAN_MEDIA_NB_1_CLASSE="p8";
//            const string PLAN_MEDIA_NB_2_CLASSE="p9";
//            string totalUnit="";

//            #endregion

//            #region On calcule la taille de la colonne Total
//            int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
//            int nbSpace=(nbtot-1)/3;
//            int nbCharTotal=nbtot+nbSpace-5;
//            if(nbCharTotal<5)nbCharTotal=0;
//            #endregion
			
//            #region script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openCreation", WebFunctions.Script.OpenCreation());
//            // On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
//            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
//            #endregion

//            #region debut Tableau
//            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("\r\n\t\t<td rowspan=3 width=\"250px\" class=\"p2\">");
//            t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
//            t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
//            t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
//            t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
//            t.Append("\n\t\t\t\t\t<td  bgcolor=#ffffff class=\"p2\"  width=1px></td>");
//            t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
//            t.Append("\r\n\t\t</td>");
//            //t.Append("\r\n\t\t<td rowspan=\"2\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//            t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            for(int h=0;h<nbCharTotal;h++){
//                t.Append("&nbsp;");
//            }
//            t.Append("</td>");
			
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");

//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++){
//                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+tab[0,k].ToString()+"</td>");
//                for(int h=0;h<nbCharTotal+5;h++){
//                    t.Append("&nbsp;");
//                }
//                t.Append("</td>");
//            }
//            //fin MAJ

//            #endregion

//            #region Période précédente
//            if (webSession.PeriodBeginningDate != zoomDate)
//            {
//                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(-1).ToString("yyyyMM")
//                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//                }
//                else{
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//                    tmp.SubWeek(1);
//                    if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//                    t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_left_up.gif\" width=\"11\"></a></td>");
//                }
//            }
//            else
//                t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneel\">&nbsp;</td>");
//            #endregion

//            #region Entête de période
//            t.Append("\r\n\t\t\t\t\t\t<td colspan=\""+(nbColTab-FIRST_PERIOD_INDEX-2)+"\" class=\"pmannee2\" align=center>"
//                +Dates.getPeriodTxt(webSession, zoomDate)
//                +"</td>");
//            #endregion

//            #region Période suivante
//            if (webSession.PeriodEndDate != zoomDate){
//                if (webSession.DetailPeriod != WebConstantes.CustomerSessions.Period.DisplayLevel.weekly){
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate="+
//                        (new DateTime(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)),1)).AddMonths(1).ToString("yyyyMM")
//                        +"\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//                }
//                else{
//                    t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\"><a href=\""+url+"?idSession="+webSession.IdSession+"&zoomDate=");
//                    AtomicPeriodWeek tmp = new AtomicPeriodWeek(int.Parse(zoomDate.Substring(0,4)),int.Parse(zoomDate.Substring(4,2)));
//                    tmp.Increment();
//                    if(tmp.Week.ToString().Length<2)t.Append(tmp.Year.ToString() +"0"+ tmp.Week.ToString());
//                    else t.Append(tmp.Year.ToString() + tmp.Week.ToString());
//                    t.Append("\"><IMG border=0 height=\"12\" src=\"/images/Common/Arrow_right_up.gif\" width=\"11\"></a></td>");
//                }
//            }
//            else
//            t.Append("\r\n\t\t\t\t\t\t<td class=\"pmbtanneer\">&nbsp;</td>");
//            t.Append("</tr><tr>");
//            #endregion

//            #region Périodes
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){

//                t.Append("<td class=\"p10\">&nbsp;"+((DateTime)tab[0,j]).ToString("dd")+"&nbsp;</td>");
//            }

//            string dayClass="";
//            t.Append("\r\n\t</tr><tr>");
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = ((DateTime)tab[0,j]);
//                if (dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ) {
//                    dayClass="p132";
//                }
//                else{
//                    dayClass="p131";
//                }	
//                t.Append("<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>");
//            }
//            t.Append("\r\n\t</tr>");
//            #endregion

//            #region Calendrier d'action
//            //t.Append("\r\n\t<tr>");
//            i=0;
//            try{
//                for(i=1;i<nbline;i++){
//                    for(j=0;j<nbColTab;j++){
//                        switch(j){
//                                // Total ou vehicle
//                            case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string  tmpHtml="";
//                                    if(i==MediaPlanAlert.TOTAL_LINE_INDEX) classe="pmtotal";
//                                    else{
//                                        classe="pmvehicle";
//                                        if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]!=null)
//                                            tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        else
//                                            tmpHtml="";
//                                    }
//                                        //										if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
//                                        //											if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
//                                        //												tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        //											}else{tmpHtml="";}
//                                        //										}else{
//                                        //											tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        //										}										
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\""+classe+"nb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+8+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                // Category
//                            case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+",-1,-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"pmcategory\">"+tmpHtml+"</td>");
//                                    currentCategoryName=tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\"pmcategorynb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    j=j+7+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                // Media
//                            case MediaPlanAlert.MEDIA_COLUMN_INDEX:
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                if(premier){
//                                    //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la catégorie "chaîne thématique"
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                    else
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"></td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=false;
//                                }
//                                else{
//                                    //On verifie si la chaine IdMedia, IdVehicle est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\" onclick=\"\" ><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','"+zoomDate+"');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                    else
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++){
//                                        t.Append("<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+6+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=true;
//                                }	
//                                //aller aux colonnes du calendrier d'action
//                                j=j+6;
//                                break;
//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)){
//                                    switch((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i,j]){
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
//                                            t.Append("<td class=\"p4\">&nbsp;</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
//                                            t.Append("<td class=\"p5\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                            break;
//                        }
//                    }
//                    t.Append("</tr>");
//                }
//            }
//            catch(System.Exception e){
//                throw(new System.Exception("erreur i="+i+",j="+j,e));
//            }
//            //t.Append("</tr></table></td></tr></table>");
			

//            //	t.Append("</table></td></tr></table>");
			
//            t.Append("</table>");
			
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #region Sans détail support date de la session
//        /// <summary>
//        /// Génère le code HTML pour afficher un calendrier d'action détaillé en jour entre les dates de la session.
//        /// Elles se base sur un tableau contenant les données
//        /// tab vide : message "Pas de données" + bouton retour
//        /// tab non vide:
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Javascripts (ouverture du détail des insertions et ouverture/fermeture des calendriers d'actions)
//        ///		Génération du code HTML des entêtes de colonne
//        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <param name="page">Page qui affiche le Plan Média</param>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code Généré</returns>
//        /// <remarks>
//        /// Utilise les méthodes:
//        ///		public static string TNS.AdExpress.Web.Functions.Script.OpenCreation()
//        ///		public static string TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan()
//        /// </remarks>
//        public static string GetMediaPlanAlertUI(Page page,object[,] tab,WebSession webSession){
		
//            #region Pas de données à afficher
//            if(tab.GetLength(0)==0){
//                return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
//                    +"<br><br><a href=\"javascript:history.back()\" onmouseover=\"bouton.src='/Images/"+webSession.SiteLanguage+"/button/back_down.gif';\" onmouseout=\"bouton.src = '/Images/"+webSession.SiteLanguage+"/button/back_up.gif';\">"
//                    +"<img src=\"/Images/"+webSession.SiteLanguage+"/button/back_up.gif\" border=0 name=bouton></a></div>");
//            }
//            #endregion

//            #region Variables
//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            int nbColYear = 0;
//            //A 0 car cette méthode ne peut être utilisé que sur une période (un mois ou une semaine)==> pas de multiannées. int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ

//            string classe;
//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            //MAJ GR : FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear --> FIRST_PERIOD_INDEX
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            //fin MAJ
//            bool premier=true;
//            string currentCategoryName="tit";
//            string totalUnit="";
//            const string PLAN_MEDIA_1_CLASSE="p6";
//            const string PLAN_MEDIA_2_CLASSE="p7";
//            const string PLAN_MEDIA_NB_1_CLASSE="p8";
//            const string PLAN_MEDIA_NB_2_CLASSE="p9";
//            #endregion

//            #region On calcule la taille de la colonne Total
//            int nbtot=WebFunctions.Units.ConvertUnitValueToString(tab[1,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit).Length;
//            int nbSpace=(nbtot-1)/3;
//            int nbCharTotal=nbtot+nbSpace-5;
//            if(nbCharTotal<5)nbCharTotal=0;
//            #endregion
			
//            #region script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openCreation", WebFunctions.Script.OpenCreation());
//            // On enregistre le script DynamicMediaPlan qui rend les calendriers d'action dynamique
//            if (!page.ClientScript.IsClientScriptBlockRegistered("DynamicMediaPlan")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DynamicMediaPlan", TNS.AdExpress.Web.Functions.Script.DynamicMediaPlan());
//            #endregion

//            #region Début du tableau
//            t.Append("<table id=\"calendartable\" border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("\r\n\t\t<td rowspan=3 width=\"250px\" class=\"p2\">");
//            t.Append("\n\t\t\t<table width=\"250px\" border=0 cellpadding=0 cellspacing=0 bgcolor=#644883>\r\n\t\t\t\t<tr>");
//            t.Append("\n\t\t\t\t\t<td class=\"p1\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"</td>");
//            t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:showAllContent3();\"><img id=pictofermer border=0 src=\"/Images/Common/button/picto_fermer.gif\"></a></td>");
//            t.Append("\n\t\t\t\t\t<td align=right width=40px><a href=\"javascript:hideAllContent3();\"><img id=pictoouvrir border=0 src=\"/Images/Common/button/picto_ouvrir.gif\"></a></td>");
//            t.Append("\n\t\t\t\t\t<td bgcolor=#ffffff width=1px class=\"p2\"></td>");
//            t.Append("\n\t\t\t\t</tr>\n\t\t\t</table>");
//            t.Append("\r\n\t\t</td>");
//            t.Append("\r\n\t\t<td rowspan=3 class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            for(int h=0;h<nbCharTotal;h++){
//                t.Append("&nbsp;");
//            }
//            t.Append("</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
//            {
//                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+tab[0,k].ToString()+"</td>");
//            }
//            //fin MAJ
//            #endregion

//            #region Période
//            string dateHtml="<tr>";
//            string headerHtml="";
//            DateTime currentDay = new DateTime(((DateTime)tab[0,FIRST_PERIOD_INDEX]).Ticks);
//            int previousMonth = currentDay.Month;
//            currentDay = currentDay.AddDays(-1);
//            int nbPeriodInMonth=0;
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                currentDay = currentDay.AddDays(1);
//                if (currentDay.Month!=previousMonth){
//                    if (nbPeriodInMonth>=8){
//                        headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                            +Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM"))
//                            +"</td>";
//                    }
//                    else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                             +"&nbsp"
//                             +"</td>";
//                    nbPeriodInMonth=0;
//                    previousMonth = currentDay.Month;
//                }
//                nbPeriodInMonth++;
//                dateHtml+="<td class=\"p10\">&nbsp;"+currentDay.ToString("dd") +"&nbsp;</td>";
//            }
//            if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                                       +Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM"))
//                                       +"</td>";
//            else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                     +"&nbsp"
//                     +"</td>";
//            dateHtml += "</tr>";

//            string dayClass="";
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            dateHtml+="\r\n\t<tr>";
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = ((DateTime)tab[0, j]);
//                if (dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ) {
//                    dayClass="p132";
//                }
//                else{
//                    dayClass="p131";
//                }	
//                dateHtml+="<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
//            }
//            dateHtml+="\r\n\t</tr>";

//            headerHtml += "</tr>";
//            t.Append(headerHtml);
//            t.Append(dateHtml);
//            #endregion

//            #region Calendrier d'actions
//            //t.Append("\r\n\t<tr>");
//            i=0;
//            try{
//                for(i=1;i<nbline;i++){
//                    for(j=0;j<nbColTab;j++){
//                        switch(j){
//                                // Total ou vehicle
//                            case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    //vérification que la catégorie n est pas les chaines thematiques
									
//                                    if(i==MediaPlanAlert.TOTAL_LINE_INDEX) classe="pmtotal";
//                                    else{

//                                        classe="pmvehicle";
//                                        if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]!=null)
//                                        tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+",-1,-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        else
//                                            tmpHtml="";
//                                    }
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //t.Append("\r\n\t<tr id=\""+tab[i,j]+"Content4\" style=\"DISPLAY:inline\">\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+Int64.Parse(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString()).ToString("### ### ### ### ###")+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+classe+"\">"+tmpHtml+"</td>");
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+classe+"nb\">"+((double)tab[i,j+8+k]).ToString("0.##")+"</td>");
//                                    }
//                                    j=j+8+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                // Category
//                            case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    string tmpHtml="";
//                                    //vérification que la catégorie n est pas les chaines thematiques
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]!=null)tmpHtml="<a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+",-1,-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    // On traite le cas des pages
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t<tr id=\""+i.ToString()+"Content4\"onclick=\"showHideContent3('"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"');\" style=\"DISPLAY:inline; CURSOR: hand\">\r\n\t\t<td class=\"pmcategory\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategorynb\">"+totalUnit+"</td><td class=\"pmcategorynb\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\"pmcategory\">"+tmpHtml+"</td>");
//                                    currentCategoryName=tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX].ToString();
//                                    //aller aux colonnes du calendrier d'action
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+7+k]).ToString("0.##")+"</td>");
//                                    }
//                                    j=j+7+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                // Media
//                            case MediaPlanAlert.MEDIA_COLUMN_INDEX:
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                if(premier){
//                                    //On verifie si la chaine IdMedia est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                    else
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_1_CLASSE+"\"></td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+PLAN_MEDIA_1_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=false;
//                                }
//                                else{
//                                    //On verifie si la chaine IdMedia, IdVehicle est affectée. Elle ne l'est pas dans le cas d'un support appartenant a la caté"gorie "chaîne thématique"
//                                    if (tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX] != null)
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"><a href=\"javascript:openCreation('"+webSession.IdSession+"','"+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_VEHICLE_COLUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_CATEGORY_COUMN_INDEX]+","+tab[i, FrameWorkResultConstantes.MediaPlanAlert.ID_MEDIA_COLUMN_INDEX]+",-1,-1','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a></td>");
//                                    else
//                                        t.Append("\r\n\t<tr id=\""+i.ToString()+currentCategoryName+"Content3\" onclick=\"showHideAllContent3('"+i.ToString()+currentCategoryName+"','+currentCategoryName+');\" style=\"DISPLAY: none;CURSOR: hand;\">\r\n\t\t<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_NB_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td><td align=\"center\" class=\""+PLAN_MEDIA_2_CLASSE+"\"></td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+PLAN_MEDIA_2_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=true;
//                                }	
//                                j=j+6;
//                                break;
//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)){
//                                    switch((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i,j]){
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
//                                            t.Append("<td class=\"p4\">&nbsp;</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
//                                            t.Append("<td class=\"p5\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    t.Append("</tr>");
//                }
//            }
//            catch(System.Exception e){
//                throw(new System.Exception("erreur i="+i+",j="+j,e));
//            }
//            //t.Append("</table></td></tr></table>");			
//            t.Append("</table>");
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #endregion

//        #region Sortie Ms Excel Avec détail support(Web)
//        /// <summary>
//        /// Génère le code HTML pour Ms Excel pour afficher un calendrier d'action entre deux périodes respectant le type de période spécifié dans la session.
//        /// Elles se base sur un tableau contenant les données. Cette méthode sert aussi bien pour les zooms que pour les alertes
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Rappel des paramètres de périodes 
//        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <returns>Code Généré</returns>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="periodBeginning">Période de début du calendrier d'action</param>
//        /// <param name="periodEnd">Période de fin du calendrier d'action</param>
//        /// <param name="showValue">Montre les valeurs dans les cases du calendrier d'action</param>
//        /// <returns>Code Généré</returns>
//        public static string GetMediaPlanAlertWithMediaDetailLevelExcelUI(object[,] tab,WebSession webSession, string periodBeginning, string periodEnd,bool showValue){

//            #region Variables
//            string classe;
//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            //Bug problème année
//            //int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            int nbColYear = int.Parse(periodEnd.Substring(0,4)) - int.Parse(periodBeginning.Substring(0,4));
			
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ

//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
//            string totalUnit="";
//            int colorItemIndex=1;
//            int colorNumberToUse=0;
//            int sloganIndex=-1;
//            //webSession.SloganColors.Clear();
//            try{webSession.SloganColors.Add((Int64)0,"pc0");}catch(System.Exception){}
//            string presentClass="p4";
//            string extendedClass="p5";
//            string stringItem="&nbsp;";
//            #endregion
		
//            #region Rappel des paramètres
////			//t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,true,false,periodBeginning,periodEnd,""));
////			t.Append(ExcelFunction.GetExcelHeader(webSession,true,periodBeginning,periodEnd));

//            if(webSession.CurrentModule==WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA || webSession.CurrentModule==WebConstantes.Module.Name.ALERTE_PLAN_MEDIA){
//                t.Append(ExcelFunction.GetExcelHeader(webSession,false,periodBeginning,periodEnd));
//            }
//            else{
//                t.Append(ExcelFunction.GetExcelHeaderForMediaPlanPopUp(webSession,false,periodBeginning,periodEnd));
//            }
//            #endregion

//            #region debut Tableau
//            t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("<td rowspan=\"3\" width=\"200px\" class=\"ptX\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//            t.Append("<td rowspan=\"3\" class=\"ptX\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            t.Append("</td>");
//            t.Append("<td rowspan=\"3\" class=\"ptX\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.DetailledMediaPlan.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++) {
//                t.Append("<td rowspan=\"3\" class=\"ptX\" align=center>"+tab[0,k].ToString()+"</td>");
//            }
//            //fin MAJ
//            #endregion

//            #region Période
//            string dateHtml="<tr>";
//            string headerHtml="";
//            DateTime currentDay = DateString.YYYYMMDDToDateTime((string)tab[0,FIRST_PERIOD_INDEX]);
//            int previousMonth = currentDay.Month;
//            currentDay = currentDay.AddDays(-1);
//            int nbPeriodInMonth=0;
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                currentDay = currentDay.AddDays(1);
//                if (currentDay.Month!=previousMonth){
//                    if (nbPeriodInMonth>=8){
//                        headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center>"
//                            + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
//                            +"</td>";
//                    }
//                    else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center>"
//                             +"&nbsp"
//                             +"</td>";
//                    nbPeriodInMonth=0;
//                    previousMonth = currentDay.Month;
//                }
//                nbPeriodInMonth++;
//                dateHtml+="<td class=\"ppX\">"+currentDay.ToString("dd")+"</td>";
//            }
//            if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center>"
//                                       + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
//                                       +"</td>";
//            else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"ptX\" align=center>"
//                     +"&nbsp"
//                     +"</td>";
//            dateHtml += "</tr>";
//            // Jour de la semaine
//            string dayClass="";
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            dateHtml+="\r\n\t<tr>";
//            for(j=FIRST_PERIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = DateString.YYYYMMDDToDateTime((string)tab[0, j]);
//                if (dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ) {
//                    dayClass="pdwX";
//                }
//                else{
//                    dayClass="pdX";
//                }	
//                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
//            }
//            dateHtml+="</tr>";


//            headerHtml += "</tr>";
//            t.Append(headerHtml);
//            t.Append(dateHtml);
//            #endregion

//            #region Tableau
//            i=0;
//            try{
//                sloganIndex=GetSloganIdIndex(webSession);
//                for(i=1;i<nbline;i++){
//                    if(sloganIndex!=-1	&& tab[i,sloganIndex]!=null){
//                        if(!webSession.SloganColors.ContainsKey((Int64)tab[i,sloganIndex])){
//                            colorNumberToUse=(colorItemIndex%30)+1;
//                            webSession.SloganColors.Add((Int64)tab[i,sloganIndex],"pc"+colorNumberToUse.ToString());
//                            colorItemIndex++;
//                        }
//                        presentClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
//                        extendedClass=webSession.SloganColors[(Int64)tab[i,sloganIndex]].ToString();
//                        stringItem="x";
//                    }
//                    else{
//                        presentClass="p4";
//                        extendedClass="p5";
//                        stringItem="&nbsp;";
//                    }
//                    for(j=0;j<nbColTab;j++){
//                        switch(j){
//                            #region Level 1
//                            case DetailledMediaPlan.L1_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    if(i==DetailledMediaPlan.TOTAL_LINE_INDEX) classe="L0X";
//                                    else classe="L1X";
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite les unités
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    //									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L1_COLUMN_INDEX]+"</td><td class=\""+classe+"nb\">"+totalUnit+"</td><td class=\""+classe+"nb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\""+classe+"nb\" nowrap>"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+10+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    //saut jusquà la 1ère colonne du calendrier d'action
//                                    j=j+10+nbColYear;
//                                    //fin MAJ								
//                                }
//                                break;
//                            #endregion

//                            #region Level 2
//                            case DetailledMediaPlan.L2_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    // On traite les unités
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t\t<td class=\"L2X\">&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L2_COLUMN_INDEX]+"</td><td class=\"L2Xnb\">"+totalUnit+"</td><td class=\"L2Xnb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\"L2Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+9+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    //saut jusquà la 1ère colonne du calendrier d'action
//                                    j=j+9+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 3
//                            case DetailledMediaPlan.L3_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    // On traite les unités
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t\t<td class=\"L3X\">&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L3_COLUMN_INDEX]+"</td><td class=\"L3Xnb\">"+totalUnit+"</td><td class=\"L3Xnb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++) {
//                                        t.Append("<td class=\"L3Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+8+k].ToString(),webSession.Unit)+"</td>");
//                                    }
//                                    //saut jusquà la 1ère colonne du calendrier d'action
//                                    j=j+8+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                            #endregion

//                            #region Level 4
//                            case DetailledMediaPlan.L4_COLUMN_INDEX:
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.DetailledMediaPlan.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                t.Append("\r\n\t\t<td class=\"L4X\">&nbsp;&nbsp;&nbsp;"+tab[i,FrameWorkResultConstantes.DetailledMediaPlan.L4_COLUMN_INDEX]+"</td><td class=\"L4Xnb\">"+totalUnit+"</td><td class=\"L4Xnb\">"+((double)tab[i,FrameWorkResultConstantes.DetailledMediaPlan.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                //MAJ GR : totaux par années si nécessaire
//                                for(k=1; k<=nbColYear; k++) {
//                                    t.Append("<td class=\"L4Xnb\">"+WebFunctions.Units.ConvertUnitValueToString(tab[i,j+7+k].ToString(),webSession.Unit)+"</td>");
//                                }
//                                //fin MAJ
//                                //saut jusquà la 1ère colonne du calendrier d'action
//                                j=j+7+nbColYear;
//                                //fin MAJ
//                                break;
//                            #endregion

//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(MediaPlanItem)){
//                                    switch(((MediaPlanItem)tab[i,j]).GraphicItemType){
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.present:
//                                            if(showValue)t.Append("<td class=\""+presentClass+"\">"+Functions.Units.ConvertUnitValueToString(((MediaPlanItem)tab[i,j]).Unit.ToString(),webSession.Unit)+"</td>");
//                                            else t.Append("<td class=\""+presentClass+"\">"+stringItem+"</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.DetailledMediaPlan.graphicItemType.extended:
//                                            t.Append("<td class=\""+extendedClass+"\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    t.Append("</tr><tr>");
//                }
//            }
//            catch(System.Exception e){
//                throw(new System.Exception("erreur i="+i+",j="+j,e));
//            }
//            t.Append("</tr></table></td></tr></table>");
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #region Sortie Ms Excel (Web)
//        /// <summary>
//        /// Génère le code HTML pour Ms Excel pour afficher un calendrier d'action entre deux périodes respectant le type de période spécifié dans la session.
//        /// Elles se base sur un tableau contenant les données. Cette méthode sert aussi bien pour les zooms que pour les alertes
//        ///		Initialisation des compteurs de colonnnes, de lignes, de nombre de périodes...
//        ///		Rappel des paramètres de périodes 
//        ///		Génération du code HTML des périodes (Libellé du mois, intitulés d'une unité de période)
//        ///		Génération du code HTML du calendrier d'action
//        /// </summary>
//        /// <returns>Code Généré</returns>
//        /// <param name="tab">Tableau contenant les données à mettre en forme</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="periodBeginning">Période de début du calendrier d'action</param>
//        /// <param name="periodEnd">Période de fin du calendrier d'action</param>
//        /// <returns>Code Généré</returns>
//        public static string GetMediaPlanAlertExcelUI(object[,] tab,WebSession webSession, string periodBeginning, string periodEnd){

//            #region Variables
//            string classe;
//            //MAJ GR : Colonnes totaux par année si nécessaire
//            int k = 0;
//            //Bug problème année
//            //int nbColYear = int.Parse(webSession.PeriodEndDate.Substring(0,4)) - int.Parse(webSession.PeriodBeginningDate.Substring(0,4));
//            int nbColYear = int.Parse(periodEnd.Substring(0,4)) - int.Parse(periodBeginning.Substring(0,4));
			
//            if (nbColYear>0) nbColYear++;
//            int FIRST_PERIOD_INDEX = FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX+nbColYear;
//            //fin MAJ

//            //A chaque fois qu'on utilise la longueur du tableau dans cette fonction, il faut penser au fait
//            //que l'idMedia ne fait pas partie de l affichage donc pour considerer le nombre de colonne 
//            //affichée, utilisez getLength-1
//            int nbColTab=tab.GetLength(1),j,i;
//            int nbline=tab.GetLength(0);
//            int nbPeriod=nbColTab-FIRST_PERIOD_INDEX-1;
//            bool premier=true;
//            const string PLAN_MEDIA_XLS_1_CLASSE="pmmediaxls1";
//            const string PLAN_MEDIA_XLS_2_CLASSE="pmmediaxls2";
//            System.Text.StringBuilder t=new System.Text.StringBuilder(1000);
//            string totalUnit="";
//            #endregion
		
//            #region Rappel des paramètres
//            // Paramètres du tableau
//            //t.Append(ExcelFunction.GetExcelHeader(webSession,false,true,false,true,false,periodBeginning,periodEnd,""));
//            t.Append(ExcelFunction.GetExcelHeader(webSession,true,periodBeginning,periodEnd));
////			t.Append("<table border=0 cellpadding=0 cellspacing=0>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(464,webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+": "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(periodBeginning,webSession.PeriodType),webSession.SiteLanguage)+" - "
////				+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(periodEnd,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			t.Append("<tr>");
////			t.Append("<td>"+GestionWeb.GetWebWord(1313,webSession.SiteLanguage)+" "+GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.XLSUnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
////			t.Append("</tr>");
////			t.Append("</table>");
//            #endregion

//            #region debut Tableau
//            t.Append("<table border=0 cellpadding=0 cellspacing=0>\r\n\t<tr>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" width=\"200px\" class=\"p2\">"+GestionWeb.GetWebWord(804,webSession.SiteLanguage)+"&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(805,webSession.SiteLanguage));
//            t.Append("</td>");
//            t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+GestionWeb.GetWebWord(806,webSession.SiteLanguage)+"</td>");
//            // MAJ GR : On affiche les années si nécessaire
//            for(k=FrameWorkResultConstantes.MediaPlanAlert.FIRST_PEDIOD_INDEX; k<FIRST_PERIOD_INDEX; k++)
//            {
//                t.Append("\r\n\t\t<td rowspan=\"3\" class=\"p2\">"+tab[0,k].ToString()+"</td>");
//            }
//            //fin MAJ
//            #endregion

//            #region Période
//            string dateHtml="<tr>";
//            string headerHtml="";
//            DateTime currentDay = new DateTime(((DateTime)tab[0,MediaPlanAlert.FIRST_PEDIOD_INDEX]).Ticks);
//            int previousMonth = currentDay.Month;
//            currentDay = currentDay.AddDays(-1);
//            int nbPeriodInMonth=0;
//            for(j=MediaPlanAlert.FIRST_PEDIOD_INDEX;j<nbColTab;j++){
//                currentDay = currentDay.AddDays(1);
//                if (currentDay.Month!=previousMonth){
//                    if (nbPeriodInMonth>=8){
//                        headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                            + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.AddDays(-1).ToString("yyyyMM")))
//                            +"</td>";
//                    }
//                    else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                             +"&nbsp"
//                             +"</td>";
//                    nbPeriodInMonth=0;
//                    previousMonth = currentDay.Month;
//                }
//                nbPeriodInMonth++;
//                dateHtml+="<td class=\"p10\">"+currentDay.ToString("dd")+"</td>";
//            }
//            if (nbPeriodInMonth>=8)headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                                       + Convertion.ToHtmlString(Dates.getPeriodTxt(webSession, currentDay.ToString("yyyyMM")))
//                                       +"</td>";
//            else headerHtml+="<td colspan=\""+nbPeriodInMonth+"\" class=\"p2\" align=center>"
//                     +"&nbsp"
//                     +"</td>";
//            dateHtml += "</tr>";
//            // Jour de la semaine
//            string dayClass="";
//            CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[webSession.SiteLanguage].Localization);
//            dateHtml+="\r\n\t<tr>";
//            for(j=MediaPlanAlert.FIRST_PEDIOD_INDEX;j<nbColTab;j++){
//                DateTime dt = ((DateTime)tab[0, j]);
//                if (dt.DayOfWeek == DayOfWeek.Saturday
//                    || dt.DayOfWeek == DayOfWeek.Sunday
//                    ) {
//                    dayClass="p133";
//                }
//                else{
//                    dayClass="p132";
//                }	
//                dateHtml += "<td class=\"" + dayClass + "\">" + DayString.GetCharacters(dt, cultureInfo, 1) + "</td>";
//            }
//            dateHtml+="\r\n\t</tr>";


//            headerHtml += "</tr>";
//            t.Append(headerHtml);
//            t.Append(dateHtml);
//            #endregion

//            #region Tableau
//            i=0;
//            try{
//                for(i=1;i<nbline;i++){
//                    for(j=0;j<nbColTab;j++){
//                        switch(j){
//                                // Total ou vehicle
//                            case MediaPlanAlert.VEHICLE_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    if(i==MediaPlanAlert.TOTAL_LINE_INDEX) classe="pmtotal";
//                                    else classe="pmvehicle";
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayEnd)){
//                                        i=int.MaxValue-2;
//                                        j=int.MaxValue-2;
//                                        break;
//                                    }
//                                    // On traite les unités
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
////									t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    t.Append("\r\n\t\t<td class=\""+classe+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.VEHICLE_COLUMN_INDEX]+"</td><td class=\""+classe+"\">"+totalUnit+"</td><td class=\""+classe+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+classe+"nb\">"+((double)tab[i,j+8+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //saut jusquà la 1ère colonne du calendrier d'action
//                                    j=j+8+nbColYear;
//                                    //fin MAJ								
//                                }
//                                break;
//                                // Category
//                            case MediaPlanAlert.CATEGORY_COLUMN_INDEX:
//                                if(tab[i,j]!=null){
//                                    // On traite les unités
//                                    totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                    t.Append("\r\n\t\t<td class=\"pmcategory\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.CATEGORY_COLUMN_INDEX]+"</td><td class=\"pmcategory\">"+totalUnit+"</td><td class=\"pmcategory\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\"pmcategorynb\">"+((double)tab[i,j+7+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //saut jusquà la 1ère colonne du calendrier d'action
//                                    j=j+7+nbColYear;
//                                    //fin MAJ
//                                }
//                                break;
//                                // Media
//                            case MediaPlanAlert.MEDIA_COLUMN_INDEX:
//                                // On traite le cas des pages
//                                totalUnit=WebFunctions.Units.ConvertUnitValueToString(tab[i,FrameWorkResultConstantes.MediaPlanAlert.TOTAL_COLUMN_INDEX].ToString(),webSession.Unit);
//                                if(premier){
//                                    t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+PLAN_MEDIA_XLS_1_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=false;
//                                }
//                                else{
//                                    t.Append("\r\n\t\t<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+tab[i,FrameWorkResultConstantes.MediaPlanAlert.MEDIA_COLUMN_INDEX]+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+totalUnit+"</td><td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,FrameWorkResultConstantes.MediaPlanAlert.PDM_COLUMN_INDEX]).ToString("0.00")+"</td>");
//                                    //MAJ GR : totaux par années si nécessaire
//                                    for(k=1; k<=nbColYear; k++)
//                                    {
//                                        t.Append("<td class=\""+PLAN_MEDIA_XLS_2_CLASSE+"\">"+((double)tab[i,j+6+k]).ToString("0.##")+"</td>");
//                                    }
//                                    //fin MAJ
//                                    premier=true;
//                                }	
//                                // MAJ GR
//                                //saut jusquà la 1ère colonne du calendrier d'action
//                                j=j+6+nbColYear;
//                                //fin MAJ
//                                break;
//                            default:
//                                if(tab[i,j]==null){
//                                    t.Append("<td class=\"p3\">&nbsp;</td>");
//                                    break;
//                                }
//                                if(tab[i,j].GetType()==typeof(FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)){
//                                    switch((FrameWorkResultConstantes.MediaPlanAlert.graphicItemType)tab[i,j]){
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.present:
//                                            t.Append("<td class=\"p4\">&nbsp;</td>");
//                                            break;
//                                        case FrameWorkResultConstantes.MediaPlanAlert.graphicItemType.extended:
//                                            t.Append("<td class=\"p5\">&nbsp;</td>");
//                                            break;
//                                        default:
//                                            t.Append("<td class=\"p3\">&nbsp;</td>");
//                                            break;
//                                    }
//                                }
//                                break;
//                        }
//                    }
//                    t.Append("</tr><tr>");
//                }
//            }
//            catch(System.Exception e){
//                throw(new System.Exception("erreur i="+i+",j="+j,e));
//            }
//            t.Append("</tr></table></td></tr></table>");
//            #endregion

//            // On vide le tableau
//            tab=null;

//            return(t.ToString());

//        }
//        #endregion

//        #region Méthode interne
//        /// <summary>
//        /// Obtient la colonne contenant le id_slogan
//        /// Si le détail support ne contient pas le niveau slogan, elle retoune -1
//        /// </summary>
//        /// <param name="webSession">Session client</param>
//        /// <returns>Colonne contenant l'id_slogan, -1 si pas de slogan</returns>
//        private static int GetSloganIdIndex(WebSession webSession){
//            switch(webSession.PreformatedMediaDetail){
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
//                    return(DetailledMediaPlan.L1_ID_COLUMN_INDEX);
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
//                    return(DetailledMediaPlan.L3_ID_COLUMN_INDEX);
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
//                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
//                    return(DetailledMediaPlan.L4_ID_COLUMN_INDEX);
//                default:
//                    return(-1);
//            }
//        }
//        #endregion
//    }
//}

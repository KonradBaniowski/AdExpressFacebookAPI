//#region Information
//// Auteur : A.Obermeyer
//// Créé le : 24/09/2004
//// Modifié le :
////	03/05/2005	K. Shehzad	Changement pour rappel des paramètres pour en tête d'Excel 
////	22/06/2005	Masson		Excel des données brutes
////	19/09/2006	D. V. Mussuma		Ajout du niveaude détail produit
//#endregion

//using System;
//using System.Web.UI;
//using System.Windows.Forms;
//using TNS.AdExpress.Web.Core.Sessions;
//using TNS.AdExpress.Domain.Translation;
//using WebText = TNS.AdExpress.Web.Functions.Text;
//using WebConstantes=TNS.AdExpress.Constantes.Web;
//using ResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results.CompetitorAlert;
//using WebFunctions = TNS.AdExpress.Web.Functions;
//using WebExceptions=TNS.AdExpress.Web.Exceptions;
//using TNS.AdExpress.Domain.Web.Navigation;
//using CustomerConstantes=TNS.AdExpress.Constantes.Customer;
//using FrameWorkResultConstantes=TNS.AdExpress.Constantes.FrameWork.Results;
//using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
//using ClassificationCst=TNS.AdExpress.Constantes.Classification;
//using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
//using TNS.FrameWork;
//using CstDB = TNS.AdExpress.Constantes.DB;

//namespace TNS.AdExpress.Web.UI.Results{
//    /// <summary>
//    /// Construit l'interface utilisateur pour l'alerte concurrentielle
//    /// </summary>
//    public class CompetitorUI{

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
//        /// Génère le code html d'un module concurentiel
//        /// </summary>
//        /// <param name="page">Page utilisée pour montrer le résultat</param>
//        /// <param name="tab">Tableau de données du résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML</returns>
//        public static string GetHtml(Page page,object[,] tab,WebSession webSession){
		
//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            int nbSelectionGroup=1;
//            string classCss="";
//            string creation="";
//            int oldProduct=-1;
//            int level=-1;
//            bool endBalise=false;
//            bool oneLevel=true;
//            bool total=false;
//            Int64 idCreation=-1;
//            string totalUnit="";
//            string firstBaliseA="";
//            string endBaliseA="";
//            string tmp="";
//            //utilisé pour savoir qd afficher création
//            bool level1=false;
//            int nbrLevel;
			
//            bool displayCreation=false;
			
//            int debResult=ResultConstantes.FIRST_MEDIA_INDEX;
//            string mediaplan="";	
//            #endregion
			
//            #region Constantes
//            const string L1="acl1";
//            const string L2="acl2";
//            const string L3="acl3";
//            const string P2="p2";
//            #endregion
			
//            #region Type de module		
//            // Type de module pour savoir si l'on affiche les créations
//            try{
//                Module currentModuleDescription=ModulesList.GetModule(webSession.CurrentModule);
//                if(currentModuleDescription.ModuleType==TNS.AdExpress.Constantes.Web.Module.Type.alert){
//                    displayCreation=true;
//                    displayCreation=DisplayCreationBoolean(webSession,-1);
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de déterminer le type de module pour savoir s'il ont doit montrer les créations",err));
//            }
//            #endregion

//            #region Script
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openCreation")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenCreationCompetitorAlert", WebFunctions.Script.OpenCreationCompetitorAlert());
			
//            if (!page.ClientScript.IsClientScriptBlockRegistered("openGad")){
//                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openGad", TNS.AdExpress.Web.Functions.Script.OpenGad());
//            }
//            //plan média
//            if (!page.ClientScript.IsClientScriptBlockRegistered("OpenMediaPlanAlert")) page.ClientScript.RegisterClientScriptBlock(page.GetType(), "OpenMediaPlanAlert", WebFunctions.Script.OpenMediaPlanAlert());			
//            #endregion

//            #region Nombre de groupe
//            try{
//                while(webSession.CompetitorUniversMedia[nbSelectionGroup]!=null){
//                    nbSelectionGroup++;
//                }
//                nbSelectionGroup--;
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de déterminer le nombre de groupes de supports",err));
//            }
//            #endregion


//            nbrLevel=LevelForCreation(webSession);

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;


//            int nbCompetitorMedia=0;
//            i=1;

//            while(webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i]!=null){
//                tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i],CustomerConstantes.Right.type.mediaAccess);
//                nbCompetitorMedia+=tmp.Split(',').GetLength(0)+1;
//                if(tmp.Split(',').GetLength(0)>1)nbCompetitorMedia++;
//                i++;
//            }
//            nbCompetitorMedia--;

//            #region Analyse Panoramique
//            if(nbSelectionGroup<2){
//                // On ne montre pas le sous total
//                debResult++;
//            }
//            #endregion

//            #region HTML
//            try{
//                t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");

//                // Première ligne
//                t.Append("\r\n\t<tr height=\"40px\">");
//                // Si l'on a des concurrents on rajoute une ligne pour les libéllés support de références
//                // et supports concurrents
//                if(nbCompetitorMedia>0){
//                    // Produits
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff rowspan=2 valign=\"middle\"  nowrap>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//                    if(displayCreation){
//                        t.Append("<td class=\""+P2+"\" bgcolor=#ffffff rowspan=2 valign=\"middle\" nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//                    }
//                    //plan média
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff  rowspan=2 nowrap>"+GestionWeb.GetWebWord(150,webSession.SiteLanguage)+"</td>");
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff rowspan=2 valign=\"middle\" nowrap>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");										
//                }
//                else{
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//                    if(displayCreation){
//                        t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+GestionWeb.GetWebWord(540,webSession.SiteLanguage)+"</td>");
//                    }
//                    //plan média
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff  nowrap>"+GestionWeb.GetWebWord(150,webSession.SiteLanguage)+"</td>");
//                    t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");										
//                }
				
//                // Cas où l'on a des univers concurrents
//                if(nbCompetitorMedia>0){
//                    t.Append("<td><img src=\"/I/gsep.gif\" width=5 height=100%></td>");
//                    t.Append("<td colspan="+nbMedia+" class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1365,webSession.SiteLanguage)+"</td>");
//                    t.Append("<td><img src=\"/I/gsep.gif\" width=5 height=100%></td>");
//                    t.Append("<td colspan="+nbCompetitorMedia+" class=\""+P2+"\" nowrap>"+GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+"</td>");
//                    t.Append("</tr>");
//                    t.Append("\r\n\t<tr height=\"20px\">");
//                }
				
//                for(i=debResult;i<nbColTab;i++){
//                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                        t.Append("<td nowrap class=\""+P2+"\" bgcolor=#ffffff>"+tab[i,2]+"</td>");	
//                    }else{				
//                        t.Append("<td><img src=\"/I/gsep.gif\" width=5 height=100%></td>");
//                        t.Append("<td class=\""+P2+"\" nowrap>"+tab[i,2]+"</td>");
//                        oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                    }
//                }
				
//                t.Append("</tr>");

//                oldProduct=-1;
//                i=ResultConstantes.FIRST_LINE_RESULT_INDEX+3;
//                try{
//                    // On régarde qu'il y ait au moins deux niveaux
//                    while(i<nbline && oneLevel==true){
//                        if(	tab[ResultConstantes.LABELL2_INDEX,i]==null || tab[ResultConstantes.LABELL2_INDEX,i].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                            oneLevel=true;
//                            i++;
//                        }
//                        else{
//                            oneLevel=false;
//                        }
//                    }			
//                }
//                catch(Exception){oneLevel=true;}

//                #region Sélection du vehicle
//                string vehicleSelection=webSession.GetSelection(webSession.SelectionUniversMedia,CustomerConstantes.Right.type.vehicleAccess);
//                DBClassificationConstantes.Vehicles.names vehicleName=(DBClassificationConstantes.Vehicles.names)int.Parse(vehicleSelection);
//                if(vehicleSelection==null || vehicleSelection.IndexOf(",")>0) throw(new WebExceptions.PortofolioUIException("La sélection de médias est incorrecte"));
//                #endregion
//                for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
//                    for(i=0;i<nbColTab;i++){
					
//                        #region switch
//                        switch(i){				
//                            #region Level 1
//                            case  ResultConstantes.LABELL1_INDEX:						
							
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    level=ResultConstantes.IDL1_INDEX;									
//                                    if(!oneLevel)classCss=L1;
//                                    else classCss=L2;

//                                    if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                        tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL1_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                        firstBaliseA="<a class=\""+classCss+"\"  href=\""+tmp+"\"> > ";
//                                        endBaliseA="</a>"; 
//                                    }else{
//                                        firstBaliseA="";
//                                        endBaliseA=""; 		
//                                    }


//                                    if((long)tab[ResultConstantes.FIRST_COLUMN_T0_SHOW_INDEX,j]==ResultConstantes.TOTAL_IDENTIFICATION){
//                                        total=true;
//                                        t.Append("\r\n\t<tr align=\"right\" bgcolor=#ffffff height=\"20px\">\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[ResultConstantes.LABELL1_INDEX,j]+endBaliseA+"</td>");
//                                        i=ResultConstantes.TOTAL_INDEX-1;	
//                                    }
//                                    else{
//                                        total=false;
//                                        idCreation= (Int64)tab[ResultConstantes.IDL1_INDEX,j];
//                                        //lien plan média
//                                        if(WebFunctions.ProductDetailLevel.ShowMediaPlanL1(webSession))mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','1');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                        else mediaplan="";
//                                        // Pour savoir si l'on affiche la création
//                                        if(nbrLevel==ResultConstantes.IDL1_INDEX || webSession.CurrentTab==ResultConstantes.PORTEFEUILLE){
//                                            level1=true;
//                                            level1=DisplayCreationBoolean(webSession,ResultConstantes.IDL1_INDEX);
//                                        }
//                                        else{
//                                            level1=false;
//                                        }										
										
//                                        t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#B1A3C1';\"  bgcolor=#B1A3C1 height=\"20px\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>"+firstBaliseA+tab[ResultConstantes.LABELL1_INDEX,j]+endBaliseA+"</td>");										
//                                        i=ResultConstantes.TOTAL_INDEX-1;
//                                    }
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }									
//                                break;
//                                #endregion

//                            #region Level 2
//                            case ResultConstantes.LABELL2_INDEX:
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    classCss=L2;									
//                                    if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                        tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL2_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                        firstBaliseA="<a class=\""+classCss+"\" href=\""+tmp+"\"> > ";
//                                        endBaliseA="</a>"; 
//                                    }else{
//                                        firstBaliseA="";
//                                        endBaliseA=""; 		
//                                    }
//                                    level=ResultConstantes.IDL2_INDEX;
//                                    total=false;
//                                    oldProduct=-1;								
//                                    idCreation= (Int64)tab[ResultConstantes.IDL2_INDEX,j];
//                                    //lien plan média
//                                    if(WebFunctions.ProductDetailLevel.ShowMediaPlanL2(webSession))mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','2');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    else mediaplan="";
//                                    if(nbrLevel==ResultConstantes.IDL2_INDEX || webSession.CurrentTab==ResultConstantes.PORTEFEUILLE){
//                                        level1=true;
//                                        level1=DisplayCreationBoolean(webSession,ResultConstantes.IDL2_INDEX);
//                                    }
//                                    else{
//                                        level1=false;
//                                    }
//                                    t.Append("\r\n\t<tr align=\"right\" onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#D0C8DA';\" bgcolor=#D0C8DA height=\"20px\">\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[ResultConstantes.LABELL2_INDEX,j]+endBaliseA+"</td>");									
//                                    i=ResultConstantes.TOTAL_INDEX-1;
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }	
								
//                                break;
//                                #endregion

//                            #region Level 3
//                            case ResultConstantes.LABELL3_INDEX:
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if(tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]!=null){
//                                        classCss=L2;
//                                        tmp="javascript:openGad('"+webSession.IdSession+"','"+tab[ResultConstantes.LABELL3_INDEX,j]+"','"+tab[ResultConstantes.ADDRESS_COLUMN_INDEX,j]+"');";
//                                        firstBaliseA="<a  class=\""+classCss+"\" href=\""+tmp+"\"> > ";
//                                        endBaliseA="</a>"; 
//                                    }else{
//                                        firstBaliseA="";
//                                        endBaliseA=""; 		
//                                    }
//                                    level=ResultConstantes.IDL3_INDEX;									
//                                    total=false;
//                                    oldProduct=-1;
//                                    classCss=L3;
//                                    idCreation= (Int64)tab[ResultConstantes.IDL3_INDEX,j];
//                                    //lien plan média
//                                    mediaplan="<a href=\"javascript:OpenMediaPlanAlert('"+webSession.IdSession+"','"+idCreation+"','3');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";
//                                    if(nbrLevel==ResultConstantes.IDL3_INDEX || webSession.CurrentTab==ResultConstantes.PORTEFEUILLE){
//                                        level1=true;
//                                        level1=DisplayCreationBoolean(webSession,ResultConstantes.IDL3_INDEX);
//                                    }
//                                    else{
//                                        level1=false;
//                                    }
//                                    t.Append("\r\n\t<tr align=\"right\" height=\"20px\" bgcolor=#E1E0DA onmouseover=\"this.bgColor='#ffffff';\" onmouseout=\"this.bgColor='#E1E0DA';\" >\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+firstBaliseA+tab[ResultConstantes.LABELL3_INDEX,j]+endBaliseA+"</td>");									
//                                    i=ResultConstantes.TOTAL_INDEX-1;
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }									
//                                break;
//                                #endregion

//                            #region Total
//                            case ResultConstantes.TOTAL_INDEX:	
//                                if(tab[i,j]!=null){
							
//                                    totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                    //String a=totalUnit;
//                                    if(totalUnit.Trim().Length==0 || totalUnit==",00" || totalUnit.Trim()==",000" || totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")
//                                        totalUnit="";
//                                    oldProduct=-1;
//                                    if(DBClassificationConstantes.Vehicles.names.outdoor==vehicleName){
//                                        if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)!=null ){
//                                            if(!total){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
//                                            else{creation="";}
//                                        }else{creation="";}
//                                    }else{
//                                        if(!total){creation="<a href=\"javascript:OpenCreationCompetitorAlert('"+webSession.IdSession+"','"+idCreation+","+level+",','');\"><img border=0 src=\"/Images/Common/picto_plus.gif\"></a>";}
//                                    }
//                                    if(displayCreation && level1){
//                                        t.Append("<td class=\""+classCss+"\" align=\"center\" >"+creation+"</td>");										
//                                    }
//                                    else if(displayCreation){
//                                        t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");										
//                                    }
//                                    //lien plan média
//                                    if(total)t.Append("<td class=\""+classCss+"\" align=\"center\" >&nbsp;</td>");
//                                    else t.Append("<td class=\""+classCss+"\" align=\"center\" >"+mediaplan+"</td>");
//                                    //unité
//                                    t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+totalUnit+"</td>");	
									
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }									
//                                break;
//                                #endregion
							
//                            #region Résultats
//                            case ResultConstantes.FIRST_MEDIA_INDEX:
//                                for(i=debResult; i<nbColTab;i++){
//                                    if(tab[i,j]!=null){
//                                        endBalise=true;
//                                        total=false;
										
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                        if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" || totalUnit.Trim()==",000" ||totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")
//                                            totalUnit="&nbsp;";
//                                        if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){											
//                                            //unité
//                                            t.Append("\r\n\t<td class=\""+classCss+"\">"+totalUnit+"</td>");
////											if(totalUnit.Length>0){
////												//resultExist=true;
////											}
											
																				
//                                        }else{											
//                                            //t.Append("\r\n\t<td class=\""+classCss+"\" style=\"BORDER-LEFT: #ffffff solid;\" >"+tab[i,j]+"</td>");
//                                            //t.Append("\r\n\t<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"></td><td class=\""+classCss+"\">"+totalUnit+"</td>");
//                                            t.Append("\r\n\t<td><img src=\"/I/gsep.gif\" width=5 height=100%></td><td class=\""+classCss+"\">"+totalUnit+"</td>");
//                                            oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
											
//                                        }
//                                    }
//                                }	
//                                if(endBalise){
//                                    t.Append("</tr>");
//                                    endBalise=false;
//                                }							
//                                break;
//                                #endregion
//                        }
//                        #endregion
												
//                    }
//                }
//                t.Append("</table>");
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de construire le tableau HTML",err));
//            }
//            #endregion
			
////			if(!resultExist){
////				#region Pas de données à afficher				
////					return("<div align=\"center\" class=\"txtViolet11Bold\">"+GestionWeb.GetWebWord(177,webSession.SiteLanguage)
////						+"</div>");					
////				#endregion
////			}
//            return t.ToString();
//        }

//        /// <summary>
//        /// Génère le code html de la synthèse du module alerte concurentielle
//        /// </summary>
//        /// <param name="page">Page utilisée pour montrer le résultat</param>
//        /// <param name="tab">Tableau de données du résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <param name="excel">Vrai si resultat au format excel</param>
//        /// <returns>Code HTML</returns>
//        public static string GetSynthesisHtml(Page page,object[,] tab,WebSession webSession,bool excel){
			
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
			

//            t.Append("<table bgcolor=#ffffff border=0 cellpadding=0 cellspacing=0 >");
			
//            #region En-tête du tableau
//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Communs
//            t.Append("<td  bgcolor=#ffffff nowrap rowspan=3 valign=\"middle\">&nbsp;</td>");
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=3 valign=\"middle\">"+GestionWeb.GetWebWord(1127,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Absents
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff colspan=3  nowrap>"+GestionWeb.GetWebWord(1126,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Exclusifs
//            t.Append("<td  class=\""+P2+"\" nowrap  colspan=3>"+GestionWeb.GetWebWord(1128,webSession.SiteLanguage)+"</td>");						
//            t.Append("</tr>");

//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Communs Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Communs Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Absents  Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Absents  Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Exclusifs Nombre
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap rowspan=2 valign=\"middle\">"+GestionWeb.GetWebWord(1852,webSession.SiteLanguage)+"</td>");
//            //Exclusifs Unité 
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap colspan=2 valign=\"middle\">"+GestionWeb.GetWebWord((int)WebConstantes.CustomerSessions.UnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");			
//            t.Append("</tr>");

//            t.Append("\r\n\t<tr height=\"20px\">");
//            //Communs supports de références
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+GestionWeb.GetWebWord(1365,webSession.SiteLanguage)+"</td>");
//            //Communs supports concurrents
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap valign=\"middle\">"+GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Absents supports de références
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+GestionWeb.GetWebWord(1365,webSession.SiteLanguage)+"</td>");
//            //Absents supports concurrents
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+"</td>");
//            if(!excel)t.Append("<td bgcolor=\"#644883\" style=\"BORDER-RIGHT: white 2px solid;BORDER-LEFT: white 1px solid\"><img width=2px></td>");
//            //Exclusifs supports de références
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+GestionWeb.GetWebWord(1365,webSession.SiteLanguage)+"</td>");
//            //Exclusifs supports concurrents
//            t.Append("<td class=\""+P2+"\" bgcolor=#ffffff nowrap  valign=\"middle\">"+GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+"</td>");								
//            t.Append("</tr>");
//            #endregion

//            #region Affichage des résultats
//            //Pour chaque ligne produit
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
//                            case ResultConstantes.NUMBER_ABSENT_COLUMN_INDEX:
//                            case ResultConstantes.NUMBER_COMMON_COLUMN_INDEX:
//                            case ResultConstantes.NUMBER_EXCLUSIVE_COLUMN_INDEX:													
//                                if(tab[i,j]!=null)t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+double.Parse(tab[i,j].ToString()).ToString("### ### ###")+"</td>");											 
//                                else t.Append("\r\n\t\t<td align=\"left\" class=\""+classCss+"\" nowrap>&nbsp;</td>");
//                                break;
//                                //Colonnes unité 
//                            case ResultConstantes.MEDIA_COMPETITOR_ABSENT_COLUMN_INDEX:
//                            case ResultConstantes.MEDIA_COMPETITOR_COMMON_COLUMN_INDEX: 
//                            case ResultConstantes.MEDIA_COMPETITOR_EXCLUSIVE_COLUMN_INDEX: 
//                            case ResultConstantes.MEDIA_REFERENCE_ABSENT_COLUMN_INDEX: 
//                            case ResultConstantes.MEDIA_REFERENCE_COMMON_COLUMN_INDEX: 
//                            case ResultConstantes.MEDIA_REFERENCE_EXCLUSIVE_COLUMN_INDEX:							
//                                if(tab[i,j]!=null)unit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,false);						
//                                else unit="";
//                                if(unit.Trim().Length==0 || unit.Trim()==",00" ||unit.Trim()==",000" || unit.Trim()=="0,00" || unit=="Non Numérique" || unit=="00 H 00 M 00 S")unit="";							
//                                t.Append("\r\n\t<td class=\""+classCss+"\" nowrap>"+unit+"</td>");									
//                                break;							
//                            default :
//                                break;
//                        }

//                        switch(j){
//                            case ResultConstantes.MEDIA_COMPETITOR_ABSENT_COLUMN_INDEX:
//                            case ResultConstantes.MEDIA_COMPETITOR_COMMON_COLUMN_INDEX: 							 							
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
//#if(DEBUG)
//            //				int i,j;
//            //				string HTML="<html><table border=1><tr>";
//            //				for(i=0;i<tab.GetLength(0);i++){
//            //					for(j=0;j<tab.GetLength(1);j++){
//            //						if(tab[i,j]!=null)HTML+="<td>"+tab[i,j].ToString()+"</td>";
//            //						else HTML+="<td>&nbsp;</td>";
//            //					}
//            //					HTML+="</tr><tr>";
//            //				}
//            //				HTML+="</tr></table></html>";
//            //			return HTML;
//#endif
//            #endregion

//            return t.ToString();

//        }
//        #endregion

//        #region Sortie Excel
//        /// <summary>
//        /// Génère le code html pour le fichier Excel d'un module concurentielle
//        /// </summary>
//        /// <param name="page">Page utilisée pour montrer le résultat</param>
//        /// <param name="tab">Tableau de données du résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML pour fichier Excel</returns>
//        public static string GetExcel(Page page,object[,] tab,WebSession webSession){

//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            string classCss="";
//            int oldProduct=-1;
//            bool endBalise=false;
//            string totalUnit="";
//            int debResult=ResultConstantes.FIRST_MEDIA_INDEX;
//            int nbSelectionGroup=1;
//            string tmp="";
//            #endregion

//            #region Nombre de groupe
//            try{
//                while(webSession.CompetitorUniversMedia[nbSelectionGroup]!=null){
//                    nbSelectionGroup++;
//                }
//                nbSelectionGroup--;
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de déterminer le nombre de groupes de supports",err));
//            }

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;

//            int nbCompetitorMedia=0;
//            i=1;
//            while(webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i]!=null){
//                tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i],CustomerConstantes.Right.type.mediaAccess);
//                nbCompetitorMedia+=tmp.Split(',').GetLength(0);
//                if(tmp.Split(',').GetLength(0)>1)nbCompetitorMedia++;
//                i++;
//            }
//        //	nbCompetitorMedia--;

//            #endregion

//            #region Analyse Panoramique
//            if(nbSelectionGroup<2){
//                // On ne montre pas le sous total
//                debResult++;
//            }
//            #endregion

//            #region Rappel des paramètres
//                t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,true,true,""));
//    //			try{
//    //				// Paramètres du tableau
//    //				t.Append("<table border=0 cellpadding=0 cellspacing=0>");
//    //				t.Append("<tr>");
//    //				t.Append("<td>"+GestionWeb.GetWebWord(464,webSession.SiteLanguage)+"</td>");
//    //				t.Append("</tr>");
//    //				t.Append("<tr>");
//    //				t.Append("<td>"+GestionWeb.GetWebWord(119,webSession.SiteLanguage)+" : "+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate,webSession.PeriodType),webSession.SiteLanguage)+" - "
//    //					+WebFunctions.Dates.dateToString(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate,webSession.PeriodType),webSession.SiteLanguage)+"</td>");
//    //				t.Append("</tr>");
//    //				// Type de resultat
//    //				t.Append("<tr>");
//    //				t.Append("<td>"+GestionWeb.GetWebWord(1493,webSession.SiteLanguage)+" : "+stringTypeResult(webSession)+"</td>");
//    //				t.Append("</tr>");
//    //				t.Append("<tr>");
//    //				t.Append("<td>"+GestionWeb.GetWebWord(1313,webSession.SiteLanguage)+" "+GestionWeb.GetWebWord((int)TNS.AdExpress.Constantes.Web.CustomerSessions.XLSUnitsTraductionCodes[webSession.Unit],webSession.SiteLanguage)+"</td>");
//    //				t.Append("</tr>");
//    //				t.Append("<tr>");
//    //				t.Append("<td>"+GestionWeb.GetWebWord(1124,webSession.SiteLanguage)+" "+stringLevelProduct(webSession)+"</td>");
//    //				t.Append("</tr>");
//    //				t.Append("<tr>");
//    //				t.Append("<td>&nbsp;</td>");
//    //				t.Append("</tr>");
//    //				t.Append("</table>");
//    //			}
//    //			catch(System.Exception err){
//    //				throw(new WebExceptions.CompetitorUIException("Impossible de construire le rappel des paramètres dans le fichier Excel: "+err.Message));
//    //			}
//            #endregion

//            #region HTML
//            t.Append("<table  border=0 cellpadding=0 cellspacing=0 >");

//            #region Première ligne
//            try{
//                t.Append("\r\n\t<tr height=\"40px\">");

//                if(nbCompetitorMedia>0){
//                    t.Append("<td class=\"p2\" rowspan=2 valign=\"middle\" bgcolor=#ffffff>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//                    t.Append("<td class=\"p2\" rowspan=2 valign=\"middle\" bgcolor=#ffffff>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");
//                }
//                else{
//                    t.Append("<td class=\"p2\" bgcolor=#ffffff>"+GestionWeb.GetWebWord(1164,webSession.SiteLanguage)+"</td>");
//                    t.Append("<td class=\"p2\" bgcolor=#ffffff>"+tab[ResultConstantes.TOTAL_INDEX,ResultConstantes.MEDIA_LINE_LABEL_INDEX]+"</td>");
				
//                }

//                if(nbCompetitorMedia>0){
//                    string name= Convertion.ToHtmlString(GestionWeb.GetWebWord(1365,webSession.SiteLanguage)) ; 
//                    t.Append("<td colspan="+nbMedia+" class=\"P2\" nowrap>"+name+"</td>");
//                    t.Append("<td colspan="+nbCompetitorMedia+" class=\"P2\" nowrap>"+GestionWeb.GetWebWord(1366,webSession.SiteLanguage)+"</td>");
//                    t.Append("</tr>");
//                    t.Append("\r\n\t<tr height=\"20px\">");
//                }


//                for(i=debResult;i<nbColTab;i++){
//                    if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                        t.Append("<td class=\"p2\" bgcolor=#ffffff>"+tab[i,2]+"</td>");	
//                    }else{
//                        t.Append("<td  style=\"BORDER-LEFT: #ffffff solid\" class=\"p2\" >"+tab[i,2]+"</td>");
//                        oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                    }
//                }
//                t.Append("</tr>");
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de contruire la première ligne du tableau dans le fichier Excel",err));
//            }
//            #endregion

//            #region Tableau de résultats
//            oldProduct=-1;
//            try{
//                for(j=ResultConstantes.TOTAL_LINE_INDEX;j<nbline;j++){
//                    for(i=0;i<nbColTab;i++){
//                        switch(i){	
//                                #region Level 1
//                            case  ResultConstantes.LABELL1_INDEX:							
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    if((long)tab[ResultConstantes.FIRST_COLUMN_T0_SHOW_INDEX,j]==ResultConstantes.TOTAL_IDENTIFICATION){
//                                        classCss="acl11";
//                                        t.Append("\r\n\t<tr height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">"+tab[ResultConstantes.LABELL1_INDEX,j]+"</td>");
//                                        i=ResultConstantes.TOTAL_INDEX-1;	
//                                    }
//                                    else{
//                                        classCss="acl11";							
//                                        t.Append("\r\n\t<tr  height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">"+tab[ResultConstantes.LABELL1_INDEX,j]+"</td>");
//                                        i=ResultConstantes.TOTAL_INDEX-1;
//                                    }	
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }				 
//                                break;
//                                #endregion

//                                #region Level 2
//                            case ResultConstantes.LABELL2_INDEX:
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    classCss="acl21";
//                                    t.Append("\r\n\t<tr  height=\"20px\" >\r\n\t\t<td class=\""+classCss+"\">&nbsp;&nbsp;&nbsp;"+tab[ResultConstantes.LABELL2_INDEX,j]+"</td>");
//                                    i=ResultConstantes.TOTAL_INDEX-1;
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }	
//                                break;
//                                #endregion

//                                #region Level 3
//                            case ResultConstantes.LABELL3_INDEX:
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    classCss="acl31";
//                                    t.Append("\r\n\t<tr  height=\"20px\">\r\n\t\t<td class=\""+classCss+"\">&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;"+tab[ResultConstantes.LABELL3_INDEX,j]+"</td>");
//                                    i=ResultConstantes.TOTAL_INDEX-1;
//                                }
//                                break;
//                                #endregion

//                                #region Total
//                            case ResultConstantes.TOTAL_INDEX:	
//                                if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                    totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                    if(totalUnit.Trim().Length==0 || totalUnit==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
//                                    t.Append("\r\n\t<td class=\""+classCss+"\">"+totalUnit+"</td>");
//                                }else if(tab[i,j]!=null){
//                                    if(tab[i,j].GetType()==typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        if(j<nbline){										
//                                            i=nbColTab;
//                                        }
//                                    }
//                                }	
//                                break;
//                                #endregion
						
//                                #region Résultat
//                            case ResultConstantes.FIRST_MEDIA_INDEX:
//                                for(i=debResult; i<nbColTab;i++){
//                                    if(tab[i,j]!=null && tab[i,j].GetType()!=typeof(TNS.AdExpress.Constantes.FrameWork.MemoryArrayNotShowLine)){
//                                        endBalise=true;
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                        if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="";
//                                        if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                                            t.Append("\r\n\t<td class=\""+classCss+"\">"+totalUnit+"</td>");
																				
//                                        }else{
//                                            t.Append("\r\n\t<td class=\""+classCss+"\" style=\"BORDER-LEFT: #ffffff solid;\" >"+totalUnit+"</td>");
//                                            oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                                        }
//                                    }
//                                }	
//                                if(endBalise){
//                                    t.Append("</tr>");
//                                    endBalise=false;
//                                }
//                                break;
//                                #endregion
//                        }						
//                    }
//                }
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de construire le tableau de résultat dans le fichier Excel",err));
//            }
//            #endregion

//            t.Append("</table>");
//            #endregion

//            return (t.ToString());
//        }


//        /// <summary>
//        /// Génère le code excel pour la synthèse de l'alerte concurrentielle
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
//        /// Génère le code html pour le fichier Excel des données brutes d'un module concurentielle
//        /// </summary>
//        /// <param name="page">Page utilisée pour montrer le résultat</param>
//        /// <param name="tab">Tableau de données du résultat</param>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Code HTML pour fichier Excel</returns>
//        public static string GetRawExcel(Page page,object[,] tab,WebSession webSession){

//            #region variables
//            System.Text.StringBuilder t = new System.Text.StringBuilder(20000);
//            int nbColTab=tab.GetLength(0),j,i;
//            int nbline=tab.GetLength(1);
//            //string classCss="";
//            int oldProduct=-1;
//            //bool endBalise=false;
//            string totalUnit="";
//            int debResult=ResultConstantes.FIRST_MEDIA_INDEX;
//            int nbSelectionGroup=1;
//            string tmp="";
//            #endregion

//            #region Nombre de groupe
//            try{
//                while(webSession.CompetitorUniversMedia[nbSelectionGroup]!=null){
//                    nbSelectionGroup++;
//                }
//                nbSelectionGroup--;
//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de déterminer le nombre de groupes de supports",err));
//            }

//            tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION],CustomerConstantes.Right.type.mediaAccess);
//            int nbMedia=tmp.Split(',').GetLength(0);
//            if(nbMedia>1)nbMedia++;

//            int nbCompetitorMedia=0;
//            i=1;
//            while(webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i]!=null){
//                tmp=webSession.GetSelection((TreeNode) webSession.CompetitorUniversMedia[ResultConstantes.N_UNIVERSE_POSITION+i],CustomerConstantes.Right.type.mediaAccess);
//                nbCompetitorMedia+=tmp.Split(',').GetLength(0);
//                if(tmp.Split(',').GetLength(0)>1)nbCompetitorMedia++;
//                i++;
//            }
//            #endregion

//            #region Analyse Panoramique
//            if(nbSelectionGroup<2){
//                // On ne montre pas le sous total
//                debResult++;
//            }
//            #endregion

//            #region Rappel des paramètres
//            t.Append(ExcelFunction.GetExcelHeader(webSession,true,true,true,true,""));
//            #endregion

//            #region Nombre de niveaux
//            int nbLevels=Functions.ProductDetailLevel.GetLevelNumber(webSession);
//            string levels = Functions.ProductDetailLevel.LevelProductToExcelString(webSession);
//            string[] levelsTab = levels.Split('/');
//            #endregion

//            #region HTML
//            t.Append("<table  border=1 cellpadding=0 cellspacing=0 >");

//            #region Première ligne
//            try{
//                if(nbCompetitorMedia>0){
//                    t.Append("<tr>");
//                    t.Append("<td colspan="+(nbLevels+1)+"></td>");
//                    t.Append("<td colspan="+nbMedia+" >"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1365,webSession.SiteLanguage))+"</td>");
//                    t.Append("<td colspan="+nbCompetitorMedia+" >"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1366,webSession.SiteLanguage))+"</td>");
//                    t.Append("</tr>");
//                }
//                t.Append("<tr>");
//                for(int k=0;k<nbLevels;k++){
//                    t.Append("<td>"+levelsTab[k].ToString()+"</td>");
//                }
//                t.Append("<td>"+GestionWeb.GetWebWord(805,webSession.SiteLanguage)+"</td>");
//                for(i=debResult;i<nbColTab;i++){
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
//                throw(new WebExceptions.CompetitorUIException("Impossible de contruire la première ligne du tableau dans le fichier Excel",err));
//            }
//            #endregion

//            #region Tableau de résultats
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

//                                case ResultConstantes.TOTAL_INDEX:
//                                    if(tab[i,j]!=null){
//                                        totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                        if(totalUnit.Trim().Length==0 || totalUnit==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="0";
//                                        t.Append("<td>"+totalUnit+"</td>");
//                                    }
//                                    break;

//                                case ResultConstantes.FIRST_MEDIA_INDEX:
//                                    for(i=debResult; i<nbColTab;i++){
//                                        if(tab[i,j]!=null){
//                                            totalUnit= WebFunctions.Units.ConvertUnitValueAndPdmToString(tab[i,j].ToString(),webSession.Unit,webSession.Percentage);
//                                            if(totalUnit.Trim().Length==0 || totalUnit.Trim()==",00" ||totalUnit.Trim()==",000"|| totalUnit.Trim()=="0,00" || totalUnit=="Non Numérique" || totalUnit=="00 H 00 M 00 S")totalUnit="0";
//                                            if((int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX]==oldProduct){
//                                                t.Append("<td>"+totalUnit+"</td>");
																			
//                                            }else{
//                                                t.Append("<td>"+totalUnit+"</td>");
//                                                oldProduct=(int)tab[i,ResultConstantes.MEDIA_GROUP_LINE_INDEX];
//                                            }
//                                        }
//                                    }	
//                                    break;
//                            }
//                        }
//                    }
//                    t.Append("</tr>");
//                }

//            }
//            catch(System.Exception err){
//                throw(new WebExceptions.CompetitorUIException("Impossible de construire le tableau de résultat dans le fichier Excel",err));
//            }
//            #endregion

//            t.Append("</table>");
//            #endregion

//            return (t.ToString());
//        }
//        #endregion

//        #region Méthodes internes
//        /// <summary>
//        /// Savoir si l'on affiche la creation
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        ///<param name="level">Niveau affichage</param>
//        /// <returns>Texte du niveau de détail produi</returns>
//        private static bool DisplayCreationBoolean(WebSession webSession,int level){
//            bool displayCreation=false;
////			string Vehicle = ((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
////			ClassificationCst.DB.Vehicles.names vehicleType = (ClassificationCst.DB.Vehicles.names)int.Parse(Vehicle);
////			if(vehicleType==ClassificationCst.DB.Vehicles.names.outdoor) return false;
//            try
//            {
//                switch(webSession.PreformatedProductDetail){
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
//                        displayCreation=false;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
//                        displayCreation=false;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:					
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
//                        displayCreation=true;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=true;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;
//                        else{
//                            displayCreation=true;
//                        }
						
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
//                        displayCreation=true;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=true;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL3_INDEX){
//                            displayCreation=true;
//                        }
//                        else{
//                            displayCreation=true;
//                        }
//                        break;

//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;
//                        else if(level==ResultConstantes.IDL3_INDEX){
//                            displayCreation=true;
//                        }
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL3_INDEX){
//                            displayCreation=true;
//                        }
//                        else{
//                            displayCreation=true;
//                        }
//                        break;

//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
//                        displayCreation=false;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
//                        displayCreation=false;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL3_INDEX){
//                            displayCreation=true;
//                        }
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
//                        displayCreation=false;
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=false;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else{
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;	
//                        else if(level==ResultConstantes.IDL3_INDEX)
//                            displayCreation=false;	
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;	
//                        else if(level==ResultConstantes.IDL3_INDEX)
//                            displayCreation=false;	
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
//                        if(level==ResultConstantes.IDL1_INDEX)
//                            displayCreation=false;
//                        else if(level==ResultConstantes.IDL2_INDEX)
//                            displayCreation=true;						
//                        else
//                        {
//                            displayCreation=true;
//                        }
//                        break;
//                    default:						
//                        break;
//                }
//            }
//            catch(Exception){
				
//            }
//            return displayCreation;
//        }


//        /// <summary>
//        /// Obtient le texte du niveau de détail produit
//        /// </summary>
//        /// <param name="webSession">Session du client</param>
//        /// <returns>Texte du niveau de détail produi</returns>
//        private static int LevelForCreation(WebSession webSession){
//            try{
//                switch(webSession.PreformatedProductDetail){
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
//                        return(ResultConstantes.IDL1_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
//                        return(ResultConstantes.IDL1_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
//                        return(ResultConstantes.IDL1_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
//                        return(ResultConstantes.IDL1_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
//                        return(ResultConstantes.IDL2_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
//                        return(ResultConstantes.IDL3_INDEX);
//                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
//                        return(ResultConstantes.IDL3_INDEX);
//                    default:
//                        return(-1);
//                }
//            }
//            catch(Exception){
//                return(-1);
//            }
//        }	

//        #endregion

//    }
//}

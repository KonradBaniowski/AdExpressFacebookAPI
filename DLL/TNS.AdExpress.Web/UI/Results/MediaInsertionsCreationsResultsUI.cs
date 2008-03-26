#region Information
// Auteur : Guillaume Ragneau
// Créé le :
// Modifié le : 22/07/2004
//	06/07/2005	K.Shehzad		Addition of Agglomeration colunm for Outdoor creations 
//	12/08/2005	G. Facon		Nom de Fonction et Gestion des exceptions
//	26/01/2006	D. V. Mussuma	Intégration des  accroches et régies au du niveau de détail média
//	09/08/2006	g Ragneau	Suppression du bouton excel car ajout du menu contextuel dans la page.
#endregion

#region namespace
using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Collections.Specialized;
using System.Web.UI;

using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions.Script;
using CstCustomerSession=TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.FrameWork;
using FrameWorkResults=TNS.AdExpress.Constantes.FrameWork.Results;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
using TNS.AdExpress.Domain.Level;
#endregion

namespace TNS.AdExpress.Web.UI.Results{

	/// <summary>
	/// Génère le code HTML des tableaux présentants le détails des insertions d'un support, d'une catégorie ou dun média
	/// </summary>
	public class MediaInsertionsCreationsResultsUI{
		
		#region Constantes		
		const string ID_SLOGAN = "ID_SLOGAN";
		const string ASSOCIATED_FILE = "ASSOCIATED_FILE";
		#endregion

		#region HTML UI	

		#region Presse
		/// <summary>
		/// Construit le code HTML du tableau présentant le détail des insertions Presse avec les créations associées
		///		data vide : message d'erreur
		///		data non vide :
		///			Génération du code nécessaire à l'appel de l'export excel
		///			Script d'ouverture d'une PopUp de zoom sur une création Presse
		///			Construction du tableau HTML présentant le détail des insertions avec la hiérarchie 
		///			vehicle > Catégorie > Support > date > page
		/// </summary>
		/// <param name="data">Tableau des données insertions</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page de résultat</param>
		/// <param name="periodBeginning">Période de début des calcul</param>
		/// <param name="periodEnd">Période de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du média</param>
		/// <returns>Code HTML des insertions Presse</returns>
		/// <remarks>
		/// Utilise les méthodes : 
		///		TNS.AdExpress.Web.Functions.Script.OpenPressCreation()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUIPress(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd,string idVehicle){

			StringBuilder HtmlTxt = new StringBuilder(1000);
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			int i = 0;
			string oldDate = "";
			string oldPage="";
			bool first=true;


			#region Pas de données
			if (data[0,0]==null){
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
			}
			#endregion

			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail insertion presse avec gestion des colonnes génériques
					
				#region variables
				string oldLabelLevel1=null, oldLabelLevel2=null,oldLabelLevel3=null;	
				bool isNewLine=false;		
				bool isInsertionNewCells=false;		
				int j=0;
				bool hasLine=false;
				#endregion									

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
				#endregion

				#region Tableaux d'insertions
					
				//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
				for( i = 1; i<data.GetLength(0);i++)
				{//A partir de la ligne 1 du tableau
						
					isNewLine=true;	
					hasLine=false;
					isInsertionNewCells=true;
					string tempVisualString="";
					string tempInsertionDetailString="";
					for(j=0; j<data.GetLength(1);j++){
							
						#region 1er Niveau de regroupement 

						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[0],data,0,j,i,oldLabelLevel1)){														

							//Affiche libellé élément de niveau 1
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
							HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
							HtmlTxt.Append("<td>");
							HtmlTxt.Append("<!-- fleche -->");
							HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
							HtmlTxt.Append("<!-- message -->");
                            HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\">");
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</td>");
							HtmlTxt.Append("</table>");
							HtmlTxt.Append("</td>");
							HtmlTxt.Append("</TR>");																						
							if(data[i,j]!=null)oldLabelLevel1=data[i,j].ToString();
							else oldLabelLevel1=null;								
							oldLabelLevel2=null;
							oldLabelLevel3=null;
							first=true;
						}	
												
						#endregion

						#region 2eme Niveau de regroupement 
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>1 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[1],data,0,j,i,oldLabelLevel2)){
						
							//Affiche libellé élément de niveau 2							
							HtmlTxt.Append("<TR vAlign=\"top\">");
							HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle2\">");
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");							
							if(data[i,j]!=null)oldLabelLevel2=data[i,j].ToString();
							else oldLabelLevel2=null;								
							oldLabelLevel3=null;
							first=true;
						}
							
										
						#endregion						

						#region 3eme Niveau de regroupement 
																								
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>2 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[2],data,0,j,i,oldLabelLevel3)){
							//Affiche libellé élément de niveau 3															 	
							HtmlTxt.Append("<TR vAlign=\"top\">");
							HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle3\">");
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");							
							if(data[i,j]!=null)oldLabelLevel3=data[i,j].ToString();
							else oldLabelLevel3=null;	
							first=true;							
						}	
										
						#endregion												
							
					}						
					
					#region Corps du tableau
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0 								
						){
						
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(j=0; j<data.GetLength(1);j++){
								if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									){									

									#region Nouvelle version colonne générique pour correction problème affichage visuel
									hasLine = true;
									if(isNewLine){
											
										HtmlTxt.Append("<tr class=\"popupinsertionligne\">");//Debut  Nouvelle ligne
                                        HtmlTxt.Append("<td " + ((!first) ? "class=\"creationVioletTopBorder\"" : "") + "><TABLE cellSpacing=\"0\" cellPadding=\"0\" border=\"0\"><tr>");////Debut tableau contenant cellules visuels et détail insertion
										isNewLine=false;
										first=false;
											
									}
									//Cas gestion visuels
									if(data[0,j].ToString().CompareTo("VISUAL")==0){
																					
										if (  data[i,j]==null || ((string)data[i, j]).CompareTo("")==0
											|| ((CstClassification.DB.Vehicles.names) int.Parse(idVehicle)== TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG)== null)
											|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG) == null)
											) {//|| data[i,j].ToString().Length==0
											//Pas de créations
											tempVisualString="<td class=\"txtViolet12Bold\" valign=\"top\">"+GestionWeb.GetWebWord(843,webSession.SiteLanguage)+"</td>";
										}
										else{
											//Affichage de chaque création
											tempVisualString="";
											string[] files = ((string)data[i,j]).Split(',');
											foreach(string str in files){
												tempVisualString+="<td valign=\"center\" style=\"WIDTH: 3px\"><a class=\"image\" href=\"javascript:openPressCreation('"+((string)data[i, j]).Replace("/Imagette","")+"');\"><IMG border=\"0\" src=\""+str+"\"></a></td>";
											}
										}
									}	//Autres cas
									else{
										if(isInsertionNewCells){
											tempInsertionDetailString="<td valign=\"top\"><TABLE width=\"240\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">";//Debut cellule affichage du détail de l'insertion
										}
											
										if(data[i,j]!=null){
											tempInsertionDetailString += "<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(currentColumn.WebTextId, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, j].ToString() + "</td></tr>";
										}
										else{
											tempInsertionDetailString += "<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(currentColumn.WebTextId, webSession.SiteLanguage) + "</td><td nowrap>: &nbsp;</td></tr>";
										}
																					
										isInsertionNewCells=false;
									}
																	
									#endregion
								}																												
							}
								
						}
						if(hasLine){
							if(tempInsertionDetailString.Length>0)tempInsertionDetailString+="</TABLE></td>";//Fin cellule affichage du détail de l'insertion
								
							HtmlTxt.Append(tempVisualString+tempInsertionDetailString);//Ajout cellules visuels et détails insertions;
							HtmlTxt.Append("</tr></TABLE></td>");//Fin tableau contenant visuels et détail insertion
							HtmlTxt.Append("</tr>");////Fin Nouvelle ligne								
						}
						tempInsertionDetailString="";
						tempVisualString="";					
					}	
					#endregion
						
				}
				#endregion
					
				

				#endregion
			
			}else{

				#region Détail insertion presse sans gestion des colonnes génériques
				
				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
				#endregion

				#region Construction du tableau
				i = 0;
				string currentMedia="";
				string currentCategory="";
		
				#region Tableau
				while (i<data.GetLength(0) && data[i,0]!=null){
			
					currentCategory = data[i,CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX].ToString();

					#region Rappel Catégorie
					HtmlTxt.Append("<TR>");
					HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
					HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
					HtmlTxt.Append("<td>");
					HtmlTxt.Append("<!-- fleche -->");
					HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
					HtmlTxt.Append("<!-- message -->");
                    HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\">");
					HtmlTxt.Append(currentCategory);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</td>");
					HtmlTxt.Append("</table>");
					HtmlTxt.Append("</td>");
					HtmlTxt.Append("</TR>");
					#endregion
			
					#region Niveau Catégorie
					while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){

						#region Niveau Support

						#region Rappel support
						currentMedia = data[i,CstWeb.PressInsertionsColumnIndex.MEDIA_INDEX].ToString();
						HtmlTxt.Append("<TR vAlign=\"top\">");
						HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle2\">");
						HtmlTxt.Append(currentMedia);
						HtmlTxt.Append("</TD>");
						HtmlTxt.Append("</TR>");
						#endregion

						#region Détail du support courant
						oldDate = "";
							oldPage="";
							first = true;
						while(i<data.GetLength(0) && data[i,0]!=null && currentMedia.CompareTo(data[i,CstWeb.PressInsertionsColumnIndex.MEDIA_INDEX].ToString())==0){
							if (oldDate.CompareTo(data[i,CstWeb.PressInsertionsColumnIndex.DATE_INDEX].ToString())!=0){
								//nouvelle date => nouvelle entête
								first = true;
								oldDate=(string)data[i,CstWeb.PressInsertionsColumnIndex.DATE_INDEX];
								HtmlTxt.Append("<TR vAlign=\"top\">");
								HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle3\">");
								HtmlTxt.Append(oldDate);
								HtmlTxt.Append("</TD>");
								HtmlTxt.Append("</TR>");
							}
							if (data[i, CstWeb.PressInsertionsColumnIndex.MEDIA_PAGING_INDEX].ToString().CompareTo(oldPage)!=0){
								//page différente de la précédente
								oldPage = data[i,CstWeb.PressInsertionsColumnIndex.MEDIA_PAGING_INDEX].ToString();
								HtmlTxt.Append("<tr class=\"popupinsertionligne\">");
                                HtmlTxt.Append("<td " + ((!first) ? "class=\"creationVioletTopBorder txtViolet11Bold\"" : "") + " class=\"txtViolet11Bold\">Page " + oldPage + "</td>");
								HtmlTxt.Append("</tr>");
								first = false;
							}
							HtmlTxt.Append("<tr class=\"popupinsertionligne\"><td ><TABLE cellSpacing=\"0\" border=\"0\"><tr>");
							if (((string)data[i, CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]).CompareTo("")==0
								|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_PRESS_CREATION_ACCESS_FLAG) == null)
								|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG) == null)
								){
								//Pas de créations
								HtmlTxt.Append("<td class=\"txtViolet12Bold\" valign=\"top\">"+GestionWeb.GetWebWord(843,webSession.SiteLanguage)+"</td>");
							}
							else{
								//Affichage de chaque création
								string[] files = ((string)data[i, CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]).Split(',');
								foreach(string str in files){
									HtmlTxt.Append("<td valign=\"center\" width=\"1%\"><a class=\"image\" href=\"javascript:openPressCreation('"+((string)data[i, CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]).Replace("/Imagette","")+"');\"><IMG border=\"0\" src=\""+str+"\"></a></td>");
								}
							}
							//affichage du détail de l'insertion
							HtmlTxt.Append("<td valign=\"top\"><TABLE width=\"240\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(176, webSession.SiteLanguage) + "</td><td width=\"550\">: " + data[i, CstWeb.PressInsertionsColumnIndex.ADVERTISER_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(174, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.GROUP_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(468, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.PRODUCT_INDEX].ToString() + "</td></tr>");
							if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
								HtmlTxt.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1881, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString() + "</td></tr>");
							if(!webSession.isCompetitorAdvertiserSelected()){
								HtmlTxt.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1384, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString() + "</td></tr>");
								HtmlTxt.Append("<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1383, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString() + "</td></tr>");
							}
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.FORMAT_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(469, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.AREA_PAGE_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(561, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.COLOR_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(471, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.EXPENDITURE_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(472, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.PressInsertionsColumnIndex.LOCATION_INDEX].ToString() + "</td></tr>");
							HtmlTxt.Append("</TABLE></td>");

							HtmlTxt.Append("</tr></TABLE></td>");
							HtmlTxt.Append("</tr>");
							i++;
						}
						#endregion

						#endregion

					}
					#endregion

				}
				#endregion
			
				#endregion
		
				#endregion				
		
			}
			//fin du tableau générale
			HtmlTxt.Append("</TABLE>");

			return HtmlTxt.ToString();
			
		}
		#endregion	

		#region Radio
		/// <summary>
		/// Retourne le code html affichant le détails des insertions radio:
		///		data vide : code HTML d'un message d'erruer
		///		data non vide : code HTML du tableau présentant le détail radio insertion par insertion
		///			Génération du code d'export Excel
		///			Enregistrement du script d'ouverture de "AccessDownloadCreationsPopUp.aspx"
		///			Rappel des paramètres
		///			Génération du tableau des insertions ordonnées par Catégorie > Support > Date
		/// </summary>
		/// <param name="data">Tableau contenant les données à afficher</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page de retour</param>
		/// <param name="periodBeginning">Période de début des calculs</param>
		/// <param name="periodEnd">Période de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUIRadio(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string idVehicle){
			
			StringBuilder HtmlTxt = new StringBuilder(1000);
			string ColSpan="13";
			const string CLASSE_1="p6";
			const string CLASSE_2="p7";
			string oldDate = "";
			bool first = true;
			string classe="";			
			string tempAssociatedFile = "";
			int i=0, indexIdSloganCol = -1;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail spots radio avec gestion des colonnes génériques
					
				#region Variables
				string oldLabelLevel1=null, oldLabelLevel2=null,oldLabelLevel3=null;	
				bool isNewLine=false;					
				int j=0;
				
				string paramColSpan="";
				int colToShow=0;
				#endregion
					
				#region Début du tableau 
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"left\" border=\"0\">");					
				#endregion
					
				#region Pas de données à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
				}
				#endregion				

				#region Tableaux d'insertions
				//Pour fusion de colonnes
				if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
					foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
						for(int c=0; c<data.GetLength(1);c++){
							if( (currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,c].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
								|| (currentColumn.DataBaseAliasField!=null && data[0,c].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0)
								){
								colToShow++;
							}
							if (data[0, c]!=null && data[0, c].ToString().CompareTo(ID_SLOGAN) == 0)
								indexIdSloganCol = c;
						}
					}
					paramColSpan=colToShow.ToString();
				}
				//Pour premieère ligne libellés colonnes
				if((webSession.DetailLevel==null || webSession.DetailLevel.Levels==null || webSession.DetailLevel.Levels.Count==0)
					&& (webSession.GenericInsertionColumns!=null &&  webSession.GenericInsertionColumns.Columns!=null&& webSession.GenericInsertionColumns.Columns.Count>0 )
					)isNewLine=true;

				//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
				for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
					for(j=0; j<data.GetLength(1);j++){
							
						#region 1er Niveau de regroupement 

						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[0],data,0,j,i,oldLabelLevel1)){														

							//Affiche libellé élément de niveau 1
							HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
							HtmlTxt.Append("<tr><td colSpan=\""+paramColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");//Si affiche accroche
							HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
                            HtmlTxt.Append("<TD class=\"insertionCategory creationpopUpBackGround\">");
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD></tr>");
							HtmlTxt.Append("</table></td></TR>");								
							if(webSession.DetailLevel.Levels.Count==0)isNewLine=true;//S'il n'y a qu'un niveau il gère l'affichage des libellés
							if(data[i,j]!=null)oldLabelLevel1=data[i,j].ToString();
							else oldLabelLevel1=null;

							if(webSession.DetailLevel.Levels.Count==1)isNewLine=true;
							oldLabelLevel2=null;
							oldLabelLevel3=null;
						}	
												
						#endregion

						#region 2eme Niveau de regroupement
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>1 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[1],data,0,j,i,oldLabelLevel2)){
						
							//Affiche libellé élément de niveau 2
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+paramColSpan+"\">"); 
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");							
							isNewLine=true;
							if(data[i,j]!=null)oldLabelLevel2=data[i,j].ToString();
							else oldLabelLevel2=null;								
							oldLabelLevel3=null;
						}
							
										
						#endregion

						#region En-têtes de tableaux

						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							if(isNewLine){
								HtmlTxt.Append("<tr>");
								foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
									for(int k=0; k<data.GetLength(1);k++){
										if(data[0,k]!=null && ( (currentColumn.DataBaseAliasField==null & currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
											|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
											){
											HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
										}
									}
								}
								HtmlTxt.Append("</tr>");
								
							}
						}					
						#endregion

						#region 3eme Niveau de regroupement 
																								
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>2 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[2],data,0,j,i,oldLabelLevel3)){
							//Affiche libellé élément de niveau 3							
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+paramColSpan+"\">&nbsp;&nbsp;");								
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>"); 																
							if(data[i,j]!=null)oldLabelLevel3=data[i,j].ToString();
							else oldLabelLevel3=null;	
							first=true;
								
						}	
										
						#endregion
												
						isNewLine=false;
					}
						
						
					#region Corps du tableau
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0 								
						){
						HtmlTxt.Append("<tr>");
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(j=0; j<data.GetLength(1);j++){
								if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									){
									//Cas gestion fichier spot radio 
									if(data[0,j].ToString().CompareTo(ASSOCIATED_FILE)==0){
										if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG) != null && data[i, j] != null && data[i, j].ToString().Length > 0) {
											tempAssociatedFile = data[i, j].ToString();
											if (indexIdSloganCol>-1 && data[i, indexIdSloganCol] != null && data[i, indexIdSloganCol].ToString().CompareTo("0") != 0)
												tempAssociatedFile = tempAssociatedFile + "," +data[i, indexIdSloganCol].ToString();											
											HtmlTxt.Append("<td class=\"" + classe + "\" nowrap><a href=\"javascript:openDownload('" + tempAssociatedFile + "','" + webSession.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/App_Themes/"+themeName+"/Images/Common/Picto_Radio.gif\"></a></td>");
										}
										else
											HtmlTxt.Append("<td class=\"" + classe + "\" nowrap></td>");
									}
									//Autres cas
									else{
										if (data[i, j] != null) {										
											HtmlTxt.Append("<td class=\"" + classe + "\" align=\"center\" nowrap>" + data[i, j].ToString() + "</td>");
										}
										else HtmlTxt.Append("<td class=\"" + classe + "\" align=\"center\" nowrap>&nbsp;" + "</td>");
									}
								}
							}
								
						}						
						HtmlTxt.Append("</tr>");
							
					}					
									
					#endregion

						
				}
				#endregion
					
				//Fin du tableau
				HtmlTxt.Append("</TABLE>");

				#endregion


			}else{

					#region Détail spots radio sans gestion des colonnes génériques

					#region Début du tableau
					HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\"");
					HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
					#endregion

					#region Pas de données à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
					}
					#endregion

					#region script d'ouverture d'une popUp de téléchargement
//					if (!page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
//						page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openDownload",WebFunctions.OpenDownload());
//					}
					#endregion

					if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected() ) {
						// si droit accroche
						ColSpan="16";								
					}else if(!Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected() ) 
						ColSpan="15";
					else ColSpan="13";

					#region Tableaux d'insertions
					i = 0;
					string currentCategory="";
					string currentMedia="";

					#region Niveau Catégorie
					while(i<data.GetLength(0) && data[i,0]!=null){
						currentCategory = data[i, CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString();
						
						#region Infos Catégorie
						HtmlTxt.Append("<tr><td colSpan=\""+ColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");
						HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
                        HtmlTxt.Append("<TD class=\"insertionCategory creationpopUpBackGround\">");
						HtmlTxt.Append(currentCategory);
						HtmlTxt.Append("</TD></tr>");
						HtmlTxt.Append("</table></td></TR>");							
						#endregion

						#region Niveau Support
						while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
							currentMedia = data[i, CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString();
							#region Infos Support
							HtmlTxt.Append("<TR>");
							//					  HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\"13\">");
							HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+ColSpan+"\">");
							HtmlTxt.Append(currentMedia);
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");							
							#endregion

							#region Entêtes de tableaux

							oldDate = "";
							 first = true;
							 classe="";

							HtmlTxt.Append("<tr>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
							if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
								HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,webSession.SiteLanguage)+"</td>");
							if(!webSession.isCompetitorAdvertiserSelected()){
								HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,webSession.SiteLanguage)+"</td>");
								HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,webSession.SiteLanguage)+"</td>");
							}
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(865,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(866,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(867,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");					
							HtmlTxt.Append("</tr>");
							#endregion

							#region Tableau de détail d'un support
							while(i<data.GetLength(0) && data[i,0]!=null && currentMedia.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString())==0){
								if (oldDate.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX].ToString())!=0){
									//nouvelle date
									first = true;
									oldDate=(string)data[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX];
									HtmlTxt.Append("<TR>"); 							
									HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+ColSpan+"\">&nbsp;&nbsp;");
									HtmlTxt.Append(oldDate);
									HtmlTxt.Append("</TD>");
									HtmlTxt.Append("</TR>"); 
								}
				
								if (first){
									first=false;
									classe=CLASSE_1;
								}
								else{
									first=true;
									classe=CLASSE_2;
								}
								if(data[i,CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString().CompareTo("")!=0
									&& webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG)!=null){
									tempAssociatedFile = data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString();
									if (data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX] != null && data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString().CompareTo("0") != 0)
										tempAssociatedFile = tempAssociatedFile + "," + data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString();											
									HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap><a href=\"javascript:openDownload('"+tempAssociatedFile+"','"+webSession.IdSession+"','"+idVehicle+"');\"><img border=\"0\" src=\"/App_Themes/"+themeName+"/Images/Common/Picto_Radio.gif\"></a></td>");
								}
								else{
									HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap></td>");
								}
								HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
								if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected()){
									if(data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
									else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
								}
						
								if(!webSession.isCompetitorAdvertiserSelected()){
									HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
									HtmlTxt.Append("<td class=\""+classe+"\" nowrap >"+data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
								}
								HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\">"+data[i,CstWeb.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_WAP_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td></tr>");
								HtmlTxt.Append("</tr>");

								i++;
								
							}
							#endregion

						}

						#endregion
			
					}
					#endregion

					#endregion

					HtmlTxt.Append("</TABLE>");
					#endregion					

			}

			return HtmlTxt.ToString();
		}
		#endregion		

		#region TV
		/// <summary>
		/// Retourne le code html affichant le détails des insertions TV:
		///		data vide : code HTML d'un message d'erruer
		///		data non vide : code HTML du tableau présentant le détail TV insertion par insertion
		///			Génération du code d'export Excel
		///			Enregistrement du script d'ouverture de "AccessDownloadCreationsPopUp.aspx"
		///			Rappel des paramètres
		///			Génération du tableau des insertions ordonnées par Catégorie > Support > Date
		/// </summary>
		/// <param name="data">Tableau contenant les données à afficher</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page de retour</param>
		/// <param name="periodBeginning">Période de début des calculs</param>
		/// <param name="periodEnd">Période de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUITV(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string idVehicle){
			StringBuilder HtmlTxt = new StringBuilder(1000);
			StringBuilder script = new StringBuilder(500);
			
			#region variables
            string ColSpan="13";
			string paramColSpan="11";
			int i = 0;
			string oldDate = "";
			bool first = true;
			string classe="";
			int colToShow=0;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
			#endregion

			#region constantes
			const string CLASSE_1="p6";
			const string CLASSE_2="p7";
			#endregion

			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail spots télévision avec gestion des colonnes génériques
					
				#region variables
				string oldLabelLevel1=null, oldLabelLevel2=null,oldLabelLevel3=null;	
				bool isNewLine=false;					
				int j=0;
					
				#endregion
					
				#region Début du tableau 
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"left\" border=\"0\">");					
				#endregion
					
				#region Pas de données à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
				}
				#endregion
					
				#region script d'ouverture d'une popUp de téléchargement
					//TODO Vérifier conséquences de cette suppression ( à cause AJAX)
//				if (!page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
//					page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openDownload",WebFunctions.OpenDownload());
//				}
				#endregion

				#region Tableaux d'insertions
				//Pour fusion de colonnes
				if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
					foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
						for(int c=0; c<data.GetLength(1);c++){
							if( (currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,c].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
								|| ( currentColumn.DataBaseAliasField!=null && data[0,c].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0)
								){
								colToShow++;
							}
						}
					}
					paramColSpan=colToShow.ToString();
				}
				//Pour premieère ligne libellés colonnes
				if((webSession.DetailLevel==null || webSession.DetailLevel.Levels==null || webSession.DetailLevel.Levels.Count==0)
					&& (webSession.GenericInsertionColumns!=null &&  webSession.GenericInsertionColumns.Columns!=null&& webSession.GenericInsertionColumns.Columns.Count>0 )
					)isNewLine=true;

				//Parcours du tableau de résultats ( ligne 0 <=> libellés ; ligne >0 <=>valeurs) 
				for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
					for(j=0; j<data.GetLength(1);j++){
							
						#region 1er Niveau de regroupement

						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[0],data,0,j,i,oldLabelLevel1)){														

							//Affiche libellé élément de niveau 1
							HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
							HtmlTxt.Append("<tr><td colSpan=\""+paramColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");//Si affiche accroche
							HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
                            HtmlTxt.Append("<TD class=\"insertionCategory creationpopUpBackGround\">");
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD></tr>");
							HtmlTxt.Append("</table></td></TR>");								
							if(webSession.DetailLevel.Levels.Count==0)isNewLine=true;//S'il n'y a qu'un niveau il gère l'affichage des libellés
							if(data[i,j]!=null)oldLabelLevel1=data[i,j].ToString();
							else oldLabelLevel1=null;

							if(webSession.DetailLevel.Levels.Count==1)isNewLine=true;
							oldLabelLevel2=null;
							oldLabelLevel3=null;
						}	
												
						#endregion

						#region 2eme Niveau de regroupement
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>1 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[1],data,0,j,i,oldLabelLevel2)){
						
							//Affiche libellé élément de niveau 2
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+paramColSpan+"\">"); 
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");							
							isNewLine=true;
							if(data[i,j]!=null)oldLabelLevel2=data[i,j].ToString();
							else oldLabelLevel2=null;								
							oldLabelLevel3=null;
						}
						#endregion

						#region En-têtes de tableaux
						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							if(isNewLine){
								HtmlTxt.Append("<tr>");
								foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
									for(int k=0; k<data.GetLength(1);k++){
										if(data[0,k]!=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
											|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
											){
											HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
										}
									}
								}
								HtmlTxt.Append("</tr>");
								
							}
						}					
						#endregion

						#region 3eme Niveau de regroupement
																								
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>2 
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[2],data,0,j,i,oldLabelLevel3)){
							//Affiche libellé élément de niveau 3							
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+paramColSpan+"\">&nbsp;&nbsp;");								
							if(data[i,j]!=null)HtmlTxt.Append(data[i,j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>"); 																
							if(data[i,j]!=null)oldLabelLevel3=data[i,j].ToString();
							else oldLabelLevel3=null;	
							first=true;
								
						}	
										
						#endregion
												
						isNewLine=false;
					}
						
						
					#region Corps du tableau
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0 								
						){
						HtmlTxt.Append("<tr>");
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(j=0; j<data.GetLength(1);j++){
								if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									){
									//Cas gestion fichier spot tv 
									if (data[0, j].ToString().CompareTo("ASSOCIATED_FILE") == 0) {
										if (data[i, j] != null && data[i, j].ToString().Length > 0
										&& (((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG) != null)
										|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OTHERS_CREATION_ACCESS_FLAG) != null))
										)
											HtmlTxt.Append("<td class=\"" + classe + "\" nowrap><a href=\"javascript:openDownload('" + data[i, j].ToString() + "','" + webSession.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/App_Themes/"+themeName+"/Images/Common/Picto_pellicule.gif\"></a></td>");
										else
											HtmlTxt.Append("<td class=\"" + classe + "\" nowrap></td>");
									}
									//Autres cas
									else {
										if (data[i, j] != null) HtmlTxt.Append("<td class=\"" + classe + "\" align=\"center\" nowrap>" + data[i, j].ToString() + "</td>");
										else HtmlTxt.Append("<td class=\"" + classe + "\" align=\"center\" nowrap>&nbsp;" + "</td>");
									}
								}
							}
								
						}						
						HtmlTxt.Append("</tr>");
							
					}		
					#endregion

						
				}
				#endregion
					
				//Fin du tableau
				HtmlTxt.Append("</TABLE>");

				#endregion
			
			}else{

				#region Détail spots télévision sans gestion des colonnes génériques
				
				
				if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected() ) {
					// si droit accroche
					ColSpan="16";
					paramColSpan="14";				
				}else if(!Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected() ) {
					ColSpan="15";
					paramColSpan="13";
				}else{ 
					ColSpan="13";
					paramColSpan="11";
				}

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
				#endregion

				#region Pas de données à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
				}
				#endregion
		
				#region script d'ouverture d'une popUp de téléchargement
//				if (!page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
//					page.ClientScript.RegisterClientScriptBlock(this.GetType(),"openDownload",WebFunctions.OpenDownload());
//				}
				#endregion

				#region Tableaux d'insertions
				
				string currentCategory="";
				string currentMedia="";

				#region Niveau Catégorie
				while(i<data.GetLength(0) && data[i,0]!=null){
					currentCategory = data[i, CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString();

					#region Infos Catégorie
					//HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
					HtmlTxt.Append("<tr><td colSpan=\""+paramColSpan+"\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");//Si affiche accroche
					HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
                    HtmlTxt.Append("<TD class=\"insertionCategory creationpopUpBackGround\">");
					HtmlTxt.Append(currentCategory);
					HtmlTxt.Append("</TD></tr>");
					HtmlTxt.Append("</table></td></TR>");							
					#endregion

					#region Niveau Support
					while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
						currentMedia = data[i, CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString();
						#region Infos Support
						HtmlTxt.Append("<TR>");
						HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\""+paramColSpan+"\">"); //Si affiche accroche
						HtmlTxt.Append(currentMedia);
						HtmlTxt.Append("</TD>");
						HtmlTxt.Append("</TR>");							
						#endregion

						#region Entêtes de tableaux

							oldDate = "";
						first = true;
							classe="";

						HtmlTxt.Append("<tr>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
						//Accroche
						if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,webSession.SiteLanguage)+"</td>");
						if(!webSession.isCompetitorAdvertiserSelected()){
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,webSession.SiteLanguage)+"</td>");
						}
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
						//Code écran
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(495,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");					
						HtmlTxt.Append("</tr>");
						#endregion

						#region Tableau de détail d'un support
						while(i<data.GetLength(0) && data[i,0]!=null && currentMedia.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString())==0){
							if (oldDate.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX].ToString())!=0){
								//nouvelle date
								first = true;
								oldDate=(string)data[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX];
								HtmlTxt.Append("<TR>");
								HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\""+paramColSpan+"\">&nbsp;&nbsp;");
								HtmlTxt.Append(oldDate);
								HtmlTxt.Append("</TD>");
								HtmlTxt.Append("</TR>"); 
							}
			
							if (first){
								first=false;
								classe=CLASSE_1;
							}
							else{
								first=true;
								classe=CLASSE_2;
							}
							if( data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX] != null && data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString().CompareTo("")!=0
								&& (((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_TV_CREATION_ACCESS_FLAG) != null)
								|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.others && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OTHERS_CREATION_ACCESS_FLAG) != null))
								){
								HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap><a href=\"javascript:openDownload('"+data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString()+"','"+webSession.IdSession+"','"+idVehicle+"');\"><img border=\"0\" src=\"/App_Themes/"+themeName+"/Images/Common/Picto_pellicule.gif\"></a></td>");
							}
							else{
								HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap></td>");
							}
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
							if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected() ){
								if(data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
								else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
							}
							if(!webSession.isCompetitorAdvertiserSelected()){
								HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
								HtmlTxt.Append("<td class=\""+classe+"\" nowrap >"+data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
							}
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td>");
							HtmlTxt.Append("</tr>");


							i++;
						}
						#endregion

					}

					#endregion
		
				}
				#endregion

				#endregion

				HtmlTxt.Append("</TABLE>");

				#endregion
			}
			



			return HtmlTxt.ToString();

		}
		#endregion

		#region Publicité extérieure
		/// <summary>
		/// Retourne le code html affichant le détails des insertions de la publicité extérieure:
		///		data vide : code HTML d'un message d'erruer
		///		data non vide : code HTML du tableau présentant le détail de la publicité extérieure insertion par insertion
		///			Génération du code d'export Excel
		///			Enregistrement du script d'ouverture de "AccessDownloadCreationsPopUp.aspx"
		///			Rappel des paramètres
		///			Génération du tableau des insertions ordonnées par Catégorie > Support > Date
		/// </summary>
		/// <param name="data">Tableau contenant les données à afficher</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page de retour</param>
		/// <param name="periodBeginning">Période de début des calculs</param>
		/// <param name="periodEnd">Période de fin des calculs</param>
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUIOutDoor(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string idVehicle){
			

			StringBuilder HtmlTxt = new StringBuilder(1000);
			string ColSpan="";
			bool first=true;
			string classe="";
			string oldDate = "";
			const string CLASSE_1="p6";
			const string CLASSE_2="p7";
			int i = 0;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

			#region Pas de données à afficher
			if (data[0, 0] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
			}
			#endregion

			//Pas de droit publicité extérieure
			if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)==null){
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"left\" border=\"0\">");
				HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage,1882));
				HtmlTxt.Append("</TABLE>");

				return HtmlTxt.ToString();
			}	
			
			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail des affiches avec gestion des colonnes génériques (Nouvelle version)

				#region variables
				string oldLabelLevel1 = null, oldLabelLevel2 = null, oldLabelLevel3 = null;
				bool isNewLine = false;
				bool isInsertionNewCells = false;
				int j = 0;
				bool hasLine = false;
				#endregion

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
				#endregion

				#region Tableaux d'insertions

				//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
				for (i = 1; i < data.GetLength(0); i++) {//A partir de la ligne 1 du tableau

					isNewLine = true;
					hasLine = false;
					isInsertionNewCells = true;
					string tempVisualString = "";
					string tempInsertionDetailString = "";
					for (j = 0; j < data.GetLength(1); j++) {

						#region 1er Niveau de regroupement

						if (webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 0
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[0], data, 0, j, i, oldLabelLevel1)) {

							//Affiche libellé élément de niveau 1
							HtmlTxt.Append("<TR>");
							HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
							HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
							HtmlTxt.Append("<td>");
							HtmlTxt.Append("<!-- fleche -->");
							HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
							HtmlTxt.Append("<!-- message -->");
                            HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\">");
							if (data[i, j] != null) HtmlTxt.Append(data[i, j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</td>");
							HtmlTxt.Append("</table>");
							HtmlTxt.Append("</td>");
							HtmlTxt.Append("</TR>");
							if (data[i, j] != null) oldLabelLevel1 = data[i, j].ToString();
							else oldLabelLevel1 = null;
							oldLabelLevel2 = null;
							oldLabelLevel3 = null;
							first = true;
						}

						#endregion

						#region 2eme Niveau de regroupement
						if (webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 1
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[1], data, 0, j, i, oldLabelLevel2)) {

							//Affiche libellé élément de niveau 2							
							HtmlTxt.Append("<TR vAlign=\"top\">");
							HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle2\">");
							if (data[i, j] != null) HtmlTxt.Append(data[i, j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");
							if (data[i, j] != null) oldLabelLevel2 = data[i, j].ToString();
							else oldLabelLevel2 = null;
							oldLabelLevel3 = null;
							first = true;
						}


						#endregion

						#region 3eme Niveau de regroupement

						if (webSession.DetailLevel != null && webSession.DetailLevel.Levels != null && webSession.DetailLevel.Levels.Count > 2
							&& IsNewDetailLevel((DetailLevelItemInformation)webSession.DetailLevel.Levels[2], data, 0, j, i, oldLabelLevel3)) {
							//Affiche libellé élément de niveau 3															 	
							HtmlTxt.Append("<TR vAlign=\"top\">");
							HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle3\">");
							if (data[i, j] != null) HtmlTxt.Append(data[i, j].ToString());
							else HtmlTxt.Append("&nbsp;");
							HtmlTxt.Append("</TD>");
							HtmlTxt.Append("</TR>");
							if (data[i, j] != null) oldLabelLevel3 = data[i, j].ToString();
							else oldLabelLevel3 = null;
							first = true;
						}

						#endregion
					}

					#region Corps du tableau
					if (webSession.GenericInsertionColumns != null && webSession.GenericInsertionColumns.Columns != null && webSession.GenericInsertionColumns.Columns.Count > 0
						) {

						foreach (GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns) {
							for (j = 0; j < data.GetLength(1); j++) {
								if (data[0, j] != null && ((currentColumn.DataBaseAliasField == null && currentColumn.DataBaseField != null && data[0, j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper()) == 0)
									|| (currentColumn.DataBaseAliasField != null && data[0, j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper()) == 0))
									) {

									#region Colonne générique 
									hasLine = true;
									if (isNewLine) {

										HtmlTxt.Append("<tr class=\"popupinsertionligne\">");//Debut  Nouvelle ligne
                                        HtmlTxt.Append("<td " + ((!first) ? "class=\"creationVioletTopBorder\"" : "") + "><TABLE cellSpacing=\"0\" cellPadding=\"0\" border=\"0\"><tr>");////Debut tableau contenant cellules visuels et détail insertion
										isNewLine = false;
										first = false;

									}
									//Cas gestion visuels
									if (data[0, j] != null && data[0, j].ToString().CompareTo(ASSOCIATED_FILE) == 0) {

										if (data[i, j] == null || data[i, j] == System.DBNull.Value || ((string)data[i, j]).CompareTo("") == 0
											|| (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG) == null)											
											) {
											//Pas de créations
											tempVisualString = "<td class=\"txtViolet12Bold\" valign=\"top\">" + GestionWeb.GetWebWord(843, webSession.SiteLanguage) + "</td>";
										}
										else {
											//Affichage de chaque création
											tempVisualString = "";
											string[] files = ((string)data[i, j]).Split(',');

											foreach (string str in files) {
												
												tempVisualString += "<td valign=\"center\" style=\"WIDTH: 3px\"><a class=\"image\" href=\"javascript:openPressCreation('" + ((string)data[i, j]).Replace("/Imagette", "") + "');\"><IMG border=\"0\" src=\"" + str + "\"></a></td>";
												
											}
										}
									}	//Autres cas
									else {
										if (isInsertionNewCells) {
											tempInsertionDetailString = "<td valign=\"top\"><TABLE width=\"240\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">";//Debut cellule affichage du détail de l'insertion
										}

										if (data[i, j] != null) {
											tempInsertionDetailString += "<tr valign=\"top\" nowrap><td>&nbsp;" + GestionWeb.GetWebWord(currentColumn.WebTextId, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, j].ToString() + "</td></tr>";
										}
										else {
											tempInsertionDetailString += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(currentColumn.WebTextId, webSession.SiteLanguage) + "</td><td>: &nbsp;</td></tr>";
										}

										isInsertionNewCells = false;
									}

									#endregion
								}
							}

						}
						if (hasLine) {
							if (tempInsertionDetailString.Length > 0) tempInsertionDetailString += "</TABLE></td>";//Fin cellule affichage du détail de l'insertion

							HtmlTxt.Append(tempVisualString + tempInsertionDetailString);//Ajout cellules visuels et détails insertions;
							HtmlTxt.Append("</tr></TABLE></td>");//Fin tableau contenant visuels et détail insertion
							HtmlTxt.Append("</tr>");////Fin Nouvelle ligne								
						}
						tempInsertionDetailString = "";
						tempVisualString = "";
					}
					#endregion

				}
				#endregion

				//fin du tableau générale
				HtmlTxt.Append("</TABLE>");

				#endregion

			}
			else{				

				#region Détail insertion presse sans gestion des colonnes génériques

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
				#endregion

				#region Construction du tableau
				i = 0;
				string currentMedia = "";
				string currentCategory = "";

				#region Tableau
				while (i < data.GetLength(0) && data[i, 0] != null) {

					currentCategory = (data[i, CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX].ToString() : "";

					#region Rappel Catégorie
					HtmlTxt.Append("<TR>");
					HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
					HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
					HtmlTxt.Append("<td>");
					HtmlTxt.Append("<!-- fleche -->");
					HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
					HtmlTxt.Append("<!-- message -->");
                    HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\">");
					HtmlTxt.Append(currentCategory);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</td>");
					HtmlTxt.Append("</table>");
					HtmlTxt.Append("</td>");
					HtmlTxt.Append("</TR>");
					#endregion

					#region Niveau Catégorie
					while (i < data.GetLength(0) && data[i, 0] != null && currentCategory.CompareTo(data[i, CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX].ToString()) == 0) {

						#region Niveau Support

						#region Rappel support
						currentMedia = (data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX].ToString() : "";
						HtmlTxt.Append("<TR vAlign=\"top\">");
						HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle2\">");
						HtmlTxt.Append(currentMedia);
						HtmlTxt.Append("</TD>");
						HtmlTxt.Append("</TR>");
						#endregion

						#region Détail du support courant
						oldDate = "";						
						first = true;
						while (i < data.GetLength(0) && data[i, 0] != null && currentMedia.CompareTo(data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX].ToString()) == 0) {
							if (oldDate.CompareTo(data[i, CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX].ToString()) != 0) {
								//nouvelle date => nouvelle entête
								first = true;
								oldDate = (data[i, CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] != null) ? (string)data[i, CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] : "";
								HtmlTxt.Append("<TR vAlign=\"top\">");
								HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle3\">");
								HtmlTxt.Append(oldDate);
								HtmlTxt.Append("</TD>");
								HtmlTxt.Append("</TR>");
							}
							HtmlTxt.Append("<tr class=\"popupinsertionligne\">");
                            HtmlTxt.Append("<td " + ((!first) ? "class=\"creationVioletTopBorder txtViolet11Bold\"" : "") + " class=\"txtViolet11Bold\">&nbsp;</td>");
							HtmlTxt.Append("</tr>");
							
							HtmlTxt.Append("<tr class=\"popupinsertionligne\"><td ><TABLE cellSpacing=\"0\" border=\"0\"><tr>");
							if (data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX]== null || ((string)data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX]).CompareTo("") == 0
								|| ( webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG) == null)								
								) {
								//Pas de créations
								HtmlTxt.Append("<td class=\"txtViolet12Bold\" valign=\"top\">" + GestionWeb.GetWebWord(843, webSession.SiteLanguage) + "</td>");
							}
							else {
								//Affichage de chaque création
								string[] files = ((string)data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX]).Split(',');
								foreach (string str in files) {
									HtmlTxt.Append("<td valign=\"center\" width=\"1%\"><a class=\"image\" href=\"javascript:openPressCreation('" + ((string)data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX]).Replace("/Imagette", "") + "');\"><IMG border=\"0\" src=\"" + str + "\"></a></td>");
								}
							}
							//affichage du détail de l'insertion							

							HtmlTxt.Append("<td valign=\"top\"><TABLE width=\"240\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(176, webSession.SiteLanguage) + "</td><td width=\"550\">: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(174, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(468, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX].ToString() : "") + "</td></tr>");
							if (Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
								HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1881, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString() : "") + "</td></tr>");
							if (!webSession.isCompetitorAdvertiserSelected()) {
								HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1384, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString() : "") + "</td></tr>");
								HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1383, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString() : "") + "</td></tr>");
							}
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1604, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1420, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1609, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX] != null) ? Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.SQLGenerator.SaleTypeOutdoor(data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX].ToString(), webSession.SiteLanguage)) : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1611, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX] != null) ? Convertion.ToHtmlString(data[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX].ToString()) : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1660, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(868, webSession.SiteLanguage) + "</td><td nowrap> : " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX].ToString() : "") + "</td></tr>");
							HtmlTxt.Append("</TABLE></td>");

							HtmlTxt.Append("</tr></TABLE></td>");
							HtmlTxt.Append("</tr>");
							i++;
							first = false;
						}
						#endregion

						#endregion

					}
					#endregion

				}
				#endregion

				#endregion

				//fin du tableau générale
				HtmlTxt.Append("</TABLE>");

				#endregion				
			}

			return HtmlTxt.ToString();

		}
		#endregion

        #region Marketing Direct
        /// <summary>
        /// Construit le code HTML du tableau présentant les versions du Marketing direct
        ///		data vide : message d'erreur
        ///		data non vide :
        ///			Génération du code nécessaire à l'appel de l'export excel
        ///			Script d'ouverture d'une PopUp de zoom sur une création MD
        ///			Construction du tableau HTML présentant le détail des insertions avec la hiérarchie 
        ///			vehicle > Catégorie > Support > semaine
        /// </summary>
        /// <param name="data">Tableau des données insertions</param>
        /// <param name="webSession">Session utilisateur</param>
        /// <param name="page">Page de résultat</param>
        /// <param name="periodBeginning">Période de début des calcul</param>
        /// <param name="periodEnd">Période de fin des calculs</param>
        /// <param name="idVehicle">Identifiant du média</param>
        /// <returns>Code HTML des insertions Presse</returns>
        /// <remarks>
        /// Utilise les méthodes : 
        ///		TNS.AdExpress.Web.Functions.Script.OpenPressCreation()
        ///		private static string GetUIEmpty(int language)
        /// </remarks>
        private static string GetUIMD(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string idVehicle, ListDictionary mediaImpactedList){

            StringBuilder HtmlTxt = new StringBuilder(1000);
            int i = 0;
            string oldDate = "";
            string oldPage = "";
            bool first = true;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

            #region Pas de données
            if (data[0, 0] == null){
                return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
            }
            #endregion

            #region Début du tableau
            HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" style=\"MARGIN-TOP: 0px; MARGIN-LEFT: 0px; MARGIN-RIGHT: 25px;\"");
            HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
            #endregion

            #region Construction du tableau
            i = 0;
            string currentMedia = "";
            string currentCategory = "";
            string[] mailContent=null;

            #region Tableau
            while (i < data.GetLength(0) && data[i, 0] != null){

                currentCategory = data[i, CstWeb.MDVersionsColumnIndex.CATEGORY_INDEX].ToString();

                #region Rappel Catégorie
                HtmlTxt.Append("<TR>");
                HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
                HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
                HtmlTxt.Append("<td>");
                HtmlTxt.Append("<!-- fleche -->");
                HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/"+themeName+"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
                HtmlTxt.Append("<!-- message -->");
                HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\">");
                HtmlTxt.Append(currentCategory);
                HtmlTxt.Append("</TD>");
                HtmlTxt.Append("</td>");
                HtmlTxt.Append("</table>");
                HtmlTxt.Append("</td>");
                HtmlTxt.Append("</TR>");
                #endregion

                #region Niveau Catégorie
                while (i < data.GetLength(0) && data[i, 0] != null && currentCategory.CompareTo(data[i, CstWeb.MDVersionsColumnIndex.CATEGORY_INDEX].ToString()) == 0){

                    #region Niveau Support

                    #region Rappel support
                    currentMedia = data[i, CstWeb.MDVersionsColumnIndex.MEDIA_INDEX].ToString();
                    HtmlTxt.Append("<TR vAlign=\"top\">");
                    HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle2\">");
                    HtmlTxt.Append(currentMedia);
                    HtmlTxt.Append("</TD>");
                    HtmlTxt.Append("</TR>");
                    #endregion

                    #region Détail du support courant
                    oldDate = "";
                    oldPage = "";
                    first = true;
                    while (i < data.GetLength(0) && data[i, 0] != null && currentMedia.CompareTo(data[i, CstWeb.MDVersionsColumnIndex.MEDIA_INDEX].ToString()) == 0){
                        if (oldDate.CompareTo(data[i, CstWeb.MDVersionsColumnIndex.DATE_INDEX].ToString()) != 0){
                            //nouvelle date => nouvelle entête
                            first = true;
                            oldDate = (string)data[i, CstWeb.MDVersionsColumnIndex.DATE_INDEX];
                            HtmlTxt.Append("<TR vAlign=\"top\">");
                            HtmlTxt.Append("<TD width=\"100%\" vAlign=\"center\" class=\"popuptitle3\">");
                            HtmlTxt.Append(oldDate);
                            HtmlTxt.Append("</TD>");
                            HtmlTxt.Append("</TR>");
                        }

                        if (first) {
                            HtmlTxt.Append("<tr class=\"popupinsertionligne\"><td ><TABLE cellSpacing=\"0\" border=\"0\"><tr>");
                            first = false;
                        }
                        else {
                            HtmlTxt.Append("<tr class=\"popupinsertionligne\">");
                            HtmlTxt.Append("<td class=\"creationVioletTopBorder txtViolet11Bold\"><TABLE cellSpacing=\"0\" border=\"0\"></td>");
                            HtmlTxt.Append("</tr>");
                        }

                        if (((string)data[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX]).CompareTo("") == 0
                            //|| ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_INTERNATIONAL_PRESS_CREATION_ACCESS_FLAG) == null)
                            )
                        {
                            //Pas de créations
                            //HtmlTxt.Append("<td class=\"txtViolet12Bold\" valign=\"top\">" + GestionWeb.GetWebWord(843, webSession.SiteLanguage) + "</td>");
                            HtmlTxt.Append("<td class=\"txtViolet12Bold\" valign=\"top\" width=\"240\">" + GestionWeb.GetWebWord(843, webSession.SiteLanguage) + "</td>");
                        }
                        else if ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing && webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_DIRECT_MARKETING_CREATION_ACCESS_FLAG) == null)
                            HtmlTxt.Append("");
                            //HtmlTxt.Append("<td class=\"txtViolet12Bold\" valign=\"top\">" + GestionWeb.GetWebWord(843, webSession.SiteLanguage) + "</td>");
                        else{
                            //Affichage de chaque création
                            string[] files = ((string)data[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX]).Split(',');
                            foreach (string str in files){
                                //HtmlTxt.Append("<td valign=\"center\" width=\"1%\"><a class=\"image\" href=\"javascript:openPressCreation('" + ((string)data[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX]).Replace("/imagette", "") + "');\"><IMG border=\"0\" src=\"" + str + "\"></a></td>");
                                HtmlTxt.Append("<td valign=\"top\" width=\"1%\"><a class=\"imageMD\" href=\"javascript:openPressCreation('" + ((string)data[i, CstWeb.MDVersionsColumnIndex.ASSOCIATED_FILE_INDEX]).Replace("/imagette", "") + "');\"><IMG border=\"0\" src=\"" + str + "\"></a></td>");
                            }
                        }
                        //affichage du détail de l'insertion
                        HtmlTxt.Append("<td valign=\"top\"><TABLE width=\"240\" cellSpacing=\"0\" border=\"0\" class=\"txtViolet11Bold\" valign=\"top\">"); //width=\"550\"
                        HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(176, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.ADVERTISER_INDEX].ToString() + "</td></tr>");
                        HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(468, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.PRODUCT_INDEX].ToString() + "</td></tr>");
                        HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(174, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.GROUP_INDEX].ToString() + "</td></tr>");
                        if (Functions.MediaDetailLevel.HasSloganRight(webSession))// si droit accroche
                            HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(1888, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.SLOGAN_INDEX].ToString() + "</td></tr>");
                        if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_POIDS_MARKETING_DIRECT) != null)
                            HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(2220, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.WEIGHT_INDEX].ToString() + "</td></tr>");
                        HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(1935, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.EXPENDITURE_INDEX].ToString() + "</td></tr>");
                        if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                            HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp;" + GestionWeb.GetWebWord(2216, webSession.SiteLanguage) + "</td><td nowrap>: " + Math.Round(double.Parse(data[i, CstWeb.MDVersionsColumnIndex.VOLUME_INDEX].ToString())) + "</td></tr>");

                        //Mail Content
                        if (data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() != "")
                            mailContent = data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString().Split(',');
                        else
                            mailContent = null;

                        //Affichage du détail pour chaque média
                        switch (data[i, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX].ToString()) {
                            case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                                HtmlTxt.Append(GetMailDetailPNA(webSession, data, i));
                                break;
                            case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                                HtmlTxt.Append(GetMailDetailCAGENRAL(webSession, data, i, mailContent));
                                break;
                            case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                                HtmlTxt.Append(GetMailDetailCAPRESSE(webSession, data, i, mailContent));
                                break;
                            case CstDB.Media.COURRIER_ADRESSE_GESTION:
                                HtmlTxt.Append(GetMailDetailCAGESTION(webSession, data, i, mailContent));
                                break;
                        }

                        #region Ancienne méthode
                        //if (mediaImpactedList["id_media"] != null && mediaImpactedList["id_media"].ToString() != "-1"){

                        //    switch (mediaImpactedList["id_media"].ToString()){

                        //        case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                        //            HtmlTxt.Append(GetMailContentListPNA(webSession,data,i));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                        //            HtmlTxt.Append(GetMailContentListCAGENRAL(webSession, data, i, mailContent));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                        //            HtmlTxt.Append(GetMailContentListCAPRESSE(webSession, data, i, mailContent));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_GESTION:
                        //            HtmlTxt.Append(GetMailContentListCAGESTION(webSession, data, i, mailContent));
                        //            break;
                        //        default:
                        //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                        //    }

                        //}
                        //else if (mediaImpactedList["id_category"] != null && mediaImpactedList["id_category"].ToString() != "-1"){

                        //    switch (mediaImpactedList["id_category"].ToString()){

                        //        case CstDB.Category.PUBLICITE_NON_ADRESSEE:
                        //            HtmlTxt.Append(GetMailContentListPNA(webSession, data, i));
                        //            break;
                        //        case CstDB.Category.COURRIER_ADRESSE:
                        //            switch (data[i, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX].ToString()) {
                        //                case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                        //                    HtmlTxt.Append(GetMailContentListCAGENRAL(webSession, data, i, mailContent));
                        //                    break;
                        //                case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                        //                    HtmlTxt.Append(GetMailContentListCAPRESSE(webSession, data, i, mailContent));
                        //                    break;
                        //                case CstDB.Media.COURRIER_ADRESSE_GESTION:
                        //                    HtmlTxt.Append(GetMailContentListCAGESTION(webSession, data, i, mailContent));
                        //                    break;
                        //            }
                        //            break;
                        //        default:
                        //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                        //    }

                        //}
                        //else{

                        //    switch (data[i, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX].ToString()) {
                        //        case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                        //            HtmlTxt.Append(GetMailContentListPNA(webSession, data, i));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                        //            HtmlTxt.Append(GetMailContentListCAGENRAL(webSession, data, i, mailContent));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                        //            HtmlTxt.Append(GetMailContentListCAPRESSE(webSession, data, i, mailContent));
                        //            break;
                        //        case CstDB.Media.COURRIER_ADRESSE_GESTION:
                        //            HtmlTxt.Append(GetMailContentListCAGESTION(webSession, data, i, mailContent));
                        //            break;
                        //    }

                        //}
                        #endregion

                        HtmlTxt.Append("</TABLE></td>");

                        HtmlTxt.Append("</tr></TABLE></td>");
                        HtmlTxt.Append("</tr>");
                        i++;
                    }
                    #endregion

                    #endregion

                }
                #endregion

            }
            #endregion

            #endregion

                        
            //fin du tableau générale
            HtmlTxt.Append("</TABLE>");

            return HtmlTxt.ToString();

        }
        #endregion	
		
		#region HTML UI (Nouvelle version D. Mussuma)
		/// <summary>
		/// Construction du code HTML affichant le détail des insertions d'un média (vehicle) quelqconque sur une certaine période respectant le type de période présent dans la session
		///		Extraction des données
		///		Construction du code HTML en fonction du média considéré
		/// </summary>
		/// <param name="webSession">Session du client</param>
        /// <param name="idMediaLevel1">Identifiant du média de niveau 1</param>
        /// <param name="idMediaLevel2">Identifiant du média de niveau 2</param>
        /// <param name="idMediaLevel3">Identifiant du média de niveau 3</param>
        /// <param name="idMediaLevel4">Identifiant du média de niveau 4</param>
		/// <param name="idAdvertiser">Identifiant annonceur</param>
		/// <param name="page">Page</param>
		/// <param name="idVehicle">Identifiant du média (vehicle) choisi depuis l'onglet de sélection du média (TV,Radio,Presse,Affichage)</param>
		/// <param name="zoomDate">période de l'étude</param>
		/// <param name="uiEmpty">Boulean utilsé pour savoir si on a des résultats (et afficher ainsi le composant ou pas)</param>
		/// <remarks>
		/// Utilise les méthodes:
		///		<code>public static object[,] MediaInsertionsCreationsRules.GetData(WebSession webSession,string idMediaLevel1,string idMediaLevel2,string idMediaLevel3,string idAdvertiser ,int dateBegin, int dateEnd,bool isMediaDetail,string idVehicleFromTab,ListDictionary mediaImpactedList)</code>
		///		<code>private static string GetUIPress(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl,bool isMediaDetail)</code>
		///		<code>private static string GetUIRadio(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>private static string GetUITV(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>GetUIOutDoor(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		/// </remarks>
		/// <returns>Code HTML détail des insertions</returns>
		public static string GetMediaInsertionsCreationsResultsUI(WebSession webSession, string idMediaLevel1, string idMediaLevel2, string idMediaLevel3,string idMediaLevel4,string idAdvertiser,Page page,string idVehicle,string zoomDate,ref bool uiEmpty){


			#region variables
			ListDictionary mediaImpactedList =null;
			object[,] tab=null;			
			int dateBegin=0;
			int dateEnd =0;
			string PeriodEndDate="";
			string PeriodBeginningDate="";
			ArrayList fieldsList=null;
			#endregion


			////Pas de droit résultat au détail insertion
			if ( idVehicle !=null && idVehicle.Length>0 
				&& (CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == CstClassification.DB.Vehicles.names.outdoor 
				&& webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG) == null				
				) {
                return "<TABLE width=\"500\" class=\"whiteBackGround insertionBorderV2\""
					+ "cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
					+ " " + GetUIEmpty(webSession.SiteLanguage, 1882)
					+ "</TABLE>";
			}

            #region Mise en forme des dates et du media
            CstWeb.CustomerSessions.Period.Type periodType = webSession.PeriodType;
            string periodBegin = webSession.PeriodBeginningDate;
            string periodEnd = webSession.PeriodEndDate;

            if (zoomDate != null && zoomDate.Length > 0)
            {
                if (webSession.DetailPeriod == CstCustomerSession.Period.DisplayLevel.weekly){
                    periodType = CstCustomerSession.Period.Type.dateToDateWeek;
                }
                else{
                    periodType = CstCustomerSession.Period.Type.dateToDateMonth;
                }
                dateBegin = Convert.ToInt32(
                    Dates.Max(Dates.getPeriodBeginningDate(zoomDate, periodType),
                        Dates.getPeriodBeginningDate(periodBegin, webSession.PeriodType)).ToString("yyyyMMdd")
                    );
                dateEnd = Convert.ToInt32(
                    Dates.Min(Dates.getPeriodEndDate(zoomDate, periodType),
                        Dates.getPeriodEndDate(periodEnd, webSession.PeriodType)).ToString("yyyyMMdd")
                    );
            }
            else
            {
                dateBegin = Convert.ToInt32(Dates.getPeriodBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                dateEnd = Convert.ToInt32(Dates.getPeriodEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
            }
            #endregion
		
			#region Récupération des données
			try {
                if (MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession) || MediaInsertionsCreationsRules.IsMDSelected(idVehicle))
					mediaImpactedList = Functions.MediaDetailLevel.GetImpactedMedia(webSession,long.Parse(idMediaLevel1),long.Parse(idMediaLevel2),long.Parse(idMediaLevel3),long.Parse(idMediaLevel4));	
				fieldsList = new ArrayList();
				tab = MediaInsertionsCreationsRules.GetData(webSession,idMediaLevel1,idMediaLevel2,idMediaLevel3,idAdvertiser,dateBegin,dateEnd,idVehicle,mediaImpactedList,fieldsList);
			}
			catch(System.Exception err){
				throw(new MediaInsertionsCreationsUIException("Impossible d'obtenir les donnes",err));
			}
			#endregion			

			#region Pas de données à afficher
			if (tab == null || tab[0, 0] == null || idVehicle == null || idVehicle.Length == 0) {
		
					return "<TABLE width=\"500\" class=\"whiteBackGround insertionBorderV2\""
						+"cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
						+GetUIEmpty(webSession.SiteLanguage)
						+"</TABLE>";		
			}
			#endregion

			try{				

				#region Construction du txt HTLM
				switch((CstClassification.DB.Vehicles.names) int.Parse(idVehicle)){
					case CstClassification.DB.Vehicles.names.press:
					case CstClassification.DB.Vehicles.names.internationalPress:
                        return GetUIPress(tab, webSession, page, periodBegin, periodEnd, idVehicle);
					case CstClassification.DB.Vehicles.names.radio:
                        return GetUIRadio(tab, webSession, page, periodBegin, periodEnd, idVehicle);
					case CstClassification.DB.Vehicles.names.tv:
					case CstClassification.DB.Vehicles.names.others:
                        return GetUITV(tab, webSession, page, periodBegin, periodEnd, idVehicle);
					case CstClassification.DB.Vehicles.names.outdoor:
                        return GetUIOutDoor(tab, webSession, page, periodBegin, periodEnd, idVehicle);
                    case CstClassification.DB.Vehicles.names.directMarketing:
                        return GetUIMD(tab, webSession, page, periodBegin, periodEnd, idVehicle, mediaImpactedList);
					default:
						throw new MediaInsertionsCreationsUIException("Le vehicle demandé n'est pas un cas traité");
				}
				#endregion
					
			}
			catch(System.Exception err){
				throw(new MediaInsertionsCreationsUIException("Impossible d'obtenir les données",err));
			}
					
		}
		#endregion

		#region Pas de Creations
		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language){

            return GetUIEmpty(language, 177);

        }

		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <param name="code">code traduction</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language,int code){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\" align=\"center\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(code,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			
			return HtmlTxt.ToString();
		}

		
		#endregion

		#endregion

		#region Excel UI
	
		#region EXCEL UI (Nouvelle version D. Mussuma)
		/// <summary>
		/// Construction du code HTML destiné à l'export excel affichant le détail des insertions d'un média (vehicle) quelqconque sur une certaine période respectant le type de période présent dans la session
		///		Extraction des données
		///		Construction du code HTML en fonction du média considéré
		/// </summary>
		/// <param name="webSession">Session du client</param>
		/// <param name="idMediaLevel1">Identifiant du média de niveau 1</param>
		/// <param name="idMediaLevel2">Identifiant du média de niveau 2</param>
		/// <param name="idMediaLevel3">Identifiant du média de niveau 3</param>
		/// <param name="idMediaLevel4">Identifiant du média de niveau 4</param>
		/// <param name="idAdvertiser">Identifiant annonceur</param>
		/// <param name="page">Page</param>
		/// <param name="idVehicle">Identifiant du média (vehicle) choisi depuis l'onglet de sélection du média (TV,Radio,Presse,Affichage)</param>		
		/// <param name="period">Période de l'étude</param>
		/// <remarks>
		/// Utilise les méthodes:
		///		<code>public static object[,] MediaInsertionsCreationsRules.GetData(WebSession webSession,string idMediaLevel1,string idMediaLevel2,string idMediaLevel3,string idAdvertiser ,int dateBegin, int dateEnd,bool isMediaDetail,string idVehicleFromTab,ListDictionary mediaImpactedList)</code>
		///		<code>private static string GetUIPress(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl,bool isMediaDetail)</code>
		///		<code>private static string GetUIRadio(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>private static string GetUITV(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		///		<code>GetUIOutDoor(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, string excelUrl, string idVehicle,bool isMediaDetail)</code>
		/// </remarks>
		/// <returns>Code HTML détail des insertions</returns>
		public static string GetMediaInsertionsCreationsExcelUI(WebSession webSession, string idMediaLevel1, string idMediaLevel2, string idMediaLevel3,string idMediaLevel4,string idAdvertiser,string period,Page page,ref string idVehicle){
			
			#region variables
			ListDictionary mediaImpactedList =null;
			object[,] tab=null;			
			int dateBegin=0;
			int dateEnd =0;
			string PeriodEndDate="";
			string PeriodBeginningDate="";
			ArrayList fieldsList=null;
			#endregion
		

			#region Mise en forme des dates et du media
            CstWeb.CustomerSessions.Period.Type periodType = webSession.PeriodType;
            string periodBegin = webSession.PeriodBeginningDate;
            string periodEnd = webSession.PeriodEndDate;

            if (period != null && period.Length > 0)
            {
                if (webSession.DetailPeriod == CstCustomerSession.Period.DisplayLevel.weekly)
                {
                    periodType = CstCustomerSession.Period.Type.dateToDateWeek;
                }
                else
                {
                    periodType = CstCustomerSession.Period.Type.dateToDateMonth;
                }
                dateBegin = Convert.ToInt32(
                    Dates.Max(Dates.getPeriodBeginningDate(period, periodType),
                        Dates.getPeriodBeginningDate(periodBegin, webSession.PeriodType)).ToString("yyyyMMdd")
                    );
                dateEnd = Convert.ToInt32(
                    Dates.Min(Dates.getPeriodEndDate(period, periodType),
                        Dates.getPeriodEndDate(periodEnd, webSession.PeriodType)).ToString("yyyyMMdd")
                    );
				PeriodBeginningDate = PeriodEndDate = period;

            }
            else
            {
                dateBegin = Convert.ToInt32(Dates.getPeriodBeginningDate(periodBegin, periodType).ToString("yyyyMMdd"));
                dateEnd = Convert.ToInt32(Dates.getPeriodEndDate(periodEnd, periodType).ToString("yyyyMMdd"));
				PeriodBeginningDate = webSession.PeriodBeginningDate;
				PeriodEndDate = webSession.PeriodEndDate;
            }
			#endregion

			#region Récupération des données
			
			try{
				
				mediaImpactedList = Functions.MediaDetailLevel.GetImpactedMedia(webSession,long.Parse(idMediaLevel1),long.Parse(idMediaLevel2),long.Parse(idMediaLevel3),long.Parse(idMediaLevel4));	
				if( MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){
					fieldsList = new ArrayList();
					tab = MediaInsertionsCreationsRules.GetData(webSession,idMediaLevel1,idMediaLevel2,idMediaLevel3,idAdvertiser,dateBegin,dateEnd,idVehicle,mediaImpactedList,fieldsList);
				}
				else tab = MediaInsertionsCreationsRules.GetData(webSession,idMediaLevel1,idMediaLevel2,idMediaLevel3,idAdvertiser,dateBegin,dateEnd,idVehicle,mediaImpactedList);
			}
			catch(System.Exception err){
				throw(new MediaInsertionsCreationsUIException("Impossible d'obtenir les donnes",err));
			}
			#endregion

			#region Pas de données à afficher
			if (tab==null || tab[0,0]==null){

				//Pas de droit publicité extérieure
				if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)==null
					&& (CstClassification.DB.Vehicles.names)int.Parse(idVehicle)==CstClassification.DB.Vehicles.names.outdoor 
					){
                    return "<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\""
						+"cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
						+" "+GetUIEmpty(webSession.SiteLanguage,1882)
						+"</TABLE>";				
				}
				else

                    return "<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\""
						+"cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">"
						+GetUIEmpty(webSession.SiteLanguage);
			}
			#endregion

			try{				
				
				#region Construction du txt HTLM
				switch((CstClassification.DB.Vehicles.names) int.Parse(idVehicle)){
					case CstClassification.DB.Vehicles.names.press:
					case CstClassification.DB.Vehicles.names.internationalPress:
						return GetUIPressExcel(tab, webSession, page,dateBegin.ToString(),dateEnd.ToString(), mediaImpactedList,int.Parse(idVehicle));
					case CstClassification.DB.Vehicles.names.radio:
						return GetUIRadioExcel(tab, webSession, page,dateBegin.ToString(),dateEnd.ToString(),mediaImpactedList,int.Parse(idVehicle));
					case CstClassification.DB.Vehicles.names.tv:
					case CstClassification.DB.Vehicles.names.others:
						return GetUITVExcel(tab, webSession, page,dateBegin.ToString(),dateEnd.ToString(), mediaImpactedList,int.Parse(idVehicle));
					case CstClassification.DB.Vehicles.names.outdoor:
						return GetUIOutDoorExcel(tab, webSession, page,dateBegin.ToString(),dateEnd.ToString(), mediaImpactedList,int.Parse(idVehicle));
                    case CstClassification.DB.Vehicles.names.directMarketing:
                        return GetUIMDExcel(tab, webSession, page, dateBegin.ToString(),dateEnd.ToString(), mediaImpactedList, int.Parse(idVehicle));
					default:
						throw new MediaInsertionsCreationsUIException("Le vehicle demandé n'est pas un cas traité");
				}
				#endregion
					
			}
			catch(System.Exception err){
				throw(new MediaInsertionsCreationsUIException("Impossible d'obtenir les données",err));
			}
					
		}
		#endregion	
	
		#region Presse
		/// <summary>
		/// Génère le code HTML nécessaire à l'export du détail des insertions presses:
		///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
		///		data non vide : 
		///			Génération du tableau ordonné par catégorie > support > date > page
		/// </summary>
		/// <param name="data">Tableau de données</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page résultat</param>
		/// <param name="periodBeginning">Période de début d'affichage</param>
		/// <param name="periodEnd">Période de fin d'affichage</param>
		/// <param name="mediaImpactedList">Liste de medias impactés</param>
		/// <param name="idVehicle">Identifiant du media sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIPressExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd,ListDictionary mediaImpactedList, int idVehicle){

			StringBuilder HtmlTxt = new StringBuilder(1000);			

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";		
		
			int i = 0;
			int j=0;
			bool first = true;
			string classe="";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(webSession,false,periodBeginning,periodEnd,mediaImpactedList,idVehicle));
			#endregion

			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail insertion presse avec gestion des colonnes génériques

				#region variables
				ArrayList idLevelList=null;
				#endregion

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");			
				#endregion

				#region Pas de données à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 13)).ToString();
				}
					#endregion

				else{
	
					#region En-têtes de tableaux						
					HtmlTxt.Append("<tr>");
					if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){														
						foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ( (currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
									|| (currentLevel.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentLevel.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
							if(idLevelList==null)idLevelList=new ArrayList();
							idLevelList.Add(currentLevel.Id.GetHashCode());
						}
					}
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){														
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ( (currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching)) && !(currentColumn.NotInExcelExport)
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
						}																							
					}	
					HtmlTxt.Append("</tr>");
					#endregion

					#region Corps du tableau
					//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
					for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
						HtmlTxt.Append("<tr>");
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){
							foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
										|| (currentLevel.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))											
										){										
										if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}	
						}
						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							
							
							foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
										|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
										&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching))  && !(currentColumn.NotInExcelExport)
										){										
										if(data[i,j]!=null){
											if(data[0,j]!=null && data[0,j].ToString().Equals("LOCATION")){
												HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString().Replace("<br>",",")+"</td>");
											}else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										}
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}												
						}
						HtmlTxt.Append("</tr>");
					}
							
					#endregion
				}
				#endregion

			}else {
				
					#region Détail insertion presse sans gestion des colonnes génériques

					#region Début du tableau
					HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");							
					#endregion

					#region Pas de données à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
					}
					#endregion

					#region Entêtes de tableau
					HtmlTxt.Append("<tr>");					
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(970,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(971,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(895,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(894,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(176,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(174,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(858,webSession.SiteLanguage) + "</td>");
					if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
						HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(1881,webSession.SiteLanguage) + "</td>");
					if(!webSession.isCompetitorAdvertiserSelected()){
						HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(1384,webSession.SiteLanguage) + "</td>");
						HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(1383,webSession.SiteLanguage) + "</td>");
					}
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(469,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(561,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(471,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(472,webSession.SiteLanguage) + "</td>");
					HtmlTxt.Append("</tr>");
					#endregion
			
					#region Tableau de détail des insertions
			
					 first = true;
					 i = 0;
					 classe="";
					while(i<data.GetLength(0) && data[i,0]!=null){

						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
				
						HtmlTxt.Append("<tr>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.MEDIA_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.DATE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.MEDIA_PAGING_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
						if( Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected()){// si droit accroche
							if(data[i,CstWeb.PressInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
							else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
						}
						if(!webSession.isCompetitorAdvertiserSelected()){					
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.FORMAT_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.AREA_PAGE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.COLOR_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i, CstWeb.PressInsertionsColumnIndex.LOCATION_INDEX].ToString().Replace("<br>",",")+"</td>");
						HtmlTxt.Append("</tr>");

						i++;
					}
			
					HtmlTxt.Append("</TABLE>");
					#endregion

					#endregion					

			}
			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));

			return HtmlTxt.ToString();
		}
		#endregion		

		#region Radio
		/// <summary>
		/// Génère le code HTML nécessaire à l'export du détail des insertions radio:
		///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
		///		data non vide : 
		///			Génération du tableau ordonné par catégorie > support > date > page
		/// </summary>
		/// <param name="data">Tableau de données</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page résultat</param>
		/// <param name="periodBeginning">Période de début d'affichage</param>
		/// <param name="periodEnd">Période de fin d'affichage</param>
		/// <param name="mediaImpactedList">Liste de medias impactés</param>
		/// <param name="idVehicle">Identifiant du media sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIRadioExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd,ListDictionary mediaImpactedList, int idVehicle){

			StringBuilder HtmlTxt = new StringBuilder(1000);

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";
			int i = 0;
			int j=0;
			bool first = true;
			string classe="";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(webSession,false,periodBeginning,periodEnd,mediaImpactedList,idVehicle));
			#endregion	
			
			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail spots radio avec gestion des colonnes génériques
					
				#region variables
				ArrayList idLevelList=null;
				#endregion

				#region Début du tableau
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");			
				#endregion

				#region Pas de données à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 13)).ToString();
				}
					#endregion

				else{
	
					#region En-têtes de tableaux						
					HtmlTxt.Append("<tr>");
					if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){														
						foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ( (currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
									|| (currentLevel.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentLevel.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
							if(idLevelList==null)idLevelList=new ArrayList();
							idLevelList.Add(currentLevel.Id.GetHashCode());
						}
					}
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){														
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ( (currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching)) && !(currentColumn.NotInExcelExport)
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
						}																							
					}	
					HtmlTxt.Append("</tr>");
					#endregion

					#region Corps du tableau
					//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
					for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
						HtmlTxt.Append("<tr>");
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){
							foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
										|| (currentLevel.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))											
										){										
										if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}	
						}
						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							
							
							foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
										|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
										&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching))  && !(currentColumn.NotInExcelExport)
										){										
										if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}												
						}
						HtmlTxt.Append("</tr>");
					}
							
					#endregion
				}
				#endregion

			}else{
 
					#region Détail spots radio sans gestion des colonnes génériques
					
					#region Début du tableau
					HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");			
					#endregion

					#region Pas de données à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 13)).ToString();
					}
						#endregion

					#region Entêtes de tableau
					else{HtmlTxt.Append("<tr>");
				
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(970,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(971,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(895,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
					if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,webSession.SiteLanguage)+"</td>");
					if(!webSession.isCompetitorAdvertiserSelected()){
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,webSession.SiteLanguage)+"</td>");
					}
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(865,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" lign=\"center\" nowrap>"+GestionWeb.GetWebWord(866,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(867,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");					
					HtmlTxt.Append("</tr>");
				}
					#endregion

					#region Tableau
					i = 0;
					first = true;
					classe="";

					while(i<data.GetLength(0) && data[i,0]!=null){
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.DATE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
						if( Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected()){// si droit accroche
							if(data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null )HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
							else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
						}
						if(!webSession.isCompetitorAdvertiserSelected()){
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"center\">"+data[i,CstWeb.RadioInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.RANK_WAP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.DURATION_BREAK_WAP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.BREAK_SPOTS_WAP_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" align=\"right\">"+data[i,CstWeb.RadioInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("</tr>");

						i++;
					}
			
					HtmlTxt.Append("</TABLE>");
					#endregion

					#endregion					
			}
			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
			return HtmlTxt.ToString();
		}
		#endregion

		#region TV
		/// <summary>
		/// Génère le code HTML nécessaire à l'export du détail des insertions TV:
		///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
		///		data non vide : 
		///			Génération du tableau ordonné par catégorie > support > date > page
		/// </summary>
		/// <param name="data">Tableau de données</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page résultat</param>
		/// <param name="periodBeginning">Période de début d'affichage</param>
		/// <param name="periodEnd">Période de fin d'affichage</param>
		/// <param name="mediaImpactedList">Liste de medias impactés</param>
		/// <param name="idVehicle">Identifiant du media sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUITVExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd,ListDictionary mediaImpactedList, int idVehicle){
			StringBuilder HtmlTxt = new StringBuilder(1000);

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";
			bool first = true;
			int i = 0;			
			string classe="";
			int j=0;

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(webSession,false,periodBeginning,periodEnd,mediaImpactedList,idVehicle));
			#endregion

			#region Début du tableau
            HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");
			#endregion
			
			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

				#region Détail spots télévision avec gestion des colonnes génériques

				#region variables
				ArrayList idLevelList=null;
				#endregion

				#region Pas de donnée à afficher
				if (data[0,0]==null){
					return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
				}
					#endregion

				else{

					#region En-têtes de tableaux
					HtmlTxt.Append("<tr>");
					if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){														
						foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ((currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
									|| (currentLevel.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentLevel.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
							if(idLevelList==null)idLevelList=new ArrayList();
							idLevelList.Add(currentLevel.Id.GetHashCode());
						}
					}
					if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){														
						foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
							for(int k=0; k<data.GetLength(1);k++){
								if(data[0,k]!=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
									|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
									&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching)) && !(currentColumn.NotInExcelExport)
									){
									HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
								}
							}
						}																							
					}	
					HtmlTxt.Append("</tr>");
					#endregion

					#region Corps du tableau
					//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
					for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
						HtmlTxt.Append("<tr>");
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){
							foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
										|| (currentLevel.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))											
										){										
										if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}	
						}
						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							
							
							foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
								for(j=0; j<data.GetLength(1);j++){
									if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
										|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
										&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching)) &&  !(currentColumn.NotInExcelExport)
										){										
										if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
										else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
									}
								}
								
							}												
						}
						HtmlTxt.Append("</tr>");
					}
							
					#endregion
				}
				#endregion

			}else{

					#region Détail spots télévision sans gestion des colonnes génériques

					#region Entêtes de tableau

					#region Pas de donnée à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
					}
					#endregion

					else{HtmlTxt.Append("<tr>");
				
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(970,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(971,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(895,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
					if(Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1881,webSession.SiteLanguage)+"</td>");
					if(!webSession.isCompetitorAdvertiserSelected()){
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,webSession.SiteLanguage)+"</td>");
					}
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(860,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(495,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(861,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(862,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(863,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(864,webSession.SiteLanguage).Replace("<br>"," ")+"</td>");
					HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");					
					HtmlTxt.Append("</tr>");
				}
					#endregion
			
					#region Tableau
					i = 0;
					first = true;
					 classe="";

					while(i<data.GetLength(0) && data[i,0]!=null){
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.DATE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
						if( Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected()){// si droit accroche
							if(data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX]!=null )HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString()+"</td>");
							else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;</td>");
						}
						if(!webSession.isCompetitorAdvertiserSelected()){
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString()+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap >"+data[i,CstWeb.TVInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString()+"</td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.TOP_DIFFUSION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.TVInsertionsColumnIndex.ID_COMMERCIAL_BREAK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.RANK_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_DURATION_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.BREAK_SPOTS_NB_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.TVInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td>");
						HtmlTxt.Append("</tr>");

						i++;
					}
			
					#endregion

					#endregion					

			}
			
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
			return HtmlTxt.ToString();

		}
		#endregion

		#region Publicité Extérieure
		/// <summary>
		/// Génère le code HTML nécessaire à l'export du détail des insertions Publicité extérieure:
		///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
		///		data non vide : 
		///			Génération du tableau ordonné par catégorie > support > date > page
		/// </summary>
		/// <param name="data">Tableau de données</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page résultat</param>
		/// <param name="periodBeginning">Période de début d'affichage</param>
		/// <param name="periodEnd">Période de fin d'affichage</param>
		/// <param name="mediaImpactedList">Liste de medias impactés</param>
		/// <param name="idVehicle">Identifiant du media sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIOutDoorExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd,ListDictionary mediaImpactedList, int idVehicle){
			
			StringBuilder HtmlTxt = new StringBuilder(1000);

			//Pas de droit publicité extérieure
			if(webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)==null){
                HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround insertionWhiteBorder\" style=\"MARGIN-TOP: 25px; MARGIN-LEFT: 25px; MARGIN-RIGHT: 25px;\"");
				HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
				HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage,1882));
				HtmlTxt.Append("</TABLE>");

				return HtmlTxt.ToString();
			}	

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";		
		
			int i = 0;
			int j=0;
			bool first = true;
			string classe="";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(webSession,false,periodBeginning,periodEnd,mediaImpactedList,idVehicle));
			#endregion

			if(MediaInsertionsCreationsRules.IsRequiredGenericColmuns(webSession)){

					#region Détail des affiches avec gestion des colonnes génériques

					#region variabbles
					ArrayList idLevelList=null;
					#endregion

					#region Début du tableau
                    HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");			
					#endregion

					#region Pas de données à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 13)).ToString();
					}
						#endregion

					else{
	
						#region En-têtes de tableaux
						HtmlTxt.Append("<tr>");
						if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){														
							foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
								for(int k=0; k<data.GetLength(1);k++){
									if(data[0,k]!=null && ( (currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
										|| (currentLevel.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))
										){
										HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentLevel.WebTextId,webSession.SiteLanguage)+"</td>");
									}
								}
								if(idLevelList==null)idLevelList=new ArrayList();
								idLevelList.Add(currentLevel.Id.GetHashCode());
							}
						}
						if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){														
							foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
								for(int k=0; k<data.GetLength(1);k++){
									if(data[0,k]!=null && ( (currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
										|| (currentColumn.DataBaseAliasField!=null && data[0,k].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
										&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching)) && !(currentColumn.NotInExcelExport)
										){
										HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(currentColumn.WebTextId,webSession.SiteLanguage)+"</td>");
									}
								}
							}																							
						}	
						HtmlTxt.Append("</tr>");
						#endregion

						#region Corps du tableau
						//Parcours du tableau de résultats ( ligne 0 <=> libellés ; lignE >0 <=>valeurs) 
						for( i = 1; i<data.GetLength(0);i++){//A partir de la ligne 1 du tableau
							HtmlTxt.Append("<tr>");
							if (first){
								first=false;
								classe=CLASSE_1;
							}
							else{
								first=true;
								classe=CLASSE_2;
							}
							if(webSession.DetailLevel!=null && webSession.DetailLevel.Levels!=null && webSession.DetailLevel.Levels.Count>0){
								foreach(DetailLevelItemInformation currentLevel in webSession.DetailLevel.Levels){
									for(j=0; j<data.GetLength(1);j++){
										if( data[0,j] !=null && ((currentLevel.DataBaseAliasField==null && currentLevel.DataBaseField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseField.ToUpper())==0)
											|| (currentLevel.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentLevel.DataBaseAliasField.ToUpper())==0))											
											){										
											if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
											else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
										}
									}
								
								}	
							}
							if(webSession.GenericInsertionColumns!=null && webSession.GenericInsertionColumns.Columns!=null && webSession.GenericInsertionColumns.Columns.Count>0){
							
							
								foreach(GenericColumnItemInformation currentColumn in webSession.GenericInsertionColumns.Columns){
									for(j=0; j<data.GetLength(1);j++){
										if( data[0,j] !=null && ((currentColumn.DataBaseAliasField==null && currentColumn.DataBaseField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseField.ToUpper())==0)
											|| (currentColumn.DataBaseAliasField!=null && data[0,j].ToString().CompareTo(currentColumn.DataBaseAliasField.ToUpper())==0))
											&& !(idLevelList!=null && idLevelList.Contains(currentColumn.IdDetailLevelMatching))  && !(currentColumn.NotInExcelExport)
											){										
											if(data[i,j]!=null)HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,j].ToString()+"</td>");
											else HtmlTxt.Append("<td class=\""+classe+"\" nowrap>&nbsp;"+"</td>");										
										}
									}
								
								}												
							}
							HtmlTxt.Append("</tr>");
						}
							
						#endregion
					}
					
					#endregion

			}else{

					#region Détail des affiches sans gestion des colonnes génériques					

					#region Début du tableau
					HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");			
					#endregion

					#region Pas de donnée à afficher
					if (data[0,0]==null){
						return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
					}
						#endregion

					#region Entêtes de tableau
					else{HtmlTxt.Append("<tr>");
				
				
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(970,webSession.SiteLanguage)+"</td>"); 
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(971,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(895,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
						if(!webSession.isCompetitorAdvertiserSelected()){
							HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1384,webSession.SiteLanguage)+"</td>");
							HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1383,webSession.SiteLanguage)+"</td>");
						}
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td>");					
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1420,webSession.SiteLanguage)+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1609,webSession.SiteLanguage))+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1611,webSession.SiteLanguage))+"</td>");
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1660,webSession.SiteLanguage)+"</td>");		
					
						HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");											
						HtmlTxt.Append("</tr>");
					}
					#endregion
			
					#region Tableau
					i = 0;
					first = true;
					classe="";

					while(i<data.GetLength(0) && data[i,0]!=null){
						if (first){
							first=false;
							classe=CLASSE_1;
						}
						else{
							first=true;
							classe=CLASSE_2;
						}
						HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.CATEGORY_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.DATE_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX].ToString() : "")+"</td>");
						if(!webSession.isCompetitorAdvertiserSelected()){
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString() : "")+"</td>");
							HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString() : "")+"</td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX] != null) ? Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.SQLGenerator.SaleTypeOutdoor(data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX].ToString(),webSession.SiteLanguage)) : "")+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX] != null) ? Convertion.ToHtmlString(data[i,CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX].ToString()) : "")+"</td>");						
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX].ToString() : "")+"</td>");				
				
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ ((data[i,CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] != null) ? data[i,CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX].ToString() : "")+"</td>");
						HtmlTxt.Append("</tr>");

						i++;
					}
			
					HtmlTxt.Append("</TABLE>");
					#endregion

					#endregion
					
			}
			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
			return HtmlTxt.ToString();

		}
		#endregion

        #region Marketing Direct
        /// <summary>
        /// Génère le code HTML nécessaire à l'export des versions du marketing direct:
        ///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
        ///		data non vide : 
        /// </summary>
        /// <param name="data">Tableau de données</param>
        /// <param name="webSession">Session utilisateur</param>
        /// <param name="page">Page résultat</param>
        /// <param name="periodBeginning">Période de début d'affichage</param>
        /// <param name="periodEnd">Période de fin d'affichage</param>
        /// <param name="mediaImpactedList">Liste de medias impactés</param>
        /// <param name="idVehicle">Identifiant du media sélectionné</param>
        /// <returns>Code html Généré</returns>
        private static string GetUIMDExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, ListDictionary mediaImpactedList, int idVehicle) {

            StringBuilder HtmlTxt = new StringBuilder(1000);

            const string CLASSE_1 = "pmmediaxls1";
            const string CLASSE_2 = "pmmediaxls2";

            int i = 0;
            int j = 0;
            bool first = true;
            string classe = "";
            ArrayList prefixePNA = new ArrayList(), prefixeCAGN = new ArrayList(), prefixeCAP = new ArrayList(), prefixeCAGT = new ArrayList();
            string prefixePNATemp = string.Empty, prefixeCAGNTemp = string.Empty, prefixeCAPTemp = string.Empty, prefixeCAGTTemp = string.Empty;

            #region Initialisation des prefixes pour la gestion de l'affichage des différents média du MD
            int k=0;
            ArrayList listMedia = new ArrayList();
            string prec = string.Empty, current = string.Empty;
            
            while (k < data.GetLength(0) && data[k, 0] != null){
                current = data[k, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX].ToString();
                if (current != prec){
                    prec = current;
                    listMedia.Add(current);
                }
                k++;
            }

            if (listMedia.Contains(CstDB.Media.PUBLICITE_NON_ADRESSEE)) {
                prefixePNATemp = "<td class=\"classe\" nowrap></td><td class=\"classe\" nowrap></td>";
            }

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GENERAL))
                prefixeCAGNTemp = "<td class=\"classe\" nowrap></td>";
            else if (listMedia.Contains(CstDB.Media.PUBLICITE_NON_ADRESSEE))
                prefixePNATemp += "<td class=\"classe\" nowrap></td>";

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GESTION)) {
                prefixeCAGTTemp = "<td class=\"classe\" nowrap></td>";
            }

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_PRESSE) || listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GESTION) || listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GENERAL))
                prefixeCAPTemp = "<td class=\"classe\" nowrap></td><td class=\"classe\" nowrap></td>";

            prefixePNA.Add(prefixeCAPTemp + prefixeCAGTTemp);
            prefixeCAGN.Add(prefixePNATemp);
            prefixeCAGN.Add(prefixeCAGTTemp);
            prefixeCAP.Add(prefixePNATemp + prefixeCAGNTemp);
            prefixeCAP.Add(prefixeCAGTTemp);
            prefixeCAGT.Add(prefixePNATemp + prefixeCAGNTemp);
            #endregion

            #region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
            HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUpFromMediaPlan(webSession, false, periodBeginning, periodEnd, mediaImpactedList, idVehicle));
            #endregion

            #region Détail insertion presse sans gestion des colonnes génériques

            #region Début du tableau
            HtmlTxt.Append("<TABLE width=\"500\" class=\"whiteBackGround\" cellPadding=\"0\" align=\"center\" border=\"0\">");
            #endregion

            #region Pas de données à afficher
            if (data[0, 0] == null){
                return HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
            }
            #endregion

            #region Entêtes de tableau
            HtmlTxt.Append("<tr>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(970, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(971, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(895, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(176, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(468, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(174, webSession.SiteLanguage) + "</td>");
            if (Functions.MediaDetailLevel.HasSloganRight(webSession))// si droit accroche
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(1888, webSession.SiteLanguage) + "</td>");
            if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_POIDS_MARKETING_DIRECT) != null)
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2220, webSession.SiteLanguage) + "</td>");
            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(1936, webSession.SiteLanguage) + "</td>");
            if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2216, webSession.SiteLanguage) + "</td>");

            if (listMedia.Contains(CstDB.Media.PUBLICITE_NON_ADRESSEE)) {
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2222, webSession.SiteLanguage) + "</td>");
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2221, webSession.SiteLanguage) + "</td>");
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            }
                

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GENERAL) && !listMedia.Contains(CstDB.Media.PUBLICITE_NON_ADRESSEE)) {
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            }

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GENERAL) || listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GESTION) || listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_PRESSE)) {
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            }

            if (listMedia.Contains(CstDB.Media.COURRIER_ADRESSE_GESTION)){
                HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2223, webSession.SiteLanguage) + "</td>");
            }

            #region Ancienne Méthode
            //if (mediaImpactedList["id_media"] != null && mediaImpactedList["id_media"].ToString() != "-1") {

            //    switch (mediaImpactedList["id_media"].ToString()) {

            //        case CstDB.Media.PUBLICITE_NON_ADRESSEE:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2222, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2221, webSession.SiteLanguage) + "</td>");
            //            break;
            //        case CstDB.Media.COURRIER_ADRESSE_GENERAL:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
            //            break;
            //        case CstDB.Media.COURRIER_ADRESSE_PRESSE:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
            //            break;
            //        case CstDB.Media.COURRIER_ADRESSE_GESTION:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2223, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
            //            break;
            //        default:
            //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
            //    }

            //}
            //else if (mediaImpactedList["id_category"] != null && mediaImpactedList["id_category"].ToString() != "-1") {

            //    switch (mediaImpactedList["id_category"].ToString()) {

            //        case CstDB.Category.PUBLICITE_NON_ADRESSEE:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2222, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2221, webSession.SiteLanguage) + "</td>");
            //            break;
            //        case CstDB.Category.COURRIER_ADRESSE:
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
            //            HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2223, webSession.SiteLanguage) + "</td>");
            //            break;
            //        default:
            //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
            //    }

            //}
            //else {

            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2222, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2221, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td>");
            //        HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(2223, webSession.SiteLanguage) + "</td>");
            //    }
            #endregion

            HtmlTxt.Append("</tr>");
            #endregion

            #region Tableau de détail des insertions

            first = true;
            i = 0;
            classe = "";
            while (i < data.GetLength(0) && data[i, 0] != null) {

                if (first) {
                    first = false;
                    classe = CLASSE_1;
                }
                else {
                    first = true;
                    classe = CLASSE_2;
                }

                HtmlTxt.Append("<tr>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.CATEGORY_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MEDIA_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.DATE_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.ADVERTISER_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.PRODUCT_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.GROUP_INDEX].ToString() + "</td>");

                if (Functions.MediaDetailLevel.HasSloganRight(webSession)) {// si droit accroche
                    if (data[i, CstWeb.MDVersionsColumnIndex.SLOGAN_INDEX] != null) HtmlTxt.Append("<td class=\"" + classe + "\" align=\"right\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.SLOGAN_INDEX].ToString() + "</td>");
                    else HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>&nbsp;</td>");
                }
                               
                if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_POIDS_MARKETING_DIRECT) != null)
                    HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.WEIGHT_INDEX].ToString() + "</td>");
                HtmlTxt.Append("<td class=\"" + classe + "\"  align=\"center\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.EXPENDITURE_INDEX].ToString() + "</td>");
                if (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_VOLUME_MARKETING_DIRECT) != null)
                    HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + Math.Round(double.Parse(data[i, CstWeb.MDVersionsColumnIndex.VOLUME_INDEX].ToString())) + "</td>");

                //Affichage du détail pour chaque média
                switch (data[i, CstWeb.MDVersionsColumnIndex.ID_MEDIA_INDEX].ToString()) {
                    case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                        HtmlTxt.Append(GetMailDetailPNAExcel(webSession, data, i, classe, prefixePNA));
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                        HtmlTxt.Append(GetMailDetailCAGENRALExcel(webSession, data, i,classe, prefixeCAGN));
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                        HtmlTxt.Append(GetMailDetailCAPRESSExcel(webSession, data, i,classe, prefixeCAP));
                        break;
                    case CstDB.Media.COURRIER_ADRESSE_GESTION:
                        HtmlTxt.Append(GetMailDetailCAGESTIONExcel(webSession, data, i,classe, prefixeCAGT));
                        break;
                }

                #region Ancienne méthode
                //if (mediaImpactedList["id_media"] != null && mediaImpactedList["id_media"].ToString() != "-1") {

                //    switch (mediaImpactedList["id_media"].ToString()) {

                //        case CstDB.Media.PUBLICITE_NON_ADRESSEE:
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX].ToString() + "</td>");
                //            break;
                //        case CstDB.Media.COURRIER_ADRESSE_GENERAL:
                //            if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_STANDARD)
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2240, webSession.SiteLanguage) + "</td>");
                //            else if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_ORIGINAL)
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2241, webSession.SiteLanguage) + "</td>");
                //            else
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap></td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>");
                //            break;
                //        case CstDB.Media.COURRIER_ADRESSE_PRESSE:
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>");
                //            break;
                //        case CstDB.Media.COURRIER_ADRESSE_GESTION:
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>");
                //            break;
                //        default:
                //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Ce support ne correspond pas à un support du MD.");
                //    }

                //}
                //else if (mediaImpactedList["id_category"] != null && mediaImpactedList["id_category"].ToString() != "-1") {

                //    switch (mediaImpactedList["id_category"].ToString()) {

                //        case CstDB.Category.PUBLICITE_NON_ADRESSEE:
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX].ToString() + "</td>");
                //            break;
                //        case CstDB.Category.COURRIER_ADRESSE:
                //            if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_STANDARD)
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2240, webSession.SiteLanguage) + "</td>");
                //            else if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_ORIGINAL)
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2241, webSession.SiteLanguage) + "</td>");
                //            else
                //                HtmlTxt.Append("<td class=\"" + classe + "\" nowrap></td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>");
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX].ToString() + "</td>");
                //            break;
                //        default:
                //            throw new Exceptions.MediaInsertionsCreationsRulesException("GetMDFields(DBClassificationConstantes.Vehicles.names idVehicle, ListDictionary mediaList, WebSession webSesssion, string prefixeMediaPlanTable) : Cette catégorie ne correspond pas à une catégorie du MD.");
                //    }

                //}
                //else {

                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX].ToString() + "</td>");
                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX].ToString() + "</td>");
                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX].ToString() + "</td>");
                //        if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_STANDARD)
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2240, webSession.SiteLanguage) + "</td>");
                //        else if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_ORIGINAL)
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2241, webSession.SiteLanguage) + "</td>");
                //        else
                //            HtmlTxt.Append("<td class=\"" + classe + "\" nowrap></td>");
                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>");
                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>");
                //        HtmlTxt.Append("<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX].ToString() + "</td>");
                //}
                #endregion

                HtmlTxt.Append("</tr>");

                i++;
            }

            HtmlTxt.Append("</TABLE>");
            #endregion

            #endregion

			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
            return HtmlTxt.ToString();
        }
        #endregion		

		#region Pas de Creations
		/// <summary>
		/// Génère le code html sprécisant qu'il n'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <param name="nbCol">Nombre de colonne que le tableau aurait du avoir en cas de présence de données</param>
		/// <returns>Code html généré</returns>
		private static string GetUIEmptyExcel(int language, int nbCol){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD colSpan=\""+nbCol+"\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(177,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			return HtmlTxt.ToString();
		}
		#endregion

		#endregion
	
		#region Méthodes  privées

        #region IsNewDetailLevel
        /// <summary>
		/// Vérifie s'il s'agit d'un nouveau niveau de détail
		/// </summary>
		/// <param name="detailLevelItemInformation">Element de niveau de détail</param>		
		/// <param name="data">Tableau de résultats</param>
		/// <param name="dataRowIndex">Index ligne de données</param>
		/// <param name="dataColumnIndex">Index Colonne de données</param>	
		/// <param name="currentLineIndex">index ligne en cours</param>
		/// <param name="oldLabelLevel">ancien libellé élémént</param>
		/// <returns>Vrai si nouveau niveau de détail</returns>
		private static bool IsNewDetailLevel(DetailLevelItemInformation detailLevelItemInformation,object[,]data,int dataRowIndex,int dataColumnIndex,int currentLineIndex,string oldLabelLevel){
			if(detailLevelItemInformation!=null){
				if(data[dataRowIndex,dataColumnIndex]!=null 
					&& ((detailLevelItemInformation.DataBaseField!=null && data[dataRowIndex,dataColumnIndex].ToString().CompareTo(detailLevelItemInformation.DataBaseField.ToUpper().Trim())==0)
				|| (detailLevelItemInformation.DataBaseAliasField!=null && data[dataRowIndex,dataColumnIndex].ToString().CompareTo(detailLevelItemInformation.DataBaseAliasField.ToUpper().Trim())==0))
					&&(data[currentLineIndex,dataColumnIndex]!=null && data[currentLineIndex,dataColumnIndex].ToString().CompareTo(oldLabelLevel)!=0))return true;
				else return false;
			}
			else return false;

        }
        #endregion

        #region Get Mail Detail
        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version PNA
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <returns></returns>
        private static string GetMailDetailPNA(WebSession webSession, object[,] data, int i) {

            string HtmlTxt = string.Empty;

            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX].ToString() + "</td></tr>";
            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2222, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX].ToString() + "</td></tr>";
            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2221, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX].ToString() + "</td></tr>";

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version CA Genral
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="mailContent">Mail Content List</param>
        /// <returns></returns>
        private static string GetMailDetailCAGENRAL(WebSession webSession, object[,] data, int i, string[] mailContent){

            string HtmlTxt = string.Empty;
            bool first = true;

            if(data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_STANDARD)
                HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td><td nowrap>: " + GestionWeb.GetWebWord(2240, webSession.SiteLanguage) + "</td></tr>";
            else if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_ORIGINAL)
                HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td><td nowrap>: " + GestionWeb.GetWebWord(2241, webSession.SiteLanguage) + "</td></tr>";
            else
                HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(299, webSession.SiteLanguage) + "</td><td nowrap>: </td></tr>";

            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td></tr>";

            if (mailContent != null) {
                foreach (string item in mailContent){
                    if (first) {
                        HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td><td nowrap>: " + item;
                        first = false;
                    }
                    else
                        HtmlTxt += "<br/>&nbsp;&nbsp;" + item;
                }
                HtmlTxt += "</td></tr>";
            }

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version CA Presse
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="mailContent">Mail Content List</param>
        /// <returns></returns>
        private static string GetMailDetailCAPRESSE(WebSession webSession, object[,] data, int i, string[] mailContent) {

            string HtmlTxt = string.Empty;
            bool first = true;

            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td></tr>";
           
            if (mailContent != null) {
                foreach (string item in mailContent) {
                    if (first) {
                        HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td><td nowrap>: " + item;
                        first = false;
                    }
                    else
                        HtmlTxt += "<br/>&nbsp;&nbsp;" + item;
                }
                HtmlTxt += "</td></tr>";
            }

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version CA Gestion
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="mailContent">Mail Content List</param>
        /// <returns></returns>
        private static string GetMailDetailCAGESTION(WebSession webSession, object[,] data, int i, string[] mailContent) {

            string HtmlTxt = string.Empty;
            bool first = true;

            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2224, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td></tr>";
            HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2223, webSession.SiteLanguage) + "</td><td nowrap>: " + data[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX].ToString() + "</td></tr>";

            if (mailContent != null) {
                foreach (string item in mailContent) {
                    if (first) {
                        HtmlTxt += "<tr valign=\"top\"><td nowrap>&nbsp;" + GestionWeb.GetWebWord(2239, webSession.SiteLanguage) + "</td><td nowrap>: " + item;
                        first = false;
                    }
                    else
                        HtmlTxt += "<br/>&nbsp;&nbsp;" + item;
                }
                HtmlTxt += "</td></tr>";
            }

            return HtmlTxt;

        }
        #endregion

        #region Get Mail Detail Excel
        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version PNA
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="classe">style utilisé</param>
        /// <param name="prefixe">prefixe</param>
        /// <returns></returns>
        private static string GetMailDetailPNAExcel(WebSession webSession, object[,] data, int i, string classe, ArrayList prefixe) {

            string HtmlTxt = string.Empty;
            string pref = string.Empty, suf = string.Empty;

            suf = (string)prefixe[0];

            suf = suf.Replace("\"classe\"","\""+classe+"\"");

            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_FORMAT_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_TYPE_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.FORMAT_INDEX].ToString() + "</td>";
            HtmlTxt += suf;

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version CA Genral
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="classe">style utilisé</param>
        /// <param name="prefixe">prefixe</param>
        /// <returns></returns>
        private static string GetMailDetailCAGENRALExcel(WebSession webSession, object[,] data, int i, string classe, ArrayList prefixe) {

            string HtmlTxt = string.Empty;
            string pref = string.Empty, suf = string.Empty;

            pref = (string)prefixe[0];
            suf = (string)prefixe[1];

            pref = pref.Replace("\"classe\"", "\"" + classe + "\"");
            suf = suf.Replace("\"classe\"", "\"" + classe + "\"");
            
            HtmlTxt += pref;

            if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_STANDARD)
                HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2240, webSession.SiteLanguage) + "</td>";
            else if (data[i, CstWeb.MDVersionsColumnIndex.WP_MAIL_FORMAT_INDEX].ToString() == CstDB.Format.FORMAT_ORIGINAL)
                HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + GestionWeb.GetWebWord(2241, webSession.SiteLanguage) + "</td>";
            else
                HtmlTxt += "<td class=\"" + classe + "\" nowrap></td>";

            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>";
            
            HtmlTxt += suf;

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères d'une version CA Gestion
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="classe">style utilisé</param>
        /// <param name="prefixe">prefixe</param>
        /// <returns></returns>
        private static string GetMailDetailCAGESTIONExcel(WebSession webSession, object[,] data, int i, string classe, ArrayList prefixe) {

            string HtmlTxt = string.Empty;
            string pref = string.Empty, suf = string.Empty;

            pref = (string)prefixe[0];

            pref = pref.Replace("\"classe\"", "\"" + classe + "\"");

            HtmlTxt += pref;
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAILING_RAPIDITY_INDEX].ToString() + "</td>";

            return HtmlTxt;

        }

        /// <summary>
        /// Renvoie le code Html correspondant aux critères 'mail content' d'une version CA
        /// </summary>
        /// <param name="webSession">WebSession</param>
        /// <param name="data">Les données</param>
        /// <param name="i">l'index dans la table de données</param>
        /// <param name="classe">style utilisé</param>
        /// <param name="prefixe">prefixe</param>
        /// <returns></returns>
        private static string GetMailDetailCAPRESSExcel(WebSession webSession, object[,] data, int i, string classe, ArrayList prefixe) {

            string HtmlTxt = string.Empty;
            string pref = string.Empty, suf = string.Empty;

            pref = (string)prefixe[0];
            suf = (string)prefixe[1];

            pref = pref.Replace("\"classe\"", "\"" + classe + "\"");
            suf = suf.Replace("\"classe\"", "\"" + classe + "\"");

            HtmlTxt += pref;
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.MAIL_CONTENT_INDEX].ToString() + "</td>";
            HtmlTxt += "<td class=\"" + classe + "\" nowrap>" + data[i, CstWeb.MDVersionsColumnIndex.OBJECT_COUNT_INDEX].ToString() + "</td>";
            HtmlTxt += suf;

            return HtmlTxt;

        }
        #endregion

        #endregion

    }
}

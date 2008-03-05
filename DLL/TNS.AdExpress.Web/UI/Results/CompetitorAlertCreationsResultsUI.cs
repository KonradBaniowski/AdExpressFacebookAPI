#region Information
// Auteur : A.Obermeyer
// Créé le : 24/09/2004
// Modifié le : 
//		06/07/2005	K.Shehzad Addition of Agglomeration colunm for Outdoor creations 
//		12/08/2005	G. Facon		Nom de fonctions et exceptions
//		09/08/2006	G. Ragneau		Suppression du bouton d'export excel <== ajout du menu contextuel
#endregion

#region NameSpace
using System;
using System.Text;
using System.Data;
using System.Collections;
using System.Web.UI;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.Exceptions;
using TNS.AdExpress.Web.Functions;
using CstClassification = TNS.AdExpress.Constantes.Classification;
using DBClassificationConstantes=TNS.AdExpress.Constantes.Classification.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebExceptions = TNS.AdExpress.Web.Exceptions;
using TNS.FrameWork;
using ExcelFunction=TNS.AdExpress.Web.UI.ExcelWebPage;
#endregion

namespace TNS.AdExpress.Web.UI.Results{
	
	/// <summary>
	/// Construit le code HTML du tableau présentant le détail des insertions avec les créations associées
	/// </summary>
	public class CompetitorAlertCreationsResultsUI{

		#region HTML UI
	
		/// <summary>
		/// Méthode globale
		/// </summary>
		/// <param name="webSession">Session Client</param>
        /// <param name="periodBegin">Début de période au format YYYYMMDD</param>
        /// <param name="periodEnd">Fin de période au format YYYYMMDD</param>
        /// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idElement">identifiant de l'élément sélectionné</param>
		/// <param name="level">Niveau où se trouve l'élément</param>
		/// <param name="page">page</param>
		/// <returns>tableau présentant le détail des insertions avec les créations associées</returns>
		public static string GetAlertCompetitorCreationsResultsUI(WebSession webSession,int periodBegin, int periodEnd, string idVehicle,Int64 idElement,int level ,Page page) {

            #region cas du média Internet
            if ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle) == CstClassification.DB.Vehicles.names.internet)
                return GetUIInternet(webSession.SiteLanguage);
            #endregion

            #region Récupération des données
            object[,] tab=null;
			try{
				tab = CompetitorAlertCreationsRules.GetData(webSession, periodBegin, periodEnd, idVehicle,idElement,level);
			}
			catch(System.Exception err){
				throw(new WebExceptions.CompetitorAlertCreationsResultsUIException("Impossible de rappatrier les données pour les création de l'alerte concurrentielle",err));
			}
			#endregion

			#region Table building
            switch ((CstClassification.DB.Vehicles.names)int.Parse(idVehicle))
            {
                case CstClassification.DB.Vehicles.names.press:
                case CstClassification.DB.Vehicles.names.internationalPress:
                    return GetUIPress(tab, webSession, page, idVehicle);
                case CstClassification.DB.Vehicles.names.radio:
                    return GetUIRadio(tab, webSession, page, idVehicle);
                case CstClassification.DB.Vehicles.names.tv:
                case CstClassification.DB.Vehicles.names.others:
                    return GetUITV(tab, webSession, page, idVehicle);
                case CstClassification.DB.Vehicles.names.outdoor:
                    return GetUIOutDoor(tab, webSession, page, idVehicle);
                default:
                    throw new CompetitorAlertCreationsResultsUIException("Le vehicle demandé n'est pas un cas traité");
            }
			#endregion
		}



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
		/// <param name="idVehicle">Identifiant du média</param>
		/// <returns>Code HTML des insertions Presse</returns>
		/// <remarks>
		/// Utilise les méthodes : 
		///		TNS.AdExpress.Web.Functions.Script.OpenPressCreation()
		///		private static string getUIEmpty(int language)
		/// </remarks>
		private static string GetUIPress(object[,] data, WebSession webSession, Page page, string idVehicle) {

			StringBuilder HtmlTxt = new StringBuilder(70000);

			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" ");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"center\" border=\"0\">");

			#region Pas de données
			if (data == null || data[0, 0] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
			}
			#endregion

			#region Données présentes
	
			#region script openCreationPress
			if (!page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openPressCreation", WebFunctions.Script.OpenPressCreation());
			}
			#endregion

			#region Construction du tableau
			int i = 0;
			string currentMedia="";
			string currentCategory="";

			#region Libellé vehicule et période
			HtmlTxt.Append("<tr height=\"25\" vAlign=\"center\"><td class=\"txtViolet14Bold\">");
			HtmlTxt.AppendFormat("{0} {1}", GestionWeb.GetWebWord(845,webSession.SiteLanguage), data[0,CstWeb.PressInsertionsColumnIndex.VEHICLE_INDEX].ToString());
			HtmlTxt.Append("");
			HtmlTxt.Append("<td></tr>");
			#endregion

			#region Tableau
			while (i<data.GetLength(0) && data[i,0]!=null){
				
				currentCategory = data[i,CstWeb.PressInsertionsColumnIndex.CATEGORY_INDEX].ToString();

				#region Rappel Catégorie
				HtmlTxt.Append("<TR>");
				HtmlTxt.Append("<td  vAlign=\"top\" width=\"100%\">");
				HtmlTxt.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
				HtmlTxt.Append("<td>");
				HtmlTxt.Append("<!-- fleche -->");
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<!-- message -->");
				HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
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
					string oldDate = "";
					string oldPage="";
					bool first = true;
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
							HtmlTxt.Append("<td "+((!first)?"style=\"BORDER-TOP: #9966cc 1px solid;\"":"") +" class=\"txtViolet11Bold\">Page "+ oldPage + "</td>");
							HtmlTxt.Append("</tr>");
							first = false;
						}
						HtmlTxt.Append("<tr class=\"popupinsertionligne\"><td ><TABLE cellSpacing=\"0\" border=\"0\"><tr>");
						if (((string)data[i, CstWeb.PressInsertionsColumnIndex.VISUAL_INDEX]).CompareTo("")==0
                            || !webSession.CustomerLogin.ShowCreatives((CstClassification.DB.Vehicles.names)int.Parse(idVehicle))) {
							//|| webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_CREATION_ACCESS_FLAG)==null){
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
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(176,webSession.SiteLanguage) + "</td><td width=\"550\">: "+data[i, CstWeb.PressInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(174,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(468,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(299,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.FORMAT_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(469,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.AREA_PAGE_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(561,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.COLOR_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(471,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\"><td>&nbsp" + GestionWeb.GetWebWord(472,webSession.SiteLanguage) + "</td><td>: "+data[i, CstWeb.PressInsertionsColumnIndex.LOCATION_INDEX].ToString()+"</td></tr>");
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

			//fin du tableau générale
			HtmlTxt.Append("</TABLE>");

			#endregion
			
			#endregion
			
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
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string getUIEmpty(int language)
		/// </remarks>
		private static string GetUIRadio(object[,] data, WebSession webSession, Page page, string idVehicle){
			
			StringBuilder HtmlTxt = new StringBuilder(70000);

			const string CLASSE_1="p6";
			const string CLASSE_2="p7";

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" ");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
			#endregion

			#region Pas de données à afficher
			if (data == null || data[0, 0] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
			}
			#endregion

			#region script d'ouverture d'une popUp de téléchargement
			if (!page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openDownload", WebFunctions.Script.OpenDownload());
			}
			#endregion

			#region Paramètres (vehicle, dates...)
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD colSpan=\"13\" valign=\"center\" class=\"txtViolet14Bold\" >");
            HtmlTxt.AppendFormat("{0} {1}", GestionWeb.GetWebWord(845, webSession.SiteLanguage), data[0, CstWeb.RadioInsertionsColumnIndex.VEHICLE_INDEX].ToString());
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");			
			#endregion

			#region Tableaux d'insertions
			int i = 0;
			string currentCategory="";
			string currentMedia="";
			
			#region Niveau Catégorie
			while(i<data.GetLength(0) && data[i,0]!=null){
				currentCategory = data[i, CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString();
				#region Infos Catégorie
				HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
				HtmlTxt.Append("<tr><td colSpan=\"13\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<TD class=\"insertionCategory\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
				HtmlTxt.Append(currentCategory);
				HtmlTxt.Append("</TD></tr>");
				HtmlTxt.Append("</table></td></TR>");							
				#endregion

				#region Niveau Support
				while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.RadioInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
					currentMedia = data[i, CstWeb.RadioInsertionsColumnIndex.MEDIA_INDEX].ToString();
					#region Infos Support
					HtmlTxt.Append("<TR>");
					HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\"13\">");
					HtmlTxt.Append(currentMedia);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</TR>");							
					#endregion

					#region Entêtes de tableaux

					string oldDate = "";
					bool first = true;
					string classe="";

					HtmlTxt.Append("<tr>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
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
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\"13\">&nbsp&nbsp");
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
						if (data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString().CompareTo("") != 0
							&& webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_RADIO_CREATION_ACCESS_FLAG) != null) {

							if (data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX] != null && data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString().CompareTo("0") != 0)
								HtmlTxt.Append("<tr><td class=\"" + classe + "\" nowrap><a href=\"javascript:openDownload('" + data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString() + "," + data[i, CstWeb.RadioInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString() + "','" + webSession.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/Images/Common/Picto_Radio.gif\"></a></td>");
							else HtmlTxt.Append("<tr><td class=\"" + classe + "\" nowrap><a href=\"javascript:openDownload('" + data[i, CstWeb.RadioInsertionsColumnIndex.FILE_INDEX].ToString() + "','" + webSession.IdSession + "','" + idVehicle + "');\"><img border=\"0\" src=\"/Images/Common/Picto_Radio.gif\"></a></td>");
						}
						else {
							HtmlTxt.Append("<tr><td class=\"" + classe + "\" nowrap></td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.RadioInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
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
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string getUIEmpty(int language)
		/// </remarks>
		private static string GetUITV(object[,] data, WebSession webSession, Page page, string idVehicle){

			StringBuilder HtmlTxt = new StringBuilder(70000);

			const string CLASSE_1="p6";
			const string CLASSE_2="p7";

			#region Début du tableau (lien excel, support et dates)
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\"");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"center\" border=\"0\">");
			#endregion

			#region Pas de données à afficher
			if (data == null || data[0, 0] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage)).ToString();
			}
			#endregion
			
			#region script d'ouverture d'une popUp de téléchargement
			StringBuilder script = new StringBuilder(500);
			if (!page.ClientScript.IsClientScriptBlockRegistered("openDownload")){
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openDownload", WebFunctions.Script.OpenDownload());
			}
			#endregion

			#region Paramètres (vehicle, dates...)
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD colSpan=\"11\" valign=\"center\" class=\"txtViolet14Bold\" >");
            HtmlTxt.AppendFormat("{0} {1}", GestionWeb.GetWebWord(845, webSession.SiteLanguage), data[0, CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX].ToString());
            HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");			
			#endregion

			#region Tableaux d'insertions
			int i = 0;
			string currentCategory="";
			string currentMedia="";

			#region Niveau Catégorie
			while(i<data.GetLength(0) && data[i,0]!=null){
				currentCategory = data[i, CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString();

				#region Infos Catégorie
				HtmlTxt.Append("<TR><td>&nbsp;</td></tr>");
				HtmlTxt.Append("<tr><td colSpan=\"11\"><table width=\"100%\" cellSpacing=\"0\" cellPadding=\"0\"><tr>");
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<TD class=\"insertionCategory\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
				HtmlTxt.Append(currentCategory);
				HtmlTxt.Append("</TD></tr>");
				HtmlTxt.Append("</table></td></TR>");							
				#endregion

				#region Niveau Support
				while(i<data.GetLength(0) && data[i,0]!=null && currentCategory.CompareTo(data[i,CstWeb.TVInsertionsColumnIndex.CATEGORY_INDEX].ToString())==0){
					currentMedia = data[i, CstWeb.TVInsertionsColumnIndex.MEDIA_INDEX].ToString();
					#region Infos Support
					HtmlTxt.Append("<TR>");
					HtmlTxt.Append("<TD class=\"insertionMedia\" colSpan=\"11\">");
					HtmlTxt.Append(currentMedia);
					HtmlTxt.Append("</TD>");
					HtmlTxt.Append("</TR>");							
					#endregion

					#region Entêtes de tableaux

					string oldDate = "";
					bool first = true;
					string classe="";

					HtmlTxt.Append("<tr>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(869,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
					HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
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
							HtmlTxt.Append("<TD class=\"insertionHeader\" colSpan=\"11\">&nbsp&nbsp");
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
						if(data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString().CompareTo("")!=0
							&& webSession.CustomerLogin.ShowCreatives((CstClassification.DB.Vehicles.names)int.Parse(idVehicle))
							){
							HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap><a href=\"javascript:openDownload('"+data[i,CstWeb.TVInsertionsColumnIndex.FILES_INDEX].ToString()+"','"+webSession.IdSession+"','"+idVehicle+"');\"><img border=\"0\" src=\"/Images/Common/Picto_pellicule.gif\"></a></td>");
						}
						else{
							HtmlTxt.Append("<tr><td class=\""+classe+"\" nowrap></td>");
						}
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
						HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.TVInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
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

			return HtmlTxt.ToString();

		}
		#endregion

		#region Publicité Extérieure		

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
		/// <param name="idVehicle">Identifiant du vehicule</param>
		/// <returns>Code html généré</returns>
		/// <remarks>
		/// Utilise les méthodes:
		///		TNS.AdExpress.Web.Functions.Script.OpenDownload()
		///		private static string GetUIEmpty(int language)
		/// </remarks>
		private static string GetUIOutDoor(object[,] data, WebSession webSession, Page page, string idVehicle) {


			StringBuilder HtmlTxt = new StringBuilder(1000);
			string ColSpan = "";
			bool first = true;
			string classe = "";
			string oldDate = "";
			const string CLASSE_1 = "p6";
			const string CLASSE_2 = "p7";
			int i = 0;

			// Pas de données à afficher
			if (data == null || data[0, 0] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage, 177)).ToString();
			}

			//Pas de droit d'accès aux insertions de la Publicité extérieure
			if (webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG] == null) {
				return HtmlTxt.Append(GetUIEmpty(webSession.SiteLanguage, 1882)).ToString();
			}

			#region script openCreationPress
			if (!page.ClientScript.IsClientScriptBlockRegistered("openPressCreation")) {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "openPressCreation", WebFunctions.Script.OpenPressCreation());
			}
			#endregion

			#region Détail insertion outdoor sans gestion des colonnes génériques

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\"");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"3\" align=\"left\" border=\"0\">");
			#endregion

			#region Paramètres (vehicle, dates...)
			HtmlTxt.Append("<TR>");
			HtmlTxt.Append("<TD valign=\"center\" class=\"txtViolet14Bold\" >");
            HtmlTxt.AppendFormat("{0} {1}", GestionWeb.GetWebWord(845, webSession.SiteLanguage), data[0, CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX].ToString());
            HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
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
				HtmlTxt.Append("<TD style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/Images/Common/fleche_1.gif\" border=\"0\"></TD>");
				HtmlTxt.Append("<!-- message -->");
				HtmlTxt.Append("<TD  vAlign=\"top\" width=\"100%\" class=\"popuptitle1\" style=\"BACKGROUND-POSITION-X: right; BACKGROUND-IMAGE: url(/Images/Common/bandeau_titre.gif);BACKGROUND-REPEAT: repeat-y\">");
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
						HtmlTxt.Append("<tr width=\"100%\" class=\"popupinsertionligne\">");
						HtmlTxt.Append("<td width=\"100%\" " + ((!first) ? "style=\"BORDER-TOP: #9966cc 1px solid;\"" : "") + " class=\"txtViolet11Bold\">&nbsp;</td>");
						HtmlTxt.Append("</tr>");

						HtmlTxt.Append("<tr class=\"popupinsertionligne\"><td width=\"100%\"><TABLE cellSpacing=\"0\" border=\"0\"><tr>");
						if (data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX] == null || ((string)data[i, CstWeb.OutDoorInsertionsColumnIndex.FILES_INDEX]).CompareTo("") == 0
							|| (webSession.CustomerLogin.GetFlag(CstDB.Flags.ID_OUTDOOR_CREATION_ACCESS_FLAG) == null)
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
						//if (Functions.MediaDetailLevel.HasSloganRight(webSession) && !webSession.isCompetitorAdvertiserSelected())// si droit accroche
						//    HtmlTxt.Append("<tr valign=\"top\" nowrap><td>&nbsp;" + GestionWeb.GetWebWord(1881, webSession.SiteLanguage) + "</td><td >: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.ID_SLOGAN_INDEX].ToString() : "") + "</td></tr>");
						//if (!webSession.isCompetitorAdvertiserSelected()) {
						//    HtmlTxt.Append("<tr valign=\"top\" nowrap><td>&nbsp;" + GestionWeb.GetWebWord(1384, webSession.SiteLanguage) + "</td><td>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.INTEREST_CENTER_INDEX].ToString() : "") + "</td></tr>");
						//    HtmlTxt.Append("<tr valign=\"top\" nowrap><td>&nbsp;" + GestionWeb.GetWebWord(1383, webSession.SiteLanguage) + "</td><td>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.MEDIA_SELLER_INDEX].ToString() : "") + "</td></tr>");
						//}
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1604, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX].ToString() : "") + "</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1420, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX].ToString() : "") + "</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1609, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX] != null) ? Convertion.ToHtmlString(TNS.AdExpress.Web.Functions.SQLGenerator.SaleTypeOutdoor(data[i, CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX].ToString(), webSession.SiteLanguage)) : "") + "</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1611, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX] != null) ? Convertion.ToHtmlString(data[i, CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX].ToString()) : "") + "</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(1660, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX].ToString() : "") + "</td></tr>");
						HtmlTxt.Append("<tr valign=\"top\" nowrap><td nowrap>&nbsp;" + GestionWeb.GetWebWord(868, webSession.SiteLanguage) + "</td><td nowrap>: " + ((data[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX] != null) ? data[i, CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX].ToString() : "") + "</td></tr>");
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


			return HtmlTxt.ToString();

		}
		#endregion

        #region Internet
        /// <summary>
        /// Génère le code html pour le média Internet
        /// </summary>
        /// <param name="language">Langue du site</param>
        /// <returns>Code html Généré</returns>
        private static string GetUIInternet(int language) {
            StringBuilder HtmlTxt = new StringBuilder(10000);

            HtmlTxt.Append("<TR vAlign=\"top\" >");
            HtmlTxt.Append("<TD vAlign=\"top\">");
            HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
            HtmlTxt.Append("<tr><td></td></tr>");
            HtmlTxt.Append("<TR vAlign=\"top\">");
            HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
            HtmlTxt.Append(GestionWeb.GetWebWord(2244, language));
            HtmlTxt.Append("</TD>");
            HtmlTxt.Append("</TR>");
            HtmlTxt.Append("</TABLE>");
            HtmlTxt.Append("</TD>");
            HtmlTxt.Append("</TR>");

            return HtmlTxt.ToString();
        }

        #endregion

        #region pas de Creations
        /// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIEmpty(int language){
			StringBuilder HtmlTxt = new StringBuilder(10000);
			
			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(177,language));			
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			return HtmlTxt.ToString();
		}

		/// <summary>
		/// Génère le code html précisant qu'il n 'y a pas de données à afficher
		/// </summary>
		/// <param name="language">Langue du site</param>
		/// <param name="code">code traduction</param>
		/// <returns>Code html Généré</returns>
		public static string GetUIEmpty(int language, int code) {
			StringBuilder HtmlTxt = new StringBuilder(10000);

			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\"");
			HtmlTxt.Append("cellPadding=\"0\" cellSpacing=\"0\" align=\"left\" border=\"0\">");

			HtmlTxt.Append("<TR vAlign=\"top\" >");
			HtmlTxt.Append("<TD vAlign=\"top\">");
			HtmlTxt.Append("<TABLE vAlign=\"top\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			HtmlTxt.Append("<tr><td></td></tr>");
			HtmlTxt.Append("<TR vAlign=\"top\">");
			HtmlTxt.Append("<TD vAlign=\"top\" height=\"50\" class=\"txtViolet11Bold\">");
			HtmlTxt.Append(GestionWeb.GetWebWord(code, language));
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");
			HtmlTxt.Append("</TABLE>");
			HtmlTxt.Append("</TD>");
			HtmlTxt.Append("</TR>");

			HtmlTxt.Append("</TABLE>");
			return HtmlTxt.ToString();
		}

		#endregion

		#endregion

		#region Excel UI	
		/// <summary>
		/// Méthode globale pour l'export Excel
		/// </summary>
		/// <param name="webSession">Session client</param>
		/// <param name="idVehicle">identifiant vehicle</param>
		/// <param name="idElement">identifiant de l'élément</param>
		/// <param name="level">Niveau de l'élément sélectionné</param>
		/// <param name="page">Page</param>
		/// <returns>tableau pour l'export Excel présentant le détail des insertions avec les créations associées</returns>
		public static string GetCompetitorAlertCreationsExcelUI(WebSession webSession, string idVehicle,Int64 idElement,int level,Page page){

            #region Mise en forme des dates

            int dateBegin = int.Parse(WebFunctions.Dates.getPeriodBeginningDate(webSession.PeriodBeginningDate, webSession.PeriodType).ToString("yyyyMMdd"));
            int dateEnd = int.Parse(WebFunctions.Dates.getPeriodEndDate(webSession.PeriodEndDate, webSession.PeriodType).ToString("yyyyMMdd"));

            #endregion


			#region Récupération des données
			object[,] tab=null;
			try{
				tab = CompetitorAlertCreationsRules.GetData(webSession, dateBegin, dateEnd, idVehicle, idElement, level);
			}
			catch(System.Exception err){
				throw(new WebExceptions.CompetitorAlertCreationsResultsUIException("Impossible de rappatrier les données pour les création de l'alerte concurrentielle",err));
			}
			#endregion

			#region construction du txt Excel
			switch((CstClassification.DB.Vehicles.names) int.Parse(idVehicle)){
				case CstClassification.DB.Vehicles.names.press:
				case CstClassification.DB.Vehicles.names.internationalPress:
					return GetUIPressExcel(tab, webSession, page,webSession.PeriodBeginningDate, webSession.PeriodEndDate, idElement, level);
				case CstClassification.DB.Vehicles.names.radio:
					return GetUIRadioExcel(tab, webSession, page, webSession.PeriodBeginningDate, webSession.PeriodEndDate, idElement, level);
				case CstClassification.DB.Vehicles.names.tv:
				case CstClassification.DB.Vehicles.names.others:
					return GetUITVExcel(tab, webSession, page, webSession.PeriodBeginningDate, webSession.PeriodEndDate, idElement, level);
				case CstClassification.DB.Vehicles.names.outdoor:
					return GetUIOutDoorExcel(tab, webSession, page, webSession.PeriodBeginningDate, webSession.PeriodEndDate, idElement, level);
				default:
					throw new CompetitorAlertCreationsResultsUIException("Le vehicle demandé n'est pas un cas traité");
			}
			#endregion
		}

	
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
		/// <param name="idElement">identifiant de l'élément</param>
		/// <param name="level">Niveau de l'élément sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIPressExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, Int64 idElement,int level){

			StringBuilder HtmlTxt = new StringBuilder(1000);

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUp(webSession,false,periodBeginning,periodEnd, idElement, level));
			#endregion

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" cellPadding=\"0\" align=\"center\" border=\"0\">");
			#endregion

			#region Pas de données à afficher
			if (data == null || data[0, 0] == null) {

				HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
				HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
				return HtmlTxt.ToString();
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
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(468,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(299,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(469,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(561,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(471,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(472,webSession.SiteLanguage) + "</td>");
			HtmlTxt.Append("</tr>");
			#endregion
			
			#region Tableau de détail des insertions
			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";

			bool first = true;
			int i = 0;
			string classe;
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
		/// <param name="idElement">identifiant de l'élément</param>
		/// <param name="level">Niveau de l'élément sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIRadioExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, Int64 idElement,int level){

			StringBuilder HtmlTxt = new StringBuilder(1000);

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUp(webSession,false,periodBeginning,periodEnd, idElement, level));
			#endregion

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" cellPadding=\"0\" align=\"center\" border=\"0\">");

//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(845,webSession.SiteLanguage) + " " + data[0,CstWeb.RadioInsertionsColumnIndex.VEHICLE_INDEX].ToString()+"</td></tr>"); 
//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(844,webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType),webSession.SiteLanguage)
//				+ GestionWeb.GetWebWord(897, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodEndDate(periodEnd, webSession.PeriodType),webSession.SiteLanguage)+ "</td></tr>");
			#endregion

			#region Pas de données à afficher
			if (data == null || data[0, 0] == null) {				
				 HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 13)).ToString();
				 HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
				 return HtmlTxt.ToString();
			}
			#endregion

			#region Entêtes de tableau
			else{HtmlTxt.Append("<tr>");
				HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(970,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td nowrap class=\"p2\">" + GestionWeb.GetWebWord(971,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(895,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"p2\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
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
			int i = 0;
			bool first = true;
			string classe="";

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
		/// <param name="idElement">identifiant de l'élément</param>
		/// <param name="level">Niveau de l'élément sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUITVExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, Int64 idElement,int level){
			StringBuilder HtmlTxt = new StringBuilder(1000);

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUp(webSession,false,periodBeginning,periodEnd, idElement, level));
			#endregion

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" cellPadding=\"0\" align=\"center\" border=\"0\">");

//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(845,webSession.SiteLanguage) + " " + data[0,CstWeb.TVInsertionsColumnIndex.VEHICLE_INDEX].ToString()+"</td></tr>"); 
//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(844,webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType),webSession.SiteLanguage)
//				+ GestionWeb.GetWebWord(897, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodEndDate(periodEnd, webSession.PeriodType),webSession.SiteLanguage)+ "</td></tr>");
			#endregion

			#region Pas de donnée à afficher
			if (data==null || data[0, 0] == null) {
				HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
				HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
				return HtmlTxt.ToString();
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
			int i = 0;
			bool first = true;
			string classe="";

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
			
			HtmlTxt.Append("</TABLE>");
			#endregion

			HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
			return HtmlTxt.ToString();

		}
		#endregion

		#region Publicité Extérieure
		/// <summary>
		/// Génère le code HTML nécessaire à l'export du détail des insertions Publicité Extérieure :
		///		data vide : Code HTML spécifiant une erreur (cas normalement impossible)
		///		data non vide : 
		///			Génération du tableau ordonné par catégorie > support > date > page
		/// </summary>
		/// <param name="data">Tableau de données</param>
		/// <param name="webSession">Session utilisateur</param>
		/// <param name="page">Page résultat</param>
		/// <param name="periodBeginning">Période de début d'affichage</param>
		/// <param name="periodEnd">Période de fin d'affichage</param>
		/// <param name="idElement">identifiant de l'élément</param>
		/// <param name="level">Niveau de l'élément sélectionné</param>
		/// <returns>Code html Généré</returns>
		private static string GetUIOutDoorExcel(object[,] data, WebSession webSession, Page page, string periodBeginning, string periodEnd, Int64 idElement,int level){
			StringBuilder HtmlTxt = new StringBuilder(1000);

			const string CLASSE_1="pmmediaxls1";
			const string CLASSE_2="pmmediaxls2";

			#region Paramètres du tableau
            HtmlTxt.Append(ExcelFunction.GetLogo(webSession));
			HtmlTxt.Append(ExcelFunction.GetExcelHeaderForCreationsPopUp(webSession,false,periodBeginning,periodEnd, idElement, level));
			#endregion

			#region Début du tableau
			HtmlTxt.Append("<TABLE width=\"500\" bgColor=\"#ffffff\" cellPadding=\"0\" align=\"center\" border=\"0\">");

//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(845,webSession.SiteLanguage) + " " + data[0,CstWeb.OutDoorInsertionsColumnIndex.VEHICLE_INDEX].ToString()+"</td></tr>"); 
//			HtmlTxt.Append("<tr><TD class=\"p2\">"+GestionWeb.GetWebWord(844,webSession.SiteLanguage) + " " + GestionWeb.GetWebWord(896, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodBeginningDate(periodBeginning, webSession.PeriodType),webSession.SiteLanguage)
//				+ GestionWeb.GetWebWord(897, webSession.SiteLanguage) + Dates.dateToString(Dates.getPeriodEndDate(periodEnd, webSession.PeriodType),webSession.SiteLanguage)+ "</td></tr>");
			#endregion

			#region Pas de donnée à afficher
			if (data == null || data[0, 0] == null || webSession.CustomerLogin.FlagsList[(long)TNS.AdExpress.Constantes.DB.Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG] == null) {
				 HtmlTxt.Append(GetUIEmptyExcel(webSession.SiteLanguage, 10)).ToString();
				HtmlTxt.Append(ExcelFunction.GetFooter(webSession));
				return HtmlTxt.ToString();
			}
			#endregion

			#region Entêtes de tableau
			else{HtmlTxt.Append("<tr>");
				//HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1610,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(857,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(858,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(859,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1604,webSession.SiteLanguage)+"</td>");					
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1420,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1609,webSession.SiteLanguage))+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+Convertion.ToHtmlString(GestionWeb.GetWebWord(1611,webSession.SiteLanguage))+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(1660,webSession.SiteLanguage)+"</td>");
				HtmlTxt.Append("<td class=\"insertionHeader\" align=\"center\" nowrap>"+GestionWeb.GetWebWord(868,webSession.SiteLanguage)+"</td>");					
				HtmlTxt.Append("</tr>");
			}
			#endregion
			
			#region Tableau
			int i = 0;
			bool first = true;
			string classe="";

			while(i<data.GetLength(0) && data[i,0]!=null){
				if (first){
					first=false;
					classe=CLASSE_1;
				}
				else{
					first=true;
					classe=CLASSE_2;
				}
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.OutDoorInsertionsColumnIndex.ADVERTISER_INDEX].ToString()+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.OutDoorInsertionsColumnIndex.PRODUCT_INDEX].ToString()+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap>"+data[i,CstWeb.OutDoorInsertionsColumnIndex.GROUP_INDEX].ToString()+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.OutDoorInsertionsColumnIndex.NUMBER_BOARD_INDEX].ToString()+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"center\">"+data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_BOARD_INDEX].ToString()+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ Convertion.ToHtmlString(SQLGenerator.SaleTypeOutdoor(data[i,CstWeb.OutDoorInsertionsColumnIndex.TYPE_SALE_INDEX].ToString(),webSession.SiteLanguage))+"</td>");
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+ Convertion.ToHtmlString(data[i,CstWeb.OutDoorInsertionsColumnIndex.POSTER_NETWORK_INDEX].ToString())+"</td>");						
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.OutDoorInsertionsColumnIndex.AGGLOMERATION_INDEX].ToString()+"</td>");						
				HtmlTxt.Append("<td class=\""+classe+"\" nowrap align=\"right\">"+data[i,CstWeb.OutDoorInsertionsColumnIndex.EXPENDITURE_INDEX].ToString()+"</td>");
				HtmlTxt.Append("</tr>");

				i++;
			}
			
			HtmlTxt.Append("</TABLE>");
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

	}
}

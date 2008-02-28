using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using System.Collections;

using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Headers {

	/// <summary>
	/// Type défini pour la sélection d'un type de page
	/// </summary>
	public enum PageType {
		/// <summary>
		/// Home Page
		/// </summary>
		homepage,
		/// <summary>
		/// Pages des mosules
		/// </summary>
		generic
	}


	/// <summary>
	/// Description résumée de Header.
	/// </summary>
	[DefaultProperty("IdLanguage"),
	ToolboxData("<{0}:HeaderWebControl Language=33 runat=server></{0}:HeaderWebControl>")]
	public class HeaderWebControl : System.Web.UI.WebControls.WebControl {

		#region Variables
		/// <summary>
		/// Langage du site
		/// </summary>
		private int language;
		///<summary>
		/// Type de page
		/// </summary>
		///  <supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		///  <label>pageType</label>
		private PageType pageType;
		/// <summary>
		/// Indique le menu qui est actif
		/// </summary>
		private int activeMenu = -1;
        /// <summary>
        /// Flash URL
        /// </summary>
        private string flashUrl = string.Empty;
        /// <summary>
        /// Missing Flash URL
        /// </summary>
        private string missingFlashUrl = string.Empty;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit propriété de langage
		/// </summary>
		[Bindable(true),
		Description("Langue du site")]
		public int Language {
			get { return language; }
			set { language = value; }
		}

		/// <summary>
		/// Obtient ou définit le type de page déterminant pour les menus présents
		/// </summary>
		[Description("Page d'accueil ou page générique")]
		public PageType Type_de_page {
			get { return pageType; }
			set { pageType = value; }
		}

		/// <summary>
		/// Obtient ou défnit le menu qui est actif
		/// </summary>
		[Description("Spécifie l'index du menu actif(0...)"),
		DefaultValue(0)]
		public int ActiveMenu {
			get { return activeMenu; }
			set { activeMenu = value; }
		}

        /// <summary>
        /// SET or GET flash URL
        /// </summary>
        public string FlashUrl {
            get { return flashUrl; }
            set { flashUrl = value; }
        }
        
        /// <summary>
        /// SET or GET missing flash URL
        /// </summary>
        public string MissingFlashUrl {
            get { return missingFlashUrl; }
            set { missingFlashUrl = value; }
        }
		#endregion

		#region Evènements

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
            //if (!Page.IsClientScriptBlockRegistered("detectFlash")) {
            //    string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
            //    Page.RegisterClientScriptBlock("detectFlash", tmp);
            //}

            if(!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")) {
                string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "detectFlash", tmp);
            }

            if(!Page.ClientScript.IsClientScriptBlockRegistered(TNS.AdExpress.Web.Functions.Script.RESIZABLE_POPUP_REGISTER)) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), TNS.AdExpress.Web.Functions.Script.RESIZABLE_POPUP_REGISTER, TNS.AdExpress.Web.Functions.Script.ReSizablePopUp());
            }

			base.OnPreRender(e);
		}
		#endregion

		#region Render (Affichage)
		/// <summary>
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			Hashtable headers = HeaderRules.Headers;

			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\">");
			output.Write("\n<tr>");
			output.Write("\n<td colspan=\"2\">");
			output.Write("\n<table  class=\"header\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			output.Write("\n<td width=\"1%\">");
			output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
			output.Write("\nif(hasRightFlashVersion==true){");
			output.Write("\ndocument.writeln('<OBJECT id=\"Shockwaveflash1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
			output.Write("\ndocument.writeln('height=\"31\" width=\"733\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cx\" VALUE=\"19394\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cy\" VALUE=\"820\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"FlashVars\" VALUE=\"\">');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"Movie\" VALUE=\"" + flashUrl + "\"');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"Src\" VALUE=\"" + flashUrl + "\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"WMode\" VALUE=\"Window\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Play\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Loop\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Quality\" VALUE=\"High\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SAlign\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Menu\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Base\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"AllowScriptAccess\" VALUE=\"always\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Scale\" VALUE=\"ShowAll\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"DeviceFont\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"EmbedMovie\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"BGColor\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SWRemote\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"MovieData\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SeamlessTabbing\" VALUE=\"1\">');");
            output.Write("\ndocument.writeln('<embed src=\"" + flashUrl + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"');");
			output.Write("\ndocument.writeln('type=\"application/x-shockwave-flash\" width=\"733\" height=\"31\"> </embed>');");
			output.Write("\ndocument.writeln('</OBJECT>');");
			output.Write("\n}\nelse{");
            output.Write("\ndocument.writeln('<img src=\"" + missingFlashUrl + "\" width=\"733\" height=\"31\">');");
			output.Write("\n}");
			output.Write("\n</script>");
			output.Write("\n</td>");
			output.Write("\n<td></td>");
			output.Write("\n</table>");//
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
            output.Write("\n<td colspan=\"2\" class=\"dupli1BackGround\"><IMG height=\"1\" src=\"/Images/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
            output.Write("\n<tr class=\"txtBlanc11 headerBackGround\">");
			output.Write("\n<td>");
            output.Write("\n<p class=\"paragraphePaddingHeader\">");
			int i = 0;
			string menus = "";
			string href = "";
			string look = "";
			string languageString, idSessionString;
			bool firstParameter;
			HeaderMenuItem currentHeaderMenuItem = null;
			for (i = 0; i < ((Header)headers[pageType.ToString()]).MenuItems.Count; i++) {
				languageString = "";
				idSessionString = "";
				firstParameter = true;
				currentHeaderMenuItem = ((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]);
				//Options dans l url suivant le menuItem
				//Differenciation et inactivation du menu actif si nécessaire
				if (activeMenu == currentHeaderMenuItem.IdMenu) {
					href = "#";
					look = "roll03";
				}
				else {
					look = "roll01";

					#region Gestion des Paramètres

					#region Paramètre de la langue
					if (currentHeaderMenuItem.GiveLanguage) {
						languageString = "siteLanguage=" + language;
						if (firstParameter) {
							languageString = "?" + languageString;
							firstParameter = false;
						}
						else {
							languageString = "&" + languageString;
						}
					}
					#endregion

					#region Paramètre de l'identifiant de la session
					if (currentHeaderMenuItem.GiveSession) {
						idSessionString = "idSession=" + Page.Request.QueryString.Get("idSession").ToString();
						if (firstParameter) {
							idSessionString = "?" + idSessionString;
							firstParameter = false;
						}
						else {
							idSessionString = "&" + idSessionString;
						}
					}
					#endregion

					#endregion

					href = ((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).TargetUrl + languageString + idSessionString;
					if (currentHeaderMenuItem.Target.Length > 0) 
                        href += "\" target=\"" + currentHeaderMenuItem.Target + "\"";
				}
                if(currentHeaderMenuItem.DisplayInPopUp)
                    menus += "\n<A class=\"" + look + "\" href=\"javascript:popupOpenBis('" + href + "','975','600','yes');\">" + GestionWeb.GetWebWord((int)((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).IdMenu, language) + "</A> |";
                else
                 menus += "\n<A class=\"" + look + "\" href=\"" + href + "\">" + GestionWeb.GetWebWord((int)((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).IdMenu, language) + "</A> |";
			}
			menus = menus.Substring(0, menus.Length - 2);
			output.Write("{0}\n</p>", menus);
			output.Write("\n</td>");
			// A remettre s'il on veut pouvoir avoir accès à la traduction du site
			if (pageType == PageType.homepage) {
				string langButton = "";
				string link = "";
				switch (language) {
					case TNS.AdExpress.Constantes.DB.Language.ENGLISH:
						langButton = "Version française";
						link = this.Parent.Page.Request.Url.AbsolutePath.ToString() + "?siteLanguage=" + TNS.AdExpress.Constantes.DB.Language.FRENCH;
						break;
					default:
						langButton = "English version";
						link = this.Parent.Page.Request.Url.AbsolutePath.ToString() + "?siteLanguage=" + TNS.AdExpress.Constantes.DB.Language.ENGLISH;
						break;
				}
                output.Write("\n<td align=\"right\">\n<p class=\"paragraphePaddingHeader\">| <a class=\"roll01\" href=\"" + link + "\">" + langButton + " </a></p>\n</td>");
			}
			output.Write("\n</tr>");
			output.Write("\n</table>");

		}
		#endregion

		#endregion
	}
}

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Drawing;

using System.Collections;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;
using System.Collections.Generic;
using TNS.AdExpress.Web.Controls.Results;

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
	public class HeaderWebControl : System.Web.UI.WebControls.WebControl,ITranslation {

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
        /// <summary>
        /// Language selection web control
        /// </summary>
        private LanguageSelectionWebControl languageSelectionWebControl;
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
		protected override void OnPreRender(EventArgs e)
        {
            if(!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")) {
                string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "detectFlash", tmp);
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("LanguageSelectionScripts")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "LanguageSelectionScripts", TNS.AdExpress.Web.Functions.Script.LanguageSelectionScripts(true));
            }

            if (Page.Request.QueryString.Get("siteLanguage") != null) {
                if (!Page.ClientScript.IsClientScriptBlockRegistered("SetCookieScript")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetCookieScript", TNS.AdExpress.Web.Functions.Script.SetCookieScript(language));
                }
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("CookieScripts")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookieScripts", TNS.AdExpress.Web.Functions.Script.CookieScripts(language,GetLinkPath()));
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
            Dictionary<string,WebHeader> headers = WebHeaders.HeadersList;
            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[language].Name;
			
            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\">");
			output.Write("\n<tr>");
			output.Write("\n<td colspan=\"2\">");

            #region Flash
            output.Write("\n<table  class=\"header\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			output.Write("\n<td width=\"1%\">");
            output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
			output.Write("\nif(hasRightFlashVersion==true){");
			output.Write("\ndocument.writeln('<OBJECT id=\"Shockwaveflash1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
			output.Write("\ndocument.writeln('height=\"60\" width=\"648\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cx\" VALUE=\"19394\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cy\" VALUE=\"820\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"FlashVars\" VALUE=\"\">');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"Movie\" VALUE=\"" + flashUrl + "\"');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"Src\" VALUE=\"" + flashUrl + "\">');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"WMode\" VALUE=\"Transparent\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Play\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Loop\" VALUE=\"-1\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Quality\" VALUE=\"High\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SAlign\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Menu\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Base\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"AllowScriptAccess\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Scale\" VALUE=\"ShowAll\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"DeviceFont\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"EmbedMovie\" VALUE=\"0\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"BGColor\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SWRemote\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"MovieData\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"SeamlessTabbing\" VALUE=\"1\">');");
            output.Write("\ndocument.writeln('<embed src=\"" + flashUrl + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"');");
            output.Write("\ndocument.writeln('type=\"application/x-shockwave-flash\" wmode=\"opaque\" width=\"648\" height=\"60\"> </embed>');");
			output.Write("\ndocument.writeln('</OBJECT>');");
			output.Write("\n}");
            output.Write("\nelse{");
            output.Write("\ndocument.writeln('<img src=\"" + missingFlashUrl + "\" width=\"648\" height=\"60\">');");
			output.Write("\n}");
			output.Write("\n</script>");
			output.Write("\n</td>");
			output.Write("\n</table>");
            #endregion

            output.Write("\n</td>");
			output.Write("\n</tr>");

            output.Write("\n<tr>");
            output.Write("\n<td colspan=\"2\" class=\"backGroundBlack\" ><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td>");
            output.Write("\n</tr>");

            // Tabs
            output.Write("\n<tr class=\"headerBackGround\">");
			output.Write("\n<td>");
            output.Write("\n<div id=\"tabsH\">");
			output.Write("\n<ul>");
			
            int i = 0;
			string menus = "";
			string href = "";
            string currentItem = "";
			string languageString, idSessionString;
			bool firstParameter;
            bool oneActivatedMenuItem = false;

			WebHeaderMenuItem currentHeaderMenuItem = null;
			
            for (i = 0; i < headers[pageType.ToString()].MenuItems.Count; i++) {
				languageString = "";
				idSessionString = "";
				firstParameter = true;
				currentHeaderMenuItem = (WebHeaderMenuItem)headers[pageType.ToString()].MenuItems[i];
				
				if (activeMenu == currentHeaderMenuItem.IdMenu) {
					href = "#";
                    currentItem = " id=\"current\" ";
                    oneActivatedMenuItem = true;
				}
				else 
                {
                    currentItem = "";

					#region Gestion des Paramètres

					#region Paramètre de la langue
					if (currentHeaderMenuItem.GiveLanguage) {
						languageString = "siteLanguage=" + language;
						if (firstParameter) {
							languageString = "?" + languageString;
							firstParameter = false;
						}
						else
							languageString = "&" + languageString;
					}
					#endregion

					#region Paramètre de l'identifiant de la session
					if (currentHeaderMenuItem.GiveSession) {
						idSessionString = "idSession=" + Page.Request.QueryString.Get("idSession").ToString();
						if (firstParameter) {
							idSessionString = "?" + idSessionString;
							firstParameter = false;
						}
						else
							idSessionString = "&" + idSessionString;
					}
					#endregion

					#endregion

					href = ((WebHeaderMenuItem)headers[pageType.ToString()].MenuItems[i]).TargetUrl + languageString + idSessionString;
					if (currentHeaderMenuItem.Target.Length > 0) 
                        href += "\" target=\"" + currentHeaderMenuItem.Target + "\"";
				}
                if(currentHeaderMenuItem.DisplayInPopUp)
                    menus += "<li" + currentItem + "><a href=\"javascript:popupOpenBis('" + href + "','975','600','yes');\"><span>" + GestionWeb.GetWebWord((int)((WebHeaderMenuItem)headers[pageType.ToString()].MenuItems[i]).IdMenu, language) + "</span></a></li>";
                else
                    menus += "<li" + currentItem + "><a href=\"" + href + "\"><span>" + GestionWeb.GetWebWord((int)((WebHeaderMenuItem)headers[pageType.ToString()].MenuItems[i]).IdMenu, language) + "</span></a></li>";
			}

            // Add tabs in a result page
            if(pageType == PageType.generic && !oneActivatedMenuItem)
            {
                menus += "<li id=\"current\" ><a href=\"#\"><span>"
                    + GestionWeb.GetWebWord(793, language) 
                    + "</span></a></li>";
            }

            output.Write("\n{0}", menus);
            output.Write("\n</ul>");
            output.Write("\n</div>");
			output.Write("\n</td>");

            // Language
            if (WebApplicationParameters.AllowedLanguages.Count > 1) 
            {
                languageSelectionWebControl = new LanguageSelectionWebControl();
                languageSelectionWebControl.Language = language;
                languageSelectionWebControl.BackColor = Color.FromArgb(92, 92, 92);
                languageSelectionWebControl.BorderColor = Color.FromArgb(92, 92, 92);
                languageSelectionWebControl.BorderWidth = new System.Web.UI.WebControls.Unit(0);
                languageSelectionWebControl.ImageButtonArrow = "/App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down_white.gif";
                languageSelectionWebControl.ID = "DDL" + this.ID;
                languageSelectionWebControl.ImageHeight = 11;
                languageSelectionWebControl.ImageWidth = 16;
                languageSelectionWebControl.Height = 15;
                languageSelectionWebControl.OutCssClass = "ddlOut1";
                languageSelectionWebControl.OverCssClass = "ddlOver1";
                languageSelectionWebControl.ShowPictures = true;

                if (pageType == PageType.homepage) {
                    languageSelectionWebControl.Width = new System.Web.UI.WebControls.Unit("80");
                    languageSelectionWebControl.ShowText = true;
                }
                else {
                    languageSelectionWebControl.Width = new System.Web.UI.WebControls.Unit("25");
                }
                Controls.Add(languageSelectionWebControl);
                output.Write("\n<td align=\"right\" valign=\"top\">");
                languageSelectionWebControl.RenderControl(output);
                output.Write("\n</td>");
            }

            output.Write("\n</tr>");
			output.Write("\n</table>");
		}
		#endregion

		#endregion

        #region Private Methods
        /// <summary>
        /// Get Path and Query
        /// </summary>
        /// <returns>Path string</returns>
        private string GetLinkPath() {

            string link = this.Parent.Page.Request.Url.PathAndQuery.ToString();
            string[] linkParam = link.Split('?');
            string[] paramsList;
            bool verif = true;

            link = linkParam[0];

            if (linkParam.Length > 1) {
                paramsList = linkParam[1].Split('&');
                for (int i = 0; i < paramsList.Length; i++)
                    if (!paramsList[i].Contains("siteLanguage"))
                        if (verif) {
                            link += "?" + paramsList[i];
                            verif = false;
                        }
                        else
                            link += "&" + paramsList[i];
            }

            if (verif)
                link += "?siteLanguage=";
            else
                link += "&siteLanguage=";

            return link;
        }
        #endregion

	}
}

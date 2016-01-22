using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections;
using TNS.AdExpress.Bastet.Common.Headers;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet.Web;
using System.Drawing;

namespace TNS.AdExpress.Bastet.WebControls{
	
	/// <summary>
	/// Type défini pour la sélection d'un type de page
	/// </summary>
	public enum PageType{
		/// <summary>
		/// Home Page
		/// </summary>
		homepage,
        generic,
	}

	/// <summary>
	/// Description résumée de HeaderWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:HeaderWebControl runat=server></{0}:HeaderWebControl>")]
	public class HeaderWebControl : System.Web.UI.WebControls.WebControl{
		
		#region Variables
		/// <summary>
		/// Langage du site
		/// </summary>
		private int _siteLanguage;
		///<summary>
		/// Type de page
		/// </summary>
		///  <supplierCardinality>1</supplierCardinality>
		///  <directed>True</directed>
		///  <label>pageType</label>
		private PageType _pageType;
		/// <summary>
		/// Indique le menu qui est actif
		/// </summary>
		private int _activeMenu=-1;
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
		public int LanguageId{
			get{return _siteLanguage;}
			set{_siteLanguage = value;}
		}

		/// <summary>
		/// Obtient ou définit le type de page déterminant pour les menus présents
		/// </summary>
		[Description("Page d'accueil ou page générique")]
		public PageType Type_de_page{
			get{return _pageType;}
			set{_pageType = value;}
		}

		/// <summary>
		/// Obtient ou défnit le menu qui est actif
		/// </summary>
		[Description("Spécifie l'index du menu actif(0...)"),
		DefaultValue(0)]
		public int ActiveMenu{
			get{return _activeMenu;}
			set{_activeMenu = value;}
		}
		#endregion

        #region Propriété
        /// <summary>
        /// Image URL
        /// </summary>
        protected string _cssClassBis = string.Empty;
        /// <summary>
        /// Set Image URL
        /// </summary>
        public string CssClassBis {
            set { _cssClassBis = value; }
        }
        /// <summary>
        /// Flash URL
        /// </summary>
        protected string _flashUrl = string.Empty;
        /// <summary>
        /// Set flash Url
        /// </summary>
        public string FlashUrl {
            set { _flashUrl = value; }
        }
        /// <summary>
        /// Missing Flash URL
        /// </summary>
        protected string _missingFlashUrl = string.Empty;
        /// <summary>
        /// Set Missing Flash URL
        /// </summary>
        public string MissingFlashUrl {
            set { _missingFlashUrl = value; }
        }
        #endregion

        #region OnInit
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            if (WebApplicationParameters.AllowedLanguages.Count > 1) {

                languageSelectionWebControl = new LanguageSelectionWebControl();

                Controls.Add(languageSelectionWebControl);
            }
        }
        #endregion

        #region PréRendu
        /// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if (!Page.IsClientScriptBlockRegistered("detectFlash")){
				string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/Scripts/FlashChecking.js\"></SCRIPT>";
				Page.RegisterClientScriptBlock("detectFlash",tmp);
			}

            if (!Page.ClientScript.IsClientScriptBlockRegistered("LanguageSelectionScripts")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "LanguageSelectionScripts", TNS.AdExpress.Bastet.Functions.Script.LanguageSelectionScripts(true));
            }

            if (Page.Request.QueryString.Get("siteLanguage") != null) {
                if (!Page.ClientScript.IsClientScriptBlockRegistered("SetCookieScript")) {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetCookieScript", TNS.AdExpress.Bastet.Functions.Script.SetCookieScript(_siteLanguage));
                }
            }

            if (!Page.ClientScript.IsClientScriptBlockRegistered("CookieScripts")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookieScripts", TNS.AdExpress.Bastet.Functions.Script.CookieScripts(_siteLanguage, GetLinkPath()));
            }

			base.OnPreRender (e);
		}
		#endregion

		#region Render (Affichage)
		/// <summary>
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
			Hashtable headers = HeaderList.List;
			
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\" >");
			output.Write("\n<tr>");
			output.Write("\n<td colspan=\"2\">");
            output.Write("\n<table  class=\"" + this.CssClass + "\" cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
			output.Write("\n<td width=\"1%\">");
			output.Write("\n<script language=\"javascript\" type=\"text/javascript\">");
			output.Write("\nif(hasRightFlashVersion==true){");
			output.Write("\ndocument.writeln('<OBJECT id=\"Shockwaveflash1\" codeBase=\"http://download.macromedia.com/pub/shockwave/cabs/flash/swflash.cab#version=6,0,29,0\"');");
			output.Write("\ndocument.writeln('height=\"31\" width=\"733\" classid=\"clsid:D27CDB6E-AE6D-11cf-96B8-444553540000\" VIEWASTEXT>');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cx\" VALUE=\"19394\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"_cy\" VALUE=\"820\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"FlashVars\" VALUE=\"\">');");
			output.Write("\ndocument.writeln('<PARAM NAME=\"Movie\" VALUE=\""+_flashUrl+"\"');");
            output.Write("\ndocument.writeln('<PARAM NAME=\"Src\" VALUE=\"" + _flashUrl + "\">');");
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
            output.Write("\ndocument.writeln('<embed src=\"" + _flashUrl + "\" quality=\"high\" pluginspage=\"http://www.macromedia.com/go/getflashplayer\"');");
			output.Write("\ndocument.writeln('type=\"application/x-shockwave-flash\" width=\"733\" height=\"31\"> </embed>');");
			output.Write("\ndocument.writeln('</OBJECT>');");
			output.Write("\n}\nelse{");
			output.Write("\ndocument.writeln('<img src=\""+_missingFlashUrl+"\" width=\"733\" height=\"31\">');");
			output.Write("\n}");
			output.Write("\n</script>");
			output.Write("\n</td>");
			output.Write("\n<td></td>");
			output.Write("\n</table>");//
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
            output.Write("\n<td colspan=\"2\" class=\"dupli1BackGround\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr class=\""+_cssClassBis+"\">");
			output.Write("\n<td>");
			output.Write("\n<p>");
			int i=0;
			string menus="";
			string href="";
			string look="";
			string languageString,idSessionString; 
			bool firstParameter;
			HeaderMenuItem currentHeaderMenuItem=null;
			for(i=0; i<((Header)headers[_pageType.ToString()]).MenuItems.Count; i++){
				languageString="";
				idSessionString="";				
				firstParameter=true;
				currentHeaderMenuItem=((HeaderMenuItem)((Header)headers[_pageType.ToString()]).MenuItems[i]);
				//Options dans l url suivant le menuItem
				//Differenciation et inactivation du menu actif si nécessaire
				if(_activeMenu==currentHeaderMenuItem.Id){
					href="#";
					look="roll03";
				}
				else{
					look="roll01";

					#region Gestion des Paramètres

//					#region Paramètre de la langue
//					if(currentHeaderMenuItem.GiveLanguage){
//						languageString="siteLanguage="+language;
//						if(firstParameter){
//							languageString="?"+languageString;
//							firstParameter=false;
//						}
//						else{
//							languageString="&"+languageString;
//						}
//					}
//					#endregion
//
//					#region Paramètre de l'identifiant de la session
//					if(currentHeaderMenuItem.GiveSession){
//						idSessionString="idSession="+Page.Request.QueryString.Get("idSession").ToString();
//						if(firstParameter){
//							idSessionString="?"+idSessionString;
//							firstParameter=false;
//						}
//						else{
//							idSessionString="&"+idSessionString;
//						}
//					}
//					#endregion

					#endregion

					href = ((HeaderMenuItem)((Header)headers[_pageType.ToString()]).MenuItems[i]).TargetUrl + languageString + idSessionString;
					//if(currentHeaderMenuItem.Target.Length>0)href+="\" target=\""+currentHeaderMenuItem.Target+"\"";
				}
				//menus += "\n<A class=\""+look+"\" href=\""+href+"\">"+GestionWeb.GetWebWord((int)((HeaderMenuItem)((Header)headers[pageType.ToString()]).MenuItems[i]).IdMenu, language)+"</A> |";
				menus += "\n<A class=\""+look+"\" href=\""+href+"\">"+GestionWeb.GetWebWord(((HeaderMenuItem)((Header)headers[_pageType.ToString()]).MenuItems[i]).IdWebText, _siteLanguage)+"</A> |";
			}
            if (menus.Length > 2) {
                menus = menus.Substring(0, menus.Length - 2);
            }
            else {
                menus = "&nbsp;";
            }
                output.Write("{0}\n</p>", menus);
            
			output.Write("\n</td>");
			
//			// A remettre s'il on veut pouvoir avoir accès à la traduction du site
//			if (pageType==PageType.homepage){ 
//				string langButton="";
//				string link="";
//				switch (language){
//					case TNS.AdExpress.Constantes.DB.Language.ENGLISH:
//						langButton = "Version française";
//						link= this.Parent.Page.Request.Url.AbsolutePath.ToString()+"?siteLanguage="+TNS.AdExpress.Constantes.DB.Language.FRENCH;
//						break;
//					default:
//						langButton = "English version";
//						link= this.Parent.Page.Request.Url.AbsolutePath.ToString()+"?siteLanguage="+TNS.AdExpress.Constantes.DB.Language.ENGLISH;
//						break;
//				}
//				output.Write("\n<td align=\"right\">\n<p style=\"PADDING-LEFT: 10px; PADDING-BOTTOM: 40px; PADDING-TOP: 4px\">| <a class=\"roll01\" href=\""+link+"\">"+langButton+" </a></p>\n</td>");
//			}

            if (WebApplicationParameters.AllowedLanguages.Count > 1) {
                languageSelectionWebControl.Language = _siteLanguage;
                languageSelectionWebControl.BackColor = Color.FromArgb(143, 123, 166);
                languageSelectionWebControl.BorderColor = Color.FromArgb(143, 123, 166);
                languageSelectionWebControl.BorderWidth = new System.Web.UI.WebControls.Unit(0);
                languageSelectionWebControl.ImageButtonArrow = "/App_Themes/" + TNS.AdExpress.Bastet.Web.WebApplicationParameters.Themes[_siteLanguage].Name + "/Images/Common/Button/bt_arrow_down_white.gif";
                languageSelectionWebControl.ID = "DDL" + this.ID;
                languageSelectionWebControl.ImageHeight = 11;
                languageSelectionWebControl.ImageWidth = 16;
                languageSelectionWebControl.Height = 15;
                languageSelectionWebControl.OutCssClass = "ddlOut1";
                languageSelectionWebControl.OverCssClass = "ddlOver1";
                languageSelectionWebControl.ShowPictures = true;

                //if (_pageType == PageType.homepage) {
                languageSelectionWebControl.Width = new System.Web.UI.WebControls.Unit("80");
                languageSelectionWebControl.ShowText = true;
                /*}
                else {
                    languageSelectionWebControl.Width = new System.Web.UI.WebControls.Unit("25");
                }*/
                output.Write("\n<td align=\"right\" valign=\"top\">");
                languageSelectionWebControl.RenderControl(output);
                output.Write("</td>");

            }

			output.Write("\n</tr>");
			output.Write("\n</table>");

		}
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

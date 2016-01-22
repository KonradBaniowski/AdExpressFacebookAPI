using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using KMI.P3.Domain.Web.Navigation;
using KMI.P3.Domain.Translation;
using KMI.P3.Web.UI;

namespace KMI.P3.Web.Controls.Headers
{

	

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

       
		#endregion

		#region Evènements

		#region PréRendu
		/// <summary>
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e)
        {
            pageType=((KMI.P3.Web.UI.WebPage)this.Page).PageTypeInfo;
            //if(!Page.ClientScript.IsClientScriptBlockRegistered("detectFlash")) {
            //    string tmp = "\n<SCRIPT LANGUAGE=\"JavaScript\" type=\"text/javascript\" src=\"/scripts/FlashChecking.js\"></SCRIPT>";
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "detectFlash", tmp);
            //}

            //if (!Page.ClientScript.IsClientScriptBlockRegistered("LanguageSelectionScripts")) {
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "LanguageSelectionScripts", TNS.AdExpress.Web.Functions.Script.LanguageSelectionScripts(true));
            //}

            //if (Page.Request.QueryString.Get("siteLanguage") != null) {
            //    if (!Page.ClientScript.IsClientScriptBlockRegistered("SetCookieScript")) {
            //        Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "SetCookieScript", TNS.AdExpress.Web.Functions.Script.SetCookieScript(language));
            //    }
            //}

            //if (!Page.ClientScript.IsClientScriptBlockRegistered("CookieScripts")) {
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookieScripts", TNS.AdExpress.Web.Functions.Script.CookieScripts(language,GetLinkPath()));
            //}

            //if(!Page.ClientScript.IsClientScriptBlockRegistered(TNS.AdExpress.Web.Functions.Script.RESIZABLE_POPUP_REGISTER)) {
            //    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), TNS.AdExpress.Web.Functions.Script.RESIZABLE_POPUP_REGISTER, TNS.AdExpress.Web.Functions.Script.ReSizablePopUp());
            //}

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
            string themeName = this.Page.Theme;

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\" width=\"100%\">");
            //output.Write("\n<tr>");
            //output.Write("\n<td colspan=\"2\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"1\"></td>");
            //output.Write("\n</tr>");

            // Tabs
            if(pageType == PageType.disconnect)
                output.Write("\n<tr style=\"background-color:#000000;\">");
            else
                output.Write("\n<tr class=\"headerBackGround\">");

            output.Write("\n<td id=\"tabsH\">");
            //output.Write("\n<div id=\"tabsH\">");
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

            output.Write("\n{0}", menus);
            output.Write("\n</ul>");
            //output.Write("\n</div>");
			output.Write("\n</td>");
            output.Write("\n</tr>");
			output.Write("\n</table>");
		}
		#endregion

		#endregion
	}
}

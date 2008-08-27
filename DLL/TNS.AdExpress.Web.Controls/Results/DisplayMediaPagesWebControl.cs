#region Informations
// Auteur: D. Mussuma
// Date de création: 26/08/2008
// Date de modification: 
#endregion
using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

using TNS.AdExpress.Web.Core.Sessions;
using WebCst = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.Date;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Results {
		/// <summary>
		/// display all pages of a media
		/// </summary>
	[ToolboxData("<{0}:DisplayMediaPagesWebControl runat=server></{0}:DisplayMediaPagesWebControl>")]
	public class DisplayMediaPagesWebControl : WebControl {
		#region Variables
		/// <summary>
		/// Date parution
		/// </summary>
		protected string _dateParution;
		/// <summary>
		/// Date cover
		/// </summary>
		protected string _dateCover;
		/// <summary>
		/// Name media
		/// </summary>
		protected string _nameMedia;
		/// <summary>
		/// Nb pages
		/// </summary>
		protected string _nbrePages;
		/// <summary>
		/// Page anchor
		/// </summary>
		protected string _pageAnchor;
		/// <summary>
		/// Media Id
		/// </summary>
		protected Int64 _idMedia;
		/// <summary>
		/// Customer session
		/// </summary>
		protected WebSession _webSession;
		
		#endregion

		#region Acccessors
		/// <summary>
		/// client session
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public WebSession CustomerWebSession {
			get {
				return _webSession;
			}

			set {
				_webSession = value;
			}
		}
		/// <summary>
		/// Date parution
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string DateParution {
			get {
				return _dateParution;
			}

			set {
				_dateParution = value;
			}
		}
		/// <summary>
		/// Date cover
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string DateCover {
			get {
				return _dateCover;
			}

			set {
				_dateCover = value;
			}
		}
		/// <summary>
		/// Name media
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string NameMedia {
			get {
				return _nameMedia;
			}

			set {
				_nameMedia = value;
			}
		}
		/// <summary>
		/// Nb pages
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string NbPages {
			get {
				return _nbrePages;
			}

			set {
				_nbrePages = value;
			}
		}
		/// <summary>
		/// Page anchor
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[DefaultValue("")]
		[Localizable(true)]
		public string PageAnchor {
			get {
				return _pageAnchor;
			}

			set {
				_pageAnchor = value;
			}
		}
		/// <summary>
		/// Page anchor
		/// </summary>
		[Bindable(true)]
		[Category("Appearance")]
		[Localizable(true)]
		public long IdMedia {
			get {
				return _idMedia;
			}

			set {
				_idMedia = value;
			}
		}
		#endregion

		#region Onload
		/// <summary>
		/// OnLoad
		/// </summary>
		/// <param name="e">arguments</param>
		protected override void OnLoad(EventArgs e) {

			#region Scripts
			// Ouverture de la popup une création
			if (!Page.ClientScript.IsClientScriptBlockRegistered("portofolioOneCreation")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "portofolioOneCreation", TNS.AdExpress.Web.Functions.Script.PortofolioOneCreation());
			}
			// Positionnement de l'image avec Anchor
			if (!Page.ClientScript.IsClientScriptBlockRegistered("goToAnchorImage")) {
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "goToAnchorImage", TNS.AdExpress.Web.Functions.Script.GoToAnchorImage());
			}
			#endregion
		}
		#endregion
		/// <summary>
		/// Render
		/// </summary>
		/// <param name="output">output</param>
		protected override void Render(HtmlTextWriter output) {
			
			#region Chemin de fer popUp
		

			#region Variables
			string pathWeb = WebCst.CreationServerPathes.IMAGES + "/" + _idMedia + "/" + _dateCover + "/imagette/";
			string path = WebCst.CreationServerPathes.LOCAL_PATH_IMAGE + _idMedia + @"\" + _dateCover + @"\imagette";

			// Pour test en localhost :
			//string path = "\\\\localhost\\ImagesPresse\\" + _idMedia + "\\" + _dateCover + "\\imagette";
			

			string[] files = Directory.GetFiles(path, "*.jpg");
			string[] endFile;
			StringBuilder t = new StringBuilder(5000);
			int i = 1;
			int compteur = 0;
			string endBalise = "";
			string day;
			string[] filesName = new string[2];
			#endregion
			CultureInfo cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
			DateTime dayDT = new DateTime(int.Parse(_dateParution.Substring(0, 4)), int.Parse(_dateParution.Substring(4, 2)), int.Parse(_dateParution.ToString().Substring(6, 2)));
			day = DayString.GetCharacters(dayDT, cultureInfo) + " " + dayDT.ToString("dd/MM/yyyy");

			t.Append("<table border=1 class=\"violetBorder paleVioletBackGroundV2\" cellpadding=0 cellspacing=0 width=100% ><tr>");
			t.Append("<td class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + day + "</td>");
			t.Append("<td align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + _nameMedia + "</td>");
			t.Append("<td align=right class=\"portofolio1\" style=\"BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">" + GestionWeb.GetWebWord(1385, _webSession.SiteLanguage) + " : " + _nbrePages + "</td>");
			t.Append("</tr></table>");

			t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100% class=\"paleVioletBackGroundV2\">");
			foreach (string name in files) {

				endFile = name.Split('\\');
				// Couverture - Dos
				if (i == 1 || i == files.Length) {
					t.Append("<tr><td colspan=4 align=center>");
					t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%><tr><td align=center>");
					if (i == 1) t.Append("<a name=\"C1\"></a><a name=\"C2\"></a>");
					if (i == files.Length) t.Append("<a name=\"C3\"></a><a name=\"C4\"></a>");
					t.Append("<a href=\"javascript:portofolioOneCreation('" + _idMedia + "','" + _dateCover + "','" + endFile[endFile.Length - 1] + "','');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
					t.Append("</td></tr></table>");
					t.Append("</td></tr>");
				}
				else {
					if (compteur == 0) {
						t.Append("<tr>");
						endBalise = "";
					}
					else if (compteur == 3) {
						compteur = -1;
						endBalise = "</tr>";

					}
					else {
						endBalise = "";
					}

					t.Append("<td align=center style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
					// Tableau niveau 2
					if (compteur == 0 || compteur == 2) {
						t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%><tr><td style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
						filesName[0] = endFile[endFile.Length - 1];
						filesName[1] = files[i].Split('\\')[endFile.Length - 1];
					}
					// Tableau niveau 1
					t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td>");
					t.Append("<a name=\"#" + i.ToString() + "\" href=\"javascript:portofolioOneCreation('" + _idMedia + "','" + _dateCover + "','" + filesName[0] + "','" + filesName[1] + "');\"><img src='" + pathWeb + endFile[endFile.Length - 1] + "' border=\"0\"></a>");
					t.Append("</td></tr>");
					t.Append("</table>");

					if (compteur == 1 || compteur == -1) {
						t.Append("<tr ><td colspan=2 align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">Pages : " + ((int)(i - 1)).ToString() + "/" + i.ToString() + "</td></tr>");
						t.Append("</td></tr></table>");
					}

					t.Append("</td>");
					t.Append(endBalise);
					compteur++;
				}
				i++;
			}
			t.Append("</table>");

			// Script location
			t.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">");
			t.Append("\ndocument.location='#" + _pageAnchor + "';");
			t.Append("\n</script>");

		#endregion
			output.Write(t.ToString());
		}
	}
}

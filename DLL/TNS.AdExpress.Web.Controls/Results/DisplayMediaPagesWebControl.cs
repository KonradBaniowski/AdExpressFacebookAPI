#region Informations
// Auteur: D. Mussuma
// Date de création: 26/08/2008
// Date de modification: 
#endregion
using System;
using System.IO;
using System.ComponentModel;
using System.Globalization;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;

using TNS.AdExpress.Web.Core.Sessions;
using WebCst = TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.Date;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Utilities;

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
        /// <summary>
        /// Page anchor
        /// </summary>
        [Bindable(true)]             
		public string SubFolder { get; set; }
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

		    bool hasPressCopyright = Web.Functions.Rights.HasPressCopyright(_idMedia);

            string pathWeb = string.Format("{0}/{1}/{2}/{3}/{4}",
                WebCst.CreationServerPathes.IMAGES, _idMedia, _dateCover,
                (string.IsNullOrEmpty(SubFolder)) ? "imagette" : SubFolder,
                hasPressCopyright ? string.Empty : "blur/");
            string path = string.Format("{0}{1}\\{2}\\{3}{4}",
                WebCst.CreationServerPathes.LOCAL_PATH_IMAGE, _idMedia, _dateCover,
                (string.IsNullOrEmpty(SubFolder)) ? "imagette" : SubFolder,
                 hasPressCopyright ? string.Empty : "\\blur");

           
			string[] files = Directory.GetFiles(path, "*.jpg");
            Array.Sort(files);

		    var t = new StringBuilder(5000);
			int i = 1;
			int compteur = 0;
		    var filesName = new string[2];
			#endregion
			var cultureInfo = new CultureInfo(WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].Localization);
			var dayDT = new DateTime(int.Parse(_dateParution.Substring(0, 4)),
                int.Parse(_dateParution.Substring(4, 2)), int.Parse(_dateParution.ToString().Substring(6, 2)));
			string day = string.Format("{0} {1}", DayString.GetCharacters(dayDT, cultureInfo),
                Dates.DateToString(dayDT, _webSession.SiteLanguage));

			t.Append("<table border=1 class=\"violetBorder paleVioletBackGroundV2\" cellpadding=0 cellspacing=0 width=100% ><tr>");
			t.AppendFormat("<td class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-BOTTOM-STYLE: none\">{0}</td>", day);
			t.AppendFormat("<td align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">{0}</td>"
                , _nameMedia);
			t.AppendFormat("<td align=right class=\"portofolio1\" style=\"BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">{0} : {1}</td>"
                , GestionWeb.GetWebWord(1385, _webSession.SiteLanguage), _nbrePages);
			t.Append("</tr></table>");

            t.Append("<table border=1 class=\"violetBorder paleVioletBackGroundV2\" cellpadding=2 cellspacing=2 width=100% ><tr>");

            t.AppendFormat("<td align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none;\">{0}</td>"
                , GestionWeb.GetWebWord(3015, _webSession.SiteLanguage));
            
            t.Append("</tr></table>");

			t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100% class=\"paleVioletBackGroundV2\">");
			foreach (string name in files) {

				string[] endFile = name.Split('\\');
				// Couverture - Dos
				if (i == 1 || i == files.Length) {
					t.Append("<tr><td colspan=4 align=center>");
					t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%><tr><td align=center>");
					if (i == 1) t.Append("<a name=\"C1\"></a><a name=\"C2\"></a>");
					if (i == files.Length) t.Append("<a name=\"C3\"></a><a name=\"C4\"></a>");
                    if (string.IsNullOrEmpty(SubFolder)) 
					t.AppendFormat("<a href=\"javascript:portofolioOneCreation('{0}','{1}','{2}','', true);\"><img src='{3}{2}' border=\"0\"></a>"
                        , _idMedia, _dateCover, endFile[endFile.Length - 1], pathWeb);
                    else t.AppendFormat("<a href=\"javascript:portofolioOneCreation2('{0}','{1}','{2}','', true,'{4}');\"><img src='{3}{2}' border=\"0\"></a>"
                        , _idMedia, _dateCover, endFile[endFile.Length - 1], pathWeb, SubFolder);
					t.Append("</td></tr></table>");
					t.Append("</td></tr>");
				}
				else {
				    string endBalise;
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
						t.Append("<table border=1 class=\"violetBorder\" cellpadding=0 cellspacing=0 width=100%>");
					    t.Append("<tr><td style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
						filesName[0] = endFile[endFile.Length - 1];
						filesName[1] = files[i].Split('\\')[endFile.Length - 1];
					}
					// Tableau niveau 1
					t.Append("<table border=0 cellpadding=0 cellspacing=0 width=100%><tr><td>");
                    if (string.IsNullOrEmpty(SubFolder)) 
                    t.AppendFormat("<a name=\"#{0}\" href=\"javascript:portofolioOneCreation('{1}','{2}','{3}','{4}', true);\"><img src='{5}{6}' border=\"0\"></a>"
                        , i.ToString(), _idMedia, _dateCover, filesName[0], filesName[1], pathWeb, endFile[endFile.Length - 1]);
                    else t.AppendFormat("<a name=\"#{0}\" href=\"javascript:portofolioOneCreation2('{1}','{2}','{3}','{4}', true,'{7}');\"><img src='{5}{6}' border=\"0\"></a>"
                       , i.ToString(), _idMedia, _dateCover, filesName[0], filesName[1], pathWeb, endFile[endFile.Length - 1],SubFolder);
					t.Append("</td></tr>");
					t.Append("</table>");

					if (compteur == 1 || compteur == -1)
					{
					    t.Append(
					        "<tr ><td colspan=2 align=center class=\"portofolio1\" style=\"BORDER-RIGHT-STYLE: none;BORDER-LEFT-STYLE: none;BORDER-BOTTOM-STYLE: none\">");
						t.AppendFormat("Pages : {0}/{1}</td></tr>"
                            , ((int)(i - 1)).ToString(), i.ToString());
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
			t.AppendFormat("\ndocument.location='#{0}';", _pageAnchor);
			t.Append("\n</script>");

		#endregion
			output.Write(t.ToString());
		}
	}
}

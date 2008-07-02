using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Composant affichant le titre  et le descriptif d'une page indépendante d'un module
	/// </summary>
	[ToolboxData("<{0}:PageTitleWebControl runat=server></{0}:PageTitleWebControl>")]
	public class PageTitleWebControl : System.Web.UI.WebControls.WebControl {

		#region Propriétés
		/// <summary>
		/// Code titre de la page
		/// </summary>
		[Bindable(true),
		Description("Code de traduction du champ titre")]
		protected int codeTitle;
		/// <summary></summary>
		public int CodeTitle{
			get{return codeTitle;}
			set{codeTitle=value;}
		}

		/// <summary>
		/// Code descriptif de la page
		/// </summary>
		[Bindable(true),
		Description("Code de traduction du champ descriptif")]
		protected int codeDescription;
		/// <summary></summary>
		public int CodeDescription{
			get{return codeDescription;}
			set{codeDescription=value;}
		}

		/// <summary>
		/// Code descriptif de la page
		/// </summary>
		[Bindable(true),
		Description("Langage du site")]
		protected int siteLang;
		/// <summary></summary>
		public int Language{
			get{return siteLang;}
			set{siteLang=value;}
		}

		/// <summary>
		/// Session du client (utile pour la langue)
		/// </summary>
		protected WebSession customerWebSession = null;
		/// <summary></summary>
		public WebSession CustomerWebSession{
			get{return customerWebSession;}
			set{customerWebSession=value;}
		}
		#endregion

		#region Evenements
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {

            string themeName = string.Empty;

			if (customerWebSession!=null){
				siteLang = customerWebSession.SiteLanguage;
			}

            themeName = WebApplicationParameters.Themes[siteLang].Name;

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"whiteBackGround\">");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			//debut tableau titre
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
            output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/App_Themes/" + themeName + "/Images/Common/block_fleche.gif\" width=\"12\"></td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" class=\"blockBackGround\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/Images/Common/pixel.gif\" width=\"13\"></td>");
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(codeTitle,siteLang)+"</td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td></td>");
            output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n</table>");
			//fin tableau titre
			output.Write("\n</td>");
			output.Write("\n</tr>");
			//Descriptif
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
			output.Write("\n<td class=\"txtGris11Bold\">");
			output.Write(GestionWeb.GetWebWord(codeDescription,siteLang));
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n</table>");
		}
		#endregion
	}
}
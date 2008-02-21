using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Navigation;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Composant affichant le titre  et le descriptif de la page
	/// </summary>
	[ToolboxData("<{0}:ModuleTitleWebControl runat=server></{0}:ModuleTitleWebControl>")]
	public class ModuleTitleWebControl : System.Web.UI.WebControls.WebControl {

		#region Propriétés
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
            string moduleTitle="Nom du module";
            string description = "Description du module";
            if(customerWebSession!=null){
                moduleTitle=GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(customerWebSession.CurrentModule),customerWebSession.SiteLanguage);
                description=GestionWeb.GetWebWord(codeDescription, customerWebSession.SiteLanguage);
            }

		    output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" bgcolor=\"#FFFFFF\">");
		    output.Write("\n<tr>");
		    output.Write("\n<td>");
		    //debut tableau titre
		    output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
		    output.Write("\n<TR>");
		    output.Write("\n<TD height=\"5\"></TD>");
		    output.Write("\n</TR>");
		    output.Write("\n<tr>");
		    output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/Images/Common/pixel.gif\"></td>");
		    output.Write("\n</tr>");
		    output.Write("\n<tr>");
		    output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\"/Images/Common/block_fleche.gif\" width=\"12\"></td>");
		    output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"/Images/Common/block_dupli.gif\"><IMG height=\"1\" src=\"/Images/Common/pixel.gif\" width=\"13\"></td>");
		    output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"
			    +moduleTitle
			    +"</td>");
		    output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/Images/pixel.gif\" width=\"1\"></td>");
		    output.Write("\n</tr>");
		    output.Write("\n<tr>");
		    output.Write("\n<td></td>");
		    output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/images/Common/pixel.gif\"></td>");
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
            output.Write(description);
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
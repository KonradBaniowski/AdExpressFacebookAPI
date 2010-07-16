using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;

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

        /// <summary>
        /// Block_Fleche image path
        /// </summary>
        protected string blockFlechePath = string.Empty;
        /// <summary>
        /// Set or Get block_fleche image path
        /// </summary>
        public string BlockFlechePath {
            get { return blockFlechePath; }
            set { blockFlechePath = value; }
        }

        /// <summary>
        /// Block_dupli image path
        /// </summary>
        protected string blockDupliPath = string.Empty;
        /// <summary>
        /// Set or Get block_dupli image path
        /// </summary>
        public string BlockDupliPath {
            get { return blockDupliPath; }
            set { blockDupliPath = value; }
        }

        /// <summary>
        /// Title style
        /// </summary>
        protected string titleUppercaseCss = string.Empty;
        /// <summary>
        /// Set or Get Title style
        /// </summary>
        public string TitleUppercaseCss {
            get { return titleUppercaseCss; }
            set { titleUppercaseCss = value; }
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
            string themeName = WebApplicationParameters.Themes[customerWebSession.SiteLanguage].Name;
            if(customerWebSession!=null){
                moduleTitle=GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(customerWebSession.CurrentModule),customerWebSession.SiteLanguage);
                description=GestionWeb.GetWebWord(codeDescription, customerWebSession.SiteLanguage);
            }

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"backGroundModuleTitleBorder\">");
		    output.Write("\n<tr>");
		    output.Write("\n<td>");
		    //debut tableau titre
            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
		    output.Write("\n<TR>");
		    output.Write("\n<TD height=\"5\"></TD>");
		    output.Write("\n</TR>");
		    output.Write("\n<tr>");
		    output.Write("\n<td class=\"headerLeft\" colSpan=\"4\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\"></td>");
		    output.Write("\n</tr>");
            output.Write("\n<tr>");
		    output.Write("\n<td style=\"HEIGHT: 14px\" vAlign=\"top\"><IMG height=\"12\" src=\""+blockFlechePath+"\" width=\"12\"></td>");
		    output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\""+blockDupliPath+"\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\" width=\"13\"></td>");
            output.Write("\n<td class=\"txtNoir11Bold backGroundModuleTitle " + titleUppercaseCss + "\" width=\"100%\">"
			    +moduleTitle
			    +"</td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\" width=\"1\"></td>");
		    output.Write("\n</tr>");
		    output.Write("\n<tr>");
		    output.Write("\n<td></td>");
		    output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/images/Common/pixel.gif\"></td>");
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
            output.Write("\n<td class=\"txtBlanc11Bold\">");
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
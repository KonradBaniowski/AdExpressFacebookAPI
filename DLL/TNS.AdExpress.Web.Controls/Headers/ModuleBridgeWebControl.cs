#region Informations
// Auteur: G. Facon 
// Date de création: 17/05/2004 
// Date de modification: 17/05/2004
//	G. Facon 01/08/2006 Gestion de l'accès au information de la page de résultat 
#endregion


using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.Controls.Headers{
	/// <summary>
	/// Contrôle qui permet de passer d'un module à l'autre
	/// </summary>
	[DefaultProperty("Text"),ToolboxData("<{0}:ModuleBridgeWebControl runat=server></{0}:ModuleBridgeWebControl>")]
	public class ModuleBridgeWebControl : System.Web.UI.WebControls.WebControl{

		#region Variables
		/// <summary>
		/// Session du Client
		/// </summary>
		protected WebSession webSession=null;
		#endregion

		#region Accesseurs

		/// <summary>
		/// Définit la session à utiliser
		/// </summary>
		[Bindable(true),Category("Appearance"),DefaultValue(null)]
		public virtual WebSession CustomerWebSession {
			set{webSession = value;}
		}

		#endregion

        #region Properties
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

        #region Evènements

        #region Rendu
        /// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){

            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;

            output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\" class=\"whiteBackGround\">");
			output.Write("\n<tr>");
			output.Write("\n<td>");
			//debut tableau titre
			output.Write("\n<table cellSpacing=\"0\" cellPadding=\"0\" width=\"100%\" border=\"0\">");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n<tr>");
            output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
            output.Write("\n<td style=\"HEIGHT: 14px\" width=\"1%\" background=\"" + blockDupliPath + "\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/Images/Common/pixel.gif\" width=\"13\"></td>");
            output.Write("\n<td class=\"txtNoir11Bold backGroundModuleTitle " + titleUppercaseCss + "\" width=\"100%\">" + GestionWeb.GetWebWord(794, webSession.SiteLanguage) + "</td>");
            output.Write("\n<td style=\"HEIGHT: 14px\" class=\"headerLeft\"><IMG height=\"1\" src=\"/App_Themes/" + themeName + "/images/Common/pixel.gif\" width=\"1\"></td>");
			output.Write("\n</tr>");
			output.Write("\n<tr>");
			output.Write("\n<td class=\"headerLeft\" colSpan=\"3\"><IMG height=\"1\" src=\"/App_Themes/"+themeName+"/images/Common/pixel.gif\"></td>");
			output.Write("\n</tr>");
			output.Write("\n</table>");
			//fin tableau titre
			output.Write("\n</td>");
			output.Write("\n</tr>");
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");

            Module currentModule=webSession.CustomerLogin.GetModule(webSession.CurrentModule);
            foreach(Int64 current in currentModule.Bridges) {
				if (webSession.CustomerLogin.GetModule(current)!=null){ 
					output.Write("\n<tr>");
					output.Write("\n<td><A class=\"roll03\" href=\"" +((ResultPageInformation)(webSession.CustomerLogin.GetModule(current)).GetResultPageInformation(0)).Url + "?idSession=" + webSession.IdSession + "\">&gt; "+GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(current),webSession.SiteLanguage)+"</td>");
					output.Write("\n</td>");
					output.Write("\n</tr>");
				}
			}
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("</table>");
			
		}
		#endregion

		#endregion
	}
}

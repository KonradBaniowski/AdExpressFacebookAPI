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
	
		#region Evènements

		#region Rendu
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
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
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(794,webSession.SiteLanguage)+"</td>");
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
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");

			//Exemple, a changer ultérieurement
			webSession.CustomerLogin.ModuleList();
			foreach(Int64 current in ((Module)webSession.CustomerLogin.HtModulesList[webSession.CurrentModule]).Bridges){
				if (webSession.CustomerLogin.HtModulesList.ContainsKey(current)){ 
					output.Write("\n<tr>");
					output.Write("\n<td><A class=\"roll03\" href=\"" +((ResultPageInformation)((Module)webSession.CustomerLogin.HtModulesList[current]).GetResultPageInformation(0)).Url + "?idSession=" + webSession.IdSession + "\">&gt; "+GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(current),webSession.SiteLanguage)+"</td>");
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

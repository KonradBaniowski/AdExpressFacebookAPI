#region Informations
// Auteur: G. Facon
// Date de création: 16/08/2004
// Date de modification: 16/08/2004
#endregion

using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Translation;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// Description résumée de HelpWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:HelpWebControl runat=server></{0}:HelpWebControl>")]
	public class HelpWebControl : System.Web.UI.WebControls.WebControl{
		
		#region Constantes
		/// <summary>
		/// Chemin d'accès à l'icône d'aide
		/// </summary>
		protected const string HELP_BUTTON_PATH="/Images/Common/button/";
		/// <summary>
		/// Nom de base de l'icône
		/// </summary>
		protected const string HELP_BUTTON_NAME="help";
		/// <summary>
		/// Largeur de la fenêtre cible
		/// </summary>
		protected const string TARGET_PAGE_WIDTH="710";
		/// <summary>
		/// Hauteur de la fenêtre cible
		/// </summary>
		protected const string TARGET_PAGE_HEIGHT="700";
		#endregion


		#region Variables
		/// <summary>
		/// Url cible de l'aide
		/// </summary>
		protected string url="#";
		/// <summary>
		/// Langue des textes du composant
		/// </summary>
		protected int language=33;
		#endregion

		#region Accesseurs
		/// <summary>
		/// Obtient ou définit l'url cible de l'aide 
		/// </summary>
		[Bindable(true),
		Description("Url cible de l'aide"),
		DefaultValue("#")]
		public string Url{
			get{return url;}
			set{url=value;}
		}

		/// <summary>
		/// Obtient ou définit la langue des textes du composant
		/// </summary>
		[Bindable(true),
		Description("Url cible de l'aide"),
		DefaultValue(33)]
		public int  Language{
			get{return language;}
			set{language=value;}
		}
		#endregion

		#region Evenements

		/// <summary>
		/// Prérendu du composant
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(!Page.ClientScript.IsClientScriptBlockRegistered("ScriptRecallpopup")){
				string script="<script language=\"JavaScript\"> ";
				script+="function popupRecallOpen(page,width,height){";
				script+="	window.open(page,'','width='+width+',height='+height+',toolbar=no,scrollbars=yes,resizable=no');";
				script+="}";
				script+="</script>";
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"ScriptRecallpopup",script);
			}
		}

		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output) {
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
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(992,language)+"</td>");
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
			// Affichage de l'icône
			output.Write("\n<table cellspacing=1 cellpading=0 border=0 bgcolor=\"#FFFFFF\">");
			output.Write("\n<tr>");
			output.Write("\n<td><img src=\"/Images/Common/pixel.gif\" width=\"7px\" height=\"1px\"></td>");
			output.Write("\n<td><a class=\"roll03\" href=\"javascript:popupRecallOpen('"+url+"?siteLanguage="+ language +"','"+TARGET_PAGE_WIDTH+"','"+TARGET_PAGE_HEIGHT+"');\"  onmouseover=\""+HELP_BUTTON_NAME+"Button.src='"+HELP_BUTTON_PATH+HELP_BUTTON_NAME+"_down.gif';\" onmouseout=\""+HELP_BUTTON_NAME+"Button.src ='"+HELP_BUTTON_PATH+HELP_BUTTON_NAME+"_up.gif';\"><img name="+HELP_BUTTON_NAME+"Button border=0 src=\""+HELP_BUTTON_PATH+HELP_BUTTON_NAME+"_up.gif\" alt=\""+ GestionWeb.GetWebWord(992,language) +"\"></a>");		
			output.Write("\n</tr>");
			output.Write("\n</table>");
			output.Write("\n</td>");
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
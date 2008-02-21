using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;


using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Headers
{
	/// <summary>
	/// COmposant gérant la possibilité de charger ou sauvegarder un univers.
	/// </summary>
	[ToolboxData("<{0}:SearchSaveWebControl runat=server></{0}:SearchSaveWebControl>")]
	public class SearchSaveWebControl : System.Web.UI.WebControls.WebControl
	{

		#region Propriétés
		/// <summary>
		/// Possibilité de sauvegarder l'univers
		/// </summary>
		protected bool save;
		/// <summary></summary>
		[Bindable(true),
		Description("Option sauvergarder l'univers active")]
		public bool SaveUnivers{
			get{return save;}
			set{save=value;}
		}
		/// <summary>
		/// Possibilité de charger un univers
		/// </summary>
		[Bindable(true),
		Description("Option charger un univers active")]
		protected bool load;
		/// <summary></summary>
		public bool LoadUnivers{
			get{return load;}
			set{load=value;}
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
		/// PréRendu
		/// </summary>
		/// <param name="e">Arguments</param>
		protected override void OnPreRender(EventArgs e) {
			if(!Page.ClientScript.IsClientScriptBlockRegistered("Scriptpopup")){
				Page.ClientScript.RegisterClientScriptBlock(this.GetType(),"Scriptpopup",TNS.AdExpress.Web.Functions.Script.Popup());			
			}		
		}


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
			output.Write("\n<td class=\"txtNoir11Bold\" style=\"PADDING-RIGHT: 5px; PADDING-LEFT: 5px; TEXT-TRANSFORM: uppercase; HEIGHT: 14px\" width=\"100%\">"+GestionWeb.GetWebWord(771,customerWebSession.SiteLanguage)+"</td>");
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
			//Option sauvegarde de l univers
			if (save){
				output.Write("\n<tr>");
				output.Write("\n<td>");
			//	output.Write("\n<a  class=\"roll03\" href=\"javascript:popupOpen('/Private/Universe/UniverseSavePopUp.aspx?idSession="+ customerWebSession.IdSession +"','470','210');\">&gt; "+ GestionWeb.GetWebWord(769,customerWebSession.SiteLanguage) +"</a>");
				output.Write("\n<a  class=\"roll03\" href=\"javascript:__doPostBack('"+this.ID+"','"+TNS.AdExpress.Constantes.Web.Universe.Type.saveUniverse+"')\">&gt; "+ GestionWeb.GetWebWord(769,customerWebSession.SiteLanguage) +"</a>");
	
				output.Write("\n</td>");
				output.Write("\n</tr>");
			}
			//Option chargement d'un univers
			if(load){
				output.Write("\n<tr>");
				output.Write("\n<td>");
				output.Write("\n<a href=\"javascript:__doPostBack('"+this.ID+"','"+TNS.AdExpress.Constantes.Web.Universe.Type.loadUniverse+"')\" class=\"roll03\">&gt; "+ GestionWeb.GetWebWord(770,customerWebSession.SiteLanguage) +"</a>");
				output.Write("\n</td>");
				output.Write("\n</tr>");
			}
			output.Write("\n<TR>");
			output.Write("\n<TD height=\"5\"></TD>");
			output.Write("\n</TR>");
			output.Write("\n</table>");
		}
		#endregion
	}
}

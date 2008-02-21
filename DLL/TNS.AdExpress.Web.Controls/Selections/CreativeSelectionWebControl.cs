#region Informations
// Auteur: G. Facon
// Date de création: 26/10/2005
// Date de modification:
#endregion

using System;
using System.Windows.Forms;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Translation;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Description résumée de CreativeSelectionWebControl.
	/// </summary>
	[DefaultProperty("Text"), 
		ToolboxData("<{0}:CreativeSelectionWebControl runat=server></{0}:CreativeSelectionWebControl>")]
	public class CreativeSelectionWebControl : System.Web.UI.WebControls.WebControl{

		#region Constantes
		/// <summary>
		/// Lien vers la page Excel
		/// </summary>
		private const string XLS_LINK="/Private/results/Excel/CreativeMediaPlanResults.aspx";
		#endregion

		#region Variables
		/// <summary>
		/// Session de l'utilisateur
		/// </summary>
		WebSession _webSession=null;
		#endregion
	
		#region Accesseur
		/// <summary>
		/// Définit la Session Client
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("")] 
		public WebSession CustomerSession {
			set{_webSession=value;}
		}
		#endregion

		#region Evènement

		#region Render
		/// <summary> 
		/// Génère ce contrôle dans le paramètre de sortie spécifié.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		protected override void Render(HtmlTextWriter output){
			string dateBegin,dateEnd;
			dateBegin=DateString.dateTimeToDD_MM_YYYY((new AtomicPeriodWeek(int.Parse(_webSession.PeriodBeginningDate.Substring(0,4)),int.Parse(_webSession.PeriodBeginningDate.Substring(4,2)))).FirstDay,_webSession.SiteLanguage);
			dateEnd=DateString.dateTimeToDD_MM_YYYY((new AtomicPeriodWeek(int.Parse(_webSession.PeriodEndDate.Substring(0,4)),int.Parse(_webSession.PeriodEndDate.Substring(4,2)))).LastDay,_webSession.SiteLanguage);

			output.Write("<table border=0 cellspacing=1 cellpadding=0 bgcolor=#999999 class=\"TexteTitreRappelScanpub\">");
			// Titre
			output.Write("<tr>");
			output.Write("<td align=center bgcolor=#644883>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1791,_webSession.SiteLanguage)+"&nbsp;&nbsp;</td>");
			output.Write("</tr>");
			output.Write("<tr>");
			output.Write("<td>");
			output.Write("<table border=0 cellspacing=0 cellpadding=0 bgcolor=#ffffff width=100% >");
			// Media
			output.Write("<tr valign=top>");
			output.Write("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1792,_webSession.SiteLanguage)+"</td>");
			output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
			output.Write("<td class=Arial7gris>");
			foreach(System.Windows.Forms.TreeNode currentNode in _webSession.SelectionUniversMedia.Nodes){
				output.Write(((LevelInformation)currentNode.Tag).Text+"&nbsp;&nbsp;<br>");	
			}
			output.Write("</td>");
			output.Write("</tr>");
			//Produit
			AddSpace(output);
			output.Write("<tr valign=top>");
			output.Write("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(858,_webSession.SiteLanguage)+"</td>");
			output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
			output.Write("<td class=Arial7gris>");
			foreach(System.Windows.Forms.TreeNode currentNode in _webSession.SelectionUniversAdvertiser.Nodes){
				output.Write(((LevelInformation)currentNode.Tag).Text+"&nbsp;&nbsp;<br>");	
			}
			output.Write("</td>");
			output.Write("</tr>");
			//Date début
			AddSpace(output);
			output.Write("<tr>");
			output.Write("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1793,_webSession.SiteLanguage)+"</td>");
			output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
			output.Write("<td class=Arial7gris>"+dateBegin+"&nbsp;</td>");
			output.Write("</tr>");
			// Date fin
			AddSpace(output);
			output.Write("<tr>");
			output.Write("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1794,_webSession.SiteLanguage)+"</td>");
			output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
			output.Write("<td class=Arial7gris>"+dateEnd+"&nbsp;</td>");
			output.Write("</tr>");
			//Unité
			AddSpace(output);
			output.Write("<tr>");
			output.Write("<td class=Arial7rouge>&nbsp;&nbsp;"+GestionWeb.GetWebWord(1795,_webSession.SiteLanguage)+"</td>");
			output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
			output.Write("<td class=Arial7gris>Euro&nbsp;&nbsp;</td>");
			output.Write("</tr>");
			// Lien vers XLS
			output.Write("<tr height=40 valign=middle>");
			output.Write("<td colspan=3 align=center>");
			output.Write("<a class=\"roll02\" href=\""+XLS_LINK+"?idSession="+_webSession.IdSession+"\">"+GestionWeb.GetWebWord(1796,_webSession.SiteLanguage)+"</a>");
			output.Write("</td>");
			output.Write("</tr>");
			output.Write("</table>");
			output.Write("</td>");
			output.Write("</tr>");
			output.Write("</table>");
		}
		#endregion

		/// <summary>
		/// Ajout espace
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel écrire </param>
		private void AddSpace(HtmlTextWriter output){
			output.Write("<tr height=10px>");
			output.Write("<td colspan=3></td>");
			output.Write("</tr>");
		}

		#endregion
	}
}

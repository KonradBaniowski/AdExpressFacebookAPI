#region Informations
// Auteur: G. Facon
// Date de cr�ation: 26/10/2005
// Date de modification:
#endregion

using System;
using System.Windows.Forms;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Collections.Generic;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Date;
using ProductClassification=TNS.AdExpress.DataAccess.Classification.ProductBranch;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Controls.Selections{
	/// <summary>
	/// Description r�sum�e de CreativeSelectionWebControl.
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
		/// D�finit la Session Client
		/// </summary>
		[Bindable(true), 
			Category("Appearance"), 
			DefaultValue("")] 
		public WebSession CustomerSession {
			set{_webSession=value;}
		}
		#endregion

		#region Ev�nement

		#region Render
		/// <summary> 
		/// G�n�re ce contr�le dans le param�tre de sortie sp�cifi�.
		/// </summary>
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		protected override void Render(HtmlTextWriter output){
			string dateBegin,dateEnd;

            dateBegin = DateString.dateTimeToDD_MM_YYYY((new DateTime(int.Parse(_webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.PeriodBeginningDate.Substring(4, 2)), int.Parse(_webSession.PeriodBeginningDate.Substring(6, 2)))), _webSession.SiteLanguage);
            dateEnd = DateString.dateTimeToDD_MM_YYYY((new DateTime(int.Parse(_webSession.PeriodEndDate.Substring(0, 4)), int.Parse(_webSession.PeriodEndDate.Substring(4, 2)), int.Parse(_webSession.PeriodEndDate.Substring(6, 2)))), _webSession.SiteLanguage);

			output.Write("<table border=0 cellspacing=1 cellpadding=0 class=\"TexteTitreRappelScanpub\">");
			// Titre
			output.Write("<tr>");
            output.Write("<td align=center class=\"RappelBGColor\">&nbsp;&nbsp;" + GestionWeb.GetWebWord(1791, _webSession.SiteLanguage) + "&nbsp;&nbsp;</td>");
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

            TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = _webSession.PrincipalProductUniverses[0];
            List<NomenclatureElementsGroup> groups = adExpressUniverse.GetIncludes();
            string productLevelIdsList = "";
            ProductClassification.PartialProductLevelListDataAccess productLabels;
            List<long> levelIdsList;

            if (groups != null && groups.Count > 0) {
                for (int i = 0; i < groups.Count; i++) {
                    productLevelIdsList = groups[0].GetAsString(TNSClassificationLevels.PRODUCT);
                    levelIdsList = groups[0].Get(TNSClassificationLevels.PRODUCT);
                    productLabels = new ProductClassification.PartialProductLevelListDataAccess(productLevelIdsList, _webSession.DataLanguage, _webSession.Source);
                    foreach (long id in levelIdsList) {
                        output.Write(productLabels[id] + "&nbsp;&nbsp;<br>");	
                    }
                }
            }
            
			output.Write("</td>");
			output.Write("</tr>");

            //Version
            if (_webSession.IdSlogans != null && _webSession.IdSlogans.Count > 0) {
                AddSpace(output);
                output.Write("<tr valign=top>");
                output.Write("<td class=Arial7rouge>&nbsp;&nbsp;" + GestionWeb.GetWebWord(1888, _webSession.SiteLanguage) + "</td>");
                output.Write("<td class=Arial7rouge>&nbsp;:&nbsp;</td>");
                output.Write("<td class=Arial7gris>");

                foreach (long id in _webSession.IdSlogans) {
                    output.Write(((LevelInformation)_webSession.SelectionUniversMedia.Nodes[0].Tag).ID + "" + id + "&nbsp;&nbsp;<br>");
                }

                output.Write("</td>");
                output.Write("</tr>");
            }

			//Date d�but
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
			//Unit�
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
		/// <param name="output"> Le writer HTML vers lequel �crire </param>
		private void AddSpace(HtmlTextWriter output){
			output.Write("<tr height=10px>");
			output.Write("<td colspan=3></td>");
			output.Write("</tr>");
		}

		#endregion
	}
}

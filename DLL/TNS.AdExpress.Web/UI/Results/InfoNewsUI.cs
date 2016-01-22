#region Informations
// Auteur: B. Masson
// Date de création: 16/11/2004 
// Date de modification: 19/11/2004 
#endregion

using System;
using System.Data;
using System.Web.UI;
using System.Collections;

using TNS.AdExpress.Web.DataAccess.MyAdExpress;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using ClassificationConstantes=TNS.AdExpress.Constantes.Classification;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using AdExpressWebRules=TNS.AdExpress.Web.Rules;
using BFWebResult = TNS.AdExpress.Web.BusinessFacade.Results;
using WebCommon = TNS.AdExpress.Web.Common;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Classe utilisé pour l'affichage des plaquettes info/news
	/// </summary>
	public class InfoNewsUI:WebPage{

		#region Méthodes pour la sortie HTML
		/// <summary>
		/// Fonction pour la construction du code html pour l'affichage des plaquettes infos/news
		/// </summary>
		/// <returns>Source html du résultat</returns>
        public static string GetHtml(Page page, ArrayList list) {

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
			int compteur=0;
			int start=1;
			#endregion

			#region Script
			// Ouverture/fermeture des fenêtres pères
			if (!page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
				//page.ClientScript.RegisterClientScriptBlock(this.GetType(),"DivDisplayer",TNS.AdExpress.Web.Functions.Script.DivDisplayer());
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
			}
			#endregion

			if (list!=null && list.Count>0){
				foreach (WebCommon.Results.InfoNewsItem currentName in list){
					if (start==1){
                        t.Append("\n<table class=\"violetBorder txtViolet11Bold whiteBackGround\" cellpadding=0 cellspacing=0 width=\"650\">");
						t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
						t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                        t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + page.Theme + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
						t.Append("\n</tr></table>");
						start=0;
					}
					else if (start==0){
                        t.Append("\n<table class=\"violetBorderWithoutTop txtViolet11Bold whiteBackGround\"  cellpadding=0 cellspacing=0 width=\"650\">");
						t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
						t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                        t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + page.Theme + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
						t.Append("\n</tr></table>");
					}

					t.Append("<div id=\""+currentName.Name+"Content\" style=\"MARGIN-LEFT: 0px; DISPLAY: none;\" >");
                    t.Append("\n<table id=\"" + currentName.Name + "\" class=\"violetBorderWithoutTop txtViolet10 paleVioletBackGround\" width=\"650\">");
					compteur=0;
					for(int i=0; i<currentName.List.GetLength(1) && currentName.List[0,i]!=null; i++){
						if(compteur==0){
                            t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + page.Theme + "/Images/Common/logoPDF.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
							compteur=1;
						}
						else{
                            t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + page.Theme + "/Images/Common/logoPDF.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
							compteur=0;
						}
					}
					if(compteur==1){
						t.Append("\n<td>&nbsp;</td></tr>");
					}
					t.Append("\n</table>");
					t.Append("</div>");
				}
			}
			return (t.ToString());
		}
		#endregion
	}
}

#region Informations
// Auteur: A.DADOUCH
// Date de création: 23/08/2005
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
	/// Classe utilisé pour l'affichage des plaquettes Fichiers resultats
	/// </summary>
	public class FilesItemUI {

        #region Méthodes pour la sortie HTML
        /// <summary>
		/// Fonction pour la construction du code html pour l'affichage des plaquettes Fichiers resultats
		/// </summary>
		/// <returns>Source html du résultat</returns>
		public static string GetHtml(Page page, ArrayList list){

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            string themeName = page.Theme;
			int compteur=0;
			int start=1;
			#endregion

			#region Script
			// Ouverture/fermeture des fenêtres pères
			if (!page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
			}
			#endregion

			if (list!=null && list.Count>0)	{
				
				foreach (WebCommon.Results.FilesItem currentName in list){
				
					if (currentName!=null){
						if (start==1){
                            t.Append("\n<table class=\"txtViolet11Bold violetBorder backGroundWhite\"  cellpadding=0 cellspacing=0 width=\"650\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                            t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
							start=0;
						}
						else if (start==0){
                            t.Append("\n<table class=\"txtViolet11Bold violetBorderWithoutTop backGroundWhite\"  cellpadding=0 cellspacing=0 width=\"650\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                            t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
						}

                        t.Append("<div id=\"" + currentName.Name + "Content\" style=\"MARGIN-LEFT: 0px; DISPLAY: none;\" class=\"BlancNoBorderColor\" >");
                        t.Append("\n<table id=\"" + currentName.Name + "\" class=\"txtViolet10 lightPurple violetBorderWithoutTop\" width=\"650\">");
						compteur=0;
						for(int i=0; i<currentName.List.GetLength(1) && currentName.List[0,i]!=null; i++){
							if(compteur==0){ 
								switch(currentName.Type){
									
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmInsertionDetail:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/LogoText.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel:
									case TNS.AdExpress.Anubis.Constantes.Result.type.amset:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.tefnout:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/LogoExcel.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;	
									case TNS.AdExpress.Anubis.Constantes.Result.type.appm:
									case TNS.AdExpress.Anubis.Constantes.Result.type.hotep:
									case TNS.AdExpress.Anubis.Constantes.Result.type.miysis:
									case TNS.AdExpress.Anubis.Constantes.Result.type.mnevis:
									case TNS.AdExpress.Anubis.Constantes.Result.type.shou:
									case TNS.AdExpress.Anubis.Constantes.Result.type.aton:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.selket:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.thoueris:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/logoPDF.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;
								}	
								compteur=1;
							}
							else{
								switch(currentName.Type){ 
									
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmInsertionDetail:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/LogoText.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;
									case TNS.AdExpress.Anubis.Constantes.Result.type.appmExcel:
									case TNS.AdExpress.Anubis.Constantes.Result.type.amset:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.tefnout:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/LogoExcel.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;	
									case TNS.AdExpress.Anubis.Constantes.Result.type.appm:								
									case TNS.AdExpress.Anubis.Constantes.Result.type.hotep:
									case TNS.AdExpress.Anubis.Constantes.Result.type.miysis:
									case TNS.AdExpress.Anubis.Constantes.Result.type.mnevis:
									case TNS.AdExpress.Anubis.Constantes.Result.type.shou:
									case TNS.AdExpress.Anubis.Constantes.Result.type.aton:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.selket:
                                    case TNS.AdExpress.Anubis.Constantes.Result.type.thoueris:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/logoPDF.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;
								}
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
			}

			return (t.ToString());
		}
		#endregion
	}
}

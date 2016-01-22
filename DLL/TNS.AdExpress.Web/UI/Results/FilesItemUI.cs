#region Informations
// Auteur: A.DADOUCH
// Date de cr�ation: 23/08/2005
#endregion

using System.Web.UI;
using System.Collections;
using WebCommon = TNS.AdExpress.Web.Common;
using TNS.AdExpress.Domain.Web;

namespace TNS.AdExpress.Web.UI.Results{
	/// <summary>
	/// Classe utilis� pour l'affichage des plaquettes Fichiers resultats
	/// </summary>
	public class FilesItemUI {

        #region M�thodes pour la sortie HTML
        /// <summary>
		/// Fonction pour la construction du code html pour l'affichage des plaquettes Fichiers resultats
		/// </summary>
		/// <returns>Source html du r�sultat</returns>
		public static string GetHtml(Page page, ArrayList list){

			#region Variables
			System.Text.StringBuilder t = new System.Text.StringBuilder(5000);
            string themeName = page.Theme;
			int compteur=0;
			int start=1;
			#endregion

			#region Script
			// Ouverture/fermeture des fen�tres p�res
			if (!page.ClientScript.IsClientScriptBlockRegistered("DivDisplayer")) {
                page.ClientScript.RegisterClientScriptBlock(page.GetType(), "DivDisplayer", TNS.AdExpress.Web.Functions.Script.ShowHideContent());
			}
			#endregion

			if (list!=null && list.Count>0)	{
				
				foreach (WebCommon.Results.FilesItem currentName in list){
				
					if (currentName!=null){
						if (start==1){
                            t.Append("\n<table class=\"txtViolet11Bold violetBorder backGroundWhite\"  cellpadding=0 cellspacing=0 width=\""+WebApplicationParameters.CustomStyles.FileItemWidth+"\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                            t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + themeName + "/Images/Common/Button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
							start=0;
						}
						else if (start==0){
                            t.Append("\n<table class=\"txtViolet11Bold violetBorderWithoutTop backGroundWhite\"  cellpadding=0 cellspacing=0 width=\"" + WebApplicationParameters.CustomStyles.FileItemWidth + "\">");
							t.Append("\n<tr onClick=\"javascript : showHideContent('"+currentName.Name+"');\" style=\"cursor : hand\">");
							t.Append("\n<td>&nbsp;"+ currentName.Name + "</td>");
                            t.Append("\n<td align=\"right\" width=\"15\"><IMG src=\"/App_Themes/" + themeName + "/Images/Common/button/bt_arrow_down.gif\" width=\"15\" height=\"15\"></td>");
							t.Append("\n</tr></table>");
						}

                        t.Append("<div id=\"" + currentName.Name + "Content\" style=\"MARGIN-LEFT: 0px; DISPLAY: none;\" class=\"BlancNoBorderColor\" >");
                        t.Append("\n<table id=\"" + currentName.Name + "\" class=\"txtViolet10 lightPurple violetBorderWithoutTop\" width=\"" + WebApplicationParameters.CustomStyles.FileItemWidth + "\">");
						compteur=0;
						for(int i=0; i<currentName.List.GetLength(1) && currentName.List[0,i]!=null; i++){
							if(compteur==0){ 
								switch(currentName.Type){
									
									case Anubis.Constantes.Result.type.appmInsertionDetail:
                                    case Anubis.Constantes.Result.type.pachet:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_text.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;
									case Anubis.Constantes.Result.type.appmExcel:
									case Anubis.Constantes.Result.type.amset:
                                    case Anubis.Constantes.Result.type.tefnout:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_excel.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;	
									case Anubis.Constantes.Result.type.appm:
									case Anubis.Constantes.Result.type.hotep:
									case Anubis.Constantes.Result.type.miysis:
									case Anubis.Constantes.Result.type.mnevis:
									case Anubis.Constantes.Result.type.shou:
									case Anubis.Constantes.Result.type.aton:
                                    case Anubis.Constantes.Result.type.selket:
                                    case Anubis.Constantes.Result.type.thoueris:
                                    case Anubis.Constantes.Result.type.apis:
                                    case Anubis.Constantes.Result.type.amon:
                                    case Anubis.Constantes.Result.type.ptah:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_pdf.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
										break;
                                    case Anubis.Constantes.Result.type.dedoum:
                                        t.Append("\n<tr><td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_zip.gif\" border=\"0\" alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td>");
                                        break;
								}	
								compteur=1;
							}
							else{
								switch(currentName.Type){ 
									
									case Anubis.Constantes.Result.type.appmInsertionDetail:
                                    case Anubis.Constantes.Result.type.pachet:

                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_text.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;
									case Anubis.Constantes.Result.type.appmExcel:
									case Anubis.Constantes.Result.type.amset:
                                    case Anubis.Constantes.Result.type.tefnout:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_excel.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;	
									case Anubis.Constantes.Result.type.appm:								
									case Anubis.Constantes.Result.type.hotep:
									case Anubis.Constantes.Result.type.miysis:
									case Anubis.Constantes.Result.type.mnevis:
									case Anubis.Constantes.Result.type.shou:
									case Anubis.Constantes.Result.type.aton:
                                    case Anubis.Constantes.Result.type.selket:
                                    case Anubis.Constantes.Result.type.thoueris:
                                    case Anubis.Constantes.Result.type.apis:
                                    case Anubis.Constantes.Result.type.amon:
                                    case Anubis.Constantes.Result.type.ptah:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_pdf.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
										break;
                                    case Anubis.Constantes.Result.type.dedoum:
                                        t.Append("\n<td width=50%><a href=\"" + currentName.List[1, i].ToString() + "\" target=\"_blank\"><img src=\"/App_Themes/" + themeName + "/Images/Common/file_zip.gif\" border=0 alt=\"" + currentName.List[0, i].ToString() + "\"></a>&nbsp;<a href=\"" + currentName.List[1, i].ToString() + "\" class=\"roll02\" target=\"_blank\">" + currentName.List[0, i].ToString() + "</a></td></tr>");
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

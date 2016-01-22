#region Information
/*
Author ; B.Masson
Creation : 25/08/2005
Last Modification : 
	26/08/2005 par B.Masson
*/
#endregion

using System;
using System.Data;
using System.Text;
using System.IO;
using System.Web.UI;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using RulesFct = TNS.AdExpress.Web.Rules.Results;
using WebFnc = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpress.Web.UI.Results.APPM
{
    /// <summary>
    /// Classe pour la construction du code html de la fiche justificative
    /// </summary>
    public class ProofUI
    {

        /// <summary>
        /// Méthode qui construit le code html
        /// </summary>
        /// <param name="page">Page</param>
        /// <param name="dataSource">DataSource</param>
        /// <param name="webSession">Session</param>
        /// <param name="idMedia">Identifiant du media</param>
        /// <param name="idProduct">Identifiant du produit</param>
        /// <param name="date">Date</param>
        /// <param name="dateCover">Date faciale</param>
        /// <param name="pageNumber">Numéro de la page</param>
        /// <returns>Code html</returns>
        public static string GetHtml(Page page, TNS.FrameWork.DB.Common.IDataSource dataSource, WebSession webSession, Int64 idMedia, Int64 idProduct, int date, int dateCover, string pageNumber)
        {

            #region GetData
            DataTable dtResult = RulesFct.APPM.ProofRules.GetData(dataSource, webSession, idMedia, idProduct, date, pageNumber);
            #endregion

            #region Variables
            StringBuilder html = new StringBuilder();
            //string pathWeb=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia+"/"+date+"/imagette/";
            //string pathWeb2=CstWeb.CreationServerPathes.IMAGES+"/"+idMedia+"/"+date+"/";
            string pathWeb = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia + "/" + dateCover + "/imagette/";
            string pathWeb2 = CstWeb.CreationServerPathes.IMAGES + "/" + idMedia + "/" + dateCover + "/";
            string[] fileList = null;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
            int i = 0;
            #endregion

            if (dtResult.Rows.Count > 0)
            {


                bool hasCopyRight = WebFnc.Rights.HasPressCopyright(idMedia);
                bool parutionDateBefore2015 = WebFnc.Rights.ParutionDateBefore2015(dateCover.ToString());
                const string blurDirectory = "blur";
                if (!hasCopyRight && !parutionDateBefore2015)
                {
                    pathWeb = string.Format("{0}{1}/", pathWeb, blurDirectory);
                    pathWeb2 = string.Format("{0}{1}/", pathWeb2, blurDirectory);
                }

                foreach (DataRow row in dtResult.Rows)
                {

                    #region Construction de la liste des images

                    if (row["visual"].ToString().Length > 0)
                    {
                        fileList = row["visual"].ToString().Split(',');
                    }

                    #endregion

                    #region Scripts

                    // Affichage de l'image taille réelle
                    if (!page.ClientScript.IsClientScriptBlockRegistered("ViewAdZoom"))
                    {
                        page.ClientScript.RegisterClientScriptBlock(page.GetType(), "ViewAdZoom", WebFnc.Script.ViewAdZoom());
                    }
                    // Ouverture de la popup chemin de fer
                    if (!page.ClientScript.IsClientScriptBlockRegistered("PortofolioCreationWithAnchor"))
                    {
                        page.ClientScript.RegisterClientScriptBlock(page.GetType(), "PortofolioCreationWithAnchor", WebFnc.Script.PortofolioCreationWithAnchor());
                    }
                    // Préchargement des images
                    if (fileList != null)
                    {
                        if (!page.ClientScript.IsClientScriptBlockRegistered("PreloadImages"))
                        {
                            page.ClientScript.RegisterClientScriptBlock(page.GetType(), "PreloadImages", WebFnc.Script.PreloadImages(fileList, pathWeb2));
                        }
                    }

                    #endregion

                    #region html

                    #region Début tableau général

                    html.Append("<TABLE  class=\"insertionBorderV2 whiteBackGround\"");
                    html.Append("cellPadding=\"0\" cellSpacing=\"1\" align=\"center\" border=\"0\" width=\"900\">");

                    html.Append("<tr align=\"center\"><td>");

                    #endregion

                    #region Début tableau fiche

                    html.Append("\n<table cellPadding=\"0\" cellSpacing=\"0\" border=\"0\" class=\"lightPurple\" width=\"100%\">");
                    //Header
                    html.Append("<tr>");
                    html.Append("<td  vAlign=\"top\" colspan=\"3\">");
                    html.Append("<table cellSpacing=\"0\" cellPadding=\"0\" border=\"0\">");
                    html.Append("<td>");
                    html.Append("<!-- fleche -->");
                    html.Append("<td style=\"WIDTH: 16px\" ><IMG height=\"16\" src=\"/App_Themes/" + themeName + "/Images/Common/fleche_1.gif\" border=\"0\"></td>");
                    html.Append("<td  vAlign=\"top\" width=\"100%\" class=\"popuptitle1 creationpopUpBackGround\" >");
                    html.Append(GestionWeb.GetWebWord(1766, webSession.SiteLanguage));
                    html.Append("</td>");
                    html.Append("</td>");
                    html.Append("</table>");
                    html.Append("</td>");
                    html.Append("</tr>");
                    html.Append("\n<tr><td colspan=\"3\" class=\"whiteBackGround\" style=\"HEIGHT: 3px;\" ></td></tr>");
                    html.Append("\n<tr valign=\"top\">");

                    #endregion

                    #region Table Visuel Zoom

                    html.Append("\n<td width=\"50%\"  valign=\"middle\">");
                    html.Append("\n<table cellPadding=\"3\" cellSpacing=\"0\" border=\"0\" align=\"center\" width=\"100%\">");
                    html.Append("\n<tr><td rowspan=\"19\" align=\"center\" valign=\"middle\">");
                    if (fileList != null)
                    {
                        html.Append("<img name=\"displayImg\" src='" + pathWeb2 + fileList.GetValue(0).ToString() + "' border=\"0\">");
                    }
                    else
                    {
                        html.Append("<img src=\"/App_Themes/" + themeName + "/Images/Culture/Others/no_visuel.gif\">");
                    }
                    html.Append("\n</td></tr>");
                    html.Append("\n</table>");
                    html.Append("\n</td>");

                    #endregion

                    #region Colone séparatrice

                    html.Append("\n<td class=\"whiteBackGround\">&nbsp;</td>");

                    #endregion

                    #region Table Informations

                    html.Append("\n<td>");
                    html.Append("\n<table cellPadding=\"3\" cellSpacing=\"0\" border=\"0\" align=\"center\" width=\"100%\">");

                    #region Visuel de la couverture du media + Nom du media + Date

                    // Chemin du répertoire virtuel de la couverture
                    string pathCouv = pathWeb + CstWeb.CreationServerPathes.COUVERTURE;

                    // Chemin du répertoire contenant le visuel de la couverture presse
                    string pathCouv2 = (hasCopyRight || parutionDateBefore2015) ? string.Format("{0}{1}\\{2}\\{3}",
                        CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE, idMedia, dateCover, CstWeb.CreationServerPathes.COUVERTURE) :
                        string.Format("{0}{1}\\{2}\\{3}\\{4}",
                        CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE, idMedia, dateCover, blurDirectory, CstWeb.CreationServerPathes.COUVERTURE);
                    //string pathCouv2 = CstWeb.CreationServerPathes.LOCAL_PATH_IMAGE+idMedia+@"\"+date+@"\"+CstWeb.CreationServerPathes.COUVERTURE;
                    // Pour test en localhost :
                    //string pathCouv2="\\\\localhost\\ImagesPresse\\"+idMedia+"\\"+date+"\\"+CstWeb.CreationServerPathes.COUVERTURE;

                    html.Append("\n<tr>");
                    if (File.Exists(pathCouv2))
                    {
                        html.Append("\n<td colspan=\"2\" align=\"center\"><img src='" + pathCouv + "' border=\"0\" width=\"100\" height=\"141\"><br><font class=\"txtViolet14Bold\">" + row["Media"] + "</font>");
                        if (row["date"] != System.DBNull.Value) html.Append("<br><font class=txtViolet11>" + Dates.DateToString((DateTime)row["date"], webSession.SiteLanguage) + "</font>"); //dateParution
                    }
                    else
                    {
                        html.Append("\n<td colspan=\"2\" align=\"center\"><img src=\"/Images/" + webSession.SiteLanguage + "/Others/no_visuel.gif\"><br><font class=\"txtViolet14Bold\">" + row["Media"] + "</font>");
                        if (row["date"] != System.DBNull.Value) html.Append("<br><font class=txtViolet11>" + Dates.DateToString((DateTime)row["date"], webSession.SiteLanguage) + "</font>"); //dateParution
                    }
                    html.Append("\n</tr>");

                    #endregion

                    #region Informations

                    html.Append("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\" ></td></tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\" width=\"50%\">&nbsp;" + GestionWeb.GetWebWord(857, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["advertiser"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(858, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["product"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(859, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["group_"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(894, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["media_paging"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1767, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["area_page"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1768, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + Decimal.Parse(row["area_mmc"].ToString()).ToString("# ### ##0.##") + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1769, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["location"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1420, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["format"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1438, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["color"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1426, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["rank_sector"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1427, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["rank_group_"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1428, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + row["rank_media"] + "</td>");
                    html.Append("\n</tr>");
                    html.Append("\n<tr valign=\"top\">");
                    html.Append("\n<td align=\"left\" class=\"txtViolet12Bold\">&nbsp;" + GestionWeb.GetWebWord(1770, webSession.SiteLanguage) + "</td>");
                    html.Append("\n<td align=\"left\" class=\"txtViolet11\">" + Decimal.Parse(row["expenditure_euro"].ToString()).ToString("# ### ##0.##") + "</td>");
                    html.Append("\n</tr>");

                    #endregion

                    if (fileList != null)
                    {

                        #region Vignettes

                        html.Append("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\" ></td></tr>");
                        html.Append("\n<tr><td colspan=\"2\" align=\"center\">");
                        foreach (string currentFile in fileList)
                        {
                            html.Append("<a href=\"#\"><img src='" + pathWeb + currentFile + "' border=\"0\" width=\"50\" height=\"64\" onMouseOver=\"javascript:viewAdZoom('Img" + i + "');\"></a>&nbsp;");
                            i++;
                        }
                        html.Append("\n</td></tr>");

                        #endregion

                        #region Lien du chemin de fer

                        html.Append("\n<tr><td colspan=\"2\" bgcolor=\"#FFFFFF\" style=\"HEIGHT: 1px;\"></td></tr>");
                        html.Append("\n<tr><td colspan=\"2\" align=\"center\">");
                        html.Append("<a href=\"javascript:portofolioCreationWithAnchor('" + webSession.IdSession + "','" + idMedia + "','" + date + "','" + dateCover + "','" + row["Media"] + "','" + row["number_page_media"].ToString() + "','" + pageNumber + "');\" class=\"roll04\">" + GestionWeb.GetWebWord(1397, webSession.SiteLanguage) + "</a>");
                        //html.Append("<a href=\"javascript:portofolioCreationWithAnchor('"+webSession.IdSession+"','"+ idMedia +"','"+ date +"','"+ row["Media"] +"','"+ row["number_page_media"].ToString()+"','"+ pageNumber +"');\" class=\"roll04\">"+ GestionWeb.GetWebWord(1397, webSession.SiteLanguage) +"</a>");
                        html.Append("\n</td></tr>");

                        #endregion

                    }

                    html.Append("\n</table>");
                    html.Append("\n</td>");

                    #endregion

                    #region Fin tableau fiche

                    html.Append("\n</tr>");
                    html.Append("\n</table>");

                    #endregion

                    #region Fin tableau général

                    html.Append("</td></tr></table></td></tr></table>");

                    #endregion

                    #endregion

                }

            }
            else
            {
                html.Append("<div align=\"center\" class=\"txtBlanc11Bold\">" + GestionWeb.GetWebWord(177, webSession.SiteLanguage) + "</div>");
            }
            return html.ToString();
        }

      

    }
}

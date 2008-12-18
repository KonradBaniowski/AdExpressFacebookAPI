#region Informations
// Auteur: Y. R'kaina
// Date de création: 03/09/2008
// Date de modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Controls.Results;

namespace TNS.AdExpress.Web.Controls.Headers {
    /// <summary>
    /// Control used to select a language
    /// </summary>
    [ToolboxData("<{0}:LanguageSelectionWebControl runat=server></{0}:LanguageSelectionWebControl>")]
    public class LanguageSelectionWebControl : ImageDropDownListWebControl {

        #region Variables
        /// <summary>
        /// Language
        /// </summary>
        private int _language;
        /// <summary>
        /// Show Text in the dropDownList
        /// </summary>
        private bool _showText = false;
        #endregion

        #region Accessors
        /// <summary>
        /// Set language
        /// </summary>
        public int Language {
            set { _language = value; }
        }
        /// <summary>
        /// Set Show Text
        /// </summary>
        public bool ShowText {
            set { _showText = value; }
        }
        #endregion

        #region Evènements

        #region Render
        /// <summary>
        /// Render
        /// </summary>
        /// <param name="writer">Sortie</param>
        protected override void Render(HtmlTextWriter writer) {

            #region Variables locales
            int i = 0;
            string img = "";

            string themeName = TNS.AdExpress.Domain.Web.WebApplicationParameters.Themes[_language].Name;

            string link = GetLinkPath();

            string bgColor = ColorToStr(this.BackColor);
            string borderColor = ColorToStr(this.BorderColor);
            double borderWidth = Math.Max(1, this.BorderWidth.Value);

            string[] idList = new string[WebApplicationParameters.AllowedLanguages.Count];
            string[] textList = new string[WebApplicationParameters.AllowedLanguages.Count];
            string[] imageList = new string[WebApplicationParameters.AllowedLanguages.Count];

            #endregion

            #region Initialisation des listes
            int iter = 0;
            foreach (WebLanguage currentLanguage in WebApplicationParameters.AllowedLanguages.Values) {
                if (_language == currentLanguage.Id)
                    index = iter;
                idList[iter] = currentLanguage.Id.ToString();
                if (_showText)
                    textList[iter] = "&nbsp;" + currentLanguage.Name;
                else
                    textList[iter] = "&nbsp;";
                imageList[iter] = "/App_Themes/" + themeName + currentLanguage.ImageSourceText;
                iter++;
            }
            #endregion

            #region Génération du code HTML

            #region Entete combolist
            writer.Write("\n\t <div>");
            writer.Write("\n\t\t <table width=\"" + this.Width + "\" border=\"0\" cellSpacing=\"0\">");
            writer.Write("\n\t\t\t <tr>");
            writer.Write("\n\t\t\t <td><table width=\"100%\" border=\"0\" cellPadding=\"0\" cellSpacing=\"0\"" +
                " style=\"cursor: hand;\"" +
                " onClick=\"javascript:ShowScrollerImageLS('" + this.ID + "');\">");

            writer.Write("\n\t\t\t\t <tr>");

            if (index > -1) {
                try {
                    if (imageList.GetUpperBound(0) >= index)
                        img = imageList[index];
                    else
                        img = "/Images/Common/pixel.gif";
                }
                catch (System.Exception) {
                    img = "/Images/Common/pixel.gif";
                }

                writer.Write("\n\t\t\t\t\t <td  height=\"" + (imageHeight + 4.0) + "\" class=\"" + OutCssClass + "\" valign=\"middle\" align=\"left\" nowrap>");
                if (pictShow) writer.Write("&nbsp;<img class=\"" + OutCssClass + " imgVAlign\" id=\"ComboImg_" + this.ID + "\" src=\"" + img + "\" width=\"" + imageWidth + "\" height=\"" + imageHeight + "\" align=\"absmiddle\">");
                if (_showText) {
                    writer.Write("<label id=\"ComBoText_" + this.ID + "\">" + textList[index] + "</label>");
                    writer.Write("<img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" width=\"5\" align=\"absmiddle\">");
                }
                else
                    writer.Write("<label id=\"ComBoText_" + this.ID + "\"></label>");
                writer.Write("</td>");
            }
            else {
                writer.Write("\n\t\t\t\t\t <td nowrap><img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" witdh=\"2\" align=\"absmiddle\">");
                if (pictShow) writer.Write("<img class=\"" + OutCssClass + "\" id=\"ComboImg_\"" + this.ID + "\" src=\"/Images/Common/pixel.gif\" width=\"15\" height=\"15\" align=\"absmiddle\">");
                writer.Write("<label id=\"ComBoText_" + this.ID + "></label>");
                writer.Write("<img class=\"" + OutCssClass + "\" src=\"/Images/Common/pixel.gif\" width=\"2\" align=\"absmiddle\"></td>)");
            }
            writer.WriteLine("\t\t\t\t\t <td height=\"" + (imageHeight + 4.0) + "\" valign=\"middle\" align=\"right\"><img class=\"" + OutCssClass + " imgVAlign\" src=\"" + imageButtonArrow + "\" border=\"0\"></td>");
            writer.WriteLine("\t\t\t\t </tr>");
            writer.WriteLine("\t\t\t </table></td>");
            writer.WriteLine("\t\t\t </tr>");
            #endregion

            #region Partie scrollable
            writer.WriteLine("\t\t\t <tr> ");
            writer.WriteLine("\t\t\t <td><div id=\"scroller_" + this.ID + "\" style=\"display: none; width:100%; overflow: visible; background-color: " + bgColor + "; layer-background-color: " + bgColor + "; border: 1px none #000000; \">");
            writer.WriteLine("\t\t\t <table width=\"100%\" border=\"0\" cellPadding=\"1\" cellSpacing=\"0\" style=\"border: " + borderWidth + "px solid " + borderColor + "; cursor: hand;\">");

            //Les items
            for (i = 0; i <= textList.GetUpperBound(0); i++) {
                try {
                    if (imageList.GetUpperBound(0) >= i) {
                        //Une image est définie pour le controle en cours
                        writer.WriteLine("<tr id=\"scroller_item_" + this.ID + "_" + i + "\" class=\"" + OutCssClass + "\" onMouseOver=\"ChangeItemClassLS(this, '" + OverCssClass + "');\" onMouseOut=\"ChangeItemClassLS(this, '" + OutCssClass + "');\" onClick=\"ItemClickLS('" + this.ID + "',this, " + i + ", '" + textList[i].Replace("'", "\'") + "', '" + imageList[i] + "','" + (link + idList[i]) + "');\">");
                        if (pictShow) {
                            writer.WriteLine("<td align=\"center\" valign=\"middle\" width=\"" + (imageWidth + 2.0) + "\" height=\"" + (imageHeight + 4) + "\">");
                            writer.WriteLine("<img src=\"" + imageList[i] + "\" width=\"" + imageWidth + "\" height=\"" + imageHeight + "\" align=\"absmiddle\" class=\"imgVAlign\">");
                            writer.WriteLine("</td>");
                        }
                        else {
                            throw new System.Exception();
                        }
                    }
                }
                catch (System.Exception) {
                    //Aucune image n'est définie, donc on y met un pixel transparent
                    writer.WriteLine("<tr id=\"scroller_item_" + this.ID + "_" + i + "\" class=\"" + OutCssClass + "\" onMouseOver=\"ChangeItemClassLS(this, '" + OverCssClass + "')\" onMouseOut=\"ChangeItemClassLS(this, '" + OutCssClass + "')\" onClick=\"ItemClickLS('" + this.ID + "',this, " + i + ", '" + textList[i].Replace("'", "\'") + "', '/Images/Common/pixel.gif','" + (link + idList[i]) + "')\">");
                    if (pictShow) {
                        writer.WriteLine("<td width=\"1\">");
                        writer.WriteLine("<img src=\"/Images/Common/pixel.gif\" height=\"15\" align=\"absmiddle\">");
                        writer.WriteLine("</td>");
                    }
                }
                if (_showText)
                    writer.WriteLine("<td nowrap>" + textList[i] + "</td>");
                else
                    writer.WriteLine("<td nowrap></td>");
                writer.WriteLine("</tr>");
            }
            //Fermeture des balises --
            writer.WriteLine("</table>");
            writer.WriteLine("</div></td>");
            writer.WriteLine("</tr>");
            #endregion

            writer.WriteLine("</table>");
            writer.WriteLine("</div>");

            writer.Write("\n\t <input class=\"" + OutCssClass + "\" type=\"hidden\" name=\"" + this.ID + "\"  id=\"" + this.ID + "\" value=\"" + index + "\">");
            
            #endregion

        }
        #endregion

        #endregion

        #region Private Methods
        /// <summary>
        /// Get Path and Query
        /// </summary>
        /// <returns>Path string</returns>
        private string GetLinkPath() {

            string link = this.Parent.Page.Request.Url.PathAndQuery.ToString();
            string[] linkParam = link.Split('?');
            string[] paramsList;
            bool verif = true;

            link = linkParam[0];

            if (linkParam.Length > 1) {
                paramsList = linkParam[1].Split('&');
                for (int i = 0; i < paramsList.Length; i++)
                    if (!paramsList[i].Contains("siteLanguage"))
                        if (verif) {
                            link += "?" + paramsList[i];
                            verif = false;
                        }
                        else
                            link += "&" + paramsList[i];
            }

            if (verif)
                link += "?siteLanguage=";
            else
                link += "&siteLanguage=";

            return link;
        }
        #endregion

    }
}

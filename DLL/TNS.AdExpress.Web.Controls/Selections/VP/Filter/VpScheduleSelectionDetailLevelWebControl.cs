using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using System;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpress.Domain.Web.Navigation;
using System.Collections;
using TNS.AdExpress.Domain.Level;
using TNS.FrameWork.Date;
namespace TNS.AdExpress.Web.Controls.Selections.VP.Filter
{
    /// <summary>
    /// Affiche le r�sultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleSelectionDetailLevelWebControl runat=server></{0}:VpScheduleSelectionDetailLevelWebControl>")]
    public class VpScheduleSelectionDetailLevelWebControl : VpScheduleSelectionFilterBaseWebControl {

        #region Property (Style)
        /// <summary>
        /// Get / Set CssClassLvl1
        /// </summary>
        public string CssClassLvl1 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl2
        /// </summary>
        public string CssClassLvl2 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl3
        /// </summary>
        public string CssClassLvl3 { get; set; }
        /// <summary>
        /// Get / Set CssClassLvl4
        /// </summary>
        public string CssClassLvl4 { get; set; }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            StringBuilder js = new StringBuilder(1000);

            js.Append("\r\nfunction onClickDetailTab_"+this.ID+"(lvl){");
            js.Append("\r\n\tif(lvl==0 && document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value!='none'){");
            js.Append("\r\n\t\tif(document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value == document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl1_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t\tif(document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value == document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl2_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t}");
            js.Append("\r\n\telse if(lvl==1 && document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value!='none'){");
            js.Append("\r\n\t\tif(document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value == document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl0_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t\tif(document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value == document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl2_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t}");
            js.Append("\r\n\telse if(lvl==2 && document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value!='none'){");
            js.Append("\r\n\t\tif(document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value == document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl1_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t\tif(document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value == document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value)");
            js.Append("\r\n\t\t\tdocument.getElementById('lvl0_" + this.ID + "').selectedIndex = 0;");
            js.Append("\r\n\t}");
            js.Append("\r\n}\r\n");
            return js.ToString();
        }
        #endregion   

        #region GetValuesSelectedMethodScriptContent
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetValuesSelectedMethodScriptContent() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tvar tab = new Array();");
            js.Append("\r\n\ttab.push(document.getElementById('lvl0_" + this.ID + "').options[document.getElementById('lvl0_" + this.ID + "').selectedIndex].value + ',' + document.getElementById('lvl1_" + this.ID + "').options[document.getElementById('lvl1_" + this.ID + "').selectedIndex].value + ',' + document.getElementById('lvl2_" + this.ID + "').options[document.getElementById('lvl2_" + this.ID + "').selectedIndex].value);");

            js.Append("\r\n\tfor(var i=0; i<document.forms[0].personalize_" + this.ID + ".length; i++) {");
            js.Append("\r\n\t\tif(document.forms[0].personalize_" + this.ID + "[i].checked){ ");
            js.Append("\r\n\t\t\ttab.push(document.forms[0].personalize_" + this.ID + "[i].value);");
            js.Append("\r\n\t\tbreak;");
            js.Append("\r\n\t\t}");
            js.Append("\r\n\t}");

            js.Append("\r\n\treturn tab;");
            return js.ToString();
        }
        #endregion

        #region Enregistrement des param�tres de construction du r�sultat
        protected override string SetCurrentResultParametersScript() {
            StringBuilder js = new StringBuilder();
            return (base.SetCurrentResultParametersScript() + js.ToString());
        }
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
            base.LoadCurrentResultParameters(o);
        }
        #endregion

        #region Enregistrement des param�tres pour les styles
        protected override string SetCurrentStyleParametersScript() {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\t obj.CssClassLvl1 = '" + CssClassLvl1 + "';");
            js.Append("\r\n\t obj.CssClassLvl2 = '" + CssClassLvl2 + "';");
            js.Append("\r\n\t obj.CssClassLvl3 = '" + CssClassLvl3 + "';");
            js.Append("\r\n\t obj.CssClassLvl4 = '" + CssClassLvl4 + "';");

            return (base.SetCurrentStyleParametersScript() + js.ToString());
        }

        protected override void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o) {
            base.LoadCurrentStyleParameters(o);
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML() {
            StringBuilder html = new StringBuilder(1000);

            int nbMonthVisu = 2;

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);


            //List<DetailLevelItemsInformation> detailLevelItemsInformationList = new List<DetailLevelItemsInformation>(module.AllowedMediaDetailLevelItems);
            //DetailLevelItemsInformation


            html.AppendFormat("<div id=\"div_{0}\" width=\"100%\" class=\"{1}\">", this.ID, CssClass);
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>");

            #region Detail Tab
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(2873, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td align=\"center\">");

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            html.Append("<tr>");

            html.Append("<td class=\"pt\" rowspan=\"2\">" + GestionWeb.GetWebWord(2872, _webSession.SiteLanguage) + "</td>");

            for (int i = 0; i < nbMonthVisu; i++) {
                html.Append("<td class=\"ptm\" colspan=\"4\">" + MonthString.GetHTMLCharacters(i + 1, _webSession.SiteLanguage, 3) + " " + DateTime.Now.ToString("yy") + "</td>");
            }


            html.Append("<tr>");
            for (int j = 0; j < nbMonthVisu * 4; j++) {
                html.Append("<td class=\"vpw\">" + (j + 1).ToString("00") + "</td>");
            }
            html.Append("</tr>");


            string space = string.Empty;

            #region LvL1
            bool hasSelected = false;
            StringBuilder htmlDropDownOptionsLvL1 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in module.AllowedMediaDetailLevelItems) {
                htmlDropDownOptionsLvL1.Append("<option value=\"" + cDetailLevelItemsInformation.Id.ToString() + "\" ");
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 1
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[0]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL1.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL1.Append(">" + GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage) + "</option>");
            }
            htmlDropDownOptionsLvL1 = new StringBuilder("<option value=\"none\">-------</option " + ((!hasSelected) ? "selected = \"selected\"" : string.Empty) + ">" + htmlDropDownOptionsLvL1.ToString());
            html.Append("<tr>");
            html.Append("<td class=\"L0\">" + space);
            html.Append("<select id=\"lvl0_" + this.ID + "\" onchange=\"javascript:onClickDetailTab_" + this.ID + "(0);\">");
            html.Append(htmlDropDownOptionsLvL1.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.Append("<td class=\"p3\" colspan=\"" + 4 * nbMonthVisu + "\">&nbsp;</td>");
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            #region LvL2
            hasSelected = false;
            StringBuilder htmlDropDownOptionsLvL2 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in module.AllowedMediaDetailLevelItems) {
                htmlDropDownOptionsLvL2.Append("<option value=\"" + cDetailLevelItemsInformation.Id.ToString() + "\" ");
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 2
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[1]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL2.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL2.Append(">" + GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage) + "</option>");
            }
            htmlDropDownOptionsLvL2 = new StringBuilder("<option value=\"none\" " + ((!hasSelected) ? "selected = \"selected\"" : string.Empty) + ">-------</option>" + htmlDropDownOptionsLvL2.ToString());

            html.Append("<tr>");
            html.Append("<td class=\"L1\">" + space);
            html.Append("<select id=\"lvl1_" + this.ID + "\" onchange=\"javascript:onClickDetailTab_" + this.ID + "(1);\">");
            html.Append(htmlDropDownOptionsLvL2.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.Append("<td class=\"p3\" colspan=\"" + 4 * nbMonthVisu + "\">&nbsp;</td>");
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            #region LvL3
            hasSelected = false;
            StringBuilder htmlDropDownOptionsLvL3 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in module.AllowedMediaDetailLevelItems) {
                htmlDropDownOptionsLvL3.Append("<option value=\"" + cDetailLevelItemsInformation.Id.ToString() + "\" ");
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 3
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[2]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL3.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL3.Append(">" + GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage) + "</option>");
            }
            htmlDropDownOptionsLvL3 = new StringBuilder("<option value=\"none\" "+((!hasSelected)?"selected = \"selected\"":string.Empty)+">-------</option>" + htmlDropDownOptionsLvL3.ToString());
            html.Append("<tr>");
            html.Append("<td class=\"L2\">" + space);
            html.Append("<select id=\"lvl2_" + this.ID + "\" onchange=\"javascript:onClickDetailTab_" + this.ID + "(2);\">");
            html.Append(htmlDropDownOptionsLvL3.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.Append("<td class=\"p3\" colspan=\"" + 4 * nbMonthVisu + "\">&nbsp;</td>");
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            html.Append("</table>");

            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td></tr>");
            html.Append("<tr><td></td></tr>");
            html.Append("<tr><td>");

            #region Promo personnalisation
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(2874, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(2875, _webSession.SiteLanguage) + " :</td></tr>");

            html.Append("<tr>");
            for (int i = 0; i < ((module.AllowedMediaDetailLevelItems.Count % 2 == 0) ? module.AllowedMediaDetailLevelItems.Count : module.AllowedMediaDetailLevelItems.Count + 1); i++) {
                if (i > 0 && i % 2 == 0) html.Append("</tr><tr>");
                html.Append("<td>");
                if (i < module.AllowedMediaDetailLevelItems.Count) {
                    DetailLevelItemInformation cDetailLevelItemInformation = ((DetailLevelItemInformation)module.AllowedMediaDetailLevelItems[i]);
                    html.Append("<input type=\"radio\" name=\"personalize_" + this.ID + "\" value=\"" + cDetailLevelItemInformation.Id + "\" " + ((_webSession.PersonnalizedLevel == cDetailLevelItemInformation.Id) ? "checked" : string.Empty) + ">" + GestionWeb.GetWebWord(((DetailLevelItemInformation)module.AllowedMediaDetailLevelItems[i]).WebTextId, _webSession.SiteLanguage) + "</input>");
                }
                html.Append("</td>");
            }
            html.Append("</tr>");

            html.Append("</table>");

            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td></tr>");
            html.Append("</table>");
            html.Append("</div>");


            return (html.ToString());
        }
        #endregion

    }
}

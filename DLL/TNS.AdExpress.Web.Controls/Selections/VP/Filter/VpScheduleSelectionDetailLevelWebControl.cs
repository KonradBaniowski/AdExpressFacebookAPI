using System.Linq;
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
    /// Affiche le résultat d'une alerte plan media
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
        /// <summary>
        /// Get / Set CssClassDetailTab
        /// </summary>
        public string CssClassDetailTab { get; set; }
        /// <summary>
        /// Get / Set CssClassDetailTabHeader
        /// </summary>
        public string CssClassDetailTabHeader { get; set; }
        /// <summary>
        /// Get / Set CssClassSeparator
        /// </summary>
        public string CssClassSeparator { get; set; }
        /// <summary>
        /// Get / Set CssClassPersonnalisationHeader
        /// </summary>
        public string CssClassPersonnalisationHeader { get; set; }
        /// <summary>
        /// Get / Set CssClassPersonnalisationHeaderBis
        /// </summary>
        public string CssClassPersonnalisationHeaderBis { get; set; }
        /// <summary>
        /// Get / Set CssClassPersonnalisation
        /// </summary>
        public string CssClassPersonnalisation { get; set; }
        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            var js = new StringBuilder(1000);
            js.Append("\r\nvar isChanged_" + this.ID + " = false;");

            js.Append("\r\nfunction onclickPernalize_" + this.ID + "(){");
            js.Append("\r\n\tisChanged_" + this.ID + " = true;");
            js.Append("\r\n}\r\n");

            js.Append("\r\nfunction onClickDetailTab_"+this.ID+"(lvl){");
            js.Append("\r\n\tisChanged_" + this.ID + " = true;");
            js.AppendFormat("\r\n\tif(lvl==0 && document.getElementById('lvl0_{0}').options[document.getElementById('lvl0_{0}').selectedIndex].value!='none'){{", this.ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl1_{0}').options[document.getElementById('lvl1_{0}').selectedIndex].value == document.getElementById('lvl0_{0}').options[document.getElementById('lvl0_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl1_{0}').selectedIndex = 0;", ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl2_{0}').options[document.getElementById('lvl2_{0}').selectedIndex].value == document.getElementById('lvl0_{0}').options[document.getElementById('lvl0_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl2_{0}').selectedIndex = 0;", this.ID);
            js.Append("\r\n\t}");
            js.AppendFormat("\r\n\telse if(lvl==1 && document.getElementById('lvl1_{0}').options[document.getElementById('lvl1_{0}').selectedIndex].value!='none'){{", this.ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl0_{0}').options[document.getElementById('lvl0_{0}').selectedIndex].value == document.getElementById('lvl1_{0}').options[document.getElementById('lvl1_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl0_{0}').selectedIndex = 0;", this.ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl2_{0}').options[document.getElementById('lvl2_{0}').selectedIndex].value == document.getElementById('lvl1_{0}').options[document.getElementById('lvl1_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl2_{0}').selectedIndex = 0;", this.ID);
            js.Append("\r\n\t}");
            js.AppendFormat("\r\n\telse if(lvl==2 && document.getElementById('lvl2_{0}').options[document.getElementById('lvl2_{0}').selectedIndex].value!='none'){{", this.ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl1_{0}').options[document.getElementById('lvl1_{0}').selectedIndex].value == document.getElementById('lvl2_{0}').options[document.getElementById('lvl2_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl1_{0}').selectedIndex = 0;", this.ID);
            js.AppendFormat("\r\n\t\tif(document.getElementById('lvl0_{0}').options[document.getElementById('lvl0_{0}').selectedIndex].value == document.getElementById('lvl2_{0}').options[document.getElementById('lvl2_{0}').selectedIndex].value)", this.ID);
            js.AppendFormat("\r\n\t\t\tdocument.getElementById('lvl0_{0}').selectedIndex = 0;", this.ID);
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
            var js = new StringBuilder(1000);
            js.Append("\r\n\tif(isChanged_" + this.ID + " == false) return null;");
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

        #region GetValuesSelectedMethodScriptContent
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetInitializeResultMethodContent() {
            return base.GetInitializeResultMethodContent() + "isChanged_" + this.ID + " = false;";
        }
        #endregion

        #region Enregistrement des paramètres de construction du résultat
        protected override string SetCurrentResultParametersScript() {
            StringBuilder js = new StringBuilder();
            return (base.SetCurrentResultParametersScript() + js.ToString());
        }
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o) {
            base.LoadCurrentResultParameters(o);
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        protected override string SetCurrentStyleParametersScript() {
            var js = new StringBuilder();

            js.AppendFormat("\r\n\t obj.CssClassLvl1 = '{0}';", CssClassLvl1);
            js.AppendFormat("\r\n\t obj.CssClassLvl2 = '{0}';", CssClassLvl2);
            js.AppendFormat("\r\n\t obj.CssClassLvl3 = '{0}';", CssClassLvl3);
            js.AppendFormat("\r\n\t obj.CssClassLvl4 = '{0}';", CssClassLvl4);
            js.AppendFormat("\r\n\t obj.CssClassDetailTab = '{0}';", CssClassDetailTab);
            js.AppendFormat("\r\n\t obj.CssClassDetailTabHeader = '{0}';", CssClassDetailTabHeader);
            js.AppendFormat("\r\n\t obj.CssClassSeparator = '{0}';", CssClassSeparator);
            js.AppendFormat("\r\n\t obj.CssClassPersonnalisationHeader = '{0}';", CssClassPersonnalisationHeader);
            js.AppendFormat("\r\n\t obj.CssClassPersonnalisationHeaderBis = '{0}';", CssClassPersonnalisationHeaderBis);
            js.AppendFormat("\r\n\t obj.CssClassPersonnalisation = '{0}';", CssClassPersonnalisation);

            return (base.SetCurrentStyleParametersScript() + js.ToString());
        }

        protected override void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o) {
            base.LoadCurrentStyleParameters(o);

            if (o.Contains("CssClassLvl1")) {
                CssClassLvl1 = o["CssClassLvl1"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl2")) {
                CssClassLvl2 = o["CssClassLvl2"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl3")) {
                CssClassLvl3 = o["CssClassLvl3"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassLvl4")) {
                CssClassLvl4 = o["CssClassLvl4"].Value.Replace("\"", "");
            }


            if (o.Contains("CssClassDetailTab")) {
                CssClassDetailTab = o["CssClassDetailTab"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassDetailTabHeader")) {
                CssClassDetailTabHeader = o["CssClassDetailTabHeader"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassPersonnalisationHeader")) {
                CssClassPersonnalisationHeader = o["CssClassPersonnalisationHeader"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassPersonnalisationHeaderBis")) {
                CssClassPersonnalisationHeaderBis = o["CssClassPersonnalisationHeaderBis"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassSeparator")) {
                CssClassSeparator = o["CssClassSeparator"].Value.Replace("\"", "");
            }
            if (o.Contains("CssClassPersonnalisation")) {
                CssClassPersonnalisation = o["CssClassPersonnalisation"].Value.Replace("\"", "");
            }
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML() {
            var html = new StringBuilder(1000);

            int nbMonthVisu = 2;

            Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);

            List<DetailLevelItemInformation> allowedMediaDetailLevelItems = GetAllowedMediaDetailLevelItems(module);
           


            html.AppendFormat("<div id=\"div_{0}\" width=\"100%\" class=\"{1}\">", this.ID, CssClass);
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\" class=\"" + CssClass + "\">");
            html.Append("<tr><td class=\"" + CssClassDetailTab + "\">");

            #region Detail Tab
            html.AppendFormat("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\" class=\"{0}\">", CssClassDetailTab);
            html.AppendFormat("<tr><td class=\"{0}\">{1} :</td></tr>", CssClassDetailTabHeader, GestionWeb.GetWebWord(2873, _webSession.SiteLanguage));
            html.Append("<tr><td align=\"center\">");

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            html.Append("<tr>");

            html.AppendFormat("<td class=\"pt\" rowspan=\"2\">{0}</td>", GestionWeb.GetWebWord(2872, _webSession.SiteLanguage));

            for (int i = 0; i < nbMonthVisu; i++) {
                html.AppendFormat("<td class=\"ptm\" colspan=\"4\">{0} {1}</td>"
                    , MonthString.GetHTMLCharacters(i + 1, _webSession.SiteLanguage, 3), DateTime.Now.ToString("yy"));
            }


            html.Append("<tr>");
            for (int j = 0; j < nbMonthVisu * 4; j++) {
                html.AppendFormat("<td class=\"vpw\">{0}</td>", (j + 1).ToString("00"));
            }
            html.Append("</tr>");


            string space = string.Empty;

            #region LvL1
            bool hasSelected = false;
            var htmlDropDownOptionsLvL1 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in allowedMediaDetailLevelItems)
            {
                htmlDropDownOptionsLvL1.AppendFormat("<option value=\"{0}\" ", cDetailLevelItemsInformation.Id.ToString());
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 1
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[0]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL1.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL1.AppendFormat(">{0}</option>", GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage));
            }
            htmlDropDownOptionsLvL1 = new StringBuilder("<option value=\"none\">-------</option " + ((!hasSelected) ? "selected = \"selected\"" : string.Empty) + ">" + htmlDropDownOptionsLvL1.ToString());
            html.Append("<tr>");
            html.AppendFormat("<td class=\"L0\">{0}", space);
            html.AppendFormat("<select id=\"lvl0_{0}\" onchange=\"javascript:onClickDetailTab_{0}(0);\">", this.ID);
            html.Append(htmlDropDownOptionsLvL1.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.AppendFormat("<td class=\"p3\" colspan=\"{0}\">&nbsp;</td>", 4 * nbMonthVisu);
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            #region LvL2
            hasSelected = false;
            var htmlDropDownOptionsLvL2 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in allowedMediaDetailLevelItems)
            {
                htmlDropDownOptionsLvL2.AppendFormat("<option value=\"{0}\" ", cDetailLevelItemsInformation.Id.ToString());
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 2
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[1]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL2.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL2.Append(">" + GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage) + "</option>");
            }
            htmlDropDownOptionsLvL2 = new StringBuilder("<option value=\"none\" " + ((!hasSelected) ? "selected = \"selected\"" : string.Empty) + ">-------</option>" + htmlDropDownOptionsLvL2.ToString());

            html.Append("<tr>");
            html.AppendFormat("<td class=\"L1\">{0}", space);
            html.AppendFormat("<select id=\"lvl1_{0}\" onchange=\"javascript:onClickDetailTab_{0}(1);\">", this.ID);
            html.Append(htmlDropDownOptionsLvL2.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.AppendFormat("<td class=\"p3\" colspan=\"{0}\">&nbsp;</td>", 4 * nbMonthVisu);
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            #region LvL3
            hasSelected = false;
            var htmlDropDownOptionsLvL3 = new StringBuilder(1000);
            foreach (DetailLevelItemInformation cDetailLevelItemsInformation in allowedMediaDetailLevelItems)
            {
                htmlDropDownOptionsLvL3.AppendFormat("<option value=\"{0}\" ", cDetailLevelItemsInformation.Id.ToString());
                if (_webSession.GenericMediaDetailLevel.LevelIds.Count >= 3
                    && ((DetailLevelItemInformation.Levels)_webSession.GenericMediaDetailLevel.LevelIds[2]) == cDetailLevelItemsInformation.Id) {
                    htmlDropDownOptionsLvL3.Append("selected = \"selected\"");
                    hasSelected = true;
                }
                htmlDropDownOptionsLvL3.AppendFormat(">{0}</option>"
                    , GestionWeb.GetWebWord(cDetailLevelItemsInformation.WebTextId, _webSession.SiteLanguage));
            }
            htmlDropDownOptionsLvL3 = new StringBuilder("<option value=\"none\" "+((!hasSelected)?"selected = \"selected\"":string.Empty)+">-------</option>" + htmlDropDownOptionsLvL3.ToString());
            html.Append("<tr>");
            html.Append("<td class=\"L2\">" + space);
            html.AppendFormat("<select id=\"lvl2_{0}\" onchange=\"javascript:onClickDetailTab_{0}(2);\">"
                , this.ID);
            html.Append(htmlDropDownOptionsLvL3.ToString());
            html.Append("</select>");
            html.Append("</td>");
            html.AppendFormat("<td class=\"p3\" colspan=\"{0}\">&nbsp;</td>"
                , 4 * nbMonthVisu);
            html.Append("</tr>");
            space += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;";
            #endregion

            html.Append("</table>");

            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td></tr>");
            html.AppendFormat("<tr><td class=\"{0}\">&nbsp;</td></tr>"
                , CssClassSeparator);
            html.Append("<tr><td>");

            #region Promo personnalisation
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\"  class=\"" + CssClassPersonnalisation + "\">");
            html.AppendFormat("<tr><td class=\"{0}\">{1} :</td></tr>"
                , CssClassPersonnalisationHeader, GestionWeb.GetWebWord(2874, _webSession.SiteLanguage));

            html.Append("<tr><td>");

            html.AppendFormat("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" class=\"{0}\">"
                , CssClassPersonnalisationHeaderBis);

            html.AppendFormat("<tr><td class=\"{0}\">{1} :</td></tr>"
                , CssClassPersonnalisationHeaderBis, GestionWeb.GetWebWord(2875, _webSession.SiteLanguage));

            html.Append("<tr><td>");

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");

            html.Append("<tr>");
            for (int i = 0; i < ((allowedMediaDetailLevelItems.Count % 2 == 0) ?
                allowedMediaDetailLevelItems.Count : allowedMediaDetailLevelItems.Count + 1); i++)
            {
                if (i > 0 && i % 2 == 0) html.Append("</tr><tr>");
                html.Append("<td>");
                if (i < allowedMediaDetailLevelItems.Count)
                {
                    var cDetailLevelItemInformation = allowedMediaDetailLevelItems[i];
                    html.AppendFormat("<input type=\"radio\" name=\"personalize_{0}\" onclick=\"javascript:onclickPernalize_{0}();\" value=\"{1}\" {2}>{3}</input>"
                        , this.ID, cDetailLevelItemInformation.Id, ((_webSession.PersonnalizedLevel == cDetailLevelItemInformation.Id) ? "checked" : string.Empty),
                        GestionWeb.GetWebWord(allowedMediaDetailLevelItems[i].WebTextId, _webSession.SiteLanguage));
                }
                html.Append("</td>");
            }
            html.Append("</tr>");

            html.Append("</table>");

            html.Append("</td></tr>");
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

        protected List<DetailLevelItemInformation> GetAllowedMediaDetailLevelItems(Domain.Web.Navigation.Module module)
        {
            string vpVehicleAccess = _webSession.CustomerLogin[Constantes.Customer.Right.type.vpVehicleAccess];
            if (!string.IsNullOrEmpty(vpVehicleAccess))
            {
                var vpArr = vpVehicleAccess.Split(',');
                if (vpArr.Count() == 1)
                {
                    return module.AllowedMediaDetailLevelItems.Cast<DetailLevelItemInformation>()
                          .Where(p => p.Id != DetailLevelItemInformation.Levels.vehicle).ToList();
                }

            }
            return module.AllowedMediaDetailLevelItems.Cast<DetailLevelItemInformation>().ToList();
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork.Date;

namespace TNS.AdExpress.Web.Controls.Selections.Rolex.Filter
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RolexSelectionDatesWebControl runat=server></{0}:RolexSelectionDatesWebControl>")]
    public class RolexScheduleSelectionDatesWebControl : RolexScheduleSelectionFilterBaseWebControl
    {
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
        protected override string GetAjaxEventScript()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\nvar isChanged_" + this.ID + " = false;");

            js.Append("\r\nfunction onClickDate_" + this.ID + "(date){");
            js.Append("\r\n isChanged_" + this.ID + " = true;");
            js.Append("\r\n}\r\n");

            return js.ToString();
        }
        #endregion

        #region GetValuesSelectedMethodScriptContent
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetValuesSelectedMethodScriptContent()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(isChanged_" + this.ID + " == false) return null;");
            js.Append("\r\n\tvar tab = new Array();");

            js.Append("\r\n\t var oYearBegin = document.getElementById('yearBegin_" + this.ID + "');  ");
            js.Append("\r\n\t var oYearEnd = document.getElementById('yearEnd_" + this.ID + "');  ");
            js.Append("\r\n\t var oMonthBegin = document.getElementById('monthBegin_" + this.ID + "');  ");
            js.Append("\r\n\t var oMonthEnd = document.getElementById('monthEnd_" + this.ID + "');  ");

            js.Append("\r\n\t if(oYearBegin.options[oYearBegin.selectedIndex].value=='none' && oMonthBegin.options[oMonthBegin.selectedIndex].value =='none' ");
            js.Append("\r\n\t && oYearEnd.options[oYearEnd.selectedIndex].value=='none' && oMonthEnd.options[oMonthEnd.selectedIndex].value =='none')");
            js.Append("\r\n\t return null;");

            //Date Begin in push format : yyyy_MM ex. 2011_01
            js.Append("\r\n\t tab.push(oYearBegin.options[oYearBegin.selectedIndex].value + '_' + oMonthBegin.options[oMonthBegin.selectedIndex].value);");

            //Date End in push format : yyyy_MM ex. 2011_02
            js.Append("\r\n\t tab.push(oYearEnd.options[oYearEnd.selectedIndex].value + '_' + oMonthEnd.options[oMonthEnd.selectedIndex].value);");

            js.Append("\r\n\treturn tab;");
            return js.ToString();
        }

        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetInitializeResultMethodContent()
        {
            return base.GetInitializeResultMethodContent() + "isChanged_" + this.ID + " = false;";
        }
        #endregion

        #region Enregistrement des paramètres de construction du résultat
        protected override string SetCurrentResultParametersScript()
        {
            StringBuilder js = new StringBuilder();
            return (base.SetCurrentResultParametersScript() + js.ToString());
        }
        protected override void LoadCurrentResultParameters(AjaxPro.JavaScriptObject o)
        {
            base.LoadCurrentResultParameters(o);
        }
        #endregion

        #region Enregistrement des paramètres pour les styles
        protected override string SetCurrentStyleParametersScript()
        {
            StringBuilder js = new StringBuilder();

            js.Append("\r\n\t obj.CssClassLvl1 = '" + CssClassLvl1 + "';");
            js.Append("\r\n\t obj.CssClassLvl2 = '" + CssClassLvl2 + "';");
            js.Append("\r\n\t obj.CssClassLvl3 = '" + CssClassLvl3 + "';");
            js.Append("\r\n\t obj.CssClassLvl4 = '" + CssClassLvl4 + "';");

            return (base.SetCurrentStyleParametersScript() + js.ToString());
        }

        protected override void LoadCurrentStyleParameters(AjaxPro.JavaScriptObject o)
        {
            base.LoadCurrentStyleParameters(o);
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML()
        {
            StringBuilder html = new StringBuilder(1000);

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);

            html.AppendFormat("<div id=\"div_{0}\" width=\"100%\" class=\"{1}\">", this.ID, CssClass);
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>");

            #region Dates Tab
            html.Append("<table cellspacing=\"0\" class=\"rolexScheduleSelectionDateWebControlPersonnalisationTab\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td  class=\"rolexScheduleSelectionDateWebControlPersonnalisationHeader\"> &nbsp;&nbsp;" + GestionWeb.GetWebWord(2881, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td align=\"center\" >");


            DateTime dateBegin = new DateTime(DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear + 1, 1, 1);
            DateTime dateEnd = DateTime.Now;


            StringBuilder monthsBegin = new StringBuilder(1000);
            monthsBegin.Append("<option value=\"none\">-------------</option>");
            StringBuilder monthsEnd = new StringBuilder(1000);
            monthsEnd.Append("<option value=\"none\">-------------</option>");


            StringBuilder yearsBegin = new StringBuilder(1000);
            yearsBegin.AppendFormat("<option value=\"none\">-------</option>");
            StringBuilder yearsEnd = new StringBuilder(1000);
            yearsEnd.AppendFormat("<option value=\"none\">-------</option>");

            List<int> yearCol = new List<int>();
            while (dateBegin.Year <= dateEnd.Year)
            {
                yearCol.Add(dateEnd.Year);
                dateEnd = dateEnd.AddYears(-1);
            }
            int periodYearBegin = 0, periodYearEnd = 0, periodMonthBegin = 0, periodMonthEnd = 0;
            bool isContainPeriod = false;
            if (_webSession.PeriodType == Constantes.Web.CustomerSessions.Period.Type.personalize)
            {
                periodYearBegin = Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(0, 4));
                periodYearEnd = Convert.ToInt32(_webSession.PeriodEndDate.Substring(0, 4));
                periodMonthBegin = Convert.ToInt32(_webSession.PeriodBeginningDate.Substring(4, 2));
                periodMonthEnd = Convert.ToInt32(_webSession.PeriodEndDate.Substring(4, 2));
                if (yearCol.Contains(periodYearBegin) && yearCol.Contains(periodYearEnd)) isContainPeriod = true;
            }
            for (int j = 0; j < yearCol.Count; j++)
            {
                yearsBegin.AppendFormat("<option value=\"{0}\" " + ((isContainPeriod && yearCol[j] == periodYearBegin) ? "selected = \"selected\"" : string.Empty) + ">{0}</option>", yearCol[j]);
                yearsEnd.AppendFormat("<option value=\"{0}\" " + ((isContainPeriod && yearCol[j] == periodYearEnd) ? "selected = \"selected\"" : string.Empty) + ">{0}</option>", yearCol[j]);
                dateEnd = dateEnd.AddYears(-1);
            }

            for (int i = 1; i <= 12; i++)
            {

                string mv = (i < 10) ? "0" + i.ToString() : i.ToString();
                string ms = MonthString.GetCharacters(i, WebApplicationParameters.AllowedLanguages[_webSession.SiteLanguage].CultureInfo, 11);
                monthsBegin.AppendFormat("<option value=\"{0}\" " + ((isContainPeriod && i == periodMonthBegin) ? "selected = \"selected\"" : string.Empty) + ">{1}</option>", mv, ms);
                monthsEnd.AppendFormat("<option value=\"{0}\" " + ((isContainPeriod && i == periodMonthEnd) ? "selected = \"selected\"" : string.Empty) + ">{1}</option>", mv, ms);
            }

            html.Append("<table cellspacing=\"3\" cellpadding=\"0\" border=\"0\" class=\"rolexScheduleSelectionDateWebControlPersonnalisationHeaderBis\">");

            //Dates beginning
            html.Append("<tr class=\"rolexScheduleSelectionDateWebControlPersonnalisationHeaderBis\" ><td class=\"rolexScheduleSelectionDateText\">" + GestionWeb.GetWebWord(1793, _webSession.SiteLanguage) + ": </td>");
            html.Append("<td><select id=\"monthBegin_" + this.ID + "\" onchange=\"javascript:onClickDate_" + this.ID + "();\">");
            html.Append(monthsBegin.ToString());
            html.Append("</select></td>");
            html.Append("<td><select id=\"yearBegin_" + this.ID + "\" onchange=\"javascript:onClickDate_" + this.ID + "();\">");
            html.Append(yearsBegin.ToString());
            html.Append("</select></td><tr>");
            html.Append("<tr><td colspan=\"3\">&nbsp;</td></tr>");

            //Dates end       
            html.Append("<tr class=\"rolexScheduleSelectionDateWebControlPersonnalisationHeaderBis\"><td class=\"rolexScheduleSelectionDateText\" >" + GestionWeb.GetWebWord(1794, _webSession.SiteLanguage) + ": </td>");
            html.Append("<td><select id=\"monthEnd_" + this.ID + "\" onchange=\"javascript:onClickDate_" + this.ID + "();\">");
            html.Append(monthsEnd.ToString());
            html.Append("</select></td>");
            html.Append("<td><select id=\"yearEnd_" + this.ID + "\" onchange=\"javascript:onClickDate_" + this.ID + "();\">");
            html.Append(yearsEnd.ToString());
            html.Append("</select></td><tr>");
            html.Append("<tr><td colspan=\"3\"></td></tr>");
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

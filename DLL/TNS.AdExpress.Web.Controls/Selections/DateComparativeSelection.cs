using System;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Controls.Selections
{
    /// <summary>
    /// Web control used to handle dependent selection (allows a checkbox list to be selectionable only if the reference one is checked)
    /// </summary>
    [ToolboxData("<{0}:CheckBoxsDependentSelection runat=server></{0}:CheckBoxsDependentSelection>")]
    public class DateComparativeSelection : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Use Date Type Comparative
        /// </summary>
        private bool _useDateTypeComparative = false;
        /// <summary>
        /// Use Date Type Disponibility Data
        /// </summary>
        private bool _useDateTypeDisponibilityData = false;
        /// <summary>
        /// Web Session
        /// </summary>
        private WebSession _webSession = null;
        /// <summary>
        /// Web Session
        /// </summary>
        private string _javascriptFunctionOnValidate = null;
        /// <summary>
        /// Period Comparative Type List Availaible
        /// </summary>
        private List<WebConstantes.globalCalendar.comparativePeriodType> _periodComparativeTypeListAvailaible;
        #endregion

        #region Assessor
        /// <summary>
        /// Get Javascript Function Name OnDisplay
        /// </summary>
        public string JavascriptFunctionOnDisplay {
            get { return "OnDisplay_" + this.ID + "();"; }
        }

        /// <summary>
        /// Set Javascript Function Name OnValidate
        /// </summary>
        public string JavascriptFunctionOnValidate {
            set { _javascriptFunctionOnValidate = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession"></param>
        /// <param name="useDateTypeComparative">Use Date Type Comparative</param>
        /// <param name="useDateTypeDisponibilityData">Use Date Type Disponibility Data</param>
        public DateComparativeSelection(WebSession webSession, bool useDateTypeComparative, bool useDateTypeDisponibilityData)
            : base() 
        {
            this.EnableViewState = true;
            _useDateTypeComparative = useDateTypeComparative;
            _useDateTypeDisponibilityData = useDateTypeDisponibilityData;
            _webSession = webSession;
        }
        #endregion

        #region Events

        #region Init
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {
            _periodComparativeTypeListAvailaible = new List<TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType>();
            switch (_webSession.PeriodType) {
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.nLastWeek:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.nLastDays:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousDay:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDate:
                    _periodComparativeTypeListAvailaible.Add(TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate);
                    _periodComparativeTypeListAvailaible.Add(TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate);
                    break;
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.nLastMonth:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.currentYear:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousYear:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousMonth:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.previousWeek:
                    _periodComparativeTypeListAvailaible.Add(TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate);
                    break;
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateWeek:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.LastLoadedWeek:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.dateToDateMonth:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.nextToLastYear:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.nLastYear:
                case TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.LastLoadedMonth:
                default:
                    _periodComparativeTypeListAvailaible = new List<TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType>();
                    break;
            }
           
        }
        #endregion

        #region Load
        /// <summary>
        /// launched when the control is loaded
        /// </summary>
        /// <param name="e">arguments</param>
        protected override void OnLoad(EventArgs e)
        {
            if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("jquery"))
                this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "jquery", this.Page.ResolveClientUrl("~/scripts/jquery.js"));
            if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("thickbox"))
                this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "thickbox", this.Page.ResolveClientUrl("~/scripts/thickbox.js"));

            base.OnLoad(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            StringBuilder html = new StringBuilder();

            #region Javascript
            html.Append("\n<script language=\"JavaScript\" type=\"text/JavaScript\">\n");

            html.Append("\nfunction OnDisplay_" + this.ID + "(){");
            if (_periodComparativeTypeListAvailaible.Count > 1)
               html.Append("tb_show('" + GestionWeb.GetWebWord(2370, _webSession.SiteLanguage) + "', '#TB_inline?height=145&width=400&inlineId=dateComparativeSelection_" + this.ID + "&caption=" + GestionWeb.GetWebWord(2371, _webSession.SiteLanguage) + "&label=" + GestionWeb.GetWebWord(2372, _webSession.SiteLanguage) + "+', '');");
            else if (_useDateTypeComparative) {
                if (_periodComparativeTypeListAvailaible.Contains(TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate))
                    html.Append("document.getElementById('selectionType_" + this.ID + "').value = \"" + WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate + "\"; ");
                else
                    html.Append("document.getElementById('selectionType_" + this.ID + "').value = \"" + WebConstantes.globalCalendar.comparativePeriodType.dateToDate + "\"; ");
                html.Append(_javascriptFunctionOnValidate);
            }
            html.Append("\n}");

            html.Append("\nfunction OnSelectedRadio_"+this.ID+"(i){");
			html.Append("\n\tswitch(i){");
            if (_useDateTypeComparative) {

                foreach (WebConstantes.globalCalendar.comparativePeriodType cCompPeriodType in _periodComparativeTypeListAvailaible) {
                    html.Append("\n\t\tcase '" + cCompPeriodType + "':");
                    foreach (WebConstantes.globalCalendar.comparativePeriodType cCompPeriodType2 in _periodComparativeTypeListAvailaible) {
                        html.Append("\n\t\t\tdocument.getElementById('" + cCompPeriodType2 + "_" + this.ID + "').checked=" + (cCompPeriodType2==cCompPeriodType).ToString().ToLower() + ";");
                    }
                    html.Append("\n\t\t\tdocument.getElementById('selectionType_" + this.ID + "').value = \"" + cCompPeriodType + "\"; ");
                    html.Append("\n\t\t\tbreak;");
                }
            }
            if (_useDateTypeDisponibilityData) {
                html.Append("\n\t\tcase '" + WebConstantes.globalCalendar.periodDisponibilityType.currentDay + "':");
                html.Append("\n\t\t\tlastPeriod_"+this.ID+".checked=false;");
                html.Append("\n\t\t\tlastDay_"+this.ID+".checked=true;");
                html.Append("\n\t\t\tdocument.getElementById('disponibilityType_"+this.ID+"').value = \"" + WebConstantes.globalCalendar.periodDisponibilityType.currentDay + "\"; ");
                html.Append("\n\t\t\tbreak;");
                html.Append("\n\t\tcase '" + WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod + "':");
                html.Append("\n\t\t\tlastPeriod_"+this.ID+".checked=true;");
                html.Append("\n\t\t\tlastDay_"+this.ID+".checked=false;");
                html.Append("\n\t\t\tdocument.getElementById('disponibilityType_"+this.ID+"').value = \"" + WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod + "\"; ");
                html.Append("\n\t\t\tbreak;	");
            }
            html.Append("\n\t\tdefault:");
            html.Append("\n\t\tbreak;");
			html.Append("\n\t}\n");
            html.Append("\n}\n");
            html.Append("\n</script>\n");
            #endregion

            #region HTML
            //'#TB_inline?height=145&width=400&inlineId=myOnPageContent&caption=" + GestionWeb.GetWebWord(2371, _webSession.SiteLanguage) + "&label=" + GestionWeb.GetWebWord(2372, _webSession.SiteLanguage) + "+'
            html.Append(" <input style=\"visibility:hidden\" type=\"hidden\" id=\"comparativeLink_" + this.ID + "\" alt=\"#TB_inline?height=200&width=400&inlineId=dateComparativeSelection_" + this.ID + "\" title=\"Sélection comparative\" class=\"thickbox\" type=\"button\" value=\"Show\" />");   
            html.Append("<input id=\"selectionType_"+this.ID+"\" type=\"hidden\" value=\"\" name=\"selectionType_"+this.ID+"\"/>");
			html.Append("<input id=\"disponibilityType_"+this.ID+"\" type=\"hidden\" value=\"\" name=\"disponibilityType_"+this.ID+"\"/>");

            html.Append("<div style=\"display:none;\"  id=\"dateComparativeSelection_"+this.ID+"\" >");

            html.Append("\n\t<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
            html.Append("\n\t\t<tr>");
            html.Append("\n\t\t\t<td>&nbsp;</td>");
            html.Append("\n\t\t</tr>");
            html.Append("\n\t\t<tr>");
            html.Append("\n\t\t\t<td>");

            #region Date Type Selection
            if (_useDateTypeComparative) {

                html.Append("\n\t\t\t\t<div id=\"comparativeDiv\">");
                html.Append("\n\t\t\t\t<table id=\"comparativeTable_"+this.ID+"\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
                html.Append("\n\t\t\t\t<tr>");
                html.Append("\n\t\t\t\t\t<td class=\"txtGris11Bold\" colspan=\"2\">" + GestionWeb.GetWebWord(2293, _webSession.SiteLanguage) + "</td>");
                html.Append("\n\t\t\t\t\t</td>");
                html.Append("\n\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t<tr>");
                html.Append("\n\t\t\t\t\t<td>");
                html.Append("\n\t\t\t\t\t\t&nbsp;<input id=\""+WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate+"_"+this.ID+"\" type=\"radio\" onClick=\"javascript:OnSelectedRadio_"+this.ID+"('" + WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate + "');\" "+((_webSession.ComparativePeriodType==TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.comparativeWeekDate)?" checked=\"checked\" ":"")+">");
                html.Append("\n\t\t\t\t\t\t\t<font class=\"txtNoir11\">" + GestionWeb.GetWebWord(2295, _webSession.SiteLanguage) + "</font>");
                html.Append("\n\t\t\t\t\t\t\t</input>");
                html.Append("\n\t\t\t\t\t</td>");
                html.Append("\n\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t<tr>");
                html.Append("\n\t\t\t\t\t<td>");
                html.Append("\n\t\t\t\t\t\t&nbsp;<input id=\""+WebConstantes.globalCalendar.comparativePeriodType.dateToDate+"_" + this.ID + "\" type=\"radio\" onClick=\"javascript:OnSelectedRadio_" + this.ID + "('" + WebConstantes.globalCalendar.comparativePeriodType.dateToDate + "');\" " + ((_webSession.ComparativePeriodType == TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate) ? " checked=\"checked\" " : "") + ">");
                html.Append("\n\t\t\t\t\t\t\t\t<font class=\"txtNoir11\">" + GestionWeb.GetWebWord(2294, _webSession.SiteLanguage) + "</font>");
                html.Append("\n\t\t\t\t\t\t</input>");
                html.Append("\n\t\t\t\t\t</td>");
                html.Append("\n\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t</table>");
                html.Append("\n\t\t\t\t</div>");
            }
            #endregion

            if (_useDateTypeComparative && _useDateTypeDisponibilityData)
                html.Append("\n\t\t\t\t<div id=\"espaceDiv1\"><br/></div>");

            #region Date Disponibility Selection
            if (_useDateTypeDisponibilityData) {
                html.Append("\n\t\t\t\t<div id=\"disponibilityDiv\">");
                html.Append("\n\t\t\t\t<table id=\"disponibilityTable_"+this.ID+"\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
                html.Append("\n\t\t\t\t\t<tr>  ");
                html.Append("\n\t\t\t\t\t\t<td class=\"txtGris11Bold\" colspan=\"2\">" + GestionWeb.GetWebWord(2296, _webSession.SiteLanguage) + "</td>");
                html.Append("\n\t\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t\t<tr>");
                html.Append("\n\t\t\t\t\t\t<td>");
                html.Append("\n\t\t\t\t\t\t\t&nbsp;<input id=\"lastDay_"+this.ID+"\" type=\"radio\" onClick=\"javascript:OnSelectedRadio_"+this.ID+"('" + WebConstantes.globalCalendar.periodDisponibilityType.currentDay + "');\" checked=\"checked\">");
                html.Append("\n\t\t\t\t\t\t\t\t<font class=\"txtNoir11\">" + GestionWeb.GetWebWord(2297, _webSession.SiteLanguage) + "</font>");
                html.Append("\n\t\t\t\t\t\t\t</input>");
                html.Append("\n\t\t\t\t\t\t</td>");
                html.Append("\n\t\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t\t<tr>");
                html.Append("\n\t\t\t\t\t\t<td>");
                html.Append("\n\t\t\t\t\t\t\t&nbsp;<input id=\"lastPeriod_"+this.ID+"\" type=\"radio\" onClick=\"javascript:OnSelectedRadio_"+this.ID+"('" + WebConstantes.globalCalendar.periodDisponibilityType.lastCompletePeriod + "');\">");
                html.Append("\n\t\t\t\t\t\t\t\t<font class=\"txtNoir11\">" + GestionWeb.GetWebWord(2298, _webSession.SiteLanguage) + "</font>");
                html.Append("\n\t\t\t\t\t\t\t</input>");
                html.Append("\n\t\t\t\t\t\t</td>");
                html.Append("\n\t\t\t\t\t</tr>");
                html.Append("\n\t\t\t\t</table>");
                html.Append("\n\t\t\t\t</div>");
            }
            #endregion

            html.Append("\n\t\t\t\t<div id=\"espaceDiv2\"  style=\"display:none;\"><br/></div>");

            html.Append("\n\t\t\t</td>");
            html.Append("\n\t\t</tr>");
            html.Append("\n\t\t<tr>");
            html.Append("\n\t\t\t<td>&nbsp;</td>");
            html.Append("\n\t\t</tr>");
            html.Append("\n\t\t<tr>");
            html.Append("\n\t\t\t<td>");
            html.Append("\n\t\t\t\t<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\">");
            html.Append("\n\t\t\t\t\t<tr>");
            html.Append("\n\t\t\t\t\t\t<td>        ");
            html.Append("\n\t\t\t\t\t\t\t<img align=right id=\"selectionType_" + this.ID + "Button\" src=\"/App_Themes/" + this.Page.Theme + "/Images/Culture/button/valider_up.gif\" onmouseover=\"selectionType_" + this.ID + "Button.src='/App_Themes/" + this.Page.Theme + "/Images/Culture/button/valider_down.gif';\" onmouseout=\"selectionType_" + this.ID + "Button.src='/App_Themes/" + this.Page.Theme + "/Images/Culture/button/valider_up.gif';\" onclick=\"if(document.getElementById('"+WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate+"_" + this.ID + "').checked==true){document.getElementById('selectionType_" + this.ID + "').value = '" + WebConstantes.globalCalendar.comparativePeriodType.comparativeWeekDate + "';}else if(document.getElementById('"+WebConstantes.globalCalendar.comparativePeriodType.dateToDate+"_" + this.ID + "').checked==true){document.getElementById('selectionType_" + this.ID + "').value = '" + WebConstantes.globalCalendar.comparativePeriodType.dateToDate + "';} tb_remove();" + ((_javascriptFunctionOnValidate != null && _javascriptFunctionOnValidate.Length > 0) ? _javascriptFunctionOnValidate : string.Empty) + "\" style=\"cursor:pointer\"/>");
            html.Append("\n\t\t\t\t\t\t</td>");
            html.Append("\n\t\t\t\t\t</tr>");
            html.Append("\n\t\t\t\t</table>");
            html.Append("\n\t\t\t</td>");
            html.Append("\n\t\t</tr>");
            html.Append("\n\t</table>");

            html.Append("</div>");
            #endregion

            output.Write(html.ToString());
        }
        #endregion

        #endregion

    }
}

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
using TNS.AdExpress.Constantes.Web;
namespace TNS.AdExpress.Web.Controls.Selections.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleResultBaseWebControl runat=server></{0}:VpScheduleResultBaseWebControl>")]
    public class VpScheduleSelectionDropDownDateListWebControl : VpScheduleAjaxSelectionBaseWebControl {

        #region AjaxEventScript
        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript() {
            StringBuilder js = new StringBuilder(1000);

            js.Append("\r\nvar oldDateSelection_" + this.ID + " = '" + _webSession.PeriodType.ToString() + "';");

            js.Append("\r\nfunction setDate_" + this.ID + "(){");
            js.Append("\r\n\tif((document.getElementById('dateSelection_" + this.ID + "').options[document.getElementById('dateSelection_" + this.ID + "').selectedIndex].value!='" + CustomerSessions.Period.Type.personalize + "' && oldDateSelection_" + this.ID + " == '" + CustomerSessions.Period.Type.personalize + "') || (document.getElementById('dateSelection_" + this.ID + "').options[document.getElementById('dateSelection_" + this.ID + "').selectedIndex].value=='" + CustomerSessions.Period.Type.personalize + "' && oldDateSelection_" + this.ID + " != '" + CustomerSessions.Period.Type.personalize + "'))");
            js.Append("\r\n\t isLoaded_VpScheduleContainerWebControl1ResultFilter_Dates = false;");
            js.Append("\r\noldDateSelection_" + this.ID + " = document.getElementById('dateSelection_" + this.ID + "').options[document.getElementById('dateSelection_" + this.ID + "').selectedIndex].value;");
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".SetDate('" + this._webSession.IdSession + "', document.getElementById('dateSelection_" + this.ID + "').options[document.getElementById('dateSelection_" + this.ID + "').selectedIndex].value, document.getElementById('datePersonnalize" + this.ID + "').value, resultParameters_" + this.ID + ",styleParameters_" + this.ID + ",setDate_" + this.ID + "_callback);");
           
            js.Append("\r\n}");

            js.Append("\r\nfunction setDate_" + this.ID + "_callback(res){");
            js.Append("if (res.error != null && res.error.Message) { alert(res.error.Message); }");
            js.Append("else {");
            js.Append(ValidationMethod);
            js.Append("}");

            js.Append("\r\n}\r\n");
            return js.ToString();
        }
        #endregion   

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
            InitializeResultToLoad = false;
        }
        #endregion

        #endregion

        #region SetDate
        /// <summary>
        /// Set Date in WebSession
        /// </summary>
        /// <param name="o">Result parameters (session Id, theme...)</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public void SetDate(string idSession, string periodType, string datePersonalize, AjaxPro.JavaScriptObject resultParameters, AjaxPro.JavaScriptObject styleParameters) {

            try {
                _webSession = (WebSession)WebSession.Load(idSession);
                TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodTypes = (TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type)Enum.Parse(typeof(TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type), periodType);
                if (periodTypes != TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.personalize) {
                    _webSession.SetVpDates(periodTypes);
                }
                else {
                    _webSession.SetVpDates(TNS.FrameWork.Date.DateString.YYYYMMDDToDateTime(datePersonalize.Split('_')[0])
                        , TNS.FrameWork.Date.DateString.YYYYMMDDToDateTime(datePersonalize.Split('_')[1]));
                }
                _webSession.Save();
            }
            catch (System.Exception err) {
                throw new Exception (OnAjaxMethodError(err, _webSession), err);
            }

        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML() {
            StringBuilder html = new StringBuilder(1000);
            if (_webSession.PeriodType == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.personalize) {
                html.Append("<input type=\"hidden\" id=\"datePersonnalize" + this.ID + "\" value=\"" + _webSession.PeriodBeginningDate + "_" + _webSession.PeriodEndDate + "\"/>");
            }
            else {
                html.Append("<input type=\"hidden\" id=\"datePersonnalize" + this.ID + "\" value=\"\"/>");
            }
            html.Append("<select id=\"dateSelection_" + this.ID + "\" onchange=\"javascript:setDate_" + this.ID + "();\" >");
            foreach (VpDateConfiguration cVpDateConfiguration in WebApplicationParameters.VpDateConfigurations.VpDateConfigurationList) {
                html.Append("<option value=\"" + cVpDateConfiguration.DateType.ToString() + "\" ");
                if (_webSession.PeriodType == cVpDateConfiguration.DateType)
                    html.Append("selected = \"selected\"");
                html.Append(">" + GestionWeb.GetWebWord(cVpDateConfiguration.TextId, _webSession.SiteLanguage) + "</option>");
            }
            if (_webSession.PeriodType == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type.personalize) {

                html.Append("<option value=\"" + _webSession.PeriodType.ToString() + "\" selected = \"selected\">" + TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodBeginningDate, _webSession.SiteLanguage) + " - " + TNS.FrameWork.Date.DateString.YYYYMMDDToDD_MM_YYYY(_webSession.PeriodEndDate, _webSession.SiteLanguage) + "</option>");
            }
            html.Append("</select>");
            return (html.ToString());
        }
        #endregion

        #region GetAjaxInitialisationHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxInitialisationHTML() {
            return ("<select id=\"dateSelection_"+this.ID+"\" onchange=\"javascript:setDate_" + this.ID + "();\" ></select>");
        }
        #endregion

        #region GetHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetHTML() {
            return GetAjaxHTML();
        }
        #endregion

    }
}


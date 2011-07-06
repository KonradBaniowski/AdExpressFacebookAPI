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
    public class VpScheduleSelectionDatesWebControl : VpScheduleSelectionFilterBaseWebControl {

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

            //Date Begin in push format : yyyy_MM
            js.Append("\r\n\ttab.push('2011_01');");

            //Date End in push format : yyyy_MM
            js.Append("\r\n\ttab.push('2011_02');");
 
            js.Append("\r\n\treturn tab;");
            return js.ToString();
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

            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);

            html.AppendFormat("<div id=\"div_{0}\" width=\"100%\" class=\"{1}\">", this.ID, CssClass);
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>");

            #region Detail Tab
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(2881, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td align=\"center\">");

            DateTime dateBegin = new DateTime(DateTime.Now.Year - WebApplicationParameters.DataNumberOfYear + 1, 1, 1);
            

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(1793, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td></td></tr>");
            html.Append("<tr><td></td></tr>");
            html.Append("<tr><td>" + GestionWeb.GetWebWord(1794, _webSession.SiteLanguage) + " :</td></tr>");
            html.Append("<tr><td></td></tr>");
            html.Append("<tr><td></td></tr>");
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


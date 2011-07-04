#region Informations
// Auteur: G. Facon 
// Date de création: 13/07/2006
// Date de modification:
//		G Ragneau - 08/08/2006 - Set GetHtml as public so as to access it 
//		G Ragneau - 08/08/2006 - GetHTML : Force media plan alert module and restaure it after process (<== because of version zoom);
//		G Ragneau - 05/05/2008 - GetHTML : implement layers
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.ComponentModel;
using System.Reflection;
using AjaxPro;
using TNS.AdExpress.Web.Controls.Headers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Common.Results;
using TNS.AdExpress.Web.UI.Results.MediaPlanVersions;
using WebFunctions = TNS.AdExpress.Web.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using FrmFct = TNS.FrameWork.WebResultUI.Functions;
using TNS.FrameWork.Date;
using TNS.FrameWork.Exceptions;
using TNS.FrameWork.WebResultUI;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using CustomCst = TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;

using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Insertions;
using TNS.AdExpressI.VP;
using System.Web.UI.HtmlControls;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleResultFilterWebControl runat=server></{0}:VpScheduleResultFilterWebControl>")]
    public class VpScheduleResultFilterWebControl : VpScheduleResultBaseWebControl {

        #region Variables
        /// <summary>
        /// Filter Result Web Control List
        /// </summary>
        Dictionary<Int64, WebControl> _filterResultWebControlList = new Dictionary<long, WebControl>();
        #endregion

        #region GetJavascript
        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected override string GetValidationJavascriptContent() {
            return base.GetValidationJavascriptContent();
        }
        /// <summary>
        /// Get Display Javascript Method
        /// </summary>
        /// <returns>Display Javascript Method</returns>
        protected override string GetDisplayJavascriptContent() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(display) {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.height=document.body.clientHeight + \"px\";");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = '';");
            //js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.top = (document.documentElement.scrollTop + ((document.documentElement.clientHeight - 550) / 2)) + \"px\";");
            //js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.left = (document.documentElement.scrollLeft + ((document.documentElement.clientWidth - 750) / 2)) + \"px\";");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.top = \"0px\";");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.left = \"0px\";");

            js.Append("\r\n\t} else {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t}");
            return (base.GetDisplayJavascriptContent() + js.ToString());
        }
        /// <summary>
        /// Get Initialize Javascript Method
        /// </summary>
        /// <returns>Initialize Javascript Method</returns>
        protected override string GetInitializeJavascriptContent() {
            return base.GetInitializeJavascriptContent();
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
          
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e) {
            Page.Response.Write("<div id=\"res_backgroud_" + this.ID + "\" class=\"vpScheduleResultFilterWebControlBackgroud\" style=\"display:none;\"></div>");

            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            base.Render(output);
        }
        #endregion

        #endregion

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetHTML() {
            StringBuilder html = new StringBuilder(1000);
            
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"1\" align=\"center\" width=\"100%\" height=\"100%\">");
            html.Append("<tr>");
            html.Append("<td width=\"100px;\">");

            #region Menu
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"1\" align=\"center\" width=\"100%\" height=\"100%\">");
            html.Append("<tr><td>");
            html.Append(GestionWeb.GetWebWord(2863, _webSession.SiteLanguage));
            html.Append("</td></tr>");
            html.Append("<tr><td>");
            html.Append("<ul>");
            //foreach(
            html.Append("</ul>");
            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td>");
            html.Append("<td>");

            #region Result

            #endregion

            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            return html.ToString();
        }
        #endregion

    }
}


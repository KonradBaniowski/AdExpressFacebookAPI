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
using TNS.AdExpress.Web.Controls.Selections.VP;
using TNS.AdExpress.Domain.Layers;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleContainerWebControl runat=server></{0}:VpScheduleContainerWebControl>")]
    public class VpScheduleContainerWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// Session client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Result Control List
        /// </summary>
        List<VpScheduleResultBaseWebControl> _vpScheduleResultWebControlList = null;
        /// <summary>
        /// Selection Web Control List
        /// </summary>
        List<VpScheduleSelectionBaseWebControl> _vpScheduleSelectionBaseWebControlList = null;
        #endregion

        #region Accesors

        #region WebSession
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession WebSession {
            get { return (_webSession); }
            set {
                _webSession = value;
            }
        }
        #endregion

        #endregion

        #region JavaScript
        /// <summary>
        /// Evenement Javascript
        /// </summary>
        /// <returns></returns>
        protected string GetJavaScript() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");

            #region RefreshVpScheduleResultWebControl
            js.Append("\r\nfunction RefreshVpScheduleResultWebControl(controlId){");
            if (_vpScheduleResultWebControlList.Count > 0) {
                for (int i=0; i< _vpScheduleResultWebControlList.Count; i++) {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _vpScheduleResultWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t"+ _vpScheduleResultWebControlList[i].ValidationMethodName+"();");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region DisplayVpScheduleResultWebControl
            js.Append("\r\nfunction DisplayVpScheduleResultWebControl(controlId){");
            if (_vpScheduleResultWebControlList.Count > 0) {
                for (int i = 0; i < _vpScheduleResultWebControlList.Count; i++) {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _vpScheduleResultWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t" + _vpScheduleResultWebControlList[i].DisplayMethod + "(true);");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region RefreshVpScheduleSelectionWebControl
            js.Append("\r\nfunction RefreshVpScheduleSelectionWebControl(controlId){");
            if (_vpScheduleSelectionBaseWebControlList.Count > 0)
            {
                for (int i = 0; i < _vpScheduleSelectionBaseWebControlList.Count; i++)
                {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _vpScheduleSelectionBaseWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t" + _vpScheduleSelectionBaseWebControlList[i].ValidationMethodName + "();");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region AskResultExportExcel
            js.Append("\r\nfunction AskResultExportExcel(){");
            js.AppendFormat("\r\n\t popupOpenBis('/Private/MyAdExpress/APPMExcelSavePopUp.aspx?idSession={0}','470','210','yes');",_webSession.IdSession);
              js.Append("\r\n}");
            #endregion

              #region AskResultExportPDF
              js.Append("\r\nfunction AskResultExportPDF(exportType){");
              js.Append("\r\n if(exportType=='schedule') ");
              js.AppendFormat("\r\n\t popupOpenBis('/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&resultType={1}','470','210','yes');", _webSession.IdSession, TNS.AdExpress.Anubis.Constantes.Result.type.thoueris.GetHashCode());
              js.Append("\r\n else if(exportType=='promofiles')");
              js.AppendFormat("\r\n\t popupOpenBis('/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&resultType={1}','470','210','yes');", _webSession.IdSession, TNS.AdExpress.Anubis.Constantes.Result.type.selket.GetHashCode());
              js.Append("\r\n}");
              #endregion

              #region SaveResult
              js.Append("\r\nfunction SaveResult(){");
              js.AppendFormat("\r\n\t popupOpenBis('/Private/MyAdExpress/MySessionSavePopUp.aspx?idSession={0}&param={0}{1}','470','270','no');", _webSession.IdSession, DateTime.Now.Hour.ToString() + DateTime.Now.Second.ToString() + DateTime.Now.Millisecond.ToString());
              js.Append("\r\n}");
              #endregion

            js.Append("\r\n-->\r\n</script>");
            return (js.ToString());
        }
        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            _vpScheduleResultWebControlList = new List<VpScheduleResultBaseWebControl>();
            foreach (ControlLayer cControlLayer in WebApplicationParameters.VpConfigurationDetail.ResultControlLayerList) {
                _vpScheduleResultWebControlList.Add((VpScheduleResultBaseWebControl)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cControlLayer.AssemblyName, cControlLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null));
                _vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1].SkinID = cControlLayer.SkinId;
                _vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1].Display = cControlLayer.Display;
                _vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1].ID = this.ID + cControlLayer.ControlId;
                _vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1].CurrentControlDetail = cControlLayer;
                _vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1].ValidationMethod = cControlLayer.ValidationMethod;
                this.Controls.Add(_vpScheduleResultWebControlList[_vpScheduleResultWebControlList.Count - 1]);
            }

            _vpScheduleSelectionBaseWebControlList = new List<VpScheduleSelectionBaseWebControl>();
            foreach (ControlLayer cControlLayer in WebApplicationParameters.VpConfigurationDetail.SelectionControlLayerList) {
                _vpScheduleSelectionBaseWebControlList.Add((VpScheduleSelectionBaseWebControl)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cControlLayer.AssemblyName, cControlLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null, null));
                _vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1].SkinID = cControlLayer.SkinId;
                _vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1].ID = this.ID + cControlLayer.ControlId;
                _vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1].TextId = cControlLayer.TextId;
                _vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1].CurrentControlDetail = cControlLayer;
                _vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1].ValidationMethod = cControlLayer.ValidationMethod;
                this.Controls.Add(_vpScheduleSelectionBaseWebControlList[_vpScheduleSelectionBaseWebControlList.Count - 1]);
            }
        }
        #endregion

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e)
        {

            foreach (VpScheduleResultBaseWebControl cVpScheduleResultBaseWebControl in _vpScheduleResultWebControlList) {
                cVpScheduleResultBaseWebControl.WebSession = this._webSession;
            }

            foreach (VpScheduleSelectionBaseWebControl cVpScheduleSelectionBaseWebControl in _vpScheduleSelectionBaseWebControlList) {
                cVpScheduleSelectionBaseWebControl.WebSession = this._webSession;
            }
            base.OnLoad(e);
        }
        #endregion

        #region PréRender
        /// <summary>
        /// Prérendu
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            output.Write(GetJavaScript());

            output.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" "+(string.IsNullOrEmpty(CssClass)?string.Empty:"class=\""+CssClass+"\"")+" >");
            if (_vpScheduleSelectionBaseWebControlList.Count > 0) {
                output.Write("<tr>");
                output.Write("<td>");
                output.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                output.Write("<tr>");
                foreach (VpScheduleSelectionBaseWebControl cVpScheduleSelectionBaseWebControl in _vpScheduleSelectionBaseWebControlList) {
                    output.Write("<td>");
                    cVpScheduleSelectionBaseWebControl.RenderControl(output);
                    output.Write("</td>");
                }
                output.Write("</tr>");
                output.Write("</table>");
                output.Write("</td>");
                output.Write("</tr>");
            }
            if (_vpScheduleResultWebControlList.Count > 0) {
                foreach (VpScheduleResultBaseWebControl cVpScheduleResultBaseWebControl in _vpScheduleResultWebControlList) {
                    output.Write("<tr>");
                    output.Write("<td>");
                    cVpScheduleResultBaseWebControl.RenderControl(output);
                    output.Write("</td>");
                    output.Write("</tr>");
                }
            }
            output.Write("</table>");
        }
        #endregion

        #endregion

    }
}


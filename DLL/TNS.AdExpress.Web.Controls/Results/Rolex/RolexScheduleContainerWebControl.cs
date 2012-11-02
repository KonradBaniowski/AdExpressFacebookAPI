using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Controls.Selections;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Results.Rolex
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RolexContainerWebControl runat=server></{0}:RolexContainerWebControl>")]
    public class RolexScheduleContainerWebControl : WebControl
    {
        #region Variables
        /// <summary>
        /// Session client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Result Control List
        /// </summary>
        List<ResultBaseWebControl> _rolexResultWebControlList = null;
        /// <summary>
        /// Selection Web Control List
        /// </summary>
        List<SelectionBaseWebControl> _rolexSelectionBaseWebControlList = null;
        #endregion

        #region Accesors

        #region WebSession
        /// <summary>
        /// Obtient ou définit la Sesion du client
        /// </summary>
        [Bindable(false)]
        public WebSession WebSession
        {
            get { return (_webSession); }
            set
            {
                _webSession = value;
            }
        }
        #endregion

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

            _rolexResultWebControlList = new List<ResultBaseWebControl>();
            foreach (ControlLayer cControlLayer in WebApplicationParameters.RolexConfigurationDetail.ResultControlLayerList)
            {
                _rolexResultWebControlList.Add((ResultBaseWebControl)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cControlLayer.AssemblyName, cControlLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null));
                _rolexResultWebControlList[_rolexResultWebControlList.Count - 1].SkinID = cControlLayer.SkinId;
                _rolexResultWebControlList[_rolexResultWebControlList.Count - 1].Display = cControlLayer.Display;
                _rolexResultWebControlList[_rolexResultWebControlList.Count - 1].ID = this.ID + cControlLayer.ControlId;
                _rolexResultWebControlList[_rolexResultWebControlList.Count - 1].CurrentControlDetail = cControlLayer;
                _rolexResultWebControlList[_rolexResultWebControlList.Count - 1].ValidationMethod = cControlLayer.ValidationMethod;
                this.Controls.Add(_rolexResultWebControlList[_rolexResultWebControlList.Count - 1]);
            }

            _rolexSelectionBaseWebControlList = new List<SelectionBaseWebControl>();
            foreach (ControlLayer cControlLayer in WebApplicationParameters.RolexConfigurationDetail.SelectionControlLayerList)
            {
                _rolexSelectionBaseWebControlList.Add((SelectionBaseWebControl)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cControlLayer.AssemblyName, cControlLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null));
                _rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1].SkinID = cControlLayer.SkinId;
                _rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1].ID = this.ID + cControlLayer.ControlId;
                _rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1].TextId = cControlLayer.TextId;
                _rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1].CurrentControlDetail = cControlLayer;
                _rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1].ValidationMethod = cControlLayer.ValidationMethod;
                this.Controls.Add(_rolexSelectionBaseWebControlList[_rolexSelectionBaseWebControlList.Count - 1]);
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

            foreach (var cResultBaseWebControl in _rolexResultWebControlList)
            {
                cResultBaseWebControl.WebSession = this._webSession;
            }

            foreach (var cSelectionBaseWebControl in _rolexSelectionBaseWebControlList)
            {
                cSelectionBaseWebControl.WebSession = this._webSession;
            }
            base.OnLoad(e);
        }
        #endregion

      

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write(GetJavaScript());

            output.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" " + (string.IsNullOrEmpty(CssClass) ? string.Empty : "class=\"" + CssClass + "\"") + " >");
            if (_rolexSelectionBaseWebControlList.Count > 0)
            {
                output.Write("<tr>");
                output.Write("<td>");
                output.Write("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\">");
                output.Write("<tr>");
                foreach (var cSelectionBaseWebControl in _rolexSelectionBaseWebControlList)
                {
                    output.Write("<td>");
                    cSelectionBaseWebControl.RenderControl(output);
                    output.Write("</td>");
                }
                output.Write("</tr>");
                output.Write("</table>");
                output.Write("</td>");
                output.Write("</tr>");
            }
            if (_rolexResultWebControlList.Count > 0)
            {
                foreach (var cResultBaseWebControl in _rolexResultWebControlList)
                {
                    output.Write("<tr>");
                    output.Write("<td>");
                    cResultBaseWebControl.RenderControl(output);
                    output.Write("</td>");
                    output.Write("</tr>");
                }
            }
            output.Write("</table>");
        }
        #endregion

        #endregion

        #region JavaScript
        /// <summary>
        /// Evenement Javascript
        /// </summary>
        /// <returns></returns>
        protected string GetJavaScript()
        {
            var js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");

            #region RefreshRolexSchedulResultWebControl
            js.Append("\r\nfunction RefreshRolexSchedulResultWebControl(controlId){");
            if (_rolexResultWebControlList.Count > 0)
            {
                for (int i = 0; i < _rolexResultWebControlList.Count; i++)
                {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _rolexResultWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t" + _rolexResultWebControlList[i].ValidationMethodName + "();");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region DisplayRolexScheduleResultWebControl
            js.Append("\r\nfunction DisplayRolexScheduleResultWebControl(controlId){");
            if (_rolexResultWebControlList.Count > 0)
            {
                for (int i = 0; i < _rolexResultWebControlList.Count; i++)
                {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _rolexResultWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t" + _rolexResultWebControlList[i].DisplayMethod + "(true);");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region RefreshRolexScheduleSelectionWebControl
            js.Append("\r\nfunction RefreshRolexScheduleSelectionWebControl(controlId){");
            if (_rolexSelectionBaseWebControlList.Count > 0)
            {
                for (int i = 0; i < _rolexSelectionBaseWebControlList.Count; i++)
                {
                    js.Append("\r\n\t");
                    if (i > 0) js.Append("else ");
                    js.Append("if(controlId == '" + _rolexSelectionBaseWebControlList[i].CurrentControlDetail.ControlId + "'){");
                    js.Append("\r\n\t\t" + _rolexSelectionBaseWebControlList[i].ValidationMethodName + "();");
                    js.Append("\r\n\t}");
                }
            }
            js.Append("\r\n}");
            #endregion

            #region AskResultExportExcel
            js.Append("\r\nfunction AskResultExportExcel(){");          
            js.AppendFormat("\r\n\t OpenNewWindow('/Private/Results/Excel/RolexResults.aspx?idSession={0}');", _webSession.IdSession);
            js.Append("\r\n}");
            #endregion

            #region AskResultExportPDF
            js.Append("\r\nfunction AskResultExportPDF(exportType){");           
            js.AppendFormat("\r\n\t popupOpenBis('/Private/MyAdExpress/PdfSavePopUp.aspx?idSession={0}&resultType={1}','470','210','yes');", _webSession.IdSession, TNS.AdExpress.Anubis.Constantes.Result.type.amon.GetHashCode());           
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
    }
}

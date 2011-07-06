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
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Controls.Selections.VP;
using System.IO;
using TNS.AdExpress.Web.Controls.Selections.VP.Filter;
namespace TNS.AdExpress.Web.Controls.Results.VP
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleResultFilterWebControl runat=server></{0}:VpScheduleResultFilterWebControl>")]
    public class VpScheduleResultFilterWebControl : VpScheduleAjaxResultBaseWebControl {

        #region Variables
        /// <summary>
        /// Filter Result Web Control List
        /// </summary>
        Dictionary<Int64, VpScheduleSelectionFilterBaseWebControl> _filterResultWebControlList = new Dictionary<long, VpScheduleSelectionFilterBaseWebControl>();
        #endregion

        #region GetJavascript

        #region Menu
        /// <summary>
        /// Get Javascript Menu
        /// </summary>
        /// <returns>Javascript Menu</returns>
        protected string GetJavascriptMenu() {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");

            foreach (VpScheduleSelectionFilterBaseWebControl cVpScheduleSelectionFilterBaseWebControl in _filterResultWebControlList.Values) {
                js.Append("\r\nvar isLoaded_" + cVpScheduleSelectionFilterBaseWebControl.ID + " = false;");
            }

            js.Append("\r\nfunction valid_menu_" + this.ID + "(controlId){");
            foreach (VpScheduleSelectionFilterBaseWebControl cVpScheduleSelectionFilterBaseWebControl in _filterResultWebControlList.Values) {
                js.Append("\r\n\tif(controlId == '" + cVpScheduleSelectionFilterBaseWebControl.ID + "'){");
                js.Append("\r\n\t" + cVpScheduleSelectionFilterBaseWebControl.DisplayMethod + "(true);");
                js.Append("\r\n\tif(isLoaded_" + cVpScheduleSelectionFilterBaseWebControl.ID + "  == false){");
                js.Append("\r\n\tisLoaded_" + cVpScheduleSelectionFilterBaseWebControl.ID + " = true;");
                js.Append("\r\n\t" + cVpScheduleSelectionFilterBaseWebControl.ValidationMethodName + "();");
                js.Append("\r\n\t}");
                js.Append("\r\n\tdocument.getElementById('menu_" + cVpScheduleSelectionFilterBaseWebControl.ID + "').className = '" + CssClassOptionMenuSelected + "';");
                js.Append("\r\n\t} else {");
                js.Append("\r\n\t" + cVpScheduleSelectionFilterBaseWebControl.DisplayMethod + "(false);");
                js.Append("\r\n\tdocument.getElementById('menu_" + cVpScheduleSelectionFilterBaseWebControl.ID + "').className = '';");
                js.Append("\r\n\t}");

            }
            js.Append("\r\n}");

            js.Append("\r\n-->\r\n</script>");
            return js.ToString();
        }
        #endregion

        #region Valid Data

        protected string GetJavascriptValidData() {
            StringBuilder js = new StringBuilder();
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");
            js.Append("\r\nfunction validData_" + this.ID + "(){");

            js.Append("\r\n\tvar mediaParameters = null;");
            js.Append("\r\n\tvar productParameters = null;");
            js.Append("\r\n\tvar detailLevelParameters = null;");
            js.Append("\r\n\tvar dateParameters = null;");

            int i = 0;
            foreach (VpScheduleSelectionFilterBaseWebControl cVpScheduleSelectionFilterBaseWebControl in _filterResultWebControlList.Values) {
                switch (i) {
                    case 0:
                        js.Append("\r\n\tmediaParameters = " + cVpScheduleSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        break;
                    case 1:
                        js.Append("\r\n\tproductParameters = " + cVpScheduleSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        break;
                    case 2:
                        js.Append("\r\n\tdetailLevelParameters = " + cVpScheduleSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        break;
                    case 3:
                        js.Append("\r\n\tdateParameters = " + cVpScheduleSelectionFilterBaseWebControl.GetValuesSelectedMethod + ";");
                        break;
                }
                i++;
            }
            js.Append("\r\n\t" + this.GetType().Namespace + "." + this.GetType().Name + ".ValidData('" + _webSession.IdSession + "'");
            js.Append(", mediaParameters");
            js.Append(", productParameters");
            js.Append(", detailLevelParameters");
            js.Append(", dateParameters");
            js.Append(", validData_" + this.ID + "_callback);");
            js.Append("\r\nreturn false;");
            js.Append("\r\n}");

            js.Append("\r\nfunction validData_" + this.ID + "_callback(res){");
            js.Append("\r\n\tif(res != null && res.value!=null){ ");
            js.Append("\r\n\t\t\t alert(res.value);");
            js.Append("\r\n\t}");
            js.Append(DisplayMethod + "(false);");
            js.Append("\r\n}\r\n");
            js.Append("\r\n-->\r\n</script>");
            return js.ToString();
        }
        #endregion

        #region GetValidationJavascriptContent
        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected override string GetValidationJavascriptContent() {
            return base.GetValidationJavascriptContent();
        }
        #endregion 

        #region GetDisplayJavascriptContent
        /// <summary>
        /// Get Display Javascript Method
        /// </summary>
        /// <returns>Display Javascript Method</returns>
        protected override string GetDisplayJavascriptContent() {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(display) {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.height=document.body.clientHeight + \"px\";");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = '';");

            js.Append("\r\n\t\tvar myWidth = 0, myHeight = 0;");
            js.Append("\r\n\t\tif( typeof( window.innerWidth ) == 'number' ) {");
            //Non-IE
            js.Append("\r\n\t\tmyWidth = window.innerWidth;");
            js.Append("\r\n\t\tmyHeight = window.innerHeight;");
            js.Append("\r\n\t\t} else if( document.documentElement && ( document.documentElement.clientWidth || document.documentElement.clientHeight ) ) {");
            //IE 6+ in 'standards compliant mode'
            js.Append("\r\n\t\tmyWidth = document.documentElement.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.documentElement.clientHeight;");
            js.Append("\r\n\t\t} else if( document.body && ( document.body.clientWidth || document.body.clientHeight ) ) {");
            //IE 4 compatible
            js.Append("\r\n\t\tmyWidth = document.body.clientWidth;");
            js.Append("\r\n\t\tmyHeight = document.body.clientHeight;");
            js.Append("\r\n\t\t}");


            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.top = (document.documentElement.scrollTop + ((myHeight - 550) / 2)) + \"px\";");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').style.left = (document.documentElement.scrollLeft + ((myWidth - 750) / 2)) + \"px\";");


            foreach (VpScheduleSelectionFilterBaseWebControl cVpScheduleSelectionFilterBaseWebControl in _filterResultWebControlList.Values) {
                js.Append("\r\n\tisLoaded_" + cVpScheduleSelectionFilterBaseWebControl.ID + " = false;");
                js.Append("\r\n\tif(document.getElementById('menu_" + cVpScheduleSelectionFilterBaseWebControl.ID + "').className == '" + CssClassOptionMenuSelected + "'){");
                js.Append("\r\n\tvalid_menu_" + this.ID + "('" + cVpScheduleSelectionFilterBaseWebControl.ID + "');");
                js.Append("\r\n\t} else {");
                js.Append("\r\n\t\t" + cVpScheduleSelectionFilterBaseWebControl.InitializeResultMethod+";");
                js.Append("\r\n\t}");
            }

            js.Append("\r\n\t} else {");
            js.Append("\r\n\t\tdocument.getElementById('res_backgroud_" + this.ID + "').style.display = 'none';");
            js.Append("\r\n\t}");
            return (base.GetDisplayJavascriptContent() + js.ToString());
        }
        #endregion

        #region GetInitializeJavascriptContent
        /// <summary>
        /// Get Initialize Javascript Method
        /// </summary>
        /// <returns>Initialize Javascript Method</returns>
        protected override string GetInitializeJavascriptContent() {
            return base.GetInitializeJavascriptContent();
        }
        #endregion

        #endregion

        #region Property (Style)
        /// <summary>
        /// Get / Set VpScheduleSelectionNodeMediaWebControlSkinId
        /// </summary>
        public string VpScheduleSelectionNodeMediaWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set VpScheduleSelectionNodeProductWebControlSkinId
        /// </summary>
        public string VpScheduleSelectionNodeProductWebControlSkinId { get; set; }
        /// <summary>
        /// Get / Set CssClassOption
        /// </summary>
        public string CssClassOption { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionHeader
        /// </summary>
        public string CssClassOptionHeader { get; set; }  
        /// <summary>
        /// Get / Set CssClassOptionMenu
        /// </summary>
        public string CssClassOptionMenu { get; set; }    
        /// <summary>
        /// Get / Set CssClassOptionMenuSelected
        /// </summary>
        public string CssClassOptionMenuSelected { get; set; }
        /// <summary>
        /// Get / Set CssClassResult
        /// </summary>
        public string CssClassResult { get; set; }
        /// <summary>
        /// Get / Set CssClassResultContent
        /// </summary>
        public string CssClassResultContent { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtons
        /// </summary>
        public string CssClassOptionButtons { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtonCancel
        /// </summary>
        public string CssClassOptionButtonCancel { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonCancel
        /// </summary>
        public string PicturePathButtonCancel { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonCancelOver
        /// </summary>
        public string PicturePathButtonCancelOver { get; set; }
        /// <summary>
        /// Get / Set CssClassOptionButtonValidation
        /// </summary>
        public string CssClassOptionButtonValidation { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonValidation
        /// </summary>
        public string PicturePathButtonValidation { get; set; }
        /// <summary>
        /// Get / Set PicturePathButtonValidationOver
        /// </summary>
        public string PicturePathButtonValidationOver { get; set; }  
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

        #region Load
        /// <summary>
        /// Chargement du composant
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
            if (module.DefaultMediaDetailLevels != null && module.DefaultMediaDetailLevels.Count > 0) {

                VpScheduleSelectionNodeWebControl vpScheduleSelectionNodeWebControl = new VpScheduleSelectionNodeWebControl();
                vpScheduleSelectionNodeWebControl.ID = this.ID + "_Media";
                vpScheduleSelectionNodeWebControl.LevelIds = ((GenericDetailLevel)(module.DefaultMediaDetailLevels[0])).LevelIds;
                vpScheduleSelectionNodeWebControl.WebSession = _webSession;
                vpScheduleSelectionNodeWebControl.Display = true;
                vpScheduleSelectionNodeWebControl.SkinID = VpScheduleSelectionNodeMediaWebControlSkinId;
                vpScheduleSelectionNodeWebControl.GenericDetailLevelComponentProfile = TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.media;
                _filterResultWebControlList.Add(2869, vpScheduleSelectionNodeWebControl);
                Controls.Add(vpScheduleSelectionNodeWebControl);

                vpScheduleSelectionNodeWebControl = new VpScheduleSelectionNodeWebControl();
                vpScheduleSelectionNodeWebControl.ID = this.ID + "_Product";
                vpScheduleSelectionNodeWebControl.LevelIds = ((GenericDetailLevel)(module.DefaultProductDetailLevels[0])).LevelIds;
                vpScheduleSelectionNodeWebControl.WebSession = _webSession;
                vpScheduleSelectionNodeWebControl.Display = false;
                vpScheduleSelectionNodeWebControl.SkinID = VpScheduleSelectionNodeProductWebControlSkinId;
                vpScheduleSelectionNodeWebControl.GenericDetailLevelComponentProfile = TNS.AdExpress.Constantes.Web.GenericDetailLevel.ComponentProfile.product;
                _filterResultWebControlList.Add(2870, vpScheduleSelectionNodeWebControl);
                Controls.Add(vpScheduleSelectionNodeWebControl);
            }
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
            output.Write(GetJavascriptMenu());
            output.Write(GetJavascriptValidData());
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

            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\">");
            html.Append("<tr>");
            html.Append("<td class=\"" + CssClassOption + "\">");

            #region Menu
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" align=\"center\" width=\"100%\" height=\"100%\" class=\"" + CssClassOption + "\">");
            html.Append("<tr><td class=\"" + CssClassOptionHeader + "\">");
            html.Append(GestionWeb.GetWebWord(2863, _webSession.SiteLanguage));
            html.Append("</td></tr>");
            html.Append("<tr><td class=\"" + CssClassOptionMenu + "\">");
            html.Append("<ul>");
            foreach (KeyValuePair<Int64, VpScheduleSelectionFilterBaseWebControl> kvp in _filterResultWebControlList) {
                html.Append("<li><a");
                if (kvp.Value.Display)
                    html.Append(" class=\"" + CssClassOptionMenuSelected + "\"");
                html.Append(" id=\"menu_" + kvp.Value.ID + "\" href=\"#\" onclick=\"javascript:valid_menu_" + this.ID + "('" + kvp.Value.ID + "');\">");
                html.Append(GestionWeb.GetWebWord(kvp.Key, _webSession.SiteLanguage));
                html.Append("</a></li>");
            }
            html.Append("</ul>");
            html.Append("</td></tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td>");
            html.Append("<td class=\"" + CssClassResult + "\">");

            html.Append("<div class=\"" + CssClassResult + "\">");
            html.Append("<div class=\""+CssClassResultContent+"\">");
            #region Result
            foreach (VpScheduleSelectionNodeWebControl cVpScheduleSelectionNodeWebControl in _filterResultWebControlList.Values) {
                using (MemoryStream memoryStream = new MemoryStream()) {
                    using (StreamWriter streamWriter = new StreamWriter(memoryStream)) {
                        using (HtmlTextWriter memoryWriter = new HtmlTextWriter(streamWriter)) {
                            cVpScheduleSelectionNodeWebControl.RenderControl(memoryWriter);
                            memoryWriter.Flush();
                            memoryStream.Position = 0;
                            using (StreamReader reader = new StreamReader(memoryStream)) {
                                html.Append(reader.ReadToEnd());
                            }
                        }
                    }
                }
            }  
            #endregion
            html.Append("</div>");
            html.Append("</div>");

            html.Append("</td>");
            html.Append("</tr>");

            html.Append("<tr>");
            html.Append("<td colspan=\"2\" class=\"" + CssClassOptionButtons + "\">");

            #region Buttons
            html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" class=\"" + CssClassOptionButtons + "\">");
            html.Append("<tr>");
            html.Append("<td class=\"" + CssClassOptionButtonCancel + "\">");
            html.Append("<img src=\"" + PicturePathButtonCancel + "\" onmouseover=\"javascript:this.src='" + PicturePathButtonCancelOver + "';\" onmouseout=\"javascript:this.src='" + PicturePathButtonCancel + "';\" onclick=\"javascript:" + DisplayMethod + "(false);\"/>");
            html.Append("</td>");
            html.Append("<td class=\"" + CssClassOptionButtonValidation + "\">");
            html.Append("<img src=\"" + PicturePathButtonValidation + "\" onmouseover=\"javascript:this.src='" + PicturePathButtonValidationOver + "';\" onmouseout=\"javascript:this.src='" + PicturePathButtonValidation + "';\" onclick=\"javascript:validData_" + this.ID + "();\"/>");
            html.Append("</td>");
            html.Append("</tr>");
            html.Append("</table>");
            #endregion

            html.Append("</td>");
            html.Append("</tr>");

            html.Append("</table>");
            return html.ToString();

        }
        #endregion

        #region ValidData
        /// <summary>
        /// Obtention du code HTML à insérer dans le composant
        /// </summary>
        /// <param name="sessionId">Session du client</param>
        /// <returns>Code HTML</returns>
        //[AjaxPro.AjaxMethod]
        //public abstract string GetData(string sessionId);
        /// <summary>
        /// Obtention du code HTML à insérer dans le composant
        /// </summary>
        /// <param name="sessionId">Session du client</param>
        /// <param name="oParams">Tableaux de paramètres</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public string ValidData(string sessionId, object[] mediaParameters, object[] productParameters, object[] detailLevelParameters, object[] dateParameters) {
            string html = null;
            try {

                #region Obtention de la session
                _webSession = (WebSession)WebSession.Load(sessionId);
                #endregion



            }
            catch (System.Exception err) {
                return (GestionWeb.GetWebWord(1973, _webSession.SiteLanguage));
            }
            return (html);
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// GetAjaxHTML
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML() {
            throw new NotImplementedException();
        }
        #endregion
    }
}


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
namespace TNS.AdExpress.Web.Controls.Selections.VP.Filter
{
    /// <summary>
    /// Affiche le résultat d'une alerte plan media
    /// </summary>
    [DefaultProperty("Text"),
      ToolboxData("<{0}:VpScheduleSelectionFilterBaseWebControl runat=server></{0}:VpScheduleSelectionFilterBaseWebControl>")]
    public class VpScheduleSelectionFilterBaseWebControl : VpScheduleAjaxSelectionBaseWebControl {

        #region Assessor

        #region Get Values Selected Method
        /// <summary>
        /// Get Get Values Selected Method
        /// </summary>
        public string GetValuesSelectedMethod {
            get { return "getValues_"+this.ID+"()"; }
        }
        #endregion

        #region Validation Method
        /// <summary>
        ///// Get InitializeResultMethod
        /// </summary>
        [Bindable(false)]
        public string InitializeResultMethod { 
            get { return "initializeResult_" + this.ID + "()"; } 
        }
        #endregion

        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected string AjaxEventScript() {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tdocument.getElementById('" + this.ID + "').innerHTML='" + GetHTML().Replace("'", "\\'") + "';");
            js.Append("\r\n}");

            js.Append("\r\n\r\n</script>");
            return (js.ToString());
        }
        #endregion

        #region ValuesSelectedMethodScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        private string ValuesSelectedMethodScript() {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append("function " + GetValuesSelectedMethod + "{");
            js.Append("\r\n" + GetValuesSelectedMethodScriptContent());   
            js.Append("\r\n}");           
            js.Append("\r\n\r\n</script>");
            return (js.ToString());
        }

        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected virtual string GetValuesSelectedMethodScriptContent() {
            return string.Empty;
        }
        #endregion

        #region GetInitializeResultMethodScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        private string GetInitializeResultMethodScript() {

            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append("function " + InitializeResultMethod + "{");
            js.Append("\r\nget_" + this.ID + "();");
            js.Append("\r\n" + GetInitializeResultMethodContent());
            js.Append("\r\n}");
            js.Append("\r\n\r\n</script>");
            return (js.ToString());
        }

        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected virtual string GetInitializeResultMethodContent() {
            return string.Empty;
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetAjaxHTML() {
            return string.Empty;
        }
        #endregion

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

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            output.Write(ValuesSelectedMethodScript());
            output.Write(GetInitializeResultMethodScript());
            base.Render(output);
        }
        #endregion

    }
}


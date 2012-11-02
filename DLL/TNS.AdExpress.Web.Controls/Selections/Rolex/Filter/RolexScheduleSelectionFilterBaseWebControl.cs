using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace TNS.AdExpress.Web.Controls.Selections.Rolex.Filter
{
    /// <summary>
    /// Rolex Selection Filter Base WebControl
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:RolexSelectionFilterBaseWebControl runat=server></{0}:RolexSelectionFilterBaseWebControl>")]
    public class RolexScheduleSelectionFilterBaseWebControl : AjaxSelectionBaseWebControl
    {
        #region Accessor

        #region Get Values Selected Method
        /// <summary>
        /// Get Get Values Selected Method
        /// </summary>
        public string GetValuesSelectedMethod
        {
            get { return "getValues_" + this.ID + "()"; }
        }
        #endregion

        #region Validation Method
        /// <summary>
        ///// Get InitializeResultMethod
        /// </summary>
        [Bindable(false)]
        public string InitializeResultMethod
        {
            get { return "initializeResult_" + this.ID + "()"; }
        }
        #endregion

        #endregion

        #region AjaxEventScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected string AjaxEventScript()
        {

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
        private string ValuesSelectedMethodScript()
        {

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
        protected virtual string GetValuesSelectedMethodScriptContent()
        {
            return string.Empty;
        }
        #endregion

        #region GetInitializeResultMethodScript
        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        private string GetInitializeResultMethodScript()
        {

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
        protected virtual string GetInitializeResultMethodContent()
        {
            return string.Empty;
        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Compute Rolex schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetAjaxHTML()
        {
            return string.Empty;
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            InitializeResultToLoad = false;
        }
        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write(ValuesSelectedMethodScript());
            output.Write(GetInitializeResultMethodScript());
            base.Render(output);
        }
        #endregion
    }
}

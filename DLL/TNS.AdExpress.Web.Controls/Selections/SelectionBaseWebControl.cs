using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Selections
{
    /// <summary>
    /// Selection base web control
    /// </summary>
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:SelectionBaseWebControl runat=server></{0}:SelectionBaseWebControl>")]
    public abstract class SelectionBaseWebControl : WebControl
    {
        #region Variables
        /// <summary>
        /// Display Control or not
        /// </summary>
        protected bool _display = true;
        /// <summary>
        /// Web Session
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        /// <summary>
        /// Validation Method
        /// </summary>
        protected string _validationMethod = string.Empty;
        /// <summary>
        /// Current control Detail
        /// </summary>
        protected ControlLayer _currentControlDetail = null;
        /// <summary>
        /// Text Id
        /// </summary>
        protected Int64 _textId = 0;
        #endregion

        #region Accesors

        #region Current control Detail
        /// <summary>
        ///// Get / Set Current control Detail
        /// </summary>
        [Bindable(false)]
        public ControlLayer CurrentControlDetail
        {
            get { return (_currentControlDetail); }
            set
            {
                _currentControlDetail = value;
            }
        }
        #endregion

        #region Display Control
        /// <summary>
        ///// Get / Set Display Control
        /// </summary>
        [Bindable(false)]
        public bool Display
        {
            get { return (_display); }
            set
            {
                _display = value;
            }
        }
        #endregion

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

        #region Validation Method Name
        /// <summary>
        /// Get Validation Method Name
        /// </summary>
        [Bindable(false)]
        public string ValidationMethodName
        {
            get { return ("validation_" + this.ID); }
        }
        #endregion

        #region Validation Method
        /// <summary>
        ///// Get / Set Validation Method
        /// </summary>
        [Bindable(false)]
        public string ValidationMethod
        {
            get { return (_validationMethod); }
            set { _validationMethod = value; }
        }
        #endregion

        #region Text Id
        /// <summary>
        ///// Get / Set Text Id
        /// </summary>
        [Bindable(false)]
        public Int64 TextId
        {
            get { return (_textId); }
            set { _textId = value; }
        }
        #endregion

        #region Display Method
        /// <summary>
        /// Get Display Method
        /// </summary>
        [Bindable(false)]
        public string DisplayMethod
        {
            get { return ("display_" + this.ID); }
        }
        #endregion

        #endregion

        #region GetJavascript
        /// <summary>
        /// Get Javascript
        /// </summary>
        /// <returns>Javascript</returns>
        private string GetJavascript()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n<!--");
            js.Append("\r\nfunction " + ValidationMethodName + "(){");
            js.Append("\r\n" + GetValidationJavascriptContent());
            js.Append("\r\n}");
            js.Append("\r\nfunction " + DisplayMethod + "(display){");
            js.Append("\r\n" + GetDisplayJavascriptContent());
            js.Append("\r\n}");
            js.Append("\r\n-->\r\n</script>");
            return (js.ToString());
        }
        /// <summary>
        /// Get Validation Javascript Method
        /// </summary>
        /// <returns>Validation Javascript Method</returns>
        protected virtual string GetValidationJavascriptContent()
        {
            return string.Empty;
        }
        /// <summary>
        /// Get Display Javascript Method
        /// </summary>
        /// <returns>Display Javascript Method</returns>
        protected virtual string GetDisplayJavascriptContent()
        {
            StringBuilder js = new StringBuilder(1000);
            js.Append("\r\n\tif(display) document.getElementById('" + this.ID + "').style.display = 'block';");
            js.Append("\r\n\telse document.getElementById('" + this.ID + "').style.display = 'none';");
            return (js.ToString());
        }
        #endregion

        #region Evènements

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output)
        {
            output.Write(GetJavascript());
            output.AddAttribute("id", this.ID);
            output.AddAttribute("style", ((Display) ? "" : " display:none; "));
            if (!string.IsNullOrEmpty(CssClass))
                output.AddAttribute("class", CssClass);
            output.RenderBeginTag(HtmlTextWriterTag.Div);
            output.Write(GetHTML());
            output.RenderEndTag();
        }
        #endregion

        #endregion

        #region GetHTML
        /// <summary>
        /// Get Html
        /// </summary>
        /// <returns>Code HTMl</returns>
        protected virtual string GetHTML()
        {
            return string.Empty;
        }
        #endregion
    }
}

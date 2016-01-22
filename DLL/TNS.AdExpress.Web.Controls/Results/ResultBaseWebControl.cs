﻿using System.Web.UI.WebControls;
using TNS.AdExpress.Web.Core.Sessions;
using System.ComponentModel;
using System.Web.UI;
using System.Text;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.FrameWork.Exceptions;
using System;
using TNS.AdExpress.Domain.Layers;

namespace TNS.AdExpress.Web.Controls.Results
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:ResultBaseWebControl runat=server></{0}:ResultBaseWebControl>")]
    public abstract class ResultBaseWebControl : WebControl
    {
        #region Variables
        /// <summary>
        /// Display Control or not
        /// </summary>
        protected bool _display = true;
        /// <summary>
        /// Session client
        /// </summary>
        protected WebSession _webSession = null;
        /// <summary>
        /// Validation Method
        /// </summary>
        protected string _validationMethod = string.Empty;
        /// <summary>
        /// Theme name
        /// </summary>
        protected string _themeName = string.Empty;
        /// <summary>
        /// Current control Detail
        /// </summary>
        protected ControlLayer _currentControlDetail = null;
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
        ///// Get Validation Method
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

        #region Display Method
        /// <summary>
        ///// Get Display Method
        /// </summary>
        [Bindable(false)]
        public string DisplayMethod
        {
            get { return ("display_" + this.ID); }
        }
        #endregion

        #region Initialize Method
        /// <summary>
        ///// Get Initialize Method
        /// </summary>
        [Bindable(false)]
        public string InitializeMethod
        {
            get { return ("initialize_" + this.ID); }
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
            js.Append("\r\nfunction " + InitializeMethod + "(){");
            js.Append("\r\n" + GetInitializeJavascriptContent());
            js.Append("\r\n}");

            js.Append("\r\nif(window.addEventListener)");
            js.Append("\r\n\twindow.addEventListener(\"load\", " + InitializeMethod + ", false);");
            js.Append("\r\nelse if(window.attachEvent)");
            js.Append("\r\n\twindow.attachEvent(\"onload\"," + InitializeMethod + "); ");

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
            js.Append("\r\n\tif(display) document.getElementById('" + this.ID + "').style.display = '';");
            js.Append("\r\n\telse document.getElementById('" + this.ID + "').style.display = 'none';");
            return (js.ToString());
        }
        /// <summary>
        /// Get Initialize Javascript Method
        /// </summary>
        /// <returns>Initialize Javascript Method</returns>
        protected virtual string GetInitializeJavascriptContent()
        {
            return string.Empty;
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
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected virtual string GetHTML()
        {
            return string.Empty;
        }
        #endregion
    }
}

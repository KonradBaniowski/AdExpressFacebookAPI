using System;
using System.ComponentModel;
using System.Text;
using System.Web.UI;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Selections.VP
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:VpExcluWebControl runat=server></{0}:VpExcluWebControl>")]
    public class VpScheduleSelectionExcluWebControl : AjaxSelectionBaseWebControl
    {
        #region AjaxEventScript

        /// <summary>
        /// Evenement Ajax
        /// </summary>
        /// <returns></returns>
        protected override string AjaxEventScript()
        {

            var js = new StringBuilder(1000);
            js.Append("\r\n<script language=\"javascript\">\r\n");
            js.Append("\r\nfunction get_" + this.ID + "(){");
            js.Append("\r\n\tvar oN=document.getElementById('" + this.ID + "');");
            js.Append("\r\n\toN.innerHTML='" + GetAjaxInitialisationHTML().Replace("'", "\\'") + "';");
            js.Append("\r\n}");

            js.Append(GetAjaxEventScript());

            js.Append("\r\n\r\n</script>");
            return (js.ToString());
        }

        /// <summary>
        /// Get Evenement Ajax
        /// </summary>
        /// <returns>Evenement Ajax</returns>
        protected override string GetAjaxEventScript()
        {
            var js = new StringBuilder(1000);

            js.AppendFormat("\r\nfunction setExcluWeb_{0}(checkBoxElem){{", this.ID);
            js.AppendFormat("\r\n\t{0}.{1}.SetExcluWeb('{2}',checkBoxElem.checked,setExcluWeb_{3}_callback);"
                , this.GetType().Namespace, this.GetType().Name, this._webSession.IdSession, this.ID);          
            js.Append("\r\n}\r\n");

            js.AppendFormat("\r\nfunction setExcluWeb_{0}_callback(res){{", this.ID);
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
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
          //  InitializeResultToLoad = false;
        }
        #endregion

        #endregion

        #region SetDate
        /// <summary>
        /// Set Date in WebSession
        /// </summary>
        ///<param name="idSession">Id Session</param>
        /// <param name="isExcluWeb">Is Exclu Web</param>
        /// <returns>Code HTML</returns>
        [AjaxPro.AjaxMethod]
        public void SetExcluWeb(string idSession, bool isExcluWeb)
        {
            try
            {
                _webSession = (WebSession)WebSession.Load(idSession);
                _webSession.IsExcluWeb = isExcluWeb;               
                _webSession.Save();
            }
            catch (Exception err)
            {
                throw new Exception(OnAjaxMethodError(err, _webSession), err);
            }

        }
        #endregion

        #region GetAjaxHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxHTML()
        {
            var html = new StringBuilder(1000);
            string checkboxChecked = _webSession.IsExcluWeb ? " checked " : string.Empty;

            html.AppendFormat("<div  style=\"vertical-align: middle\" class={0}>", CssClass);


            html.Append("<label style=\"vertical-align: middle\" ");       
            html.Append("> ");

            html.Append("<input  type=\"checkbox\" style=\"vertical-align: bottom\"  ");
            html.AppendFormat("id=\"excluWeb_{0}\" ", this.ID);
            html.AppendFormat(" onclick=\"javascript:setExcluWeb_{0}(this);\" ", this.ID);
            html.Append("class=\"" + CssClass + "\" ");
            html.Append(checkboxChecked);
            html.Append(" /> ");

            html.Append(GestionWeb.GetWebWord(2997, _webSession.SiteLanguage));
            html.Append("</label>");
            html.Append("</div>");
            return (html.ToString());
        }
        #endregion

        #region GetAjaxInitialisationHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetAjaxInitialisationHTML()
        {
            var html = new StringBuilder();           
            return html.ToString();
        }
        #endregion

        #region GetHTML
        /// <summary>
        /// Get  loading HTML  
        /// </summary>
        /// <returns></returns>
        protected override string GetHTML()
        {
            return GetAjaxHTML();
        }
        #endregion

        
    }
}

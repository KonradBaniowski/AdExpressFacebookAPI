using System.ComponentModel;
using System.Text;
using TNS.AdExpress.Domain.Translation;

namespace TNS.AdExpress.Web.Controls.Selections
{
    public class SelectionImageWebControl : SelectionBaseWebControl 
    {
        #region Property (Style)
        /// <summary>
        /// Picture Src Path
        /// </summary>
        protected string _pictureSrcPath = "";
        /// <summary>
        /// Get / Set Picture Src Path
        /// </summary>
        [Bindable(true),
        Category("Picture Src Path"),
        DefaultValue("")]
        public string PictureSrcPath
        {
            get { return _pictureSrcPath; }
            set { _pictureSrcPath = value; }
        }
        /// <summary>
        /// Picture Src Path Over
        /// </summary>
        protected string _pictureSrcPathOver = "";
        /// <summary>
        /// Get / Set Picture Src Path Over
        /// </summary>
        [Bindable(true),
        Category("Picture Src Path Over"),
        DefaultValue("")]
        public string PictureSrcPathOver
        {
            get { return _pictureSrcPathOver; }
            set { _pictureSrcPathOver = value; }
        }
        #endregion

        #region GetHTML
        /// <summary>
        /// Compute VP schedule
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <returns>Code HTMl</returns>
        protected override string GetHTML()
        {
            StringBuilder html = new StringBuilder();
            html.Append("<img ");
            html.Append("id=\"" + this.ID + "\" ");
            html.Append("src=\"" + _pictureSrcPath + "\" ");
            html.Append("onmouseover=\"javascript:this.src='" + _pictureSrcPathOver + "';\" ");
            html.Append("onmouseout=\"javascript:this.src='" + _pictureSrcPath + "';\" ");
            html.Append("onclick=\"" + _validationMethod + "\" ");
            html.Append("class=\"" + CssClass + "\" ");
            html.Append(" />");

            html.Append("<label ");
            html.Append("onclick=\"" + _validationMethod + "\" ");
            html.Append("onmouseover=\"javascript:document.getElementById('" + this.ID + "').src='" + _pictureSrcPathOver + "';\" ");
            html.Append("onmouseout=\"javascript:document.getElementById('" + this.ID + "').src='" + _pictureSrcPath + "';\" ");
            html.Append(">");
            html.Append(GestionWeb.GetWebWord(_textId, _webSession.SiteLanguage));
            html.Append("</label>");
            return html.ToString();
        }
        #endregion


        
    }
}

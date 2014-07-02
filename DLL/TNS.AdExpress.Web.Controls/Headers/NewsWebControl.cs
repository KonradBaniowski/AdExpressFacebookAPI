using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Controls.Headers {
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:NewsWebControl runat=server></{0}:GadWebControl>")]
    public class NewsWebControl : WebControl {

        #region variables
        /// <summary>
        /// User session
        /// </summary>
        private WebSession _session;
        /// <summary>
        /// Flag to display the web control or not
        /// </summary>
        private bool _display = false;
        /// <summary>
        /// Title font
        /// </summary>
        private string _titleFont = string.Empty;
        /// <summary>
        /// Title Style
        /// </summary>
        private string _titleStyle = string.Empty;
        /// <summary>
        /// Content Font
        /// </summary>
        private string _contentFont = string.Empty;
        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        public WebSession CustomerWebSession {
            set {
                _session = value;
            }
        }
        /// <summary>
        /// Set / Get Display Flag
        /// </summary>
        public bool Display {
            get { return _display; }
            set { _display = value; }
        }
        /// <summary>
        /// Set / Get Title Font
        /// </summary>
        public string TitleFont {
            get { return _titleFont; }
            set { _titleFont = value; }
        }
        /// <summary>
        /// Set / Get Title Style
        /// </summary>
        public string TitleStyle {
            get { return _titleStyle; }
            set { _titleStyle = value; }
        }
        /// <summary>
        /// Set / Get Content Font
        /// </summary>
        public string ContentFont {
            get { return _contentFont; }
            set { _contentFont = value; }
        }
        #endregion

        #region Prérender
        /// <summary>
        /// On Pre Render
        /// </summary>
        /// <param name="e">Sender</param>
        protected override void OnPreRender(EventArgs e) {

            if (_display) {
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("jquery"))
                    this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "jquery", this.Page.ResolveClientUrl("~/scripts/jquery.js"));
                if (!this.Page.ClientScript.IsClientScriptIncludeRegistered("thickbox"))
                    this.Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "thickbox", this.Page.ResolveClientUrl("~/scripts/thickboxNoConflict.js"));

                #region Script
                StringBuilder js = new StringBuilder();
                js.Append("\r\n <script language=\"javascript\" type=\"text/javascript\"> ");

                js.Append("\r\n function getCookieContent(cname) { ");
                js.Append("\r\n     var name = cname + \"=\"; ");
                js.Append("\r\n     var ca = document.cookie.split(';'); ");
                js.Append("\r\n     for(var i=0; i<ca.length; i++) { ");
                js.Append("\r\n         var c = ca[i].trim(); ");
                js.Append("\r\n         if (c.indexOf(name) == 0) return c.substring(name.length,c.length); ");
                js.Append("\r\n     } ");
                js.Append("\r\n     return \"\"; ");
                js.Append("\r\n } ");

                js.Append("\r\n //no conflict jquery ");
                js.Append("\r\n jQuery.noConflict(); ");

                js.Append("\r\n jQuery(document).ready(function(){ ");
                js.Append("\r\n     var checkCookie = getCookieContent('NewsOverlay'); ");
                js.Append("\r\n     if(checkCookie == \"\") {");
                js.Append("\r\n         document.cookie = \"NewsOverlay=yes\"; ");
                js.Append("\r\n         tb_show(\"" + GestionWeb.GetWebWord(1377, _session.SiteLanguage) + "\",\"#TB_inline?height=300&width=500&inlineId=newsContent&caption=" + GestionWeb.GetWebWord(2371, _session.SiteLanguage) + "&label=" + GestionWeb.GetWebWord(2372, _session.SiteLanguage) + "\"); ");
                js.Append("\r\n     }");
                js.Append("\r\n }); ");

                js.Append("\r\n</script>");

                if (!Page.ClientScript.IsClientScriptBlockRegistered("ThickBoxCall")) Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "ThickBoxCall", js.ToString());
                #endregion

            }

            base.OnPreRender(e);
        }
        #endregion

        #region Render
        /// <summary>
        /// Render Contents
        /// </summary>
        /// <param name="output">Output</param>
        protected override void RenderContents(HtmlTextWriter output) {

            if (_display) {
                output.Write("<div id=\"newsContent\" style=\"display:none;\">"); 
                output.Write("<div class=\"" + _titleFont + " " + _titleStyle + "\">" + GestionWeb.GetWebWord(3008, _session.SiteLanguage) + "</div>");
                output.Write("<div class=\"" + _contentFont + "\">" + GestionWeb.GetWebWord(3009, _session.SiteLanguage) + "</div>");
                output.Write("</div>");
            }


        }
        #endregion


    }
}

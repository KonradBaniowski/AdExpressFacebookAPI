using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Web.UI;
using KMI.PromoPSA.Web.Core.Sessions;
using KMI.PromoPSA.Web.Domain.Translation;

namespace KMI.PromoPSA.Web.Controls.Header {
    /// <summary>
    /// Disconnect User WebControl
    /// </summary>
    [ToolboxData("<{0}:DisconnectUserWebControl runat=server></{0}:DisconnectUserWebControl>")]
    public class DisconnectUserWebControl : System.Web.UI.WebControls.WebControl {

        #region Variables
        /// <summary>
        /// WebSession
        /// </summary>
        private WebSession _webSession;
        /// <summary>
        /// Specifies the image url
        /// </summary>
        private string _imageUrl = string.Empty;
        #endregion

        #region Assessor
        /// <summary>
        /// Get / Set Site Language
        /// </summary>
        [Bindable(true),
        Description("WebSession"),
        DefaultValue(0)]
        public WebSession WebSession {
            set { _webSession = value; }
        }
        /// <summary>
        /// Specifies the image url
        /// </summary>
        [Bindable(true),
        Category("Appearance"),
        DefaultValue(""),
        Description("Specifies the image url for the button when in its normal state.")]
        public string ImageUrl {
            set {
                _imageUrl = value;
            }
        }
        #endregion

        #region Evènements

        #region Initialisation
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnInit(EventArgs e) {
            base.OnInit(e);
        }
        #endregion

        #region PréRender
        /// <summary>
        /// On Pre Render
        /// </summary>
        /// <param name="e">Event Args</param>
        protected override void OnPreRender(EventArgs e) {
            base.OnPreRender(e);
        }

        #endregion

        #region Render
        /// <summary> 
        /// Génère ce contrôle dans le paramètre de sortie spécifié.
        /// </summary>
        /// <param name="output"> Le writer HTML vers lequel écrire </param>
        protected override void Render(HtmlTextWriter output) {
            
            if (_webSession != null) {

                StringBuilder html = new StringBuilder();
                
                html.Append("<table class=\"disconnectUser\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" height=\"100%\"><tr>");
                html.Append("<td><a href=\"#\" onclick=\"javascript:document.location='" + Page.ResolveUrl("~/disconnectUser.aspx?id=" + _webSession.CustomerLogin.IdLogin) + "';\">");
                html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" height=\"100%\"><tr>");
                html.AppendFormat("<td>");
                html.AppendFormat("<img src=\"{0}\" align=\"absBottom\"/>", _imageUrl);
                html.AppendFormat("</td>");
                html.Append("<td>");
                html.Append(GestionWeb.GetWebWord(19, _webSession.SiteLanguage));
                html.Append("</td>");
                html.Append("</tr></table>");
                html.Append("</a></td></tr></table>");
                
                output.Write(html.ToString());

            }
        }
        #endregion

        #endregion

    }
}

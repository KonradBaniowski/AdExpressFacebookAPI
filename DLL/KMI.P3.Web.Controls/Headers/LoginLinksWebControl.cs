using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using TNS.Isis.Right.Common;
using KMI.P3.Web.Core.Sessions;
using KMI.P3.Domain.Web;
using KMI.P3.Domain.Translation;
using System.Data;
using KMI.P3.Domain.DataBaseDescription;

namespace KMI.P3.Web.Controls.Headers
{
    [DefaultProperty("Text")]
    [ToolboxData("<{0}:LoginLinksWebControl runat=server></{0}:LoginLinksWebControl>")]
    public class LoginLinksWebControl : WebControl
    {
        const int NB_ADEX_ARGS = 2;
        const string SPLITTER = "¤¤";

        #region Variables
        /// <summary>
        /// Web session
        /// </summary>
        protected WebSession _webSession = null;
        protected AdExpressLogin _AdExpressLogin = null;
        #endregion

        #region Accessors
        /// <summary>
        /// Get \ Set customer Web session
        /// </summary>
        [Description("Customer web session ")]
        public WebSession CustomerWebSession
        {
            get { return _webSession; }
            set { _webSession = value; }
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// Evènement de chargement du composant
        /// </summary>
        /// <param name="e">Argument</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            _AdExpressLogin = new AdExpressLogin(_webSession.Source, _webSession.Login, _webSession.Password);           
           
        }
        #endregion

        #region RenderContents
        protected override void RenderContents(HtmlTextWriter output)
        {
           
            output.Write("<table cellpadding=\"0\" cellspacing=\"0\" border=\"0\">");
            output.Write(" <tr><td><img src=\"App_Themes/"+this.Page.Theme+"/Images/Common/pixel.gif\" Height=\"20px\" width=\"260px\" />");
            output.Write(" </td></tr>");
            output.Write(" <tr><td>");

            #region AdExpress's Links
            //AdExpress's Links
            if (_AdExpressLogin.CanAccessToProject())
            {
                string encryptedParams = KMI.P3.Web.Functions.QueryStringEncryption.EncryptQueryString(_webSession.Login + SPLITTER + _webSession.Password + SPLITTER + _webSession.SiteLanguage + SPLITTER + DateTime.Now.ToString("yyyyMMdd"));
                output.Write(" <div id=\"listLoginAdexpress\" class=\"menuloginAdExpress\">");
                output.Write("<ul>");
                output.Write(" <li><a href=\"#\" onclick=\"javascript:window.open('" + KMI.P3.Constantes.Web.URL.ADEXPRESS_URL + "?p=" + encryptedParams + "','AdExpress');\" >&nbsp;AdExpress [" + _AdExpressLogin.Label + "]</a></li>");
                output.Write(" </ul>");
                output.Write(" </div>");
            }
            #endregion

            #region Adscope's Links
            // TODO Adscope's Links 
            if (_AdExpressLogin.AdScopeExternalLoginList.Count>0)
            { 
                output.Write(" <br><div id=\"listLoginAdexpress\" class=\"menulogins\">");
                output.Write("<ul>");
            }
            foreach (AdScopeExternalLogin adScopeLogin in _AdExpressLogin.AdScopeExternalLoginList)
            {
                
                string cryptedLogin = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(adScopeLogin.Login);
                string cryptedPassword = KMI.P3.Web.Functions.QueryStringEncryption.AdScopeCrypt(adScopeLogin.Password);
                string url = Page.ResolveUrl( KMI.P3.Constantes.Web.URL.ADSCOPE_URL + "?cryptedLogin=" + cryptedLogin + "&cryptedPassword=" + cryptedPassword + "&crypteIdent=1");
                //output.Write(" <li><a href=\"#\" onclick=\"javascript:window.open('" + KMI.P3.Constantes.Web.URL.ADSCOPE_URL + "?cryptedLogin="+cryptedLogin+"&cryptedPassword="+cryptedPassword+"&crypteIdent=1','AdScope');\" >&nbsp;AdScope [" + adScopeLogin.Login + "]</a></li>");
                output.Write(" <li><a href=\"#\" onclick=\"javascript:window.open('" + url+"','AdScope');\" >&nbsp;AdScope [" + adScopeLogin.Login + "]</a></li>");
            }
            if (_AdExpressLogin.AdScopeExternalLoginList.Count > 0)
            {
                output.Write(" </ul>");
                output.Write(" </div>");
            }

          
            #endregion

            output.Write(" </td></tr>");
            output.Write(" </table>");
           
        }
        #endregion
    }
}

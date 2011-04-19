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

            string eventSender = "", eventArguments = "";
            string[] arrArgs = null;

            if (Page.Request.Form.GetValues("__loginChoiceEVENTTARGET") != null)
            {
                eventSender = Page.Request.Form.GetValues("__loginChoiceEVENTTARGET")[0];
            }
            if (Page.Request.Form.GetValues("__loginChoiceEVENTARGUMENT") != null)
            {
                eventArguments = Page.Request.Form.GetValues("__loginChoiceEVENTARGUMENT")[0];
                arrArgs = eventArguments.Split('-');
            }

            if (Page.IsPostBack)
            {

                try
                {

                    //Connect automatically to AdExpress
                    if (arrArgs != null && arrArgs.Length == NB_ADEX_ARGS && _AdExpressLogin.CanAccessToProject())
                    {
                        string encryptedParams = KMI.P3.Web.Functions.QueryStringEncryption.EncryptQueryString(_webSession.Login + SPLITTER + _webSession.Password + SPLITTER + _webSession.SiteLanguage + SPLITTER + DateTime.Now.ToString("yyyyMMdd"));
                        //Page.Response.Redirect(KMI.P3.Constantes.Web.URL.ADEXPRESS_URL + "?p=" + encryptedParams);
                        Page.Response.Write("<script language=javascript>");
                        Page.Response.Write("window.open('" + KMI.P3.Constantes.Web.URL.ADEXPRESS_URL + "?p=" + encryptedParams + "','AdExpress');"); ;
                        Page.Response.Write("</script>");

                    }
                    ////Connect automatically to AdScope
                    //else if (arrArgs != null && arrArgs.Length == NB_ADEX_ARGS && _webSession.CustomerLogin.CanAccessToProject(KMI.P3.Constantes.Project.P3_ID))
                    //{                     
                    //    Page.Response.Write("<script language=javascript>");
                    //    Page.Response.Write("window.open('" + KMI.P3.Constantes.Web.URL.ADSCOPE_URL + "','AdScope');"); ;
                    //    Page.Response.Write("</script>");

                    //}
                    //else
                    //{
                    //    Page.Response.Write("<script language=javascript>");
                    //    Page.Response.Write("	alert(\"" + GestionWeb.GetWebWord(18, WebApplicationParameters.DefaultLanguage) + "\");");
                    //    Page.Response.Write("</script>");
                    //}

                   
                }
                catch (System.Exception)
                {
                    // L'accès est impossible
                    Page.Response.Write("<script language=javascript>");
                    Page.Response.Write("	alert(\"" + GestionWeb.GetWebWord(17, WebApplicationParameters.DefaultLanguage) + "\");");
                    Page.Response.Write("</script>");
                }
            }
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
                //output.Write(" <li><a href=\"javascript:loginChoiceDoPostBack('buttonadexpress','" + KMI.P3.Constantes.Project.ADEXPRESS_ID + "-" + _AdExpressLogin.LoginId + "')\" class=\"adexpresslink\">&nbsp;AdExpress [" + _AdExpressLogin.Label + "]</a></li>");
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
                output.Write(" <li><a href=\"#\" onclick=\"javascript:window.open('" + KMI.P3.Constantes.Web.URL.ADEXPRESS_URL + "?p=33SS','AdScope');\" >&nbsp;AdScope [" + adScopeLogin.Login + "]</a></li>");
            if (_AdExpressLogin.AdScopeExternalLoginList.Count > 0)
            {
                output.Write(" </ul>");
                output.Write(" </div>");
            }

            //DataSet ds = _webSession.CustomerLogin.GetLoginsParams(_webSession.CustomerLogin.Login, _webSession.CustomerLogin.PassWord);
            //if (ds != null && ds.Tables[0].Rows.Count > 0) {

            //    output.Write(" <div id=\"listLoginAdexpress\" class=\"menulogins\">");
            //    output.Write("<ul>");
            //    foreach (DataRow dr in ds.Tables[0].Rows)
            //    {
            //        output.Write(" <li><a href=\"javascript:loginChoiceDoPostBack('buttonadscope','" + KMI.P3.Constantes.Project.P3_ID + "-" + _webSession.CustomerLogin.IdLogin + "')\" >&nbsp;AdScope [" + dr["login"].ToString() + "]</a></li>");
            //    }
            //    output.Write(" </ul>");
            //    output.Write(" </div>");
            //}
            #endregion

            output.Write(" </td></tr>");
            output.Write(" </table>");
           
        }
        #endregion
    }
}

using System.ComponentModel;
using System.Web.UI;
using KMI.PromoPSA.Web.Core.Sessions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KMI.PromoPSA.Web.Domain;
using KMI.PromoPSA.Web.Domain.Translation;
using TNS.FrameWork.Exceptions;
using KMI.PromoPSA.Rules;
using KMI.PromoPSA.BusinessEntities;

namespace KMI.PromoPSA.Web.Controls.Header {
    /// <summary>
    /// Header Banner Information
    /// </summary>
    [ToolboxData("<{0}:PromotionInformationWebControl runat=server></{0}:PromotionInformationWebControl>")]
    public class PromotionInformationWebControl : System.Web.UI.WebControls.WebControl{

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

        #region Accessors
        /// <summary>
        /// Get / Set Site Language
        /// </summary>
        [Bindable(true),
        Description("WebSession"),
        DefaultValue(0)]
        public WebSession WebSession {
            get { return _webSession; }
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
        /// 
        /// </summary>
        /// <param name="e"></param>
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

            try {

                #region Variables
                
                IResults results = new Results();
                var list = results.GetAdverts(201309);
                int promoNumber = list.Count;
                #endregion

                if (_webSession != null) {

                    StringBuilder html = new StringBuilder();

                    html.Append("<table class=\"headerPromotionInformation\" cellspacing=\"0\" cellpadding=\"0\" border=\"0\" height=\"100%\"><tr>");
                    html.Append("<td>");
                    html.Append("<table cellspacing=\"0\" cellpadding=\"0\" border=\"0\" height=\"100%\"><tr>");
                    html.AppendFormat("<td>");
                    html.AppendFormat("<img src=\"{0}\" align=\"absBottom\"/>", _imageUrl);
                    html.AppendFormat("</td>");
                    html.Append("<td>");
                    html.Append(GestionWeb.GetWebWord(168, _webSession.SiteLanguage) + " : <font id=\"promotionsNb\"><strong>" + promoNumber + "</strong></font>");
                    html.Append("</td>");
                    html.Append("</tr></table>");
                    html.Append("</td></tr></table>");

                    output.Write(html.ToString());

                }

            }
            catch (Exception err) {
                output.Write(OnError(err, _webSession));
            }
        }
        #endregion

        #endregion

        #region On Error
        /// <summary>
        /// Appelé sur erreur à l'exécution des méthodes Ajax
        /// </summary>
        /// <param name="errorException">Exception</param>
        /// <param name="customerSession">Session utilisateur</param>
        /// <returns>Message d'erreur</returns>
        protected string OnError(Exception errorException, WebSession customerSession) {
            KMI.PromoPSA.Web.Exceptions.CustomerWebException cwe = null;
            try {
                BaseException err = (BaseException)errorException;
                cwe = new KMI.PromoPSA.Web.Exceptions.CustomerWebException(err.Message, err.GetHtmlDetail(), customerSession);
            }
            catch (System.Exception) {
                try {
                    cwe = new KMI.PromoPSA.Web.Exceptions.CustomerWebException(errorException.Message, errorException.StackTrace, customerSession);
                }
                catch (System.Exception es) {
                    throw (es);
                }
            }
            cwe.SendMail();
            return GetMessageError(customerSession, 62);
        }
        /// <summary>
        /// Message d'erreur
        /// </summary>
        /// <param name="customerSession">Session du client</param>
        /// <param name="code">Code message</param>
        /// <returns>Message d'erreur</returns>
        protected string GetMessageError(WebSession customerSession, int code) {
            string errorMessage = "<div align=\"center\" class=\"txtViolet11Bold\">";
            if (customerSession != null)
                errorMessage += GestionWeb.GetWebWord(code, customerSession.SiteLanguage) + ". ";
            else
                errorMessage += GestionWeb.GetWebWord(code, 33) + ". ";

            errorMessage += "</div>";
            return errorMessage;
        }
        #endregion

    }
}

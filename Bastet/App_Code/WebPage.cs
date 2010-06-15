#region Informations
// Auteur: B. Masson, G.Facon
// Date de création: 18/11/2005
// Date de modification: 
// 22/11/2005 Par B.Masson > Ajout de la gestion d'erreur et d'envoi du mail d'erreur
#endregion

using System;
using System.Text;
using TNS.FrameWork;
using TNS.FrameWork.Exceptions;
using TNSMail = TNS.FrameWork.Net.Mail;
using Localization = Bastet.Localization;
using TNS.AdExpress.Bastet.Translation;
using TNS.AdExpress.Bastet;
using System.Web;
using TNS.AdExpress.Bastet.Web;

namespace BastetWeb{
	/// <summary>
	/// Classe de base d'une page Web de Bastet
	/// </summary>
    public class WebPage : System.Web.UI.Page {

		#region Variables
        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage=33;
        /// <summary>
        /// Specify whereas to use themes or not
        /// </summary>
        public bool _useThemes = true;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        public WebPage() {
            try {
                if(HttpContext.Current.Request.QueryString.Get("siteLanguage")!=null) {
                    _siteLanguage=int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage"));
                }
                else {
                    _siteLanguage = WebApplicationParameters.DefaultLanguage;
                }
            }
            catch(System.Exception) {
                _siteLanguage = WebApplicationParameters.DefaultLanguage;
            }
        }
        #endregion

        #region Events

        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e) {
            // TODO Gestion des exceptions
            if (_useThemes)
                this.Theme = WebApplicationParameters.Themes[_siteLanguage].Name;

            this.Response.Charset = WebApplicationParameters.AllowedLanguages[_siteLanguage].Charset;
            this.Response.ContentEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_siteLanguage].ContentEncoding);
        }
        #endregion

        #region OnLoad
        /// <summary>
        /// On Load event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnLoad(EventArgs e) {
            base.OnLoad(e);
            Response.Cache.SetNoStore(); 
            Translate.SetAllTextLanguage(this,_siteLanguage);

            if (!Page.ClientScript.IsClientScriptBlockRegistered("CookiesJScript")) {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "CookiesJScript", TNS.AdExpress.Bastet.Functions.Script.CookiesJScript());
            }
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "activateActiveX", ResolveClientUrl("~/scripts/activateActiveX.js"));

        }
        #endregion

        #endregion

	}
}

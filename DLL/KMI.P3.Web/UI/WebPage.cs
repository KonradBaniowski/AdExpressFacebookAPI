using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using KMI.P3.Domain.Web;
using System.Web.UI.WebControls;
namespace KMI.P3.Web.UI
{

    /// <summary>
    /// Type défini pour la sélection d'un type de page
    /// </summary>
    public enum PageType
    {
        /// <summary>
        /// Mode Deconnecte
        /// </summary>
        disconnect,
        /// <summary>
        /// Mode Connecte
        /// </summary>
        connect,
        /// <summary>
        /// Page Contact
        /// </summary>
        contact
    }

    /// <summary>
    /// Base class for public and private P3 page
    /// </summary>
    public class WebPage : System.Web.UI.Page 
    {


        #region Variables
        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage = 33;
        /// <summary>
        /// Specify whereas to use themes or not
        /// </summary>
        public bool _useThemes = true;
        
        /// <summary>
        /// Specify whereas to use themes or not
        /// </summary>
        public PageType PageTypeInfo = PageType.disconnect;
        #endregion

        #region Constructor
        /// <summary>
        /// constructor
        /// </summary>
        public WebPage()
        {
            try
            {
                if (HttpContext.Current.Request.QueryString.Get("siteLanguage") != null)
                {
                    _siteLanguage = int.Parse(HttpContext.Current.Request.QueryString.Get("siteLanguage"));
                }
                else
                {
                    _siteLanguage = WebApplicationParameters.DefaultLanguage;
                }
            }
            catch (System.Exception)
            {
                _siteLanguage = WebApplicationParameters.DefaultLanguage;
            }
        }
        #endregion
        #region On PreInit
        /// <summary>
        /// On preinit event
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            // TODO Gestion des exceptions
            if (_useThemes)
                this.Theme = WebApplicationParameters.Themes[_siteLanguage].Name;

            this.Response.Charset = WebApplicationParameters.AllowedLanguages[_siteLanguage].Charset;
            this.Response.ContentEncoding = Encoding.GetEncoding(WebApplicationParameters.AllowedLanguages[_siteLanguage].ContentEncoding);
        }
        #endregion

        
    }
}

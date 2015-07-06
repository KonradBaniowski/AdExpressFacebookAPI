
#region Informations
// Auteur: D. Mussuma
// Date de cr�ation : 15/01/2007
// Date de modification : 
#endregion

using System;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;

namespace AdExpress.Private.Alerts
{
    /// <summary>
    /// Description r�sum�e de AlertsDownloadCreationsPopUp.
    /// </summary>
    public partial class AlertsDownloadCreationsPopUp : System.Web.UI.Page
    {

        /// <summary>
        /// Titre de la PopUp
        /// </summary>
        public string title = "";

        /// <summary>
        /// Code html de fermeture du flash d'attente
        /// </summary>
        public string divClose = LoadingSystem.GetHtmlCloseDiv();

        /// <summary>
        /// Affiche les cr�ations
        /// </summary>

        /// <summary>
        /// Langue du site
        /// </summary>
        public int _siteLanguage = 33;

        /// <summary>
        /// D�termine la valeur de PostBack
        /// Initialise la propri�t� CustomerSession des composants "options de r�sultats" et gestion de la navigation"
        /// </summary>
        /// <returns>DeterminePostBackMode</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            return tmp;
        }

        /// <summary>
        /// Chargement
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (Page.Request.QueryString.Get("parameters") == null)
            {
                _siteLanguage = TNS.AdExpress.Constantes.DB.Language.FRENCH;
            }
            else
            {
                string[] parametersList = Page.Request.QueryString.Get("parameters").Split(',');
                _siteLanguage = parametersList.Length >= 6 ? int.Parse(parametersList[5]) : TNS.AdExpress.Constantes.DB.Language.FRENCH;
            }

            //Titre de la popUp
            title = GestionWeb.GetWebWord(876, _siteLanguage);


            #region Flash d'attente
            Page.Response.Write(LoadingSystem.GetHtmlDiv(_siteLanguage, Page));
            Page.Response.Flush();
            #endregion
        }

        #region Code g�n�r� par le Concepteur Web Form
        /// <summary>
        /// Initiailisation
        /// </summary>
        /// <param name="e">arguments</param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN�: Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// M�thode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette m�thode avec l'�diteur de code.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion
    }
}

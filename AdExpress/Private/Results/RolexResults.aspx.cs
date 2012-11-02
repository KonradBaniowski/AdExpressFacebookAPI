using System;

namespace Private.Results
{
      /// <summary>
    /// Rolex visibility results page
    /// </summary>
    public partial class RolexResults :  TNS.AdExpress.Web.UI.BaseResultWebPage 
    {
        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public RolexResults()
            : base()
        {
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Evènement de chargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {
                if (!Page.ClientScript.IsClientScriptBlockRegistered("OpenNewWindow"))
                {
                    Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenNewWindow", TNS.AdExpress.Web.Functions.Script.OpenNewWindow());
                }
                if (!Page.IsPostBack)
                {
                    _webSession.LastReachedResultUrl = Page.Request.Url.AbsolutePath;
                    _webSession.ReachedModule = true;
                    _webSession.Save();
                }
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion



        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        protected void Page_UnLoad(object sender, System.EventArgs e)
        {
            if (!Page.ClientScript.IsClientScriptBlockRegistered("popupOpenBis"))
            {
                Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "popupOpenBis", TNS.AdExpress.Web.Functions.Script.ReSizablePopUp());
            }
          
        }
        #endregion

        #region Initialisation
        /// <summary>
        /// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
        /// </summary>
        /// <param name="e"></param>
        override protected void OnInit(EventArgs e)
        {
            _adExpressText1.Code = (int)_currentModule.GetResultPageInformation(_webSession.CurrentTab).IdWebText;
            //
            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
            RolexScheduleContainerWebControl1.WebSession = _webSession;
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            
        }
        #endregion

        #region DeterminePostBack
        /// <summary>
        /// Détermine la valeur de PostBack
        /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
        /// </summary>
        /// <returns>DeterminePostBackMode</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();


            return (tmp);
        }
        #endregion

        #region PreRender
        /// <summary>
        /// PreRendu De la page
        /// </summary>
        /// <param name="e">Arguments</param>
        protected override void OnPreRender(EventArgs e)
        {
            try
            {


            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
        }
        #endregion

        #endregion

        #region Abstract Methods
        /// <summary>
        /// Get next Url from contextual menu
        /// </summary>
        /// <returns></returns>
        protected override string GetNextUrlFromMenu()
        {
            return string.Empty;
        }
        #endregion
    }
}
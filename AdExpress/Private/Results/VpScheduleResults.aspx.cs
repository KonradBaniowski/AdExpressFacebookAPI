#region Informations
// Auteur: G. Facon
// Date de création: 14/04/2006
// Date de modification:
#endregion

#region Namespaces
using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes=TNS.AdExpress.Constantes.Web;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebFunctions=TNS.AdExpress.Web.Functions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using TNS.AdExpress.Web.BusinessFacade.Global.Loading;
using TNS.FrameWork.Date;

using TNS.AdExpress.Web.Controls.Results.MediaPlan;
using AjaxPro;

using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
#endregion

namespace AdExpress.Private.Results{
    /// <summary>
    /// Page de calendrier d'action d'un plan media
    /// </summary>
    public partial class VpScheduleResults : TNS.AdExpress.Web.UI.BaseResultWebPage {

        #region Variables
        
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public VpScheduleResults()
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
        protected void Page_Load(object sender, System.EventArgs e){
            try {
            }
            catch (System.Exception exc) {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException)) {
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
            VpScheduleContainerWebControl1.WebSession = _webSession;
            InitializeComponent();
            base.OnInit(e);
        }

        /// <summary>
        /// Méthode requise pour la prise en charge du concepteur - ne modifiez pas
        /// le contenu de cette méthode avec l'éditeur de code.
        /// </summary>
        private void InitializeComponent()
        {
            this.Unload += new System.EventHandler(this.Page_UnLoad);
        }
        #endregion

        #region DeterminePostBack
        /// <summary>
        /// Détermine la valeur de PostBack
        /// Initialise la propriété CustomerSession des composants "options de résultats" et gestion de la navigation"
        /// </summary>
        /// <returns>DeterminePostBackMode</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode(){
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
            return string.Empty;//this.MenuWebControl2.NextUrl;
        }
        #endregion

    }
}

#region Informations
// Auteur: b.Masson
// Date de création: 20/06/2005
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions = TNS.AdExpress.Web.DataAccess.Functions;
using WebBF = TNS.AdExpress.Web.BusinessFacade;

namespace AdExpress.Private.Results.RawExcel
{
    /// <summary>
    /// Sortie Excel des données brutes de l'alerte concurentielle
    /// </summary>
    public partial class CompetitorAlertResults : TNS.AdExpress.Web.UI.ExcelWebPage
    {

        #region Variables
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string result = "";
        /// <summary>
        /// Identifiant de session
        /// </summary>
        public string idsession = "";
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public CompetitorAlertResults()
            : base()
        {
            idsession = HttpContext.Current.Request.QueryString.Get("idSession");
        }
        #endregion

        #region Chargement de la page
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                Response.ContentType = "application/vnd.ms-excel";

                #region Calcul du résultat
                // On charge les données
                //result=WebBF.Results.CompetitorSystem.GetRawExcel(this.Page,_webSession);
                #endregion

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

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation
        /// </summary>
        /// <param name="e">Arguments</param>
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN : Cet appel est requis par le Concepteur Web Form ASP.NET.
            //
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

        #region Déchargement de la page
        /// <summary>
        /// Evènement de déchargement de la page
        /// </summary>
        /// <param name="sender">Objet qui lance l'évènement</param>
        /// <param name="e">Arguments</param>
        private void Page_UnLoad(object sender, System.EventArgs e)
        {
        }
        #endregion

        #region DetermeinePostBack
        /// <summary>
        /// DeterminePostBackMode
        /// </summary>
        /// <returns>PostBackMode</returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            try
            {
                ResultWebControl1.CustomerWebSession = _webSession;
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            return tmp;
        }
        #endregion

    }
}

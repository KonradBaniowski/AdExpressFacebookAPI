#region Information
// Auteur B.Masson
// Date de création : 15/11/04
//date de modification : 30/12/2004  D. Mussuma Intégration de WebPage
#endregion

#region Namespace
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
using TNS.FrameWork.WebResultUI;
using CstPreformatedDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;

#endregion

namespace AdExpress.Private.Results.Excel
{
    /// <summary>
    /// Excel report of product class analysis
    /// </summary>
    public partial class ProductClassReport : TNS.AdExpress.Web.UI.ExcelWebPage
    {


        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public ProductClassReport()
            : base()
        {
        }
        #endregion

        #region Evènements

        #region Chargement de la page
        /// <summary>
        /// Chargement de la page
        /// </summary>
        /// <param name="sender">page</param>
        /// <param name="e">arguments</param>
        protected void Page_Load(object sender, System.EventArgs e)
        {
            try
            {

                Response.ContentType = "application/vnd.ms-excel";

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

        #region DeterminePostBackMode
        /// <summary>
        /// Evaluation de l'évènement PostBack:
        ///		base.DeterminePostBackMode();
        ///		Initialisation de la session ds les composants 'options de resultats" et "gestion de la navigation"
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

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Evènement d'initialisation
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

        #region PreInit
        protected override void OnPreInit(EventArgs e)
        {
            try
            {
                base.OnPreInit(e);
                switch (_webSession.PreformatedTable)
                {
                    case CstPreformatedDetail.PreformatedTables.media_X_Year:
                    case CstPreformatedDetail.PreformatedTables.product_X_Year:
                        ResultWebControl1.SkinID = "productClassResultTableClassif1XYearXL";
                        break;
                    case CstPreformatedDetail.PreformatedTables.productYear_X_Media:
                    case CstPreformatedDetail.PreformatedTables.productYear_X_Cumul:
                    case CstPreformatedDetail.PreformatedTables.productYear_X_Mensual:
                    case CstPreformatedDetail.PreformatedTables.mediaYear_X_Cumul:
                    case CstPreformatedDetail.PreformatedTables.mediaYear_X_Mensual:
                        ResultWebControl1.SkinID = "productClassResultTableProductXMediaXL";
                        break;
                    default:
                        ResultWebControl1.SkinID = "productClassResultTableXL";
                        break;
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

        #endregion

    }
}

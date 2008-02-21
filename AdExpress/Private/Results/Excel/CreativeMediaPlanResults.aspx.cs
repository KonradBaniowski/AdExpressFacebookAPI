#region Informations
// Auteur: F. Facon
// Date de création: 26/10/2005 
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
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Affichage d'une version Excel d'un plan media pour Creative
	/// </summary>
	public partial class CreativeMediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result="";
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession="";
		#endregion

		#region Constructeur
		/// <summary>
		/// Constructeur
		/// </summary>
		public CreativeMediaPlanResults():base(){
			idsession=HttpContext.Current.Request.QueryString.Get("idSession");
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

                #region Calcul du résultat
                MediaSchedulePeriod period = null;
                object[,] tab = null;


                period = new MediaSchedulePeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, _webSession.DetailPeriod);

                tab = GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);

                if (_webSession.IdSlogans != null && _webSession.IdSlogans.Count > 0 && tab.GetLength(0) == 0) {
                    _webSession.IdSlogans = new ArrayList();
                    tab = GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);
                }

                result = GenericMediaScheduleUI.GetExcel(tab, _webSession, period, "", false, (int)_webSession.DetailPeriod,true).HTMLCode;
                #endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
                    WebFunctions.CreativeErrorTreatment.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession), Page, _webSession.SiteLanguage);
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
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

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
	}
}

#region Informations
// Auteur: F. Facon
// Date de cr�ation: 26/10/2005 
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Reflection;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.MediaSchedule;
using TNS.AdExpress.Domain.Web.Navigation;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Affichage d'une version Excel d'un plan media pour Creative
	/// </summary>
	public partial class CreativeMediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{

		#region Variables
		/// <summary>
		/// Code HTML du r�sultat
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
            this._useThemes = false;
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_Load(object sender, System.EventArgs e){
			try {

                this.Response.ContentType = "application/vnd.ms-excel";

                #region Calcul du r�sultat
                MediaSchedulePeriod period = null;
                MediaScheduleData resultTmp = null;
                object[,] tab = null;


                period = new MediaSchedulePeriod(_webSession.PeriodBeginningDate, _webSession.PeriodEndDate, _webSession.DetailPeriod);

                //tab = GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);

                //if (_webSession.IdSlogans != null && _webSession.IdSlogans.Count > 0 && tab.GetLength(0) == 0) {
                //    _webSession.IdSlogans = new ArrayList();
                //    tab = GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1);
                //}

                //result = GenericMediaScheduleUI.GetExcel(tab, _webSession, period, "", false, (int)_webSession.DetailPeriod,true).HTMLCode;
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                object[] param = new object[2];
                param[0] = _webSession;
                param[1] = period;
                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                resultTmp = mediaScheduleResult.GetExcelHtmlCreativeDivision(false);

                /* When we don't have a media schedule for a list of versions we show the message : 'no result for this selection'
                 * in the old version we switched to the product media schedule
                 * */
                //if (resultTmp.HTMLCode.Length <= 0)
                //{
                //    _webSession.IdSlogans = new ArrayList();
                //    resultTmp = mediaScheduleResult.GetExcelHtmlCreativeDivision(false);
                //}
                result += resultTmp.HTMLCode;
                #endregion

			}
			catch(System.Exception exc){
				if (exc.GetType() != typeof(System.Threading.ThreadAbortException)){
                    WebFunctions.CreativeErrorTreatment.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession), Page, _webSession.SiteLanguage);
				}
			}
		}

		#endregion

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		private void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion

		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation
		/// </summary>
		/// <param name="e">Arguments</param>
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

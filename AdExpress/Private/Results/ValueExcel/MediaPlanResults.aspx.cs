#region Informations
// Auteur: G. Facon
// Date de création: 12/01/2006
// Date de modification: 
#endregion

#region Using
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
using TNS.AdExpress.Web.Core.Selection;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Web.DataAccess.Results;
using TNS.AdExpress.Web.Rules.Results;
using TNS.AdExpress.Web.UI.Results;
using DBFunctions=TNS.AdExpress.Web.DataAccess.Functions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using ConstantePeriods = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFunctions = TNS.AdExpress.Web.Functions;
#endregion

namespace AdExpress.Private.Results.ValueExcel{
	/// <summary>
	/// Page Excel d'un calendrier d'action d'un plan Média
	/// </summary>
	public partial class MediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{
		
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
		public MediaPlanResults():base(){
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
        protected void MyPage_Load(object sender, System.EventArgs e)
        {

            WebConstantes.CustomerSessions.Period.Type periodType = _webSession.PeriodType;
            WebConstantes.CustomerSessions.Period.DisplayLevel periodDisplayLevel = _webSession.DetailPeriod;
            string periodBegSave = _webSession.PeriodBeginningDate;
            string periodEndSave = _webSession.PeriodEndDate;
            MediaSchedulePeriod period = null;
            string zoomDate = string.Empty;
            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                #region Period Detail
                zoomDate = Page.Request.QueryString.Get("zoomDate");

                DateTime begin;
                DateTime end;
                if (zoomDate != null && zoomDate != string.Empty)
                {
                    if (_webSession.DetailPeriod == ConstantePeriods.DisplayLevel.weekly)
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantePeriods.Type.dateToDateWeek);
                        end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantePeriods.Type.dateToDateWeek);
                    }
                    else
                    {
                        begin = WebFunctions.Dates.getPeriodBeginningDate(zoomDate, ConstantePeriods.Type.dateToDateMonth);
                        end = WebFunctions.Dates.getPeriodEndDate(zoomDate, ConstantePeriods.Type.dateToDateMonth);
                    }
                    begin = WebFunctions.Dates.Max(begin,
                        WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType));
                    end = WebFunctions.Dates.Min(end,
                        WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType));

                    _webSession.DetailPeriod = ConstantePeriods.DisplayLevel.dayly;
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriods.DisplayLevel.dayly);

                }
                else
                {
                    begin = WebFunctions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                    end = WebFunctions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                    if (_webSession.DetailPeriod == ConstantePeriods.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _webSession.DetailPeriod = ConstantePeriods.DisplayLevel.monthly;
                    }
                    period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod);

                }
                #endregion

                #region Calcul du résultat
                // On charge les données
                result = GenericMediaScheduleUI.GetExcel(GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1), _webSession, period, zoomDate, true,(int)periodDisplayLevel).HTMLCode;
                #endregion

            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                }
            }
            finally
            {
                _webSession.DetailPeriod = periodDisplayLevel;
                _webSession.PeriodType = periodType;
                _webSession.PeriodBeginningDate = periodBegSave;
                _webSession.PeriodEndDate = periodEndSave;
            }

        }

		#endregion

		#region Déchargement de la page
		/// <summary>
		/// Evènement de déchargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'évènement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// Détermine la valeur de la propriété PostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

	
		#endregion

		#region Code généré par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
            this.Load += new System.EventHandler(this.MyPage_Load);
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

	}
}

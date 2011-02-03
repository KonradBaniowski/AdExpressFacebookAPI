#region Informations
// Auteur: G. Facon
// Date de cr�ation: 12/01/2006
// Date de modification: 
#endregion

using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using ConstantePeriods = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFunctions = TNS.AdExpress.Web.Functions;

using System.Reflection;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.MediaSchedule;

namespace AdExpress.Private.Results.ValueExcel{
	/// <summary>
	/// Page Excel d'un calendrier d'action d'un plan M�dia
	/// </summary>
	public partial class MediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{
		
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
		public MediaPlanResults():base(){
			idsession=HttpContext.Current.Request.QueryString.Get("idSession");
		}
		#endregion

		#region Ev�nements

		#region Chargement de la page
		/// <summary>
		/// Ev�nement de chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
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
                    if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule && _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, ConstantePeriods.DisplayLevel.dayly, _webSession.ComparativePeriodType);
                    else
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
                    if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule && _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod, _webSession.ComparativePeriodType);
                    else
                        period = new MediaSchedulePeriod(begin, end, _webSession.DetailPeriod);

                }
                #endregion

                #region Calcul du r�sultat
                // On charge les donn�es
                //result = GenericMediaScheduleUI.GetExcel(GenericMediaPlanRules.GetFormattedTableWithMediaDetailLevel(_webSession, period, -1), _webSession, period, zoomDate, true,(int)periodDisplayLevel).HTMLCode;
                object[] param = null;
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                if (zoomDate != null && zoomDate.Length > 0)
                {
                    param = new object[3];
                    param[2] = zoomDate;
                }
                else
                {
                    param = new object[2];
                }
                param[0] = _webSession;
                param[1] = period;
                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null, null);
                result = mediaScheduleResult.GetExcelHtml(true).HTMLCode;

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

		#region D�chargement de la page
		/// <summary>
		/// Ev�nement de d�chargement de la page
		/// </summary>
		/// <param name="sender">Objet qui lance l'�v�nement</param>
		/// <param name="e">Arguments</param>
		protected void Page_UnLoad(object sender, System.EventArgs e){			
		}
		#endregion
		
		#region DeterminePostBack
		/// <summary>
		/// D�termine la valeur de la propri�t� PostBack
		/// </summary>
		/// <returns></returns>
		protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode() {
			System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode ();
			return tmp;
		}
		#endregion

	
		#endregion

		#region Code g�n�r� par le Concepteur Web Form
		/// <summary>
		/// Initialisation des controls de la page (ViewState et valeurs modifi�es pas encore charg�s)
		/// </summary>
		/// <param name="e"></param>
		override protected void OnInit(EventArgs e){
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
		private void InitializeComponent(){
            this.Load += new System.EventHandler(this.MyPage_Load);
			this.Unload += new System.EventHandler(this.Page_UnLoad);

		}
		#endregion

	}
}

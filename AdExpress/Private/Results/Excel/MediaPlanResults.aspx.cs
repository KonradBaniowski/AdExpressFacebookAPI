#region Informations
// Auteur: A. Obermeyer
// Date de création: 
// Date de modification: 
//		30/12/2004 A. Obermeyer Intégration de WebPage
#endregion

using System;
using System.Web;
using TNS.AdExpress.Web.Core.Selection;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using ConstantePeriods = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using WebFunctions = TNS.AdExpress.Web.Functions;

using System.Reflection;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.MediaSchedule;

namespace AdExpress.Private.Results.Excel{
	/// <summary>
	/// Page Excel d'un calendrier d'action d'un plan Média
	/// </summary>
	public partial class MediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage{
		
		#region Variables
		/// <summary>
		/// Code HTML du résultat
		/// </summary>
		public string result=string.Empty;
		/// <summary>
		/// Identifiant de session
		/// </summary>
		public string idsession=string.Empty;

	    private string _idUnit = string.Empty;
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
            long oldModuleId = _webSession.CurrentModule;
            long oldCurrentTab = _webSession.CurrentTab;
            System.Windows.Forms.TreeNode oldReferenceUniversMedia = _webSession.ReferenceUniversMedia;
            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                #region Period Detail
                zoomDate = Page.Request.QueryString.Get("zoomDate");

                _idUnit = Page.Request.QueryString.Get("u");
                if (!string.IsNullOrEmpty(_idUnit))
                    _webSession.Unit = (WebConstantes.CustomerSessions.Unit)int.Parse(_idUnit);

                DateTime begin;
                DateTime end;
                if (!string.IsNullOrEmpty(zoomDate))
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

                #region Calcul du résultat
                // On charge les données              
                object[] param = null;
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(_webSession.CurrentModule);
                if (module.Id != WebConstantes.Module.Name.BILAN_CAMPAGNE){                   
                    module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                    _webSession.CurrentTab = 0;
                    _webSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
                }
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                if (!string.IsNullOrEmpty(zoomDate))
                {
                    param = new object[3];
                    param[2] = zoomDate;
                }
                else
                {
                    param = new object[2];
                }
                _webSession.CurrentModule = module.Id;
                param[0] = _webSession;
                param[1] = period;
                var mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap
                    (string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, module.CountryRulesLayer.AssemblyName),
                    module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                mediaScheduleResult.Module = module;
                result = mediaScheduleResult.GetExcelHtml(false).HTMLCode;

                #endregion

            }
            catch (Exception exc)
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
                _webSession.CurrentModule = oldModuleId;
                _webSession.CurrentTab = oldCurrentTab;
                _webSession.ReferenceUniversMedia = oldReferenceUniversMedia;
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

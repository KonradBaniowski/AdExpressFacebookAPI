using System;
using System.Reflection;
using System.Web;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpressI.MediaSchedule;

namespace Private.Results.RawExcel
{
    public partial class MediaPlanResults : TNS.AdExpress.Web.UI.ExcelWebPage
    {
    
        #region Variables
        /// <summary>
        /// Code HTML du résultat
        /// </summary>
        public string result = "";    
        #endregion

        #region Constructeur
        /// <summary>
        /// Constructeur
        /// </summary>
        public MediaPlanResults()
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

            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.Type periodType = _webSession.PeriodType;
            TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel periodDisplayLevel = _webSession.DetailPeriod;
            string periodBegSave = _webSession.PeriodBeginningDate;
            string periodEndSave = _webSession.PeriodEndDate;
            string zoomDate;
            long oldModuleId = _webSession.CurrentModule;
            long oldCurrentTab = _webSession.CurrentTab;
            System.Windows.Forms.TreeNode oldReferenceUniversMedia = _webSession.ReferenceUniversMedia;

            try
            {
                Response.ContentType = "application/vnd.ms-excel";

                #region Period Detail
                zoomDate = Page.Request.QueryString.Get("zoomDate");

                DateTime begin;
                DateTime end;
                MediaSchedulePeriod period = null;
                if (!string.IsNullOrEmpty(zoomDate))
                {
                    if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.weekly)
                    {
                        begin = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(zoomDate, CustomerSessions.Period.Type.dateToDateWeek);
                        end = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(zoomDate, CustomerSessions.Period.Type.dateToDateWeek);
                    }
                    else
                    {
                        begin = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(zoomDate, CustomerSessions.Period.Type.dateToDateMonth);
                        end = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(zoomDate, CustomerSessions.Period.Type.dateToDateMonth);
                    }
                    begin = TNS.AdExpress.Web.Functions.Dates.Max(begin,
                        TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType));
                    end = TNS.AdExpress.Web.Functions.Dates.Min(end,
                        TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType));

                    _webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.dayly;
                    if (_webSession.ComparativeStudy && TNS.AdExpress.Domain.Web.WebApplicationParameters.UseComparativeMediaSchedule && _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, CustomerSessions.Period.DisplayLevel.dayly, _webSession.ComparativePeriodType);
                    else
                        period = new MediaSchedulePeriod(begin, end, CustomerSessions.Period.DisplayLevel.dayly);

                }
                else
                {
                    begin = TNS.AdExpress.Web.Functions.Dates.getPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                    end = TNS.AdExpress.Web.Functions.Dates.getPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                    if (_webSession.DetailPeriod == CustomerSessions.Period.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _webSession.DetailPeriod = CustomerSessions.Period.DisplayLevel.monthly;
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
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA);
                _webSession.CurrentTab = 0;
                _webSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
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
                IMediaScheduleResults mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                result = mediaScheduleResult.GetRawExcel(false).HTMLCode;

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
                _webSession.CurrentModule = oldModuleId;
                _webSession.CurrentTab = oldCurrentTab;
                _webSession.ReferenceUniversMedia = oldReferenceUniversMedia;
            }

        }

        #endregion

      
        #region DeterminePostBack
        /// <summary>
        /// Détermine la valeur de la propriété PostBack
        /// </summary>
        /// <returns></returns>
        protected override System.Collections.Specialized.NameValueCollection DeterminePostBackMode()
        {
            System.Collections.Specialized.NameValueCollection tmp = base.DeterminePostBackMode();
            return tmp;
        }
        #endregion


        #endregion

        #region Code généré par le Concepteur Web Form
        /// <summary>
        /// Initialisation des controls de la page (ViewState et valeurs modifiées pas encore chargés)
        /// </summary>
        /// <param name="e"></param>
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
            this.Load += new System.EventHandler(this.Page_Load);
          

        }
        #endregion
    }
}
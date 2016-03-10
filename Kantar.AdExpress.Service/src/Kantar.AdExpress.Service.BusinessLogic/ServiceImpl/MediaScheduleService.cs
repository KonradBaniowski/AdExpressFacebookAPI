#define DEBUG
using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.MediaSchedule;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using ConstantePeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using System.Reflection;
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaScheduleService : IMediaScheduleService
    {
        private WebSession CustomerSession = null;

        public object[,] GetMediaScheduleData(string idWebSession)
        {
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, "");
            
            return mediaScheduleResult.ComputeData();
        }

        public GridResult GetGridResult(string idWebSession, string periodType)
        {
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, periodType);

            return mediaScheduleResult.GetGridResult();
        }

        private IMediaScheduleResults InitMediaScheduleCall(string idWebSession, string periodType)
        {
            //CustomerSession = (WebSession)WebSession.Load("201603021047501084");
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

#if DEBUG
            //TODO : Mock selection marché : a supprimer dès que page marché terminée
            TNS.AdExpress.Classification.AdExpressUniverse universe = new TNS.AdExpress.Classification.AdExpressUniverse("test", TNS.Classification.Universe.Dimension.product);
            var group = new TNS.Classification.Universe.NomenclatureElementsGroup("Annonceur", 0, TNS.Classification.Universe.AccessType.includes);
            group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER, "54410,34466,7798,50270,71030");
            universe.AddGroup(universe.Count(), group);
            var universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            universeDictionary.Add(universeDictionary.Count, universe);
            CustomerSession.PrincipalProductUniverses = universeDictionary;
            ArrayList levels = new ArrayList();
            // Media/catégorie/Support/Annonceur
            levels.Add(1);
            levels.Add(2);
            levels.Add(3);
            levels.Add(8);
            CustomerSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            CustomerSession.Save();
#endif


            TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
            MediaScheduleData result = null;
            MediaSchedulePeriod period = null;
            Int64 moduleId = CustomerSession.CurrentModule;
            
            WebConstantes.CustomerSessions.Unit oldUnit = CustomerSession.Unit;
            // TODO : Commented temporarily for new AdExpress
            //if (UseCurrentUnit) webSession.Unit = CurrentUnit;
            object[] param = null;
            long oldCurrentTab = CustomerSession.CurrentTab;
            System.Windows.Forms.TreeNode oldReferenceUniversMedia = CustomerSession.ReferenceUniversMedia;

            if(!String.IsNullOrEmpty(periodType))
            {
                switch (periodType)
                {
                    case "Mois": CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly; break;
                    case "Semaine": CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.weekly; break;
                    case "Jour": CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly; break;
                }
            }
            ConstantePeriod.DisplayLevel periodDisplay = CustomerSession.DetailPeriod;

            #region Period Detail
            DateTime begin;
            DateTime end;
            string _zoomDate = string.Empty;
            if (!string.IsNullOrEmpty(_zoomDate))
            {
                if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = Dates.getPeriodBeginningDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = Dates.getPeriodEndDate(_zoomDate, ConstantePeriod.Type.dateToDateMonth);
                }
                begin = Dates.Max(begin,
                    Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType));
                end = Dates.Min(end,
                    Dates.getPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType));

                CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                if (CustomerSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && CustomerSession.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, CustomerSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

            }
            else
            {
                begin = Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType);
                end = Dates.getPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType);
                if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.dayly && begin < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.monthly;
                }

                if (CustomerSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && CustomerSession.CurrentModule
                    == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                    period = new MediaSchedulePeriod(begin, end, CustomerSession.DetailPeriod, CustomerSession.ComparativePeriodType);
                else
                    period = new MediaSchedulePeriod(begin, end, CustomerSession.DetailPeriod);

            }
            #endregion

            if (_zoomDate.Length > 0)
            {
                param = new object[3];
                param[2] = _zoomDate;
            }
            else
            {
                param = new object[2];
            }
            CustomerSession.CurrentModule = module.Id;
            if (CustomerSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) CustomerSession.CurrentTab = 0;
            CustomerSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
            param[0] = CustomerSession;
            param[1] = period;
            var mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, module.CountryRulesLayer.AssemblyName), module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            mediaScheduleResult.Module = module;

            return mediaScheduleResult;
        }

    
    }
}

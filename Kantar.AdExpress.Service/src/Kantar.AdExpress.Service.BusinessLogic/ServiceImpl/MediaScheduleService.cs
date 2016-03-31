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
using CustomCst = TNS.AdExpress.Constantes.Customer;
using System.Reflection;
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Insertions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Insertions.Cells;
using TNS.AdExpressI.MediaSchedule.Style;

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

        public GridResult GetGridResult(string idWebSession, string zoomDate)
        {
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate);

            return mediaScheduleResult.GetGridResult();
        }

        public MSCreatives GetMSCreatives(string idWebSession, string zoomDate)
        {
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            #region MSCreatives
            object[] paramMSCraetives = new object[2];
            paramMSCraetives[0] = CustomerSession;
            paramMSCraetives[1] = CustomerSession.CurrentModule;

            MediaSchedulePeriod period = new MediaSchedulePeriod(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodEndDate, CustomerSession.DetailPeriod);

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.insertions];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the insertions rules"));
            var resultMSCreatives = (IInsertionsResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + cl.AssemblyName, cl.Class, false, System.Reflection.BindingFlags.CreateInstance
                | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Public, null, paramMSCraetives, null, null);

            ResultTable data = null;
            string[] vehicles = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
            string filters = string.Empty;
            int fromDate = Convert.ToInt32(period.Begin.ToString("yyyyMMdd"));
            int toDate = Convert.ToInt32(period.End.ToString("yyyyMMdd"));
            #endregion

            VehicleInformation vehicle = VehiclesInformation.Get(Int64.Parse(vehicles[0]));
            data = resultMSCreatives.GetMSCreatives(vehicle, fromDate, toDate, filters, -1, zoomDate);

            MSCreatives creatives = new MSCreatives();
            creatives.Items = new List<MSCreative>();
            DefaultMediaScheduleStyle style = new DefaultMediaScheduleStyle();
            Int64 nbVisuals = 0;
            List<string> visuals;

            creatives.VehicleId = vehicle.Id;

            for (int i = 0; i < data.LinesNumber; i++)
            {
                CellCreativesInformation cell = (CellCreativesInformation)data[i, 1];
                nbVisuals = 0;
                visuals = new List<string>();

                // Limit to 12 visuals
                if (cell.NbVisuals <= 12)
                {
                    nbVisuals = cell.NbVisuals;
                    visuals = cell.Visuals;
                }
                else
                {
                    nbVisuals = 12;
                    visuals = cell.Visuals.Take(12).ToList();
                }

                MSCreative creative = new MSCreative { Id = cell.IdVersion, SessionId = CustomerSession.IdSession, Vehicle = cell.Vehicle, NbVisuals = nbVisuals, Visuals = visuals };
                creative.Class = CustomerSession.SloganColors[cell.IdVersion].ToString();
                creatives.Items.Add(creative);
            }

            if (vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.outdoor
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.mailValo)
                creatives.Items = creatives.Items.OrderBy(i => i.NbVisuals).ToList();
            else
                creatives.Items = creatives.Items.OrderByDescending(i => i.NbVisuals).ToList();

            return creatives;
        }

        public void SetMSCreatives(string idWebSession, ArrayList slogans) {

            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            CustomerSession.IdSlogans = slogans;
            CustomerSession.Save();
            CustomerSession.Source.Close();

        }

        private IMediaScheduleResults InitMediaScheduleCall(string idWebSession, string zoomDate)
        {
           
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

#if DEBUG
            //TODO : Mock selection marché : a supprimer dès que page marché terminée
           //TNS.AdExpress.Classification.AdExpressUniverse universe = new TNS.AdExpress.Classification.AdExpressUniverse("test", TNS.Classification.Universe.Dimension.product);
            //var group = new TNS.Classification.Universe.NomenclatureElementsGroup("Annonceur", 0, TNS.Classification.Universe.AccessType.includes);
            //group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER, "54410,34466,7798,50270,71030");
            //universe.AddGroup(universe.Count(), group);
            //var universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            //universeDictionary.Add(universeDictionary.Count, universe);
            //CustomerSession.PrincipalProductUniverses = universeDictionary;
            //ArrayList levels = new ArrayList();
            //// Media/catégorie/Support/Annonceur
            //levels.Add(1);
            //levels.Add(2);
            //levels.Add(3);
            //levels.Add(8);
            //CustomerSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
            //CustomerSession.Save();
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

            

            #region Period Detail
            DateTime begin;
            DateTime end;
            if (!string.IsNullOrEmpty(zoomDate))
            {
                if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                {
                    begin = Dates.getPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    end = Dates.getPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                }
                else
                {
                    begin = Dates.getPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    end = Dates.getPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
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

            if (zoomDate.Length > 0)
            {
                param = new object[3];
                param[2] = zoomDate;
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

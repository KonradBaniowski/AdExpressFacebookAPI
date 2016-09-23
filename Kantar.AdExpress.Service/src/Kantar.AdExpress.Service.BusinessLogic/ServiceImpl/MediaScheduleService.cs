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
using TNS.AdExpressI.Classification.DAL;
using NLog;
using TNS.AdExpress.Domain.Translation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaScheduleService : IMediaScheduleService
    {
        private WebSession CustomerSession = null;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public object[,] GetMediaScheduleData(string idWebSession)
        {
            object[,] result = null;
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            try
            {
                IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, "", "");
                result = mediaScheduleResult.ComputeData();
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, CustomerSession.UserAgent, CustomerSession.CustomerLogin.Login, CustomerSession.CustomerLogin.PassWord, ex.InnerException +ex.Message, ex.StackTrace,GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(CustomerSession.CurrentModule), CustomerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return result;
        }

        public GridResult GetGridResult(string idWebSession, string zoomDate)
        {
            GridResult girdResult = new GridResult();
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate, "");

            return mediaScheduleResult.GetGridResult();
        }

        public GridResult GetGridResult(string idWebSession, string zoomDate, string idVehicle)
        {
            GridResult girdResult = new GridResult();
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate, idVehicle);

            return mediaScheduleResult.GetGridResult();
        }

        public MSCreatives GetMSCreatives(string idWebSession, string zoomDate)
        {
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            VehicleInformation vehicle = new VehicleInformation();
            MSCreatives result = new MSCreatives();
            ResultTable data = null;
            try
            {

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
                string[] vehicles = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, CustomCst.Right.type.vehicleAccess).Split(',');
                string filters = string.Empty;
                int fromDate = Convert.ToInt32(period.Begin.ToString("yyyyMMdd"));
                int toDate = Convert.ToInt32(period.End.ToString("yyyyMMdd"));
                #endregion

                vehicle = VehiclesInformation.Get(Int64.Parse(vehicles[0]));
                data = resultMSCreatives.GetMSCreatives(vehicle, fromDate, toDate, filters, -1, zoomDate);
                switch (vehicle.Id)
                {
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                        result= GetEvaliantCreatives(data, vehicle);
                        break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                        result= GetEvaliantMobileCreatives(data, vehicle);
                        break;
                    default:
                        result = GetCreatives(data, vehicle);
                        break;
                }
            }
            catch(Exception ex )
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, CustomerSession.UserAgent, CustomerSession.CustomerLogin.Login, CustomerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(CustomerSession.CurrentModule), CustomerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return result;          
        }

        private MSCreatives GetCreatives(ResultTable data, VehicleInformation vehicle)
        {
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
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.dooh
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.directMarketing
                    || vehicle.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.mailValo)
                creatives.Items = creatives.Items.OrderBy(i => i.NbVisuals).ToList();
            else
                creatives.Items = creatives.Items.OrderByDescending(i => i.NbVisuals).ToList();

            return creatives;

        }

        private MSCreatives GetEvaliantCreatives(ResultTable data, VehicleInformation vehicle)
        {
            MSCreatives creatives = new MSCreatives();
            creatives.Items = new List<MSCreative>();
            DefaultMediaScheduleStyle style = new DefaultMediaScheduleStyle();
            Int64 nbVisuals = 0;
            List<string> visuals;

            creatives.VehicleId = vehicle.Id;

            for (int i = 0; i < data.LinesNumber; i++)
            {
                CellCreativesEvaliantInformation cell = (CellCreativesEvaliantInformation)data[i, 1];
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

                MSCreative creative = new MSCreative { Id = cell.IdVersion, SessionId = CustomerSession.IdSession, Vehicle = cell.Vehicle, NbVisuals = nbVisuals, Visuals = visuals, Format = cell.Format, Dimension = cell.Dimension };
                creative.Class = CustomerSession.SloganColors[cell.IdVersion].ToString();
                creatives.Items.Add(creative);
            }

            creatives.Items = creatives.Items.OrderByDescending(i => i.NbVisuals).ToList();

            return creatives;
        }

        private MSCreatives GetEvaliantMobileCreatives(ResultTable data, VehicleInformation vehicle)
        {
            MSCreatives creatives = new MSCreatives();
            creatives.Items = new List<MSCreative>();
            DefaultMediaScheduleStyle style = new DefaultMediaScheduleStyle();
            Int64 nbVisuals = 0;
            List<string> visuals;

            creatives.VehicleId = vehicle.Id;

            for (int i = 0; i < data.LinesNumber; i++)
            {
                CellCreativesEvaliantMobileInformation cell = (CellCreativesEvaliantMobileInformation)data[i, 1];
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

                MSCreative creative = new MSCreative { Id = cell.IdVersion, SessionId = CustomerSession.IdSession, Vehicle = cell.Vehicle, NbVisuals = nbVisuals, Visuals = visuals, Format = cell.Format, Dimension = cell.Dimension };
                creative.Class = CustomerSession.SloganColors[cell.IdVersion].ToString();
                creatives.Items.Add(creative);
            }

            creatives.Items = creatives.Items.OrderByDescending(i => i.NbVisuals).ToList();

            return creatives;
        }

        public void SetMSCreatives(string idWebSession, ArrayList slogans)
        {

            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            CustomerSession.IdSlogans = slogans;
            CustomerSession.Save();
            CustomerSession.Source.Close();

        }

        private IMediaScheduleResults InitMediaScheduleCall(string idWebSession, string zoomDate, string idVehicle)
        {

            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            IMediaScheduleResults mediaScheduleResult = null;
            try
            {
                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                MediaSchedulePeriod period = null;
                Int64 moduleId = CustomerSession.CurrentModule;
                ConstantePeriod.DisplayLevel periodDisplay = CustomerSession.DetailPeriod;
                WebConstantes.CustomerSessions.Unit oldUnit = CustomerSession.Unit;
                // TODO : Commented temporarily for new AdExpress
                //if (UseCurrentUnit) webSession.Unit = CurrentUnit;
                object[] param = null;
                long oldCurrentTab = CustomerSession.CurrentTab;
                System.Windows.Forms.TreeNode oldReferenceUniversMedia = CustomerSession.ReferenceUniversMedia;

                if (!string.IsNullOrEmpty(idVehicle))
                {
                    if (!string.IsNullOrEmpty(zoomDate))
                    {
                        param = new object[4];
                        param[2] = Int64.Parse(idVehicle);
                        param[3] = zoomDate;
                    }
                    else
                    {
                        param = new object[3];
                        param[2] = Int64.Parse(idVehicle);
                    }
                    CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                }
                else
                {
                    if (!string.IsNullOrEmpty(zoomDate))
                    {
                        param = new object[3];
                        param[2] = zoomDate;
                    }
                    else
                    {
                        param = new object[2];
                    }
                }

                #region Period Detail
                DateTime begin;
                DateTime end;
                if (!string.IsNullOrEmpty(zoomDate))
                {
                    if (CustomerSession.DetailPeriod == ConstantePeriod.DisplayLevel.weekly)
                    {
                        begin = Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                        end = Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateWeek);
                    }
                    else
                    {
                        begin = Dates.GetPeriodBeginningDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                        end = Dates.GetPeriodEndDate(zoomDate, ConstantePeriod.Type.dateToDateMonth);
                    }
                    begin = Dates.Max(begin,
                        Dates.GetPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType));
                    end = Dates.Min(end,
                        Dates.GetPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType));

                    CustomerSession.DetailPeriod = ConstantePeriod.DisplayLevel.dayly;
                    if (CustomerSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule && CustomerSession.CurrentModule
                        == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
                        period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly, CustomerSession.ComparativePeriodType);
                    else
                        period = new MediaSchedulePeriod(begin, end, ConstantePeriod.DisplayLevel.dayly);

                }
                else
                {
                    begin = Dates.GetPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType);
                    end = Dates.GetPeriodEndDate(CustomerSession.PeriodEndDate, CustomerSession.PeriodType);
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

                
                
                CustomerSession.CurrentModule = module.Id;
                if (CustomerSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA) CustomerSession.CurrentTab = 0;
                CustomerSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
                param[0] = CustomerSession;
                param[1] = period;
                mediaScheduleResult = (IMediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, module.CountryRulesLayer.AssemblyName), module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                    | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                mediaScheduleResult.Module = module;

                CustomerSession.CurrentModule = moduleId;
                CustomerSession.DetailPeriod = periodDisplay;
                CustomerSession.CurrentTab = oldCurrentTab;
                CustomerSession.Unit = oldUnit;
                CustomerSession.ReferenceUniversMedia = oldReferenceUniversMedia;
                
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, CustomerSession.UserAgent, CustomerSession.CustomerLogin.Login, CustomerSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(CustomerSession.CurrentModule), CustomerSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);

                throw;
            }
            return mediaScheduleResult;
        }

        #region Identifiant des éléments de la nomenclature produit

        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        public void SetProductLevel(string idWebSession, int id, int level)
        {
            WebSession customerSession = (WebSession)WebSession.Load(idWebSession);
            var currentLevel = (DetailLevelItemInformation.Levels)customerSession.GenericProductDetailLevel.GetDetailLevelItemInformation(level);
            SetSessionProductDetailLevel(customerSession, id, currentLevel);
        }

        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        public void SetProductLevel(string idWebSession, int id, DetailLevelItemInformation.Levels level)
        {
            WebSession customerSession = (WebSession)WebSession.Load(idWebSession);
            SetSessionProductDetailLevel(customerSession, id, level);
        }

        private void SetSessionProductDetailLevel(WebSession webSession, int id, DetailLevelItemInformation.Levels level)
        {
            var tree = new System.Windows.Forms.TreeNode();
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            var param = new object[2];
            param[0] = webSession.CustomerDataFilters.DataSource;
            param[1] = webSession.DataLanguage;
            var factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            ClassificationLevelListDAL levels = null;

            switch (level)
            {
                case DetailLevelItemInformation.Levels.sector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.sector, tree);
                    break;
                case DetailLevelItemInformation.Levels.subSector:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subsector, tree);
                    break;
                case DetailLevelItemInformation.Levels.@group:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.groupAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.@group, tree);
                    break;
                case DetailLevelItemInformation.Levels.segment:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new TNS.AdExpress.Web.Core.Sessions.ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.segment, tree);
                    break;
                case DetailLevelItemInformation.Levels.product:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.productAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.product, tree);
                    break;
                case DetailLevelItemInformation.Levels.advertiser:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.advertiser, tree);
                    break;
                case DetailLevelItemInformation.Levels.brand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.brandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.brand, tree);
                    break;
                case DetailLevelItemInformation.Levels.holdingCompany:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.holding_company, tree);
                    break;
                case DetailLevelItemInformation.Levels.subBrand:
                    levels = factoryLevels.CreateClassificationLevelListDAL(TNS.AdExpress.Constantes.Customer.Right.type.subBrandAccess, id.ToString());
                    tree.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.subBrandAccess, id, levels[id]);
                    tree.Checked = true;
                    webSession.ProductDetailLevel = new ProductLevelSelection(TNS.AdExpress.Constantes.Classification.Level.type.subBrand, tree);
                    break;
            }

            webSession.Save();
        }


        #endregion

    }
}

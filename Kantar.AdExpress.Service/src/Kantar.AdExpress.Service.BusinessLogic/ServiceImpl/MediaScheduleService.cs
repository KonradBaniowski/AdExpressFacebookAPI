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
using TNS.AdExpress.Web.Core.Exceptions;
using System.Web;
using TNS.AdExpress.Web.Utilities.Exceptions;
using TNS.AdExpress;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpressI.Classification.DAL.MediaBrand;
using TNS.Classification.Universe;
using TNS.AdExpress.Classification;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaScheduleService : IMediaScheduleService
    {
        private WebSession CustomerSession = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public object[,] GetMediaScheduleData(string idWebSession, HttpContextBase httpContext)
        {
            object[,] result = null;
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            try
            {
                IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, "", "", httpContext);
                result = mediaScheduleResult.ComputeData();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return result;
        }

        public object[,] GetMediaScheduleData(string idWebSession, string zoomDate, string idVehicle, HttpContextBase httpContext)
        {
            object[,] result = null;
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            try
            {
                IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate, idVehicle, httpContext);
                result = mediaScheduleResult.ComputeData();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return result;
        }

        public GridResultResponse GetMediaScheduleCreativeData(CreativeMediaScheduleRequest creativeMediaScheduleRequest)
        {
            GridResultResponse response = new GridResultResponse();

            try
            {
                MediaScheduleResults  mediaScheduleResult = InitMediaScheduleCreativeCall(creativeMediaScheduleRequest);
                response.Data = mediaScheduleResult.ComputeData();
                response.Success = true;
            }
            catch (Exception err)
            {
                response.Message = err.Message;
                response.Success = false;
                return response;
            }
            return response;
        }

        public GridResult GetGridResult(string idWebSession, string zoomDate, HttpContextBase httpContext)
        {
            GridResult girdResult = new GridResult();
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate, "", httpContext);

            return mediaScheduleResult.GetGridResult();
        }

        public GridResult GetGridResult(string idWebSession, string zoomDate, string idVehicle, HttpContextBase httpContext)
        {
            GridResult girdResult = new GridResult();
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            IMediaScheduleResults mediaScheduleResult = InitMediaScheduleCall(idWebSession, zoomDate, idVehicle, httpContext);

            return mediaScheduleResult.GetGridResult();
        }

        public GridResultResponse GetGridResult(CreativeMediaScheduleRequest creativeMediaScheduleRequest)
        {
            GridResultResponse response = new GridResultResponse();

            try
            {
                MediaScheduleResults mediaScheduleResult = InitMediaScheduleCreativeCall(creativeMediaScheduleRequest);
                response.GridResult = mediaScheduleResult.GetCreativeMSGridResult();
                response.Success = true;
            }
            catch (System.Exception err)
            {
                response.Message = err.Message;
                response.Success = false;
                return response;
            }

            return response;
        }



        public MSCreatives GetMSCreatives(string idWebSession, string zoomDate, HttpContextBase httpContext)
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
                        result = GetEvaliantCreatives(data, vehicle);
                        break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                        result = GetEvaliantMobileCreatives(data, vehicle);
                        break;
                    default:
                        result = GetCreatives(data, vehicle);
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            result.SiteLanguage = CustomerSession.SiteLanguage;
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

        private IMediaScheduleResults InitMediaScheduleCall(string idWebSession, string zoomDate, string idVehicle, HttpContextBase httpContext)
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
                CustomerSession.Units = new List<WebConstantes.CustomerSessions.Unit> {oldUnit};
                CustomerSession.ReferenceUniversMedia = oldReferenceUniversMedia;

            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return mediaScheduleResult;
        }

        private MediaScheduleResults InitMediaScheduleCreativeCall(CreativeMediaScheduleRequest creativeMediaScheduleRequest)
        {
            const string LOGIN = WebConstantes.CreativeMSAccount.LOGIN;
            const string PASSWORD = WebConstantes.CreativeMSAccount.PASSWORD;
            WebSession webSession = null;
            System.Windows.Forms.TreeNode tmpNode;
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            AdExpressUniverse adExpressUniverse = null;
            Dictionary<int, AdExpressUniverse> universeDictionary = null;
            List<long> products = null;
            string vehicleId = string.Empty;
            MediaScheduleResults mediaScheduleResult = null;

            List<long> mediaTypeIds = creativeMediaScheduleRequest.MediaTypeIds.Split(',').Select(Int64.Parse).ToList();
            if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.POLAND))
            {
                mediaTypeIds = SwicthToAdExpressVehicle(mediaTypeIds, creativeMediaScheduleRequest.CreativeIds);
            }

            string[] productListId = creativeMediaScheduleRequest.ProductIds.Split(',');

            Right loginRight = new Right(LOGIN, PASSWORD, creativeMediaScheduleRequest.SiteLanguage);

            if (loginRight.CanAccessToAdExpress())
            {
                List<long> pIds = creativeMediaScheduleRequest.ProductIds.Split(',').Select(Int64.Parse).ToList();

                // Regarde à partir de quel tables charger les droits clients
                // (template ou droits propres au login)
                loginRight.SetModuleRights();
                loginRight.SetFlagsRights();
                loginRight.SetRights();
                if (WebApplicationParameters.VehiclesFormatInformation.Use)
                    loginRight.SetBannersAssignement();

                //Creer une session
                webSession = new WebSession(loginRight);
                // On Met à jour la session avec les paramètres
                webSession.IdSession = creativeMediaScheduleRequest.IdWebSession;
                webSession.CurrentModule = WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA;
                webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;

                if (mediaTypeIds.Count == 1 && VehiclesInformation.Get(mediaTypeIds[0]).Id == Vehicles.names.adnettrack)
                {
                    webSession.SiteLanguage = creativeMediaScheduleRequest.SiteLanguage;
                    webSession.Units = new List<WebConstantes.CustomerSessions.Unit>
                    {
                        WebConstantes.CustomerSessions.Unit.occurence
                    };
                    SetAdNetTrackProductSelection(webSession, Convert.ToInt64(creativeMediaScheduleRequest.CreativeIds));
                    SetSessionProductDetailLevel(webSession, Convert.ToInt32(productListId[0]), DetailLevelItemInformation.Levels.product);
                }
                else webSession.Units = new List<WebConstantes.CustomerSessions.Unit> { UnitsInformation.DefaultCurrency};

                webSession.LastReachedResultUrl = string.Empty;

                #region Media
                try
                {
                    webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
                    var vehicleLabels = new VehicleLevelListDAL(creativeMediaScheduleRequest.SiteLanguage, loginRight.Source);

                    foreach (long currentVehicleId in mediaTypeIds)
                    {

                        tmpNode = new System.Windows.Forms.TreeNode(vehicleLabels[currentVehicleId])
                        {
                            Tag =
                                new LevelInformation(
                                    TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess,
                                    currentVehicleId, vehicleLabels[currentVehicleId])
                        };
                        webSession.CurrentUniversMedia.Nodes.Add(tmpNode);
                    }
                    webSession.SelectionUniversMedia = (System.Windows.Forms.TreeNode)webSession.CurrentUniversMedia.Clone();
                }
                catch (System.Exception err)
                {
                    throw new Exception("Impossible de construire l'univers media : " + err.Message);
                }
                #endregion

                #region Produit
                try
                {
                    nomenclatureElementsGroup = new NomenclatureElementsGroup(0, AccessType.includes);

                    products = productListId.Select(Int64.Parse).ToList();

                    nomenclatureElementsGroup.AddItems(TNSClassificationLevels.PRODUCT, products);
                    adExpressUniverse = new AdExpressUniverse(Dimension.product);
                    adExpressUniverse.AddGroup(0, nomenclatureElementsGroup);
                    universeDictionary = new Dictionary<int, AdExpressUniverse>();
                    universeDictionary.Add(0, adExpressUniverse);
                    webSession.PrincipalProductUniverses = universeDictionary;
                }
                catch (System.Exception err)
                {
                    throw new Exception("Impossible de construire l'univers produit : " + err.Message);
                }
                #endregion

                #region Dates
                webSession.DetailPeriod = WebConstantes.CustomerSessions.Period.DisplayLevel.dayly;
                webSession.PeriodType = WebConstantes.CustomerSessions.Period.Type.dateToDate;
                webSession.PeriodBeginningDate = creativeMediaScheduleRequest.BeginDate.ToString();
                webSession.PeriodEndDate = creativeMediaScheduleRequest.EndDate.ToString();
                #endregion

                #region Versions
                try
                {
                    if (!string.IsNullOrEmpty(creativeMediaScheduleRequest.CreativeIds))
                    {
                        string[] versionListId = creativeMediaScheduleRequest.CreativeIds.Split(',');
                        webSession.IdSlogans = new ArrayList();
                        if (mediaTypeIds.Count > 1)
                        {
                            throw new Exception("On ne peut pas avoir un plan media pluri media si on passe un numéro de version en paramètre");
                        }

                        foreach (string idVersion in versionListId.Where(idVersion => !idVersion.Equals("0")))
                        {
                            if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FRANCE))
                            {
                                vehicleId = idVersion.Substring(0, creativeMediaScheduleRequest.MediaTypeIds.Length);
                                if (!vehicleId.Equals(creativeMediaScheduleRequest.MediaTypeIds))
                                {
                                    throw new Exception("La version " + idVersion + " n'appartient pas au media : " + creativeMediaScheduleRequest.MediaTypeIds);
                                }
                                webSession.IdSlogans.Add(Int64.Parse(idVersion.Substring(creativeMediaScheduleRequest.MediaTypeIds.Length)));
                            }
                            else
                            {
                                webSession.IdSlogans.Add(Int64.Parse(idVersion));
                            }
                        }
                    }
                    else
                    {
                        webSession.DetailPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.weekly;
                    }
                }
                catch (System.Exception err)
                {
                    throw new Exception("Impossible de construire la liste des versions : " + err.Message);
                }
                #endregion

                #region Niveau de détail générique
                var levels = new ArrayList { 1, 2, 3 };
                webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                webSession.GenericAdNetTrackDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion

                webSession.SiteLanguage = creativeMediaScheduleRequest.SiteLanguage;
                //Sauvegarder la session
                webSession.Save();

                GridResult girdResult = new GridResult();
                object[,] tab = null;
                MediaSchedulePeriod period = null;
                MediaScheduleData resultTmp = null;
                string idMediaType = string.Empty;

                if (mediaTypeIds.Count == 1)
                    idMediaType = mediaTypeIds[0].ToString();
                period = new MediaSchedulePeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate, webSession.DetailPeriod);

                TNS.AdExpress.Domain.Web.Navigation.Module module = ModulesList.GetModule(webSession.CurrentModule);
                if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the Media Schedule result"));
                var param = (string.IsNullOrEmpty(idMediaType)) ? new object[2] : new object[3];
                param[0] = webSession;
                param[1] = period;
                if (!string.IsNullOrEmpty(idMediaType))
                {
                    param[2] = Convert.ToInt64(idMediaType);
                    //creativeSelectionWebControl.IdVehicle = idMediaType;
                }
                mediaScheduleResult = (MediaScheduleResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, module.CountryRulesLayer.AssemblyName), module.CountryRulesLayer.Class, false,
                    BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            }
            else
                throw new Exception(GestionWeb.GetWebWord(880, creativeMediaScheduleRequest.SiteLanguage));


            return mediaScheduleResult;
        }

        #region Identifiant des éléments de la nomenclature produit

        /// <summary>
        /// Set Product classification filter
        /// </summary>
        /// <param name="webSession">session</param>
        /// <param name="id">Element ID</param>
        /// <param name="level">Element Classification level</param>
        public void SetProductLevel(string idWebSession, Int64 id, int level)
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

        private void SetSessionProductDetailLevel(WebSession webSession, Int64 id, DetailLevelItemInformation.Levels level)
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


        private List<long> SwicthToAdExpressVehicle(List<long> mediaTypeIds, string creativeIds)
        {
            var newVehicle = new List<long>();

            mediaTypeIds.ForEach(p =>
            {
                switch (p)
                {
                    case 1:
                        newVehicle.Add(3);
                        break;
                    case 3:
                        newVehicle.Add(1);
                        break;
                    case 8:
                        newVehicle.Add(5);
                        break;
                    case 9: newVehicle.Add(6); break;
                    case 20: newVehicle.Add(9); break;
                    case 7:
                        newVehicle.Add(!string.IsNullOrEmpty(creativeIds) ? 8 : p);
                        break;
                    case 6:
                        newVehicle.Add(8); break;
                    default: newVehicle.Add(p); break;

                }
            });
            return newVehicle;
        }

        private void SetAdNetTrackProductSelection(WebSession webSession, long id)
        {

            webSession.AdNetTrackSelection =
                new AdNetTrackProductSelection(TNS.AdExpress.Constantes.FrameWork.Results.AdNetTrackMediaSchedule.Type.visual, id);

        }
        #endregion

    }
}

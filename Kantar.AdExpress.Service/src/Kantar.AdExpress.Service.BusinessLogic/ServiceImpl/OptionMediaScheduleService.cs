using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using WebCore = TNS.AdExpress.Web.Core;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using System.Reflection;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using TNS.AdExpress.Domain.Web;
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;
using ConstantesSession = TNS.AdExpress.Constantes.Web.CustomerSessions;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class OptionMediaScheduleService : IOptionMediaScheduleService
    {
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        private WebSession _customerWebSession = null;
        private WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
        private WebNavigation.Module _currentModule;
        private int _nbDetailLevelItemList = 4;
        private Hashtable _genericDetailLevelsSaved = new Hashtable();
        private GenericDetailLevel _customerGenericDetailLevel = null;
        protected WebConstantes.GenericDetailLevel.Type _genericColumnDetailLevelType;
        protected int _nbColumnDetailLevelItemList = 1;

        public OptionsMediaSchedule GetOptions(string idWebSession, bool isAdNetTrack, HttpContextBase httpContext)
        {

            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            OptionsMediaSchedule options = new OptionsMediaSchedule();
            try
            {
                _currentModule = WebNavigation.ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);
                options.SiteLanguage = _customerWebSession.SiteLanguage;

                #region GenericDetailLevelOption
                GenericDetailLevelOption genericDetailLevelOption = new GenericDetailLevelOption();

                #region on vérifie que le niveau sélectionné à le droit d'être utilisé
                bool canAddDetail = false;
                switch (_componentProfile)
                {
                    case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                        try
                        {
                            if (isAdNetTrack)
                                canAddDetail = CanAddDetailLevel(_customerWebSession.GenericAdNetTrackDetailLevel, _customerWebSession.CurrentModule, true);
                            else
                                canAddDetail = CanAddDetailLevel(_customerWebSession.GenericMediaDetailLevel, _customerWebSession.CurrentModule, false);
                        }
                        catch { }
                        if (!canAddDetail)
                        {
                            // Niveau de détail par défaut
                            ArrayList levelsIds = new ArrayList();
                            switch (_customerWebSession.CurrentModule)
                            {
                                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                                    levelsIds.Add((int)DetailLevelItemInformation.Levels.media);
                                    break;
                                case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                                    levelsIds.Add((int)DetailLevelItemInformation.Levels.advertiser);
                                    levelsIds.Add((int)DetailLevelItemInformation.Levels.sector);
                                    break;
                                default:
                                    levelsIds.Add((int)DetailLevelItemInformation.Levels.vehicle);
                                    levelsIds.Add((int)DetailLevelItemInformation.Levels.category);
                                    break;
                            }
                            if (isAdNetTrack)
                                _customerWebSession.GenericAdNetTrackDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
                            else
                                _customerWebSession.GenericMediaDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
                        }
                        break;
                    case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                        try
                        {
                            canAddDetail = CanAddDetailLevel(_customerWebSession.GenericProductDetailLevel, _customerWebSession.CurrentModule, isAdNetTrack);
                        }
                        catch { }
                        if (!canAddDetail)
                        {
                            // Niveau de détail par défaut
                            ArrayList levelsIds = new ArrayList();
                            levelsIds.Add((int)DetailLevelItemInformation.Levels.advertiser);
                            _customerWebSession.GenericProductDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
                        }
                        break;
                }
                #endregion

                #region Niveau de détaille par défaut
                genericDetailLevelOption.DefaultDetail = new SelectControl();
                genericDetailLevelOption.DefaultDetail.Id = "defaultDetail";
                genericDetailLevelOption.DefaultDetail.Items = new List<SelectItem>();
                genericDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = "-------", Value = "-1" });
                int DefaultDetailLevelId = 0;
                bool containsSlogan = false;
                ArrayList DefaultDetailLevels = GetDefaultDetailLevels();
                foreach (GenericDetailLevel currentLevel in DefaultDetailLevels)
                {
                    if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule, isAdNetTrack))
                    {
                        containsSlogan = false;
                        foreach (DetailLevelItemInformation.Levels level in currentLevel.LevelIds)
                        {
                            if (level == DetailLevelItemInformation.Levels.slogan)
                                containsSlogan = true;
                        }
                        if (containsSlogan)
                            genericDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = currentLevel.GetLabel(_customerWebSession.SiteLanguage), Value = DefaultDetailLevelId.ToString(), slogan = true });
                        else
                            genericDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = currentLevel.GetLabel(_customerWebSession.SiteLanguage), Value = DefaultDetailLevelId.ToString(), slogan = false });
                    }
                    DefaultDetailLevelId++;
                }
                if (WebApplicationParameters.CountryCode == WebConstantes.CountryCode.TURKEY &&
                    (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE
                     || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                     || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE))
                    genericDetailLevelOption.DefaultDetail.SelectedId = "3";
                else
                    genericDetailLevelOption.DefaultDetail.SelectedId = "0";
                #endregion

                #region Niveau de détaille par personnalisé
                // Obtient les niveaux de détail sauvegardés		
                ArrayList genericDetailLevelsSaved = GetGenericDetailLevelsSaved();
                genericDetailLevelOption.CustomDetail = new SelectControl();
                genericDetailLevelOption.CustomDetail.Id = "customDetail";
                genericDetailLevelOption.CustomDetail.Items = new List<SelectItem>();
                genericDetailLevelOption.CustomDetail.Items.Add(new SelectItem { Text = "-------", Value = "-1" });
                foreach (GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved)
                {
                    if (CanAddDetailLevel(currentGenericLevel, _customerWebSession.CurrentModule, isAdNetTrack) && currentGenericLevel.GetNbLevels <= _nbDetailLevelItemList)
                    {
                        containsSlogan = false;
                        foreach (DetailLevelItemInformation.Levels level in currentGenericLevel.LevelIds)
                        {
                            if (level == DetailLevelItemInformation.Levels.slogan)
                                containsSlogan = true;
                        }
                        if (containsSlogan)
                            genericDetailLevelOption.CustomDetail.Items.Add(new SelectItem { Text = currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage), Value = currentGenericLevel.Id.ToString(), slogan = true });
                        else
                            genericDetailLevelOption.CustomDetail.Items.Add(new SelectItem { Text = currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage), Value = currentGenericLevel.Id.ToString(), slogan = false });
                        _genericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
                    }
                }
                #endregion

                #region Niveau de détaille par défaut

                #region L1
                if (_nbDetailLevelItemList >= 1)
                {
                    genericDetailLevelOption.L1Detail = DetailLevelItemInit(1, isAdNetTrack);
                }
                #endregion

                #region L2
                if (_nbDetailLevelItemList >= 2)
                {
                    genericDetailLevelOption.L2Detail = DetailLevelItemInit(2, isAdNetTrack);
                }
                #endregion

                #region L3
                if (_nbDetailLevelItemList >= 3)
                {
                    genericDetailLevelOption.L3Detail = DetailLevelItemInit(3, isAdNetTrack);
                }
                #endregion

                #region L4
                if (_nbDetailLevelItemList >= 4)
                {
                    genericDetailLevelOption.L4Detail = DetailLevelItemInit(4, isAdNetTrack);
                }
                #endregion

                #endregion

                options.GenericDetailLevel = genericDetailLevelOption;

                #region GRP Turkey
                //CheckBoxOption grp = new CheckBoxOption();
                //grp.Id = "grp";
                //grp.Value = _customerWebSession.Grp;
                //options.Grp = grp;

                //CheckBoxOption grp30S = new CheckBoxOption();
                //grp30S.Id = "grp30S";
                //grp30S.Value = _customerWebSession.Grp30S;
                //options.Grp30S = grp30S;

                //CheckBoxOption spendsGrp = new CheckBoxOption();
                //spendsGrp.Id = "spendsGrp";
                //spendsGrp.Value = _customerWebSession.SpendsGrp;
                //options.SpendsGrp = spendsGrp;
                //if (_customerWebSession.Unit == ConstantesSession.Unit.euro
                //    || _customerWebSession.Unit == ConstantesSession.Unit.tl
                //    || _customerWebSession.Unit == ConstantesSession.Unit.usd)
                //    options.SpendsSelected = true;
                #endregion

                #endregion

                #region PeriodDetailOption
                PeriodDetailOption PeriodDetail = new PeriodDetailOption();

                PeriodDetail.PeriodDetailType = new SelectControl();
                PeriodDetail.PeriodDetailType.Id = "periodDetailType";
                PeriodDetail.PeriodDetailType.Items = new List<SelectItem>();
                PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2290, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.monthly.GetHashCode().ToString() });
                PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(848, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.weekly.GetHashCode().ToString() });
                DateTime begin = WebCore.Utilities.Dates.GetPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
                if (begin >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2289, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.dayly.GetHashCode().ToString() });
                }

                if (_customerWebSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
                {
                    if (WebCore.Utilities.Dates.GetPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                        < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _customerWebSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                    }
                }

                PeriodDetail.PeriodDetailType.SelectedId = _customerWebSession.DetailPeriod.GetHashCode().ToString();

                if (isAdNetTrack)
                    PeriodDetail.PeriodDetailType.Visible = false;
                else
                    PeriodDetail.PeriodDetailType.Visible = true;

                options.PeriodDetail = PeriodDetail;
                #endregion

                _customerWebSession.ReachedModule = true;
                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return options;
        }

        public void SetOptions(string idWebSession, UserFilter userFilter, bool isAdNetTrack, HttpContextBase httpContext)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                _currentModule = WebNavigation.ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA);

                #region GenericDetailLevelFilter
                ArrayList levels = new ArrayList();

                ArrayList genericDetailLevelsSaved = GetGenericDetailLevelsSaved();
                foreach (GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved)
                {
                    if (CanAddDetailLevel(currentGenericLevel, _customerWebSession.CurrentModule, isAdNetTrack) && currentGenericLevel.GetNbLevels <= _nbDetailLevelItemList)
                    {
                        _genericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
                    }
                }

                if (userFilter.GenericDetailLevelFilter.DefaultDetailValue >= 0)
                {
                    _customerGenericDetailLevel = (GenericDetailLevel)GetDefaultDetailLevels()[userFilter.GenericDetailLevelFilter.DefaultDetailValue];
                }
                if (userFilter.GenericDetailLevelFilter.CustomDetailValue >= 0)
                {
                    _customerGenericDetailLevel = (GenericDetailLevel)_genericDetailLevelsSaved[(Int64)userFilter.GenericDetailLevelFilter.CustomDetailValue];
                }
                if (_nbDetailLevelItemList >= 1 && userFilter.GenericDetailLevelFilter.L1DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L1DetailValue);
                }
                if (_nbDetailLevelItemList >= 2 && userFilter.GenericDetailLevelFilter.L2DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L2DetailValue);
                }
                if (_nbDetailLevelItemList >= 3 && userFilter.GenericDetailLevelFilter.L3DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L3DetailValue);
                }
                if (_nbDetailLevelItemList >= 4 && userFilter.GenericDetailLevelFilter.L4DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L4DetailValue);
                }
                if (levels.Count > 0)
                {
                    _customerGenericDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
                }
                #endregion

                switch (_currentModule.Id)
                {
                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                        if (isAdNetTrack)
                            _customerWebSession.GenericAdNetTrackDetailLevel = _customerGenericDetailLevel;
                        else
                            _customerWebSession.GenericMediaDetailLevel = _customerGenericDetailLevel;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                        _customerWebSession.GenericProductDetailLevel = _customerGenericDetailLevel;
                        break;
                }

                #region PeriodDetailFilter
                _customerWebSession.DetailPeriod = (ConstantesPeriod.DisplayLevel)userFilter.PeriodDetailFilter.PeriodDetailType;
                #endregion

                //_customerWebSession.Grp = userFilter.Grp;
                //_customerWebSession.Grp30S = userFilter.Grp30S;
                //_customerWebSession.SpendsGrp = userFilter.SpendsGrp;

                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
        }

        #region Generic Detail Level Option Methodes
        private bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel, Int64 module, bool isAdnettrack)
        {
            ArrayList allowedDetailLevelItems;

            if(isAdnettrack)
                allowedDetailLevelItems = WebCore.AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems;
            else
                allowedDetailLevelItems = GetAllowedDetailLevelItems();

            TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
            if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            object[] param = new object[2];
            param[0] = _customerWebSession;
            param[1] = _componentProfile;
            WebCore.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (WebCore.Utilities.MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            foreach (DetailLevelItemInformation currentDetailLevelItem in currentDetailLevel.Levels)
            {
                if (!allowedDetailLevelItems.Contains(currentDetailLevelItem)) return (false);
                if (isAdnettrack)
                {
                    if (!mediaDetailLevelUtilities.CanAddAdnettrackDetailLevelItem(currentDetailLevelItem))
                        return (false);
                }
                else
                {
                    if (!mediaDetailLevelUtilities.CanAddDetailLevelItem(currentDetailLevelItem, module))
                        return (false);
                }
            }
            mediaDetailLevelUtilities = null;
            clMediaU = null;
            return (true);
        }

        private ArrayList GetAllowedDetailLevelItems()
        {

            List<DetailLevelItemInformation.Levels> vehicleAllowedDetailLevelList;
            ArrayList allowedDetailLevelList;
            ArrayList list = new ArrayList();

            switch (_componentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
                    return GetModuleAllowedDetailLevelItems();
                default:

                    vehicleAllowedDetailLevelList = GetVehicleAllowedDetailLevelItems();
                    allowedDetailLevelList = GetModuleAllowedDetailLevelItems();

                    foreach (DetailLevelItemInformation currentLevel in allowedDetailLevelList)
                        if (vehicleAllowedDetailLevelList.Contains(currentLevel.Id))
                            list.Add(currentLevel);

                    return list;
            }
        }

        private ArrayList GetModuleAllowedDetailLevelItems()
        {
            switch (_componentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    return (_currentModule.AllowedMediaDetailLevelItems);
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    return (_currentModule.AllowedProductDetailLevelItems);
                case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
                    return (WebCore.AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems);
                default:
                    return (new ArrayList());
            }
        }

        private List<DetailLevelItemInformation.Levels> GetVehicleAllowedDetailLevelItems()
        {
            List<Int64> vehicleList = GetVehicles();
            List<DetailLevelItemInformation.Levels> levelList = VehiclesInformation.GetCommunDetailLevelList(vehicleList);

            return levelList;
        }

        private List<Int64> GetVehicles()
        {

            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (!string.IsNullOrEmpty(listStr))
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return vehicleList;
        }

        private ArrayList GetDefaultDetailLevels()
        {
            switch (_componentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    return (_currentModule.DefaultMediaDetailLevels);
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    return (_currentModule.DefaultProductDetailLevels);
                case WebConstantes.GenericDetailLevel.ComponentProfile.adnettrack:
                    return (WebCore.AdNetTrackDetailLevelsDescription.DefaultAdNetTrackDetailLevels);
                default:
                    return (new ArrayList());
            }
        }

        private ArrayList GetGenericDetailLevelsSaved()
        {
            DataSet ds;
            int currentIndex = 0;
            ArrayList genericDetailLevelsSaved = new ArrayList();
            Int64 currentId = -1;
            int currentLevelId;
            try
            {
                ds = GenericDetailLevelDataAccess.Load(_customerWebSession);
                if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    foreach (DataRow currentRow in ds.Tables[0].Rows)
                    {
                        if (currentId != Int64.Parse(currentRow["id_list"].ToString()))
                        {
                            currentId = Int64.Parse(currentRow["id_list"].ToString());
                            currentIndex = genericDetailLevelsSaved.Add(new GenericDetailLevelSaved(currentId, new ArrayList()));
                        }
                        currentLevelId = int.Parse(currentRow["id_type_level"].ToString());
                        ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).AddLevel(currentLevelId);
                    }
                }
            }
            catch (System.Exception err)
            {
                string t = err.Message;
            }
            return (genericDetailLevelsSaved);
        }

        private SelectControl DetailLevelItemInit(int level, bool isAdnettrack)
        {
            SelectControl selectControl = new SelectControl();
            selectControl.Id = "l" + level.ToString() + "Detail";
            selectControl.Items = new List<SelectItem>();
            selectControl.Items.Add(new SelectItem { Text = "-------", Value = "-1" });
            ArrayList AllowedDetailLevelItems;
            if (isAdnettrack)
                AllowedDetailLevelItems = WebCore.AdNetTrackDetailLevelsDescription.AllowedAdNetTrackLevelItems;
            else
                AllowedDetailLevelItems = GetAllowedDetailLevelItems();
            bool canAddLevel = false;

            TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
            if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            object[] param = new object[2];
            param[0] = _customerWebSession;
            param[1] = _componentProfile;
            WebCore.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (WebCore.Utilities.MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            foreach (DetailLevelItemInformation currentDetailLevelItem in AllowedDetailLevelItems)
            {
                if(isAdnettrack)
                    canAddLevel = mediaDetailLevelUtilities.CanAddAdnettrackDetailLevelItem(currentDetailLevelItem);
                else
                    canAddLevel = mediaDetailLevelUtilities.CanAddDetailLevelItem(currentDetailLevelItem,_customerWebSession.CurrentModule);

                if (canAddLevel)
                {
                    if (currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan)
                        selectControl.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, _customerWebSession.SiteLanguage), Value = currentDetailLevelItem.Id.GetHashCode().ToString(), slogan = true });
                    else
                        selectControl.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, _customerWebSession.SiteLanguage), Value = currentDetailLevelItem.Id.GetHashCode().ToString(), slogan = false });
                }
            }
            mediaDetailLevelUtilities = null;
            clMediaU = null;

            return selectControl;
        }
        #endregion
    }
}

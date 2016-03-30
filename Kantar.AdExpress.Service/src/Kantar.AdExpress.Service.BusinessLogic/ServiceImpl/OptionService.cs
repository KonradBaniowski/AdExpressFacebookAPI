﻿#define Debug
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using TNS.AdExpress.Domain.Level;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebCore = TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Classification;
using System.Data;
using TNS.AdExpress.Web.Core.DataAccess.Session;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using ConstantesPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using ConstantesSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Constantes.Customer;
using ConstantesDB = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Constantes.Classification.DB;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using FrameWorkResults = TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public partial class OptionService : IOptionService
    {
        private WebSession _customerWebSession = null;
        private WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
        private WebNavigation.Module _currentModule;
        private int _nbDetailLevelItemList = 4;
        private Hashtable _genericDetailLevelsSaved = new Hashtable();
        private GenericDetailLevel _customerGenericDetailLevel = null;      
        protected WebConstantes.GenericDetailLevel.Type _genericColumnDetailLevelType;
        protected int _nbColumnDetailLevelItemList = 1;

        public Options GetOptions(string idWebSession)
        {

            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            _currentModule = WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule);

            Options options = new Options();

            options.SiteLanguage = _customerWebSession.SiteLanguage;

            switch (_customerWebSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
                    _nbDetailLevelItemList = 4;
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;
                    _nbDetailLevelItemList = 3;
                    break;
            }

            #region GenericDetailLevelOption
            GenericDetailLevelOption genericDetailLevelOption = new GenericDetailLevelOption();

            #region on vérifie que le niveau sélectionné à le droit d'être utilisé
            bool canAddDetail = false;
            switch (_componentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    try
                    {
                        canAddDetail = CanAddDetailLevel(_customerWebSession.GenericMediaDetailLevel, _customerWebSession.CurrentModule);
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
                        _customerWebSession.GenericMediaDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
                    }
                    break;
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    try
                    {
                        canAddDetail = CanAddDetailLevel(_customerWebSession.GenericProductDetailLevel, _customerWebSession.CurrentModule);
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
            ArrayList DefaultDetailLevels = GetDefaultDetailLevels();
            foreach (GenericDetailLevel currentLevel in DefaultDetailLevels)
            {
                if (CanAddDetailLevel(currentLevel, _customerWebSession.CurrentModule))
                    genericDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = currentLevel.GetLabel(_customerWebSession.SiteLanguage), Value = DefaultDetailLevelId.ToString() });
                DefaultDetailLevelId++;
            }
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
                if (CanAddDetailLevel(currentGenericLevel, _customerWebSession.CurrentModule) && currentGenericLevel.GetNbLevels <= _nbDetailLevelItemList)
                {
                    genericDetailLevelOption.CustomDetail.Items.Add(new SelectItem { Text = currentGenericLevel.GetLabel(_customerWebSession.SiteLanguage), Value = currentGenericLevel.Id.ToString() });
                    _genericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
                }
            }
            #endregion

            #region Niveau de détaille par défaut

            #region L1
            if (_nbDetailLevelItemList >= 1)
            {
                genericDetailLevelOption.L1Detail = DetailLevelItemInit(1);
            }
            #endregion

            #region L2
            if (_nbDetailLevelItemList >= 2)
            {
                genericDetailLevelOption.L2Detail = DetailLevelItemInit( 2);
            }
            #endregion

            #region L3
            if (_nbDetailLevelItemList >= 3)
            {
                genericDetailLevelOption.L3Detail = DetailLevelItemInit(3);
            }
            #endregion

            #region L4
            if (_nbDetailLevelItemList >= 4)
            {
                genericDetailLevelOption.L4Detail = DetailLevelItemInit(4);
            }
            #endregion
            
            #endregion

            options.GenericDetailLevel = genericDetailLevelOption;

          
            #endregion

            #region PeriodDetailOption
            PeriodDetailOption PeriodDetail = new PeriodDetailOption();

            PeriodDetail.PeriodDetailType = new SelectControl();
            PeriodDetail.PeriodDetailType.Id = "periodDetailType";
            PeriodDetail.PeriodDetailType.Items = new List<SelectItem>();
            PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2290, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.monthly.GetHashCode().ToString() });
            PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(848, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.weekly.GetHashCode().ToString() });
            DateTime begin = WebCore.Utilities.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
            if (begin >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            {
                PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2289, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.dayly.GetHashCode().ToString() });
            }

            if (_customerWebSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
            {
                if (WebCore.Utilities.Dates.getPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                    < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    _customerWebSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                }
            }

            PeriodDetail.PeriodDetailType.SelectedId = _customerWebSession.DetailPeriod.GetHashCode().ToString();

            options.PeriodDetail = PeriodDetail;
            #endregion

            #region resultTypeOption
            if(WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA != _customerWebSession.CurrentModule)
            {
                ResultTypeOption resultTypeOption = new ResultTypeOption();

                resultTypeOption.ResultType = new SelectControl();
                resultTypeOption.ResultType.Id = "resultType";
                resultTypeOption.ResultType.Items = new List<SelectItem>();

                List<long> resultToShow = new List<long>();
                var selectedMediaUniverse = GetSelectedUniverseMedia(_customerWebSession);
                List<WebNavigation.ResultPageInformation> resultPages = _currentModule.GetValidResultsPage(selectedMediaUniverse);

                foreach (WebNavigation.ResultPageInformation current in resultPages)
                {
                    if (!CanShowResult(_customerWebSession, current)) continue;
                    resultToShow.Add(current.Id);
                    resultTypeOption.ResultType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(current.IdWebText,
                        _customerWebSession.SiteLanguage), Value = current.Id.ToString() });
                }
                SetDefaultTab(resultToShow, _customerWebSession, resultTypeOption, options);
            }
           
            #endregion

            #region UnitOption
            UnitOption unitOption = new UnitOption();

            unitOption.Unit = new SelectControl();
            unitOption.Unit.Id = "unit";
            unitOption.Unit.Items = new List<SelectItem>();

            List<UnitInformation> units = _customerWebSession.GetValidUnitForResult();

            foreach (UnitInformation currentUnit in units)
            {
                if (currentUnit.Id != ConstantesSession.Unit.volume || _customerWebSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_MARKETING_DIRECT))
                {
                    if (currentUnit.Id != ConstantesSession.Unit.volumeMms || _customerWebSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_DISPLAY))
                        unitOption.Unit.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentUnit.WebTextId, _customerWebSession.SiteLanguage), Value = currentUnit.Id.GetHashCode().ToString() });
                    else if (_customerWebSession.Unit == ConstantesSession.Unit.volumeMms)
                        _customerWebSession.Unit = UnitsInformation.DefaultCurrency;
                }
                else if (_customerWebSession.Unit == ConstantesSession.Unit.volume)
                    _customerWebSession.Unit = UnitsInformation.DefaultCurrency;
            }

            if (!units.Contains(UnitsInformation.Get(_customerWebSession.Unit)))
            {
                if (ContainsDefaultCurrency(units))
                    _customerWebSession.Unit = UnitsInformation.DefaultCurrency;
                else
                    _customerWebSession.Unit = units[0].Id;
            }

            unitOption.Unit.SelectedId = _customerWebSession.Unit.GetHashCode().ToString();

            options.UnitOption = unitOption;
            #endregion

            #region Options by Vehicle
            string vehicleListId = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, Right.type.vehicleAccess);
            string[] vehicles = vehicleListId.Split(',');
            bool autopromoEvaliantOption = false;
            bool insertOption = false;
            foreach (string cVehicle in vehicles)
            {
                switch (VehiclesInformation.DatabaseIdToEnum(Int64.Parse(cVehicle)))
                {
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.mms:
                        autopromoEvaliantOption = VehiclesInformation.Get(Int64.Parse(cVehicle)).Autopromo;
                        break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                        insertOption = true;
                        break;
                }
            }
            #endregion

            #region InsertionOption
            InsertionOption insertionOption = new InsertionOption();

            insertionOption.Insertion = new SelectControl();
            insertionOption.Insertion.Id = "insertion";
            insertionOption.Insertion.Visible = insertOption;
            insertionOption.Insertion.Items = new List<SelectItem>();

            List<ConstantesSession.Insert> inserts = WebCore.Utilities.Units.getInserts();
            for (int j = 0; j < inserts.Count; j++)
            {
                insertionOption.Insertion.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.InsertsTraductionCodes[(ConstantesSession.Insert)inserts[j]], _customerWebSession.SiteLanguage), Value = ((int)(ConstantesSession.Insert)inserts[j]).ToString() });
            }

            insertionOption.Insertion.SelectedId = _customerWebSession.Insert.GetHashCode().ToString();

            options.InsertionOption = insertionOption;
            #endregion

            #region AutoPromoOption
            AutoPromoOption autoPromoOption = new AutoPromoOption();

            autoPromoOption.AutoPromo = new SelectControl();
            autoPromoOption.AutoPromo.Id = "autoPromo";
            autoPromoOption.AutoPromo.Visible = autopromoEvaliantOption;
            autoPromoOption.AutoPromo.Items = new List<SelectItem>();

            ArrayList autoPromoItems = new ArrayList();
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.total], _customerWebSession.SiteLanguage),  Value = "0" });
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoAdvertiser], _customerWebSession.SiteLanguage), Value = "1" });
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoHoldingCompany], _customerWebSession.SiteLanguage), Value = "2" });

            autoPromoOption.AutoPromo.SelectedId = _customerWebSession.AutoPromo.GetHashCode().ToString();

            options.AutoPromoOption = autoPromoOption;
            #endregion

            #region FormatOption
            FormatOption formatOption = new FormatOption();

            formatOption.Format = new SelectControl();
            formatOption.Format.Id = "format";
            formatOption.Format.Visible = autopromoEvaliantOption;
            formatOption.Format.Items = new List<SelectItem>();

            var activeBannersFormatList = new List<FilterItem>(_customerWebSession.GetValidFormatList(_customerWebSession.GetVehiclesSelected()).Values);
            if (activeBannersFormatList.Count > 0)
            {
                foreach (FilterItem item in activeBannersFormatList)
                {
                    formatOption.Format.Items.Add(new SelectItem { Text = item.Label, Value = item.Id.ToString() });
                }

                if (string.IsNullOrEmpty(_customerWebSession.SelectedBannersFormatList))
                    formatOption.Format.SelectedId = string.Join(",", activeBannersFormatList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
                else formatOption.Format.SelectedId = _customerWebSession.SelectedBannersFormatList;
            }
            else
            {
                formatOption.Format.Visible = false;
            }

            options.FormatOption = formatOption;
            #endregion

            #region PurchaseModeOption
            PurchaseModeOption purchaseModeOption = new PurchaseModeOption();

            purchaseModeOption.PurchaseMode = new SelectControl();
            purchaseModeOption.PurchaseMode.Id = "purchaseMode";
            purchaseModeOption.PurchaseMode.Visible = autopromoEvaliantOption;
            purchaseModeOption.PurchaseMode.Items = new List<SelectItem>();

            Dictionary<Int64, VehicleInformation> VehicleInformationList = _customerWebSession.GetVehiclesSelected();
            if (VehicleInformationList.ContainsKey(VehiclesInformation.Get(Vehicles.names.mms).DatabaseId))
            {
                var purchaseModeList = new List<FilterItem>(PurchaseModeList.GetList().Values);
                if (purchaseModeList.Count > 0)
                {
                    foreach (FilterItem item in purchaseModeList)
                    {
                        purchaseModeOption.PurchaseMode.Items.Add(new SelectItem { Text = item.Label, Value = item.Id.ToString() });
                    }

                    if (string.IsNullOrEmpty(_customerWebSession.SelectedPurchaseModeList))
                        purchaseModeOption.PurchaseMode.SelectedId = string.Join(",", purchaseModeList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
                    else purchaseModeOption.PurchaseMode.SelectedId = _customerWebSession.SelectedPurchaseModeList;
                }
                else
                {
                    purchaseModeOption.PurchaseMode.Visible = false;
                }
            }
            else
            {
                purchaseModeOption.PurchaseMode.Visible = false;
            }

            options.PurchaseModeOption = purchaseModeOption;
            #endregion

            switch (_customerWebSession.CurrentModule)
            {              
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    options.GenericColumnDetailLevelOption = GetGenericColumnLevelDetailOptions();
                    break;
            }

            return options;
        }

        public void SetOptions(string idWebSession, UserFilter userFilter)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            _currentModule = WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule);
          
            switch (_customerWebSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
                    _nbDetailLevelItemList = 4;
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    _nbDetailLevelItemList = 3;
                    _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;                  
                    break;
            }

            if(_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE
                || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
            SetGenericColumnLevelDetailOptions(userFilter);

            #region GenericDetailLevelFilter
            ArrayList levels = new ArrayList();

            ArrayList genericDetailLevelsSaved = GetGenericDetailLevelsSaved();
            foreach (GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved)
            {
                if (CanAddDetailLevel(currentGenericLevel, _customerWebSession.CurrentModule) && currentGenericLevel.GetNbLevels <= _nbDetailLevelItemList)
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

            switch (_customerWebSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    _customerWebSession.GenericMediaDetailLevel = _customerGenericDetailLevel;
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    _customerWebSession.GenericProductDetailLevel = _customerGenericDetailLevel;
                    #region  resultTypeFilter
                    _customerWebSession.CurrentTab = userFilter.ResultTypeFilter.ResultType;
                    #endregion
                    break;
            }


            #region PeriodDetailFilter
            _customerWebSession.DetailPeriod = (ConstantesPeriod.DisplayLevel)userFilter.PeriodDetailFilter.PeriodDetailType;
            #endregion

            #region UnitFilter
            _customerWebSession.Unit = (ConstantesSession.Unit)userFilter.UnitFilter.Unit;
            #endregion

            #region Options by Vehicle
            string vehicleListId = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, Right.type.vehicleAccess);
            string[] vehicles = vehicleListId.Split(',');
            bool autopromoEvaliantOption = false;
            bool insertOption = false;
            foreach (string cVehicle in vehicles)
            {
                switch (VehiclesInformation.DatabaseIdToEnum(Int64.Parse(cVehicle)))
                {
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.adnettrack:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.mms:
                        autopromoEvaliantOption = VehiclesInformation.Get(Int64.Parse(cVehicle)).Autopromo;
                        break;
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.press:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.newspaper:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.magazine:
                    case TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.internationalPress:
                        insertOption = true;
                        break;
                }
            }
            #endregion

            #region InsertionFilter
            if (insertOption)
                _customerWebSession.Insert = (ConstantesSession.Insert)userFilter.InsertionFilter.Insertion;
            #endregion

            #region AutoPromoFilter
            if (autopromoEvaliantOption)
                _customerWebSession.AutoPromo = (ConstantesSession.AutoPromo)userFilter.AutoPromoFilter.AutoPromo;
            #endregion

            #region FormatFilter
            if (autopromoEvaliantOption)
                _customerWebSession.SelectedBannersFormatList = userFilter.FormatFilter.Formats;
            #endregion

            #region PurchaseModeFilter
            if (autopromoEvaliantOption)
                _customerWebSession.SelectedPurchaseModeList = userFilter.PurchaseModeFilter.PurchaseModes;
            #endregion

            _customerWebSession.Save();
        }

        #region Generic Detail Level Option Methodes
        private bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel, Int64 module)
        {
            ArrayList AllowedDetailLevelItems = GetAllowedDetailLevelItems();

            TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
            if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            object[] param = new object[2];
            param[0] = _customerWebSession;
            param[1] = _componentProfile;
            WebCore.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (WebCore.Utilities.MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            foreach (DetailLevelItemInformation currentDetailLevelItem in currentDetailLevel.Levels)
            {
                if (!AllowedDetailLevelItems.Contains(currentDetailLevelItem)) return (false);
                if (!mediaDetailLevelUtilities.CanAddDetailLevelItem(currentDetailLevelItem, module)) return (false);
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
#if Debug
            ////TODO : Resultat pour calendrier d'actiion : a enlever apres tests
            //// _customerSession.CurrentTab = 6;

            //_customerWebSession.SelectionUniversMedia.Nodes.Clear();
            //System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("RADIO");
            //tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, 2, "RADIO");
            //_customerWebSession.SelectionUniversMedia.Nodes.Add(tmpNode);
            //_customerWebSession.CurrentUniversMedia = _customerWebSession.SelectionUniversMedia;

            ////TODO :  selection support :  : a enlever apres tests
            //TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
            //Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

            //int groupIndex = 0;
            //Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            //NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, AccessType.includes);
            //Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
            //List<long> idUniversItems = new List<long>();
            //idUniversItems.Add(2003);//EUROPE 1
            //treeNomenclatureEG.AddItems(TNSClassificationLevels.MEDIA, idUniversItems);
            //adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
            //universes.Add(universes.Count, adExpressUniverse);
            //_customerWebSession.PrincipalMediaUniverses = universes;

            ////ArrayList levelIds = new ArrayList();
            ////levelIds.Add(11);
            ////levelIds.Add(12);
            ////levelIds.Add(10);            
            ////_customerSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levelIds, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

            //_customerWebSession.Save();
#endif
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

        private SelectControl DetailLevelItemInit(int level)
        {
            SelectControl selectControl = new SelectControl();
            selectControl.Id = "l" + level.ToString() + "Detail";
            selectControl.Items = new List<SelectItem>();
            selectControl.Items.Add(new SelectItem { Text = "-------", Value = "-1" });
            ArrayList AllowedDetailLevelItems = GetAllowedDetailLevelItems();

            TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
            if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            object[] param = new object[2];
            param[0] = _customerWebSession;
            param[1] = _componentProfile;
            WebCore.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (WebCore.Utilities.MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            foreach (DetailLevelItemInformation currentDetailLevelItem in AllowedDetailLevelItems)
            {
                if (mediaDetailLevelUtilities.CanAddDetailLevelItem(currentDetailLevelItem, _customerWebSession.CurrentModule))
                {
                    selectControl.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, _customerWebSession.SiteLanguage), Value = currentDetailLevelItem.Id.GetHashCode().ToString() });
                }
            }
            mediaDetailLevelUtilities = null;
            clMediaU = null;

            return selectControl;
        }
        #endregion

        #region Unit Option Methodes
        private bool ContainsDefaultCurrency(List<UnitInformation> units)
        {
            foreach (UnitInformation currentUnit in units)
                if (currentUnit.Id == UnitsInformation.DefaultCurrency)
                    return true;

            return false;
        }
        #endregion

        protected MediaItemsList GetSelectedUniverseMedia(WebSession webSession)
        {
            MediaItemsList selectedUniverseMedia = new MediaItemsList();
            // vehicle Selected
            selectedUniverseMedia.VehicleList = webSession.GetSelection(webSession.SelectionUniversMedia, Right.type.vehicleAccess);
            return selectedUniverseMedia;
        }

        /// <summary>
        /// Determine si un résultat doit être montré.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="current">Page en cours</param>
        /// <returns>Vrai si un résultat doit être montré </returns>
        protected bool CanShowResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {
            switch (webSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    return CanShowPortofolioResult(webSession, current);
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    return CanShowSynthesisResult(webSession, current);
                case WebConstantes.Module.Name.INDICATEUR:
                    return CanShowProductClassAnalysisResult(webSession, current);
                case WebConstantes.Module.Name.BILAN_CAMPAGNE:
                    if ((webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
                   && current.Id == FrameWorkResults.APPM.mediaPlanByVersion && !webSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_SLOGAN_ACCESS_FLAG)) return false;
                    else return true;
                default: return true;
            }
        }
        /// <summary>
        /// Check if a result can be shown
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="current">Current result</param>
        /// <returns>True or False</returns>
        protected bool CanShowProductClassAnalysisResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {

            VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            if (vehicleInfo != null && (vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia
                || vehicleInfo.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.PlurimediaWithoutMms)
                && current.Id == FrameWorkResults.ProductClassAnalysis.EVOLUTION && WebApplicationParameters.HidePlurimediaEvol)
            {
                return false;
            }

            return true;
        }
        /// <summary>
        /// Determine si un résultat doit être montré.
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="current">Page en cours</param>
        protected bool CanShowPortofolioResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {
            #region VehicleInformation
            VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            #endregion

            switch (vehicleInformation.Id)
            {
                case ClassificationCst.DB.Vehicles.names.directMarketing:
                case ClassificationCst.DB.Vehicles.names.internet:
                case ClassificationCst.DB.Vehicles.names.czinternet:
                case ClassificationCst.DB.Vehicles.names.mailValo:
                    return ((current.Id == FrameWorkResults.Portofolio.SYNTHESIS
                        || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO));
                case ClassificationCst.DB.Vehicles.names.outdoor:
                case ClassificationCst.DB.Vehicles.names.instore:
                case ClassificationCst.DB.Vehicles.names.indoor:
                case ClassificationCst.DB.Vehicles.names.cinema:
                case ClassificationCst.DB.Vehicles.names.adnettrack:
                case ClassificationCst.DB.Vehicles.names.evaliantMobile:
                case ClassificationCst.DB.Vehicles.names.mms:
                case ClassificationCst.DB.Vehicles.names.search:
                case ClassificationCst.DB.Vehicles.names.social:
                    return (current.Id == FrameWorkResults.Portofolio.SYNTHESIS || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO
                        || (current.Id == FrameWorkResults.Portofolio.CALENDAR && webSession.CustomerPeriodSelected.IsSliding4M));
                case ClassificationCst.DB.Vehicles.names.others:
                case ClassificationCst.DB.Vehicles.names.tv:
                case ClassificationCst.DB.Vehicles.names.tvGeneral:
                case ClassificationCst.DB.Vehicles.names.tvSponsorship:
                case ClassificationCst.DB.Vehicles.names.tvAnnounces:
                case ClassificationCst.DB.Vehicles.names.tvNonTerrestrials:
                case ClassificationCst.DB.Vehicles.names.radio:
                case ClassificationCst.DB.Vehicles.names.radioGeneral:
                case ClassificationCst.DB.Vehicles.names.radioSponsorship:
                case ClassificationCst.DB.Vehicles.names.radioMusic:
                case ClassificationCst.DB.Vehicles.names.press:
                case ClassificationCst.DB.Vehicles.names.newspaper:
                case ClassificationCst.DB.Vehicles.names.magazine:
                case ClassificationCst.DB.Vehicles.names.internationalPress:
                    return (current.Id == FrameWorkResults.Portofolio.SYNTHESIS || current.Id == FrameWorkResults.Portofolio.DETAIL_PORTOFOLIO
                        || (webSession.CustomerPeriodSelected.IsSliding4M));
                default: throw new Exception("ResultsOptionsWebControl : Vehicle unknown.");
            }
        }

        /// <summary>
        /// Can Show Synthesis Result
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="current">Current Page</param>
        protected bool CanShowSynthesisResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {

            VehicleInformation vehicleInformation = VehiclesInformation.Get(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);

            if ((vehicleInformation.Id == ClassificationCst.DB.Vehicles.names.search
                || vehicleInformation.Id == ClassificationCst.DB.Vehicles.names.social
                ) && current.Id == FrameWorkResults.DynamicAnalysis.SYNTHESIS)
                return false;

            return true;
        }

        /// <summary>
        /// Set default current tab
        /// </summary>
        /// <param name="resultToShow">tab list</param>
        /// <remarks>Synthesis tab is define by default for portofolio module depending on poeriod selected or vehicle</remarks>
        protected void SetDefaultTab(List<long> resultToShow,WebSession customerWebSession, ResultTypeOption resultTypeOption, Options options)
        {
            switch (customerWebSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                        options.ResultTypeOption = resultTypeOption;                                              
                    }
                    else
                    {
                        customerWebSession.CurrentTab = FrameWorkResults.Portofolio.SYNTHESIS;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                        options.ResultTypeOption = resultTypeOption;
                    }
                    else {                       
                        customerWebSession.CurrentTab = FrameWorkResults.DynamicAnalysis.PORTEFEUILLE;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();                     
                    }
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                        options.ResultTypeOption = resultTypeOption;
                    }
                    else {
                        customerWebSession.CurrentTab = FrameWorkResults.ProductClassAnalysis.SUMMARY;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                default:
                    resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    options.ResultTypeOption = resultTypeOption;
                    break;
            }
        }




    }
}

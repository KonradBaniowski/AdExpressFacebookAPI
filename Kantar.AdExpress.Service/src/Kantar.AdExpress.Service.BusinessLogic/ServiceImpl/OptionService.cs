
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
using Kantar.AdExpress.Service.Core.Domain;
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;
using System.Linq;
using System.Web.UI.WebControls;
using System.Windows.Forms;


namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public partial class OptionService : IOptionService
    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private WebSession _customerWebSession = null;
        private WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
        private WebNavigation.Module _currentModule;
        private int _nbDetailLevelItemList = 4;
        private Hashtable _genericDetailLevelsSaved = new Hashtable();
        private GenericDetailLevel _customerGenericDetailLevel = null;
        protected WebConstantes.GenericDetailLevel.Type _genericColumnDetailLevelType;
        protected int _nbColumnDetailLevelItemList = 1;

        public Options GetOptions(string idWebSession, HttpContextBase httpContext)
        {

            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            Options options = new Options();
            try
            {
                _currentModule = WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule);
                options.SiteLanguage = _customerWebSession.SiteLanguage;
                switch (_customerWebSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
                        _nbDetailLevelItemList = 4;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    case WebConstantes.Module.Name.NEW_CREATIVES:
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;
                        _nbDetailLevelItemList = 3;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;
                        _nbDetailLevelItemList = 4;
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
                            _customerWebSession.GenericMediaDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels);
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
                            _customerWebSession.GenericProductDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels);
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
                switch (_componentProfile)
                {
                    case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                        _customerGenericDetailLevel = _customerWebSession.GenericMediaDetailLevel;
                        break;
                    case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                        _customerGenericDetailLevel = _customerWebSession.GenericProductDetailLevel;
                        break;
                }

                bool hasSelectedId = false;
                if (_customerWebSession.GenericMediaDetailLevel.FromControlItem == WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels)
                {
                    int index = -1;
                    foreach (GenericDetailLevel currentLevel in DefaultDetailLevels)
                    {
                        index++;
                        if (currentLevel.EqualLevelItems(_customerGenericDetailLevel))
                        {
                            genericDetailLevelOption.DefaultDetail.SelectedId = index.ToString();
                            hasSelectedId = true;
                        }
                    }
                }

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
                if (!hasSelectedId && _customerGenericDetailLevel.FromControlItem == WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels)
                {
                    foreach (GenericDetailLevelSaved currentLevel in _genericDetailLevelsSaved.Values)
                    {
                        if (currentLevel.EqualLevelItems(_customerGenericDetailLevel))
                        {
                            genericDetailLevelOption.CustomDetail.SelectedId = currentLevel.Id.ToString();
                            hasSelectedId = true;
                        }
                    }
                }
                #endregion

                #region Niveau de détaille par défaut

                #region L1
                if (_nbDetailLevelItemList >= 1)
                {
                    int? selectedL1Id = null;
                    if (!hasSelectedId && _customerGenericDetailLevel.LevelIds.Count>0)
                    {
                        selectedL1Id = Convert.ToInt32(_customerGenericDetailLevel.LevelIds[0]);
                    }
                    genericDetailLevelOption.L1Detail = DetailLevelItemInit(1, selectedL1Id);

                }
                #endregion

                #region L2
                if (_nbDetailLevelItemList >= 2)
                {
                    int? selectedL2Id = null;
                    if (!hasSelectedId && _customerGenericDetailLevel.LevelIds.Count > 1)
                    {
                        selectedL2Id = Convert.ToInt32(_customerGenericDetailLevel.LevelIds[1]);
                    }
                    genericDetailLevelOption.L2Detail = DetailLevelItemInit(2, selectedL2Id);
                }
                #endregion

                #region L3
                if (_nbDetailLevelItemList >= 3)
                {
                    int? selectedL3Id = null;
                    if (!hasSelectedId && _customerGenericDetailLevel.LevelIds.Count > 2)
                    {
                        selectedL3Id = Convert.ToInt32(_customerGenericDetailLevel.LevelIds[2]);
                    }
                    genericDetailLevelOption.L3Detail = DetailLevelItemInit(3, selectedL3Id);
                }
                #endregion

                #region L4
                if (_nbDetailLevelItemList >= 4)
                {
                    int? selectedL4Id = null;
                    if (!hasSelectedId && _customerGenericDetailLevel.LevelIds.Count > 3)
                    {
                        selectedL4Id = Convert.ToInt32(_customerGenericDetailLevel.LevelIds[3]);
                    }
                    genericDetailLevelOption.L4Detail = DetailLevelItemInit(4, selectedL4Id);
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
                DateTime begin = WebCore.Utilities.Dates.GetPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, _customerWebSession.PeriodType);
                if ((_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE) || (begin >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3)))
                {
                    PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2289, _customerWebSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.dayly.GetHashCode().ToString() });
                }

                if ((_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES)
                    && _customerWebSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
                {
                    if (WebCore.Utilities.Dates.GetPeriodBeginningDate(_customerWebSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                        < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                    {
                        _customerWebSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                    }
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                    _customerWebSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE &&
                    WebApplicationParameters.CountryCode == WebConstantes.CountryCode.TURKEY)
                {
                    _customerWebSession.DetailPeriod = ConstantesPeriod.DisplayLevel.dayly;
                    PeriodDetail.PeriodDetailType.Visible = true;
                }
                else if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
                    PeriodDetail.PeriodDetailType.Visible = false;

                PeriodDetail.PeriodDetailType.SelectedId = _customerWebSession.DetailPeriod.GetHashCode().ToString();

                options.PeriodDetail = PeriodDetail;
                #endregion

                #region resultTypeOption
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                {
                    ResultTypeOption resultTypeOption = new ResultTypeOption();
                    resultTypeOption.ResultType = new SelectControl();
                    resultTypeOption.ResultType.Id = "resultType";
                    resultTypeOption.ResultType.Visible = true;
                    resultTypeOption.ResultType.Items = new List<SelectItem>();


                    resultTypeOption.ResultType.Items.Add(new SelectItem
                    {
                        Text = string.Format("{0} / {1}", "Autres Dimensions", "Periode"),
                        Value = ConstantesSession.PreformatedDetails.PreformatedTables.othersDimensions_X_Period.GetHashCode().ToString()
                    });
                    resultTypeOption.ResultType.Items.Add(new SelectItem
                    {
                        Text = string.Format("{0} / {1}", "Autres Dimensions", "Supports"),
                        Value = ConstantesSession.PreformatedDetails.PreformatedTables.othersDimensions_X_Media.GetHashCode().ToString()
                    });
                    // "Produits+Supports / Année"
                    resultTypeOption.ResultType.Items.Add(new SelectItem
                    {
                        Text = string.Format("{0} / {1}", "Autres Dimensions", "Unites"),
                        Value = ConstantesSession.PreformatedDetails.PreformatedTables.othersDimensions_X_Units.GetHashCode().ToString()
                    });
                    resultTypeOption.ResultType.SelectedId = _customerWebSession.PreformatedTable.GetHashCode().ToString();
                    options.ResultTypeOption = resultTypeOption;
                }
                else if (WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA != _customerWebSession.CurrentModule)
                {
                    ResultTypeOption resultTypeOption = new ResultTypeOption();

                    resultTypeOption.ResultType = new SelectControl();
                    resultTypeOption.ResultType.Id = "resultType";
                    resultTypeOption.ResultType.Items = new List<SelectItem>();

                    if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES &&
                        WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
                        resultTypeOption.ResultType.Visible = true;

                    List<long> resultToShow = new List<long>();
                    var selectedMediaUniverse = GetSelectedUniverseMedia(_customerWebSession);
                    List<WebNavigation.ResultPageInformation> resultPages = _currentModule.GetValidResultsPage(selectedMediaUniverse);

                    foreach (WebNavigation.ResultPageInformation current in resultPages)
                    {
                        if (!CanShowResult(_customerWebSession, current)) continue;
                        resultToShow.Add(current.Id);
                        resultTypeOption.ResultType.Items.Add(new SelectItem
                        {
                            Text = GestionWeb.GetWebWord(current.IdWebText,
                            _customerWebSession.SiteLanguage),
                            Value = current.Id.ToString()
                        });
                    }
                    SetDefaultTab(resultToShow, _customerWebSession, resultTypeOption, options);
                }

                #endregion

                #region Options by Vehicle
                string vehicleListId = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, Right.type.vehicleAccess);
                string[] vehicles = vehicleListId.Split(',');
                bool autopromoEvaliantOption = false;
                bool filterSpotSubType = false;

                bool insertOption = false;
                foreach (string cVehicle in vehicles)
                {
                    switch (VehiclesInformation.DatabaseIdToEnum(Int64.Parse(cVehicle)))
                    {
                        case Vehicles.names.adnettrack:
                        case Vehicles.names.evaliantMobile:
                        case Vehicles.names.mms:
                            autopromoEvaliantOption = VehiclesInformation.Get(Int64.Parse(cVehicle)).Autopromo;
                            break;
                        case Vehicles.names.press:
                        case Vehicles.names.newspaper:
                        case Vehicles.names.magazine:
                        case Vehicles.names.internationalPress:
                            if (WebApplicationParameters.AllowInsetOption) insertOption = true;
                            break;
                        case Vehicles.names.tv:
                            filterSpotSubType = WebApplicationParameters.UseSpotSubType
                 && _customerWebSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES;
                            break;
                    }
                }
                #endregion

                #region UnitOption

                UnitOption unitOption = new UnitOption();

                unitOption.Unit = new UnitSelectControl();
                unitOption.Unit.Id = "unit";
                unitOption.Unit.Items = new List<SelectItem>();
                var unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
                var vehicleInformation = VehiclesInformation.Get(Convert.ToInt64(vehicles[0]));
                List<UnitInformation> units;

                //if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                //    && WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
                //{
                //    units = GetValidUnitForResult(_customerWebSession.CurrentModule, FrameWorkResults.Portofolio.CALENDAR);
                //}
                //else
                    units = _customerWebSession.GetValidUnitForResult();

                for (int i = 0; i < units.Count; i++)
                {
                    unitInformationDictionary.Add(units[i].Id, units[i]);
                }

                if ((_customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                      || !WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY)))
                {
                    if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA &&
                        (!_customerWebSession.ReachedModule ||
                         !unitInformationDictionary.ContainsKey(_customerWebSession.Unit)))
                    {
                        _customerWebSession.Units = new List<ConstantesSession.Unit>
                        {
                            WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule)
                                .GetResultPageInformation(_customerWebSession.CurrentTab)
                                .GetDefaultUnit(vehicleInformation.Id)
                        };
                    }
                }

                AddUnitOptions(units, unitOption);

                var exceptUnits = units.Intersect(UnitsInformation.Get(_customerWebSession.Units));
                if (!exceptUnits.Any())
                {
                    if (ContainsDefaultCurrency(units))
                    {
                        _customerWebSession.Units = new List<ConstantesSession.Unit>
                        {
                            UnitsInformation.DefaultCurrency
                        };
                    }
                    else
                        _customerWebSession.Units = new List<ConstantesSession.Unit> {units[0].Id};
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    _customerWebSession.Units = ContainsDefaultCurrency(units)
                        ? new List<ConstantesSession.Unit> {UnitsInformation.DefaultCurrency}
                        : new List<ConstantesSession.Unit> {units[0].Id};
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES
                    && WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
                {
                    _customerWebSession.Units = new List<ConstantesSession.Unit> { ConstantesSession.Unit.versionNb };
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                   && WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
                {
                    unitOption.Unit.Visible = true;
                }

                unitOption.Unit.SelectedIds = _customerWebSession.Units.Select(u => u.GetHashCode().ToString()).ToList();

                

                options.UnitOption = unitOption;
                #endregion

                #region PercentageOption

                PercentageOption percentageOption = new PercentageOption();

                percentageOption.Percentage = new SelectControl();
                percentageOption.Percentage.Id = "percentage";
                percentageOption.Percentage.Items = new List<SelectItem>();

                percentageOption.Percentage.Items.Add(new SelectItem { Text = "----------------------", Value = WebConstantes.Percentage.Alignment.none.GetHashCode().ToString() });
                percentageOption.Percentage.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2065, _customerWebSession.SiteLanguage), Value = WebConstantes.Percentage.Alignment.vertical.GetHashCode().ToString() });

                if (_customerWebSession.PreformatedTable != ConstantesSession.PreformatedDetails.PreformatedTables.othersDimensions_X_Units)
                    percentageOption.Percentage.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2064, _customerWebSession.SiteLanguage), Value = WebConstantes.Percentage.Alignment.horizontal.GetHashCode().ToString() });

                try
                {
                    foreach (var item in percentageOption.Percentage.Items)
                        if (_customerWebSession.PercentageAlignment.GetHashCode().ToString() == item.Value)
                            percentageOption.Percentage.SelectedId = item.Value;

                }
                catch (System.Exception)
                {
                    try
                    {
                        percentageOption.Percentage.SelectedId = WebConstantes.Percentage.Alignment.none.GetHashCode().ToString();
                    }
                    catch (System.Exception)
                    {
                    }
                }

                options.PercentageOption = percentageOption;
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
                autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.total], _customerWebSession.SiteLanguage), Value = "0" });
                autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoAdvertiser], _customerWebSession.SiteLanguage), Value = "1" });
                autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoHoldingCompany], _customerWebSession.SiteLanguage), Value = "2" });

                autoPromoOption.AutoPromo.SelectedId = _customerWebSession.AutoPromo.GetHashCode().ToString();

                options.AutoPromoOption = autoPromoOption;
                #endregion



                #region FormatOption              
                FormatOption formatOption = new FormatOption();

                formatOption.Format = new SelectControl();
                formatOption.Format.Id = "format";
                formatOption.Format.Visible = autopromoEvaliantOption ;
                formatOption.Format.Items = new List<SelectItem>();

                var activeBannersFormatList = new List<FilterItem>(_customerWebSession.GetValidFormatList(_customerWebSession.GetVehiclesSelected()).Values);
                if (activeBannersFormatList.Count > 0)
                {
                    foreach (FilterItem item in activeBannersFormatList)
                    {
                        formatOption.Format.Items.Add(new SelectItem { Text = item.Label, Value = item.Id.ToString(),Enabled = item.IsEnable });
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

                #region Spot Sub Type Option
                if (filterSpotSubType)
                {
                    SpotSubTypeOption spotSubTypeOption = new SpotSubTypeOption();
                    spotSubTypeOption.SubType = new SelectControl();
                    spotSubTypeOption.SubType.Id = "__spotSubType";
                    spotSubTypeOption.SubType.Visible = filterSpotSubType;
                    spotSubTypeOption.SubType.Items = new List<SelectItem>();

                    var spotSubTypeItems = SpotSubTypes.GetItems()[_customerWebSession.DataLanguage];
                    if (spotSubTypeItems.Any())
                    {
                        spotSubTypeItems.ForEach(spotSubType =>
                        {
                            spotSubTypeOption.SubType.Items.Add(new SelectItem { Text = spotSubType.Label, Value = spotSubType.Id.ToString() });
                        });
                        spotSubTypeOption.SubType.SelectedId = !string.IsNullOrEmpty(_customerWebSession.SelectedSpotSubTypes) ? 
                            string.Join(",", spotSubTypeItems.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray())
                            : string.Empty;
                    }
                    else
                    {
                        spotSubTypeOption.SubType.Visible = false;
                    }
                    options.SpotSubTypeOption = spotSubTypeOption;
                }              
                #endregion

                #region PurchaseModeOption
                bool showPurchaseMode = WebApplicationParameters.UsePurchaseMode && _customerWebSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_PURCHASE_MODE_DISPLAY_FLAG)
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.TABLEAU_DYNAMIQUE;

                PurchaseModeOption purchaseModeOption = new PurchaseModeOption();

                purchaseModeOption.PurchaseMode = new SelectControl();
                purchaseModeOption.PurchaseMode.Id = "purchaseMode";
                purchaseModeOption.PurchaseMode.Visible = autopromoEvaliantOption;
                purchaseModeOption.PurchaseMode.Items = new List<SelectItem>();

                Dictionary<Int64, VehicleInformation> VehicleInformationList = _customerWebSession.GetVehiclesSelected();
                if (showPurchaseMode && VehiclesInformation.Contains(Vehicles.names.mms) && VehicleInformationList.ContainsKey(VehiclesInformation.Get(Vehicles.names.mms).DatabaseId))
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

                #region initializeMedia
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA)
                {
                    CheckBoxOption initializeMedia = new CheckBoxOption();
                    initializeMedia.Id = "initializeMedia";

                    if (_customerWebSession.PrincipalMediaUniverses.Count > 0)
                    {
                        initializeMedia.Enabled = true;
                    }
                    else
                    {
                        initializeMedia.Enabled = false;
                    }

                    options.InitializeMedia = initializeMedia;
                }
                #endregion

                #region initializeProduct
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE)
                {
                    CheckBoxOption initializeProduct = new CheckBoxOption();
                    initializeProduct.Id = "initializeProduct";

                    if (_customerWebSession.PrincipalProductUniverses.Count > 0
                        || _customerWebSession.SecondaryProductUniverses.Count > 0)
                    {
                        initializeProduct.Enabled = true;
                    }
                    else
                    {
                        initializeProduct.Enabled = false;
                    }

                    options.InitializeProduct = initializeProduct;
                }
                #endregion

                #region PDM
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                  || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE)
                {
                    CheckBoxOption pdm = new CheckBoxOption();
                    pdm.Id = "pdmEvol";

                    //TODO : verifier :
                    //if (_customerWebSession.CurrentTab == FrameWorkResults.CompetitorMarketShare.FORCES
                    //    || _customerWebSession.CurrentTab == FrameWorkResults.CompetitorMarketShare.POTENTIELS
                    //    )
                    //    pdm.Value = true;
                    //else pdm.Value = _customerWebSession.Percentage;
                    pdm.Value = _customerWebSession.Percentage;

                    options.PDM = pdm;
                }
                #endregion

                #region PDV
                if (WebApplicationParameters.CountryCode == WebConstantes.CountryCode.TURKEY &&
                    (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                  || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE))
                {
                    CheckBoxOption pdv = new CheckBoxOption();
                    pdv.Id = "pdvEvol";

                    pdv.Value = _customerWebSession.PDV;

                    options.PDV = pdv;
                }
                #endregion

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    #region Evolution
                    CheckBoxOption evol = new CheckBoxOption();
                    evol.Id = "analysisEvol";
                    evol.Value = _customerWebSession.Evolution;
                    evol.Enabled = false;
                    #endregion

                    #region PDM
                    CheckBoxOption pdm = new CheckBoxOption();
                    pdm.Id = "pdmEvol";
                    pdm.Value = _customerWebSession.PDM;
                    #endregion

                    #region PDV
                    CheckBoxOption pdv = new CheckBoxOption();
                    pdv.Id = "pdvEvol";
                    pdv.Value = _customerWebSession.PDV;
                    #endregion

                    options.Evol = evol;
                    options.PDM = pdm;
                    options.PDV = pdv;
                }

                switch (_customerWebSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                        options.GenericColumnDetailLevelOption = GetGenericColumnLevelDetailOptions(httpContext);
                        break;
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    options.GenericColumnDetailLevelOption = GetGenericMediaColumnLevelDetailOptions(httpContext);
                    options.GenericColumnDetailLevelOption.L1Detail.Visible = true;
                    options.GenericColumnDetailLevelOption.L1Detail.SelectedId = _customerWebSession.PreformatedMediaDetail.GetHashCode().ToString();
                    //_customerWebSession.PreformatedMediaDetail = (ConstantesSession.PreformatedDetails.PreformatedMediaDetails)ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode();
                }

                if (WebApplicationParameters.UseComparativeMediaSchedule || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    options.ComparativeStudy = _customerWebSession.ComparativeStudy;
                    _customerWebSession.ComparativePeriodType = _customerWebSession.ComparativePeriodType; //TNS.AdExpress.Constantes.Web.globalCalendar.comparativePeriodType.dateToDate;
                    options.ComparativePeriodType = _customerWebSession.ComparativePeriodType;
                }
                else
                    options.ComparativeStudy = false;

                if (WebApplicationParameters.UseRetailer)
                {
                    options.IsSelectRetailerDisplay = _customerWebSession.IsSelectRetailerDisplay;
                }
                else
                    options.IsSelectRetailerDisplay = false;

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

                _customerWebSession.ReachedModule = true;
                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return options;
        }

        private void AddUnitOptions(List<UnitInformation> units, UnitOption unitOption)
        {
            foreach (UnitInformation currentUnit in units)
            {
               

                if (currentUnit.Id == ConstantesSession.Unit.volume &&
                   !_customerWebSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_MARKETING_DIRECT))
                {
                    _customerWebSession.Units = new List<ConstantesSession.Unit>
                        {
                            UnitsInformation.DefaultCurrency
                        };
                    continue;
                }

                if (currentUnit.Id == ConstantesSession.Unit.volumeMms &&
                    !_customerWebSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_DISPLAY))
                {
                    _customerWebSession.Units = new List<ConstantesSession.Unit>
                        {
                            UnitsInformation.DefaultCurrency
                        };
                    continue;
                }


                    unitOption.Unit.Items.Add(new SelectItem
                {
                    Text = GestionWeb.GetWebWord(currentUnit.WebTextId,
                               _customerWebSession.SiteLanguage),
                    Value = currentUnit.Id.GetHashCode().ToString(),
                    GroupId = currentUnit.GroupId,
                    GroupType = currentUnit.GroupType,
                    GroupTextId = currentUnit.GroupTextId
                    });
            }
        }

        public void SetOptions(string idWebSession, UserFilter userFilter, HttpContextBase httpContext)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                _currentModule = WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule);
                switch (_customerWebSession.CurrentModule)
                {
                    case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
                        _nbDetailLevelItemList = 4;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    case WebConstantes.Module.Name.NEW_CREATIVES:
                        _nbDetailLevelItemList = 3;
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                        _nbDetailLevelItemList = 4;
                        _componentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.product;
                        break;
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
                {
                    SetGenericColumnLevelDetailOptions(userFilter);
                    _customerWebSession.Percentage = userFilter.PDM;
                    if (WebApplicationParameters.CountryCode == WebConstantes.CountryCode.TURKEY)
                        _customerWebSession.PDV = userFilter.PDV;
                }


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
                    _customerGenericDetailLevel.FromControlItem = WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels;
                }
                if (userFilter.GenericDetailLevelFilter.CustomDetailValue >= 0)
                {
                    _customerGenericDetailLevel = (GenericDetailLevel)_genericDetailLevelsSaved[(Int64)userFilter.GenericDetailLevelFilter.CustomDetailValue];
                    _customerGenericDetailLevel.FromControlItem = WebConstantes.GenericDetailLevel.SelectedFrom.savedLevels;
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
                    case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        _customerWebSession.GenericMediaDetailLevel = _customerGenericDetailLevel;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                        _customerWebSession.GenericProductDetailLevel = _customerGenericDetailLevel;
                        _customerWebSession.CurrentTab = userFilter.ResultTypeFilter.ResultType;
                        break;
                    case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                        _customerWebSession.GenericProductDetailLevel = _customerGenericDetailLevel;
                        break;
                    case WebConstantes.Module.Name.NEW_CREATIVES:
                        _customerWebSession.GenericProductDetailLevel = _customerGenericDetailLevel;
                        _customerWebSession.CurrentTab = userFilter.ResultTypeFilter.ResultType;
                        break;
                }

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                    SetGenericMediaColumnLevelDetailOptions(userFilter);


                #region PeriodDetailFilter
                if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                    _customerWebSession.DetailPeriod = (ConstantesPeriod.DisplayLevel)userFilter.PeriodDetailFilter.PeriodDetailType;
                #endregion

                #region UnitFilter

                if (!WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
                {
                    if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.NEW_CREATIVES
                        && _customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                        && !userFilter.UnitFilter.Unit.Contains(WebConstantes.CustomerSessions.Unit.none)
                    )
                        _customerWebSession.Units = userFilter.UnitFilter.Unit;
                }
                else
                {
                    if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.NEW_CREATIVES

                        && !userFilter.UnitFilter.Unit.Contains(WebConstantes.CustomerSessions.Unit.none)
                    )
                        _customerWebSession.Units = userFilter.UnitFilter.Unit;
                }

                #endregion

                #region PercentageFilter
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                    _customerWebSession.PercentageAlignment = (WebConstantes.Percentage.Alignment)userFilter.PercentageFilter.Percentage;
                #endregion

                #region Options by Vehicle
                Dictionary<Int64, VehicleInformation> VehicleInformationList = _customerWebSession.GetVehiclesSelected();
                string vehicleListId = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, Right.type.vehicleAccess);
                string[] vehicles = vehicleListId.Split(',');
                bool autopromoEvaliantOption = false;
                bool filterSpotSubType = false;
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
                        case Vehicles.names.tv:
                            filterSpotSubType = WebApplicationParameters.UseSpotSubType
                 && _customerWebSession.CurrentModule == WebConstantes.Module.Name.NEW_CREATIVES;
                            break;
                    }
                }
                #endregion

                #region InsertionFilter
                if (insertOption
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_MANDATAIRES
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                    _customerWebSession.Insert = (ConstantesSession.Insert)userFilter.InsertionFilter.Insertion;
                #endregion

                #region AutoPromoFilter
                if (autopromoEvaliantOption
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.NEW_CREATIVES
                    && _customerWebSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS)
                    _customerWebSession.AutoPromo = (ConstantesSession.AutoPromo)userFilter.AutoPromoFilter.AutoPromo;
                #endregion

                #region FormatFilter
                if (autopromoEvaliantOption && userFilter.FormatFilter.Formats != null)
                    _customerWebSession.SelectedBannersFormatList = userFilter.FormatFilter.Formats;
                #endregion

                #region SpotSub type filter
                if (filterSpotSubType && !string.IsNullOrEmpty(userFilter.SpotSubTypeFilter?.SpotSubTypes))
                    _customerWebSession.SelectedSpotSubTypes = userFilter.SpotSubTypeFilter.SpotSubTypes;
                #endregion

                #region PurchaseModeFilter
                if (VehiclesInformation.Contains(Vehicles.names.mms) && VehicleInformationList.ContainsKey(VehiclesInformation.Get(Vehicles.names.mms).DatabaseId))
                {
                    _customerWebSession.SelectedPurchaseModeList = (!string.IsNullOrEmpty(userFilter.PurchaseModeFilter.PurchaseModes))
                        ? userFilter.PurchaseModeFilter.PurchaseModes : string.Empty;
                }

                #endregion

                #region initializeMedia
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA && userFilter.InitializeMedia)
                    _customerWebSession.PrincipalMediaUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                #endregion

                #region initializeProduct
                if ((_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE) && userFilter.InitializeProduct)
                {
                    _customerWebSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                    _customerWebSession.SecondaryProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

                }
                #endregion

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS
                    || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                    _customerWebSession.PreformatedTable = (ConstantesSession.PreformatedDetails.PreformatedTables)userFilter.ResultTypeFilter.ResultType;

                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    _customerWebSession.Evolution = userFilter.Evol;
                    _customerWebSession.PDM = userFilter.PDM;
                    _customerWebSession.PDV = userFilter.PDV;
                }

                if (WebApplicationParameters.UseComparativeMediaSchedule || _customerWebSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    _customerWebSession.ComparativeStudy = userFilter.ComparativeStudy;
                    _customerWebSession.ComparativePeriodType = (WebConstantes.globalCalendar.comparativePeriodType)userFilter.ComparativePeriodType;
                }

                if (WebApplicationParameters.UseRetailer)
                {
                    _customerWebSession.IsSelectRetailerDisplay = userFilter.IsSelectRetailerDisplay;
                }

                //_customerWebSession.Grp = userFilter.Grp;
                //_customerWebSession.Grp30S = userFilter.Grp30S;
                //_customerWebSession.SpendsGrp = userFilter.SpendsGrp;

                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
        }

        public SaveLevelsResponse SaveCustomDetailLevels(string idWebSession, string levels, string type, HttpContextBase httpContext)
        {
            SaveLevelsResponse response = new SaveLevelsResponse();
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                if (levels.Length > 0 && type.Length > 0)
                {
                    WebConstantes.GenericDetailLevel.Type detailLevelType = (WebConstantes.GenericDetailLevel.Type)int.Parse(type);
                    int levelId;
                    string[] levelList = levels.Split(',');
                    ArrayList levelIds = new ArrayList();
                    foreach (string currentLevel in levelList)
                    {
                        if (currentLevel.Length > 0)
                        {
                            levelId = int.Parse(currentLevel);
                            if (levelId > 0) levelIds.Add(levelId);
                        }
                    }

                    if (levelIds.Count > 0)
                    {
                        GenericDetailLevel genericDetailLevel = new GenericDetailLevel(levelIds, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.savedLevels, detailLevelType);
                        if (IsDetailLevelsAlreadySaved(genericDetailLevel))
                        {//Tests if detail levels are already saved
                            response.CustomDetailLavelsId = -1;
                            response.Message = GestionWeb.GetWebWord(2256, _customerWebSession.SiteLanguage);
                        }
                        else
                        {
                            Int64 listId = TNS.AdExpress.Web.Core.DataAccess.Session.GenericDetailLevelDataAccess.Save(_customerWebSession, genericDetailLevel);

                            response.CustomDetailLavelsId = listId;
                            response.CustomDetailLavelsLabel = genericDetailLevel.GetLabel(_customerWebSession.SiteLanguage);
                            response.Message = GestionWeb.GetWebWord(3121, _customerWebSession.SiteLanguage);
                        }
                    }
                    else
                    {
                        response.CustomDetailLavelsId = -1;
                        response.Message = GestionWeb.GetWebWord(1945, _customerWebSession.SiteLanguage);
                    }
                }
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return response;
        }

        public string RemoveCustomDetailLevels(string idWebSession, string detailLevel)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            if (detailLevel.Length > 0 && int.Parse(detailLevel) != -1)
            {
                GenericDetailLevelDataAccess.Remove(_customerWebSession, Int64.Parse(detailLevel));
                return GestionWeb.GetWebWord(3120, _customerWebSession.SiteLanguage);
            }

            return GestionWeb.GetWebWord(3119, _customerWebSession.SiteLanguage);
        }

        #region IsDetailLevelsAlreadySaved
        private bool IsDetailLevelsAlreadySaved(GenericDetailLevel toTest)
        {
            DataSet ds;
            int currentLevelId;
            Int64 currentId = -1;
            int currentIndex = 0;
            ArrayList genericDetailLevelsSaved = new ArrayList();

            ds = GenericDetailLevelDataAccess.Load(_customerWebSession);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                foreach (DataRow currentRow in ds.Tables[0].Rows)
                {

                    if (currentId != Int64.Parse(currentRow["id_list"].ToString()))
                    {
                        if (currentId > -1 && ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).EqualLevelItems(toTest)) return true;
                        currentId = Int64.Parse(currentRow["id_list"].ToString());
                        currentIndex = genericDetailLevelsSaved.Add(new GenericDetailLevelSaved(currentId, new ArrayList()));
                    }
                    currentLevelId = int.Parse(currentRow["id_type_level"].ToString());
                    ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).AddLevel(currentLevelId);
                }
                if (currentId > -1 && ((GenericDetailLevelSaved)genericDetailLevelsSaved[currentIndex]).EqualLevelItems(toTest)) return true;
            }

            return false;
        }
        #endregion

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

        private SelectControl DetailLevelItemInit(int level, int? selectedId = null)
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
                    if (selectedId.HasValue && currentDetailLevelItem.Id.GetHashCode() == selectedId.Value)
                    {
                        selectControl.SelectedId = selectedId.Value.ToString();
                    }
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

        #region GetSelectedUniverseMedia
        protected MediaItemsList GetSelectedUniverseMedia(WebSession webSession)
        {
            MediaItemsList selectedUniverseMedia = new MediaItemsList();
            // vehicle Selected
            selectedUniverseMedia.VehicleList = webSession.GetSelection(webSession.SelectionUniversMedia, Right.type.vehicleAccess);
            return selectedUniverseMedia;
        }
        #endregion

        #region CanShowResult
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
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    return CanShowPresentAbsentResult(webSession, current);
                case WebConstantes.Module.Name.INDICATEUR:
                    return CanShowProductClassAnalysisResult(webSession, current);
                case WebConstantes.Module.Name.BILAN_CAMPAGNE:
                    if ((webSession.CurrentModule == WebConstantes.Module.Name.BILAN_CAMPAGNE)
                   && current.Id == FrameWorkResults.APPM.mediaPlanByVersion && !webSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_SLOGAN_ACCESS_FLAG)) return false;
                    else return true;
                default: return true;
            }
        }
        #endregion

        #region CanShowProductClassAnalysisResult
        /// <summary>
        /// Check if a result can be shown
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="current">Current result</param>
        /// <returns>True or False</returns>
        protected bool CanShowProductClassAnalysisResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {

            VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)webSession.SelectionUniversMedia.FirstNode.Tag).ID);
            if (vehicleInfo != null &&
                (vehicleInfo.Id == Vehicles.names.plurimedia                
                )
                && current.Id == FrameWorkResults.ProductClassAnalysis.EVOLUTION &&
                WebApplicationParameters.HidePlurimediaEvol)
            {
                return false;
            }

            return true;
        }
        #endregion

        #region CanShowPortofolioResult
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
                case ClassificationCst.DB.Vehicles.names.dooh:
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
        #endregion

        #region CanShowPresentAbsentResult
        protected bool CanShowPresentAbsentResult(WebSession webSession, WebNavigation.ResultPageInformation current)
        {
            return (webSession.PrincipalMediaUniverses.Count > 1 || current.Id == FrameWorkResults.CompetitorMarketShare.PORTEFEUILLE);
        }
        #endregion

        #region CanShowSynthesisResult
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
        #endregion

        #region SetDefaultTab
        /// <summary>
        /// Set default current tab
        /// </summary>
        /// <param name="resultToShow">tab list</param>
        /// <remarks>Synthesis tab is define by default for portofolio module depending on poeriod selected or vehicle</remarks>
        protected void SetDefaultTab(List<long> resultToShow, WebSession customerWebSession, ResultTypeOption resultTypeOption, Options options)
        {
            switch (customerWebSession.CurrentModule)
            {
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
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
                    }
                    else
                    {
                        customerWebSession.CurrentTab = FrameWorkResults.DynamicAnalysis.PORTEFEUILLE;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    else
                    {
                        customerWebSession.CurrentTab = FrameWorkResults.CompetitorMarketShare.PORTEFEUILLE;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    else
                    {
                        customerWebSession.CurrentTab = FrameWorkResults.ProductClassAnalysis.SUMMARY;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                case WebConstantes.Module.Name.NEW_CREATIVES:
                    if (resultToShow != null && resultToShow.Count > 0 && resultToShow.Contains(customerWebSession.CurrentTab))
                    {
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    else
                    {
                        customerWebSession.CurrentTab = FrameWorkResults.NewCreative.NEW_CREATIVE_REPORT;
                        resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    }
                    break;
                default:
                    resultTypeOption.ResultType.SelectedId = _customerWebSession.CurrentTab.ToString();
                    break;
            }
            options.ResultTypeOption = resultTypeOption;
        }
        #endregion

        private GenericColumnDetailLevelOption GetGenericMediaColumnLevelDetailOptions(HttpContextBase httpContext)
        {
            GenericColumnDetailLevelOption genericColumnDetailLevelOption = new GenericColumnDetailLevelOption();
            try
            {
                genericColumnDetailLevelOption.L1Detail = new SelectControl();
                genericColumnDetailLevelOption.L1Detail.Id = "columnDetail";

                var selectItems = new List<SelectItem>();

                selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1382, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.category.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.media))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(18, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.Media.GetHashCode().ToString() });

                selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1976, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.title.GetHashCode().ToString() });
                selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1142, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.media))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1544, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1860, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller.GetHashCode().ToString() });

                selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(2969, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicleTitle.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1383, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.mediaSeller.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(2812, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicle.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.media) && HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1862, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(2813, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.mediaSellerCategory.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.mediaSeller))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(2970, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.mediaSellerTitle.GetHashCode().ToString() });

                if (HasMediaClassifLevel(DetailLevelItemInformation.Levels.media))
                    selectItems.Add(new SelectItem { Text = GestionWeb.GetWebWord(1143, _customerWebSession.SiteLanguage), Value = ConstantesSession.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString() });

                genericColumnDetailLevelOption.L1Detail.Items = selectItems;
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return genericColumnDetailLevelOption;
        }

        private void SetGenericMediaColumnLevelDetailOptions(UserFilter userFilter)
        {
            _customerWebSession.PreformatedMediaDetail = (ConstantesSession.PreformatedDetails.PreformatedMediaDetails)userFilter.GenericColumnDetailLevelFilter.L1DetailValue;
            ////TODO : a checker :
            //ArrayList levels = new ArrayList();
            //levels.Add(userFilter.GenericColumnDetailLevelFilter.L1DetailValue);
            //if (levels.Count > 0)
            //{
            //    _genericColumnDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
            //}
            //_customerWebSession.GenericColumnDetailLevel = _genericColumnDetailLevel;
        }

        private bool HasMediaClassifLevel(DetailLevelItemInformation.Levels level)
        {
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            bool hasMediaLevel = false;
            if (!string.IsNullOrEmpty(listStr))
            {
                List<string> arrStr = new List<string>(listStr.Split(','));
                List<Int64> idMedias = arrStr.ConvertAll(Convert.ToInt64);

                List<DetailLevelItemInformation> levelInfos = VehiclesInformation.GetSelectionDetailLevelList(idMedias);
                foreach (DetailLevelItemInformation currentDetailLevelItem in levelInfos)
                {
                    if (currentDetailLevelItem.Id == level)
                    {
                        hasMediaLevel = true; break;
                    }
                }

            }
            return hasMediaLevel;
        }

        /// <summary>
        /// Get Valid Unit List for a specific result
        /// </summary>
        /// <returns>Valid Unit List for a specific result</returns>
        public List<UnitInformation> GetValidUnitForResult(long currentModule, int currentTab)
        {
            WebNavigation.Module moduleDescription = WebNavigation.ModulesList.GetModule(currentModule);
            WebNavigation.ResultPageInformation resultPageInformation = moduleDescription.GetResultPageInformation(currentTab);
            Dictionary<Int64, VehicleInformation> vehcileInformationList = _customerWebSession.GetVehiclesSelected();
            List<Vehicles.names> vehicleList = new List<Vehicles.names>();
            foreach (var cVehicleInformation in vehcileInformationList.Values)
            {
                vehicleList.Add(cVehicleInformation.Id);
            }

            List<UnitInformation> units = resultPageInformation.GetValidUnits(vehicleList);

            return units;
        }

    }
}

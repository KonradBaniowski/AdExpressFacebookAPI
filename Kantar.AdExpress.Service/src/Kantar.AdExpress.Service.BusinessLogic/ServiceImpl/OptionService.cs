using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
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

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class OptionService : IOptionService
    {
        private WebSession CustomerSession = null;
        private WebConstantes.GenericDetailLevel.ComponentProfile ComponentProfile = WebConstantes.GenericDetailLevel.ComponentProfile.media;
        private WebNavigation.Module CurrentModule;
        private int NbDetailLevelItemList = 4;
        private Hashtable GenericDetailLevelsSaved = new Hashtable();
        private GenericDetailLevel CustomerGenericDetailLevel = null;

        public Options GetOptions(string idWebSession)
        {
            CustomerSession = (WebSession)WebSession.Load(idWebSession);

            TNS.AdExpress.Classification.AdExpressUniverse universe = new TNS.AdExpress.Classification.AdExpressUniverse("test", TNS.Classification.Universe.Dimension.product);
            var group = new TNS.Classification.Universe.NomenclatureElementsGroup("Annonceur", 0, TNS.Classification.Universe.AccessType.includes);
            group.AddItems(TNS.Classification.Universe.TNSClassificationLevels.ADVERTISER, "54410,34466,7798,50270,71030");
            universe.AddGroup(universe.Count(), group);
            var universeDictionary = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            universeDictionary.Add(universeDictionary.Count, universe);
            CustomerSession.PrincipalProductUniverses = universeDictionary;
            CurrentModule = WebNavigation.ModulesList.GetModule(CustomerSession.CurrentModule);

            Options options = new Options();

            #region GenericDetailLevelOption
            GenericDetailLevelOption genericDetailLevelOption = new GenericDetailLevelOption();

            #region on vérifie que le niveau sélectionné à le droit d'être utilisé
            bool canAddDetail = false;
            switch (ComponentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    try
                    {
                        canAddDetail = CanAddDetailLevel(CustomerSession.GenericMediaDetailLevel, CustomerSession.CurrentModule);
                    }
                    catch { }
                    if (!canAddDetail)
                    {
                        // Niveau de détail par défaut
                        ArrayList levelsIds = new ArrayList();
                        switch (CustomerSession.CurrentModule)
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
                        CustomerSession.GenericMediaDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
                    }
                    break;
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    try
                    {
                        canAddDetail = CanAddDetailLevel(CustomerSession.GenericProductDetailLevel, CustomerSession.CurrentModule);
                    }
                    catch { }
                    if (!canAddDetail)
                    {
                        // Niveau de détail par défaut
                        ArrayList levelsIds = new ArrayList();
                        levelsIds.Add((int)DetailLevelItemInformation.Levels.advertiser);
                        CustomerSession.GenericProductDetailLevel = new GenericDetailLevel(levelsIds, WebConstantes.GenericDetailLevel.SelectedFrom.unknown);
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
                if (CanAddDetailLevel(currentLevel, CustomerSession.CurrentModule))
                    genericDetailLevelOption.DefaultDetail.Items.Add(new SelectItem { Text = currentLevel.GetLabel(CustomerSession.SiteLanguage), Value = DefaultDetailLevelId.ToString() });
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
                if (CanAddDetailLevel(currentGenericLevel, CustomerSession.CurrentModule) && currentGenericLevel.GetNbLevels <= NbDetailLevelItemList)
                {
                    genericDetailLevelOption.CustomDetail.Items.Add(new SelectItem { Text = currentGenericLevel.GetLabel(CustomerSession.SiteLanguage), Value = currentGenericLevel.Id.ToString() });
                    GenericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
                }
            }
            #endregion

            #region Niveau de détaille par défaut

            #region L1
            if (NbDetailLevelItemList >= 1)
            {
                genericDetailLevelOption.L1Detail = DetailLevelItemInit(1);
            }
            #endregion

            #region L2
            if (NbDetailLevelItemList >= 2)
            {
                genericDetailLevelOption.L2Detail = DetailLevelItemInit( 2);
            }
            #endregion

            #region L3
            if (NbDetailLevelItemList >= 3)
            {
                genericDetailLevelOption.L3Detail = DetailLevelItemInit(3);
            }
            #endregion

            #region L4
            if (NbDetailLevelItemList >= 4)
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
            PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2290, CustomerSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.monthly.GetHashCode().ToString() });
            PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(848, CustomerSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.weekly.GetHashCode().ToString() });
            DateTime begin = WebCore.Utilities.Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, CustomerSession.PeriodType);
            if (begin >= DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
            {
                PeriodDetail.PeriodDetailType.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2289, CustomerSession.SiteLanguage), Value = ConstantesPeriod.DisplayLevel.dayly.GetHashCode().ToString() });
            }

            if (CustomerSession.DetailPeriod == ConstantesPeriod.DisplayLevel.dayly)
            {
                if (WebCore.Utilities.Dates.getPeriodBeginningDate(CustomerSession.PeriodBeginningDate, ConstantesPeriod.Type.dateToDate)
                    < DateTime.Now.Date.AddDays(1 - DateTime.Now.Day).AddMonths(-3))
                {
                    CustomerSession.DetailPeriod = ConstantesPeriod.DisplayLevel.monthly;
                }
            }

            PeriodDetail.PeriodDetailType.SelectedId = CustomerSession.DetailPeriod.GetHashCode().ToString();

            options.PeriodDetail = PeriodDetail;
            #endregion

            #region UnitOption
            UnitOption unitOption = new UnitOption();

            unitOption.Unit = new SelectControl();
            unitOption.Unit.Id = "unit";
            unitOption.Unit.Items = new List<SelectItem>();

            List<UnitInformation> units = CustomerSession.GetValidUnitForResult();

            foreach (UnitInformation currentUnit in units)
            {
                if (currentUnit.Id != ConstantesSession.Unit.volume || CustomerSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_MARKETING_DIRECT))
                {
                    if (currentUnit.Id != ConstantesSession.Unit.volumeMms || CustomerSession.CustomerLogin.CustormerFlagAccess(ConstantesDB.Flags.ID_VOLUME_DISPLAY))
                        unitOption.Unit.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentUnit.WebTextId, CustomerSession.SiteLanguage), Value = currentUnit.Id.GetHashCode().ToString() });
                    else if (CustomerSession.Unit == ConstantesSession.Unit.volumeMms)
                        CustomerSession.Unit = UnitsInformation.DefaultCurrency;
                }
                else if (CustomerSession.Unit == ConstantesSession.Unit.volume)
                    CustomerSession.Unit = UnitsInformation.DefaultCurrency;
            }

            if (!units.Contains(UnitsInformation.Get(CustomerSession.Unit)))
            {
                if (ContainsDefaultCurrency(units))
                    CustomerSession.Unit = UnitsInformation.DefaultCurrency;
                else
                    CustomerSession.Unit = units[0].Id;
            }

            unitOption.Unit.SelectedId = CustomerSession.Unit.GetHashCode().ToString();

            options.UnitOption = unitOption;
            #endregion

            #region Options by Vehicle
            string vehicleListId = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, Right.type.vehicleAccess);
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
                insertionOption.Insertion.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.InsertsTraductionCodes[(ConstantesSession.Insert)inserts[j]], CustomerSession.SiteLanguage), Value = ((int)(ConstantesSession.Insert)inserts[j]).ToString() });
            }

            insertionOption.Insertion.SelectedId = CustomerSession.Insert.GetHashCode().ToString();

            options.InsertionOption = insertionOption;
            #endregion

            #region AutoPromoOption
            AutoPromoOption autoPromoOption = new AutoPromoOption();

            autoPromoOption.AutoPromo = new SelectControl();
            autoPromoOption.AutoPromo.Id = "autoPromo";
            autoPromoOption.AutoPromo.Visible = autopromoEvaliantOption;
            autoPromoOption.AutoPromo.Items = new List<SelectItem>();

            ArrayList autoPromoItems = new ArrayList();
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.total], CustomerSession.SiteLanguage),  Value = "0" });
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoAdvertiser], CustomerSession.SiteLanguage), Value = "1" });
            autoPromoOption.AutoPromo.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord((int)ConstantesSession.AutoPromoTraductionCodes[ConstantesSession.AutoPromo.exceptAutoPromoHoldingCompany], CustomerSession.SiteLanguage), Value = "2" });

            autoPromoOption.AutoPromo.SelectedId = CustomerSession.AutoPromo.GetHashCode().ToString();

            options.AutoPromoOption = autoPromoOption;
            #endregion

            #region FormatOption
            FormatOption formatOption = new FormatOption();

            formatOption.Format = new SelectControl();
            formatOption.Format.Id = "format";
            formatOption.Format.Visible = autopromoEvaliantOption;
            formatOption.Format.Items = new List<SelectItem>();

            var activeBannersFormatList = new List<FilterItem>(CustomerSession.GetValidFormatList(CustomerSession.GetVehiclesSelected()).Values);
            if (activeBannersFormatList.Count > 0)
            {
                foreach (FilterItem item in activeBannersFormatList)
                {
                    formatOption.Format.Items.Add(new SelectItem { Text = item.Label, Value = item.Id.ToString() });
                }

                if (string.IsNullOrEmpty(CustomerSession.SelectedBannersFormatList))
                    formatOption.Format.SelectedId = string.Join(",", activeBannersFormatList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
                else formatOption.Format.SelectedId = CustomerSession.SelectedBannersFormatList;
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

            Dictionary<Int64, VehicleInformation> VehicleInformationList = CustomerSession.GetVehiclesSelected();
            if (VehicleInformationList.ContainsKey(VehiclesInformation.Get(Vehicles.names.mms).DatabaseId))
            {
                var purchaseModeList = new List<FilterItem>(PurchaseModeList.GetList().Values);
                if (purchaseModeList.Count > 0)
                {
                    foreach (FilterItem item in purchaseModeList)
                    {
                        purchaseModeOption.PurchaseMode.Items.Add(new SelectItem { Text = item.Label, Value = item.Id.ToString() });
                    }

                    if (string.IsNullOrEmpty(CustomerSession.SelectedPurchaseModeList))
                        purchaseModeOption.PurchaseMode.SelectedId = string.Join(",", purchaseModeList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
                    else purchaseModeOption.PurchaseMode.SelectedId = CustomerSession.SelectedPurchaseModeList;
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

            return options;
        }

        public void SetOptions(string idWebSession, UserFilter userFilter)
        {
            CustomerSession = (WebSession)WebSession.Load(idWebSession);
            CurrentModule = WebNavigation.ModulesList.GetModule(CustomerSession.CurrentModule);
            
            #region GenericDetailLevelFilter
            ArrayList levels = new ArrayList();

            ArrayList genericDetailLevelsSaved = GetGenericDetailLevelsSaved();
            foreach (GenericDetailLevelSaved currentGenericLevel in genericDetailLevelsSaved)
            {
                if (CanAddDetailLevel(currentGenericLevel, CustomerSession.CurrentModule) && currentGenericLevel.GetNbLevels <= NbDetailLevelItemList)
                {
                    GenericDetailLevelsSaved.Add(currentGenericLevel.Id, currentGenericLevel);
                }
            }

            if (userFilter.GenericDetailLevelFilter.DefaultDetailValue >= 0)
            {
                CustomerGenericDetailLevel = (GenericDetailLevel)GetDefaultDetailLevels()[userFilter.GenericDetailLevelFilter.DefaultDetailValue];
            }
            if (userFilter.GenericDetailLevelFilter.CustomDetailValue >= 0)
            {
                CustomerGenericDetailLevel = (GenericDetailLevel)GenericDetailLevelsSaved[(Int64)userFilter.GenericDetailLevelFilter.CustomDetailValue];
            }
            if (NbDetailLevelItemList >= 1 && userFilter.GenericDetailLevelFilter.L1DetailValue >= 0)
            {
                levels.Add(userFilter.GenericDetailLevelFilter.L1DetailValue);
            }
            if (NbDetailLevelItemList >= 2 && userFilter.GenericDetailLevelFilter.L2DetailValue >= 0)
            {
                levels.Add(userFilter.GenericDetailLevelFilter.L2DetailValue);
            }
            if (NbDetailLevelItemList >= 3 && userFilter.GenericDetailLevelFilter.L3DetailValue >= 0)
            {
                levels.Add(userFilter.GenericDetailLevelFilter.L3DetailValue);
            }
            if (NbDetailLevelItemList >= 4 && userFilter.GenericDetailLevelFilter.L4DetailValue >= 0)
            {
                levels.Add(userFilter.GenericDetailLevelFilter.L4DetailValue);
            }
            if (levels.Count > 0)
            {
                CustomerGenericDetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
            }

            switch (ComponentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    CustomerSession.GenericMediaDetailLevel = CustomerGenericDetailLevel;
                    break;
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    CustomerSession.GenericProductDetailLevel = CustomerGenericDetailLevel;
                    break;
            }
            #endregion

            #region PeriodDetailFilter
            CustomerSession.DetailPeriod = (ConstantesPeriod.DisplayLevel)userFilter.PeriodDetailFilter.PeriodDetailType;
            #endregion

            #region UnitFilter
            CustomerSession.Unit = (ConstantesSession.Unit)userFilter.UnitFilter.Unit;
            #endregion

            #region Options by Vehicle
            string vehicleListId = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, Right.type.vehicleAccess);
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
                CustomerSession.Insert = (ConstantesSession.Insert)userFilter.InsertionFilter.Insertion;
            #endregion

            #region AutoPromoFilter
            if (autopromoEvaliantOption)
                CustomerSession.AutoPromo = (ConstantesSession.AutoPromo)userFilter.AutoPromoFilter.AutoPromo;
            #endregion

            #region FormatFilter
            if (autopromoEvaliantOption)
                CustomerSession.SelectedBannersFormatList = userFilter.FormatFilter.Formats;
            #endregion

            #region PurchaseModeFilter
            if (autopromoEvaliantOption)
                CustomerSession.SelectedPurchaseModeList = userFilter.PurchaseModeFilter.PurchaseModes;
            #endregion

            CustomerSession.Save();
        }

        #region Generic Detail Level Option Methodes
        private bool CanAddDetailLevel(GenericDetailLevel currentDetailLevel, Int64 module)
        {
            ArrayList AllowedDetailLevelItems = GetAllowedDetailLevelItems();

            TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
            if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            object[] param = new object[2];
            param[0] = CustomerSession;
            param[1] = ComponentProfile;
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

            switch (ComponentProfile)
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
            switch (ComponentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    return (CurrentModule.AllowedMediaDetailLevelItems);
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    return (CurrentModule.AllowedProductDetailLevelItems);
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
            string listStr = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (!string.IsNullOrEmpty(listStr))
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)CustomerSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return vehicleList;
        }

        private ArrayList GetDefaultDetailLevels()
        {
            switch (ComponentProfile)
            {
                case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                    return (CurrentModule.DefaultMediaDetailLevels);
                case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                    return (CurrentModule.DefaultProductDetailLevels);
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
                ds = GenericDetailLevelDataAccess.Load(CustomerSession);
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
            param[0] = CustomerSession;
            param[1] = ComponentProfile;
            WebCore.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (WebCore.Utilities.MediaDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            foreach (DetailLevelItemInformation currentDetailLevelItem in AllowedDetailLevelItems)
            {
                if (mediaDetailLevelUtilities.CanAddDetailLevelItem(currentDetailLevelItem, CustomerSession.CurrentModule))
                {
                    selectControl.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, CustomerSession.SiteLanguage), Value = currentDetailLevelItem.Id.GetHashCode().ToString() });
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
    }
}

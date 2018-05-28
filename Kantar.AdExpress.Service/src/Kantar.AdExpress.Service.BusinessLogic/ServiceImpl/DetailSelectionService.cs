﻿using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using Kantar.AdExpress.Service.Core.Domain.DetailSelectionDomain;
using KM.AdExpressI.MyAdExpress.DAL;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using TNS.AdExpress.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using LS = TNS.Ares.Domain.LS;
using TNS.Ares.Alerts.DAL;
using TNSDomain = TNS.Alert.Domain;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using NLog;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Constantes.Customer;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class DetailSelectionService : IDetailSelectionService

    {
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public DetailSelectionResponse GetDetailSelection(string idWebSession)
        {
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var result = LoadDetailsSelection(_webSession);
            return result;
        }
        public DetailSelectionResponse LoadSessionDetails(string idSession, string idWebSession)
        {
            Int64 idMySession = 0;
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(idSession) && !String.IsNullOrWhiteSpace(idWebSession))
            {
                idMySession = Int64.Parse(idSession);
                var webSession = (WebSession)WebSession.Load(idWebSession);
                var webSessionSave = (WebSession)MyResultsDAL.GetResultMySession(idMySession.ToString(), webSession);
                webSessionSave.CustomerLogin = webSession.CustomerLogin;
                webSessionSave.Source= webSession.Source;
                result = LoadDetailsSelection(webSessionSave);
            }
            return result;
        }

        public DetailSelectionResponse LoadUniversDetails(string idUnivers, string idWebSession)
        {
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(idUnivers) && !String.IsNullOrWhiteSpace(idWebSession))
            {
                long id = Int64.Parse(idUnivers);
                var webSession = (WebSession)WebSession.Load(idWebSession);
                Dictionary<int, AdExpressUniverse> adExpressUniverse = null;
                if (UniversListDataAccess.IsUniverseBelongToClientDescription(webSession, id, WebConstantes.LoadableUnivers.GENERIC_UNIVERSE))
                {
                    adExpressUniverse = (Dictionary<int, AdExpressUniverse>)UniversListDataAccess.GetObjectUniverses(id, webSession);
                    if (adExpressUniverse != null && adExpressUniverse.Count > 0)
                        result = GetUniversDetails(webSession, adExpressUniverse);
                }
            }

        
            return result;
        }

       

        public DetailSelectionResponse LoadAlertDetails(string id, string idWebSession)
        {
            DetailSelectionResponse result = new DetailSelectionResponse();
            if (!String.IsNullOrEmpty(id) && !String.IsNullOrWhiteSpace(idWebSession) && TNS.Alert.Domain.AlertConfiguration.IsActivated)
            {
                int idAlert = Int32.Parse(id);
                TNS.Ares.Domain.Layers.DataAccessLayer layer = LS.PluginConfiguration.GetDataAccessLayer(LS.PluginDataAccessLayerName.Alert);
                TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                IAlertDAL alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
                TNSDomain.Alert alert = alertDAL.GetAlert(idAlert);
                var webSessionSave = (WebSession)alert.Session;
                var webSession = (WebSession)WebSession.Load(idWebSession);
                webSessionSave.CustomerLogin = webSession.CustomerLogin;
                webSessionSave.Source = webSession.Source;
                result = LoadDetailsSelection(webSessionSave);
            }
            return result;
        }

        private DetailSelectionResponse LoadDetailsSelection(WebSession _webSession, bool showUnity = true, bool showStudyType = true)
        {
            var domain = new DetailSelectionResponse
            {
                ShowUnity = showUnity,
                ShowStudyType = showStudyType
            };

            domain.SiteLanguage = _webSession.SiteLanguage;

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _webSession.Source;
            param[1] = _webSession.SiteLanguage;

            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            //TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems = null;

            #region Choix de l'étude :
            domain.ModuleId = _webSession.CurrentModule;
            domain.ModuleLabel = GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule), _webSession.SiteLanguage);
            #endregion

            #region Niveau détaillés par :
            ArrayList detailSelections = null;
            TNS.AdExpress.Domain.Web.Navigation.Module currentModule = _webSession.CustomerLogin.GetModule(_webSession.CurrentModule);
            if (currentModule.Id != WebConstantes.Module.Name.HEALTH)
            {
                try
                {
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)_webSession.CurrentTab)).DetailSelectionItemsType;
                }
                catch (System.Exception)
                {
                    if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                        detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
                }
                foreach (int currentType in detailSelections)
                {
                    switch ((WebConstantes.DetailSelection.Type)currentType)
                    {
                        case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
                            domain.NiveauDetailLabel = _webSession.GenericMediaDetailLevel.GetLabel(_webSession.SiteLanguage);
                            break;
                        case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
                            domain.NiveauDetailLabel = _webSession.GenericProductDetailLevel.GetLabel(_webSession.SiteLanguage);
                            break;
                        case WebConstantes.DetailSelection.Type.genericColumnLevelDetail:
                            domain.GenericLevelDetailColumn = _webSession.GenericColumnDetailLevel.GetLabel(_webSession.SiteLanguage);
                            break;
                        default:
                            break;
                    }
                }
            }
            #endregion

            #region Niveaux de détail colonne générique
            //WebFunctions.MediaDetailLevel.GetGenericLevelDetailColumn(_webSession, ref displayGenericlevelDetailColumnLabel, genericlevelDetailColumnLabel, false);
            #endregion

            #region Unité :
            domain.UniteLabel = GestionWeb.GetWebWord(_webSession.GetSelectedUnit().WebTextId, _webSession.SiteLanguage);
            #endregion

            #region Slogan :
            if (_webSession.IdSlogans != null && _webSession.IdSlogans.Count > 0)
            {
                foreach (Int64 currentSlogan in _webSession.IdSlogans)
                {
                    domain.IdSlogansLabel += currentSlogan + ", ";
                }
                domain.IdSlogansLabel = domain.IdSlogansLabel.Remove(domain.IdSlogansLabel.Length - 2);
            }
            #endregion

            #region Univers Market :
            domain.UniversMarket = new List<Core.Domain.Tree>();
            ExtractTreeFromAdExpressUniverse(_webSession.PrincipalProductUniverses, domain.UniversMarket, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);
            #endregion

            #region Media : 
            if (currentModule.Id == WebConstantes.Module.Name.HEALTH)
                domain.MediasSelected = GetHealthLabels(_webSession.SelectionUniversMedia, _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
            else
                domain.MediasSelected = GetLabels(_webSession.SelectionUniversMedia, _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
            domain.UniversMedia = new List<Core.Domain.Tree>();
            domain.MediasSelectedLabel = string.Join(",", domain.MediasSelected.Select(_ => _.Label));

            if (currentModule.Id == WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
            {
                domain.SponsorshipMediasSelected = GetLabels(_webSession.CurrentUniversMedia, _webSession.SiteLanguage, _webSession.DataLanguage, _webSession.CustomerDataFilters.DataSource);
                domain.SponsorshipMediasSelectedLabel = string.Join(",", domain.SponsorshipMediasSelected.Select(_ => _.Label));
                domain.ShowSponsorshipMedia = true;
            }

            if (domain.MediasSelected[0].Id != 50) //Plurimedia
            {
                ExtractTreeFromAdExpressUniverse(_webSession.PrincipalMediaUniverses, domain.UniversMedia, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);
            }
            #endregion

            #region Période sélectionnée :
            switch (_webSession.CurrentModule)
            {
                case WebConstantes.Module.Name.HEALTH:
                    if (!string.IsNullOrEmpty(_webSession.PeriodBeginningDate))
                        domain.DateBegin = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, true);
                    if (!string.IsNullOrEmpty(_webSession.PeriodEndDate))
                        domain.DateEnd = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, false);
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    domain.DateBegin = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, true);
                    domain.DateEnd = Dates.GetAnalysisDate(_webSession.PeriodBeginningDate, false);
                    break;
                default:                  
                    DateTime? dateBegin = Dates.GetPeriodBeginningDate(_webSession.PeriodBeginningDate, _webSession.PeriodType);
                    DateTime? dateEnd = Dates.GetPeriodEndDate(_webSession.PeriodEndDate, _webSession.PeriodType);
                    domain.DateBegin = !string.IsNullOrEmpty(_webSession.PeriodBeginningDate) ? dateBegin : null;
                    domain.DateEnd = !string.IsNullOrEmpty(_webSession.PeriodEndDate) ? dateEnd : null;
                    break;

            }

            if (Dates.isPeriodSet(domain.DateBegin, domain.DateEnd))
            {
                domain.Dates = Dates.GetPeriodDetail(_webSession);
            }
            #endregion

            #region StudyPeriod: 
            domain.StudyPeriod = ManageStudyPeriod(_webSession);
            #endregion

            #region ComparativePeriod:
            domain.ComparativePeriod = ManageComparativePeriod(_webSession);
            #endregion

            #region ComparativePeriodType:
            domain.ComparativePeriodType = ManageComparativePeriodType(_webSession);
            #endregion

            #region PeriodDisponibilityType:
            domain.PeriodDisponibilityType = ManagePeriodDisponibilityType(_webSession);
            #endregion

            #region defined
            domain.ShowDate = Dates.isPeriodSet(domain.DateBegin, domain.DateEnd);
            domain.ShowUnivers = !(domain.MediasSelected.Count == 0);
            domain.ShowUniversDetails = (!(domain.MediasSelected.Count == 0) && !(domain.UniversMedia.Count == 0));
            domain.ShowMarket = (domain.UniversMarket.Count > 0);
            domain.ShowGenericlevelDetail = !string.IsNullOrEmpty(domain.NiveauDetailLabel);
            domain.ShowGenericLevelDetailColumn = !string.IsNullOrEmpty(domain.GenericLevelDetailColumn);
            domain.ShowStudyPeriod = !string.IsNullOrEmpty(domain.StudyPeriod);
            domain.ShowComparativePeriod = !string.IsNullOrEmpty(domain.ComparativePeriod);
            domain.ShowComparativePeriodType = !string.IsNullOrEmpty(domain.ComparativePeriodType);
            domain.ShowPeriodDisponibilityType = !string.IsNullOrEmpty(domain.PeriodDisponibilityType);
            domain.ShowIdSlogansLabel = !string.IsNullOrEmpty(domain.IdSlogansLabel);
            #endregion

            return domain;
        }

        private static void ExtractTreeFromAdExpressUniverse(Dictionary<int, AdExpressUniverse> Principal, List<Tree> treeDefined, ClassificationLevelListDALFactory factoryLevels, int SiteLanguage, long currentModule, bool defaultFcbUniverse = false)
        {
            List<long> itemIdList = null;
            int nbUniverse = 0;

            foreach (KeyValuePair<int, AdExpressUniverse> kvp in Principal)
            {
                
                foreach (KeyValuePair<long, NomenclatureElementsGroup> kvpElements in kvp.Value.ElementsGroupDictionary)
                {

                    Core.Domain.Tree model = new Core.Domain.Tree();

                    List<long> levelIdsList = kvpElements.Value.GetLevelIdsList();
                    model.AccessType = kvpElements.Value.AccessType == AccessType.excludes ? AccessType.excludes : AccessType.includes;
                    if (model.AccessType == AccessType.excludes)
                    {
                        model.Label = GestionWeb.GetWebWord(2269, SiteLanguage); // 2269 = Exclure
                        model.LabelId = 2269;
                    }
                    else
                    {
                        model.Label = GestionWeb.GetWebWord(2270, SiteLanguage); 
                        model.LabelId = 2270;
                        if (Principal.Count() > 1)
                        {
                            model.Label = (nbUniverse == 0) ? GestionWeb.GetWebWord(3039, SiteLanguage) : GestionWeb.GetWebWord(2869, SiteLanguage); //3039 = Référents; 2869 = Concurrents
                            model.LabelId = (nbUniverse == 0) ? 3039 : 2869;
                        }                      
                    }
                    #region populate Universlevels
                    model.UniversLevels = new List<Core.Domain.UniversLevel>();
                    if (levelIdsList != null)
                    {
                        for (int j = 0; j < levelIdsList.Count; j++)
                        {
                            var universLevel = new UniversLevel();
                            universLevel.LabelId = UniverseLevels.Get(levelIdsList[j]).LabelId;
                            universLevel.UniversItems = new List<UniversItem>();
                            universLevel.Label = GestionWeb.GetWebWord(UniverseLevels.Get(levelIdsList[j]).LabelId, SiteLanguage);
                            universLevel.Id = levelIdsList[j];
                            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDAL universeItems =
                                factoryLevels.CreateDefaultClassificationLevelListDAL(UniverseLevels.Get(levelIdsList[j]), kvpElements.Value.GetAsString(levelIdsList[j]));
                            if (universeItems != null)
                            {
                                itemIdList = universeItems.IdListOrderByClassificationItem;
                                if (itemIdList != null && itemIdList.Count > 0)
                                {
                                    for (int z = 0; z < itemIdList.Count; z++)
                                    {
                                        var universItem = new UniversItem();
                                        universItem.Label = universeItems[itemIdList[z]];
                                        universLevel.UniversItems.Add(universItem);

                                        universItem.Id = itemIdList[z];
                                        universItem.IdLevelUniverse = levelIdsList[j];
                                    }
                                }
                            }
                            model.UniversLevels.Add(universLevel);
                        }
                    }
                    #endregion
                    treeDefined.Add(model);
                }
                //}
                nbUniverse++;
            }

        }

        private List<TextData> GetLabels(System.Windows.Forms.TreeNode elem, int idLanguage, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {
            var levelInformations = elem.Nodes.Cast<System.Windows.Forms.TreeNode>().Select(_ => _.Tag).Cast<LevelInformation>().ToList();
            if (levelInformations.Count == 0)
                return new List<TextData>();

            var searchTodo = levelInformations.GroupBy(_ => _.Type, _ => _.ID).ToDictionary(e => e.Key, e => e.Distinct().ToList());
            var key = searchTodo.Keys.SingleOrDefault();

            var cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            object[] param = new object[2];
            param[0] = source;
            param[1] = dataLanguage;
            var factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            string pluriMediaLabel = IsPlurimedia(key, searchTodo, dataLanguage);


            if (!string.IsNullOrEmpty(pluriMediaLabel))
            {                     
                return new List<TextData> { new TextData { Id = searchTodo[key].FirstOrDefault(), Label = pluriMediaLabel } };
            }
            var dal = factoryLevels.CreateClassificationLevelListDAL(key, string.Join(",", searchTodo[key]));

            return searchTodo[key].Select(_ => new TextData { Id = _, Label = dal[_] }).ToList();
        }

        private List<TextData> GetHealthLabels(System.Windows.Forms.TreeNode elem, int idLanguage, int dataLanguage, TNS.FrameWork.DB.Common.IDataSource source)
        {
            var levelInformations = elem.Nodes.Cast<System.Windows.Forms.TreeNode>().Select(_ => _.Tag).Cast<LevelInformation>().ToList();
            if (levelInformations.Count == 0)
                return new List<TextData>();

            var searchTodo = levelInformations.GroupBy(_ => _.Type, _ => _.ID).ToDictionary(e => e.Key, e => e.Distinct().ToList());
            var key = searchTodo.Keys.SingleOrDefault();

            var cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            object[] param = new object[2];
            param[0] = source;
            param[1] = dataLanguage;
            var factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            var dal = factoryLevels.CreateClassificationLevelListDAL(DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.canal), string.Join(",", searchTodo[key]), WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.khealth01).Label);

            return searchTodo[key].Select(_ => new TextData { Id = _, Label = dal[_] }).ToList();
        }

        private string ManageStudyPeriod(WebSession webSession)
        {
            string StudyPeriod = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    StudyPeriod = Dates.GetStudyPeriodDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                )
            {
                if (webSession.isStudyPeriodSelected())
                    StudyPeriod = Dates.GetStudyPeriodDetail(webSession, webSession.CurrentModule);
            }
            return StudyPeriod;
        }
        private string ManageComparativePeriod(WebSession webSession)
        {
            string ComparativePeriod = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    ComparativePeriod = Dates.GetComparativePeriodDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isPeriodComparative())
                    ComparativePeriod = Dates.GetComparativePeriodDetail(webSession, webSession.CurrentModule);
            }
            return ComparativePeriod;
        }

        private string ManageComparativePeriodType(WebSession webSession)
        {
            string ComparativePeriodType = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                if (webSession.ComparativeStudy && WebApplicationParameters.UseComparativeMediaSchedule)
                    ComparativePeriodType = Dates.GetComparativePeriodTypeDetail(webSession, webSession.CurrentModule);
            }
            else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isComparativePeriodTypeSelected())
                    ComparativePeriodType = Dates.GetComparativePeriodTypeDetail(webSession, webSession.CurrentModule);
            }
            return ComparativePeriodType;
        }

        private string ManagePeriodDisponibilityType(WebSession webSession)
        {
            string PeriodDisponibilityType = string.Empty;
            if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
            {
                if (webSession.isPeriodDisponibilityTypeSelected())
                {
                    PeriodDisponibilityType = Dates.GetPeriodDisponibilityTypeDetail(webSession);
                }
            }
            return PeriodDisponibilityType;
        }
        private DetailSelectionResponse GetUniversDetails(WebSession webSession, Dictionary<int, AdExpressUniverse> adExpressUniverse)
        {
            var result = new DetailSelectionResponse();
            result.SiteLanguage = webSession.SiteLanguage;
            CoreLayer cl = WebApplicationParameters.CoreLayers[WebConstantes.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = webSession.Source;
            param[1] = webSession.SiteLanguage;
            ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            result.ModuleLabel = adExpressUniverse.FirstOrDefault().Value.UniverseDimension.ToString();
            switch (adExpressUniverse.FirstOrDefault().Value.UniverseDimension)
            {
                case Dimension.product:
                    result.UniversMarket = new List<Tree>();
                    result.ShowMarket = true;
                    result.ShowUniversDetails = true;
                    result.ModuleLabel = GestionWeb.GetWebWord(468, webSession.SiteLanguage);
                    ExtractTreeFromAdExpressUniverse(adExpressUniverse, result.UniversMarket, factoryLevels, webSession.SiteLanguage, webSession.CurrentModule);
                    break;
                case Dimension.media:
                    //TODO //result.MediasSelected = GetLabels(webSession.SelectionUniversMedia, webSession.SiteLanguage, webSession.DataLanguage, webSession.CustomerDataFilters.DataSource);
                    //TODO //result.MediasSelectedLabel = string.Join(",", result.MediasSelected.Select(p => p.Label));
                    result.UniversMedia = new List<Tree>();
                    result.ShowUnivers = true;
                    result.ShowUniversDetails = true;
                    result.ModuleLabel = GestionWeb.GetWebWord(363, webSession.SiteLanguage);
                    ExtractTreeFromAdExpressUniverse(adExpressUniverse, result.UniversMedia, factoryLevels, webSession.SiteLanguage, webSession.CurrentModule);
                    break;
                default:
                    return result;
            }
            return result;
        }

        public List<Tree> GetMarket(string idWebSession)
        {
            var _webSession = (WebSession)WebSession.Load(idWebSession);

            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = _webSession.Source;
            param[1] = _webSession.SiteLanguage;

            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            var UniversMarket = new List<Core.Domain.Tree>();

            ExtractTreeFromAdExpressUniverse(_webSession.PrincipalProductUniverses, UniversMarket, factoryLevels, _webSession.SiteLanguage, _webSession.CurrentModule);

            return UniversMarket;
        }

        private static string GetSchema(long moduleId)
        {
            switch (moduleId)
            {
                case WebConstantes.Module.Name.HEALTH:
                    return WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.khealth01).Label;
                default:
                    return WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
            }
        }

        private string IsPlurimedia(Right.type key,Dictionary<Right.type,List<long>> searchTodo,int dataLanguage)
        {
            if (key == Right.type.vehicleAccess)
            {
               if (VehiclesInformation.Contains(Vehicles.names.plurimedia)
                && VehiclesInformation.EnumToDatabaseId(Vehicles.names.plurimedia) == searchTodo[key].FirstOrDefault()               
                ) return GestionWeb.GetWebWord(210, dataLanguage);
                 if(VehiclesInformation.Contains(Vehicles.names.plurimediaOffline)
                && VehiclesInformation.EnumToDatabaseId(Vehicles.names.plurimediaOffline) == searchTodo[key].FirstOrDefault())
                    return GestionWeb.GetWebWord(3111, dataLanguage);
                 if (VehiclesInformation.Contains(Vehicles.names.plurimediaOnline)
              && VehiclesInformation.EnumToDatabaseId(Vehicles.names.plurimediaOnline) == searchTodo[key].FirstOrDefault())
                    return GestionWeb.GetWebWord(3112, dataLanguage);
                 if (VehiclesInformation.Contains(Vehicles.names.plurimediaWithSearch)
              && VehiclesInformation.EnumToDatabaseId(Vehicles.names.plurimediaWithSearch) == searchTodo[key].FirstOrDefault())
                    return GestionWeb.GetWebWord(3113, dataLanguage);
            }

            return string.Empty;
        }
    }
}

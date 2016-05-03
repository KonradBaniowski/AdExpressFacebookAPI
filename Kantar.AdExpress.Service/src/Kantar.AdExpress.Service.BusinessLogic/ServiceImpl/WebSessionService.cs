﻿using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using CstWeb = TNS.AdExpress.Constantes.Web;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Units;
using System.Reflection;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes.DB;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Classification;
using TNS.Classification.Universe;
using System.Linq;
using FrameWorkSelection = TNS.AdExpress.Constantes.FrameWork.Selection;
using TNS.AdExpress.Domain;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class WebSessionService : IWebSessionService
    {
        private WebSession _webSession = null;
        private const int _nbMaxItemByLevel = 1000;
        private const int MediaRequiredCode = 1052;
        private const int MaxItemsPerLevel = 100;
        public WebSessionResponse SaveMediaSelection(List<long> mediaIds, string webSessionId, List<Tree> trees, Dimension dimension, Security security, bool mediaSupportRequired)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                MediaScheduleStep = MediaScheduleStep.Media
            };
            bool success = false;
            try
            {
                List<long> commun = Array.ConvertAll(Lists.GetIdList(CstWeb.GroupList.ID.media, CstWeb.GroupList.Type.mediaInSelectAll).Split(','), Convert.ToInt64).ToList();
                bool isAllCommun = mediaIds.All(e => commun.Contains(e));
                var _webSession = (WebSession)WebSession.Load(webSessionId);
                if (!isAllCommun && mediaIds.Count() > 1)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(CstWeb.LanguageConstantes.UnityError, _webSession.SiteLanguage);
                }
                else
                {
                    if (mediaSupportRequired && !trees.Any())
                    {
                        response.ErrorMessage = GestionWeb.GetWebWord(CstWeb.LanguageConstantes.MediaRequiredCode, _webSession.SiteLanguage);
                    }
                    else
                    {
                        #region Save Media Selection in WebSession
                        WebNavigation.Module _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);
                        _webSession.Insert = CstWeb.CustomerSessions.Insert.total;
                        List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                        System.Windows.Forms.TreeNode tmpNode;
                        bool containsSearch = false;
                        bool containsSocial = false;
                        foreach (var item in mediaIds)
                        {
                            tmpNode = new System.Windows.Forms.TreeNode(item.ToString());
                            tmpNode.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, item, item.ToString());
                            tmpNode.Checked = true;
                            levelsSelected.Add(tmpNode);
                            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.search)
                                && item.ToString() == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.search).DatabaseId.ToString())
                                containsSearch = true;
                            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.social)
                                && item.ToString() == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.social).DatabaseId.ToString())
                                containsSocial = true;

                        }
                        if (levelsSelected.Count == 0)
                        {
                            response.ErrorMessage = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage);
                        }
                        else if (containsSearch && levelsSelected.Count > 1)
                        {
                            response.ErrorMessage = GestionWeb.GetWebWord(3011, _webSession.SiteLanguage);
                        }
                        else if (containsSocial && levelsSelected.Count > 1)
                        {
                            response.ErrorMessage = GestionWeb.GetWebWord(3030, _webSession.SiteLanguage);
                        }
                        else
                        {

                            //Reinitialize banners selection if change vehicle
                            Dictionary<Int64, VehicleInformation> vehicleInformationList = _webSession.GetVehiclesSelected();
                            if (mediaIds.Count != vehicleInformationList.Count)
                            {
                                foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                                {
                                    if (!vehicleInformationList.ContainsKey(((LevelInformation)node.Tag).ID))
                                    {
                                        _webSession.SelectedBannersFormatList = string.Empty;
                                        break;
                                    }
                                }
                            }
                            else
                            {
                                _webSession.SelectedBannersFormatList = string.Empty;
                            }

                            // Sauvegarde de la sélection dans la session
                            //Si la sélection comporte des éléments, on la vide
                            _webSession.SelectionUniversMedia.Nodes.Clear();

                            foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                            {
                                _webSession.SelectionUniversMedia.Nodes.Add(node);
                                // Tracking
                                _webSession.OnSetVehicle(((LevelInformation)node.Tag).ID);
                            }

                            //verification que l unite deja sélectionnée convient pour tous les medias
                            var vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstWebCustomer.Right.type.vehicleAccess);

                            List<CstWeb.CustomerSessions.Unit> unitList = FctUtilities.Units.getUnitsFromVehicleSelection(vehicleSelection);
                            unitList = GetAllowedUnits(unitList, _currentModule.AllowedUnitEnumList);
                            if (unitList.Count == 0)
                            {
                                response.ErrorMessage = GestionWeb.GetWebWord(2541, _webSession.SiteLanguage);

                            }
                            else
                            {
                                success = true;
                            }
                        }
                    }
                    #endregion

                    #region Save Media support if any
                    if (trees.Any())
                    {
                        Dictionary<int, AdExpressUniverse> universDictionary = new Dictionary<int, AdExpressUniverse>();
                        long idModule = _webSession.CurrentModule;
                        if (idModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA || idModule == CstWeb.Module.Name.ANALYSE_PORTEFEUILLE || idModule == CstWeb.Module.Name.ANALYSE_DYNAMIQUE)
                        {
                            AdExpressUnivers univers = GetUnivers(trees, _webSession, dimension, security);
                            if (univers.AdExpressUniverse != null && univers.AdExpressUniverse.Count() > 0)
                            {
                                bool mustSelectIncludeItems = MustSelectIncludeItems(_webSession);
                                List<NomenclatureElementsGroup> nGroups = univers.AdExpressUniverse.GetIncludes();
                                if ((mustSelectIncludeItems && nGroups != null && nGroups.Count > 0) || !mustSelectIncludeItems)
                                {
                                    universDictionary.Add(universDictionary.Count, univers.AdExpressUniverse);
                                    _webSession.PrincipalMediaUniverses = universDictionary;
                                    success = true;
                                }
                                else
                                {
                                    response.ErrorMessage = GestionWeb.GetWebWord(2299, _webSession.SiteLanguage);
                                }
                            }
                            else
                            {
                                response.ErrorMessage = GestionWeb.GetWebWord(878, _webSession.SiteLanguage);
                            }
                        }
                        else if (idModule == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE)
                        {
                            Dictionary<int, AdExpressUniverse> universes = GetConcurrentUniverses(trees, _webSession, dimension, security);
                            _webSession.PrincipalMediaUniverses = universes;
                            success = true;
                        }
                        //AdExpressUniverse univers = GetUnivers(trees, _webSession, dimension, security);
                        //if (univers != null && univers.Count() > 0)
                        //{
                        //    bool mustSelectIncludeItems = MustSelectIncludeItems(_webSession);
                        //    List<NomenclatureElementsGroup> nGroups = univers.GetIncludes();
                        //    if ((mustSelectIncludeItems && nGroups != null && nGroups.Count > 0) || !mustSelectIncludeItems)
                        //    {                                
                        //        universDictionary.Add(universDictionary.Count, univers);
                        //        _webSession.PrincipalMediaUniverses = universDictionary;
                        //        success = true;
                        //    }
                        //    else
                        //    {
                        //        response.ErrorMessage = GestionWeb.GetWebWord(2299, _webSession.SiteLanguage);
                        //    }
                        //}
                        //else
                        //{
                        //    response.ErrorMessage = GestionWeb.GetWebWord(878, _webSession.SiteLanguage);
                        //}
                    }
                    #endregion

                    #region Save WebSession
                    if (success)
                    {
                        _webSession.Save();
                        _webSession.Source.Close();
                        response.Success = success;
                    }
                    #endregion
                }
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    //this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));                    
                }
            }

            return response;
        }

        public WebSessionResponse SaveMarketSelection(string webSessionId, List<Tree> trees, Dimension dimension, Security security, bool required)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                MediaScheduleStep = MediaScheduleStep.Market
            };
            var _webSession = (WebSession)WebSession.Load(webSessionId);
            bool isValid = (required) ? (trees.Any() && trees.Where(p => p.UniversLevels != null).Any() && trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).Any()) : true;

            if (isValid)
            {
                #region Fix max items per level
                if (trees.Where(p => p.UniversLevels.Where(x => x.UniversItems.Count > MaxItemsPerLevel).Any()).Any())
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                }
                #endregion
                #region try catch block
                else
                {
                    AdExpressUnivers univers = GetUnivers(trees, _webSession, dimension, security);
                    if (univers.Success)
                    {
                        try
                        {
                            if (univers.AdExpressUniverse != null && univers.AdExpressUniverse.Count() > 0)
                            {
                                bool mustSelectIncludeItems = MustSelectIncludeItems(_webSession);
                                List<NomenclatureElementsGroup> nGroups = univers.AdExpressUniverse.GetIncludes();
                                if ((mustSelectIncludeItems && nGroups != null && nGroups.Count > 0) || !mustSelectIncludeItems)
                                {
                                    Dictionary<int, AdExpressUniverse>
                                            universDictionary = new Dictionary<int, AdExpressUniverse>();
                                    universDictionary.Add(universDictionary.Count, univers.AdExpressUniverse);

                                    if (!IsValidUniverseLevels(univers.AdExpressUniverse, _webSession))
                                    {
                                        response.ErrorMessage = GestionWeb.GetWebWord(2990, _webSession.SiteLanguage);
                                    }
                                    else
                                    {
                                        _webSession.PrincipalProductUniverses = universDictionary;
                                        response.Success = true;
                                        _webSession.Save();
                                        _webSession.Source.Close();
                                    }
                                }
                                else
                                {
                                    response.ErrorMessage = GestionWeb.GetWebWord(2299, _webSession.SiteLanguage);
                                }
                            }
                            else if (required)
                            {
                                response.ErrorMessage = GestionWeb.GetWebWord(878, _webSession.SiteLanguage);
                            }
                            else
                            {
                                response.Success = true;
                            }

                        }

                        catch (SecurityException)
                        {
                            _webSession.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
                            _webSession.Save();
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(2285, _webSession.SiteLanguage));
                        }
                        catch (CapacityException)
                        {
                            _webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                            _webSession.Save();
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(2286, _webSession.SiteLanguage));
                        }
                        catch (Exception)
                        {
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(922, _webSession.SiteLanguage));
                        }
                    }
                    else
                    {
                        response.ErrorMessage = String.Format(GestionWeb.GetWebWord(2285, _webSession.SiteLanguage));
                    }
                }
                #endregion
            }
            else
            {
                response.ErrorMessage = GestionWeb.GetWebWord(2299, _webSession.SiteLanguage);
            }

            return response;
        }

        public WebSessionResponse GetWebSessionLanguage(string webSessionId)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                MediaScheduleStep = MediaScheduleStep.Market
            };
            var _webSession = (WebSession)WebSession.Load(webSessionId);


            return response;
        }


        public void SaveCurrentModule(string webSessionId, int moduleId)
        {
            var _webSession = (WebSession)WebSession.Load(webSessionId);

            Int64 tmp = _webSession.CurrentModule = moduleId;
            if (_webSession.CurrentModule == CstWeb.Module.Name.INDICATEUR)
                _webSession.CurrentTab = TNS.AdExpress.Constantes.FrameWork.Results.SynthesisRecap.SYNTHESIS;//Règle : planche synthèse par défaut dans le module indicateurs
            else
            {
                if (_webSession.CurrentTab == 0) _webSession.OnSetResult();
                else
                    _webSession.CurrentTab = 0;
            }
            _webSession.ModuleTraductionCode = (int)WebNavigation.ModulesList.GetModuleWebTxt(tmp);

            _webSession.ReachedModule = false;
            _webSession.CurrentUniversMedia = new System.Windows.Forms.TreeNode("media");
            _webSession.CurrentUniversAdvertiser = new System.Windows.Forms.TreeNode("advertiser");
            _webSession.CurrentUniversProduct = new System.Windows.Forms.TreeNode("produit");

            _webSession.SelectionUniversAdvertiser = new System.Windows.Forms.TreeNode("advertiser");
            _webSession.SelectionUniversMedia = new System.Windows.Forms.TreeNode("media");
            _webSession.SelectionUniversProduct = new System.Windows.Forms.TreeNode("produit");

            _webSession.ReferenceUniversAdvertiser = new System.Windows.Forms.TreeNode("advertiser");
            _webSession.ReferenceUniversMedia = new System.Windows.Forms.TreeNode("media");
            _webSession.ReferenceUniversProduct = new System.Windows.Forms.TreeNode("produit");
            _webSession.SelectionUniversProgramType = new System.Windows.Forms.TreeNode("programtype");
            _webSession.SelectionUniversSponsorshipForm = new System.Windows.Forms.TreeNode("sponsorshipform");
            _webSession.CurrentUniversProgramType = new System.Windows.Forms.TreeNode("programtype");
            _webSession.CurrentUniversSponsorshipForm = new System.Windows.Forms.TreeNode("sponsorshipform");

            _webSession.CompetitorUniversAdvertiser = new Hashtable(5);
            //_webSession.CompetitorUniversAdvertiser.Add(0, new System.Windows.Forms.TreeNode("advertiser"));
            _webSession.CompetitorUniversMedia = new Hashtable(5);
            _webSession.CompetitorUniversProduct = new Hashtable(5);

            _webSession.TemporaryTreenode = null;
            _webSession.PeriodLength = 0;
            _webSession.PeriodBeginningDate = "";
            _webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
            _webSession.PeriodEndDate = "";
            _webSession.Graphics = true;

            _webSession.Unit = UnitsInformation.DefaultCurrency;

            TNS.AdExpress.Domain.Layers.CoreLayer clProductU = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.productDetailLevelUtilities];
            if (clProductU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
            TNS.AdExpress.Web.Core.Utilities.ProductDetailLevel productDetailLevelUtilities = (TNS.AdExpress.Web.Core.Utilities.ProductDetailLevel)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + clProductU.AssemblyName, clProductU.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
            _webSession.PreformatedProductDetail = productDetailLevelUtilities.GetDefaultPreformatedProductDetails(_webSession);

            _webSession.LastReachedResultUrl = "";
            _webSession.Percentage = false;
            _webSession.ProductDetailLevel = null;
            if (_webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE && _webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
            {

                TNS.AdExpress.Domain.Layers.CoreLayer clMediaU = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.mediaDetailLevelUtilities];
                if (clMediaU == null) throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
                TNS.AdExpress.Web.Core.Utilities.MediaDetailLevel mediaDetailLevelUtilities = (TNS.AdExpress.Web.Core.Utilities.MediaDetailLevel)
                    AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + clMediaU.AssemblyName, clMediaU.Class
                    , false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);
                _webSession.PreformatedMediaDetail = mediaDetailLevelUtilities.GetDefaultPreformatedMediaDetails(_webSession);

                #region Niveau de détail media (Generic)

                ArrayList levels = mediaDetailLevelUtilities.GetDefaultGenericDetailLevelIds(_webSession.CurrentModule);
                _webSession.GenericMediaDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                _webSession.GenericAdNetTrackDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion

                #region Niveau de détail produit (Generic)
                CstWeb.GenericDetailLevel.SelectedFrom selectedFrom;
                // Initialisation à annonceur
                levels.Clear();


                levels = productDetailLevelUtilities.GetDefaultGenericDetailLevelIds(_webSession.CurrentModule);
                switch (_webSession.CurrentModule)
                {
                    case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                        selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
                        break;
                    case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES:
                        selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels;
                        break;
                    default:
                        selectedFrom = TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels;
                        break;
                }
                _webSession.GenericProductDetailLevel = new GenericDetailLevel(levels, selectedFrom);
                #endregion

                #region Niveau de détail colonne (Generic)
                levels.Clear();
                levels.Add(3);
                _webSession.GenericColumnDetailLevel = new GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                #endregion
            }
            _webSession.MediaDetailLevel = null;
            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PRESSE
                || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_RADIO
                || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_TELEVISION
                || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_PAN_EURO
                || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DE_BORD_EVALIANT
            )
                _webSession.PreformatedTable = CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.vehicleInterestCenterMedia_X_Units;

            else if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES
                || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS
            )
                _webSession.PreformatedTable = CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.othersDimensions_X_Period;
            else
                _webSession.PreformatedTable = CstWeb.CustomerSessions.PreformatedDetails.PreformatedTables.media_X_Year;

            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS)
            {
                _webSession.SelectionUniversMedia.Nodes.Clear();
                System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("TELEVISION");
                tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.tv), "TELEVISION");
                _webSession.SelectionUniversMedia.Nodes.Add(tmpNode);
            }


            #region paramètres rajoutés pour Bilan de campagne
            //Setting vehicle as press for APPM
            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.BILAN_CAMPAGNE)
            {
                System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode();
                tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, DBConstantes.Vehicles.names.press.GetHashCode(), GestionWeb.GetWebWord(1298, _webSession.SiteLanguage));
                tmpNode.Checked = true;
                _webSession.SelectionUniversMedia.Nodes.Add(tmpNode);
            }
            #endregion

            #region paramètres rajoutés pour tableaux de bord
            _webSession.DetailPeriodBeginningDate = "";
            _webSession.DetailPeriodEndDate = "";
            _webSession.Format = CstWeb.Repartition.Format.Total;
            _webSession.TimeInterval = CstWeb.Repartition.timeInterval.Total;
            _webSession.NamedDay = CstWeb.Repartition.namedDay.Total;
            #endregion

            //Rajouté le 17/08/05 par D.V. Mussuma
            _webSession.CurrentUniversAEPMWave = new System.Windows.Forms.TreeNode("wave");
            _webSession.CurrentUniversAEPMTarget = new System.Windows.Forms.TreeNode("target");
            _webSession.CurrentUniversOJDWave = new System.Windows.Forms.TreeNode("ojdWave");
            _webSession.SelectionUniversAEPMWave = new System.Windows.Forms.TreeNode("wave");
            _webSession.SelectionUniversAEPMTarget = new System.Windows.Forms.TreeNode("target");
            _webSession.SelectionUniversOJDWave = new System.Windows.Forms.TreeNode("ojdWave");

            #region Paramètres pour les accroches - G Ragneau - 23/12/2005
            _webSession.IdSlogans = new ArrayList();
            _webSession.SloganColors = new Hashtable();
            _webSession.SloganIdZoom = long.MinValue;
            #endregion

            //Rajouté le 14/12/05 par D.V. Mussuma
            _webSession.PercentageAlignment = CstWeb.Percentage.Alignment.none;

            //Initialisaiton des tris des tableaux génériques (G.Ragneau - 12/10/2007)
            _webSession.Sort = 0;
            _webSession.SortKey = string.Empty;

            _webSession.SelectedBannersFormatList = string.Empty;
            if (WebApplicationParameters.UsePurchaseMode && _webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_PURCHASE_MODE_DISPLAY_FLAG))
            {
                var purchaseModeList = new List<FilterItem>(PurchaseModeList.GetList().Values);
                _webSession.SelectedPurchaseModeList = string.Join(",", purchaseModeList.FindAll(p => p.IsEnable).ConvertAll(p => p.Id.ToString()).ToArray());
            }

            _webSession.ComparativeStudy = false;
            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES)
            {
                _webSession.Evolution = false;
            }

            _webSession.IsSelectRetailerDisplay = false;

            //Défintion des medias et  périodes par défaut pour les modules d'Analyses Sectorielles
            //TODO 
            //if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
            //    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
            //{

            //    SetRecapDefaultMediaSelection();
            //    SetRecapDefaultPeriodSelection();
            //}
            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.VP)
                _webSession.SetDates(WebApplicationParameters.VpDateConfigurations.DateTypeDefault);

            if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ROLEX)
                _webSession.SetDates(WebApplicationParameters.RolexDateConfigurations.DateTypeDefault);


            //Nouveaux univers produit
            _webSession.PrincipalProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.SecondaryProductUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            //Nouveaux univers media
            _webSession.PrincipalMediaUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.SecondaryMediaUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            //New advertising agency univers
            _webSession.PrincipalAdvertisingAgnecyUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.SecondaryAdvertisingAgnecyUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            //Profession universes
            _webSession.PrincipalProfessionUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

            if (WebApplicationParameters.VpConfigurationDetail != null)
                _webSession.PersonnalizedLevel = WebApplicationParameters.VpConfigurationDetail.DefaultPersoLevel;
            else
                _webSession.PersonnalizedLevel = DetailLevelItemInformation.Levels.vpBrand;

            //Campaign type 
            _webSession.CampaignType = TNS.AdExpress.Constantes.Web.CustomerSessions.CampaignType.notDefined;

            //Avertisement Type
            _webSession.AdvertisementTypeUniverses = new System.Collections.Generic.Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            _webSession.SelectedLocations = new List<long>();
            _webSession.SelectedPresenceTypes = new List<long>();

            _webSession.IsExcluWeb = false;

            //Initialisation de customerPeriod
            try
            {
                if (_webSession.CustomerPeriodSelected != null)
                    _webSession.CustomerPeriodSelected = null;
            }
            catch (System.Exception) { }

            _webSession.Save();
        }

        //#region Méthodes internes
        ///// <summary>
        ///// Set default media selection
        ///// <remarks>Plurimedia will be the default choice</remarks>
        ///// </summary>
        //private void SetRecapDefaultMediaSelection()
        //{

        //    if (!_webSession.isMediaSelected())
        //    {

        //        // Extraction Last Available Recap Month
        //        var cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
        //        var param = new object[1];
        //        param[0] = _webSession;
        //        var dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
        //            , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
        //            , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

        //        _webSession.LastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.plurimedia));

        //        var current = new System.Windows.Forms.TreeNode("ChoixMedia");
        //        System.Windows.Forms.TreeNode vehicle = null;
        //        int pluriWordCode = 210;
        //        var vehicleNames = DBConstantes.Vehicles.names.plurimedia;
        //        bool isMissingMmms = false;
        //        if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FRANCE))
        //        {
        //            string mmsLastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.mms));
        //            if (Convert.ToInt64(mmsLastAvailableRecapMonth) < Convert.ToInt64(_webSession.LastAvailableRecapMonth))
        //            {
        //                pluriWordCode = 3020;
        //                vehicleNames = DBConstantes.Vehicles.names.PlurimediaWithoutMms;
        //            }
        //        }



        //        //Creating new plurimedia	node	             
        //        vehicle = new TreeNode(GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage))
        //        {
        //            Tag = new LevelInformation(Right.type.vehicleAccess, vehicleNames.GetHashCode(),
        //            GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage)),
        //            Checked = true
        //        };
        //        current.Nodes.Add(vehicle);

        //        //Tracking
        //        _webSession.OnSetVehicle(vehicleNames.GetHashCode());

        //        _webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
        //        _webSession.PreformatedMediaDetail = CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
        //    }
        //}

        ///// <summary>
        ///// Set default period selection
        ///// <remarks>Current or last years will be the default period</remarks>
        ///// </summary>
        //private void SetRecapDefaultPeriodSelection()
        //{

        //    DateTime downloadDate = new DateTime(_webSession.DownLoadDate, 12, 31);
        //    string absolutEndPeriod = "";


        //    try
        //    {

        //        //Choix par défaut année courante
        //        _webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.currentYear;
        //        _webSession.PeriodLength = 1;
        //        // Cas où l'année de chargement est inférieur à l'année en cours
        //        if (DateTime.Now.Year > _webSession.DownLoadDate)
        //        {
        //            _webSession.PeriodBeginningDate = downloadDate.ToString("yyyy01");
        //            _webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
        //        }
        //        else
        //        {
        //            _webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
        //            _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
        //        }

        //        //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
        //        //du dernier mois dispo en BDD
        //        //traitement de la notion de fréquence
        //        CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
        //        object[] param = new object[1];
        //        param[0] = _webSession;
        //        IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
        //        absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);

        //        if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
        //        {
        //            throw (new TNS.AdExpress.Domain.Exceptions.NoDataException());
        //        }

        //        _webSession.PeriodEndDate = absolutEndPeriod;
        //        _webSession.DetailPeriod = CstPeriodDetail.monthly;

        //        //Activation de l'option etude comparative 
        //        _webSession.ComparativeStudy = true;

        //    }
        //    catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
        //    {

        //        //Sinon choix par défaut année précédente
        //        _webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.previousYear;
        //        _webSession.PeriodLength = 1;

        //        // Cas où l'année de chargement est inférieur à l'année en cours
        //        if (DateTime.Now.Year > _webSession.DownLoadDate)
        //        {
        //            _webSession.PeriodBeginningDate = downloadDate.AddYears(-1).ToString("yyyy01");
        //            _webSession.PeriodEndDate = downloadDate.AddYears(-1).ToString("yyyy12");
        //        }
        //        else
        //        {
        //            _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
        //            _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
        //        }
        //        _webSession.DetailPeriod = CstPeriodDetail.monthly;
        //        _webSession.ComparativeStudy = true;
        //    }
        //    catch (System.Exception exc)
        //    {
        //        if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
        //        {
        //            this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
        //        }
        //    }



        //}
        //#endregion

        #region To be deplaced
        /// <summary>
        /// Get allowed units 
        /// </summary>
        /// <param name="vehicleSelection">List of media joined by commas</param>
        /// <returns>Allowed units</returns>
        public static List<CstWeb.CustomerSessions.Unit> GetAllowedUnits(List<CstWeb.CustomerSessions.Unit> unitList, List<CstWeb.CustomerSessions.Unit> AllowedUnitEnumList)
        {
            List<CstWeb.CustomerSessions.Unit> temp = new List<CstWeb.CustomerSessions.Unit>();
            if (AllowedUnitEnumList != null && AllowedUnitEnumList.Count > 0)
            {
                foreach (var item in unitList)
                {
                    if (AllowedUnitEnumList.Contains(item))
                    {
                        temp.Add(item);
                    }
                }
                return temp;
            }
            return unitList;
        }

        #endregion

        #region Private Methods
        private AdExpressUnivers GetUnivers(List<Tree> trees, WebSession webSession, Dimension dimension, Security security)
        {
            AdExpressUnivers result = new AdExpressUnivers(dimension);
            try
            {
                NomenclatureElementsGroup group = null;
                result.AdExpressUniverse.Security = security;
                int index = 0;
                foreach (Tree tree in trees)
                {
                    group = new NomenclatureElementsGroup(index, tree.AccessType);
                    if (tree.UniversLevels.Any())
                    {
                        foreach (var level in tree.UniversLevels.Where(x => x.UniversItems != null))
                        {
                            if (level.UniversItems != null && level.UniversItems.Count > _nbMaxItemByLevel)
                                throw new CapacityException("Dépassement du nombre d'éléments autorisés pour un niveau");
                            List<long> levelItems = new List<long>();
                            foreach (var item in level.UniversItems)
                            {
                                levelItems.Add(item.Id);
                            }
                            if (levelItems.Any())
                                group.AddItems(level.Id, levelItems);
                        }
                    }
                    index++;
                    if (group != null && group.Count() > 0)
                        result.AdExpressUniverse.AddGroup(result.AdExpressUniverse.Count(), group);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.Message = ex.Message;
            }
            return result;
        }

        private bool MustSelectIncludeItems(WebSession webSession)
        {
            switch (webSession.CurrentModule)
            {
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.JUSTIFICATIFS_PRESSE:
                case CstWeb.Module.Name.BILAN_CAMPAGNE:
                case CstWeb.Module.Name.DONNEES_DE_CADRAGE:
                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case CstWeb.Module.Name.INDICATEUR:
                case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                case CstWeb.Module.Name.CELEBRITIES:
                    return true;
                default: return false;
            }
        }
        public bool IsValidUniverseLevels(AdExpressUniverse universe, WebSession webSession)
        {
            if (webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)//&&IsCheckUniverseLevels
            {
                var vehiclesSelected = webSession.GetVehiclesSelected();

                if (vehiclesSelected != null && vehiclesSelected.Count > 0)
                {
                    var param = new object[1];
                    param[0] = _webSession;
                    var clMediaU = WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.mediaDetailLevelUtilities];
                    if (clMediaU == null)
                        throw (new NullReferenceException("Core layer is null for the Media detail level utilities class"));
                    var mediaDetailLevelUtilities = (FctUtilities.MediaDetailLevel)
                                                    AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                                                        string.Format("{0}Bin\\{1}"
                                                                      , AppDomain.CurrentDomain.BaseDirectory,
                                                                      clMediaU.AssemblyName), clMediaU.Class, false,
                                                        BindingFlags.CreateInstance
                                                        | BindingFlags.Instance | BindingFlags.Public, null, param, null,
                                                        null);

                    var activeVehicles =
                        mediaDetailLevelUtilities.GetAllowedVehicles(webSession.GetVehiclesSelected().Keys.ToList(),
                                                                     universe);
                    return (activeVehicles.Count == vehiclesSelected.Count);

                }
            }
            return true;
        }
        private Dictionary<int, AdExpressUniverse> GetConcurrentUniverses(List<Tree> trees, WebSession webSession, Dimension dimension, Security security)
        {
            Dictionary<int, AdExpressUniverse> adExpressUniverses = new Dictionary<int, AdExpressUniverse>(trees.Count);
            try
            {
                int index = 0;
                foreach (Tree tree in trees)
                {
                    AdExpressUniverse adExpressUniverse = new AdExpressUniverse(dimension)
                    {
                        Security = security
                    };
                    Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
                    NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(index, tree.AccessType);
                    if (tree.UniversLevels.Any())
                    {
                        foreach (var level in tree.UniversLevels.Where(x => x.UniversItems != null))
                        {
                            if (level.UniversItems != null && level.UniversItems.Count > _nbMaxItemByLevel)
                                throw new CapacityException("Dépassement du nombre d'éléments autorisés pour un niveau");
                            List<long> levelItems = new List<long>();
                            foreach (var item in level.UniversItems)
                            {
                                levelItems.Add(item.Id);
                            }
                            if (levelItems.Any())
                                treeNomenclatureEG.AddItems(level.Id, levelItems);
                        }
                    }
                    if (treeNomenclatureEG != null && treeNomenclatureEG.Count() > 0)
                    {
                        adExpressUniverse.AddGroup(index, treeNomenclatureEG);
                        adExpressUniverses.Add(index, adExpressUniverse);
                        index++;
                    }
                }
            }
            catch
            {

            }
            return adExpressUniverses;
        }

        public void UpdateSiteLanguage(string webSessionId, int siteLanguage)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            _webSession.SiteLanguage = siteLanguage;
            _webSession.Save();
        }
        #endregion
    }
}


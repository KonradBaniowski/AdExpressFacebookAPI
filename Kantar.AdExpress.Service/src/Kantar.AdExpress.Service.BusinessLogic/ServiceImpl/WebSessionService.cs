﻿using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Data;
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
using System.Text;
using DBClassifConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpressI.Date.DAL;
using CstPeriodDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel;
using TNS.AdExpress.Domain.Layers;
using System.Windows.Forms;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class WebSessionService : IWebSessionService
    {

        #region CONST
        private const string SELECTION = "Selection";
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string ANALYSIS = "Analysis";
        private const string RESULTS = "Results";
        private const string ADVERTISING_AGENCY = "AdvertisingAgency";
        private const string NEW_CREATIVES = "NewCreatives";
        private const string FACEBOOK = "SocialMedia";
        private const string ANALYSE_DES_DISPOSITIFS = "CampaignAnalysis";
        private const string PROGRAM_ANALYSIS = "ProgramAnalysis";
        private const string HEALTH = "Health";
        private const int _nbMaxItemByLevel = 1000;
        private const int MediaRequiredCode = 1052;
        //TODO : A checker pour 100 ou 1000
        private const int MaxItemsPerLevel = 1000;
        private const int PresentAbsentMaxItemsPerLevel = 200;
        private const int ADVERTISERID = 6;
        private const int BRANDID = 8;
        private const int OUTDOOR = 8;
        private const int DOOH = 22;
        private const string FcbMarketErrorMsg = "Please select at maximum 5 advertisers or brands.";
        #endregion
        private WebSession _webSession = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        public WebSessionResponse SaveMediaSelection(SaveMediaSelectionRequest request, HttpContextBase httpContext)
        {
            var _webSession = (WebSession)WebSession.Load(request.WebSessionId);
            WebSessionResponse response = new WebSessionResponse
            {
                StudyStep = StudyStep.Media,
                ControllerDetails = GetCurrentControllerDetails(_webSession.CurrentModule, request.NextStep)
            };
            bool success = false;
            #region Try Catch block
            try
            {
                bool maxSizeErr = true;
                if (request.MediaSupportRequired && !request.Trees.Any())
                {
                    maxSizeErr = false;
                    response.ErrorMessage = GestionWeb.GetWebWord(CstWeb.LanguageConstantes.MediaRequiredCode, _webSession.SiteLanguage);
                }
                else
                {
                    #region Fix max items per level
                    if (_webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE)
                    {
                        if (request.Trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null && x.UniversItems.Count > PresentAbsentMaxItemsPerLevel).Any()).Any())
                        {
                            maxSizeErr = false;
                            response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                        }
                    }
                    else
                    {
                        if (request.Trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null && x.UniversItems.Count > MaxItemsPerLevel).Any()).Any())
                        {
                            maxSizeErr = false;
                            response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                        }
                    }
                    #endregion
                }

                if (maxSizeErr)
                {
                    #region Save Media Selection in WebSession
                    WebNavigation.Module _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);
                    if (_webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA
                        && request.MediaIds.Contains(OUTDOOR) && request.MediaIds.Contains(DOOH))
                    {
                        response.ErrorMessage = GestionWeb.GetWebWord(3080, _webSession.SiteLanguage);
                        return response;
                    }
                    SetInsertOption(_webSession, request.MediaIds);
                    List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                    System.Windows.Forms.TreeNode tmpNode;
                    bool containsSearch = false;
                    bool containsSocial = false;
                    foreach (var item in request.MediaIds)
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
                        if (request.MediaIds.Count != vehicleInformationList.Count)
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
                if (request.Trees.Any())
                {
                    switch (_webSession.CurrentModule)
                    {
                        case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                        case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                        case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                        case CstWeb.Module.Name.INDICATEUR:
                        case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                        case CstWeb.Module.Name.ANALYSE_MANDATAIRES:
                            success = SetDefaultUnivers(request, _webSession, response);
                            break;
                        case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                            Dictionary<int, AdExpressUniverse> universes = GetConcurrentUniverses(request.Trees, _webSession, request.Dimension, request.Security);
                            _webSession.PrincipalMediaUniverses = universes;
                            success = true;
                            break;
                        default:
                            break;
                    }
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
            catch (System.Exception ex)
            {
                if (ex.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    response.ErrorMessage = ex.Message;
                }
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _webSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            #endregion
            return response;
        }


        public WebSessionResponse SaveSponsorshipMediaSelection(SaveMediaSelectionRequest request, HttpContextBase httpContext)
        {
            var _webSession = (WebSession)WebSession.Load(request.WebSessionId);
            WebSessionResponse response = new WebSessionResponse
            {
                StudyStep = StudyStep.Media,
                ControllerDetails = GetCurrentControllerDetails(_webSession.CurrentModule, request.NextStep)
            };
            bool success = false;
            #region Try Catch block
            try
            {
                bool maxSizeErr = true;
                if (request.MediaSupportRequired && !request.Trees.Any())
                {
                    maxSizeErr = false;
                    response.ErrorMessage = GestionWeb.GetWebWord(CstWeb.LanguageConstantes.MediaRequiredCode, _webSession.SiteLanguage);
                }
                else
                {
                    #region Fix max items per level
                    if (_webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE)
                    {
                        if (request.Trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null && x.UniversItems.Count > PresentAbsentMaxItemsPerLevel).Any()).Any())
                        {
                            maxSizeErr = false;
                            response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                        }
                    }
                    else
                    {
                        if (request.Trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null && x.UniversItems.Count > MaxItemsPerLevel).Any()).Any())
                        {
                            maxSizeErr = false;
                            response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                        }
                    }
                    #endregion
                }

                if (maxSizeErr)
                {
                    #region Save Media Selection in WebSession
                    WebNavigation.Module _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);

                    List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                    System.Windows.Forms.TreeNode tmpNode;
                    foreach (var item in request.MediaIds)
                    {
                        tmpNode = new System.Windows.Forms.TreeNode(item.ToString());
                        tmpNode.Tag = new LevelInformation(CstWebCustomer.Right.type.mediaAccess, item, item.ToString());
                        tmpNode.Checked = true;
                        levelsSelected.Add(tmpNode);
                    }
                    if (levelsSelected.Count == 0)
                    {
                        response.ErrorMessage = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage);
                    }
                    else
                    {

                        // Sauvegarde de la sélection dans la session
                        //Si la sélection comporte des éléments, on la vide
                        _webSession.CurrentUniversMedia.Nodes.Clear();

                        foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                        {
                            _webSession.CurrentUniversMedia.Nodes.Add(node);
                            // Tracking
                            //_webSession.OnSetVehicle(((LevelInformation)node.Tag).ID);
                        }

                        success = true;
                    }

                    #endregion
                }

                #region Save Media support if any
                if (request.Trees.Any())
                {
                    switch (_webSession.CurrentModule)
                    {
                        case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                        case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                        case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                        case CstWeb.Module.Name.INDICATEUR:
                        case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                        case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                            success = SetDefaultUnivers(request, _webSession, response);
                            break;
                        case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                            Dictionary<int, AdExpressUniverse> universes = GetConcurrentUniverses(request.Trees, _webSession, request.Dimension, request.Security);
                            _webSession.PrincipalMediaUniverses = universes;
                            success = true;
                            break;
                        default:
                            break;
                    }
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
            catch (System.Exception ex)
            {
                if (ex.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    response.ErrorMessage = ex.Message;
                }
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _webSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            #endregion
            return response;
        }

        public WebSessionResponse SaveHealthMediaSelection(SaveMediaSelectionRequest request)
        {
            var _webSession = (WebSession)WebSession.Load(request.WebSessionId);
            WebSessionResponse response = new WebSessionResponse
            {
                StudyStep = StudyStep.Media,
                ControllerDetails = GetCurrentControllerDetails(_webSession.CurrentModule, request.NextStep),
                Success = false
            };

            try
            {
                WebNavigation.Module _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);

                List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                System.Windows.Forms.TreeNode tmpNode;

                request.MediaIds.ForEach(item =>
                {
                    tmpNode = new System.Windows.Forms.TreeNode(item.ToString());
                    tmpNode.Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, item, item.ToString());
                    tmpNode.Checked = true;
                    levelsSelected.Add(tmpNode);
                });

                if (levelsSelected.Count == 0)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage);
                }
                else
                {
                    // Sauvegarde de la sélection dans la session
                    //Si la sélection comporte des éléments, on la vide
                    _webSession.SelectionUniversMedia.Nodes.Clear();

                    foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                    {
                        _webSession.SelectionUniversMedia.Nodes.Add(node);
                        // Tracking
                        // _webSession.OnSetVehicle(((LevelInformation)node.Tag).ID);
                    }
                    response.Success = true;
                }

                #region Save Media support if any
                if (request.Trees.Any())
                {
                    response.Success = SetDefaultUnivers(request, _webSession, response);
                }
                #endregion

                #region Save WebSession
                if (response.Success)
                {
                    _webSession.Save();
                    _webSession.Source.Close();
                    response.Success = true;
                }
                #endregion

            }
            catch (Exception ex)
            {
                SetLog(request, _webSession, ex);
                throw;
            }

            return response;
        }

      


        public WebSessionResponse SaveMarketSelection(SaveMarketSelectionRequest request, HttpContextBase httpContext)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                StudyStep = StudyStep.Market
            };
            var _webSession = (WebSession)WebSession.Load(request.WebSessionId);
            try
            {
                response.ControllerDetails = GetCurrentControllerDetails(_webSession.CurrentModule, request.NextStep);
                IsValidMarketRequest(request, _webSession, response);

                if (response.Success)
                {
                    bool maxSizeErr = true;
                    #region Fix max items per level

                    if (request.Trees.Where(p => p.UniversLevels != null && p.UniversLevels.Where(x => x.UniversItems != null && x.UniversItems.Count > MaxItemsPerLevel).Any()).Any())
                    {
                        maxSizeErr = false;
                        response.ErrorMessage = GestionWeb.GetWebWord(2286, _webSession.SiteLanguage);
                    }
                    #endregion

                    #region try catch block
                    if (maxSizeErr)
                    {
                        try
                        {
                            switch (_webSession.CurrentModule)
                            {
                                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                                case CstWeb.Module.Name.INDICATEUR:
                                case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                                case CstWeb.Module.Name.ANALYSE_MANDATAIRES:
                                case CstWeb.Module.Name.NEW_CREATIVES:
                                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                                case CstWeb.Module.Name.HEALTH:
                                    AdExpressUnivers univers = GetUnivers(request.Trees, _webSession, request.Dimension, request.Security);
                                    SetDefaultMarketUniverse(response, univers, request, _webSession, httpContext);
                                    break;
                                case CstWeb.Module.Name.FACEBOOK:
                                    Dictionary<int, AdExpressUniverse> universes = GetConcurrentUniverses(request.Trees, _webSession, request.Dimension, request.Security);
                                    _webSession.PrincipalProductUniverses = universes;
                                    response.Success = true;
                                    _webSession.Save();
                                    break;
                                default:
                                    break;
                            }
                        }

                        catch (SecurityException)
                        {
                            _webSession.PrincipalProductUniverses = new Dictionary<int, AdExpressUniverse>();
                            _webSession.PrincipalAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
                            _webSession.Save();
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(2285, _webSession.SiteLanguage));
                        }
                        catch (CapacityException)
                        {
                            _webSession.PrincipalProductUniverses = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
                            _webSession.PrincipalAdvertisingAgnecyUniverses = new Dictionary<int, AdExpressUniverse>();
                            _webSession.Save();
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(2286, _webSession.SiteLanguage));
                        }
                        catch (Exception)
                        {
                            response.ErrorMessage = String.Format("{0} - {1}", FrameWorkSelection.error.SECURITY_EXCEPTION, GestionWeb.GetWebWord(922, _webSession.SiteLanguage));
                        }
                    }
                    #endregion
                }
                else if (_webSession.CurrentModule != CstWeb.Module.Name.FACEBOOK)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(2299, _webSession.SiteLanguage);
                }
            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _webSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return response;
        }

        public WebSessionDetails GetWebSession(string webSessionId)
        {
            var webSession = (WebSession)WebSession.Load(webSessionId);
            WebSessionDetails response = new WebSessionDetails
            {
                WebSession = webSession,
                ControllerDetails = GetCurrentControllerDetails(webSession.CurrentModule)
            };
            return response;
        }

        //Start from here
        public void SaveCurrentModule(string webSessionId, int moduleId, HttpContextBase httpContext)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
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

                _webSession.Units = new List<CstWeb.CustomerSessions.Unit> { UnitsInformation.DefaultCurrency };

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES
                    && WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                {
                    _webSession.Units = new List<CstWeb.CustomerSessions.Unit> {CstWeb.CustomerSessions.Unit.versionNb };
                }

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                   && WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                {
                    _webSession.Units.Add(CstWeb.CustomerSessions.Unit.spot);
                    _webSession.Units.Add(CstWeb.CustomerSessions.Unit.duration);
                }

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

                if (_webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE
                    || _webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                {

                    SetRecapDefaultMediaSelection();
                    SetRecapDefaultPeriodSelection();
                }
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

                //Intiliaze Facebook module Criteria
                if (_webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK)
                {
                    SetSocialMediaDefaultSelection();
                }

                _webSession.Save();

            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _webSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());
                throw;
            }
        }

        public int GetSiteLanguage(string webSessionId)
        {
            var webSession = (WebSession)WebSession.Load(webSessionId);
            return webSession.SiteLanguage;
        }

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

        public void UpdateSiteLanguage(string webSessionId, int siteLanguage)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            _webSession.SiteLanguage = siteLanguage;
            _webSession.Save();
        }

        public bool IsValidUniverseLevels(AdExpressUniverse universe, WebSession webSession, HttpContextBase httpContext)
        {
            bool result = true;
            try
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
                        result = (activeVehicles.Count == vehiclesSelected.Count);

                    }
                }

            }
            catch (Exception ex)
            {
                CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                Logger.Log(LogLevel.Error, cwe.GetLog());

                throw;
            }
            return result;
        }

        public PostModel GetPostModel(string webSessionId, string period)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            PostModel pM = new PostModel();
            if (string.IsNullOrEmpty(period))
            {
                pM.BeginDate = long.Parse(_webSession.PeriodBeginningDate);
                pM.EndDate = long.Parse(_webSession.PeriodEndDate);
            }
            else
            {
                int year = int.Parse(period.Substring(0, 4));
                int month = int.Parse(period.Substring(4, 2));
                int endDay = DateTime.DaysInMonth(year, month);
                string startDate = new DateTime(year, month, 01).ToString("yyyyMMdd");
                string endDate = new DateTime(year, month, endDay).ToString("yyyyMMdd");
                pM.BeginDate = long.Parse(startDate);
                pM.EndDate = long.Parse(endDate);
            }
            pM.IdLogin = (int)_webSession.CustomerLogin.IdLogin;
            pM.IdLanguage = _webSession.DataLanguage;

            return pM;
        }

        public UserCriteria GetUserCriteria(string webSessionId, string period)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            UserCriteria uC = new UserCriteria();
            List<NomenclatureElementsGroup> nElmtGr = new List<NomenclatureElementsGroup>();
            string tempListAsString = string.Empty;

            #region Period 
            if (string.IsNullOrEmpty(period))
            {
                int endDayPeriodEndDate = DateTime.DaysInMonth(int.Parse(_webSession.PeriodEndDate.Substring(0, 4)), int.Parse(_webSession.PeriodEndDate.Substring(4, 2)));
                uC.StartDate = new DateTime(int.Parse(_webSession.PeriodBeginningDate.Substring(0, 4)), int.Parse(_webSession.PeriodBeginningDate.Substring(4, 2)), 01);
                uC.EndDate = new DateTime(int.Parse(_webSession.PeriodEndDate.Substring(0, 4)), int.Parse(_webSession.PeriodEndDate.Substring(4, 2)), endDayPeriodEndDate);
            }
            #endregion

            #region Media
            uC.CanalIds = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstRight.type.vehicleAccess).Split(',').Select(double.Parse).ToList();

            #region Media Filter
            if (_webSession.PrincipalMediaUniverses.Count > 0)
            {
                //Includes
                nElmtGr = _webSession.PrincipalMediaUniverses[0].GetIncludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.MEDECIN);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.MediaFilterIncludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                }

                //Excludes
                nElmtGr = _webSession.PrincipalMediaUniverses[0].GetExludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.MEDECIN);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.MediaFilterExcludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                }
            }
            #endregion

            #endregion

            #region Market

            if (_webSession.PrincipalProductUniverses.Count > 0)
            {
                //Includes
                nElmtGr = _webSession.PrincipalProductUniverses[0].GetIncludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.CATEGORY);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.CategoryIncludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.PRODUCT);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.ProductIncludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GRP_PHARMA);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.GrpPharmaIncludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                }

                //Excludes
                nElmtGr = _webSession.PrincipalProductUniverses[0].GetExludes();
                if (nElmtGr != null && nElmtGr.Count > 0)
                {
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.CATEGORY);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.CategoryExcludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.PRODUCT);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.ProductExcludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                    tempListAsString = nElmtGr[0].GetAsString(TNSClassificationLevels.GRP_PHARMA);
                    if (tempListAsString != null && tempListAsString.Length > 0) uC.GrpPharmaExcludeIds = tempListAsString.Split(',').Select(double.Parse).ToList();
                }
            }

            #endregion

            return uC;
        }

        #region Dates sélectionnées

        //public string GetDateSelected(string webSessionId, bool dateFormatText = true)
        //{

        //    _webSession = (WebSession)WebSession.Load(webSessionId);

        //    StringBuilder html = new StringBuilder();
        //    string startDate = "";
        //    string endDate = "";

        //    if (dateFormatText)
        //    {
        //        startDate = FctUtilities.Dates.getPeriodTxt(_webSession, _webSession.PeriodBeginningDate);
        //        endDate = FctUtilities.Dates.getPeriodTxt(_webSession, _webSession.PeriodEndDate);
        //    }


        //    return null;
        //}
        #endregion


        #region Méthodes internes

        private void SetSocialMediaDefaultSelection()
        {
            //Set default media
            _webSession.SelectionUniversMedia.Nodes.Clear();
            System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("SOCIAL MEDIA");
            tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.social), "SOCIAL MEDIA");
            _webSession.SelectionUniversMedia.Nodes.Add(tmpNode);

            //Set last 3 months by defaut
            _webSession.PeriodLength = 3;
            _webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.nLastMonth;
            _webSession.DetailPeriod = CstPeriodDetail.dayly;
            var endDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            _webSession.PeriodBeginningDate = DateTime.Now.AddMonths(1 - _webSession.PeriodLength).ToString("yyyyMM01");
            _webSession.PeriodEndDate = endDate.ToString("yyyyMMdd");

            //TODO: Set default product universe
            var defaultUniverse = GetUniverses(Dimension.product, _webSession, 0, true).FirstOrDefault();
            if (defaultUniverse != null)
            {
                SetFcbProductUniverse(defaultUniverse.Id, _webSession);
                //var a = UniversListDataAccess.GetTreeNodeUniverse(defaultUniverse.Id, _webSession);
            }


        }

        private void SetFcbProductUniverse(long idUniverse, WebSession webSession)
        {
            Dictionary<int, AdExpressUniverse> result = new Dictionary<int, AdExpressUniverse>();
            List<long> idMedia = new List<long>();
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universe = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)UniversListDataAccess.GetTreeNodeUniverseWithMedia(idUniverse, _webSession, out idMedia);
            webSession.PrincipalProductUniverses = universe;

        }

        /// <summary>
        /// Set default media selection
        /// <remarks>Plurimedia will be the default choice</remarks>
        /// </summary>
        private void SetRecapDefaultMediaSelection()
        {

            if (!_webSession.isMediaSelected())
            {

                // Extraction Last Available Recap Month
                var cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                var param = new object[1];
                param[0] = _webSession;
                var dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
                    , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                //System.Windows.Forms.TreeNode vehicle = null;
                System.Windows.Forms.TreeNode current = new System.Windows.Forms.TreeNode("ChoixMedia");
                int pluriWordCode = 210;
                DBConstantes.Vehicles.names vehicleNames = DBConstantes.Vehicles.names.plurimedia;

                if (WebApplicationParameters.CountryCode.Equals(CstWeb.CountryCode.TURKEY))
                {

                    _webSession.LastAvailableRecapMonth =
                        dateDAL.CheckAvailableDateForMedia(
                            VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.tv));                    
                    pluriWordCode = 206;
                    vehicleNames = DBConstantes.Vehicles.names.tv;

                }
                else
                {
                    _webSession.LastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(VehiclesInformation.EnumToDatabaseId(DBConstantes.Vehicles.names.plurimedia));                 
                }

                if (WebApplicationParameters.CountryCode.Equals(CstWeb.CountryCode.FRANCE))
                {
                    string mmsLastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(VehiclesInformation.EnumToDatabaseId(DBClassifConstantes.Vehicles.names.mms));
                    if (Convert.ToInt64(mmsLastAvailableRecapMonth) < Convert.ToInt64(_webSession.LastAvailableRecapMonth))
                        _webSession.LastAvailableRecapMonth = mmsLastAvailableRecapMonth;
                }

                //Creating new plurimedia	node
                System.Windows.Forms.TreeNode vehicle;
                if (WebApplicationParameters.CountryCode.Equals(CstWeb.CountryCode.TURKEY))
                {
                   vehicle = new TreeNode(GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage))
                    {
                        Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(vehicleNames),
                  GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage)),
                        Checked = true
                    };
                }
                else
                {
                     vehicle = new TreeNode(GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage))
                    {
                        Tag = new LevelInformation(CstWebCustomer.Right.type.vehicleAccess, vehicleNames.GetHashCode(),
                  GestionWeb.GetWebWord(pluriWordCode, _webSession.SiteLanguage)),
                        Checked = true
                    };
                }              
                current.Nodes.Add(vehicle);

                //Tracking
                _webSession.OnSetVehicle(vehicleNames.GetHashCode());

                _webSession.SelectionUniversMedia = _webSession.CurrentUniversMedia = current;
                _webSession.PreformatedMediaDetail = CstWeb.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
            }
        }

        /// <summary>
        /// Set default period selection
        /// <remarks>Current or last years will be the default period</remarks>
        /// </summary>
        private void SetRecapDefaultPeriodSelection()
        {

            DateTime downloadDate = new DateTime(_webSession.DownLoadDate, 12, 31);
            string absolutEndPeriod = "";


            try
            {

                //Choix par défaut année courante
                _webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.currentYear;
                _webSession.PeriodLength = 1;
                // Cas où l'année de chargement est inférieur à l'année en cours
                if (DateTime.Now.Year > _webSession.DownLoadDate)
                {
                    _webSession.PeriodBeginningDate = downloadDate.ToString("yyyy01");
                    _webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");
                }
                else
                {
                    _webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");
                    _webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
                }

                //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
                //du dernier mois dispo en BDD
                //traitement de la notion de fréquence
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" 
+ cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                absolutEndPeriod = dateDAL.CheckPeriodValidity(_webSession, _webSession.PeriodEndDate);

                if ((int.Parse(absolutEndPeriod) < int.Parse(_webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
                {
                    throw (new TNS.AdExpress.Domain.Exceptions.NoDataException());
                }

                _webSession.PeriodEndDate = absolutEndPeriod;
                _webSession.DetailPeriod = CstPeriodDetail.monthly;

                //Activation de l'option etude comparative 
                _webSession.ComparativeStudy = true;

            }
            catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
            {

                //Sinon choix par défaut année précédente
                _webSession.PeriodType = CstWeb.CustomerSessions.Period.Type.previousYear;
                _webSession.PeriodLength = 1;

                // Cas où l'année de chargement est inférieur à l'année en cours
                if (DateTime.Now.Year > _webSession.DownLoadDate)
                {
                    _webSession.PeriodBeginningDate = downloadDate.AddYears(-1).ToString("yyyy01");
                    _webSession.PeriodEndDate = downloadDate.AddYears(-1).ToString("yyyy12");
                }
                else
                {
                    _webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy01");
                    _webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy12");
                }
                _webSession.DetailPeriod = CstPeriodDetail.monthly;
                _webSession.ComparativeStudy = true;
            }
            catch (System.Exception ex)
            {
                throw (ex);
            }



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
                case CstWeb.Module.Name.NEW_CREATIVES:
                case CstWeb.Module.Name.ANALYSE_MANDATAIRES:
                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                    return true;
                default: return false;
            }
        }

        private Dictionary<int, AdExpressUniverse> GetConcurrentUniverses(List<Tree> trees, WebSession webSession, Dimension dimension, Security security)
        {
            Dictionary<int, AdExpressUniverse> adExpressUniverses = new Dictionary<int, AdExpressUniverse>(trees.Count);
            try
            {
                int index = 0;
                int indexFb = trees.Count - 1;
                foreach (Tree tree in trees)
                {
                    NomenclatureElementsGroup treeNomenclatureEG = null;
                    AdExpressUniverse adExpressUniverse = new AdExpressUniverse(dimension)
                    {
                        Security = security
                    };
                    Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
                    if (webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK)
                        treeNomenclatureEG = new NomenclatureElementsGroup(indexFb, tree.AccessType);
                    else
                        treeNomenclatureEG = new NomenclatureElementsGroup(index, tree.AccessType);
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
                        int key = (webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK) ? index : 0;
                        adExpressUniverse.AddGroup(key, treeNomenclatureEG);
                        adExpressUniverses.Add(index, adExpressUniverse);
                        index++;
                        indexFb--;
                    }
                }
            }
            catch (Exception ex)
            {
                adExpressUniverses = new Dictionary<int, AdExpressUniverse>(trees.Count);
            }
            return adExpressUniverses;
        }

        private ControllerDetails GetCurrentControllerDetails(long currentModule, string nextStep = "")
        {
            long currentModuleCode = 0;
            string currentController = string.Empty;
            string currentModuleIcon = "icon-chart";
            switch (currentModule)
            {
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentModuleCode = CstWeb.LanguageConstantes.MediaScheduleCode;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? MEDIASCHEDULE : SELECTION;
                    currentModuleIcon = "icon-chart";
                    break;
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = CstWeb.LanguageConstantes.PortfolioCode;
                    currentController = "Portfolio";
                    currentModuleIcon = "icon-layers";
                    break;
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = CstWeb.LanguageConstantes.LostWonCode;
                    currentController = "LostWon";
                    currentModuleIcon = "icon-calculator";
                    break;
                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = CstWeb.LanguageConstantes.PresentAbsentCode;
                    currentController = "PresentAbsent";
                    currentModuleIcon = "icon-equalizer";
                    break;
                case CstWeb.Module.Name.INDICATEUR:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalysisGraphics;
                    currentController = "Selection";
                    break;
                case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalysisDetailedReport;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSIS : SELECTION;
                    currentModuleIcon = "icon-book-open";
                    break;
                case CstWeb.Module.Name.FACEBOOK:
                    currentModuleCode = CstWeb.LanguageConstantes.FacebookCode;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? FACEBOOK : SELECTION;
                    currentModuleIcon = "icon-social-facebook";
                    break;
                case CstWeb.Module.Name.ANALYSE_MANDATAIRES:
                    currentModuleCode = CstWeb.LanguageConstantes.MediaAgencyAnalysis;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ADVERTISING_AGENCY : SELECTION;
                    currentModuleIcon = "icon-picture";
                    break;
                case CstWeb.Module.Name.NEW_CREATIVES:
                    currentModuleCode = CstWeb.LanguageConstantes.NewCreatives;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? NEW_CREATIVES : SELECTION;
                    currentModuleIcon = "icon-camrecorder";
                    break;
                case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalyseDispositifsLabel;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? ANALYSE_DES_DISPOSITIFS : SELECTION;
                    currentModuleIcon = "icon-puzzle";
                    break;
                case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalyseProgrammesLabel;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? PROGRAM_ANALYSIS : SELECTION;
                    currentModuleIcon = "icon-puzzle";
                    break;
                case CstWeb.Module.Name.HEALTH:
                    currentModuleCode = CstWeb.LanguageConstantes.Health;
                    currentController = (!string.IsNullOrEmpty(nextStep) && nextStep == RESULTS) ? HEALTH : SELECTION;
                    currentModuleIcon = "icon-heart";
                    break;
                default:
                    break;
            }
            var current = new ControllerDetails
            {
                ModuleCode = currentModuleCode,
                Name = currentController,
                ModuleId = currentModule,
                ModuleIcon = currentModuleIcon
            };
            return current;
        }

        private bool SetDefaultUnivers(SaveMediaSelectionRequest request, WebSession _webSession, WebSessionResponse response)
        {
            bool success = false;
            Dictionary<int, AdExpressUniverse> universDictionary = new Dictionary<int, AdExpressUniverse>();
            AdExpressUnivers univers = GetUnivers(request.Trees, _webSession, request.Dimension, request.Security);
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
            return success;
        }

        public bool IsAllSelectionStep(string webSessionId)
        {
            _webSession = (WebSession)WebSession.Load(webSessionId);
            if (_webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_MANDATAIRES)
                return _webSession.isMediaSelected() && _webSession.isDatesSelected() && _webSession.IsCurrentUniversAdvertisingAgnecySelected();
            else
                return _webSession.isMediaSelected() && _webSession.isDatesSelected() && _webSession.IsCurrentUniversProductSelected();
        }

        private List<UserUnivers> GetUniverses(Dimension dimension, WebSession webSession, long idGroup = 0, bool getDefaultUniverse = false)
        {
            List<UserUnivers> result = new List<UserUnivers>();
            var branch = (dimension == Dimension.product) ? Branch.type.product.GetHashCode().ToString() : Branch.type.media.GetHashCode().ToString();
            if (webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK)
                branch = Branch.type.productSocial.GetHashCode().ToString();
            var data = UniversListDataAccess.GetData(webSession, branch, string.Empty);
            if (data != null && data.Tables[0].AsEnumerable().Any())
            {
                var list = data.Tables[0].AsEnumerable().Select(p => new
                {
                    GroupID = p.Field<long?>("ID_GROUP_UNIVERSE_CLIENT"),
                    GroupDescription = p.Field<string>("GROUP_UNIVERSE_CLIENT"),
                    UniversID = p.Field<long?>("ID_UNIVERSE_CLIENT"),
                    UniversDescription = p.Field<string>("UNIVERSE_CLIENT"),
                    IsDefault = (p["IS_DEFAULT"] != null && Convert.ToInt32(p["IS_DEFAULT"]) == 1) ? true : false
                }).ToList();
                if (idGroup > 0)
                    list = list.Where(p => p.GroupID == idGroup).ToList();
                foreach (var item in list)
                {
                    UserUnivers UserUnivers = new UserUnivers
                    {
                        ParentId = item.GroupID ?? 0,
                        ParentDescription = item.GroupDescription,
                        Id = item.UniversID ?? 0,
                        Description = item.UniversDescription,
                        IsDefault = item.IsDefault
                    };
                    result.Add(UserUnivers);
                }
                if (getDefaultUniverse && webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK)
                    result = result.Where(p => p.IsDefault).ToList();
            }
            return result;
        }

        private void IsValidMarketRequest(SaveMarketSelectionRequest request, WebSession webSession, WebSessionResponse response)
        {
            response.Success = (request.Required) ? (request.Trees.Any() && request.Trees.Where(p => p.UniversLevels != null).Any() && request.Trees.Where(p => p.UniversLevels.Where(x => x.UniversItems != null).Any()).Any()) : true;

            if (webSession.CurrentModule == CstWeb.Module.Name.FACEBOOK && response.Success)
            {
                if (!request.Trees.Where(p => p.Id == 1 && (p.UniversLevels != null)).Any())
                {
                    response.Success = false;
                    response.ErrorMessage = GestionWeb.GetWebWord(2299, webSession.SiteLanguage);
                    return;
                }
                List<UniversLevel> advertisers = request.Trees.SelectMany(p => p.UniversLevels)
                                                                .Where(x => x.Id == ADVERTISERID && x.UniversItems.Count > 0)
                                                                .ToList();
                List<UniversLevel> brands = request.Trees.SelectMany(p => p.UniversLevels)
                                                                .Where(x => x.Id == BRANDID && x.UniversItems.Count > 0)
                                                                .ToList();
                if (advertisers.Count > 0 && brands.Count > 0)
                {
                    response.Success = false;
                }
                else
                {
                    List<long> referents = GetFacebookSelectedItems(request, 1);
                    List<long> concurrents = GetFacebookSelectedItems(request, 0);
                    response.Success = CheckFacebookItems(referents) && CheckFacebookItems(concurrents);
                    if (response.Success)
                    {
                        List<long> result = referents.Where(p => concurrents.Any(p2 => p2 == p)).ToList();
                        if (result.Any())
                        {
                            response.Success = false;
                            response.ErrorMessage = " You can not select an item as a referent and as competitor.";
                        }
                    }
                    else
                    {
                        response.ErrorMessage = FcbMarketErrorMsg;
                    }
                }

            }
        }

        private bool CheckFacebookItems(List<long> level)
        {
            int maxItems = int.Parse(System.Configuration.ConfigurationManager.AppSettings["FacebookMaxItems"]);
            bool isValid = level.Count < maxItems + 1;
            return isValid;
        }

        private void SetDefaultMarketUniverse(WebSessionResponse response, AdExpressUnivers univers, SaveMarketSelectionRequest request, WebSession webSession, HttpContextBase httpContext)
        {
            if (univers.AdExpressUniverse != null && univers.AdExpressUniverse.Count() > 0)
            {
                bool mustSelectIncludeItems = MustSelectIncludeItems(webSession);
                List<NomenclatureElementsGroup> nGroups = univers.AdExpressUniverse.GetIncludes();
                if ((mustSelectIncludeItems && nGroups != null && nGroups.Count > 0) || !mustSelectIncludeItems)
                {
                    Dictionary<int, AdExpressUniverse>
                            universDictionary = new Dictionary<int, AdExpressUniverse>();
                    universDictionary.Add(universDictionary.Count, univers.AdExpressUniverse);

                    if (!IsValidUniverseLevels(univers.AdExpressUniverse, webSession, httpContext))
                    {
                        response.ErrorMessage = GestionWeb.GetWebWord(2990, webSession.SiteLanguage);
                        response.Success = false;
                    }
                    else
                    {
                        if (webSession.CurrentModule == CstWeb.Module.Name.ANALYSE_MANDATAIRES)
                            webSession.PrincipalAdvertisingAgnecyUniverses = universDictionary;
                        else
                            webSession.PrincipalProductUniverses = universDictionary;
                        response.Success = true;
                        webSession.Save();
                        webSession.Source.Close();
                    }
                }
                else
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(2299, webSession.SiteLanguage);
                    response.Success = false;
                }
            }
            else if (request.Required)
            {
                response.ErrorMessage = GestionWeb.GetWebWord(878, webSession.SiteLanguage);
                response.Success = false;
            }
            else
            {
                response.Success = true;
            }
        }

        private void SetFacebookMarketUniverse(WebSessionResponse response, WebSession webSession, SaveMarketSelectionRequest request)
        {
            Dictionary<int, AdExpressUniverse> adExpressUniverses = new Dictionary<int, AdExpressUniverse>(request.Trees.Count);
            try
            {
                int index = 0;
                foreach (Tree tree in request.Trees)
                {
                    AdExpressUniverse adExpressUniverse = new AdExpressUniverse(request.Dimension)
                    {
                        Security = request.Security
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
                        adExpressUniverse.AddGroup(0, treeNomenclatureEG);
                        adExpressUniverses.Add(index, adExpressUniverse);
                        index++;
                    }
                }
            }
            catch (Exception ex)
            {
                adExpressUniverses = new Dictionary<int, AdExpressUniverse>(request.Trees.Count);
            }
            webSession.PrincipalProductUniverses = adExpressUniverses;
            response.Success = true;
            webSession.Save();

        }

        private List<long> GetFacebookSelectedItems(SaveMarketSelectionRequest request, int idTree)
        {
            List<UniversLevel> levels = request.Trees.Where(x => x.Id == idTree).SelectMany(p => p.UniversLevels)
                                                                .Where(x => (x.Id == ADVERTISERID || x.Id == BRANDID) && x.UniversItems.Count > 0)
                                                                .ToList();
            List<long> ids = levels.SelectMany(p => p.UniversItems.Select(x => x.Id))
                                        .ToList();
            return ids;
        }


        private static void SetLog(SaveMediaSelectionRequest request, WebSession webSession, Exception ex)
        {
            CustomerWebException cwe = new CustomerWebException(webSession, ex);
            cwe.Browser = request.ClientInformation.Browser;
            cwe.VersionBrowser = request.ClientInformation.BrowserVersion;
            cwe.MinorVersionBrowser = request.ClientInformation.BrowserMinorVersion;
            cwe.Platform = request.ClientInformation.BrowserPlatform;
            cwe.UserAgent = request.ClientInformation.UserAgent;
            cwe.UserHostAddress = request.ClientInformation.UserHostAddress;
            cwe.Url = request.ClientInformation.Url.ToString();
            cwe.ServerName = request.ClientInformation.ServerMachineName;
            Logger.Log(LogLevel.Error, cwe.GetLog());
        }

        private void SetInsertOption(WebSession webSession, List<long> mediaIds)
        {
            if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.press)
                         && !mediaIds.Contains(VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.press).DatabaseId))
            {
                webSession.Insert = CstWeb.CustomerSessions.Insert.total;
            }

        }
        #endregion
    }
}


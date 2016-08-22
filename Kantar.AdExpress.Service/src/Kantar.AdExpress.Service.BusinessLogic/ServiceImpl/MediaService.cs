﻿using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using System.Data;
using TNS.AdExpress.Domain.Classification;
using System.Linq;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain;
using System.Text;
using TNS.AdExpress.Vehicle.DAL;
using TNS.AdExpress.Domain.Web;
using CstWeb = TNS.AdExpress.Constantes.Web;
using VhCstes = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;
using TNS.AdExpress.Domain.Translation;
using NLog;
using TNS.AdExpress.Domain.Web.Navigation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaService : IMediaService
    {
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        private const string SELECTION = "Selection";
        private const string PORTFOLIO = "Portfolio";
        private const string LOSTWON = "LostWon";
        private const string PRESENTABSENT = "PresentAbsent";
        private const string MEDIASCHEDULE = "MediaSchedule";
        private const string ANALYSIS = "Analysis";
        private const string RESULTS = "Results";
        private const string MEDIAAGENCY = "MediaAgency";
        private const string NEW_CREATIVES = "NewCreatives";
        private const string FACEBOOK = "SocialMedia";
        public MediaResponse GetMedia(string idWebSession)
        {

            var webSession = (WebSession)WebSession.Load(idWebSession);
            var result = new MediaResponse(webSession.SiteLanguage, webSession.CurrentModule);
            try
            {
                string[] media = Lists.GetIdList(CstWeb.GroupList.ID.media, CstWeb.GroupList.Type.mediaInSelectAll).Split(',');
                result.MediaCommon = Array.ConvertAll(media, Convert.ToInt32).ToList();
                result.ControllerDetails = GetCurrentControllerDetails(webSession.CurrentModule);
                webSession.SelectionUniversMedia.Nodes.Clear();
                webSession.PrincipalMediaUniverses.Clear();
                webSession.Save();
                if (webSession.CurrentModule == CstWeb.Module.Name.INDICATEUR || webSession.CurrentModule == CstWeb.Module.Name.TABLEAU_DYNAMIQUE)
                {
                    result = GetAnalysisVehicleList(webSession, result);
                }
                else
                {
                    result = GetDefaultVehicleList(webSession, result);
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, webSession.UserAgent, webSession.CustomerLogin.Login, webSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule), webSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);
                result.ErrorMessage = message;
            }
            return result;
        }

        public List<Core.Domain.Media> GetAllMedia(string idWebSession)
        {
            var result = new List<Core.Domain.Media>();
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                var vehiclesInfos = VehiclesInformation.GetAll();
                result = GetMyMedia(_webSession);
            }
             catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", idWebSession, _webSession.UserAgent, _webSession.CustomerLogin.Login, _webSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(_webSession.CurrentModule), _webSession.SiteLanguage));
                Logger.Log(LogLevel.Error, message);
            }
            //DetailLevelItemInformation.Levels.vehicle       
            return result;
        }

        private List<Core.Domain.Media> GetMyMedia(WebSession _webSession)
        {
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[CstWeb.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = _webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.
                CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, param, null, null);
            DataTable data = classficationDAL.GetMediaType().Tables[0];
            List<Core.Domain.Media> result = new List<Core.Domain.Media>();
            foreach (var item in data.AsEnumerable())
            {
                int id = int.Parse(item.ItemArray[0].ToString());
                if (VehiclesInformation.Contains(id))
                {
                    Core.Domain.Media media = new Core.Domain.Media();
                    media.Id = id;
                    media.Label = item.ItemArray[1].ToString();
                    media.MediaEnum = VehiclesInformation.DatabaseIdToEnum(id);
                    result.Add(media);
                }
            }
            return result;
        }


        private ClassificationLevelListDAL GetVehicleLabel(string idMedias, WebSession webSession, DetailLevelItemInformation detailLevelItemInformation)
        {
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classificationLevelList];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[2];
            param[0] = webSession.CustomerDataFilters.DataSource;
            param[1] = webSession.DataLanguage;
            TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory factoryLevels = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            //TNS.AdExpressI.Classification.DAL.ClassificationLevelListDALFactory cLevel = (ClassificationLevelListDALFactory)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

            return factoryLevels.CreateClassificationLevelListDAL(detailLevelItemInformation, idMedias);


            //return levels[idMedias];

        }

        private ControllerDetails GetCurrentControllerDetails(long currentModule)
        {
            long currentModuleCode = 0;
            string currentController = string.Empty;
            string currentModuleIcon = "icon-chart";
            switch (currentModule)
            {
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentModuleCode = CstWeb.LanguageConstantes.MediaScheduleCode;
                    currentController = SELECTION;
                    currentModuleIcon = "icon-chart";
                    break;
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = CstWeb.LanguageConstantes.PortfolioCode;
                    currentController = PORTFOLIO;
                    currentModuleIcon = "icon-layers";
                    break;
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = CstWeb.LanguageConstantes.LostWonCode;
                    currentController = LOSTWON;
                    currentModuleIcon = "icon-calculator";
                    break;
                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = CstWeb.LanguageConstantes.PresentAbsentCode;
                    currentController = PRESENTABSENT;
                    currentModuleIcon = "icon-equalizer";
                    break;
                case CstWeb.Module.Name.INDICATEUR:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalysisGraphics;
                    currentController = SELECTION;
                    break;
                case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = CstWeb.LanguageConstantes.AnalysisDetailedReport;
                    currentController = SELECTION;
                    currentModuleIcon = "icon-book-open";
                    break;
                case CstWeb.Module.Name.FACEBOOK:
                    currentModuleCode = CstWeb.LanguageConstantes.FacebookCode;
                    currentController = SELECTION;
                    currentModuleIcon = "icon-social-facebook";
                    break;
                case CstWeb.Module.Name.ANALYSE_MANDATAIRES:
                    currentModuleCode = CstWeb.LanguageConstantes.MediaAgencyAnalysis;
                    currentController = SELECTION;
                    currentModuleIcon = "icon-picture";
                    break;
                case CstWeb.Module.Name.NEW_CREATIVES:
                    currentModuleCode = CstWeb.LanguageConstantes.NewCreatives;
                    currentController =  SELECTION;
                    currentModuleIcon = "icon-picture";
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

        private MediaResponse GetDefaultVehicleList(WebSession webSession, MediaResponse mediaResponse)
        {
            //var response = new MediaResponse(webSession.SiteLanguage);
            var vehiclesInfos = VehiclesInformation.GetAll();
            var myMedia = GetMyMedia(webSession);
            string ids = vehiclesInfos.Select(p => p.Value.DatabaseId.ToString()).Aggregate((c, n) => c + "," + n);
            var levels = GetVehicleLabel(ids, webSession, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vehicle));
            VehicleInformation vehicleInfo = new VehicleInformation();
            vehicleInfo = VehiclesInformation.Get(VhCstes.plurimedia);
            webSession.SelectionUniversMedia.Nodes.Clear();
            webSession.PrincipalMediaUniverses.Clear();
            foreach (var item in vehiclesInfos.Values)
            {
                //Added temporarily for Finland
                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND) && vehicleInfo.DatabaseId == item.DatabaseId)
                    continue;

                Core.Domain.Media media = new Core.Domain.Media();
                //var label = GetVehicleLabel(item.DatabaseId.ToString(),_webSession, DetailLevelItemInformation.Levels.vehicle)
                media.Id = item.DatabaseId;
                media.MediaEnum = item.Id;
                media.Disabled = myMedia.FirstOrDefault(p => p.Id == media.Id) != null ? false : true;
                media.Label = levels[item.DatabaseId];
                mediaResponse.Media.Add(media);
            }
            return mediaResponse;
        }

        private MediaResponse GetAnalysisVehicleList(WebSession webSession, MediaResponse response)
        {
            #region Get Vehciles List
            //Data loading
            List<Core.Domain.Media> vehiclesList = new List<Core.Domain.Media>();
            DataTable dtVehicle = new DataTable();
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            DataSet ds = classficationDAL.GetRecapDetailMedia(false);

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                dtVehicle = ds.Tables[0];
            VehicleInformation vehicleInfo = new VehicleInformation();
            vehicleInfo = VehiclesInformation.Get(VhCstes.plurimedia);
            Core.Domain.Media pluremedia = new Core.Domain.Media();
            if (vehicleInfo != null)
            {
                pluremedia.Id = (long)VhCstes.plurimedia;
                pluremedia.MediaEnum = VehiclesInformation.DatabaseIdToEnum((long)VhCstes.plurimedia);
                pluremedia.Label = GestionWeb.GetWebWord(CstWeb.LanguageConstantes.Plurimedia, webSession.SiteLanguage);
                vehiclesList.Add(pluremedia);
            }
            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
                dtVehicle = ds.Tables[0];

            foreach (var item in dtVehicle.AsEnumerable())
            {
                int id = int.Parse(item.ItemArray[0].ToString());
                if (VehiclesInformation.Contains(id))
                {
                    Core.Domain.Media medium = new Core.Domain.Media();
                    medium.Id = id;
                    medium.Label = item.ItemArray[1].ToString();
                    medium.MediaEnum = VehiclesInformation.DatabaseIdToEnum(id);
                    vehiclesList.Add(medium);
                }
            }
            #endregion
            response.Media = vehiclesList;
            return response;
        }
    }
}

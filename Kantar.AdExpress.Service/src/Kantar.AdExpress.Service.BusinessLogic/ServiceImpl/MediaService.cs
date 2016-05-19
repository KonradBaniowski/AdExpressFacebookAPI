using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using CstWeb = TNS.AdExpress.Constantes.Web;
using System.Data;
using TNS.AdExpress.Domain.Classification;
using System.Linq;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain;
using System.Text;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaService : IMediaService
    {
        private WebSession _webSession = null;
        public MediaResponse GetMedia(string idWebSession)
        {
            
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var result = new MediaResponse(_webSession.SiteLanguage);
            result.ControllerDetails = GetCurrentControllerDetails(_webSession.CurrentModule);
            result.MediaCommon= Array.ConvertAll(Lists.GetIdList(CstWeb.GroupList.ID.media, CstWeb.GroupList.Type.mediaInSelectAll).Split(','), Convert.ToInt32).ToList();
            var vehiclesInfos = VehiclesInformation.GetAll();
            var myMedia = GetMyMedia(_webSession);
            string ids = vehiclesInfos.Select(p => p.Value.DatabaseId.ToString()).Aggregate((c,n)=>c+","+n);
            var levels = GetVehicleLabel(ids, _webSession, DetailLevelItemsInformation.Get(DetailLevelItemInformation.Levels.vehicle) );

            foreach ( var item in vehiclesInfos.Values)
            {
                Core.Domain.Media media = new Core.Domain.Media();
                //var label = GetVehicleLabel(item.DatabaseId.ToString(),_webSession, DetailLevelItemInformation.Levels.vehicle)
                media.Id = item.DatabaseId;
                media.MediaEnum = item.Id;
                media.Disabled = myMedia.FirstOrDefault(p=>p.Id== media.Id)!=null? false :true;
                media.Label = levels[item.DatabaseId];
                result.Media.Add(media);
             }

            return result;
        }

        public List<Core.Domain.Media> GetAllMedia(string idWebSession)
        {
            var result = new List<Core.Domain.Media>();
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var vehiclesInfos = VehiclesInformation.GetAll();
            var myMedia = GetMyMedia(_webSession);
            //DetailLevelItemInformation.Levels.vehicle       
            return myMedia;
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
            foreach ( var item in data.AsEnumerable())
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

        private ControllerDetails  GetCurrentControllerDetails(long currentModule)
        {
            long currentControllerCode = 0;
            string currentController = string.Empty;
            switch (currentModule)
            {
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentControllerCode = CstWeb.LanguageConstantes.MediaScheduleCode;
                    currentController = "MediaSchedule";
                    break;
                case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentControllerCode =CstWeb.LanguageConstantes.PortfolioCode;
                    currentController = "Portfolio";
                    break;
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                    currentControllerCode =CstWeb.LanguageConstantes.LostWonCode;
                    currentController = "LostWon";
                    break;
                case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentControllerCode =CstWeb.LanguageConstantes.PresentAbsentCode;
                    currentController = "PresentAbsent";
                    break;
                case CstWeb.Module.Name.INDICATEUR:
                    currentControllerCode =CstWeb.LanguageConstantes.AnalysisGraphics;
                    currentController = "Analysis";
                    break;
                case CstWeb.Module.Name.TABLEAU_DYNAMIQUE:
                    currentControllerCode =CstWeb.LanguageConstantes.AnalysisDetailedReport;
                    currentController = "Analysis";
                    break;
                default:
                    break;
            }
            var current = new ControllerDetails
            {
                ControllerCode = currentControllerCode,
                Name = currentController,
                ModuleId = currentModule
            };
            return current;
        }
    }
}

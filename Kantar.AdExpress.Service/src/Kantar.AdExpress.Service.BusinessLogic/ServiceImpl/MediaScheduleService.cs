using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System.Collections.Generic;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Classification.DAL;
using System.Reflection;
using TNS.AdExpress.Constantes.Web;
using System.Data;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MediaScheduleService : IMediaScheduleService
    {
        private WebSession _webSession = null;
        public List<Media> GetMedia(string idWebSession)
        {
            var result = new List<Media>();
            var _webSession = (WebSession)WebSession.Load(idWebSession);
            var myMedia =GetMyMedia(_webSession);

            return result;
        }

        private DataTable GetMyMedia(WebSession _webSession)
        {
            CoreLayer cl = TNS.AdExpress.Domain.Web.WebApplicationParameters.CoreLayers[Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = _webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.
                CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance
                | BindingFlags.Public, null, param, null, null);
            var data = classficationDAL.GetMediaType().Tables[0];
            return data;
        }
    }
}

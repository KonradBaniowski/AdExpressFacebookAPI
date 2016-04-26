using System;
using System.Reflection;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpress.Web.Core.DataAccess.Helpers
{
    public class Functions
    {
        public static FrameWork.DB.Common.IDataSource GetDataSource(WebSession webSession)
        {
            Domain.Layers.CoreLayer cl = Domain.Web.WebApplicationParameters.CoreLayers[Constantes.Web.Layers.Id.sourceProvider];
            object[] param = new object[1];
            param[0] = webSession;
            if (cl == null) throw (new NullReferenceException("Core layer is null for the source provider layer"));
            ISourceProvider sourceProvider = (SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            return sourceProvider.GetSource();
        }
    }
}

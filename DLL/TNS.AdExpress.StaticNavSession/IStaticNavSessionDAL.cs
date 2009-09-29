using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.FrameWork.DB.Common;
using System.Collections;

using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;

using AnubisConst = TNS.AdExpress.Anubis.Constantes;
using TNS.AdExpress.Domain.Classification;

namespace TNS.AdExpress.StaticNavSession
{
    public interface IStaticNavSessionDAL
    {
        DataTable Get();
        DataTable Get(int status);
        DataTable Get(int status, Dictionary<PluginType, PluginInformation> plugins);

        DataRow GetRow(long staticNavSession);

        DataTable GetExpired(Dictionary<PluginType, PluginInformation> plugins);
        void UpdateStatus(int staticNavSessionId, int status);
        void DeleteExpiredRequests(Dictionary<PluginType, PluginInformation> plugins, string filePath);
        void DeleteRow(int staticNavSession);

        int InsertData(WebSession webSession, AnubisConst.Result.type resultType, string fileName);
        void InsertData(AnubisConst.Result.type resultType, string fileName, int alertId, int loginId);
    }
}

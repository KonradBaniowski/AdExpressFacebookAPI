using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.FrameWork.DB.Common;
using System.Collections;
using TNS.Ares.Domain.LS;
using AresConst = TNS.Ares.Constantes.Constantes;

namespace TNS.Ares.StaticNavSession.DAL
{
    public interface IStaticNavSessionDAL
    {
        DataTable Get();
        DataTable Get(int status);
        DataTable Get(int status, Dictionary<PluginType, PluginInformation> plugins);

        DataSet GetData(Dictionary<PluginType, PluginInformation> pluginList, Int64 loginId);

        Object LoadData(Int64 idStaticNavSession);

        DataRow GetRow(long staticNavSession);

        DataTable GetExpired(Dictionary<PluginType, PluginInformation> plugins);
        void UpdateStatus(Int64 staticNavSessionId, int status);
        void DeleteExpiredRequests(Dictionary<PluginType, PluginInformation> plugins, string filePath);
        void DeleteRow(long staticNavSession);

        int InsertData(byte[] binaryData, Int64 loginId, int resultType, string fileName);
        int InsertData(byte[] binaryData, Int64 loginId, int resultType, string fileName, object option);
        void InsertData(int resultType, string fileName, int alertId, Int64 loginId);
        void InsertData(int resultType, string fileName, int alertId, Int64 loginId, DateTime dateExec);

        void RegisterFile(Int64 idStaticNavSession, string fileName);
    }
}

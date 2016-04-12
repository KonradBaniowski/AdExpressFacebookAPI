using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using KM.AdExpressI.MyAdExpress;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MyAdExpressService : IMyAdExpressService
    {
        public string MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId)
        {
            var result= string.Empty;
            try {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idOldDirectory) && !string.IsNullOrEmpty(idNewDirectory) && !string.IsNullOrEmpty(idNewDirectory))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    bool success = MyResultsDAL.MoveSession(Int64.Parse(idOldDirectory), Int64.Parse(idNewDirectory), Int64.Parse(id), webSession);
                }
                else
                {
                    result = "Would you please select a destination directory.";
                }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string RenameUnivers(string name, string universId, string webSessionId)
        {
            throw new NotImplementedException();
        }
    }
}

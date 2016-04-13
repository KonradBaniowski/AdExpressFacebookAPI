using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using KM.AdExpressI.MyAdExpress;
using UniversDAL = TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Domain.Translation;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MyAdExpressService : IMyAdExpressService
    {
        public string MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId)
        {
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idOldDirectory) && !string.IsNullOrEmpty(idNewDirectory) && !string.IsNullOrEmpty(idNewDirectory))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    bool success = MyResultsDAL.MoveSession(Int64.Parse(idOldDirectory), Int64.Parse(idNewDirectory), Int64.Parse(id), webSession);
                    result = "Success";
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

        public string RenameSession(string name, string universId, string webSessionId)
        {
            var result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(universId) && !string.IsNullOrEmpty(webSessionId))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    bool success = MyResultsDAL.RenameSession(name, Int64.Parse(universId), webSession);
                    result = "Success";
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
            var result = string.Empty;
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                if (!String.IsNullOrEmpty(universId))
                {

                    if (name.Length != 0 && name.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                    {
                        Int64 idUnivers = Int64.Parse(universId);
                        if (!UniversDAL.UniversListDataAccess.IsUniverseExist(webSession, name))
                        {
                            UniversDAL.UniversListDataAccess.RenameUniverse(name, idUnivers, webSession);
                            result = "Success";
                        }
                        else {
                            //Error : L'univers already exists
                            return result= GestionWeb.GetWebWord(1101, webSession.SiteLanguage);
                        }
                    }
                    else if (name.Length == 0)
                    {
                        // Error : Empty name field
                        return result = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                    }
                    else {
                        // Error : max field length exceeded
                        return result= GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    }

                }
                else {
                    // Error : Select at least an element
                    return result = GestionWeb.GetWebWord(926, webSession.SiteLanguage);
                }
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string MoveUnivers(string id, string idOldGroupUnivers, string idNewGroupUnivers, string webSessionId)
        {
            var result = string.Empty;
            var webSession = (WebSession)WebSession.Load(webSessionId);
                     
            try
            {

                    if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(idOldGroupUnivers) && !String.IsNullOrEmpty(idNewGroupUnivers) && !String.IsNullOrEmpty(webSessionId))
                    {
                        UniversDAL.UniversListDataAccess.MoveUniverse(Int64.Parse(idOldGroupUnivers), Int64.Parse(idNewGroupUnivers), Int64.Parse(id), webSession);
                        result = "Success";
                    }
                    else if (String.IsNullOrEmpty(id))
                    {
                        //Erreur : Aucun univers n'a été sélectionné
                        result = GestionWeb.GetWebWord(926, webSession.SiteLanguage);
                    }
            }
            catch (Exception ex)
            {
                result = ex.Message;
            }
            return result;
        }

        public string RenameUniversDirectory(string name, string universId, string webSessionId)
        {
            var result = string.Empty;
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(universId) && !string.IsNullOrEmpty(webSessionId))
                {
                    webSession = (WebSession)WebSession.Load(webSessionId);
                    if (name.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                    {
                        return result = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    }
                    if (!UniversDAL.UniversListDataAccess.IsGroupUniverseExist(webSession, name))
                    {
                        UniversDAL.UniversListDataAccess.RenameGroupUniverse(name, Int64.Parse(universId), webSession);
                        result= GestionWeb.GetWebWord(934, webSession.SiteLanguage); 
                    }
                    else
                    {
                        return result = GestionWeb.GetWebWord(928, webSession.SiteLanguage);
                    }
                }
                else
                {
                    return result = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                }
            }
            catch (System.Exception ex)
            {
                result = ex.Message;
                //this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));

            }
            return result;
        }
    }
}

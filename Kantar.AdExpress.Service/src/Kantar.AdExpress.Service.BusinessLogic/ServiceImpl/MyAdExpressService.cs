using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using KM.AdExpressI.MyAdExpress;
using UniversDAL = TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Domain.Translation;
using Kantar.AdExpress.Service.Core.Domain;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class MyAdExpressService : IMyAdExpressService
    {
        public AdExpressResponse MoveSession(string id, string idOldDirectory, string idNewDirectory, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            try
            {
                if (!string.IsNullOrEmpty(id) && !string.IsNullOrEmpty(idOldDirectory) && !string.IsNullOrEmpty(idNewDirectory) && !string.IsNullOrEmpty(idNewDirectory))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    result.Success = MyResultsDAL.MoveSession(Int64.Parse(idOldDirectory), Int64.Parse(idNewDirectory), Int64.Parse(id), webSession);
                    result.Message = "Success";
                    
                }
                else
                {
                    result.Message = "Would you please select a destination directory.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public AdExpressResponse RenameSession(string name, string universId, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(universId) && !string.IsNullOrEmpty(webSessionId))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    result.Success = MyResultsDAL.RenameSession(name, Int64.Parse(universId), webSession);
                    result.Message = "Success";
                }
                else
                {
                    result.Message = "Would you please select a destination directory.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public AdExpressResponse DeleteSession(string id, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            try
            {
                if (!string.IsNullOrEmpty(id))
                {
                    var webSession = (WebSession)WebSession.Load(webSessionId);
                    result.Success = MyResultsDAL.DeleteSession(Int64.Parse(id), webSession);
                    result.Message = "Success";

                }
                else
                {
                    result.Message = "Would you please select a session to delete.";
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public AdExpressResponse RenameUnivers(string name, string universId, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
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
                            result.Message = "Success";
                            result.Success = true;
                        }
                        else {
                            //Error : L'univers already exists
                             result.Message= GestionWeb.GetWebWord(1101, webSession.SiteLanguage);
                        }
                    }
                    else if (name.Length == 0)
                    {
                        // Error : Empty name field
                        result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                    }
                    else {
                        // Error : max field length exceeded
                         result.Message= GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    }

                }
                else {
                    // Error : Select at least an element
                    result.Message = GestionWeb.GetWebWord(926, webSession.SiteLanguage);
                }
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public AdExpressResponse MoveUnivers(string id, string idOldGroupUnivers, string idNewGroupUnivers, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
                     
            try
            {

                    if (!String.IsNullOrEmpty(id) && !String.IsNullOrEmpty(idOldGroupUnivers) && !String.IsNullOrEmpty(idNewGroupUnivers) && !String.IsNullOrEmpty(webSessionId))
                    {
                        UniversDAL.UniversListDataAccess.MoveUniverse(Int64.Parse(idOldGroupUnivers), Int64.Parse(idNewGroupUnivers), Int64.Parse(id), webSession);
                        result.Message = "Success";
                        result.Success = true;
                    }
                    else if (String.IsNullOrEmpty(id))
                    {
                        result.Message = GestionWeb.GetWebWord(926, webSession.SiteLanguage);
                    }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }

        public AdExpressResponse DeleteUnivers (string id, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                if (!String.IsNullOrEmpty(id))
                {

                    if (UniversDAL.UniversListDataAccess.DropUniverse(Int64.Parse(id), webSession))
                    {
                        result.Message = GestionWeb.GetWebWord(937, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = GestionWeb.GetWebWord(830, webSession.SiteLanguage);
                    }
                }
                else
                {
                    result.Message = GestionWeb.GetWebWord(831, webSession.SiteLanguage);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public AdExpressResponse RenameUniversDirectory(string name, string universId, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(universId) && !string.IsNullOrEmpty(webSessionId))
                {
                    webSession = (WebSession)WebSession.Load(webSessionId);
                    if (name.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                    {
                        result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    }
                    if (!UniversDAL.UniversListDataAccess.IsGroupUniverseExist(webSession, name))
                    {
                        UniversDAL.UniversListDataAccess.RenameGroupUniverse(name, Int64.Parse(universId), webSession);
                        result.Message= GestionWeb.GetWebWord(934, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = GestionWeb.GetWebWord(928, webSession.SiteLanguage);
                    }
                }
                else
                {
                     result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                }
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
                //this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));

            }
            return result;
        }

        
    }
}

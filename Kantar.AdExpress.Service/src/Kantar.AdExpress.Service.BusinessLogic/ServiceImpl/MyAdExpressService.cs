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
                            result.Message = GestionWeb.GetWebWord(1101, webSession.SiteLanguage);
                        }
                    }
                    else if (name.Length == 0)
                    {
                        // Error : Empty name field
                        result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                    }
                    else {
                        // Error : max field length exceeded
                        result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
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

        public AdExpressResponse DeleteUnivers(string id, string webSessionId)
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

        public AdExpressResponse CreateDirectory(string directoryName, UniversType type, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                switch (type)
                {
                    case UniversType.Result:
                        result = CreateSessionDirectory(directoryName, webSession);
                        break;
                    case UniversType.Univers:
                        result = CreateUniversDirectory(directoryName, webSession);
                        break;
                };
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        public AdExpressResponse RenameDirectory(string directoryName, UniversType type, string idDirectory, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                switch (type)
                {
                    case UniversType.Result:
                        result = RenameSessionDirectory(directoryName, idDirectory, webSession);
                        break;
                    case UniversType.Univers:
                        result = RenameUniversDirectory(directoryName, idDirectory, webSession);
                        break;
                };
            }
            catch (System.Exception exc)
            {
                //if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                //{
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                //}
            }
            return result;
        }
        public AdExpressResponse DropDirectory(string idDirectory, UniversType type, string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                switch (type)
                {
                    case UniversType.Result:
                        result = DropSessionDirectory(idDirectory, webSession);
                        break;
                    case UniversType.Univers:
                        result = DropUniversDirectory(idDirectory, webSession);
                        break;
                };
            }
            catch (System.Exception exc)
            {
                //if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                //{
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                //}
            }
            return result;
        }

        public bool LoadSession(string idSession, UniversType type ,string webSessionId)
        {
            var result = false;
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                switch (type)
                {
                    case UniversType.Result:
                        result = LoadSessionInfo(idSession, webSession);
                        break;
                    case UniversType.Univers:
                        result = LoadUniversInfo(idSession, webSession);
                        break;
                };
            }
            catch (System.Exception exc)
            {
                //if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                //{
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));
                //}
            }
            return result;
        }
        #region Private methods
        #region Handling session directories
        private AdExpressResponse CreateSessionDirectory(string directoryName, WebSession webSession)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            if (!String.IsNullOrEmpty(directoryName) && directoryName.Length != 0 && directoryName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
            {
                if (!MyResultsDAL.IsDirectoryExist(webSession, directoryName))
                {
                    if (MyResultsDAL.CreateDirectory(directoryName, webSession))
                    {
                        result.Message = GestionWeb.GetWebWord(835, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = GestionWeb.GetWebWord(836, webSession.SiteLanguage);
                    }
                }
                else
                {
                    result.Message = GestionWeb.GetWebWord(834, webSession.SiteLanguage);
                }
            }
            else if (directoryName.Length == 0)
            {
                result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
            }
            else
            {
                result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
            }
            return result;
        }
        private AdExpressResponse RenameSessionDirectory(string directoryName, string idDirectory, WebSession webSession)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            if (!String.IsNullOrEmpty(directoryName) && directoryName.Length != 0 && directoryName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
            {
                if (!MyResultsDAL.IsDirectoryExist(webSession, directoryName))
                {
                    if (MyResultsDAL.RenameDirectory(directoryName, Int64.Parse(idDirectory), webSession))
                    {
                        result.Message = GestionWeb.GetWebWord(835, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = GestionWeb.GetWebWord(836, webSession.SiteLanguage);
                    }
                }
                else
                {
                    result.Message = GestionWeb.GetWebWord(834, webSession.SiteLanguage);
                }
            }
            else if (directoryName.Length == 0)
            {
                result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
            }
            else
            {
                result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
            }
            return result;
        }
        private AdExpressResponse DropSessionDirectory(string idDirectory, WebSession webSession)
        {
            var result = new AdExpressResponse

            {
                Message = string.Empty
            };
            try
            {
                if (!String.IsNullOrEmpty(idDirectory))
                {
                    if (MyResultsDAL.ContainsDirectories(webSession))
                    {
                        if (!MyResultsDAL.IsSessionsInDirectoryExist(webSession, Int64.Parse(idDirectory)))
                        {
                            MyResultsDAL.DropDirectory(Int64.Parse(idDirectory), webSession);
                            result.Success = true;
                            result.Message = "Success";
                        }
                        else {
                            result.Message = GestionWeb.GetWebWord(838, webSession.SiteLanguage);
                        }
                    }
                    else {
                        //Directory can't be deleted
                        result.Message = GestionWeb.GetWebWord(840, webSession.SiteLanguage);
                    }
                }
                else {
                    //No directory has been found
                    result.Message = GestionWeb.GetWebWord(839, webSession.SiteLanguage);
                }
            }
            catch (Exception ex)
            {
                result.Message = ex.Message;
            }
            return result;
        }
        #endregion
        #region Handling univers directories 
        private AdExpressResponse CreateUniversDirectory(string directoryName, WebSession webSession)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            if (!String.IsNullOrEmpty(directoryName) && directoryName.Length != 0 && directoryName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
            {
                if (!UniversDAL.UniversListDataAccess.IsGroupUniverseExist(webSession, directoryName))
                {
                    if (UniversDAL.UniversListDataAccess.CreateGroupUniverse(directoryName, webSession))
                    {
                        result.Message = GestionWeb.GetWebWord(835, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        result.Message = GestionWeb.GetWebWord(836, webSession.SiteLanguage);
                    }
                }
                else
                {
                    result.Message = GestionWeb.GetWebWord(834, webSession.SiteLanguage);
                }
            }
            else if (directoryName.Length == 0)
            {
                result.Message = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
            }
            else
            {
                result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
            }
            return result;
        }
        private AdExpressResponse RenameUniversDirectory(string name, string idDirectory, WebSession webSession)
        {
            var result = new AdExpressResponse
            {
                Message = string.Empty
            };
            try
            {
                if (!string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(idDirectory))
                {
                    if (name.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                    {
                        result.Message = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    }
                    if (!UniversDAL.UniversListDataAccess.IsGroupUniverseExist(webSession, name))
                    {
                        UniversDAL.UniversListDataAccess.RenameGroupUniverse(name, Int64.Parse(idDirectory), webSession);
                        result.Message = GestionWeb.GetWebWord(934, webSession.SiteLanguage);
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
        private AdExpressResponse DropUniversDirectory(string idDirectory, WebSession webSession)
        {
            var result = new AdExpressResponse

            {
                Message = string.Empty
            };
            try
            {
                if (!String.IsNullOrEmpty(idDirectory))
                    if (UniversDAL.UniversListDataAccess.GetGroupUniverses(webSession).Tables[0].Rows.Count > 1)
                    {
                        if (!UniversDAL.UniversListDataAccess.IsUniversInGroupUniverseExist(webSession, Int64.Parse(idDirectory)))
                        {
                            UniversDAL.UniversListDataAccess.DropGroupUniverse(Int64.Parse(idDirectory), webSession);
                            result.Success = true;
                            result.Message = "Success";
                        }
                        else
                        {
                            // Directory is not empty                            
                            result.Message = GestionWeb.GetWebWord(931, webSession.SiteLanguage);
                        }
                    }
                    else
                    {
                        // Directory can't be dropped                           
                        result.Message = GestionWeb.GetWebWord(929, webSession.SiteLanguage);
                    }
                else
                {
                    // No Directory has been found                          
                    result.Message = GestionWeb.GetWebWord(927, webSession.SiteLanguage);
                }
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;

            }
            return result;
        }
        #endregion
        #region Set Websession 
        private bool LoadSessionInfo(string idSession, WebSession webSession)
        {
            var result = false;
            return result;
        }
        private bool LoadUniversInfo(string idSession, WebSession webSession)
        {
            var result = false;
            return result;
        }
        #endregion
        #endregion
    }
}

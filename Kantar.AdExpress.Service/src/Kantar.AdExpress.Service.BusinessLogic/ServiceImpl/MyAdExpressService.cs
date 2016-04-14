using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using TNS.AdExpress.Web.Core.Sessions;
using KM.AdExpressI.MyAdExpress;
using UniversDAL = TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using TNS.AdExpress.Domain.Translation;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Web;
using System.Data;
using TNS.AdExpress.Constantes.DB;
using System.Collections;
using System.Collections.Generic;
using CstCustomerSession = TNS.AdExpress.Constantes.Web.CustomerSessions;
using TNS.AdExpress.Web.Core;
using DBClassifConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpressI.Date;
using System.Reflection;
//using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Utilities;

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
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession));
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
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession));
                //}
            }
            return result;
        }

        public AdExpressResponse LoadSession(string idSession, UniversType type ,string webSessionId)
        {
            var result = new AdExpressResponse
            {
                Message= String.Empty
            };
            var webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                switch (type)
                {
                    case UniversType.Result:
                        result = LoadSessionInfo(idSession, webSession);
                        break;
                };
            }
            catch (System.Exception exc)
            {
                //if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                //{
                //    this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession));
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
                //this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, webSession));

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
        private AdExpressResponse LoadSessionInfo(string idSession, WebSession webSession)
        {
            var result = new AdExpressResponse
            {
                Message= String.Empty
            };
            try
            {
                WebSession webSessionSave;
                AtomicPeriodWeek tmp;
                bool validModule = false;
                bool notValidPeriod = false;
                //TNS.FrameWork.DB.Common.IDataSource source = new TNS.FrameWork.DB.Common.InternationalOracleDataSource("User Id=" + webSession.CustomerLogin.Login + "; Password=" + webSession.CustomerLogin.PassWord + " " + TNS.AdExpress.Constantes.DB.Connection.RIGHT_CONNECTION_STRING);
                TNS.AdExpress.Right right = new TNS.AdExpress.Right(webSession.CustomerLogin.Login, webSession.CustomerLogin.PassWord, webSession.SiteLanguage);
                string PeriodBeginningDate = "";
                string PeriodEndDate = "";
                string invalidPeriodMessage = "";
                DateTime tmpEndDate;
                DateTime tmpBeginDate;
                DateTime lastDayEnable = DateTime.Now;
                DateTime FirstDayNotEnable = DateTime.Now;
                globalCalendar.comparativePeriodType comparativePeriodType;
                globalCalendar.periodDisponibilityType periodDisponibilityType;
                bool verifCustomerPeriod = false;
                bool validResultPage = true;

                CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = webSession;
                IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                Int64 idMySession = 0;

                if (!String.IsNullOrEmpty(idSession))
                {
                    idMySession = Int64.Parse(idSession);
                    webSessionSave = (WebSession)MyResultsDAL.GetResultMySession(idMySession.ToString(), webSession);

                    DataTable dtModulesList = right.GetCustomerModuleListHierarchy();

                    #region Vérification des droits sur les modules
                    foreach (DataRow currentRow in dtModulesList.Rows)
                    {
                        if ((Int64)currentRow["idModule"] == webSessionSave.CurrentModule)
                        {
                            validModule = true;
                        }
                        //Verifie droit accès resultat courant
                        TNS.AdExpress.Domain.Web.Navigation.Module module = right.GetModule(webSessionSave.CurrentModule);
                        if (module != null)
                        {
                            validResultPage = (module.GetResultPageInformation(Convert.ToInt32(webSessionSave.CurrentTab)) != null);
                        }
                    }
                    #endregion

                    //Patch page de résultats Tableaux dynamiques
                    if (webSessionSave != null && webSessionSave.LastReachedResultUrl.Length > 0 && webSessionSave.LastReachedResultUrl.IndexOf("ASDynamicTables.aspx") >= 0)
                    {
                        webSessionSave.LastReachedResultUrl = webSessionSave.LastReachedResultUrl.Replace("ASDynamicTables.aspx", "ProductClassReport.aspx");
                    }

                    #region Vérification des flags produit pour le niveau de détail produit					
                    if ((!webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_HOLDING_COMPANY) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("holdingcompany") >= 0)) ||
                        (!webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_MARQUE) && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("brand") >= 0))
                        || (!webSession.CustomerLogin.HasAtLeastOneMediaAgencyFlag() && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency") >= 0))
                        )
                    {
                        webSession.PreformatedProductDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser;
                    }
                    else {
                        webSession.PreformatedProductDetail = webSessionSave.PreformatedProductDetail;
                    }


                    #endregion

                    #region Vérification des flags produit pour le niveau de détail support
                    if ((!webSession.CustomerLogin.CustormerFlagAccess(Flags.ID_SLOGAN_ACCESS_FLAG) && (webSessionSave.PreformatedMediaDetail.ToString().ToLower().IndexOf("slogan") >= 0))
                        )
                    {
                        webSession.PreformatedMediaDetail = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
                    }
                    else {
                        webSession.PreformatedMediaDetail = webSessionSave.PreformatedMediaDetail;
                    }
                    #endregion

                    #region Paramètres
                    webSession.UserParameters = webSessionSave.UserParameters;
                    #endregion

                    #region Niveau de détail media (Generic)
                    try
                    {
                        if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE ||
                           webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE)
                        {
                            ArrayList levels = new ArrayList();
                            levels.Add(1);
                            levels.Add(2);
                            levels.Add(3);
                            webSession.GenericMediaDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        }
                        if (webSessionSave.GenericMediaDetailLevel == null)
                        {
                            ArrayList levels = new ArrayList();
                            levels.Add(1);
                            levels.Add(2);
                            webSession.GenericMediaDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

                        }
                    }
                    catch (System.Exception)
                    {
                        ArrayList levels = new ArrayList();
                        levels.Add(1);
                        levels.Add(2);
                        webSession.GenericMediaDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    }
                    #endregion

                    #region Niveau de détail produit (Generic)
                    try
                    {
                        if (webSessionSave.GenericProductDetailLevel == null)
                        {
                            ArrayList levels = PopulateGenericProductDetailLevel(webSessionSave);
                            webSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels);
                        }
                    }
                    catch (System.NotImplementedException)
                    {
                        ArrayList levels = PopulateGenericProductDetailLevel(webSessionSave);
                        webSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.customLevels);
                    }
                    catch (System.Exception)
                    {
                        ArrayList levels = new ArrayList();
                        levels.Add(8);
                        webSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    }
                    #endregion

                    #region Niveau de détail media AdnetTrack (Generic)
                    try
                    {
                        if (webSessionSave.GenericAdNetTrackDetailLevel == null)
                        {

                            // Initialisation à media\catégorie
                            ArrayList levels = new ArrayList();
                            levels.Add(1);
                            levels.Add(2);
                            webSession.GenericMediaDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

                        }
                    }
                    catch (System.Exception)
                    {
                        ArrayList levels = new ArrayList();
                        levels.Add(1);
                        levels.Add(2);
                        webSession.GenericAdNetTrackDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                    }
                    #endregion

                    #region Niveau de détail colonne (Generic)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE ||
                           webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                    {
                        try
                        {

                            if (webSessionSave.GenericColumnDetailLevel == null)
                            {
                                ArrayList levels = new ArrayList();
                                levels.Add(3);
                                webSession.GenericColumnDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

                            }

                        }
                        catch (System.Exception)
                        {
                            ArrayList levels = new ArrayList();
                            levels.Add(3);
                            webSession.GenericColumnDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levels, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);
                        }
                    }
                    #endregion

                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS && (webSessionSave.SelectionUniversMedia.FirstNode == null || webSessionSave.SelectionUniversMedia.FirstNode.Tag == null))
                    {
                        webSession.SelectionUniversMedia.Nodes.Clear();
                        System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("TELEVISION");
                        tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, VehiclesInformation.EnumToDatabaseId(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.tv), "TELEVISION");
                        webSessionSave.SelectionUniversMedia.Nodes.Add(tmpNode);
                    }

                    webSession.CompetitorUniversAdvertiser = webSessionSave.CompetitorUniversAdvertiser;
                    webSession.CompetitorUniversMedia = webSessionSave.CompetitorUniversMedia;
                    webSession.CompetitorUniversProduct = webSessionSave.CompetitorUniversProduct;
                    webSession.CurrentModule = webSessionSave.CurrentModule;
                    webSession.CurrentTab = webSessionSave.CurrentTab;
                    webSession.SelectionUniversAdvertiser = webSessionSave.SelectionUniversAdvertiser;
                    webSession.SelectionUniversMedia = webSessionSave.SelectionUniversMedia;
                    webSession.SelectionUniversProduct = webSessionSave.SelectionUniversProduct;
                    webSession.CurrentUniversAdvertiser = webSessionSave.CurrentUniversAdvertiser;
                    webSession.CurrentUniversMedia = webSessionSave.CurrentUniversMedia;
                    webSession.CurrentUniversProduct = webSessionSave.CurrentUniversProduct;

                    webSession.DetailPeriod = webSessionSave.DetailPeriod;


                    webSession.Percentage = webSessionSave.Percentage;
                    webSession.Insert = webSessionSave.Insert;


                    webSession.PeriodLength = webSessionSave.PeriodLength;
                    webSession.PeriodType = webSessionSave.PeriodType;



                    #region Période sélectionnée (GlobalDateSelection)
                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES
                          || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.CELEBRITIES)
                    {

                        int oldYear = 2000;
                        long selectedVehicle = ((LevelInformation)webSessionSave.SelectionUniversMedia.FirstNode.Tag).ID;
                        if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                            //FirstDayNotEnable = Dates.GetFirstDayNotEnabled(webSessionSave, selectedVehicle, oldYear,webSession.Source); 
                            FirstDayNotEnable = dateDAL.GetFirstDayNotEnabled(new List<Int64>(new Int64[] { selectedVehicle }), oldYear);

                        switch (webSessionSave.DetailPeriod)
                        {
                            case CstCustomerSession.Period.DisplayLevel.monthly:
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.currentYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastMonth &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousMonth)
                                {
                                    if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate)
                                    {
                                        string startYearMonth = webSessionSave.PeriodBeginningDate;
                                        string endYearMonth = webSessionSave.PeriodEndDate;
                                        DateTime firstDayOfMonth = new DateTime(int.Parse(endYearMonth.ToString().Substring(0, 4)), int.Parse(endYearMonth.ToString().Substring(4, 2)), 1);
                                        Int32 lastDayOfMonth = ((firstDayOfMonth.AddMonths(1)).AddDays(-1)).Day;
                                        webSession.PeriodBeginningDate = startYearMonth + "01";
                                        webSession.PeriodEndDate = endYearMonth + lastDayOfMonth;
                                    }
                                    else {
                                        webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                        webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                    }
                                    webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                                }
                                break;
                            case CstCustomerSession.Period.DisplayLevel.weekly:
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastWeek &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousWeek)
                                {
                                    if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.dateToDate)
                                    {
                                        AtomicPeriodWeek startWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodBeginningDate.ToString().Substring(4, 2)));
                                        AtomicPeriodWeek endWeek = new AtomicPeriodWeek(int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(0, 4)), int.Parse(webSessionSave.PeriodEndDate.ToString().Substring(4, 2)));
                                        DateTime dateBegin = startWeek.FirstDay;
                                        DateTime dateEnd = endWeek.FirstDay.AddDays(6);
                                        webSession.PeriodBeginningDate = dateBegin.Year.ToString() + dateBegin.Month.ToString("00") + dateBegin.Day.ToString("00");
                                        webSession.PeriodEndDate = dateEnd.Year.ToString() + dateEnd.Month.ToString("00") + dateEnd.Day.ToString("00");
                                    }
                                    else {
                                        webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                        webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                    }
                                    webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                                }
                                break;
                            default:
                                webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                webSession.PeriodType = CstCustomerSession.Period.Type.dateToDate;
                                break;
                        }

                        switch (webSessionSave.DetailPeriod)
                        {
                            case CstCustomerSession.Period.DisplayLevel.monthly:
                            case CstCustomerSession.Period.DisplayLevel.weekly:
                            case CstCustomerSession.Period.DisplayLevel.dayly:
                                if (webSessionSave.PeriodType != CstCustomerSession.Period.Type.currentYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousYear &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastMonth &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousMonth &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastWeek &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousWeek &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.nLastDays &&
                                    webSessionSave.PeriodType != CstCustomerSession.Period.Type.previousDay)
                                {

                                    tmpEndDate = new DateTime(Convert.ToInt32(webSession.PeriodEndDate.Substring(0, 4)), Convert.ToInt32(webSession.PeriodEndDate.Substring(4, 2)), Convert.ToInt32(webSession.PeriodEndDate.Substring(6, 2)));
                                    tmpBeginDate = new DateTime(Convert.ToInt32(webSession.PeriodBeginningDate.Substring(0, 4)), Convert.ToInt32(webSession.PeriodBeginningDate.Substring(4, 2)), Convert.ToInt32(webSession.PeriodBeginningDate.Substring(6, 2)));

                                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                                    {

                                        if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.monthly)
                                        {
                                            comparativePeriodType = globalCalendar.comparativePeriodType.dateToDate;
                                            periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                        }
                                        else if (webSessionSave.DetailPeriod == CstCustomerSession.Period.DisplayLevel.weekly)
                                        {
                                            comparativePeriodType = globalCalendar.comparativePeriodType.comparativeWeekDate;
                                            periodDisponibilityType = globalCalendar.periodDisponibilityType.lastCompletePeriod;
                                        }
                                        else {
                                            comparativePeriodType = webSessionSave.CustomerPeriodSelected.ComparativePeriodType;
                                            periodDisponibilityType = webSessionSave.CustomerPeriodSelected.PeriodDisponibilityType;
                                        }

                                        if (WebApplicationParameters.UseComparativeLostWon
                                            && webSessionSave.CustomerPeriodSelected != null
                                            && webSessionSave.CustomerPeriodSelected.WithComparativePeriodPersonnalized)
                                        {
                                            webSession.CustomerPeriodSelected = new CustomerPeriod(webSessionSave.PeriodBeginningDate, webSessionSave.PeriodEndDate, webSessionSave.CustomerPeriodSelected.ComparativeStartDate, webSessionSave.CustomerPeriodSelected.ComparativeEndDate);
                                        }
                                        else {

                                            switch (periodDisponibilityType)
                                            {

                                                case globalCalendar.periodDisponibilityType.currentDay:
                                                    lastDayEnable = DateTime.Now;
                                                    break;
                                                case globalCalendar.periodDisponibilityType.lastCompletePeriod:
                                                    lastDayEnable = FirstDayNotEnable.AddDays(-1);
                                                    break;

                                            }

                                            if (CompareDateEnd(lastDayEnable, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                                webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate, true, comparativePeriodType, periodDisponibilityType);
                                            else
                                                webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, lastDayEnable.ToString("yyyyMMdd"), true, comparativePeriodType, periodDisponibilityType);
                                        }
                                    }
                                    else {
                                        if (CompareDateEnd(DateTime.Now, tmpEndDate) || CompareDateEnd(tmpBeginDate, DateTime.Now))
                                            webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, webSession.PeriodEndDate);
                                        else
                                            webSession.CustomerPeriodSelected = new CustomerPeriod(webSession.PeriodBeginningDate, DateTime.Now.ToString("yyyyMMdd"));
                                    }
                                    verifCustomerPeriod = true;
                                }
                                break;
                        }
                    }
                    #endregion


                    if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR
                        || webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE)
                    {

                        long levelInfoId = ((LevelInformation)webSession.SelectionUniversMedia.Nodes[0].Tag).ID;
                        webSession.LastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(levelInfoId);

                        if (WebApplicationParameters.CountryCode.Equals(CountryCode.FRANCE)
                            && VehiclesInformation.Get(DBClassifConstantes.Vehicles.names.plurimedia).DatabaseId == levelInfoId)
                        {
                            string mmsLastAvailableRecapMonth = dateDAL.CheckAvailableDateForMedia(VehiclesInformation.EnumToDatabaseId(DBClassifConstantes.Vehicles.names.mms));
                            if (Convert.ToInt64(mmsLastAvailableRecapMonth) < Convert.ToInt64(webSession.LastAvailableRecapMonth))
                                webSession.LastAvailableRecapMonth = mmsLastAvailableRecapMonth;
                        }

                    }

                    if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES
                        || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.CELEBRITIES)
                    {
                        if (!verifCustomerPeriod)
                            UpdateGlobalDates(webSessionSave.PeriodType, webSessionSave, FirstDayNotEnable, webSession);
                    }
                    else if (!Modules.IsDashBoardModule(webSessionSave) && webSessionSave.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                    {
                        switch (webSessionSave.PeriodType)
                        {

                            case CstCustomerSession.Period.Type.nLastMonth:

                                webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-(webSessionSave.PeriodLength - 1)).ToString("yyyyMM");
                                webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");

                                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                                {
                                    UpdateRecapDates(CstCustomerSession.Period.Type.nLastMonth, ref notValidPeriod, ref invalidPeriodMessage, webSession);
                                }
                                break;

                            case CstCustomerSession.Period.Type.currentYear:

                                webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
                                webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");

                                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                                {
                                    UpdateRecapDates(CstCustomerSession.Period.Type.currentYear, ref notValidPeriod, ref invalidPeriodMessage, webSession);
                                }
                                break;

                            case CstCustomerSession.Period.Type.nLastYear:
                                webSession.PeriodBeginningDate = DateTime.Now.AddYears(1 - webSessionSave.PeriodLength).ToString("yyyy01");
                                webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMM");
                                break;

                            case CstCustomerSession.Period.Type.previousMonth:
                                webSession.PeriodEndDate = webSession.PeriodBeginningDate = DateTime.Now.AddMonths(-1).ToString("yyyyMM");
                                break;

                            case CstCustomerSession.Period.Type.previousYear:

                               if ((webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                                    && (DateTime.Now.AddYears(-1).Year == webSession.DownLoadDate)
                                    )
                                {
                                    webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy") + "12";
                                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy") + "01";
                                }
                                else {
                                    webSession.PeriodEndDate = DateTime.Now.AddYears(-1).ToString("yyyy") + "12";
                                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-1).ToString("yyyy") + "01";

                                }
                                break;

                            case CstCustomerSession.Period.Type.nextToLastYear:
                                if ((webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                                    && (DateTime.Now.AddYears(-1).Year == webSession.DownLoadDate)
                                    )
                                {
                                    webSession.PeriodEndDate = DateTime.Now.AddYears(-3).ToString("yyyy") + "12";
                                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-3).ToString("yyyy") + "01";
                                }
                                else {
                                    webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy") + "12";
                                    webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy") + "01";

                                }
                                break;

                            case CstCustomerSession.Period.Type.dateToDateMonth:

                                webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                webSession.PeriodEndDate = webSessionSave.PeriodEndDate;

                                if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE || webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
                                {
                                    UpdateRecapDates(CstCustomerSession.Period.Type.dateToDateMonth, ref notValidPeriod, ref invalidPeriodMessage, webSession);
                                }
                                break;
                            case CstCustomerSession.Period.Type.dateToDateWeek:
                                webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                break;

                            case CstCustomerSession.Period.Type.nLastWeek:
                                tmp = new AtomicPeriodWeek(DateTime.Now);
                                if (tmp.Week < 10)
                                {
                                    webSession.PeriodEndDate = tmp.Year.ToString() + "0" + tmp.Week.ToString();
                                }
                                else {
                                    webSession.PeriodEndDate = tmp.Year.ToString() + tmp.Week.ToString();
                                }
                                tmp.SubWeek(webSessionSave.PeriodLength - 1);
                                if (tmp.Week < 10)
                                {
                                    webSession.PeriodBeginningDate = tmp.Year.ToString() + "0" + tmp.Week.ToString();
                                }
                                else {
                                    webSession.PeriodBeginningDate = tmp.Year.ToString() + tmp.Week.ToString();
                                }
                                break;

                            case CstCustomerSession.Period.Type.previousWeek:
                                tmp = new AtomicPeriodWeek(DateTime.Now);
                                tmp.SubWeek(1);
                                if (tmp.Week < 10)
                                {
                                    webSession.PeriodBeginningDate = webSession.PeriodEndDate = tmp.Year.ToString() + "0" + tmp.Week.ToString();
                                }
                                else {
                                    webSession.PeriodBeginningDate = webSession.PeriodEndDate = tmp.Year.ToString() + tmp.Week.ToString();
                                }
                                break;

                            case CstCustomerSession.Period.Type.dateToDate:
                                webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                break;

                            case CstCustomerSession.Period.Type.nLastDays:

                                webSession.PeriodBeginningDate = DateTime.Now.AddDays(1 - webSession.PeriodLength).ToString("yyyyMMdd");
                                webSession.PeriodEndDate = DateTime.Now.ToString("yyyyMMdd");
                                break;

                            case CstCustomerSession.Period.Type.previousDay:

                                webSession.PeriodBeginningDate = webSession.PeriodEndDate = DateTime.Now.AddDays(1 - webSession.PeriodLength).ToString("yyyyMMdd");

                                break;

                            default:
                                webSession.PeriodBeginningDate = webSessionSave.PeriodBeginningDate;
                                webSession.PeriodEndDate = webSessionSave.PeriodEndDate;
                                break;
                        }
                    }
                    else {
                        if (Modules.IsDashBoardModule(webSessionSave))
                        {
                            try
                            {
                                Dates.WebSessionSaveDownloadDates(webSessionSave, ref PeriodBeginningDate, ref PeriodEndDate);
                            }
                            catch (System.Exception err)
                            {
                                notValidPeriod = true;
                                invalidPeriodMessage = err.Message;
                            }
                            webSession.PeriodBeginningDate = PeriodBeginningDate;
                            webSession.PeriodEndDate = PeriodEndDate;
                            if (webSession.PeriodType == CstCustomerSession.Period.Type.LastLoadedWeek || webSession.PeriodType == CstCustomerSession.Period.Type.LastLoadedMonth)
                                webSession.DetailPeriodBeginningDate = webSession.DetailPeriodEndDate = "";
                        }
                        else if (webSessionSave.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE)
                        {
                            try
                            {
                                Modules.LoadModuleStudyPeriodDates(webSession, webSessionSave);
                            }
                            catch (System.Exception err)
                            {
                                notValidPeriod = true;
                                invalidPeriodMessage = err.Message;
                            }
                        }
                    }

                    webSession.MediaAgencyFileYear = webSessionSave.MediaAgencyFileYear;
                    if (webSession.CustomerLogin.HasAtLeastOneMediaAgencyFlag() && (webSessionSave.PreformatedProductDetail.ToString().ToLower().IndexOf("agency") >= 0)
                        && !String.IsNullOrEmpty(webSession.MediaAgencyFileYear)
                        && webSession.PeriodBeginningDate.Length > 0
                        )
                    {
                        webSession.MediaAgencyFileYear = TNS.AdExpress.Constantes.DB.Tables.PRODUCT_GROUP_ADV_AGENCY + webSession.PeriodBeginningDate.Substring(0, 4);
                    }

                    webSession.ReachedModule = webSessionSave.ReachedModule;
                    webSession.ReferenceUniversAdvertiser = webSessionSave.ReferenceUniversAdvertiser;
                    webSession.ReferenceUniversMedia = webSessionSave.ReferenceUniversMedia;
                    webSession.ReferenceUniversProduct = webSessionSave.ReferenceUniversProduct;
                    webSession.Sorting = webSessionSave.Sorting;
                    webSession.Unit = webSessionSave.Unit;

                    //Patch last reahce result URL pour tableaux Dynamiques
                    webSession.LastReachedResultUrl = webSessionSave.LastReachedResultUrl;

                    webSession.ModuleTraductionCode = webSessionSave.ModuleTraductionCode;

                    webSession.ComparaisonCriterion = webSessionSave.ComparaisonCriterion;

                    if (webSession.PeriodEndDate.Length > 0 && (DateTime.Now.Year - int.Parse(webSession.PeriodEndDate.Substring(0, 4)) < 2))
                        webSession.ComparativeStudy = webSessionSave.ComparativeStudy;
                    else webSession.ComparativeStudy = false;
                    webSession.CustomizedReferenceComcurrentElements = webSessionSave.CustomizedReferenceComcurrentElements;
                    webSession.PreformatedTable = webSessionSave.PreformatedTable;
                    webSession.PDM = webSessionSave.PDM;
                    webSession.PDV = webSessionSave.PDV;
                    webSession.PersonalizedElementsOnly = webSessionSave.PersonalizedElementsOnly;
                    webSession.Graphics = webSessionSave.Graphics;
                    webSession.ComparativePeriodType = webSessionSave.ComparativePeriodType;
                    webSession.PeriodSelectionType = webSessionSave.PeriodSelectionType;

                    if (!webSession.ComparativeStudy || !webSessionSave.Evolution)
                        webSession.Evolution = false;

                    webSession.Format = webSessionSave.Format;
                    webSession.NamedDay = webSessionSave.NamedDay;
                    webSession.TimeInterval = webSessionSave.TimeInterval;
                    webSession.DetailPeriodBeginningDate = webSessionSave.DetailPeriodBeginningDate;
                    webSession.DetailPeriodEndDate = webSessionSave.DetailPeriodEndDate;

                    #region Add for (APPM)
                    webSession.CurrentUniversAEPMTarget = webSessionSave.CurrentUniversAEPMTarget;
                    webSession.SelectionUniversAEPMWave = webSessionSave.SelectionUniversAEPMWave;
                    webSession.SelectionUniversOJDWave = webSessionSave.SelectionUniversOJDWave;
                    webSession.SelectionUniversAEPMTarget = webSessionSave.SelectionUniversAEPMTarget;
                    webSession.CurrentUniversAEPMWave = webSessionSave.CurrentUniversAEPMWave;
                    webSession.CurrentUniversOJDWave = webSessionSave.CurrentUniversOJDWave;
                    webSession.EmailRecipient = webSessionSave.EmailRecipient;
                    webSession.Ecart = webSessionSave.Ecart;
                    webSession.ExportedPDFFileName = webSessionSave.ExportedPDFFileName;
                    webSession.PublicationBeginningDate = webSessionSave.PublicationBeginningDate;
                    webSession.PublicationEndDate = webSessionSave.PublicationEndDate;
                    webSession.PublicationDateType = webSessionSave.PublicationDateType;
                    #endregion

                    //Set the new product univers
                    webSession.PrincipalProductUniverses = webSessionSave.PrincipalProductUniverses;
                    webSession.SecondaryProductUniverses = webSessionSave.SecondaryProductUniverses;
                    //Set the new media univers
                    webSession.PrincipalMediaUniverses = webSessionSave.PrincipalMediaUniverses;
                    webSession.SecondaryMediaUniverses = webSessionSave.SecondaryMediaUniverses;
                    //New univers Advertising Agency
                    webSession.PrincipalAdvertisingAgnecyUniverses = webSessionSave.PrincipalAdvertisingAgnecyUniverses;
                    webSession.SecondaryAdvertisingAgnecyUniverses = webSessionSave.SecondaryAdvertisingAgnecyUniverses;
                    //Profession universes
                    webSession.PrincipalProfessionUniverses = webSessionSave.PrincipalProfessionUniverses;
                    webSession.ProductDetailLevel = null;
                    webSession.SelectedLocations = webSessionSave.SelectedLocations;
                    webSession.SelectedPresenceTypes = webSessionSave.SelectedPresenceTypes;

                    webSession.IsExcluWeb = webSessionSave.IsExcluWeb;
                    webSession.EvaliantCountryAccessList = string.Empty;

                    if (notValidPeriod)
                    {
                        result.Message = GestionWeb.GetWebWord(1787, webSession.SiteLanguage);
                    }
                    else if (validModule)
                    {
                        if (validResultPage)
                        {
                            webSession.Save();
                            if (webSession.LastReachedResultUrl.Length != 0)
                            {
                                result.Success = true;
                                //Response.Redirect(webSession.LastReachedResultUrl + "?idSession=" + webSession.IdSession);
                            }
                            else
                            {
                                //Error :The  requested session can't be loaded.
                                result.Message = GestionWeb.GetWebWord(851, webSession.SiteLanguage);
                            }
                        }
                        else
                        {
                            //Error :The  requested session is no more available.
                            result.Message = GestionWeb.GetWebWord(2455, webSession.SiteLanguage);
                        }
                    }
                    else {
                        //Error :You have not the required permissions
                        result.Message = GestionWeb.GetWebWord(832, webSession.SiteLanguage);                        
                    }
                }
                else {
                    //Error : Please select a session to load.
                    result.Message = GestionWeb.GetWebWord(831, webSession.SiteLanguage);                    
                }
            }
            catch (TNS.AdExpress.Domain.Exceptions.NoDataException)
            {
                result.Message = GestionWeb.GetWebWord(1787, webSession.SiteLanguage);
            }
            catch (System.Exception ex)
            {
                result.Message = ex.Message;
            }
        
            return result;
        }
        
        private bool CompareDateEnd(DateTime dateBegin, DateTime dateEnd)
        {
            if (dateEnd < dateBegin)
                return true;
            else
                return false;
        }

        private void UpdateGlobalDates(CstCustomerSession.Period.Type type, WebSession webSessionSave, DateTime FirstDayNotEnable, WebSession webSession)
        {

            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.date];
            IDate date = (IDate)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, null, null, null);

            date.UpdateDate(type, ref webSession, webSessionSave, FirstDayNotEnable);
        }
        #region UpdateRecapDates
        /// <summary>
        /// Mets à jour les dates des Recap en fonction de la fréquenece de livraison des données
        /// </summary>
        /// <param name="periodType">Type de période</param>
        /// <param name="notValidPeriod">Indique si période invalide</param>
        /// <param name="invalidPeriodMessage">Message</param>
        private void UpdateRecapDates(CstCustomerSession.Period.Type periodType, ref bool notValidPeriod, ref string invalidPeriodMessage, WebSession webSession)
        {

            DateTime downloadDate = new DateTime(webSession.DownLoadDate, 12, 31);

            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
            //du dernier mois dispo en BDD
            //traitement de la notion de fréquence
            switch (periodType)
            {

                case CstCustomerSession.Period.Type.nLastMonth:

                    if (DateTime.Now.Year > webSession.DownLoadDate)
                    {
                        webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - webSession.PeriodLength).ToString("yyyyMM");
                        webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");

                    }
                    else if (webSession.DownLoadDate == DateTime.Now.Year && int.Parse(webSession.PeriodBeginningDate.Substring(0, 4)) < DateTime.Now.Year)
                        webSession.PeriodBeginningDate = DateTime.Now.ToString("yyyy01");//L'étude se fait sur une année civile pour les module d'analyse sectorieilles
                    break;

                case CstCustomerSession.Period.Type.currentYear:
                    if (DateTime.Now.Year > webSession.DownLoadDate)
                    {
                        webSession.PeriodBeginningDate = downloadDate.AddMonths(1 - webSession.PeriodLength).ToString("yyyy01");
                        webSession.PeriodEndDate = downloadDate.ToString("yyyyMM");

                    }
                    break;

                case CstCustomerSession.Period.Type.dateToDateMonth:
                    //Les analyses sectorielles portent au minimumn jusqu'à l'année N-2
                    //Si la période enregistrée est inferieur à l'année N-2, alors il faut la ramener à l'année N-2
                    if (int.Parse(webSession.PeriodBeginningDate.ToString().Substring(0, 4)) < DateTime.Now.AddYears(-2).Year ||
                        int.Parse(webSession.PeriodEndDate.ToString().Substring(0, 4)) < DateTime.Now.AddYears(-2).Year)
                    {
                        webSession.PeriodBeginningDate = DateTime.Now.AddYears(-2).ToString("yyyy") + webSession.PeriodBeginningDate.Substring(4, 2);
                        webSession.PeriodEndDate = DateTime.Now.AddYears(-2).ToString("yyyy") + webSession.PeriodEndDate.Substring(4, 2);
                    }

                    if (int.Parse(webSession.PeriodBeginningDate.ToString().Substring(0, 4)) < DateTime.Now.AddYears(-1).Year ||
                        int.Parse(webSession.PeriodEndDate.ToString().Substring(0, 4)) < DateTime.Now.AddYears(-1).Year)
                        webSession.ComparativeStudy = false;
                    break;

                default:
                    break;
            }

            if (periodType == CstCustomerSession.Period.Type.nLastMonth || periodType == CstCustomerSession.Period.Type.currentYear)
            {

                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = webSession;
                var dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
                    , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                string absolutEndPeriod = dateDAL.CheckPeriodValidity(webSession, webSession.PeriodEndDate);

                if ((int.Parse(absolutEndPeriod) < int.Parse(webSession.PeriodBeginningDate)) || (absolutEndPeriod.Substring(4, 2).Equals("00")))
                {
                    notValidPeriod = true;
                    invalidPeriodMessage = GestionWeb.GetWebWord(1787, webSession.SiteLanguage);
                }
                else {
                    webSession.PeriodEndDate = absolutEndPeriod;
                }
            }
        }
        #endregion
        #region PopulateGenericProductDetailLevel
        /// <summary>
		/// Populate GenericProductDetailLevel array list while GenericProductDetailLevel save field is null
		/// </summary>
		/// <param name="webSessionSave">Customer session saved</param>
		/// <returns>List of levels Ids</returns>
		private ArrayList PopulateGenericProductDetailLevel(WebSession webSessionSave)
        {
            ArrayList levels = new ArrayList();
            try
            {
                switch (webSessionSave.PreformatedProductDetail)
                {
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sector:
                        levels.Add(11);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiser:
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrand:
                        levels.Add(8);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct:
                        levels.Add(8);
                        levels.Add(9);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupBrand:
                        levels.Add(8);
                        levels.Add(13);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupBrandProduct:
                        levels.Add(8);
                        levels.Add(13);
                        levels.Add(9);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupProduct:
                        levels.Add(8);
                        levels.Add(13);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupSegmentBrandProduct:
                        levels.Add(8);
                        levels.Add(13);
                        levels.Add(14);
                        levels.Add(9);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserGroupSegmentProduct:
                        levels.Add(8);
                        levels.Add(13);
                        levels.Add(14);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.advertiserProduct:
                        levels.Add(8);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyAdvertiser:
                        levels.Add(16);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.agencyProduct:
                        levels.Add(16);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.brand:
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.group:
                        levels.Add(13);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgency:
                        levels.Add(15);
                        levels.Add(16);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyAdvertiser:
                        levels.Add(15);
                        levels.Add(16);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.group_agencyAgencyProduct:
                        levels.Add(15);
                        levels.Add(16);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiser:
                        levels.Add(13);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserBrand:
                        levels.Add(13);
                        levels.Add(8);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAdvertiserProduct:
                        levels.Add(13);
                        levels.Add(8);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupAvertiserBrandProduct:
                        levels.Add(13);
                        levels.Add(8);
                        levels.Add(9);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrand:
                        levels.Add(13);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupBrandProduct:
                        levels.Add(13);
                        levels.Add(9);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupProduct:
                        levels.Add(13);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.groupSegment:
                        levels.Add(13);
                        levels.Add(14);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompany:
                        levels.Add(7);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiser:
                        levels.Add(7);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserBrand:
                        levels.Add(7);
                        levels.Add(8);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.holdingCompanyAdvertiserProduct:
                        levels.Add(7);
                        levels.Add(8);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.product:
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser:
                        levels.Add(11);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorAdvertiserProduct:
                        levels.Add(11);
                        levels.Add(8);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorHoldingCompanyAdvertiser:
                        levels.Add(11);
                        levels.Add(7);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorProduct:
                        levels.Add(11);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsector:
                        levels.Add(11);
                        levels.Add(12);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.sectorSubsectorGroup:
                        levels.Add(11);
                        levels.Add(12);
                        levels.Add(13);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser:
                        levels.Add(14);
                        levels.Add(8);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserBrand:
                        levels.Add(14);
                        levels.Add(8);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentAdvertiserProduct:
                        levels.Add(14);
                        levels.Add(8);
                        levels.Add(10);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentBrand:
                        levels.Add(14);
                        levels.Add(9);
                        break;
                    case CustomerSessions.PreformatedDetails.PreformatedProductDetails.segmentProduct:
                        levels.Add(14);
                        levels.Add(10);
                        break;
                    default:
                        levels.Add(8);
                        break;
                }
            }
            catch (System.Exception)
            {
                levels.Clear();
                levels.Add(8);
            }
            return (levels);
        }
        #endregion
        #endregion
        #endregion
    }
}

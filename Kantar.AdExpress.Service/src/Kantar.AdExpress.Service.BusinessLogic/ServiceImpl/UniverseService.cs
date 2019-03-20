using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using TNS.AdExpress.Web.Core.Selection;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using DomainWebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;
using KM.AdExpressI.MyAdExpress.DAL;
using LS = TNS.Ares.Domain.LS;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.Ares.Alerts.DAL;
using TNS.Alert.Domain;
using TNS.AdExpress.Domain.Web.Navigation;
using AutoMapper;
using KM.Framework.Constantes;
using TNS.AdExpress.Web.Core.Utilities;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using TNS.AdExpress.Constantes.Classification;
using TNS.AdExpress.Classification;
using NLog;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;
using TNS.AdExpress.Vehicle.DAL;
using TNS.AdExpress.Domain.Classification;
using TNS.FrameWork.WebResultUI;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class UniverseService : IUniverseService
    {
        private WebSession webSession = null;
        private static Logger Logger = LogManager.GetCurrentClassLogger();
        private const long Capacity = 1000;
        private const long ExceptionMsg = 922;
        private const long SecurityMsg = 2285;
        private const long OverLimitMsgCode = 2286;
        public const long ElementLabelCode = 2278;
        public const int CATEGORY = 1;
        public const int PARENT = 7;

        public List<UniversItem> GetItems(SearchRequest request, out int nbItems, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(request.WebSessionId);
            var result = new List<UniversItem>();
            try
            {
                webSession.SelectionUniversMedia.Nodes.Clear();
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                object[] param = new object[3];
                param[0] = webSession;
                param[1] = request.Dimension;
                if (request.MediaList != null)
                    param[2] = string.Join(",", request.MediaList.Select(e => e));
                IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                    string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
                  cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                classficationDAL.DBSchema = GetSchema(webSession.CurrentModule);

                //Exclude ZZ Inconnu
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    Dictionary<long, string> filters;
                    filters = new Dictionary<long, string>();
                    filters.Add(19, "9999,999");
                    filters.Add(20, "9999,999,2990");
                    classficationDAL.Filters = filters;
                }

                DataTable data = classficationDAL.GetItems(request.LevelId, request.Keyword).Tables[0];
                foreach (var item in data.AsEnumerable())
                {
                    var UItem = new UniversItem
                    {
                        Id = Int64.Parse(item.ItemArray[0].ToString()),
                        Label = item.ItemArray[1].ToString()
                    };
                    result.Add(UItem);
                }
                nbItems = result.Count;
            }
            catch (Exception ex)
            {
                nbItems = 0;

                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result.Take(1000).ToList();
        }


        public List<UniversItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession, Dimension dimension, List<int> idMedias, out int nbItems, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(idSession);
            List<UniversItem> result = new List<UniversItem>();
            try
            {
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                object[] param = new object[3];
                param[0] = webSession;
                param[1] = dimension;
                if (idMedias != null)
                    param[2] = string.Join(",", idMedias.Select(e => e));
                IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                    string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
                  cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                classficationDAL.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;

                //Exclude ZZ Inconnu
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    Dictionary<long, string> filters;
                    filters = new Dictionary<long, string>();
                    filters.Add(19, "9999,999");
                    filters.Add(20, "9999,999,2990");
                    classficationDAL.Filters = filters;
                }

                DataTable data = classficationDAL.GetItems(levelId, selectedClassificationItemsIds, selectedLevelId).Tables[0];

                foreach (var item in data.AsEnumerable())
                {
                    var UItem = new UniversItem
                    {
                        Id = Int64.Parse(item.ItemArray[0].ToString()),
                        Label = item.ItemArray[1].ToString()
                    };
                    result.Add(UItem);
                }
                nbItems = result.Count;
            }
            catch (Exception ex)
            {
                nbItems = 0;

                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return result.Take(1000).ToList();
        }
        public UniversBranchResult GetBranches(string webSessionId, Dimension dimension, HttpContextBase httpContext, bool selectionPage = true, int MaxIncludeNbr = 2, int MaxExcludeNbr = 1)
        {
                var webSession = (WebSession)WebSession.Load(webSessionId);
            UniversBranchResult result = new UniversBranchResult();
            try
            {
                var tuple = GetAllowedIds(webSession, dimension, selectionPage);
                result.SiteLanguage = tuple.Item4;
                result.DefaultBranchId = tuple.Item5;
                result.Trees = new List<Tree>(MaxIncludeNbr + MaxExcludeNbr);
                result.ControllerDetails = GetCurrentControllerDetails(tuple.Item3.CurrentModule);
                var allowedBranchesIds = tuple.Item2;
                if (dimension == Dimension.product)
                    ClearProduct(tuple.Item3);
                var allUnivers = new List<UniversLevel>();
                if (allowedBranchesIds.Any())
                {
                    foreach (var id in allowedBranchesIds)
                    {
                        var domainBranch = UniverseBranches.Get(id);
                        var branch = new UniversBranch
                        {
                            Id = id,
                            UniversLevels = new List<UniversLevel>()
                        };
                        branch.IsSelected = (id == result.DefaultBranchId);
                        branch.Label = GestionWeb.GetWebWord(domainBranch.LabelId, result.SiteLanguage);
                        foreach (var item in domainBranch.Levels)
                        {
                            if (tuple != null && tuple.Item1.Contains(item.ID))
                            {
                                UniversLevel level = new UniversLevel
                                {
                                    Id = item.ID,
                                    LabelId = item.LabelId,
                                    Capacity = Capacity,
                                    Label = GestionWeb.GetWebWord(item.LabelId, result.SiteLanguage),
                                    BranchId = branch.Id,
                                    OverLimitMessage = GestionWeb.GetWebWord(OverLimitMsgCode, result.SiteLanguage),
                                    SecurityMessage = GestionWeb.GetWebWord(SecurityMsg, result.SiteLanguage),
                                    ExceptionMessage = GestionWeb.GetWebWord(ExceptionMsg, result.SiteLanguage)
                                };
                                if (!allUnivers.Any(p => p.Id == level.Id))
                                {
                                    allUnivers.Add(level);
                                }
                                branch.UniversLevels.Add(level);
                            }

                        }
                        result.Branches.Add(branch);
                    }
                    // Create trees according to the dimension
                    int idTree = 0;
                    if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
                    {
                        MaxIncludeNbr = MaxExcludeNbr = 1;
                        result.MaxUniverseItems = int.Parse(System.Configuration.ConfigurationManager.AppSettings["FacebookMaxItems"]);
                        for (int i = 1; i <= MaxIncludeNbr + MaxExcludeNbr; i++)
                        {
                            var tree = new Tree
                            {
                                Id = idTree,
                                LabelId = ElementLabelCode,
                                UniversLevels = allUnivers,
                                AccessType = AccessType.includes,
                                IsDefaultActive = idTree == MaxIncludeNbr + MaxExcludeNbr
                            };
                            idTree++;
                            result.Trees.Add(tree);
                        }
                    }
                    else
                    {
                        foreach (AccessType type in Enum.GetValues(typeof(AccessType)))
                        {
                            var maxTreesNbr = (type == AccessType.includes) ? MaxIncludeNbr : MaxExcludeNbr;//(Enum.GetValues(typeof(AccessType)))
                            for (int i = 1; i <= maxTreesNbr; i++)
                            {
                                var tree = new Tree
                                {
                                    Id = idTree,
                                    LabelId = ElementLabelCode,
                                    UniversLevels = allUnivers,
                                    AccessType = type,
                                    IsDefaultActive = (type == AccessType.includes && idTree == 1)
                                };
                                idTree++;
                                result.Trees.Add(tree);
                            }
                        }
                    }
                }
                result.Success = true;
            }
            catch (Exception ex)
            {
                string message = String.Format("IdWebSession: {0}\n User Agent: {1}\n Login: {2}\n password: {3}\n error: {4}\n StackTrace: {5}\n Module: {6}", webSession.IdSession, webSession.UserAgent, webSession.CustomerLogin.Login, webSession.CustomerLogin.PassWord, ex.InnerException + ex.Message, ex.StackTrace, GestionWeb.GetWebWord((int)ModulesList.GetModuleWebTxt(webSession.CurrentModule), webSession.SiteLanguage));
                result.ErrorMessage = message;

                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return result;
        }
        public UniversGroupsResponse GetUserSavedUniversGroups(string webSessionId, Dimension dimension, HttpContextBase httpContext, bool selectionPage = true)
        {
            var webSession = (WebSession)WebSession.Load(webSessionId);
            UniversGroupsResponse result = new UniversGroupsResponse();
            try
            {
                var tuple = GetAllowedIds(webSession, dimension, selectionPage);
                result.SiteLanguage = tuple.Item4;
                var allowedLevels = (RequireForceLevel(webSession.CurrentModule) && dimension == Dimension.media) ? new List<long> { TNSClassificationLevels.MEDIA } : tuple.Item1;
                var listUniverseClientDescription = TNS.AdExpress.Constantes.Web.LoadableUnivers.GENERIC_UNIVERSE.ToString();
                var branch = (dimension == Dimension.product) ? Branch.type.product.GetHashCode().ToString() : Branch.type.media.GetHashCode().ToString();

                //branch product type associated to module Facebook
                if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK) branch = Branch.type.productSocial.GetHashCode().ToString();

                List<long> allowedFilters = GetAllowedFilters(webSession, dimension);
                var data = UniversListDataAccess.GetData(tuple.Item3, branch.ToString(), listUniverseClientDescription, allowedLevels, allowedFilters);

                List<UserUnivers> UserUniversList = new List<UserUnivers>();
                if (data != null && data.Rows.Count > 0)
                {
                    foreach (DataRow row in data.Rows)
                    {
                        UserUnivers UserUnivers = new UserUnivers
                        {
                            ParentId = (long)row[0],
                            ParentDescription = row[1].ToString(),
                            Id = (long)row[2],
                            Description = row[3].ToString()
                        };
                        UserUniversList.Add(UserUnivers);
                    }
                    var groupedUniversList = UserUniversList.GroupBy(p => p.ParentId);
                    foreach (var item in groupedUniversList)
                    {
                        UserUniversGroup universGroup = new UserUniversGroup
                        {
                            Id = item.Key,
                            Description = item.FirstOrDefault().ParentDescription,
                            UserUnivers = item.ToList(),
                            Count = item.Count()
                        };
                        result.UniversGroups.Add(universGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public UniversResponse GetTreesByUserUnivers(int userUniversId, string webSessionId, Dimension dimension, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            UniversResponse result = new UniversResponse
            {
                Trees = new List<Tree>(),
                UniversMediaIds = new List<long>(),
                ModuleId = webSession.CurrentModule,
                Message = string.Empty
            };
            try
            {
                #region try block

                int index = 0;
                List<long> medias = new List<long>();
                //long idModule = webSession.CurrentModule;
                Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> Universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)
                UniversListDataAccess.GetTreeNodeUniverseWithMedia(userUniversId, webSession, out medias);
                if (medias != null && medias.Any())
                    result.UniversMediaIds = medias;
                // MediaSchedule, Portfolio & Lost/won

                if (HasCompetitorUniverse(result.ModuleId, dimension))
                {

                    for (int counter = 0; counter < Math.Min(5, Universes.Count()); counter++)
                    {

                        #region Iterate by Access Type
                        var presentAbsentUnivers = Universes[counter];
                        if (presentAbsentUnivers != null && presentAbsentUnivers.Count() > 0)
                        {
                            List<NomenclatureElementsGroup> elementsGroups = new List<NomenclatureElementsGroup>();
                            Tree tree = new Tree
                            {
                                AccessType = AccessType.includes,
                                UniversLevels = new List<UniversLevel>(),
                                Id = counter,
                                Label = (counter == 0) ? GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Referent, webSession.SiteLanguage) : GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Concurrent, webSession.SiteLanguage)
                            };
                            elementsGroups = presentAbsentUnivers.GetIncludes();
                            if (elementsGroups != null && elementsGroups.Count > 0)
                            {
                                #region Repository 

                                foreach (NomenclatureElementsGroup elementGroup in elementsGroups)
                                {
                                    #region looping inside elementsgroups                         
                                    DataSet ds = null;
                                    List<long> oldLevelsId = new List<long>();
                                    CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                                    object[] param = new object[2];
                                    param[0] = webSession;
                                    param[1] = dimension;
                                    ClassificationDAL classficationDAL = (ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                                    classficationDAL.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
                                    var tuple = GetAllowedIds(webSession, dimension, true);


                                    AddUniverseLevel(tree, ds, elementGroup, classficationDAL, tuple, oldLevelsId);

                                    cl = null;
                                    classficationDAL = null;
                                    result.Trees.Add(tree);
                                    #endregion
                                    if (elementsGroups.Count > 1)
                                    {
                                        tree = new Tree
                                        {
                                            AccessType = AccessType.includes,
                                            UniversLevels = new List<UniversLevel>(),
                                            Id = counter,
                                            Label = (counter == 0) ? "Referents" : "Concurrents"
                                            //Label = (counter == 0) ? GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Referent, webSession.SiteLanguage)
                                            //                    : GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Referent, webSession.SiteLanguage)
                                        };
                                    }
                                }
                                #endregion
                            }
                            //if (elementsGroups.Count == 0)
                            //{
                            //    result.Trees.Add(tree);
                            //}

                        }
                        #endregion
                    }
                }
                else
                {
                    var adExpressUniverse = Universes[index];
                    #region Iterate by Access Type
                    int id = 0;
                    if (adExpressUniverse != null && adExpressUniverse.Count() > 0)
                    {
                        foreach (AccessType type in Enum.GetValues(typeof(AccessType)))
                        {
                            List<NomenclatureElementsGroup> elementsGroups = new List<NomenclatureElementsGroup>();
                            Tree tree = new Tree
                            {
                                AccessType = type,
                                UniversLevels = new List<UniversLevel>(),
                                Id = id
                            };
                            elementsGroups = (type == AccessType.excludes) ? adExpressUniverse.GetExludes() : adExpressUniverse.GetIncludes();
                            if (elementsGroups != null && elementsGroups.Count > 0)
                            {
                                #region Repository 

                                foreach (NomenclatureElementsGroup elementGroup in elementsGroups)
                                {
                                    #region looping inside elementsgroups                         
                                    DataSet ds = null;
                                    List<long> oldLevelsId = new List<long>();
                                    CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                                    if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                                    object[] param = new object[2];
                                    param[0] = webSession;
                                    param[1] = dimension;
                                    ClassificationDAL classficationDAL = (ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                                    classficationDAL.DBSchema = GetSchema(webSession.CurrentModule);
                                    var tuple = GetAllowedIds(webSession, dimension, true);

                                    AddUniverseLevel(tree, ds, elementGroup, classficationDAL, tuple, oldLevelsId);

                                    cl = null;
                                    classficationDAL = null;
                                    result.Trees.Add(tree);
                                    #endregion
                                    id++;
                                    if (elementsGroups.Count > 1 && elementsGroups.Count + 1 > id)
                                    {
                                        tree = new Tree
                                        {
                                            AccessType = type,
                                            UniversLevels = new List<UniversLevel>(),
                                            Id = id
                                        };
                                    }
                                }
                                #endregion
                            }
                            //if (elementsGroups.Count == 0)
                            //{
                            //    result.Trees.Add(tree);
                            //    id++;
                            //}
                        }
                    }
                    #endregion
                }

                #endregion
            }


            catch (System.Exception ex)
            {
                //TODO
                result.Message = String.Format("Impossible de construire votre univers {0}", userUniversId);
                //throw (new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible de construire le Treeview Obout", err));

                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return result;
        }

        private bool HasCompetitorUniverse(long moduleId, Dimension dimension)
        {
            return ((WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE == moduleId && dimension == Dimension.media)
                || WebConstantes.Module.Name.FACEBOOK == moduleId);
        }

        private void AddUniverseLevel(Tree tree, DataSet ds,
            NomenclatureElementsGroup elementGroup, ClassificationDAL classficationDAL, Tuple<List<long>, List<int>, WebSession, int, int> tuple,
             List<long> oldLevelsId)
        {
            foreach (var currentLevel in tuple.Item1)
            {
                if (!oldLevelsId.Contains(currentLevel))
                {
                    if (elementGroup != null && elementGroup.Contains(currentLevel))
                    {
                        var table = UniverseLevels.Get(currentLevel).TableName;
                        ds = classficationDAL.GetSelectedItems(table, elementGroup.GetAsString(currentLevel));
                        if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                        {
                            UniversLevel level = new UniversLevel
                            {
                                Id = currentLevel,
                                UniversItems = new List<UniversItem>()
                            };
                            foreach (DataRow dr in ds.Tables[0].Rows)
                            {
                                UniversItem item = new UniversItem();
                                item.Id = (Int64)dr[0];
                                item.Label = dr[1].ToString();
                                item.IdLevelUniverse = currentLevel;
                                level.UniversItems.Add(item);

                            }
                            tree.UniversLevels.Add(level);
                        }
                    }
                }
            }

        }

        public UniversGroupSaveResponse SaveUserUnivers(UniversGroupSaveRequest request, HttpContextBase httpContext)
        {
            UniversGroupSaveResponse result = new UniversGroupSaveResponse();
            webSession = (WebSession)WebSession.Load(request.WebSessionId);
            try
            {
                #region To be Refactored
                if (request.Trees.Any() && request.Trees.Where(p => p.UniversLevels != null).Any() && request.UniversGroupId > 0 && (request.UserUniversId > 0 || !String.IsNullOrEmpty(request.Name)))
                {

                    #region Try block
                    try
                    {
                        if (request.Trees.Count > 0)
                        {
                            #region Get universe to save
                            result = SetUnivers(webSession, request);
                            #endregion
                        }

                        else
                        {
                            // Erreur : Impossible de sauvegarder, pas de groupe d'univers créé
                            result.ErrorMessage = GestionWeb.GetWebWord(925, webSession.SiteLanguage);
                            result.Success = false;
                        }
                    }
                    #endregion
                    #region catch block
                    catch (System.Exception err)
                    {
                        if (err.GetType() == typeof(TNS.Classification.Universe.SecurityException) ||
                                err.GetBaseException().GetType() == typeof(TNS.Classification.Universe.SecurityException))
                        {
                            webSession.Source.Close();
                            result.ErrorMessage = GestionWeb.GetWebWord(2285, webSession.SiteLanguage);
                            result.Success = false;
                        }
                        else if (err.GetType() == typeof(TNS.Classification.Universe.CapacityException))
                        {
                            webSession.Source.Close();
                            result.ErrorMessage = GestionWeb.GetWebWord(2286, webSession.SiteLanguage);
                            result.Success = false;
                        }
                        else if (err.GetType() != typeof(System.Threading.ThreadAbortException))
                        {
                            //TODO SendEmail(err.Message);
                            result.ErrorMessage = err.Message;
                            result.Success = false;
                        }
                    }
                    #endregion
                }
                else
                {
                    var message = string.Empty;
                    if (request.UserUniversId == 0 && string.IsNullOrEmpty(request.Name))
                    {
                        message = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.EmptyField, webSession.SiteLanguage);
                    }
                    if (request.Trees.Count() == 0 || request.UniversGroupId == 0 || request.Trees.Where(p => p.UniversLevels != null).Count() == 0)
                    {
                        message = String.Format("{0} \n {1}", message, GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.EmptyUnivers, webSession.SiteLanguage));
                    }
                    result.ErrorMessage = message;
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return result;
        }

        public UniversGroupsResponse GetUserUniversGroups(string webSessionId, Dimension dimension, HttpContextBase httpContext, long idGroup = 0)
        {
            var webSession = (WebSession)WebSession.Load(webSessionId);
            UniversGroupsResponse result = new UniversGroupsResponse();
            try
            {
                result.SiteLanguage = webSession.SiteLanguage;
                List<UserUnivers> userUniversList = new List<UserUnivers>();
                var branch = (dimension == Dimension.product) ? Branch.type.product.GetHashCode().ToString() : Branch.type.media.GetHashCode().ToString();

                if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
                {
                    branch = Branch.type.productSocial.GetHashCode().ToString();
                    result.CanSetDefaultUniverse = true;
                }

                userUniversList = GetUniverses(dimension, webSession, idGroup);
                if (userUniversList.Any())
                {
                    var groupedUniversList = userUniversList.GroupBy(p => p.ParentId);
                    foreach (var item in groupedUniversList)
                    {
                        UserUniversGroup universGroup = new UserUniversGroup
                        {
                            Id = item.Key,
                            Description = item.FirstOrDefault().ParentDescription,
                            UserUnivers = item.ToList(),
                            Count = item.Count()
                        };
                        result.UniversGroups.Add(universGroup);
                    }
                }
                else
                {
                    var ds = UniversListDataAccess.GetGroupUniverses(webSession);
                    foreach (DataRow row in ds.Tables[0].Rows)
                    {
                        UserUniversGroup universGroup = new UserUniversGroup
                        {
                            Id = (long)row[0],
                            Description = row[1].ToString(),
                            UserUnivers = new List<UserUnivers>(),
                            Count = 0
                        };
                        result.UniversGroups.Add(universGroup);
                    }
                }
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public string SaveUserResult(string webSessionId, string folderId, string saveAsResultId, string saveResult, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            string result = string.Empty;
            try
            {
                string sortKey = httpContext.Request.Cookies["sortKey"].Value;
                string sortOrder = httpContext.Request.Cookies["sortOrder"].Value;

                if (!string.IsNullOrEmpty(sortKey) && !string.IsNullOrEmpty(sortOrder))
                {
                    webSession.SortKey = sortKey;
                    webSession.Sorting = (ResultTable.SortOrder)Enum.Parse(typeof(ResultTable.SortOrder), sortOrder);
                    webSession.Save();
                }

                if (saveResult.Length == 0 && !saveAsResultId.Equals("0"))
                {
                    string savedSessionName = CheckedText.CheckedAccentText(MyResultsDAL.GetSession(Int64.Parse(saveAsResultId), webSession));
                    if (savedSessionName.Length > 0 && MyResultsDAL.UpdateMySession(Int64.Parse(folderId), saveAsResultId, savedSessionName, webSession))
                    {
                        #region Tracking utilisation sauvegarde
                        webSession.OnUseMyAdExpressSave();
                        #endregion

                        // Validation : confirmation d'enregistrement de la requête
                        result = GestionWeb.GetWebWord(826, webSession.SiteLanguage);
                        return result;
                    }
                    else
                    {
                        // Erreur : Echec de l'enregistrement de la requête		
                        result = GestionWeb.GetWebWord(825, webSession.SiteLanguage);
                        return result;
                    }

                }
                else if (saveResult.Length != 0 && saveResult.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                {
                    if (!MyResultsDAL.IsSessionExist(webSession, saveResult))
                    {
                        if (MyResultsDAL.SaveMySession(Int64.Parse(folderId), saveResult, webSession))
                        {

                            #region Tracking utilisation sauvegarde
                            webSession.OnUseMyAdExpressSave();
                            #endregion

                            // Validation : confirmation d'enregistrement de la requête
                            result = GestionWeb.GetWebWord(826, webSession.SiteLanguage);
                            return result;
                        }
                        else
                        {
                            // Erreur : Echec de l'enregistrement de la requête
                            result = GestionWeb.GetWebWord(825, webSession.SiteLanguage);
                            return result;
                        }
                    }
                    else
                    {
                        // Erreur : session déjà existante
                        result = GestionWeb.GetWebWord(824, webSession.SiteLanguage);
                        return result;
                    }
                }
                else if (saveResult.Length == 0)
                {
                    // Erreur : Le champs est vide
                    result = GestionWeb.GetWebWord(822, webSession.SiteLanguage);
                    return result;
                }
                else
                {
                    // Erreur : suppérieur à 50 caractères
                    result = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    return result;
                }
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public AdExpressUniversResponse GetResultUnivers(string webSessionId, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            var result = new AdExpressUniversResponse
            {
                UniversType = UniversType.Result,
                UniversGroups = new List<UserUniversGroup>(),
                Labels = LoadPageLabels(webSession.SiteLanguage),
                SiteLanguage = webSession.SiteLanguage
            };
            try
            {
                var dsListRepertory = MyResultsDAL.GetData(webSession);
                List<UserUnivers> userUniversList = new List<UserUnivers>();
                if (dsListRepertory != null && dsListRepertory.Tables[0].AsEnumerable().Any())
                {
                    var list = dsListRepertory.Tables[0].AsEnumerable().Select(p => new
                    {
                        GroupID = p.Field<long?>("ID_DIRECTORY") ?? 0,
                        GroupDescription = p.Field<string>("DIRECTORY"),
                        UniversID = p.Field<long?>("ID_MY_SESSION") ?? 0,
                        UniversDescription = p.Field<string>("MY_SESSION"),
                    }).ToList();
                    foreach (var item in list)
                    {
                        UserUnivers UserUnivers = new UserUnivers
                        {
                            ParentId = item.GroupID,
                            ParentDescription = item.GroupDescription,
                            Id = item.UniversID,
                            Description = item.UniversDescription
                        };
                        userUniversList.Add(UserUnivers);
                    }
                    var groupedUniversList = userUniversList.GroupBy(p => p.ParentId);
                    foreach (var item in groupedUniversList)
                    {
                        UserUniversGroup universGroup = new UserUniversGroup
                        {
                            Id = item.Key,
                            Description = item.FirstOrDefault().ParentDescription,
                            UserUnivers = item.ToList(),
                            Count = item.Count()
                        };
                        result.UniversGroups.Add(universGroup);
                    }
                    result.NbrFolder = result.UniversGroups.Count();
                    result.NbrUnivers = list.Count(p => p.UniversID > 0);
                }
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

            return result;
        }

        public AdExpressUniversResponse GetUnivers(string webSessionId, string branch, string listUniverseClientDescription, HttpContextBase httpContext)
        {
            #region Init
            webSession = (WebSession)WebSession.Load(webSessionId);
            var result = new AdExpressUniversResponse
            {
                UniversType = UniversType.Univers,
                UniversGroups = new List<UserUniversGroup>(),
                Labels = LoadPageLabels(webSession.SiteLanguage),
                SiteLanguage = webSession.SiteLanguage
            };

            if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
            {
                branch = Branch.type.productSocial.GetHashCode().ToString();
                result.CanSetDefaultUniverse = true;
            }

            List<UserUnivers> userUniversList = new List<UserUnivers>();
            #endregion
            try
            {
                #region Repository
                var data = UniversListDataAccess.GetData(webSession, branch, listUniverseClientDescription, true);
                if (data != null && data.Tables[0].AsEnumerable().Any())
                {
                    var list = data.Tables[0].AsEnumerable().Select(p => new
                    {
                        GroupID = p.Field<long?>("ID_GROUP_UNIVERSE_CLIENT"),
                        GroupDescription = p.Field<string>("GROUP_UNIVERSE_CLIENT"),
                        UniversID = p.Field<long?>("ID_UNIVERSE_CLIENT"),
                        UniversDescription = p.Field<string>("UNIVERSE_CLIENT"),
                    }).ToList();
                    foreach (var item in list)
                    {
                        UserUnivers UserUnivers = new UserUnivers
                        {
                            ParentId = item.GroupID ?? 0,
                            ParentDescription = item.GroupDescription,
                            Id = item.UniversID ?? 0,
                            Description = item.UniversDescription
                        };
                        userUniversList.Add(UserUnivers);
                    }
                    var groupedUniversList = userUniversList.GroupBy(p => p.ParentId);
                    foreach (var item in groupedUniversList)
                    {
                        UserUniversGroup universGroup = new UserUniversGroup
                        {
                            Id = item.Key,
                            Description = item.FirstOrDefault().ParentDescription,
                            UserUnivers = item.ToList(),
                            Count = item.Count()
                        };
                        result.UniversGroups.Add(universGroup);
                        result.NbrFolder = result.UniversGroups.Count();
                        result.NbrUnivers = list.Count(p => p.UniversID > 0);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public AlertResponse GetUserAlerts(string webSessionId, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            AlertResponse result = new AlertResponse
            {
                Alerts = new List<Core.Domain.Alert>(),
                SiteLanguage = webSession.SiteLanguage
            };
            try
            {
                #region Alerts
                if (AlertConfiguration.IsActivated)
                {
                    var layer = LS.PluginConfiguration.GetDataAccessLayer(LS.PluginDataAccessLayerName.Alert);
                    TNS.FrameWork.DB.Common.IDataSource src = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.alert);
                    var alertDAL = (IAlertDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + layer.AssemblyName, layer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { src }, null, null);
                    AlertCollection alerts = alertDAL.GetAlerts(webSession.CustomerLogin.IdLogin);
                    if (alerts.Count == 0)
                        result.ErrorMessage = GestionWeb.GetWebWord(833, result.SiteLanguage);
                    else
                    {
                        var alertsModel = Mapper.Map<List<Core.Domain.Alert>>(alerts);
                        foreach (var alert in alertsModel)
                        {
                            var occurences = alertDAL.GetOccurrences(alert.Id);
                            alert.Occurrences = Mapper.Map<List<Occurence>>(occurences);
                            alert.TimeSchedule = (new DateTime(alertDAL.GetAlertHours().FirstOrDefault(p => p.IdAlertSchedule == alert.IdAlertSchedule).HoursSchedule.Ticks)).ToShortTimeString();
                            alert.Module = GestionWeb.GetWebWord(ModulesList.GetModule(alert.IdModule).IdWebText, webSession.SiteLanguage);
                            switch (alert.Periodicity)
                            {
                                case Periodicity.Daily:
                                    alert.Frequency = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Daily, result.SiteLanguage);
                                    alert.PeriodicityDescription = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Daily, result.SiteLanguage);
                                    break;
                                case Periodicity.Weekly:
                                    alert.Frequency = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.EveryWeek, result.SiteLanguage);
                                    alert.PeriodicityDescription = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Weekly, result.SiteLanguage);
                                    break;
                                case Periodicity.Monthly:
                                    alert.Frequency = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.EveryMonth, result.SiteLanguage);
                                    alert.PeriodicityDescription = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Monthly, result.SiteLanguage);
                                    break;
                            };
                        }
                        result.Alerts = alertsModel;

                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public List<UniversItem> GetGategoryItems(SearchItemsCriteria criteria, out int nbItems, HttpContextBase httpContext, Dimension dimension = Dimension.product)
        {
            List<UniversItem> result = new List<UniversItem>();
            webSession = (WebSession)WebSession.Load(criteria.WebSessionId);
            try
            {
                int levelId = criteria.UniverseLevel;
                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                object[] param = new object[3];
                param[0] = webSession;
                param[1] = criteria.Dimension;
                if (criteria.Dimension == Dimension.media && criteria.MediaIds != null && webSession.CurrentModule != WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES)
                    param[2] = string.Join(",", criteria.MediaIds.Select(e => e));
                IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                    string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
                  cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);

                classficationDAL.DBSchema = GetSchema(webSession.CurrentModule);

                //Exclude ZZ Inconnu
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                {
                    Dictionary<long, string> filters;
                    filters = new Dictionary<long, string>();
                    filters.Add(19, "9999,999");
                    filters.Add(20, "9999,999,2990");
                    classficationDAL.Filters = filters;
                }

                DataTable data = classficationDAL.GetItems(levelId, "*").Tables[0];
                foreach (var item in data.AsEnumerable())
                {
                    var UItem = new UniversItem
                    {
                        Id = Int64.Parse(item.ItemArray[0].ToString()),
                        Label = item.ItemArray[1].ToString()
                    };
                    result.Add(UItem);
                }
                nbItems = result.Count;
            }
            catch (Exception ex)
            {
                nbItems = 0;

                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result.Take(1000).ToList();
        }
        public List<UserUnivers> GetUniverses(Dimension dimension, string webSessionId, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            List<UserUnivers> result = new List<UserUnivers>();
            try
            {
                result = GetUniverses(dimension, webSession, 0, false);
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return result;
        }

        public void ChangeMarketUniverse(long universeId, string webSessionId, HttpContextBase httpContext)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
            try
            {
                var adExpressUniverse = (Dictionary<int, AdExpressUniverse>)UniversListDataAccess.GetObjectUniverses(universeId, webSession);
                if (adExpressUniverse != null && adExpressUniverse.Count > 0)
                {
                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                        webSession.PrincipalAdvertisingAgnecyUniverses = adExpressUniverse;
                    else
                        webSession.PrincipalProductUniverses = adExpressUniverse;
                    webSession.Save();
                }
            }
            catch (Exception ex)
            {
                if (webSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, webSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
        }

        #region private methods
        private Tuple<List<long>, List<int>, WebSession, int, int> GetAllowedIds(WebSession webSession, Dimension dimension, bool selectionPage = true)
        {

            var siteLanguage = webSession.SiteLanguage;
            string listUniverseClientDescription = "";
            ILevelsRules levelsRules = null;
            List<int> tempBranchIds = new List<int>();
            List<UniverseLevel> tempLevels = new List<UniverseLevel>();
            List<long> _forbiddenLevelsId = new List<long>();
            List<int> _allowedBranchesIds = new List<int>();
            List<long> _allowedLevelsId = new List<long>();
            int defaultBranchId = 0;
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
            DomainWebNavigation.Module currentModuleDescription = DomainWebNavigation.ModulesList.GetModule(webSession.CurrentModule);
            if (selectionPage)
            {
                //Apply rigth Rules for getting levels and branches
                int pageId = 0;
                switch (dimension)
                {
                    case Dimension.product:
                        pageId = 2;
                        break;
                    case Dimension.media:
                        pageId = 6;
                        break;
                }
                DomainWebNavigation.SelectionPageInformation currentPage = currentModuleDescription.SelectionsPages.Cast<DomainWebNavigation.SelectionPageInformation>().ToList().FirstOrDefault(p => p.Id == pageId);///Private/Helps/UniverseProductSelectionHelp.aspx

                if (currentPage != null)
                {
                    listUniverseClientDescription += currentPage.LoadableUniversString;

                    levelsRules = new AdExpressLevelsRules(webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds), dimension);
                    tempBranchIds = levelsRules.GetAuthorizedBranches();
                    tempLevels = levelsRules.GetAuthorizedLevels();
                    if (tempBranchIds.Count > 0)
                        _allowedBranchesIds = tempBranchIds;
                    if (tempLevels.Count > 0)
                    {
                        foreach (var item in tempLevels)
                        {
                            if (_forbiddenLevelsId.Count == 0 || !_forbiddenLevelsId.Contains(item.ID))
                            {
                                _allowedLevelsId.Add(item.ID);
                            }
                        }
                    }
                }
                #region Default Branch
                defaultBranchId = currentPage.DefaultBranchId;//GetDefaultBranch(defaultBranchId, currentModuleDescription, currentPage);
                #endregion

            }
            var result = new Tuple<List<long>, List<int>, WebSession, int, int>(_allowedLevelsId, _allowedBranchesIds, webSession, siteLanguage, defaultBranchId);
            return result;
        }

        private int GetDefaultBranch(int defaultBranchId, DomainWebNavigation.Module currentModuleDescription, SelectionPageInformation currentPage)
        {
            if (webSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR || webSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)
            {
                foreach (SelectionPageInformation current in currentModuleDescription.SelectionsPages)
                {

                    if (webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess] != null && webSession.CustomerLogin[CustomerRightConstante.type.advertiserAccess].Length > 0 && current.ForceBranchId > 0)
                        defaultBranchId = current.ForceBranchId; //Force Branch advertiser
                    else defaultBranchId = current.DefaultBranchId; //Branch by default
                    break;

                }
            }
            else
            {
                defaultBranchId = currentPage.DefaultBranchId;
            }

            return defaultBranchId;
        }

        private Branch.type GetBrancheType(Dimension dimension, long idModule = 0)
        {

            switch (dimension)
            {
                case (Dimension.media):
                    return Branch.type.media;
                case (Dimension.product):
                    Branch.type type = (idModule == WebConstantes.Module.Name.FACEBOOK) ? Branch.type.productSocial : Branch.type.product;
                    return type;
                case (Dimension.advertisingAgency):
                    return Branch.type.advertisingAgency;
                case (Dimension.advertisementType):
                    return Branch.type.advertisementType;
                case (Dimension.profession):
                    return Branch.type.profession;
                default:
                    return 0;

            };
        }

        private AdExpressUniverse GetUniverseToSave(UniversGroupSaveRequest request)
        {
            AdExpressUniverse adExpressUniverse = new AdExpressUniverse(request.Dimension);
            int groupIndex = 0;
            Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            foreach (var tree in request.Trees)
            {
                NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, tree.AccessType);//tree=NomenclatureelementGroup
                //elementGroup.AccessType = tree.AccessType;
                foreach (var univers in tree.UniversLevels)
                {
                    Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
                    List<long> idUniversItems = new List<long>();
                    foreach (var item in univers.UniversItems)
                    {
                        idUniversItems.Add(item.Id);
                    }
                    if (idUniversItems.Any())
                        treeNomenclatureEG.AddItems(univers.Id, idUniversItems);
                }
                adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
                groupIndex++;
            }
            return adExpressUniverse;
        }

        private List<TNS.AdExpress.Classification.AdExpressUniverse> GetConcurrentUniversesToSave(UniversGroupSaveRequest request)
        {
            //TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(request.Dimension);
            List<TNS.AdExpress.Classification.AdExpressUniverse> adExpressUniverses = new List<TNS.AdExpress.Classification.AdExpressUniverse>(request.Trees.Count);
            int groupIndex = 0;
            foreach (var tree in request.Trees)
            {
                TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(request.Dimension);
                Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
                NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, tree.AccessType);//tree=NomenclatureelementGroup
                //elementGroup.AccessType = tree.AccessType;
                foreach (var univers in tree.UniversLevels)
                {
                    Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
                    List<long> idUniversItems = new List<long>();
                    foreach (var item in univers.UniversItems)
                    {
                        idUniversItems.Add(item.Id);
                    }
                    if (idUniversItems.Any())
                        treeNomenclatureEG.AddItems(univers.Id, idUniversItems);
                }
                adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
                adExpressUniverses.Add(adExpressUniverse);
                groupIndex++;
            }
            return adExpressUniverses;
        }
        private Labels LoadPageLabels(int siteLanguage)
        {
            var result = new Labels
            {
                Save = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.SaveUniversCode, siteLanguage),
                MyResults = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.ResultsCode, siteLanguage),
                SaveUnivers = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.SaveUniversCode, siteLanguage),
                UserUniversCode = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.UserSavedUniversCode, siteLanguage),
                MyResultsDescription = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.MyResultsDescription, siteLanguage),
                NoSavedUnivers = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.NoSavedUniversCode, siteLanguage),
                MoveSelectedResult = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.MoveSelectedResult, siteLanguage),
                MoveResultTitle = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.MoveSelectedResult, siteLanguage),
                Submit = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Submit, siteLanguage),
                RenameFolderTitle = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.RenameFolderTitle, siteLanguage),
                RenameNewFodler = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.RenameNewFodler, siteLanguage),
                SelectFolderToDelete = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.SelectFolderToDelete, siteLanguage),
                SelectFolder = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.SelectFolder, siteLanguage),
                RenameSelectedFolder = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.RenameSelectedFolder, siteLanguage),
                ErrorMsgNoFolderCreated = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.ErrorMsgNoFolderCreated, siteLanguage),
                CreateDirectory = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.CreateFolder, siteLanguage),
                RenameDirectory = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.RenameSelectedFolder, siteLanguage),
                DropDirectory = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.DropFolder, siteLanguage),
                Results = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Results, siteLanguage),
                Directories = GestionWeb.GetWebWord(WebConstantes.LanguageConstantes.Directories, siteLanguage)
            };
            return result;
        }

        private string GetSchema(long currentModule)
        {
            string schema = String.Empty;
            switch (currentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                case WebConstantes.Module.Name.FACEBOOK:
                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                case WebConstantes.Module.Name.NEW_CREATIVES:
                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                    schema = WebApplicationParameters.DataBaseDescription.
                    GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
                    break;
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                case WebConstantes.Module.Name.INDICATEUR:
                    schema = WebApplicationParameters.DataBaseDescription.
                    GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.recap01).Label;
                    break;
                case WebConstantes.Module.Name.HEALTH:
                    schema = WebApplicationParameters.DataBaseDescription.
                    GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.khealth01).Label;
                    break;
            }
            return schema;
        }
        private UniversGroupSaveResponse SetUnivers(WebSession webSession, UniversGroupSaveRequest request)
        {
            UniversGroupSaveResponse result = new UniversGroupSaveResponse();
            List<AdExpressUniverse> adExpressUniverses = new List<AdExpressUniverse>();
            AdExpressUniverse adExpressUniverse = new AdExpressUniverse(request.Dimension);
            Dictionary<int, AdExpressUniverse> universes = new Dictionary<int, AdExpressUniverse>();
            long idSelectedUniverse = request.UserUniversId ?? 0;
            long idSelectedDirectory = request.UniversGroupId;
            long idModule = webSession.CurrentModule;
            string mediaIds = null;
            if (request.MediaIds.Any())
                mediaIds = string.Join(", ", request.MediaIds);
            List<long> levels = new List<long>();
            int isDefault = request.IsDefaultUniverse ? 1 : 0;
            foreach (var item in request.Trees)
            {
                levels.AddRange(item.UniversLevels.Where(d => d.UniversItems.Any()).Select(x => x.Id).Except(levels).ToList());
            }

            #region Build univers
            switch (idModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.INDICATEUR:
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                case WebConstantes.Module.Name.HEALTH:
                case WebConstantes.Module.Name.NEW_CREATIVES:

                    adExpressUniverse = GetUniverseToSave(request);
                    if (adExpressUniverse == null || adExpressUniverse.Count() == 0)
                    {
                        result.ErrorMessage = GestionWeb.GetWebWord(927, webSession.SiteLanguage);
                        result.Success = false;
                    }
                    else
                    {
                        universes.Add(universes.Count, adExpressUniverse);
                    }
                    break;
                case WebConstantes.Module.Name.FACEBOOK:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    adExpressUniverses = GetConcurrentUniversesToSave(request);
                    int id = 0;
                    foreach (var item in adExpressUniverses)
                    {
                        universes.Add(id, item);
                        id++;
                    }
                    break;
                default:
                    break;

            }
            #endregion
            #region Sauvegarde de l'univers
            if (universes.Any())
            {
                Branch.type branchType = GetBrancheType(request.Dimension, idModule);
                string universeName = request.Name;
                if (String.IsNullOrEmpty(request.Name) && idSelectedUniverse != 0) //if (universeName.Length == 0 && !idSelectedUniverse.Equals("0"))
                {

                    if (UniversListDataAccess.UpdateUniverse(idSelectedUniverse, webSession, request.IdUniverseClientDescription, branchType.GetHashCode(), universes, isDefault))
                    {

                        // Validation : confirmation d'enregistrement de la requête
                        webSession.Source.Close();
                        result.ErrorMessage = GestionWeb.GetWebWord(921, webSession.SiteLanguage);
                        result.Success = true;
                    }
                    else
                    {
                        // Erreur : Echec de l'enregistrement de la requête	
                        webSession.Source.Close();
                        result.ErrorMessage = GestionWeb.GetWebWord(922, webSession.SiteLanguage);
                        result.Success = false;
                    }
                }
                else if (universeName.Length != 0 && universeName.Length < TNS.AdExpress.Constantes.Web.MySession.MAX_LENGHT_TEXT)
                {
                    if (!UniversListDataAccess.IsUniverseExist(webSession, universeName))
                    {
                        if (idSelectedDirectory > 0 && UniversListDataAccess.SaveUniverse(idSelectedDirectory, universeName, universes, branchType, request.IdUniverseClientDescription, webSession, isDefault, string.Join(", ", levels), mediaIds))
                        {
                            if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
                            {
                                if (request.IsDefaultUniverse)
                                {
                                    ManageFacebookDefaultUniverse(webSession, idSelectedUniverse, universes);
                                }
                                else
                                {
                                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                                        webSession.PrincipalAdvertisingAgnecyUniverses = universes;
                                    else
                                        webSession.PrincipalProductUniverses = universes;
                                    webSession.Save();
                                }
                            }
                            webSession.Source.Close();
                            result.ErrorMessage = GestionWeb.GetWebWord(921, webSession.SiteLanguage);
                            result.Success = true;
                        }
                        else
                        {
                            webSession.Source.Close();
                            result.ErrorMessage = GestionWeb.GetWebWord(922, webSession.SiteLanguage);
                            result.Success = false;
                        }
                    }
                    else
                    {
                        // Erreur : univers déjà existant
                        webSession.Source.Close();
                        result.ErrorMessage = GestionWeb.GetWebWord(923, webSession.SiteLanguage);
                        result.Success = false;
                    }
                }
                else if (universeName.Length == 0)
                {
                    // Erreur : Le champs est vide
                    webSession.Source.Close();
                    result.ErrorMessage = GestionWeb.GetWebWord(837, webSession.SiteLanguage);
                    result.Success = false;
                }
                else
                {
                    // Erreur : suppérieur à 50 caractères
                    webSession.Source.Close();
                    result.ErrorMessage = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                    result.Success = false;
                }

            }
            #endregion
            return result;
        }
        private ControllerDetails GetCurrentControllerDetails(long currentModule)
        {
            long currentModuleCode = 0;
            string currentController = string.Empty;
            string currentModuleIcon = "icon-chart";
            switch (currentModule)
            {
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    currentModuleCode = WebConstantes.LanguageConstantes.MediaScheduleCode;
                    currentController = "Selection";
                    currentModuleIcon = "icon-chart";
                    break;
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    currentModuleCode = WebConstantes.LanguageConstantes.PortfolioCode;
                    currentController = "Portfolio";
                    currentModuleIcon = "icon-layers";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                    currentModuleCode = WebConstantes.LanguageConstantes.LostWonCode;
                    currentController = "LostWon";
                    currentModuleIcon = "icon-calculator";
                    break;
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                    currentModuleCode = WebConstantes.LanguageConstantes.PresentAbsentCode;
                    currentController = "PresentAbsent";
                    currentModuleIcon = "icon-equalizer";
                    break;
                case WebConstantes.Module.Name.INDICATEUR:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalysisGraphics;
                    currentController = "Selection";
                    break;
                case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalysisDetailedReport;
                    currentController = "Selection";
                    currentModuleIcon = "icon-book-open";
                    break;
                case WebConstantes.Module.Name.FACEBOOK:
                    currentModuleCode = WebConstantes.LanguageConstantes.FacebookCode;
                    currentController = "Selection";
                    currentModuleIcon = "icon-social-facebook";
                    break;
                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                    currentModuleCode = WebConstantes.LanguageConstantes.MediaAgencyAnalysis;
                    currentController = "Selection";
                    currentModuleIcon = "icon-picture";
                    break;
                case WebConstantes.Module.Name.NEW_CREATIVES:
                    currentModuleCode = WebConstantes.LanguageConstantes.NewCreatives;
                    currentController = "Selection";
                    currentModuleIcon = "icon-camrecorder";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalyseDispositifsLabel;
                    currentController = "Selection";
                    currentModuleIcon = "icon-puzzle";
                    break;
                case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                    currentModuleCode = WebConstantes.LanguageConstantes.AnalyseProgrammesLabel;
                    currentController = "Selection";
                    currentModuleIcon = "icon-puzzle";
                    break;
                case WebConstantes.Module.Name.HEALTH:
                    currentModuleCode = WebConstantes.LanguageConstantes.Health;
                    currentController = "Selection";
                    currentModuleIcon = "icon-heart";
                    break;
                default:
                    break;
            }
            var current = new ControllerDetails
            {
                ModuleCode = currentModuleCode,
                Name = currentController,
                ModuleId = currentModule,
                ModuleIcon = currentModuleIcon
            };
            return current;
        }
        private void ClearProduct(WebSession webSession)
        {
            webSession.PrincipalProductUniverses.Clear();
            webSession.Save();
        }
        private List<UserUnivers> GetUniverses(Dimension dimension, WebSession webSession, long idGroup = 0, bool getDefaultUniverse = false)
        {
            List<UserUnivers> result = new List<UserUnivers>();
            var tuple = GetAllowedIds(webSession, dimension);
            var allowedLevels = RequireForceLevel(webSession.CurrentModule) ? new List<long> { TNSClassificationLevels.MEDIA } : tuple.Item1;
            var listUniverseClientDescription = TNS.AdExpress.Constantes.Web.LoadableUnivers.GENERIC_UNIVERSE.ToString();
            var branch = (dimension == Dimension.product) ? Branch.type.product.GetHashCode().ToString() : Branch.type.media.GetHashCode().ToString();
            if (webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
                branch = Branch.type.productSocial.GetHashCode().ToString();
            List<long> allowedFilters = GetAllowedFilters(webSession, dimension);
            var data = UniversListDataAccess.GetData(webSession, branch, "", allowedLevels, allowedFilters);
            if (data != null && data.AsEnumerable().Any())
            {
                var list = data.AsEnumerable().Select(p => new
                {
                    GroupID = p.Field<long?>("ID_GROUP_UNIVERSE_CLIENT"),
                    GroupDescription = p.Field<string>("GROUP_UNIVERSE_CLIENT"),
                    UniversID = p.Field<long?>("ID_UNIVERSE_CLIENT"),
                    UniversDescription = p.Field<string>("UNIVERSE_CLIENT"),
                    IsDefault = (p["IS_DEFAULT"] != null && Convert.ToInt32(p["IS_DEFAULT"]) == 1) ? true : false
                }).ToList();
                if (idGroup > 0)
                    list = list.Where(p => p.GroupID == idGroup).ToList();
                foreach (var item in list)
                {
                    UserUnivers UserUnivers = new UserUnivers
                    {
                        ParentId = item.GroupID ?? 0,
                        ParentDescription = item.GroupDescription,
                        Id = item.UniversID ?? 0,
                        Description = item.UniversDescription,
                        IsDefault = item.IsDefault
                    };
                    result.Add(UserUnivers);
                }
                if (getDefaultUniverse && webSession.CurrentModule == WebConstantes.Module.Name.FACEBOOK)
                    result = result.Where(p => p.IsDefault).ToList();
            }
            return result;
        }
        private void ManageFacebookDefaultUniverse(WebSession webSession, long idSelectedUniverse, Dictionary<int, AdExpressUniverse> universes)
        {
            bool success = false;
            UserUnivers defaultUniverse = GetUniverses(Dimension.product, webSession, 0, true).FirstOrDefault();
            if (defaultUniverse != null && defaultUniverse.Id == idSelectedUniverse)
            {
                success = UniversListDataAccess.UpdateDefaultFcbUniverse(defaultUniverse.Id, webSession, 0);
            }
            else
            {
                success = UniversListDataAccess.UpdateDefaultFcbUniverses(idSelectedUniverse, defaultUniverse.Id, webSession);
            }
            if (success)
            {
                if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_MANDATAIRES)
                    webSession.PrincipalAdvertisingAgnecyUniverses = universes;
                else
                    webSession.PrincipalProductUniverses = universes;
                webSession.Save();
            }

        }

        private List<long> GetAllowedFilters(WebSession webSession, Dimension dimension)
        {

            string filter = string.Empty;
            DomainWebNavigation.Module module = ModulesList.GetModule(webSession.CurrentModule);

            switch (dimension)
            {
                case Dimension.media:
                    switch (webSession.CurrentModule)
                    {
                        case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                        case WebConstantes.Module.Name.HEALTH:
                            return null;
                        case WebConstantes.Module.Name.INDICATEUR:
                        case WebConstantes.Module.Name.TABLEAU_DYNAMIQUE:
                            return GetVehiclesListAnalysis(webSession);
                        default:
                            return module.AllowedMediaUniverse.GetVehicles();
                    }
                default:
                    return null;
            }
        }

        private bool RequireForceLevel(long moduleId)
        {
            switch (moduleId)
            {
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                    return true;
                default:
                    return false;
            }
        }

        private List<long> GetVehiclesListAnalysis(WebSession webSession)
        {
            VehicleListDataAccess vl = new VehicleListDataAccess(webSession);
            DataTable dtVehicle = vl.List;


            List<long> ids = new List<long>();
           

            if (!WebApplicationParameters.CountryCode.Equals(WebConstantes.CountryCode.TURKEY))
            {
                ids.Add(VehiclesInformation.Get(TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.plurimedia).DatabaseId);
            }

            foreach (DataRow row in dtVehicle.Rows)
            {
                long id = Int64.Parse(row["ID_VEHICLE"].ToString());
                if (!ids.Contains(id))
                    ids.Add(id);
            }

            return ids;
        }
        #endregion
    }

 
}

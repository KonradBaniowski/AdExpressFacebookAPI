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
using AutoMapper;
using TNS.AdExpress.Web.Core.DataAccess.ClassificationList;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class UniverseService : IUniverseService
    {
        private WebSession webSession = null;
        private const int MarketSelectionPageId = 2;// /Private/Helps/UniverseProductSelectionHelp.aspx
        private const long Capacity = 1000;
        private const long ExceptionMsg = 922;
        private const long SecurityMsg = 2285;
        private const long OverLimitMsgCode = 2286;
        public const long ElementLabelCode = 2278;
        public const int MaxIncludeNbr = 2;
        public const int MaxExcludeNbr = 1;

        public List<UniverseItem> GetItems(int universeLevelId, string keyWord, string idSession)
        {
            webSession = (WebSession)WebSession.Load(idSession);
            //CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
              cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            classficationDAL.DBSchema = WebApplicationParameters.DataBaseDescription.
            GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
            DataTable data = classficationDAL.GetItems(universeLevelId, keyWord).Tables[0];
            var result = new List<UniverseItem>();
            foreach (var item in data.AsEnumerable())
            {
                var UItem = new UniverseItem
                {
                    IdItem = int.Parse(item.ItemArray[0].ToString()),
                    Label = item.ItemArray[1].ToString()
                };
                result.Add(UItem);
            }
            return result;
        }


        public List<UniverseItem> GetItems(int levelId, string selectedClassificationItemsIds, int selectedLevelId, string idSession)
        {
            webSession = (WebSession)WebSession.Load(idSession);
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
              cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            classficationDAL.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
            DataTable data = classficationDAL.GetItems(levelId, selectedClassificationItemsIds, selectedLevelId).Tables[0];
            var result = new List<UniverseItem>();
            foreach (var item in data.AsEnumerable())
            {
                var UItem = new UniverseItem
                {
                    IdItem = int.Parse(item.ItemArray[0].ToString()),
                    Label = item.ItemArray[1].ToString()
                };
                result.Add(UItem);
            }
            return result;
        }
        public UniversBranchResult GetBranches(string webSessionId, Dimension dimension, bool selectionPage = true)
        {
            var tuple = GetAllowedIds(webSessionId, dimension, selectionPage);

            var result = new UniversBranchResult
            {
                Branches = new List<UniversBranch>(),
                SiteLanguage = tuple.Item4,
                DefaultBranchId = tuple.Item5,
                Trees = new List<Tree>(MaxIncludeNbr+ MaxExcludeNbr)
            };
            var allowedBranchesIds = tuple.Item2;
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
                        if (!allUnivers.Any(p=>p.Id==level.Id))
                        {
                            allUnivers.Add(level);
                        }
                        branch.UniversLevels.Add(level);
                    }
                    result.Branches.Add(branch);
                }
                // Create trees according to the dimension
                int idTree = 1;
                foreach (AccessType type in Enum.GetValues(typeof(AccessType)))
                {
                    var maxTreesNbr = (type == AccessType.includes) ? MaxIncludeNbr : MaxExcludeNbr;//(Enum.GetValues(typeof(AccessType)))
                    for (int i = 1; i <= maxTreesNbr; i++)
                    {
                        var tree = new Tree
                        {
                            Id= idTree,
                            LabelId = ElementLabelCode,
                            UniversLevels = allUnivers,
                            AccessType = type
                        };
                        idTree++;
                        result.Trees.Add(tree);
                    }
                }               
            }
            return result;
        }

        public UniversGroupsResponse GetUserSavedUniversGroups(string webSessionId, Dimension dimension, bool selectionPage = true)
        {
            var tuple = GetAllowedIds(webSessionId, dimension, selectionPage);
            UniversGroupsResponse result = new UniversGroupsResponse
            {
                UniversGroups = new List<UserUniversGroup>(),
                SiteLanguage = tuple.Item4
            };
            var allowedLevels = tuple.Item1;
            var listUniverseClientDescription = TNS.AdExpress.Constantes.Web.LoadableUnivers.GENERIC_UNIVERSE.ToString();
            var branch = TNS.AdExpress.Constantes.Classification.Branch.type.product.GetHashCode().ToString();//To review how the vaule is set with Dédé.
            var data = TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetData(tuple.Item3, branch.ToString(), listUniverseClientDescription, allowedLevels);
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
            return result;
        }
        
        public List<Tree> GetTreesByUserUnivers(int userUniversId, string webSessionId, Dimension dimension)
        {
            List<Tree> result = new List<Tree>();
            try
            {
                #region try block
                #region Get AdExpressUniverse
                int index = 0;
                webSession = (WebSession)WebSession.Load(webSessionId);
                Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> Universes = (Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>)
                TNS.AdExpress.Web.Core.DataAccess.ClassificationList.UniversListDataAccess.GetObjectUniverses(userUniversId, webSession);
                var adExpressUniverse = Universes[index];
                #endregion
                #region Iterate by Access Type
                if (adExpressUniverse !=null && adExpressUniverse.Count() > 0)
                { 
                foreach (AccessType type in Enum.GetValues(typeof(AccessType)))
                {
                    List<NomenclatureElementsGroup> elementsGroups = new List<NomenclatureElementsGroup>();
                    
                        elementsGroups = (type == AccessType.excludes)?adExpressUniverse.GetExludes(): adExpressUniverse.GetIncludes();
                        if (elementsGroups != null && elementsGroups.Count > 0)
                        {
                            #region Repository 
                            foreach (NomenclatureElementsGroup elementGroup in elementsGroups)
                            {
                                Tree tree = new Tree
                                {
                                    AccessType = type,
                                    UniversLevels = new List<UniversLevel>()
                                };                                                              
                                DataSet ds = null;
                                List<long> oldLevelsId = new List<long>();
                                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.classification];
                                if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
                                object[] param = new object[2];
                                param[0] = webSession;
                                param[1] = dimension;
                                ClassificationDAL classficationDAL = (ClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                                classficationDAL.DBSchema = WebApplicationParameters.DataBaseDescription.GetSchema(TNS.AdExpress.Domain.DataBaseDescription.SchemaIds.adexpr03).Label;
                                var tuple = GetAllowedIds(webSessionId, dimension, true);
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
                                                foreach (DataRow dr in ds.Tables[0].Rows)
                                                {
                                                    UniversLevel level = new UniversLevel();
                                                    level.Id = (Int64)dr[0];
                                                    level.Label = dr[1].ToString();
                                                    tree.UniversLevels.Add(level);                                                                                                      
                                                }
                                                tree.Id = (int)currentLevel;
                                            }
                                        }
                                        oldLevelsId.Add(currentLevel);
                                    }
                                }
                                cl = null;
                                classficationDAL = null;                                
                                result.Add(tree);
                                #endregion
                            }
                        }
                    }
                }
                #endregion
                #endregion
            }


            catch (System.Exception err)
            {
                //TODO
                //throw (new TNS.AdExpress.Web.Controls.Exceptions.SelectItemsInClassificationWebControlException("Impossible de construire le Treeview Obout", err));
            }
            
            return result;
        }

        public UniversGroupSaveResponse SaveUserUnivers(UniversGroupSaveRequest request)
        {
            UniversGroupSaveResponse result = new UniversGroupSaveResponse();
            #region To be Refactored
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();
            try
            {
                if (request.Trees.Count > 0)
                {
                    webSession = (WebSession)WebSession.Load(request.WebSessionId);
                    long idSelectedUniverse = request.UserUniversId ?? 0;
                    long idSelectedDirectory = request.UniversGroupId;

                    //Identification de la branche de l'univers					
                    TNS.AdExpress.Constantes.Classification.Branch.type branchType = GetBrancheType(request.Dimension);

                    //Get universe to save
                    TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = null;// TODO //GetUniverseToSave();

                    if (adExpressUniverse == null || adExpressUniverse.Count() == 0)
                    {
                        // Erreur : Aucun groupe d'univers, veuillez en créer un.
                        result.ErrorMessage = GestionWeb.GetWebWord(927, webSession.SiteLanguage);
                        result.Success = false;
                    }
                    else {
                        #region Sauvegarde de l'univers
                        string universeName = request.Name;

                        if (String.IsNullOrEmpty(request.Name)&& idSelectedUniverse !=0) //if (universeName.Length == 0 && !idSelectedUniverse.Equals("0"))
                        {

                            //Add AdExpress universe to collection
                            universes.Add(universes.Count, adExpressUniverse);

                            if (UniversListDataAccess.UpdateUniverse(idSelectedUniverse, webSession, request.IdUniverseClientDescription, branchType.GetHashCode(), universes))
                            {

                                // Validation : confirmation d'enregistrement de la requête
                                webSession.Source.Close();
                                result.ErrorMessage = GestionWeb.GetWebWord(921, webSession.SiteLanguage);
                                result.Success = true;
                            }
                            else {
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

                                //Add AdExpress universe to collection
                                universes.Add(universes.Count, adExpressUniverse);
                                if (idSelectedDirectory>0 && UniversListDataAccess.SaveUniverse(idSelectedDirectory, universeName, universes, branchType, request.IdUniverseClientDescription, webSession))
                                //if (idSelectedDirectory != null && idSelectedDirectory.Length > 0 && UniversListDataAccess.SaveUniverse(Int64.Parse(idSelectedDirectory), universeName, universes, branchType, idUniverseClientDescription, _webSession))
                                {
                                    // Validation : confirmation d'enregistrement de l'univers
                                    webSession.Source.Close();
                                    result.ErrorMessage = GestionWeb.GetWebWord(921, webSession.SiteLanguage);
                                    result.Success = true;
                                }
                                else {
                                    // Erreur : Echec de l'enregistrement de l'univers
                                    webSession.Source.Close(); 
                                    result.ErrorMessage = GestionWeb.GetWebWord(922, webSession.SiteLanguage);
                                    result.Success = false;
                                }
                            }
                            else {
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
                        else {
                            // Erreur : suppérieur à 50 caractères
                            webSession.Source.Close();
                            result.ErrorMessage = GestionWeb.GetWebWord(823, webSession.SiteLanguage);
                            result.Success = false;
                        }

                        #endregion
                    }
                }

                else {
                    // Erreur : Impossible de sauvegarder, pas de groupe d'univers créé
                    result.ErrorMessage = GestionWeb.GetWebWord(925, webSession.SiteLanguage);
                    result.Success = false;
                }
            }
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
            #endregion
            return result;
        }

        #region private methods
        private Tuple<List<long>, List<int>, WebSession, int, int> GetAllowedIds(string webSessionId, Dimension dimension, bool selectionPage = true)
        {
            webSession = (WebSession)WebSession.Load(webSessionId);
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
                DomainWebNavigation.SelectionPageInformation currentPage = currentModuleDescription.SelectionsPages.Cast<DomainWebNavigation.SelectionPageInformation>().ToList().FirstOrDefault(p => p.Id == MarketSelectionPageId);///Private/Helps/UniverseProductSelectionHelp.aspx

                if (currentPage != null)
                {
                    listUniverseClientDescription += currentPage.LoadableUniversString;
                    defaultBranchId = currentPage.DefaultBranchId;
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
            }
            var result = new Tuple<List<long>, List<int>, WebSession, int, int>(_allowedLevelsId, _allowedBranchesIds, webSession, siteLanguage, defaultBranchId);
            return result;
        }

        private TNS.AdExpress.Constantes.Classification.Branch.type GetBrancheType(Dimension dimension)
        {

            switch (dimension)
            {
                case (Dimension.media):
                    return TNS.AdExpress.Constantes.Classification.Branch.type.media;
                case (Dimension.product):
                    return TNS.AdExpress.Constantes.Classification.Branch.type.product;
                case (Dimension.advertisingAgency):
                    return TNS.AdExpress.Constantes.Classification.Branch.type.advertisingAgency;
                case (Dimension.advertisementType):
                    return TNS.AdExpress.Constantes.Classification.Branch.type.advertisementType;
                case (Dimension.profession):
                    return TNS.AdExpress.Constantes.Classification.Branch.type.profession;
                default:
                    return 0;

            };
        }
        #endregion
    }
}

﻿using Kantar.AdExpress.Service.Core.BusinessService;
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
            List<ClientUnivers> clientUniversList = new List<ClientUnivers>();
            if (data != null && data.Rows.Count > 0)
            {
                foreach (DataRow row in data.Rows)
                {
                    ClientUnivers clientUnivers = new ClientUnivers
                    {
                        ParentId = (long)row[0],
                        ParentDescription = row[1].ToString(),
                        Id = (long)row[2],
                        Description = row[3].ToString()
                    };
                    clientUniversList.Add(clientUnivers);
                }
                var groupedUniversList = clientUniversList.GroupBy(p => p.ParentId);
                foreach (var item in groupedUniversList)
                {
                    UserUniversGroup universGroup = new UserUniversGroup
                    {
                        Id = item.Key,
                        Description = item.FirstOrDefault().ParentDescription,
                        ClientUnivers = item.ToList(),
                        Count = item.Count()
                    };
                    result.UniversGroups.Add(universGroup);
                }
            }
            return result;
        }

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

    }
}

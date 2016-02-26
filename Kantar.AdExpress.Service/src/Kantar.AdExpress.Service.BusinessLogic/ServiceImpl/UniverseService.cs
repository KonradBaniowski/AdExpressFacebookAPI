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
            var result = new UniversBranchResult
            {
                Branches = new List<UniversBranch>()
            };
            webSession = (WebSession)WebSession.Load(webSessionId);
            result.SiteLanguage = webSession.SiteLanguage;
            string listUniverseClientDescription = "";
            ILevelsRules levelsRules = null;
            List<int> tempBranchIds = new List<int>();
            List<UniverseLevel> tempLevels = new List<UniverseLevel>();
            List<long> _forbiddenLevelsId = new List<long>();
            List<int> _allowedBranchesIds = new List<int>();
            List<long> _allowedLevelsId = new List<long>();
            string themeName = WebApplicationParameters.Themes[webSession.SiteLanguage].Name;
            DomainWebNavigation.Module currentModuleDescription = DomainWebNavigation.ModulesList.GetModule(webSession.CurrentModule);
            if (selectionPage)
            {
                //Apply rigth Rules for getting levels and branches
                DomainWebNavigation.SelectionPageInformation currentPage = currentModuleDescription.SelectionsPages.Cast<DomainWebNavigation.SelectionPageInformation>().ToList().FirstOrDefault(p => p.Id == MarketSelectionPageId);///Private/Helps/UniverseProductSelectionHelp.aspx

                if (currentPage != null)
                {
                    listUniverseClientDescription += currentPage.LoadableUniversString;
                    result.DefaultBranchId = currentPage.DefaultBranchId;
                    levelsRules = new AdExpressLevelsRules(webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds), dimension);
                    tempBranchIds = levelsRules.GetAuthorizedBranches();
                    tempLevels = levelsRules.GetAuthorizedLevels();
                    if (tempBranchIds.Count > 0)
                        _allowedBranchesIds = tempBranchIds;
                    if (tempLevels.Count > 0)
                    {
                        for (int i = 0; i < tempLevels.Count; i++)
                        {
                            if (_forbiddenLevelsId == null || _forbiddenLevelsId.Count == 0 || !_forbiddenLevelsId.Contains(tempLevels[i].ID))
                                _allowedLevelsId.Add(tempLevels[i].ID);
                        }
                    }
                }
            }
            //else {
            //TODO  For Optional Selection
            //listUniverseClientDescription += currentPage.LoadableUniversString;
            ////Apply rigth Rules for getting levels and branches
            //levelsRules = new AdExpressLevelsRules(webSession, currentPage.AllowedBranchesIds, UniverseLevels.GetList(currentPage.AllowedLevelsIds), _dimension);
            //tempBranchIds = levelsRules.GetAuthorizedBranches();
            //tempLevels = levelsRules.GetAuthorizedLevels();
            //if (tempBranchIds != null && tempBranchIds.Count > 0)
            //    _allowedBranchesIds = tempBranchIds;
            //if (tempLevels != null && tempLevels.Count > 0)
            //{
            //    for (int i = 0; i < tempLevels.Count; i++)
            //    {
            //        if (_forbiddenLevelsId == null || _forbiddenLevelsId.Count == 0 || !_forbiddenLevelsId.Contains(tempLevels[i].ID))
            //            _allowedLevelsId.Add(tempLevels[i].ID);
            //    }
            //}      
            //}
            if (_allowedBranchesIds.Any())
            {
                foreach (var id in _allowedBranchesIds)
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
                            ExceptionMessage= GestionWeb.GetWebWord(ExceptionMsg, result.SiteLanguage)
                        };
                        branch.UniversLevels.Add(level);
                    }
                    result.Branches.Add(branch);
                }
            }
            return result;
        }
    }
}

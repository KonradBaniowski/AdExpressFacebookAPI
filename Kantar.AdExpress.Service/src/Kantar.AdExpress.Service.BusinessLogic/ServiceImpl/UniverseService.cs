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
using DomainWebNavigation=TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Classification.DAL;
using TNS.Classification.Universe;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class UniverseService : IUniverseService
    {
        private WebSession webSession = null;
        private const int MarketSelectionPageId = 2;// /Private/Helps/UniverseProductSelectionHelp.aspx

        public List<UniverseItem> GetItems(int universeLevelId, string keyWord, string idSession)
        {
            webSession = (WebSession)WebSession.Load(idSession);
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
              cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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
            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.sourceProvider];
            if (cl == null) throw (new NullReferenceException("Core layer is null for the Classification DAL"));
            object[] param = new object[1];
            param[0] = webSession;
            IClassificationDAL classficationDAL = (IClassificationDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(
                string.Format("{0}Bin\\{1}", AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName),
              cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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
        public UniversBranchResult GetBranches(string webSessionId, Dimension dimension, bool selectionPage=true)
        {
            var result = new UniversBranchResult();
            
            webSession = (WebSession)WebSession.Load(webSessionId);
            result.SiteLanguage = webSession.SiteLanguage;
            string listUniverseClientDescription = "";
            ILevelsRules levelsRules = null;
            List<int> tempBranchIds = new List<int>();
            List<UniverseLevel> tempLevels = new List<UniverseLevel>();
            List<Int64> _forbiddenLevelsId = new List<long>();
            List<int> _allowedBranchesIds = new List<int>();
            List<Int64> _allowedLevelsId = new List<long>();
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
            else {
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
            }
            return result;
        }
    }
}

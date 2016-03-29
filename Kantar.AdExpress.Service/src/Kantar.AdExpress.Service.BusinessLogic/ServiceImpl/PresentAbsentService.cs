﻿#define Debug

using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.PresentAbsent;
using System.Reflection;
using TNS.Classification.Universe;


namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
   public class PresentAbsentService : IPresentAbsentService
    {
        private WebSession _customerSession = null;

        public GridResult GetGridResult(string idWebSession)
        {
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the present/absent result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var presentAbsentResult = (IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            var gridResult = presentAbsentResult.GetGridResult();
            return gridResult;
        }

        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);
#if Debug
            //TODO : Resultat pour calendrier d'actiion : a enlever apres tests
            //_customerSession.CurrentTab = 6;

            //TODO :  selection support :  : a enlever apres tests
            TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
            Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

            int groupIndex = 0;
            Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, AccessType.includes);
            Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
            List<long> idUniversItems = new List<long>();
            idUniversItems.Add(2003);//EUROPE 1
            treeNomenclatureEG.AddItems(TNSClassificationLevels.MEDIA, idUniversItems);
            adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
            universes.Add(universes.Count, adExpressUniverse);


            adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
            elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, AccessType.includes);
            elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
            idUniversItems = new List<long>();
            idUniversItems.Add(2001);//RMC INFO
            treeNomenclatureEG.AddItems(TNSClassificationLevels.MEDIA, idUniversItems);
            adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
            universes.Add(universes.Count, adExpressUniverse);

            _customerSession.PrincipalMediaUniverses = universes;

            //ArrayList levelIds = new ArrayList();
            //levelIds.Add(11);
            //levelIds.Add(12);
            //levelIds.Add(10);            
            //_customerSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levelIds, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

            _customerSession.Save();
#endif           

            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the present/absent result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var presentAbsentResult = (IPresentAbsentResult)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            data = presentAbsentResult.GetResult();
            return data;
        }
    }
}
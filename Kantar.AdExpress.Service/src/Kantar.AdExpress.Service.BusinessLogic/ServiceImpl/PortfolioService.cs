#define Debug
using System;
using Kantar.AdExpress.Service.Core.BusinessService;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpressI.Portofolio;
using System.Reflection;
using TNS.Classification.Universe;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Level;
using System.Collections;
using System.Data;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class PortfolioService : IPortfolioService
    {
        private WebSession _customerSession = null;

        public GridResult GetGridResult(string idWebSession)
        {
            _customerSession = (WebSession)WebSession.Load(idWebSession);
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);

#if Debug
            ////TODO : Resultat pour calendrier d'actiion : a enlever apres tests
            //// _customerSession.CurrentTab = 6;

            //_customerSession.SelectionUniversMedia.Nodes.Clear();
            //System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("RADIO");
            //tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, 2, "RADIO");
            //_customerSession.SelectionUniversMedia.Nodes.Add(tmpNode);
            //_customerSession.CurrentUniversMedia = _customerSession.SelectionUniversMedia;

            ////TODO :  selection support :  : a enlever apres tests
            //TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
            //Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

            //int groupIndex = 0;
            //Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            //NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, AccessType.includes);
            //Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
            //List<long> idUniversItems = new List<long>();
            //idUniversItems.Add(2003);//EUROPE 1
            //treeNomenclatureEG.AddItems(TNSClassificationLevels.MEDIA, idUniversItems);
            //adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
            //universes.Add(universes.Count, adExpressUniverse);
            //_customerSession.PrincipalMediaUniverses = universes;

            ////ArrayList levelIds = new ArrayList();
            ////levelIds.Add(11);
            ////levelIds.Add(12);
            ////levelIds.Add(10);            
            ////_customerSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levelIds, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

            //_customerSession.Save();
#endif

            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            switch (_customerSession.CurrentTab)
            {

                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_MEDIA:
                    return portofolioResult.GetDetailMediaGridResult(false);
                    
                case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.STRUCTURE:
                    return portofolioResult.GetStructureGridResult(false);
                    
                default:
                   return  portofolioResult.GetGridResult();
                    

            }
           
        }

        public List<GridResult> GetGraphGridResult(string idWebSession)
        {

            _customerSession = (WebSession)WebSession.Load(idWebSession);

            

            TNS.AdExpress.Domain.Web.Navigation.Module module = _customerSession.CustomerLogin.GetModule(_customerSession.CurrentModule);
            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
            object[] parameters = new object[1];
            parameters[0] = _customerSession;
            IPortofolioResults portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);


            return portofolioResult.GetGraphGridResult();

        }


        public ResultTable GetResultTable(string idWebSession)
        {
            ResultTable data = null;
            var module = ModulesList.GetModule(WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE);
            _customerSession = (WebSession)WebSession.Load(idWebSession);
#if Debug
            ////TODO : Resultat pour calendrier d'actiion : a enlever apres tests
            //// _customerSession.CurrentTab = 6;

            //_customerSession.SelectionUniversMedia.Nodes.Clear();
            //System.Windows.Forms.TreeNode tmpNode = new System.Windows.Forms.TreeNode("RADIO");
            //tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, 2, "RADIO");
            //_customerSession.SelectionUniversMedia.Nodes.Add(tmpNode);
            //_customerSession.CurrentUniversMedia = _customerSession.SelectionUniversMedia;

            ////TODO :  selection support :  : a enlever apres tests
            //TNS.AdExpress.Classification.AdExpressUniverse adExpressUniverse = new TNS.AdExpress.Classification.AdExpressUniverse(Dimension.media);
            //Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse> universes = new Dictionary<int, TNS.AdExpress.Classification.AdExpressUniverse>();

            //int groupIndex = 0;
            //Dictionary<int, NomenclatureElementsGroup> elementGroupDictionary = new Dictionary<int, NomenclatureElementsGroup>();
            //NomenclatureElementsGroup treeNomenclatureEG = new NomenclatureElementsGroup(groupIndex, AccessType.includes);
            //Dictionary<long, List<long>> elementGroup = new Dictionary<long, List<long>>();// UniversLevel=ElementGroup                    
            //List<long> idUniversItems = new List<long>();
            //idUniversItems.Add(2003);//EUROPE 1
            //treeNomenclatureEG.AddItems(TNSClassificationLevels.MEDIA, idUniversItems);
            //adExpressUniverse.AddGroup(groupIndex, treeNomenclatureEG);
            //universes.Add(universes.Count, adExpressUniverse);
            //_customerSession.PrincipalMediaUniverses = universes;

            ////ArrayList levelIds = new ArrayList();
            ////levelIds.Add(11);
            ////levelIds.Add(12);
            ////levelIds.Add(10);            
            ////_customerSession.GenericProductDetailLevel = new TNS.AdExpress.Domain.Level.GenericDetailLevel(levelIds, TNS.AdExpress.Constantes.Web.GenericDetailLevel.SelectedFrom.defaultLevels);

            //_customerSession.Save();
#endif



            if (module.CountryRulesLayer == null) throw (new NullReferenceException("Rules layer is null for the portofolio result"));
            var parameters = new object[1];
            parameters[0] = _customerSession;
            var portofolioResult = (IPortofolioResults)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory
                + @"Bin\" + module.CountryRulesLayer.AssemblyName, module.CountryRulesLayer.Class, false, BindingFlags.CreateInstance
                | BindingFlags.Instance | BindingFlags.Public, null, parameters, null, null);
            data = portofolioResult.GetResultTable();
            return data;
        }
    }
}

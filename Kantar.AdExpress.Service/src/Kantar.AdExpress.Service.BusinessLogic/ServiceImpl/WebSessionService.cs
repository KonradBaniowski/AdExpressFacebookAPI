using Kantar.AdExpress.Service.Core.BusinessService;
using System;
using System.Collections;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Web.Core.Sessions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using CstWebCustomer = TNS.AdExpress.Constantes.Customer;
using CstWeb = TNS.AdExpress.Constantes.Web;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Domain.Units;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class WebSessionService : IWebSessionService
    {
        private WebSession _webSession = null;
        public WebSessionResponse SaveMediaSelection (List<long> mediaIds, string webSessionId)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                MediaScheduleStep = MediaScheduleStep.Media
            };
            try
            {
                var _webSession = (WebSession)WebSession.Load(webSessionId);
                WebNavigation.Module _currentModule = WebNavigation.ModulesList.GetModule(_webSession.CurrentModule);
                _webSession.Insert = TNS.AdExpress.Constantes.Web.CustomerSessions.Insert.total;
                List<System.Windows.Forms.TreeNode> levelsSelected = new List<System.Windows.Forms.TreeNode>();
                System.Windows.Forms.TreeNode tmpNode;
                bool containsSearch = false;
                bool containsSocial = false;
                foreach (var item in mediaIds)
                {
                    
                        tmpNode = new System.Windows.Forms.TreeNode(item.ToString());
                        tmpNode.Tag = new LevelInformation(TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess, item, item.ToString());
                        tmpNode.Checked = true;
                        levelsSelected.Add(tmpNode);
                        if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.search)
                            && item.ToString() == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.search).DatabaseId.ToString())
                            containsSearch = true;
                        if (VehiclesInformation.Contains(DBClassificationConstantes.Vehicles.names.social)
                            && item.ToString() == VehiclesInformation.Get(DBClassificationConstantes.Vehicles.names.social).DatabaseId.ToString())
                            containsSocial = true;
                    
                }
                if (levelsSelected.Count == 0)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(1052, _webSession.SiteLanguage);
                }
                else if (containsSearch && levelsSelected.Count > 1)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(3011, _webSession.SiteLanguage);
                }
                else if (containsSocial && levelsSelected.Count > 1)
                {
                    response.ErrorMessage = GestionWeb.GetWebWord(3030, _webSession.SiteLanguage);
                }
                else {

                    //Reinitialize banners selection if change vehicle
                    Dictionary<Int64, VehicleInformation> vehicleInformationList = _webSession.GetVehiclesSelected();
                    if (mediaIds.Count != vehicleInformationList.Count)
                    {
                        foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                        {
                            if (!vehicleInformationList.ContainsKey(((LevelInformation)node.Tag).ID))
                            {
                                _webSession.SelectedBannersFormatList = string.Empty;
                                break;
                            }
                        }
                    }
                    else
                    {
                        _webSession.SelectedBannersFormatList = string.Empty;
                    }

                    // Sauvegarde de la sélection dans la session
                    //Si la sélection comporte des éléments, on la vide
                    _webSession.SelectionUniversMedia.Nodes.Clear();

                    foreach (System.Windows.Forms.TreeNode node in levelsSelected)
                    {
                        _webSession.SelectionUniversMedia.Nodes.Add(node);
                        // Tracking
                        _webSession.OnSetVehicle(((LevelInformation)node.Tag).ID);
                    }

                    //verification que l unite deja sélectionnée convient pour tous les medias
                    var vehicleSelection = _webSession.GetSelection(_webSession.SelectionUniversMedia, CstWebCustomer.Right.type.vehicleAccess);

                    List<CstWeb.CustomerSessions.Unit> unitList = FctUtilities.Units.getUnitsFromVehicleSelection(vehicleSelection);
                    unitList = GetAllowedUnits(unitList, _currentModule.AllowedUnitEnumList);
                    if (unitList.Count == 0)
                    {
                        response.ErrorMessage = GestionWeb.GetWebWord(2541, _webSession.SiteLanguage);

                    }
                    else
                    {
                        _webSession.Save();
                        response.Success = true;
                    }
                }
            }
            catch (System.Exception exc)
            {
                if (exc.GetType() != typeof(System.Threading.ThreadAbortException))
                {
                    //this.OnError(new TNS.AdExpress.Web.UI.ErrorEventArgs(this, exc, _webSession));                    
                }
            }

            return response;
    }

    public WebSessionResponse SaveMarketSelection(string webSessionId)
        {
            WebSessionResponse response = new WebSessionResponse
            {
                MediaScheduleStep = MediaScheduleStep.Market
            };
            return response;
        }

        // TODO To be deplaced to framework 
        #region To be deplaced
        /// <summary>
        /// Get allowed units 
        /// </summary>
        /// <param name="vehicleSelection">List of media joined by commas</param>
        /// <returns>Allowed units</returns>
        public static List<CstWeb.CustomerSessions.Unit> GetAllowedUnits(List<CstWeb.CustomerSessions.Unit> unitList, List<CstWeb.CustomerSessions.Unit> AllowedUnitEnumList)
        {
            List<CstWeb.CustomerSessions.Unit> temp = new List<CstWeb.CustomerSessions.Unit>();
            if (AllowedUnitEnumList != null && AllowedUnitEnumList.Count > 0)
            {                
                foreach(var item in unitList)
                {
                    if (AllowedUnitEnumList.Contains(item))
                    {
                        temp.Add(item);
                    }
                }
                return temp;
            }
            return unitList;
        }
        
        #endregion
    }

}

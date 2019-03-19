using System;
using System.Collections.Generic;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain;
using TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Translation;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System.Collections;
using NLog;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    class DetailLevelService : IDetailLevelService
    {
        private List<GenericDetailLevel> defaultDetailItemList = null;
        private List<DetailLevelItemInformation> allowedDetailItemList = null;
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        public List<DetailLevel> GetDetailLevelItem(string idWebSession, int vehicleId, bool isVehicleChanged, HttpContextBase httpContext)
        {
            WebSession CustomerSession = (WebSession)WebSession.Load(idWebSession);
            List<DetailLevel> detailLevelList = new List<DetailLevel>();
            ArrayList levels = new ArrayList();
            try
            {
                allowedDetailItemList = WebApplicationParameters.InsertionsDetail.GetAllowedMediaDetailLevelItems(vehicleId);
                defaultDetailItemList = WebApplicationParameters.InsertionsDetail.GetDefaultMediaDetailLevels(vehicleId);

                if (isVehicleChanged)
                {
                    foreach (GenericDetailLevel detailItem in defaultDetailItemList)
                        levels = detailItem.LevelIds;

                    CustomerSession.DetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.defaultLevels);
                    CustomerSession.Save();
                }

                for (int i = 1; i < 4; i++) {
                    DetailLevel detaiLevelltmp = new DetailLevel();
                    detaiLevelltmp.Items = new List<DetailLevelItems>();
                    detaiLevelltmp.level = i;
                    DetailLevelItemInformation.Levels loadDetailLevel = new DetailLevelItemInformation.Levels();

                    if (((CustomerSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_OUTDOOR_ACCESS_FLAG)) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == DBClassificationConstantes.Vehicles.names.outdoor))
                        || ((CustomerSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_DOOH_ACCESS_FLAG)) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == DBClassificationConstantes.Vehicles.names.dooh))
                        || ((CustomerSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_INSTORE_ACCESS_FLAG)) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == DBClassificationConstantes.Vehicles.names.instore))
                        || ((CustomerSession.CustomerLogin.CustormerFlagAccess(Flags.ID_DETAIL_INDOOR_ACCESS_FLAG)) && (VehiclesInformation.DatabaseIdToEnum(vehicleId) == DBClassificationConstantes.Vehicles.names.indoor))
                        || (VehiclesInformation.DatabaseIdToEnum(vehicleId) != DBClassificationConstantes.Vehicles.names.outdoor
                            && VehiclesInformation.DatabaseIdToEnum(vehicleId) != DBClassificationConstantes.Vehicles.names.instore
                            && VehiclesInformation.DatabaseIdToEnum(vehicleId) != DBClassificationConstantes.Vehicles.names.dooh
                            && VehiclesInformation.DatabaseIdToEnum(vehicleId) != DBClassificationConstantes.Vehicles.names.indoor)
                        )
                    {

                        if (CustomerSession.DetailLevel.LevelIds.Count >= i)
                        {
                            loadDetailLevel = (DetailLevelItemInformation.Levels)CustomerSession.DetailLevel.LevelIds[i - 1];
                        }

                        foreach (DetailLevelItemInformation currentDetailLevelItem in allowedDetailItemList)
                        {

                            if (CanAddDetailLevelItem(CustomerSession, currentDetailLevelItem))
                            {

                                DetailLevelItems tmpDetailLevelItems = new DetailLevelItems
                                {
                                    Label = GestionWeb.GetWebWord(currentDetailLevelItem.WebTextId, CustomerSession.SiteLanguage),
                                    DetailLevel = currentDetailLevelItem.Id.GetHashCode().ToString()
                                };

                                if (loadDetailLevel == currentDetailLevelItem.Id)
                                {
                                    tmpDetailLevelItems.IsSelected = true;
                                }


                                detaiLevelltmp.Items.Add(tmpDetailLevelItems);
                            }
                        }
                    }

                    detailLevelList.Add(detaiLevelltmp);
                }
            }
            catch (Exception ex)
            {
                if (CustomerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return detailLevelList;
        }


        public void SetDetailLevelItem(string idWebSession, UserFilter userFilter, HttpContextBase httpContext)
        {
            WebSession CustomerSession = (WebSession)WebSession.Load(idWebSession);

            ArrayList levels = new ArrayList();
            try {
                if (userFilter.GenericDetailLevelFilter.L1DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L1DetailValue);
                }
                if (userFilter.GenericDetailLevelFilter.L2DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L2DetailValue);
                }
                if (userFilter.GenericDetailLevelFilter.L3DetailValue >= 0)
                {
                    levels.Add(userFilter.GenericDetailLevelFilter.L3DetailValue);
                }
                if (levels.Count > 0)
                {
                    CustomerSession.DetailLevel = new GenericDetailLevel(levels, WebConstantes.GenericDetailLevel.SelectedFrom.customLevels);
                }

                CustomerSession.Save();
            }
            catch (Exception ex)
            {
                if (CustomerSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, CustomerSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }

        }


        private bool CanAddDetailLevelItem(WebSession CustomerSession, DetailLevelItemInformation currentDetailLevel)
        {

            switch (currentDetailLevel.Id)
            {
                case DetailLevelItemInformation.Levels.slogan:
                    return CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG);
                case DetailLevelItemInformation.Levels.interestCenter:
                case DetailLevelItemInformation.Levels.mediaSeller:
                    return (!CustomerSession.isCompetitorAdvertiserSelected());
                case DetailLevelItemInformation.Levels.brand:
                    return ((CheckProductDetailLevelAccess(CustomerSession)) && CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE));
                case DetailLevelItemInformation.Levels.product:
                    return ((CheckProductDetailLevelAccess(CustomerSession)) && CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.advertiser:
                    return (CheckProductDetailLevelAccess(CustomerSession));
                case DetailLevelItemInformation.Levels.sector:
                case DetailLevelItemInformation.Levels.subSector:
                case DetailLevelItemInformation.Levels.group:
                    return (CheckProductDetailLevelAccess(CustomerSession) && CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_GROUP_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.segment:
                    return (CheckProductDetailLevelAccess(CustomerSession) && CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG));
                case DetailLevelItemInformation.Levels.holdingCompany:
                    return (CheckProductDetailLevelAccess(CustomerSession));
                case DetailLevelItemInformation.Levels.groupMediaAgency:
                case DetailLevelItemInformation.Levels.agency:
                    List<Int64> vehicleList = GetVehicles(CustomerSession);
                    return (CustomerSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList));
                case DetailLevelItemInformation.Levels.mediaGroup:
                    return CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_GROUP);
                default:
                    return (true);
            }
        }

        private bool CheckProductDetailLevelAccess(WebSession CustomerSession)
        {
            return (CustomerSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));
        }

        private List<Int64> GetVehicles(WebSession CustomerSession)
        {
            List<Int64> vehicleList = new List<Int64>();
            string listStr = CustomerSession.GetSelection(CustomerSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0)
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)CustomerSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return vehicleList;
        }

    }
}

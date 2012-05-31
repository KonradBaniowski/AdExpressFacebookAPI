using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Reflection;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using CustomerRightType = TNS.AdExpress.Constantes.Customer.Right.type;
using CustomerSessionsPeriod = TNS.AdExpress.Constantes.Web.CustomerSessions.Period;
using LevelType = TNS.AdExpress.Constantes.Classification.Level.type;
using ModuleName = TNS.AdExpress.Constantes.Web.Module.Name;
using UniverseAccessType = TNS.Classification.Universe.AccessType;

namespace TNS.AdExpressI.Common.DAL.Russia
{
    public class CommonDAL
    {
        private readonly WebSession _session;
        private readonly Int64 _moduleId;

        public const int ProcedureTimeout = 180;

        public CommonDAL(WebSession session, Int64 moduleId)
        {
            _session = session;
            _moduleId = moduleId;
        }

        public IDataSource GetDataSource()
        {
            CoreLayer cl = WebApplicationParameters.CoreLayers[Layers.Id.sourceProvider];
            if (cl == null)
            {
                throw (new NullReferenceException("Core layer is null for the source provider layer"));
            }

            ISourceProvider sourceProvider = (SourceProvider)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, new object[] { _session }, null, null, null);
            return sourceProvider.GetSource();
        }

        public object IsNull(string list)
        {
            if (string.IsNullOrEmpty(list) || list.Equals("-1") || list.Equals(long.MinValue.ToString()))
            {
                return Convert.DBNull;
            }

            return list;
        }

        public string GetCustomerRightTypeValue(Dictionary<CustomerRightType, string> rights, CustomerRightType type)
        {
            if (rights.ContainsKey(type) && rights[type].Length > 0)
            {
                return rights[type];
            }

            return null;
        }

        public MediaRight GetMediaRight()
        {
            MediaRight result = new MediaRight();
            Dictionary<CustomerRightType, string> mediaRights = _session.CustomerDataFilters.MediaRights;

            result.MediaRightMediaAccess = GetCustomerRightTypeValue(mediaRights, CustomerRightType.vehicleAccess);
            result.MediaRightMediaExcept = GetCustomerRightTypeValue(mediaRights, CustomerRightType.vehicleException);
            result.MediaRightRegionAccess = GetCustomerRightTypeValue(mediaRights, CustomerRightType.regionAccess);
            result.MediaRightRegionExcept = GetCustomerRightTypeValue(mediaRights, CustomerRightType.regionException);
            result.MediaRightVehicleAccess = GetCustomerRightTypeValue(mediaRights, CustomerRightType.mediaAccess);
            result.MediaRightVehicleExcept = GetCustomerRightTypeValue(mediaRights, CustomerRightType.mediaException);

            return result;
        }

        public ProductRight GetProductRight()
        {
            ProductRight result = new ProductRight();
            Dictionary<CustomerRightType, string> productRights = _session.CustomerDataFilters.ProductsRights;

            result.ProductRightAdvertiserAccess = GetCustomerRightTypeValue(productRights, CustomerRightType.advertiserAccess);
            result.ProductRightAdvertiserExcept = GetCustomerRightTypeValue(productRights, CustomerRightType.advertiserException);
            result.ProductRightBrandAccess = GetCustomerRightTypeValue(productRights, CustomerRightType.brandAccess);
            result.ProductRightBrandExcept = GetCustomerRightTypeValue(productRights, CustomerRightType.brandException);
            result.ProductRightCategory1Access = GetCustomerRightTypeValue(productRights, CustomerRightType.sectorAccess);
            result.ProductRightCategory1Except = GetCustomerRightTypeValue(productRights, CustomerRightType.sectorException);
            result.ProductRightCategory2Access = GetCustomerRightTypeValue(productRights, CustomerRightType.subSectorAccess);
            result.ProductRightCategory2Except = GetCustomerRightTypeValue(productRights, CustomerRightType.subSectorException);
            result.ProductRightCategory3Access = GetCustomerRightTypeValue(productRights, CustomerRightType.groupAccess);
            result.ProductRightCategory3Except = GetCustomerRightTypeValue(productRights, CustomerRightType.groupException);
            result.ProductRightCategory4Access = GetCustomerRightTypeValue(productRights, CustomerRightType.segmentAccess);
            result.ProductRightCategory4Except = GetCustomerRightTypeValue(productRights, CustomerRightType.segmentException);

            return result;
        }

        public ProfessionClassification GetProfessionClassification()
        {
            ProfessionClassification result = new ProfessionClassification();

            //Get product classification selections
            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.PrincipalProfessionUniverses;

            // First list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.NameAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.nameAccess);
                    result.ProfAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.professionAccess);
                }
            }

            // Second list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.NameAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.nameAccess);
                    result.ProfAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.professionAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (_session.CustomerDataFilters.PrincipalProductUniverses.ContainsKey(UniverseAccessType.excludes) && _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.NameExcept = GetCustomerRightTypeValue(items, CustomerRightType.nameException);
                    result.ProfExcept = GetCustomerRightTypeValue(items, CustomerRightType.professionException);
                }
            }

            return result;
        }

        public ProductClassification GetProductClassification()
        {
            ProductClassification result = new ProductClassification();

            //Get product classification selections
            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.PrincipalProductUniverses;

            // First list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.advertiserAccess);
                    result.BrandAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.brandAccess);
                    result.SubBrandAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.subBrandAccess);
                    result.ProductAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.productAccess);
                    result.Category1Access1 = GetCustomerRightTypeValue(items, CustomerRightType.sectorAccess);
                    result.Category2Access1 = GetCustomerRightTypeValue(items, CustomerRightType.subSectorAccess);
                    result.Category3Access1 = GetCustomerRightTypeValue(items, CustomerRightType.groupAccess);
                    result.Category4Access1 = GetCustomerRightTypeValue(items, CustomerRightType.segmentAccess);
                }
            }

            // Second list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.advertiserAccess);
                    result.BrandAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.brandAccess);
                    result.SubBrandAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.subBrandAccess);
                    result.ProductAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.productAccess);
                    result.Category1Access2 = GetCustomerRightTypeValue(items, CustomerRightType.sectorAccess);
                    result.Category2Access2 = GetCustomerRightTypeValue(items, CustomerRightType.subSectorAccess);
                    result.Category3Access2 = GetCustomerRightTypeValue(items, CustomerRightType.groupAccess);
                    result.Category4Access2 = GetCustomerRightTypeValue(items, CustomerRightType.segmentAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (_session.CustomerDataFilters.PrincipalProductUniverses.ContainsKey(UniverseAccessType.excludes) && _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserExcept = GetCustomerRightTypeValue(items, CustomerRightType.advertiserException);
                    result.BrandExcept = GetCustomerRightTypeValue(items, CustomerRightType.brandException);
                    result.SubBrandExcept = GetCustomerRightTypeValue(items, CustomerRightType.subBrandException);
                    result.ProductExcept = GetCustomerRightTypeValue(items, CustomerRightType.productException);
                    result.Category1Except = GetCustomerRightTypeValue(items, CustomerRightType.sectorException);
                    result.Category2Except = GetCustomerRightTypeValue(items, CustomerRightType.subSectorException);
                    result.Category3Except = GetCustomerRightTypeValue(items, CustomerRightType.groupException);
                    result.Category4Except = GetCustomerRightTypeValue(items, CustomerRightType.segmentException);
                }
            }

            return result;
        }

        public ProductClassification GetProductClassification(int universId)
        {
            ProductClassification result = new ProductClassification();

            //Get product classification selections
            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.GetPrincipalProductUniverses((universId < 0) ? 0 : universId);

            // First list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.advertiserAccess);
                    result.BrandAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.brandAccess);
                    result.SubBrandAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.subBrandAccess);
                    result.ProductAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.productAccess);
                    result.Category1Access1 = GetCustomerRightTypeValue(items, CustomerRightType.sectorAccess);
                    result.Category2Access1 = GetCustomerRightTypeValue(items, CustomerRightType.subSectorAccess);
                    result.Category3Access1 = GetCustomerRightTypeValue(items, CustomerRightType.groupAccess);
                    result.Category4Access1 = GetCustomerRightTypeValue(items, CustomerRightType.segmentAccess);
                }
            }

            // Second list of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.advertiserAccess);
                    result.BrandAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.brandAccess);
                    result.SubBrandAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.subBrandAccess);
                    result.ProductAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.productAccess);
                    result.Category1Access2 = GetCustomerRightTypeValue(items, CustomerRightType.sectorAccess);
                    result.Category2Access2 = GetCustomerRightTypeValue(items, CustomerRightType.subSectorAccess);
                    result.Category3Access2 = GetCustomerRightTypeValue(items, CustomerRightType.groupAccess);
                    result.Category4Access2 = GetCustomerRightTypeValue(items, CustomerRightType.segmentAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (_session.CustomerDataFilters.PrincipalProductUniverses.ContainsKey(UniverseAccessType.excludes) && _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = _session.CustomerDataFilters.PrincipalProductUniverses[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdvertiserExcept = GetCustomerRightTypeValue(items, CustomerRightType.advertiserException);
                    result.BrandExcept = GetCustomerRightTypeValue(items, CustomerRightType.brandException);
                    result.SubBrandExcept = GetCustomerRightTypeValue(items, CustomerRightType.subBrandException);
                    result.ProductExcept = GetCustomerRightTypeValue(items, CustomerRightType.productException);
                    result.Category1Except = GetCustomerRightTypeValue(items, CustomerRightType.sectorException);
                    result.Category2Except = GetCustomerRightTypeValue(items, CustomerRightType.subSectorException);
                    result.Category3Except = GetCustomerRightTypeValue(items, CustomerRightType.groupException);
                    result.Category4Except = GetCustomerRightTypeValue(items, CustomerRightType.segmentException);
                }
            }

            return result;
        }

        public MediaClassification GetMediaClassification()
        {
            MediaClassification result = new MediaClassification();

            //if module is module “Present-Absent report” or “Vehicle portofolio” or "LostWon"
            if (_moduleId == ModuleName.ANALYSE_CONCURENTIELLE
                || _moduleId == ModuleName.ANALYSE_PORTEFEUILLE
                || _moduleId == ModuleName.ANALYSE_DYNAMIQUE
                || _moduleId == ModuleName.TABLEAU_DYNAMIQUE
                || _moduleId == ModuleName.INDICATEUR)
            {
                Dictionary<CustomerRightType, string> items = _session.CustomerDataFilters.SelectedVehicles;
                if (items != null && items.Count > 0)
                {
                    result.MediaAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.vehicleAccess);
                    result.RegionAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                    result.VehicleAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                }
            }
            else
            {
                Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.SecondaryMediaUniverses;

                // Lists of the current classiifcation items to include
                if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
                {
                    Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                    if (items != null && items.Count > 0)
                    {
                        result.MediaAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.vehicleAccess);
                        result.RegionAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                        result.VehicleAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                    }
                }

                if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
                {
                    Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                    if (items != null && items.Count > 0)
                    {
                        result.MediaAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.vehicleAccess);
                        result.RegionAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                        result.VehicleAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                    }
                }

                // List of the current classiifcation items to exclude
                if (selections != null && selections.ContainsKey(UniverseAccessType.excludes) && selections[UniverseAccessType.excludes].Count > 0)
                {
                    Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.excludes][0];
                    if (items != null && items.Count > 0)
                    {
                        result.MediaExcept = GetCustomerRightTypeValue(items, CustomerRightType.vehicleException);
                        result.RegionExcept = GetCustomerRightTypeValue(items, CustomerRightType.regionException);
                        result.VehicleExcept = GetCustomerRightTypeValue(items, CustomerRightType.mediaException);
                    }
                }
            }

            return result;
        }

        public MediaClassification GetMediaScheduleClassification()
        {
            MediaClassification result = new MediaClassification();

            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.SecondaryMediaUniverses;

            // Lists of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.MediaAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.vehicleAccess);
                    result.RegionAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                    result.VehicleAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                }
            }

            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.MediaAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.vehicleAccess);
                    result.RegionAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                    result.VehicleAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (selections != null && selections.ContainsKey(UniverseAccessType.excludes) && selections[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.MediaExcept = GetCustomerRightTypeValue(items, CustomerRightType.vehicleException);
                    result.RegionExcept = GetCustomerRightTypeValue(items, CustomerRightType.regionException);
                    result.VehicleExcept = GetCustomerRightTypeValue(items, CustomerRightType.mediaException);
                }
            }

            return result;
        }

        public RegionClassification GetRegionClassification()
        {
            RegionClassification result = new RegionClassification();

            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.RegionUniverses;

            // Lists of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.RegionAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                    result.VehicleAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                }
            }

            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.RegionAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.regionAccess);
                    result.VehicleAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.mediaAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (selections != null && selections.ContainsKey(UniverseAccessType.excludes) && selections[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.RegionExcept = GetCustomerRightTypeValue(items, CustomerRightType.regionException);
                    result.VehicleExcept = GetCustomerRightTypeValue(items, CustomerRightType.mediaException);
                }
            }

            return result;
        }

        public AdTypeClassification GetAdTypeClassification()
        {
            AdTypeClassification result = new AdTypeClassification();

            Dictionary<UniverseAccessType, List<Dictionary<CustomerRightType, string>>> selections = _session.CustomerDataFilters.AdvertisementTypeUniverses;

            // Lists of the current classiifcation items to include
            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdTypeAccess1 = GetCustomerRightTypeValue(items, CustomerRightType.advertisementTypeAccess);
                }
            }

            if (selections != null && selections.ContainsKey(UniverseAccessType.includes) && selections[UniverseAccessType.includes].Count > 1)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.includes][1];
                if (items != null && items.Count > 0)
                {
                    result.AdTypeAccess2 = GetCustomerRightTypeValue(items, CustomerRightType.advertisementTypeAccess);
                }
            }

            // List of the current classiifcation items to exclude
            if (selections != null && selections.ContainsKey(UniverseAccessType.excludes) && selections[UniverseAccessType.excludes].Count > 0)
            {
                Dictionary<CustomerRightType, string> items = selections[UniverseAccessType.excludes][0];
                if (items != null && items.Count > 0)
                {
                    result.AdTypeExcept = GetCustomerRightTypeValue(items, CustomerRightType.advertisementTypeException);
                }
            }

            return result;
        }

        public ProductLevel GetProductLevel(bool includeLevel)
        {
            ProductLevel result = new ProductLevel();

            if (includeLevel
                || _moduleId == ModuleName.ANALYSE_PLAN_MEDIA
                || _moduleId == ModuleName.CELEBRITIES
                || _moduleId == ModuleName.ANALYSE_DYNAMIQUE)
            {
                if (_session.CustomerDataFilters.LevelProduct != null && _session.CustomerDataFilters.LevelProduct.Count > 0)
                {
                    switch (_session.ProductDetailLevel.LevelProduct)
                    {
                        case LevelType.advertiser:
                            result.ProductLevelAdvertiserAccess = _session.CustomerDataFilters.LevelProduct[CustomerRightType.advertiserAccess];
                            break;
                        case LevelType.brand:
                            result.ProductLevelBrandAccess = _session.CustomerDataFilters.LevelProduct[CustomerRightType.brandAccess];
                            break;
                        case LevelType.subBrand:
                            result.ProductLevelSubBrandAccess = _session.CustomerDataFilters.LevelProduct[CustomerRightType.subBrandAccess];
                            break;
                        case LevelType.product:
                            result.ProductLevelProductAccess = _session.CustomerDataFilters.LevelProduct[CustomerRightType.productAccess];
                            break;
                        case LevelType.sector:
                            result.ProductLevelCategory1Access = _session.CustomerDataFilters.LevelProduct[CustomerRightType.sectorAccess];
                            break;
                        case LevelType.subsector:
                            result.ProductLevelCategory2Access = _session.CustomerDataFilters.LevelProduct[CustomerRightType.subSectorAccess];
                            break;
                        case LevelType.group:
                            result.ProductLevelCategory3Access = _session.CustomerDataFilters.LevelProduct[CustomerRightType.groupAccess];
                            break;
                        case LevelType.segment:
                            result.ProductLevelCategory4Access = _session.CustomerDataFilters.LevelProduct[CustomerRightType.segmentAccess];
                            break;
                    }
                }
            }

            return result;
        }

        public DetailLevel GetDetailLevel(string filters)
        {
            DetailLevel result = new DetailLevel();

            if (!string.IsNullOrEmpty(filters))
            {
                TNS.AdExpress.Domain.Level.GenericDetailLevel genericDetailLevel = null;
                switch (_moduleId)
                {
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_CONCURENTIELLE: //if module is module “Present-Absent report”
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_PORTEFEUILLE: //if module is “Vehicle portofolio”
                        genericDetailLevel = _session.GenericProductDetailLevel;
                        break;

                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA: //if module is module “Media schedule”
                    case AdExpress.Constantes.Web.Module.Name.ANALYSE_DYNAMIQUE: //if module is module “Lost Won report”
                    case AdExpress.Constantes.Web.Module.Name.CELEBRITIES: //if module is module “Celebrities”
                        genericDetailLevel = _session.GenericMediaDetailLevel;
                        break;
                }

                if (genericDetailLevel != null)
                {
                    string[] identityStrings = filters.Split(',');
                    for (int i = 0; i < identityStrings.Length && i < genericDetailLevel.Levels.Count; ++i)
                    {
                        string identity = identityStrings[i];
                        DetailLevelItemInformation level = (DetailLevelItemInformation)genericDetailLevel.Levels[i];

                        switch (level.Id)
                        {
                            case DetailLevelItemInformation.Levels.advertisementType:
                                result.DetailAdType = identity;
                                break;
                            case DetailLevelItemInformation.Levels.advertisment:
                            case DetailLevelItemInformation.Levels.slogan:
                                result.DetailAdvertisment = identity;
                                break;
                            case DetailLevelItemInformation.Levels.region:
                                result.DetailRegion = identity;
                                break;
                            case DetailLevelItemInformation.Levels.vehicle:
                                result.DetailMedia = identity;
                                break;
                            case DetailLevelItemInformation.Levels.media:
                                result.DetailVehicle = identity;
                                break;

                            case DetailLevelItemInformation.Levels.advertiser:
                                result.DetailAdvertiser = identity;
                                break;
                            case DetailLevelItemInformation.Levels.brand:
                                result.DetailBrand = identity;
                                break;
                            case DetailLevelItemInformation.Levels.subBrand:
                                result.DetailSubBrand = identity;
                                break;
                            case DetailLevelItemInformation.Levels.product:
                                result.DetailProduct = identity;
                                break;

                            case DetailLevelItemInformation.Levels.sector:
                                result.DetailCategory1 = identity;
                                break;
                            case DetailLevelItemInformation.Levels.subSector:
                                result.DetailCategory2 = identity;
                                break;
                            case DetailLevelItemInformation.Levels.group:
                                result.DetailCategory3 = identity;
                                break;
                            case DetailLevelItemInformation.Levels.segment:
                                result.DetailCategory4 = identity;
                                break;

                            case DetailLevelItemInformation.Levels.profession:
                                result.DetailProfession = identity;
                                break;
                            case DetailLevelItemInformation.Levels.name:
                                result.DetailName = identity;
                                break;
                            case DetailLevelItemInformation.Levels.programme:
                                result.DetailProgramme = identity;
                                break;
                            case DetailLevelItemInformation.Levels.programmeGenre:
                                result.DetailProgrammeGenre = identity;
                                break;
                            case DetailLevelItemInformation.Levels.presenceType:
                                result.DetailPresenceType = identity;
                                break;
                            case DetailLevelItemInformation.Levels.rubric:
                                result.DetailRubric = identity;
                                break;
                        }
                    }
                }
            }

            return result;
        }

        public string GetSelectedVehicles()
        {
            string result = string.Empty;

            //if module is module “Present-Absent report” or “Vehicle portofolio” or "LostWon"
            if (_moduleId == ModuleName.ANALYSE_CONCURENTIELLE ||
                _moduleId == ModuleName.ANALYSE_PORTEFEUILLE ||
                _moduleId == ModuleName.ANALYSE_DYNAMIQUE)
            {
                Dictionary<CustomerRightType, string> items = _session.CustomerDataFilters.SelectedVehicles;
                if (items != null && items.Count > 0 && items.ContainsKey(CustomerRightType.mediaAccess))
                {
                    result = items[CustomerRightType.mediaAccess];
                }
            }

            return result;
        }

        public string GetSelectedMediaTypes()
        {
            string result = string.Empty;

            //if module is “Media schedule”
            if (_moduleId == ModuleName.ANALYSE_PLAN_MEDIA || _moduleId == ModuleName.CELEBRITIES)
            {
                result = _session.CustomerDataFilters.SelectedMediaType;
            }

            return result;
        }

        public string GetSelectedVersions(CustomerSessionsPeriod.DisplayLevel periodDisplay)
        {
            string result = string.Empty;

            // Zoom on a specific version
            if (_session.SloganIdZoom > long.MinValue && periodDisplay == CustomerSessionsPeriod.DisplayLevel.dayly)
            {
                result = _session.SloganIdZoom.ToString();
            }
            else
            {
                // Refine vesions
                if (_session.SloganIdList.Length > 0 && periodDisplay == CustomerSessionsPeriod.DisplayLevel.dayly)
                {
                    result = _session.SloganIdList;
                }
            }

            return result;
        }

        public string GetSelectedVersions()
        {
            string result = string.Empty;

            //if module is “Media schedule”
            if (_moduleId == ModuleName.ALERTE_PLAN_MEDIA)
            {
                result = _session.SloganIdList;
            }

            return result;
        }

        public string GetAllowedMediaTypes()
        {
            string result = String.Empty;

            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_moduleId);
            // List of Media available for current module
            result = module.AllowedMediaUniverse.VehicleList;

            return result;
        }

        public string GetSelectedCampaignType()
        {
            string result = string.Empty;

            result = _session.CustomerDataFilters.CampaignType.ToString();

            return result;
        }

        /// <summary>
        /// Populate query parameters from a hashtable
        /// </summary>
        /// <param name="parameters">Parameters</param>
        /// <param name="cmd">SQL command</param>
        public static void InitParams(Hashtable parameters, SqlCommand cmd)
        {

            IDictionaryEnumerator cur = parameters.GetEnumerator();

            while (cur.MoveNext())
            {
                cmd.Parameters.AddWithValue(cur.Key.ToString(), cur.Value);
            }

        }
    }
}

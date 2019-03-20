using Kantar.AdExpress.Service.Core.BusinessService;
using Kantar.AdExpress.Service.Core.Domain.ResultOptions;
using System.Linq;
using TNS.AdExpress.Web.Core.Sessions;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using ClassificationCst = TNS.AdExpress.Constantes.Classification;
using SessionCst = TNS.AdExpress.Constantes.Web.CustomerSessions;
using System.Collections.Generic;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Domain.Level;
using CstDB = TNS.AdExpress.Constantes.DB;
using DBConstantesClassification = TNS.AdExpress.Constantes.Classification.DB;
using NLog;
using System;
using TNS.AdExpress.Web.Utilities.Exceptions;
using System.Web;
using TNS.AdExpress.Domain.Units;

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class OptionAnalysisService : IOptionAnalysisService
    {
        private static Logger Logger= LogManager.GetCurrentClassLogger();
        private WebSession _customerWebSession;
        private bool _showSegment = false;

        public OptionsAnalysis GetOptions(string idWebSession, HttpContextBase httpContext)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            
            OptionsAnalysis options = new OptionsAnalysis();
            try
            {
                _showSegment = _customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)
                    &&  !WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY)
                ;
                options.SiteLanguage = _customerWebSession.SiteLanguage;

                GenericLevelOption mediaDetail = new GenericLevelOption();
                GenericLevelOption productDetail = new GenericLevelOption();

                #region Unitf
                VehicleInformation vehicle = VehiclesInformation.Get(((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID);
                _customerWebSession.Units = new List<SessionCst.Unit>
                {
                    WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule)
                        .GetResultPageInformation(_customerWebSession.CurrentTab)
                        .GetDefaultUnit(vehicle.Id)
                };
                #endregion

                if (vehicle.Id != DBConstantesClassification.Vehicles.names.plurimedia)
                    _customerWebSession.PreformatedMediaDetail = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
                else
                    _customerWebSession.PreformatedMediaDetail = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle;

                #region mediaDetailOption
                if (_customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR || _customerWebSession.CurrentModule == WebConstantes.Module.Name.TABLEAU_DYNAMIQUE)
                {
                    mediaDetail.LevelDetail = new SelectControl();
                    mediaDetail.LevelDetail.Id = "mediaDetail";
                    mediaDetail.LevelDetail.Visible = true;
                    mediaDetail.LevelDetail.Items = new List<SelectItem>();

                    VehicleInformation vehicleInfo = VehiclesInformation.Get(((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID);
                    /* WARNING !!! : This patch is just temporarily used in order to add specific levels for the Russia version of the site
               * */
                    if (!WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.RUSSIA))
                    {
                        switch (vehicleInfo.Id)
                        {
                            case ClassificationCst.DB.Vehicles.names.tv:
                            case ClassificationCst.DB.Vehicles.names.tvGeneral:
                            case ClassificationCst.DB.Vehicles.names.tvSponsorship:
                            case ClassificationCst.DB.Vehicles.names.tvAnnounces:
                            case ClassificationCst.DB.Vehicles.names.tvNonTerrestrials:
                            case ClassificationCst.DB.Vehicles.names.radio:
                            case ClassificationCst.DB.Vehicles.names.radioGeneral:
                            case ClassificationCst.DB.Vehicles.names.radioSponsorship:
                            case ClassificationCst.DB.Vehicles.names.radioMusic:
                            case ClassificationCst.DB.Vehicles.names.outdoor:
                            case ClassificationCst.DB.Vehicles.names.dooh:
                            case ClassificationCst.DB.Vehicles.names.indoor:
                            case ClassificationCst.DB.Vehicles.names.mediasTactics:
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                                if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category))
                                    mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1142, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString() });
                                if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                                    mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1544, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString() });
                                if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category) && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                                    mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1143, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString() });
                                if (
                                    WebApplicationParameters.CountryCode.Equals(
                                        TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                                {
                                    if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category) 
                                        && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)
                                        && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.interestCenter))
                                        mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3236, _customerWebSession.SiteLanguage),
                                            Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.categoryInterestCenterMedia.GetHashCode().ToString() });

                                    if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.programTypology))
                                        mediaDetail.LevelDetail.Items.Add(new SelectItem
                                        {
                                            Text = GestionWeb.GetWebWord(3242, _customerWebSession.SiteLanguage),
                                            Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.programTypology.GetHashCode().ToString()
                                        });


                                    if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media)                                      
                                        && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.spotSubType))
                                        mediaDetail.LevelDetail.Items.Add(new SelectItem
                                        {
                                            Text = GestionWeb.GetWebWord(3234, _customerWebSession.SiteLanguage),
                                            Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.mediaSpotSubType.GetHashCode().ToString()
                                        });

                                    if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.spotSubType)
                                     && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.spotType))
                                        mediaDetail.LevelDetail.Items.Add(new SelectItem
                                        {
                                            Text = GestionWeb.GetWebWord(3235, _customerWebSession.SiteLanguage),
                                            Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.spotTypeSpotSubType.GetHashCode().ToString()
                                        });

                                    if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.spotSubType)
                                   && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.programTypology))
                                        mediaDetail.LevelDetail.Items.Add(new SelectItem
                                        {
                                            Text = GestionWeb.GetWebWord(3243, _customerWebSession.SiteLanguage),
                                            Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.programTypologySpotSubType.GetHashCode().ToString()
                                        });
                                }
                                break;
                            case ClassificationCst.DB.Vehicles.names.press:
                            case ClassificationCst.DB.Vehicles.names.newspaper:
                            case ClassificationCst.DB.Vehicles.names.magazine:
                            case ClassificationCst.DB.Vehicles.names.internationalPress:
                            case ClassificationCst.DB.Vehicles.names.adnettrack:
                            case ClassificationCst.DB.Vehicles.names.internet:
                            case ClassificationCst.DB.Vehicles.names.czinternet:
                            case ClassificationCst.DB.Vehicles.names.mobileTelephony:
                            case ClassificationCst.DB.Vehicles.names.emailing:
                            case ClassificationCst.DB.Vehicles.names.plurimedia:                           
                            case ClassificationCst.DB.Vehicles.names.directMarketing:
                            case ClassificationCst.DB.Vehicles.names.mms:
                            case ClassificationCst.DB.Vehicles.names.search:
                            case ClassificationCst.DB.Vehicles.names.social:
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                                if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category))
                                    mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1142, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString() });
                                break;
                            default:
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                                break;
                        }
                    }
                    else
                    {
                        switch (vehicleInfo.Id)
                        {
                            case ClassificationCst.DB.Vehicles.names.plurimedia:                          
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2652, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.region.GetHashCode().ToString() });                               
                                break;
                            default:                                

                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(971, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.Media.GetHashCode().ToString() });
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2652, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.region.GetHashCode().ToString() });
                              
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1544, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString() });                             
                                break;
                        }
                    }
                    try
                    {
                        mediaDetail.LevelDetail.SelectedId = _customerWebSession.PreformatedMediaDetail.GetHashCode().ToString();
                    }
                    catch (System.Exception)
                    {
                        mediaDetail.LevelDetail.SelectedId = mediaDetail.LevelDetail.Items[0].Value;
                        try
                        {
                            _customerWebSession.PreformatedMediaDetail = (SessionCst.PreformatedDetails.PreformatedMediaDetails)int.Parse(mediaDetail.LevelDetail.Items[0].Value);
                        }
                        catch (System.Exception) { }
                    }
                }
                #endregion

                #region ProductDetailOption
                productDetail.LevelDetail = new SelectControl();
                productDetail.LevelDetail.Id = "productDetail";
                productDetail.LevelDetail.Visible = true;
                productDetail.LevelDetail.Items = new List<SelectItem>();
                long productLabelCode = 1164;

              

                if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND))
                {
                    productLabelCode = 1146;
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(175, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.sector.GetHashCode().ToString() });
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1532, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.sectorSubsector.GetHashCode().ToString() });
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1491, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser.GetHashCode().ToString() });
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(552, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.subSector.GetHashCode().ToString() });
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3213, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.subSectorGroup.GetHashCode().ToString() });
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2610, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.subSectorAdvertiser.GetHashCode().ToString() });
                }

                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1110, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.group.GetHashCode().ToString() });
                if (_showSegment)
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1144, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.groupSegment.GetHashCode().ToString() });
                //Rights verification for Brand
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1111, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.groupBrand.GetHashCode().ToString() });
                }
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1112, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.groupProduct.GetHashCode().ToString() });

                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1145, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.groupAdvertiser.GetHashCode().ToString() });
                //modifications for segmentAdvertiser,segmentProduct,SegmentBrand(3 new items added in the dropdownlist)
                if (_showSegment)
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1577, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.GetHashCode().ToString() });

                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1578, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentProduct.GetHashCode().ToString() });
                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
                    {
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1579, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentBrand.GetHashCode().ToString() });
                    }

                }


                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1146, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString() });
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1147, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserBrand.GetHashCode().ToString() });
                }
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1148, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserProduct.GetHashCode().ToString() });
                    if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1109, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserBrandProduct.GetHashCode().ToString() });
                }
                    
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1149, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString() });
                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                    {
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2736, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.brandProduct.GetHashCode().ToString() });
                        if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                        {
                            productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3237, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.brandProductSlogan.GetHashCode().ToString() });
                            productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3238, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.productBrand.GetHashCode().ToString() });

                        }

                    }
                }
                if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                {
                    productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(858, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString() });
                    if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.TURKEY))
                    {
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3239, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.productSlogan.GetHashCode().ToString()});
                        productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(3139, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.purchasingAgency.GetHashCode().ToString() });
                    }
                }
                   

                try
                {
                    productDetail.LevelDetail.SelectedId = _customerWebSession.PreformatedProductDetail.GetHashCode().ToString();
                }
                catch (System.Exception)
                {
                    productDetail.LevelDetail.SelectedId = productDetail.LevelDetail.Items[0].Value;
                }
                #endregion

                #region Evolution
                CheckBoxOption evol = new CheckBoxOption();
                evol.Id = "analysisEvol";
                evol.Value = _customerWebSession.Evolution;
                #endregion

                #region PDM
                CheckBoxOption pdm = new CheckBoxOption();
                pdm.Id = "pdmEvol";
                pdm.Value = _customerWebSession.PDM;
                #endregion

                #region PDV
                CheckBoxOption pdv = new CheckBoxOption();
                pdv.Id = "pdvEvol";
                pdv.Value = _customerWebSession.PDV;
                #endregion

                #region Result Type
                ResultTypeOption resultTypeOption = new ResultTypeOption();
                resultTypeOption.ResultType = new SelectControl();
                resultTypeOption.ResultType.Id = "resultType";
                resultTypeOption.ResultType.Visible = true;
                resultTypeOption.ResultType.Items = new List<SelectItem>();


                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0} / {1}", GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.media_X_Year.GetHashCode().ToString()
                });
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0} / {1}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.product_X_Year.GetHashCode().ToString()
                });
                // "Produits+Supports / Année"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.productMedia_X_Year.GetHashCode().ToString()
                });
                //"Supports+Produits / Année  "
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}", GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(1164, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.mediaProduct_X_Year.GetHashCode().ToString()
                });
                //"Produits+Supports / Année+Mensuel"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}+{3}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3079, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.productMedia_X_YearMensual.GetHashCode().ToString()
                });
                //"Supports+Produits / Année+Mensuel"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}+{3}", GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(1164, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3079, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.mediaProduct_X_YearMensual.GetHashCode().ToString()
                });
                // "Produits+Année / Supports"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Media.GetHashCode().ToString()
                });
                //"Produits+Année / Mensuel+Cumul"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}+{3}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3079, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3078, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Mensual.GetHashCode().ToString()
                });
                //"Produits+Année / Mensuel Cumulé"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}", GestionWeb.GetWebWord(productLabelCode, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3077, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Cumul.GetHashCode().ToString()
                });
                //"Supports+Année / Mensuel+Cumul"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}+{3}", GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3079, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3078, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.mediaYear_X_Mensual.GetHashCode().ToString()
                });
                //"Supports+Année / Mensuel Cumulé"
                resultTypeOption.ResultType.Items.Add(new SelectItem
                {
                    Text = string.Format("{0}+{1} / {2}", GestionWeb.GetWebWord(2933, _customerWebSession.SiteLanguage),
                    GestionWeb.GetWebWord(3076, _customerWebSession.SiteLanguage), GestionWeb.GetWebWord(3077, _customerWebSession.SiteLanguage)),
                    Value = SessionCst.PreformatedDetails.PreformatedTables.mediaYear_X_Cumul.GetHashCode().ToString()
                });

                //resultTypeOption.ResultType.SelectedId = SessionCst.PreformatedDetails.PreformatedTables.media_X_Year.GetHashCode().ToString();
                resultTypeOption.ResultType.SelectedId = _customerWebSession.PreformatedTable.GetHashCode().ToString();
                #endregion

                #region UnitOption

                UnitOption unitOption = new UnitOption();

                unitOption.Unit = new UnitSelectControl();
                unitOption.Unit.Id = "unit";
                unitOption.Unit.Items = new List<SelectItem>();
                var unitInformationDictionary = new Dictionary<TNS.AdExpress.Constantes.Web.CustomerSessions.Unit, UnitInformation>();
               
                List<UnitInformation> units = _customerWebSession.GetValidUnitForResult();
                for (int i = 0; i < units.Count; i++)
                {
                    unitInformationDictionary.Add(units[i].Id, units[i]);
                }
                if ((!_customerWebSession.ReachedModule || !unitInformationDictionary.ContainsKey(_customerWebSession.Unit)))
                {
                    _customerWebSession.Units = new List<SessionCst.Unit>
                    {
                        WebNavigation.ModulesList.GetModule(_customerWebSession.CurrentModule)
                            .GetResultPageInformation(_customerWebSession.CurrentTab)
                            .GetDefaultUnit(vehicle.Id)
                    };
                }

                AddUnitOptions(units, unitOption);

                var exceptUnits = units.Intersect(UnitsInformation.Get(_customerWebSession.Units));
                if (!exceptUnits.Any())
                {
                    if (ContainsDefaultCurrency(units))
                    {
                        _customerWebSession.Units = new List<SessionCst.Unit>
                        {
                            UnitsInformation.DefaultCurrency
                        };
                    }
                    else
                        _customerWebSession.Units = new List<SessionCst.Unit> { units[0].Id };
                }

               
                unitOption.Unit.SelectedIds = _customerWebSession.Units.Select(u => u.GetHashCode().ToString()).ToList();



                options.UnitOption = unitOption;
                #endregion

                options.MediaDetailLevel = mediaDetail;
                options.ProductDetailLevel = productDetail;
                options.Evol = evol;
                options.PDM = pdm;
                options.PDV = pdv;
                options.ResultTypeOption = resultTypeOption;

                if (WebApplicationParameters.UseRetailer)
                {
                    options.IsSelectRetailerDisplay = _customerWebSession.IsSelectRetailerDisplay;
                }
                else
                    options.IsSelectRetailerDisplay = false;

                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
            return options;
        }

        public void SetOptions(string idWebSession, UserAnalysisFilter userFilter, HttpContextBase httpContext)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            try
            {
                #region UnitFilter

                if (!userFilter.UnitFilter.Unit.Contains(WebConstantes.CustomerSessions.Unit.none)
                )
                    _customerWebSession.Units = userFilter.UnitFilter.Unit;

                #endregion

                _customerWebSession.PreformatedMediaDetail =
                    (SessionCst.PreformatedDetails.PreformatedMediaDetails) userFilter.MediaDetailLevel.LevelDetailValue;
                _customerWebSession.PreformatedProductDetail =
                    (SessionCst.PreformatedDetails.PreformatedProductDetails)
                    userFilter.ProductDetailLevel.LevelDetailValue;
                _customerWebSession.Evolution = userFilter.Evol;
                _customerWebSession.PDM = userFilter.PDM;
                _customerWebSession.PDV = userFilter.PDV;
                _customerWebSession.PreformatedTable =
                    (SessionCst.PreformatedDetails.PreformatedTables) userFilter.ResultTypeFilter.ResultType;

                if (WebApplicationParameters.UseRetailer)
                {
                    _customerWebSession.IsSelectRetailerDisplay = userFilter.IsSelectRetailerDisplay;
                }

                _customerWebSession.Save();
            }
            catch (Exception ex)
            {
                if (_customerWebSession.EnableTroubleshooting)
                {
                    CustomerWebException cwe = new CustomerWebException(httpContext, ex.Message, ex.StackTrace, _customerWebSession);
                    Logger.Log(LogLevel.Error, cwe.GetLog());
                }

                throw;
            }
        }

        private void AddUnitOptions(List<UnitInformation> units, UnitOption unitOption)
        {
            foreach (UnitInformation currentUnit in units)
            {

                unitOption.Unit.Items.Add(new SelectItem
                {
                    Text = GestionWeb.GetWebWord(currentUnit.WebTextId,
                           _customerWebSession.SiteLanguage),
                    Value = currentUnit.Id.GetHashCode().ToString(),
                    GroupId = currentUnit.GroupId,
                    GroupType = currentUnit.GroupType,
                    GroupTextId = currentUnit.GroupTextId
                });
            }
        }

        #region Unit Option Methodes
        private bool ContainsDefaultCurrency(List<UnitInformation> units)
        {
            foreach (UnitInformation currentUnit in units)
                if (currentUnit.Id == UnitsInformation.DefaultCurrency)
                    return true;

            return false;
        }
        #endregion
    }
}

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

namespace Kantar.AdExpress.Service.BusinessLogic.ServiceImpl
{
    public class OptionAnalysisService : IOptionAnalysisService
    {

        private WebSession _customerWebSession;
        private bool _showSegment = false;

        public OptionsAnalysis GetOptions(string idWebSession)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);
            _showSegment = _customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG);

            OptionsAnalysis options = new OptionsAnalysis();

            options.SiteLanguage = _customerWebSession.SiteLanguage;

            GenericLevelOption mediaDetail = new GenericLevelOption();
            GenericLevelOption productDetail = new GenericLevelOption();

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
                        case ClassificationCst.DB.Vehicles.names.indoor:
                        case ClassificationCst.DB.Vehicles.names.mediasTactics:
                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                            if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category))
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1142, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategory.GetHashCode().ToString() });
                            if (_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR && vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1544, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString() });
                            if (vehicleInfo.AllowedRecapMediaLevelItemsEnumList != null && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.category) && vehicleInfo.AllowedRecapMediaLevelItemsEnumList.Contains(DetailLevelItemInformation.Levels.media))
                                mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1143, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia.GetHashCode().ToString() });
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
                        case ClassificationCst.DB.Vehicles.names.PlurimediaWithoutMms:
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
                        case ClassificationCst.DB.Vehicles.names.PlurimediaWithoutMms:
                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString() });
                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2652, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.region.GetHashCode().ToString() });
                            //if ((_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR) ||
                            //    (_customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR && !graphRadioButton.Checked))
                            //    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2740, _customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleRegion.GetHashCode().ToString()));
                            break;
                        default:
                            //if ((_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR) ||
                            //   (_customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR && !graphRadioButton.Checked))
                            //    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(1141, _customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicle.GetHashCode().ToString()));

                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(971, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.Media.GetHashCode().ToString() });
                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2652, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.region.GetHashCode().ToString() });
                            //if ((_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR) ||
                            //  (_customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR && !graphRadioButton.Checked))
                            //    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2740, _customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleRegion.GetHashCode().ToString()));
                            mediaDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1544, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleMedia.GetHashCode().ToString() });
                            //if ((_customerWebSession.CurrentModule != WebConstantes.Module.Name.INDICATEUR) ||
                            //   (_customerWebSession.CurrentModule == WebConstantes.Module.Name.INDICATEUR && !graphRadioButton.Checked))
                            //{
                            //    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2731, _customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.regionMedia.GetHashCode().ToString()));
                            //    mediaDetail.Items.Add(new ListItem(GestionWeb.GetWebWord(2741, _customerWebSession.SiteLanguage), SessionCst.PreformatedDetails.PreformatedMediaDetails.vehicleRegionMedia.GetHashCode().ToString()));
                            //}
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

            if (WebApplicationParameters.CountryCode.Equals(TNS.AdExpress.Constantes.Web.CountryCode.FINLAND))
            {
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(175, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.sector.GetHashCode().ToString() });
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1491, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.sectorAdvertiser.GetHashCode().ToString() });
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(552, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.subSector.GetHashCode().ToString() });
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
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1577, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentAdvertiser.GetHashCode().ToString() });
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1578, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentProduct.GetHashCode().ToString() });
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
            {
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1579, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.segmentBrand.GetHashCode().ToString() });
            }
            productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1146, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiser.GetHashCode().ToString() });
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
            {
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1147, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserBrand.GetHashCode().ToString() });
            }
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1148, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.advertiserProduct.GetHashCode().ToString() });
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_MARQUE))
            {
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(1149, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.brand.GetHashCode().ToString() });
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(2736, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.brandProduct.GetHashCode().ToString() });
            }
            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                productDetail.LevelDetail.Items.Add(new SelectItem { Text = GestionWeb.GetWebWord(858, _customerWebSession.SiteLanguage), Value = SessionCst.PreformatedDetails.PreformatedProductDetails.product.GetHashCode().ToString() });

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

            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Supports / Année", Value = SessionCst.PreformatedDetails.PreformatedTables.media_X_Year.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits / Année", Value = SessionCst.PreformatedDetails.PreformatedTables.product_X_Year.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits+Supports / Année", Value = SessionCst.PreformatedDetails.PreformatedTables.productMedia_X_Year.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Supports+Produits / Année  ", Value = SessionCst.PreformatedDetails.PreformatedTables.mediaProduct_X_Year.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits+Supports / Année+Mensuel", Value = SessionCst.PreformatedDetails.PreformatedTables.productMedia_X_YearMensual.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Supports+Produits / Année+Mensuel", Value = SessionCst.PreformatedDetails.PreformatedTables.mediaProduct_X_YearMensual.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits+Année / Supports", Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Media.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits+Année / Mensuel+Cumul", Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Mensual.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Produits+Année / Mensuel Cumulé", Value = SessionCst.PreformatedDetails.PreformatedTables.productYear_X_Cumul.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Supports+Année / Mensuel+Cumul", Value = SessionCst.PreformatedDetails.PreformatedTables.mediaYear_X_Mensual.GetHashCode().ToString() });
            resultTypeOption.ResultType.Items.Add(new SelectItem { Text = "Supports+Année / Mensuel Cumulé", Value = SessionCst.PreformatedDetails.PreformatedTables.mediaYear_X_Cumul.GetHashCode().ToString() });

            resultTypeOption.ResultType.SelectedId = SessionCst.PreformatedDetails.PreformatedTables.media_X_Year.GetHashCode().ToString();
            #endregion

            options.MediaDetailLevel = mediaDetail;
            options.ProductDetailLevel = productDetail;
            options.Evol = evol;
            options.PDM = pdm;
            options.PDV = pdv;
            options.ResultTypeOption = resultTypeOption;

            _customerWebSession.Save();

            return options;
        }

        public void SetOptions(string idWebSession, UserAnalysisFilter userFilter)
        {
            _customerWebSession = (WebSession)WebSession.Load(idWebSession);

            _customerWebSession.PreformatedMediaDetail = (SessionCst.PreformatedDetails.PreformatedMediaDetails)userFilter.MediaDetailLevel.LevelDetailValue;
            _customerWebSession.PreformatedProductDetail = (SessionCst.PreformatedDetails.PreformatedProductDetails)userFilter.ProductDetailLevel.LevelDetailValue;
            _customerWebSession.Evolution = userFilter.Evol;
            _customerWebSession.PDM = userFilter.PDM;
            _customerWebSession.PDV = userFilter.PDV;
            _customerWebSession.PreformatedTable = (SessionCst.PreformatedDetails.PreformatedTables)userFilter.ResultTypeFilter.ResultType;

            _customerWebSession.Save();
        }
    }
}

using System;
using System.Collections.Generic;
using TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.Classification.Universe;
using WebConstantes = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpress.Web.Core.France.Utilities
{
   public class MediaDetailLevel : Core.Utilities.MediaDetailLevel
    {
       public MediaDetailLevel()
           : base()
       {
       }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerWebSession">_customer WebSession</param>
        /// <param name="componentProfile">component Profile</param>
        public MediaDetailLevel(WebSession customerWebSession, WebConstantes.GenericDetailLevel.ComponentProfile componentProfile)
            : base(customerWebSession, componentProfile)
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerWebSession">_customer WebSession</param>
        public MediaDetailLevel(WebSession customerWebSession)
            : base(customerWebSession)
        {
        }

        /// <summary>
        /// Test si l'élément de niveau de détail peut être montré
        /// </summary>
        /// <remarks>
        /// AdNetTrack selection [Ko]
        /// </remarks>
        /// <param name="currentDetailLevelItem">Elément de niveau de détail</param>
        /// <param name="module">Module</param>
        /// <returns>True si oui false sinon</returns>
        public override bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem, Int64 module)
        {
            List<Int64> vehicleList = null;
            switch (module)
            {
                case WebConstantes.Module.Name.ALERTE_CONCURENTIELLE:
                case WebConstantes.Module.Name.ANALYSE_CONCURENTIELLE:
                case WebConstantes.Module.Name.ALERTE_POTENTIELS:
                case WebConstantes.Module.Name.ANALYSE_POTENTIELS:
                case WebConstantes.Module.Name.ANALYSE_DYNAMIQUE:
                case WebConstantes.Module.Name.ALERTE_PORTEFEUILLE:
                case WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE:
                case WebConstantes.Module.Name.NEW_CREATIVES:
                    switch (_componentProfile)
                    {
                        case WebConstantes.GenericDetailLevel.ComponentProfile.media:
                            switch (currentDetailLevelItem.Id)
                            {
                                #region Annonceur
                                case DetailLevelItemInformation.Levels.advertiser:

                                  
                                    if (
                                        // Droit sur les niveaux de détail produit
                                        CheckProductDetailLevelAccess() &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        )
                                        return (true);
                                    return (false);
                                #endregion

                                #region  product
                                case DetailLevelItemInformation.Levels.product:
                                    if (
                                        // Droit sur les niveaux de détail produit
                                        CheckProductDetailLevelAccess() &&
                                        // Products rights (For Finland)
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        )
                                        return (true);
                                    return (false);
                                #endregion

                                #region Marques
                                case DetailLevelItemInformation.Levels.brand:
                                    if (
                                        // Droit sur les niveaux de détail produit
                                        CheckProductDetailLevelAccess() &&
                                        // Droit des Marques
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_MARQUE) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe, groupe, groupe d'annonceur
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        )
                                        return (true);
                                    return (false);

                                #endregion

                                #region Famille, classe, groupe, variété
                                case DetailLevelItemInformation.Levels.sector:
                                case DetailLevelItemInformation.Levels.subSector:                             
                                    if (CheckProductDetailLevelAccess()) return (true);
                                    return (false);
                                case DetailLevelItemInformation.Levels.group:
                                    if (CheckProductDetailLevelAccess() &&
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_GROUP_LEVEL_ACCESS_FLAG)) return (true);
                                    return (false);
                                case DetailLevelItemInformation.Levels.segment:
                                    if (CheckProductDetailLevelAccess() &&
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                                    return (false);
                                #endregion

                                #region Groupe de société
                                case DetailLevelItemInformation.Levels.holdingCompany:
                                    if (
                                        CheckProductDetailLevelAccess() &&
                                        // Droit sur les groupe de société
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        ) return (true);
                                    return (false);
                                #endregion

                                #region Agences et groupe d'agence
                                case DetailLevelItemInformation.Levels.groupMediaAgency:
                                case DetailLevelItemInformation.Levels.agency:
                                    vehicleList = GetVehicles();
                                    if (
                                        // Droit sur les agences media
                                        _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
                                        ) return (true);
                                    return (false);
                                #endregion

                                #region Version
                                case DetailLevelItemInformation.Levels.slogan:
                                    if (
                                        currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                                        // Sélection par produit ou marque ou annonceur
                                        (
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.advertiserAccess).Length > 0) &&
                                        // Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.subSectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.groupAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.segmentAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, Constantes.Customer.Right.type.holdingCompanyAccess).Length == 0 &&
                                        // Niveau de détail par jour
                                        _customerWebSession.DetailPeriod == Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly

                                        )
                                        return (true);
                                    return (false);
                                #endregion
                                default:
                                    return (true);
                            }
                        case WebConstantes.GenericDetailLevel.ComponentProfile.product:
                            switch (currentDetailLevelItem.Id)
                            {

                                #region  sector, subsector, group ,segment,Annonceur
                                case DetailLevelItemInformation.Levels.sector:
                                case DetailLevelItemInformation.Levels.subSector:                              
                                case DetailLevelItemInformation.Levels.advertiser:
                                case DetailLevelItemInformation.Levels.publicationType:
                                    return (true);
                                case DetailLevelItemInformation.Levels.group:
                                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_GROUP_LEVEL_ACCESS_FLAG)) return true;
                                    return false;
                                case DetailLevelItemInformation.Levels.segment:
                                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return true;
                                    return false;
                                #endregion

                                #region product
                                case DetailLevelItemInformation.Levels.product:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
                                #endregion

                                #region Marques
                                // Droit des Marques
                                case DetailLevelItemInformation.Levels.brand:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_MARQUE));
                                #endregion

                                #region Sub Brand
                                case DetailLevelItemInformation.Levels.subBrand:
                                    return (true);
                                #endregion

                                #region Groupe de société
                                case DetailLevelItemInformation.Levels.holdingCompany:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_HOLDING_COMPANY));

                                #endregion

                                #region Agences et groupe d'agence
                                case DetailLevelItemInformation.Levels.groupMediaAgency:
                                case DetailLevelItemInformation.Levels.agency:
                                    vehicleList = GetVehicles();
                                    return (_customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList));

                                #endregion

                                default:
                                    return (false);
                            }
                        default:
                            return (true);
                    }

                case WebConstantes.Module.Name.ALERTE_PLAN_MEDIA:
                case WebConstantes.Module.Name.ANALYSE_PLAN_MEDIA:
                    switch (currentDetailLevelItem.Id)
                    {
                        #region Annonceur
                        case DetailLevelItemInformation.Levels.advertiser:
                            vehicleList = GetVehicles();
                          if(vehicleList.Count == 1 && VehiclesInformation.Contains(Vehicles.names.mailValo) &&
                                               VehiclesInformation.Get(Vehicles.names.mailValo).DatabaseId ==
                                               vehicleList[0])  return (true);

                            if (
                                // Droit sur les niveaux de détail produit
                                CheckProductDetailLevelAccess() &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
                                ) &&
                                // Pas de famille, classe                                
                             !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes)  &&                              
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Product
                        case DetailLevelItemInformation.Levels.product:
                            if (
                                // Droit sur les niveaux de détail produit
                                CheckProductDetailLevelAccess() &&
                                // Products rights
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                 _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:

                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                               _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Famille, classe, groupe, variété
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:                      
                            if (CheckProductDetailLevelAccess()) return (true);
                            return (false);
                        case DetailLevelItemInformation.Levels.group:
                            if (CheckProductDetailLevelAccess() &&
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_GROUP_LEVEL_ACCESS_FLAG)) return (true);
                            return (false);
                        case DetailLevelItemInformation.Levels.segment:
                            if (CheckProductDetailLevelAccess() &&
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:

                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
                                  _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_HOLDING_COMPANY) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Agences et groupe d'agence
                        case DetailLevelItemInformation.Levels.groupMediaAgency:
                        case DetailLevelItemInformation.Levels.agency:
                            vehicleList = GetVehicles();
                            if (
                                // Droit sur les agences media
                                 _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
                                ) return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:

                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                                // Sélection par produit ou marque ou annonceur						
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)
                                ) &&
                                // Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) &&
                                // Niveau de détail par jour
                                _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly

                                )
                                return (true);
                            return (false);

                        #endregion

                        #region Media group
                        case DetailLevelItemInformation.Levels.mediaGroup:
                            return _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_MEDIA_GROUP);
                        #endregion

                        default:
                            return (true);
                    }


                case WebConstantes.Module.Name.ANALYSE_MANDATAIRES:
                    switch (currentDetailLevelItem.Id)
                    {
                        #region Annonceur
                        case DetailLevelItemInformation.Levels.advertiser:
                            if (
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Product
                        case DetailLevelItemInformation.Levels.product:
                            if (
                                // Products rights
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:

                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Famille, classe, groupe, variété
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:                      
                            return (true);
                        case DetailLevelItemInformation.Levels.group:
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_GROUP_LEVEL_ACCESS_FLAG)) return (true);
                            return (false);
                        case DetailLevelItemInformation.Levels.segment:
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:

                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_HOLDING_COMPANY) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SEGMENT, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISING_AGENCY, AccessType.includes) ||
                                _customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_ADVERTISING_AGENCY, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalAdvertisingAgnecyUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Agences et groupe d'agence
                        case DetailLevelItemInformation.Levels.groupMediaAgency:
                        case DetailLevelItemInformation.Levels.agency:
                            vehicleList = GetVehicles();
                            if (
                                // Droit sur les agences media
                                 _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
                                ) return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:
                            return (false);

                        #endregion

                        default:
                            return (true);
                    }


                case WebConstantes.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    switch (currentDetailLevelItem.Id)
                    {

                        #region Annonceur
                        case DetailLevelItemInformation.Levels.advertiser:

                            if (
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);

                        #endregion

                        #region product
                        case DetailLevelItemInformation.Levels.product:

                            if (
                                // Products level rights
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);

                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:

                            if (
                                // Droit des Marques
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_MARQUE) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);

                        #endregion

                        #region Famille, classe, groupe
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:
                        case DetailLevelItemInformation.Levels.group:
                            return (true);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:

                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_HOLDING_COMPANY) &&
                                // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                ) &&
                                // Pas de famille, classe
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Variété
                        case DetailLevelItemInformation.Levels.segment:
                            return (false);
                        #endregion

                        #region Agences et groupe d'agence
                        case DetailLevelItemInformation.Levels.groupMediaAgency:
                        case DetailLevelItemInformation.Levels.agency:
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:

                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                                // Sélection par produit ou marque ou annonceur ou Groupe de sociétés
                                (_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.PRODUCT, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.HOLDING_COMPANY, AccessType.includes) ||
                                _customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                ) &&
                                // Pas de famille, classe, groupe, groupe d'annonceur 							
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SUB_SECTOR, AccessType.includes) &&
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.GROUP_, AccessType.includes)
                                )
                                return (true);
                            return (false);

                        #endregion

                        default:
                            return (true);
                    }
                case WebConstantes.Module.Name.ANALYSE_DES_PROGRAMMES:
                    switch (currentDetailLevelItem.Id)
                    {

                        #region Emissions, Genres d'émissions,formes de parrainage,support
                        case DetailLevelItemInformation.Levels.program:
                        case DetailLevelItemInformation.Levels.programType:
                        case DetailLevelItemInformation.Levels.sponsorshipForm:
                        case DetailLevelItemInformation.Levels.media:
                            return (true);
                        #endregion

                        #region famille, classe, groupe, Annonceur, produit
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:
                        case DetailLevelItemInformation.Levels.group:
                        case DetailLevelItemInformation.Levels.advertiser:
                            return (true);
                        #endregion

                        #region Products
                        case DetailLevelItemInformation.Levels.product:
                            // Product level rights
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:
                            // Droit des Marques
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_MARQUE))
                                return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:
                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:
                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(Constantes.DB.Flags.ID_HOLDING_COMPANY)
                                ) return (true);
                            return (false);
                        #endregion

                        default:
                            return (false);
                    }
                default:
                    return (true);
            }
        }

    }
}

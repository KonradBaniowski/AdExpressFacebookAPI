#region Informations
/*
 * Author : G Ragneau
 * Created on : 23/09/2008
 * Modifications :
 *      Date - Author - Description
 * 
 * 
 * 
 * 
 * 
 * */
/*
 * history: moved from TNS.AdExpress.Web
 * Auteur:D. V. Mussuma
 * Création: 12/12/2005
 * Modification:
 *      12/01/2006 B. Masson Ajout des niveaux de détail supports

 * */
#endregion

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Domain.Level;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Core.Russia.Utilities
{
    /// <summary>
    /// Fonctions des niveaux de détail média
    /// </summary>
    public class MediaDetailLevel : TNS.AdExpress.Web.Core.Utilities.MediaDetailLevel
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
        /// Obtient le niveau de détail media par défaut en fonction du module
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Niveau de détail</returns>
        public override WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails GetDefaultPreformatedMediaDetails(WebSession webSession)
        {
            if (webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE && webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
            {

                if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS)
                    return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.Media;
                else return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia;
            }
            else return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
        }

        /// <summary>
        /// Get Default Generic Detail Level Ids
        /// </summary>
        /// <param name="currentModule">current Module</param>
        /// <returns>Array List etail Level Ids</returns>
        public override ArrayList GetDefaultGenericDetailLevelIds(long currentModule)
        {
            #region Niveau de détail media (Generic)
            var levels = new ArrayList();
            switch (currentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    // Support
                    levels.Add(3);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                    levels.Add(8);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                    // Famille
                    levels.Add(11);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.CELEBRITIES:
                    //Profession/Name
                    levels.Add(68);
                    levels.Add(69);                   
                    break;
                default:
                    // Media/Support
                    levels.Add(1);
                    levels.Add(3);
                    break;
            }
            return levels;
            #endregion
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
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0
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
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0
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
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe, groupe, groupe d'annonceur
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        )
                                        return (true);
                                    return (false);

                                #endregion

                                #region Famille, classe, groupe, variété
                                case DetailLevelItemInformation.Levels.sector:
                                case DetailLevelItemInformation.Levels.subSector:
                                case DetailLevelItemInformation.Levels.group:
                                case DetailLevelItemInformation.Levels.segment:
                                    if (CheckProductDetailLevelAccess()) return (true);
                                    return (false);
                                #endregion

                                #region Groupe de société
                                case DetailLevelItemInformation.Levels.holdingCompany:
                                    if (
                                        CheckProductDetailLevelAccess() &&
                                        // Droit sur les groupe de société
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
                                        // Accès si sélection en groupe de société, annonceur, marque, produit, groupe et variété
                                        (_customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length > 0) &&
                                        // Pas de famille, classe
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0
                                        ) return (true);
                                    return (false);
                                #endregion

                                #region Agences et groupe d'agence
                                case DetailLevelItemInformation.Levels.groupMediaAgency:
                                case DetailLevelItemInformation.Levels.agency:
                                    if (
                                        CheckProductDetailLevelAccess() &&
                                        // Droit sur les agences media
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY)
                                        ) return (true);
                                    return (false);
                                #endregion

                                #region Version
                                case DetailLevelItemInformation.Levels.slogan:
                                    if (
                                        currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                                        // Sélection par produit ou marque ou annonceur
                                        (
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.productAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.brandAccess).Length > 0 ||
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.advertiserAccess).Length > 0) &&
                                        // Pas de famille, classe, groupe,variété, groupe d'annonceur webSession.ProductDetailLevel.LevelProduct
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.sectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.subSectorAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.groupAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.segmentAccess).Length == 0 &&
                                        _customerWebSession.GetSelection(_customerWebSession.ProductDetailLevel.ListElement, TNS.AdExpress.Constantes.Customer.Right.type.holdingCompanyAccess).Length == 0 &&
                                        // Niveau de détail par jour
                                        _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly

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
                                case DetailLevelItemInformation.Levels.group:
                                case DetailLevelItemInformation.Levels.segment:
                                case DetailLevelItemInformation.Levels.advertiser:
                                case DetailLevelItemInformation.Levels.publicationType:
                                    return (true);
                                #endregion

                                #region product
                                case DetailLevelItemInformation.Levels.product:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG));
                                #endregion

                                #region Marques
                                // Droit des Marques
                                case DetailLevelItemInformation.Levels.brand:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE));
                                #endregion

                                #region Sub Brand
                                case DetailLevelItemInformation.Levels.subBrand:
                                    return (true);
                                #endregion

                                #region Groupe de société
                                case DetailLevelItemInformation.Levels.holdingCompany:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY));

                                #endregion

                                #region Agences et groupe d'agence
                                case DetailLevelItemInformation.Levels.groupMediaAgency:
                                case DetailLevelItemInformation.Levels.agency:
                                    return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY));

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
                            if ( // Droit sur les niveaux de détail produit
                                CheckProductDetailLevelAccess()
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:

                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Famille, classe, groupe, variété
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:
                        case DetailLevelItemInformation.Levels.group:
                        case DetailLevelItemInformation.Levels.segment:
                            if (CheckProductDetailLevelAccess()) return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:

                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Agences et groupe d'agence
                        case DetailLevelItemInformation.Levels.groupMediaAgency:
                        case DetailLevelItemInformation.Levels.agency:
                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les agences media
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MEDIA_AGENCY)
                                ) return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:

                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
                                // Niveau de détail par jour
                                _customerWebSession.DetailPeriod == TNS.AdExpress.Constantes.Web.CustomerSessions.Period.DisplayLevel.dayly
                                )
                                return (true);
                            return (false);

                        #endregion

                        default:
                            return (true);
                    }

                case WebConstantes.Module.Name.CELEBRITIES:
                    switch (currentDetailLevelItem.Id)
                    {
                        #region Annonceur
                        case DetailLevelItemInformation.Levels.advertiser:
                            if ( // Droit sur les niveaux de détail produit
                                CheckProductDetailLevelAccess()
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(TNS.AdExpress.Constantes.DB.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:

                            if (
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_MARQUE)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Famille, classe, groupe, variété
                        case DetailLevelItemInformation.Levels.sector:
                        case DetailLevelItemInformation.Levels.subSector:
                        case DetailLevelItemInformation.Levels.group:
                        case DetailLevelItemInformation.Levels.segment:
                            if (CheckProductDetailLevelAccess()) return (true);
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

                            return (true);

                        #endregion

                        #region product
                        case DetailLevelItemInformation.Levels.product:

                            return (true);


                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:


                            return (true);


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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY)
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
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)

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
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG))
                                return (true);
                            return (false);
                        #endregion

                        #region Marques
                        case DetailLevelItemInformation.Levels.brand:
                            // Droit des Marques
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE))
                                return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:
                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG)
                                )
                                return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:
                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY)
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

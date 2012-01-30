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
using ConstantesFrwkResults = TNS.AdExpress.Constantes.FrameWork.Results;
using DBConstantes = TNS.AdExpress.Constantes.DB;

using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.FrameWork;
using TNS.AdExpress.Domain.Translation;
using TNS.Classification.Universe;

namespace TNS.AdExpress.Web.Core.Utilities{
	/// <summary>
	/// Fonctions des niveaux de détail média
	/// </summary>
	public class MediaDetailLevel{
        ///<summary>
		/// Profile du composant
		/// </summary>
		///  <label>_componentProfile</label>
		protected WebConstantes.GenericDetailLevel.ComponentProfile _componentProfile=WebConstantes.GenericDetailLevel.ComponentProfile.media;
        	///<summary>
		/// Session du client
		/// </summary>
		///  <label>_customerWebSession</label>
		protected WebSession _customerWebSession = null;

        /// <summary>
        /// Constructor
        /// </summary>
        public MediaDetailLevel()
        {
        }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="customerWebSession">_customer WebSession</param>
        /// <param name="componentProfile">component Profile</param>
        public MediaDetailLevel(WebSession customerWebSession, WebConstantes.GenericDetailLevel.ComponentProfile componentProfile)
        {
            _customerWebSession = customerWebSession;
            _componentProfile = componentProfile;
        }


        /// <summary>
        /// Obtient le niveau de détail media par défaut en fonction du module
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Niveau de détail</returns>
        public virtual WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails GetDefaultPreformatedMediaDetails(WebSession webSession)
        {
            if (webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.TABLEAU_DYNAMIQUE && webSession.CurrentModule != TNS.AdExpress.Constantes.Web.Module.Name.INDICATEUR)
            {

                if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS)
                    return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.Media;
                else if (webSession.CurrentModule == TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_MANDATAIRES)
                    return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
                else return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory;
            }
            else return TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle;
        }

        /// <summary>
        /// Get Default Generic Detail Level Ids
        /// </summary>
        /// <param name="currentModule">current Module</param>
        /// <returns>Array List etail Level Ids</returns>
        public virtual ArrayList GetDefaultGenericDetailLevelIds(long currentModule)
        {
            #region Niveau de détail media (Generic)
            ArrayList levels = new ArrayList();
            switch (currentModule)
            {
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    // Support
                    levels.Add(3);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_DES_PROGRAMMES:
                    levels.Add(8);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case TNS.AdExpress.Constantes.Web.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                    // Media/catégorie/Support
                    levels.Add(1);
                    levels.Add(2);
                    levels.Add(3);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.NEW_CREATIVES:
                    // Famille
                    levels.Add(11);
                    break;
                case TNS.AdExpress.Constantes.Web.Module.Name.VP:
                    //VP Product category
                    levels.Add(65);
                    //VP product
                    levels.Add(67);
                    //VP Brand
                    levels.Add(66);
                    break;
                default:
                    // Media/catégorie
                    levels.Add(1);
                    levels.Add(2);
                    break;
            }
            return levels;
            #endregion
        }

		/// <summary>
		/// Get List of filters in generic detail levels
		/// </summary>
        /// <param name="session">User session</param>	
		/// <param name="idLevel1">Level 1 ID</param>
        /// <param name="idLevel2">Level 2 ID</param>
        /// <param name="idLevel3">Level 3 ID</param>
        /// <param name="idLevel4">Level 4 ID</param>
		/// <returns>List of levels matching filters</returns>
        public static Dictionary<DetailLevelItemInformation, long> GetFilters(WebSession session, Int64 idLevel1, Int64 idLevel2, Int64 idLevel3, Int64 idLevel4) {
            string temp = "";
            Dictionary<DetailLevelItemInformation, long> mediaCol = null;
            int indexLevel = 0;
            if (session.PreformatedMediaDetail.ToString().Length > 0)
            {
                temp = session.PreformatedMediaDetail.ToString().ToUpper();
                mediaCol = new Dictionary<DetailLevelItemInformation, long>();

                if (session.GenericMediaDetailLevel != null && session.GenericMediaDetailLevel.Levels != null && session.GenericMediaDetailLevel.Levels.Count > 0)
                {
                    for (int i = 0; i < session.GenericMediaDetailLevel.Levels.Count; i++)
                    {
                        if (session.GenericMediaDetailLevel.Levels[i] != null
                            && ((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i]).DataBaseIdField != null
                            && ((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i]).DataBaseIdField.Length > 0
                            ) {
                            indexLevel++;
                            switch (indexLevel) {
                                case 1:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel1); //identifiant du média est la clé
                                    break;
                                case 2:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel2);
                                    break;
                                case 3:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel3);
                                    break;
                                case 4:
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], idLevel4);
                                    break;
                                default: 
                                    mediaCol.Add((DetailLevelItemInformation)session.GenericMediaDetailLevel.Levels[i], -1);
                                    break;
                            }
                        }
                    }
                }

            }
            return mediaCol;

        }

        /// <summary>
        /// Obtient le texte du niveau de détail support
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Texte du niveau de détail support</returns>
        public static string LevelMediaToHtmlString(WebSession webSession)
        {
            try
            {
                switch (webSession.PreformatedMediaDetail)
                {
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicle:
                        return (GestionWeb.GetWebWord(1292, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategory:
                        return (GestionWeb.GetWebWord(1142, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMedia:
                        return (GestionWeb.GetWebWord(1143, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMedia:
                        return (GestionWeb.GetWebWord(1544, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                        return (GestionWeb.GetWebWord(1542, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                        return (GestionWeb.GetWebWord(1543, webSession.SiteLanguage));

                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSeller:
                        return (GestionWeb.GetWebWord(1860, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMedia:
                        return (GestionWeb.GetWebWord(1861, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMedia:
                        return (GestionWeb.GetWebWord(1862, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMedia:
                        return (GestionWeb.GetWebWord(1863, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenter:
                        return (GestionWeb.GetWebWord(1864, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMedia:
                        return (GestionWeb.GetWebWord(1865, webSession.SiteLanguage));

                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleCategoryMediaSlogan:
                        return (GestionWeb.GetWebWord(1866, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSlogan:
                        return (GestionWeb.GetWebWord(1867, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMediaSlogan:
                        return (GestionWeb.GetWebWord(1868, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleMediaSellerMediaSlogan:
                        return (GestionWeb.GetWebWord(1869, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerMediaSlogan:
                        return (GestionWeb.GetWebWord(1870, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerVehicleMediaSlogan:
                        return (GestionWeb.GetWebWord(1871, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.mediaSellerInterestCenterMediaSlogan:
                        return (GestionWeb.GetWebWord(1872, webSession.SiteLanguage));

                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMedia:
                        return (GestionWeb.GetWebWord(1873, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMedia:
                        return (GestionWeb.GetWebWord(1874, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleCategoryMedia:
                        return (GestionWeb.GetWebWord(1875, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleInterestCenterMedia:
                        return (GestionWeb.GetWebWord(1876, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganVehicleMediaSellerMedia:
                        return (GestionWeb.GetWebWord(1877, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerMedia:
                        return (GestionWeb.GetWebWord(1878, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerVehicleMedia:
                        return (GestionWeb.GetWebWord(1879, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sloganMediaSellerInterestCenterMedia:
                        return (GestionWeb.GetWebWord(1880, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.Media:
                        return (GestionWeb.GetWebWord(971, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.programTypeProgram:
                        return (GestionWeb.GetWebWord(2070, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.sponsorshipForm:
                        return (GestionWeb.GetWebWord(2052, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.region:
                        return (GestionWeb.GetWebWord(2652, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleRegion:
                        return (GestionWeb.GetWebWord(2740, webSession.SiteLanguage));                  
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.regionMedia:
                        return (GestionWeb.GetWebWord(2731, webSession.SiteLanguage));
                    case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleRegionMedia:
                        return (GestionWeb.GetWebWord(2741, webSession.SiteLanguage));
                    default:
                        return ("no value");
                }
            }
            catch (Exception)
            {
                return ("no value");
            }
        }

        /// <summary>
        /// Obtient le texte du niveau de détail support
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <returns>Texte du niveau de détail support</returns>
        public static string LevelMediaToExcelString(WebSession webSession)
        {
            return (Convertion.ToHtmlString(LevelMediaToHtmlString(webSession)));
        }

        /// <summary>
        /// Vérifie si le niveau 2 de détail média concerne les centres d'intérêts 
        /// </summary>
        /// <param name="webSession">session du client</param>
        /// <returns>vrai si centres d'intérêts </returns>
        public static bool IsLevel2InterestCenter(WebSession webSession)
        {
            switch (webSession.PreformatedMediaDetail)
            {
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenter:
                case WebConstantes.CustomerSessions.PreformatedDetails.PreformatedMediaDetails.vehicleInterestCenterMedia:
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Obtient la liste des média impacté par le détail média.
        /// </summary>
        /// <param name="webSession">Session du client</param>	
        /// <param name="idMediaLevel1">Identifiant du media niveau 1</param>
        /// <param name="idMediaLevel2">Identifiant du media niveau 2</param>
        /// <param name="idMediaLevel3">Identifiant du media niveau 3</param>
        /// <param name="idMediaLevel4">Identifiant du media niveau 4</param>
        /// <returns>liste des média </returns>
        public static ListDictionary GetImpactedMedia(WebSession webSession, Int64 idMediaLevel1, Int64 idMediaLevel2, Int64 idMediaLevel3, Int64 idMediaLevel4)
        {
            string temp = "";
            ListDictionary mediaCol = null;
            //			object[,] tempMediaList = null;
            int indexLevel = 0;
            if (webSession.PreformatedMediaDetail.ToString().Length > 0)
            {
                temp = webSession.PreformatedMediaDetail.ToString().ToUpper();
                mediaCol = new ListDictionary();

                if (webSession.GenericMediaDetailLevel != null && webSession.GenericMediaDetailLevel.Levels != null && webSession.GenericMediaDetailLevel.Levels.Count > 0)
                {
                    for (int i = 0; i < webSession.GenericMediaDetailLevel.Levels.Count; i++)
                    {
                        if (webSession.GenericMediaDetailLevel.Levels[i] != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField != null
                            && ((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField.Length > 0
                            )
                        {
                            indexLevel++;
                            switch (indexLevel)
                            {
                                case 1:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel1); //identifiant du média est la clé
                                    break;
                                case 2:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel2);
                                    break;
                                case 3:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel3);
                                    break;
                                case 4:
                                    mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, idMediaLevel4);
                                    break;
                                default: mediaCol.Add(((DetailLevelItemInformation)webSession.GenericMediaDetailLevel.Levels[i]).DataBaseIdField, -1);
                                    break;
                            }
                        }
                    }
                }

            }
            return mediaCol;
        }

        /// <summary>
        /// Obtient la liste de niveaux de détail média
        /// </summary>
        /// <returns>liste de niveaux de détail média</returns>
        public static object[,] GetMediaList()
        {
            object[,] tab = new object[6, 2];

            tab[0, 0] = ConstantesFrwkResults.CommonMother.VEHICLE_LABEL;
            tab[0, 1] = DBConstantes.Fields.ID_VEHICLE;

            tab[1, 0] = ConstantesFrwkResults.CommonMother.CATEGORY_LABEL;
            tab[1, 1] = DBConstantes.Fields.ID_CATEGORY;

            tab[2, 0] = ConstantesFrwkResults.CommonMother.INTERESTCENTER_LABEL;
            tab[2, 1] = DBConstantes.Fields.ID_INTEREST_CENTER;

            tab[3, 0] = ConstantesFrwkResults.CommonMother.MEDIASELLER_LABEL;
            tab[3, 1] = DBConstantes.Fields.ID_MEDIA_SELLER;

            tab[4, 0] = ConstantesFrwkResults.CommonMother.MEDIA_LABEL;
            tab[4, 1] = DBConstantes.Fields.ID_MEDIA;

            tab[5, 0] = ConstantesFrwkResults.CommonMother.SLOGAN_LABEL;
            tab[5, 1] = DBConstantes.Fields.ID_SLOGAN;

            return tab;
        }

        ///// <summary>
        ///// Vérifie si le client aux accroches en fonction du niveau de détail produit et média.
        ///// </summary>
        ///// <remarks>Les niveaux de détail par accroche sont disponibles uniquement dans 
        ///// un plan media défini par marque ou par produit
        ///// </remarks>
        ///// <param name="webSession">Session du client</param>
        ///// <returns>Vrai si le client à accès aux accroches</returns>
        //public static bool HasSloganRight(WebSession webSession){
        //    if(webSession!=null && webSession.CustomerLogin.GetFlag((long)TNS.AdExpress.Constantes.DB.Flags.ID_SLOGAN_ACCESS_FLAG)!=null 
        //        )return true;
        //    else return false;
        //}

        #region Niveau de détail support générique
        /// <summary>
        /// Niveau de détail support ou média(Niveaux détaillés par  :)
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="displayGenericlevelDetailLabel">Indique si affiche niveau de détail générique</param>
        /// <param name="genericlevelDetailLabel">Libellé niveaux de détail</param>
        /// <param name="excel">Indique si sortie excel</param>
        /// <returns>HTML</returns>
        public static void GetGenericLevelDetail(WebSession webSession, ref bool displayGenericlevelDetailLabel, System.Web.UI.WebControls.Label genericlevelDetailLabel, bool excel)
        {
            ArrayList detailSelections = null;

            //webSession.CustomerLogin.ModuleList();
            Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
            try
            {
                detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)webSession.CurrentTab)).DetailSelectionItemsType;
            }
            catch (System.Exception)
            {
                if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
            }
            foreach (int currentType in detailSelections)
            {
                switch ((WebConstantes.DetailSelection.Type)currentType)
                {
                    case WebConstantes.DetailSelection.Type.genericMediaLevelDetail:
                        genericlevelDetailLabel.Text = (excel) ? Convertion.ToHtmlString(webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage)) : webSession.GenericMediaDetailLevel.GetLabel(webSession.SiteLanguage);
                        if (genericlevelDetailLabel.Text != null && genericlevelDetailLabel.Text.Length > 0)
                            displayGenericlevelDetailLabel = true;
                        break;
                    case WebConstantes.DetailSelection.Type.genericProductLevelDetail:
                        genericlevelDetailLabel.Text = (excel) ? Convertion.ToHtmlString(webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage)) : webSession.GenericProductDetailLevel.GetLabel(webSession.SiteLanguage);
                        if (genericlevelDetailLabel.Text != null && genericlevelDetailLabel.Text.Length > 0)
                            displayGenericlevelDetailLabel = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion

        #region Niveau de détail colonne générique
        /// <summary>
        /// Niveau de détail colonne générique
        /// </summary>
        /// <param name="webSession">Session du client</param>
        /// <param name="displayGenericlevelDetailColumnLabel">Indique si affiche niveau de détail générique</param>
        /// <param name="genericlevelDetailColumnLabel">Libellé niveaux de détail</param>
        /// <param name="excel">Indique si sortie excel</param>
        /// <returns>HTML</returns>
        public static void GetGenericLevelDetailColumn(WebSession webSession, ref bool displayGenericlevelDetailColumnLabel, System.Web.UI.WebControls.Label genericlevelDetailColumnLabel, bool excel)
        {
            ArrayList detailSelections = null;

            //webSession.CustomerLogin.ModuleList();
            Module currentModule = webSession.CustomerLogin.GetModule(webSession.CurrentModule);
            try
            {
                detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation((int)webSession.CurrentTab)).DetailSelectionItemsType;
            }
            catch (System.Exception)
            {
                if (currentModule.Id == WebConstantes.Module.Name.ALERTE_PORTEFEUILLE)
                    detailSelections = ((ResultPageInformation)currentModule.GetResultPageInformation(5)).DetailSelectionItemsType;
            }
            foreach (int currentType in detailSelections)
            {
                switch ((WebConstantes.DetailSelection.Type)currentType)
                {
                    case WebConstantes.DetailSelection.Type.genericColumnLevelDetail:
                        genericlevelDetailColumnLabel.Text = (excel) ? Convertion.ToHtmlString(webSession.GenericColumnDetailLevel.GetLabel(webSession.SiteLanguage)) : webSession.GenericColumnDetailLevel.GetLabel(webSession.SiteLanguage);
                        if (genericlevelDetailColumnLabel.Text != null && genericlevelDetailColumnLabel.Text.Length > 0)
                            displayGenericlevelDetailColumnLabel = true;
                        break;
                    default:
                        break;
                }
            }
        }
        #endregion


        /// <summary>
        /// Test si l'élément de niveau de détail peut être montré
        /// </summary>
        /// <remarks>
        /// AdNetTrack selection [Ko]
        /// </remarks>
        /// <param name="currentDetailLevelItem">Elément de niveau de détail</param>
        /// <param name="module">Module</param>
        /// <returns>True si oui false sinon</returns>
        public virtual bool CanAddDetailLevelItem(DetailLevelItemInformation currentDetailLevelItem, Int64 module)
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
                                    if (CheckProductDetailLevelAccess()) return (true);
                                    return (false);
                                case DetailLevelItemInformation.Levels.segment:
                                    if (CheckProductDetailLevelAccess() &&
                                        _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
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
                                    vehicleList = GetVehicles();
                                    if (
                                        CheckProductDetailLevelAccess() &&
                                        // Droit sur les agences media
                                        _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
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
                                case DetailLevelItemInformation.Levels.advertiser:
                                case DetailLevelItemInformation.Levels.publicationType:
                                    return (true);
                                case DetailLevelItemInformation.Levels.segment:
                                    if (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return true;
                                    return false;
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
                                !_customerWebSession.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.SECTOR, AccessType.includes) &&
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
                        case DetailLevelItemInformation.Levels.group:
                            if (CheckProductDetailLevelAccess()) return (true);
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG) &&
                                  _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
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
                                CheckProductDetailLevelAccess() &&
                                // Droit sur les agences media
                                 _customerWebSession.CustomerLogin.CustomerMediaAgencyFlagAccess(vehicleList)
                                ) return (true);
                            return (false);
                        #endregion

                        #region Version
                        case DetailLevelItemInformation.Levels.slogan:

                            if (
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
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
                        case DetailLevelItemInformation.Levels.group:
                            return (true);
                        case DetailLevelItemInformation.Levels.segment:
                            if (_customerWebSession.CustomerLogin.CustormerFlagAccess((long)TNS.AdExpress.Constantes.DB.Flags.ID_SEGMENT_LEVEL_ACCESS_FLAG)) return (true);
                            return (false);
                        #endregion

                        #region Groupe de société
                        case DetailLevelItemInformation.Levels.holdingCompany:

                            if (
                                // Droit sur les groupe de société
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_PRODUCT_LEVEL_ACCESS_FLAG) &&
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_MARQUE) &&
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
                                _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_HOLDING_COMPANY) &&
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
                                currentDetailLevelItem.Id == DetailLevelItemInformation.Levels.slogan && _customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_SLOGAN_ACCESS_FLAG) &&
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

		/// <summary>
		/// Vérifie si le client à le droit de voir un détail produit dans les plan media
		/// </summary>
		/// <remarks>
		/// AdNetTrack selection [Ok]
		/// </remarks>
		/// <returns>True si oui false sinon</returns>
		public virtual bool CheckProductDetailLevelAccess(){
			return (_customerWebSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.MEDIA_SCHEDULE_PRODUCT_DETAIL_ACCESS_FLAG));
		}

        protected List<Int64> GetVehicles()
        {
            List<Int64> vehicleList = new List<Int64>();
            string listStr = _customerWebSession.GetSelection(_customerWebSession.SelectionUniversMedia, TNS.AdExpress.Constantes.Customer.Right.type.vehicleAccess);
            if (listStr != null && listStr.Length > 0)
            {
                string[] list = listStr.Split(',');
                for (int i = 0; i < list.Length; i++)
                    vehicleList.Add(Convert.ToInt64(list[i]));
            }
            else
            {
                //When a vehicle is not checked but one or more category, this get the vehicle correspondly
                string Vehicle = ((LevelInformation)_customerWebSession.SelectionUniversMedia.FirstNode.Tag).ID.ToString();
                vehicleList.Add(Convert.ToInt64(Vehicle));
            }
            return vehicleList;
        }
	}
}

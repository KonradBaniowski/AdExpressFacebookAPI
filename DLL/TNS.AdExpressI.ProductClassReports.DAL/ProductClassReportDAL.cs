#region Information
/*
 * Author : G Ragneau
 * Created on : 17/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * Origine:
 *      Auteur: G. Ragneau
 *      Date de création: 22/09/2004
 *      Date de modification: 27/09/2004
 *      18/02/2005	A.Obermeyer		rajout Marque en personnalisation
 *      23/08/2005	G. Facon		Solution temporaire pour les IDataSource
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;


using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstDB = TNS.AdExpress.Constantes.DB;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Exceptions;
using TNS.FrameWork.DB.Common;
using TNS.AdExpressI.ProductClassReports.DAL.Exceptions;
using TNS.Classification.Universe;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpressI.Date.DAL;
using System.Reflection;


namespace TNS.AdExpressI.ProductClassReports.DAL
{

    /// <summary>
    /// Default behaviour of DAL layer
    /// </summary>
    public abstract class ProductClassReportDAL : IProductClassReportsDAL
    {

        #region Attributes
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Type of result
        /// </summary>
        protected int _tableType;
        /// <summary>
        /// Report vehicle
        /// </summary>
        protected CstDBClassif.Vehicles.names _vehicle;
        /// <summary>
        /// Data Table
        /// </summary>
        protected Table _dataTable;
        /// <summary>
        /// Type of report
        /// </summary>
        protected CstFormat.PreformatedTables _reportFormat;
        /// <summary>
        /// Key word for searching advertisers
        /// </summary>
        protected string _keyWord = "";

        #region Tables
        Table _recapVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
        Table _recapCategory = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
        Table _recapMedia = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
        Table _recapGroup = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapGroup);
        Table _recapSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSegment);
        Table _recapBrand = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapBrand);
        Table _recapProduct = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapProduct);
        Table _recapAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapAdvertiser);
        /* WARNING !!! the two following tables are added temporarily in order to add specific levels for the Finnish version
        **/
        Table _recapSector = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSector);
        Table _recapSubSector = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSubSector);
        Table _dataRadio = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadio);
        Table _dataTV = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTv);
        Table _dataPluri = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluri);
        Table _dataPress = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPress);
        Table _dataOutdoor = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoor);
        Table _dataInternet = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternet);
        Table _dataCinema = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinema);
        Table _dataTactic = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTactic);
        Table _dataRadioSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadioSegment);
        Table _dataTVSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTvSegment);
        Table _dataPluriSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluriSegment);
        Table _dataPressSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPressSegment);
        Table _dataOutdoorSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoorSegment);
        Table _dataInternetSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternetSegment);
        Table _dataCinemaSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinemaSegment);
        Table _dataTacticSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTacticSegment);
        #endregion

        #endregion

        #region Accessors
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession Session
        {
            get { return _session; }
            set { _session = value; }
        }
        /// <summary>
        /// Type of result
        /// </summary>
        protected int TableType
        {
            get { return _tableType; }
            set { _tableType = value; }
        }
        #endregion

        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session)
        {
            _session = session;

            _recapVehicle = WebApplicationParameters.GetDataTable(TableIds.recapVehicle, _session.IsSelectRetailerDisplay);
            _recapCategory = WebApplicationParameters.GetDataTable(TableIds.recapCategory, _session.IsSelectRetailerDisplay);
            _recapMedia = WebApplicationParameters.GetDataTable(TableIds.recapMedia, _session.IsSelectRetailerDisplay);
            _recapGroup = WebApplicationParameters.GetDataTable(TableIds.recapGroup, _session.IsSelectRetailerDisplay);
            _recapSegment = WebApplicationParameters.GetDataTable(TableIds.recapSegment, _session.IsSelectRetailerDisplay);
            _recapBrand = WebApplicationParameters.GetDataTable(TableIds.recapBrand, _session.IsSelectRetailerDisplay);
            _recapProduct = WebApplicationParameters.GetDataTable(TableIds.recapProduct, _session.IsSelectRetailerDisplay);
            _recapAdvertiser = WebApplicationParameters.GetDataTable(TableIds.recapAdvertiser, _session.IsSelectRetailerDisplay);
            /* WARNING !!! the two following tables are added temporarily in order to add specific levels for the Finnish version
            **/
            _recapSector = WebApplicationParameters.GetDataTable(TableIds.recapSector, _session.IsSelectRetailerDisplay);
            _recapSubSector = WebApplicationParameters.GetDataTable(TableIds.recapSubSector, _session.IsSelectRetailerDisplay);
            _dataRadio = WebApplicationParameters.GetDataTable(TableIds.recapRadio, _session.IsSelectRetailerDisplay);
            _dataTV = WebApplicationParameters.GetDataTable(TableIds.recapTv, _session.IsSelectRetailerDisplay);
            _dataPluri = WebApplicationParameters.GetDataTable(TableIds.recapPluri, _session.IsSelectRetailerDisplay);
            _dataPress = WebApplicationParameters.GetDataTable(TableIds.recapPress, _session.IsSelectRetailerDisplay);
            _dataOutdoor = WebApplicationParameters.GetDataTable(TableIds.recapOutDoor, _session.IsSelectRetailerDisplay);
            _dataInternet = WebApplicationParameters.GetDataTable(TableIds.recapInternet, _session.IsSelectRetailerDisplay);
            _dataCinema = WebApplicationParameters.GetDataTable(TableIds.recapCinema, _session.IsSelectRetailerDisplay);
            _dataTactic = WebApplicationParameters.GetDataTable(TableIds.recapTactic, _session.IsSelectRetailerDisplay);
            _dataRadioSegment = WebApplicationParameters.GetDataTable(TableIds.recapRadioSegment, _session.IsSelectRetailerDisplay);
            _dataTVSegment = WebApplicationParameters.GetDataTable(TableIds.recapTvSegment, _session.IsSelectRetailerDisplay);
            _dataPluriSegment = WebApplicationParameters.GetDataTable(TableIds.recapPluriSegment, _session.IsSelectRetailerDisplay);
            _dataPressSegment = WebApplicationParameters.GetDataTable(TableIds.recapPressSegment, _session.IsSelectRetailerDisplay);
            _dataOutdoorSegment = WebApplicationParameters.GetDataTable(TableIds.recapOutDoorSegment, _session.IsSelectRetailerDisplay);
            _dataInternetSegment = WebApplicationParameters.GetDataTable(TableIds.recapInternetSegment, _session.IsSelectRetailerDisplay);
            _dataCinemaSegment = WebApplicationParameters.GetDataTable(TableIds.recapCinemaSegment, _session.IsSelectRetailerDisplay);
            _dataTacticSegment = WebApplicationParameters.GetDataTable(TableIds.recapTacticSegment, _session.IsSelectRetailerDisplay);
        }
        #endregion

        #region IProductClassReportsDAL Membres
        /// <summary>
        /// Get Data from database for the report sepcified in the user session
        /// </summary>
        /// <returns>DataSet with required data to build the report</returns>
        public DataSet GetData()
        {
            return GetData((int)_session.PreformatedTable);
        }
        /// <summary>
        /// Get Data from database
        /// </summary>
        /// <param name="resultType">Type of report</param>
        /// <returns>DataSet with required data to build the report</returns>
        public DataSet GetData(int resultType)
        {
            this._tableType = resultType;
            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
            _reportFormat = (CstFormat.PreformatedTables)_tableType;
            return GetDataSet();
        }
        #endregion

        #region IProductClassReportsDAL : GetUniversAdvertisers
         /// <summary>
        /// Get Advertisers which are part of the selected univers
        /// </summary>
        /// <param name="exclude">List of Advertiser Ids to exclude from the result</param>
        /// <param name="keyWord">Key word for advertisers to search</param>
        /// <returns>DataSet with (id_advertiser, advertiser) records</returns>
        public virtual DataSet GetUniversAdvertisers(string exclude,string keyWord)
        {
            _keyWord = keyWord;

            _vehicle = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID);
            StringBuilder sql = new StringBuilder(2000);

            #region Request building
            try
            {

                _dataTable = GetVehicleTableName(true);
                sql.AppendFormat("select distinct {0}.id_advertiser, {1}.advertiser from {2}, {3} ", _dataTable.Prefix, _recapAdvertiser.Prefix, _dataTable.SqlWithPrefix, _recapAdvertiser.SqlWithPrefix);
                AppendJointClause(sql);
                AppendSelectionClause(sql);
                if (exclude != null && exclude.Length > 0)
                {
                    string magicList = FctUtilities.SQLGenerator.GetInClauseMagicMethod(string.Format("{0}.id_advertiser", _recapAdvertiser.Prefix), exclude, false);
                    if (magicList != null && magicList.Length > 0)
                    {
                        sql.AppendFormat(" and {0} ", magicList);
                    }
                }
                //Searching advertisers
                if(!string.IsNullOrEmpty(_keyWord))
                    sql.AppendFormat(" and upper({0}.advertiser) like upper('%{1}%')", _recapAdvertiser.Prefix, _keyWord);

                AppendRightClause(sql);
                AppendActivationLanguageClause(sql);
                sql.AppendFormat(" order by {0}.advertiser, {1}.id_advertiser", _recapAdvertiser.Prefix, _dataTable.Prefix);

            }
            catch (NoDataException e1) { throw e1; }
            catch (DeliveryFrequencyException e3) { throw e3; }
            catch (System.Exception e2) { throw e2; }
            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
            try
            {
                return (dataSource.Fill(sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw (new ProductClassReportsDALException("Unable to load data for report: " + sql.ToString(), err));
            }
            #endregion
        }
        /// <summary>
        /// Get Advertisers which are part of the selected univers
        /// </summary>
        /// <param name="exclude">List of Advertiser Ids to exclude from the result</param>
        /// <returns>DataSet with (id_advertiser, advertiser) records</returns>
        public virtual DataSet GetUniversAdvertisers(string exclude)
        {
            _keyWord = null;
            return GetUniversAdvertisers(exclude, _keyWord);

        }
        #endregion

        #region DAL Access
        /// <summary>
        /// Build and execute request
        /// It returns required data to the building of a report considering user session parameters.
        /// It builds the request by adding different clauses like select, from joins, product and media selection, rights, language and activation, sorts et groupings
        /// <seealso cref="TNS.AdExpress.Web.Core.Sessions._session"/>
        /// </summary>
        /// <remarks>!!!!!The rules class uses order of the fields in the select clause. It is stringly unadvised to change order of product and medias classification.
        ///	Uses methods:
        ///		void appendSelectClause(Stringuilder);
        ///		void appendFromClause(Stringuilder, String);
        ///		void appendJointClause(Stringuilder);
        ///		void appendSelectionClause(Stringuilder);
        ///		void appendRightClause(Stringuilder);
        ///		void appendActivationLanguageClause(Stringuilder, String);
        ///		void appendRegroupmentAndOrderClause(Stringuilder);
        /// </remarks>
        /// <exception cref="TNS.AdExpress.Domain.Exceptions.NoDataException()">
        /// Thrown when periods is unvalid
        /// </exception>
        /// <exception cref="TNS.AdExpress.Domain.Exceptions.DeliveryFrequencyException()">
        /// Thrown when data delivering frequency is not valid
        /// </exception>
        /// <exception cref="TNS.AdExpressI.ProductClassReport.DAL.Exceptions.ProductClassReportDALException()">
        /// Thrown when errors while connecting to database, runiing request, closing database
        /// </exception>
        /// <returns>DAL result</returns>
        protected virtual DataSet GetDataSet()
        {
            StringBuilder sql = new StringBuilder(2000);

            #region Construction de la requête
            try
            {

                _dataTable = GetVehicleTableName();
                //!!!!!The rules class uses order of the fields in the select clause. It is stringly unadvised to change order of product and medias classification.
                AppendSelectClause(sql);
                AppendFromClause(sql);
                AppendJointClause(sql);
                AppendSelectionClause(sql);
                AppendRightClause(sql);
                AppendActivationLanguageClause(sql);
                AppendRegroupmentAndOrderClause(sql);
            }
            catch (NoDataException e1) { throw e1; }
            catch (DeliveryFrequencyException e3) { throw e3; }
            catch (System.Exception e2) { throw e2; }
            #endregion

            #region Execution de la requête
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.
                GetDefaultConnection(DefaultConnectionIds.productClassAnalysis, 
                WebApplicationParameters.AllowedLanguages[_session.SiteLanguage].NlsSort);
            try
            {
                return (dataSource.Fill(sql.ToString()));
            }
            catch (Exception err)
            {
                throw (new ProductClassReportsDALException("Unable to load data for report: " + sql.ToString(), err));
            }
            #endregion

        }
        #endregion

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!droits media
        #region Méthodes Privées

        #region getVehicleTableName
        /// <summary>
        /// Méthode privée qui détecte la table de recap à utiliser en fonction de la sélection média, produit
        /// et du niveau de détail choisi
        ///		détection d'une étude monomédia ou pluri média ==> recap_tv ... ou recap_pluri
        ///		niveau de détail de la nomenclature produit ==> recap ou recap_segment
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTablesDataAccessException">
        /// Lancée si aucune table de la base de doonées ne correspond au vehicle spécifié dans la session utilisateur.
        /// </exception>
        /// <param name="productRequired">Specify if we need the product/advertiser/brand info</param>
        /// <returns>Chaîne de caractère correspondant au nom de la table à attaquer</returns>
        protected virtual Table GetVehicleTableName(bool productRequired)
        {		
			

            bool useTableWithLowestLevel = UseTableWithLowestLevel();

			#region Détection du type de table vehicle (pluri ou mono?)
            switch (_vehicle)
            {
                case CstDBClassif.Vehicles.names.cinema:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapCinema, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapCinemaSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.adnettrack:
                case CstDBClassif.Vehicles.names.czinternet:
                case CstDBClassif.Vehicles.names.internet:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapInternet, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapInternetSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.outdoor:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapOutDoor, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapOutDoorSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.instore:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapInStore, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapInStoreSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.indoor:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapInDoor, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapInDoorSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.radio:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapRadio, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapRadioSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.tv:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapTv, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapTvSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.press:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapPress, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapPressSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.magazine:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapMagazine, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapMagazineSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.newspaper:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapNewspaper, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapNewspaperSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.plurimedia:
                case CstDBClassif.Vehicles.names.PlurimediaWithoutMms:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapPluri, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapPluriSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mediasTactics:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapTactic, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapTacticSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mobileTelephony:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapMobileTel, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapMobileTelSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.emailing:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapEmailing, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapEmailingSegment, _session.IsSelectRetailerDisplay);
				case CstDBClassif.Vehicles.names.directMarketing:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketing, _session.IsSelectRetailerDisplay) 
                        : WebApplicationParameters.GetDataTable(TableIds.recapDirectMarketingSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.mms:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapMms, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapMmsSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.search:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapSearch, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapSearchSegment, _session.IsSelectRetailerDisplay);
                case CstDBClassif.Vehicles.names.social:
                    return (productRequired || useTableWithLowestLevel) ? WebApplicationParameters.GetDataTable(TableIds.recapSocial, _session.IsSelectRetailerDisplay)
                        : WebApplicationParameters.GetDataTable(TableIds.recapSocialSegment, _session.IsSelectRetailerDisplay);  
                default:
                    throw new ProductClassReportsDALException(string.Format("Vehicle n° {0} is not allowed.", _vehicle.GetHashCode()));
            }
            #endregion
   
        }

        /// <summary>
        /// Use table with lowets level 
        /// </summary>
        /// <returns>true if use table with lowets level </returns>
        protected virtual bool UseTableWithLowestLevel()
        {
            return(_session.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.includes)
                 || _session.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.ADVERTISER, AccessType.excludes)
                  || _session.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.includes)
                  || _session.PrincipalProductUniverses[0].ContainsLevel(TNSClassificationLevels.BRAND, AccessType.excludes)
                 || _session.CustomerLogin[CstRight.type.advertiserException].Length > 0
                 || _session.CustomerLogin[CstRight.type.advertiserAccess].Length > 0
                  || _session.CustomerLogin[CstRight.type.brandAccess].Length > 0
                 || _session.CustomerLogin[CstRight.type.brandException].Length > 0);
        }

        /// <summary>
        /// Méthode privée qui détecte la table de recap à utiliser en fonction de la sélection média, produit
        /// et du niveau de détail choisi
        ///		détection d'une étude monomédia ou pluri média ==> recap_tv ... ou recap_pluri
        ///		niveau de détail de la nomenclature produit ==> recap ou recap_segment
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTablesDataAccessException">
        /// Lancée si aucune table de la base de doonées ne correspond au vehicle spécifié dans la session utilisateur.
        /// </exception>
        /// <param name="_session">Session utilisateur</param>
        /// <returns>Chaîne de caractère correspondant au nom de la table à attaquer</returns>
        protected virtual Table GetVehicleTableName()
        {
            
            CstFormat.PreformatedProductDetails detail = _session.PreformatedProductDetail;
            bool segment =
                detail != CstFormat.PreformatedProductDetails.groupBrand
                && detail != CstFormat.PreformatedProductDetails.groupProduct
                && detail != CstFormat.PreformatedProductDetails.groupAdvertiser
                && detail != CstFormat.PreformatedProductDetails.advertiser
                && detail != CstFormat.PreformatedProductDetails.advertiserBrand
                && detail != CstFormat.PreformatedProductDetails.advertiserProduct
                && detail != CstFormat.PreformatedProductDetails.brand
                && detail != CstFormat.PreformatedProductDetails.brandProduct
                && detail != CstFormat.PreformatedProductDetails.segmentAdvertiser
                && detail != CstFormat.PreformatedProductDetails.segmentBrand
                && detail != CstFormat.PreformatedProductDetails.segmentProduct
                && detail != CstFormat.PreformatedProductDetails.product
                /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                **/
                && detail != CstFormat.PreformatedProductDetails.sectorAdvertiser
                && detail != CstFormat.PreformatedProductDetails.subSectorAdvertiser;			

            return GetVehicleTableName(!segment);
        }
        #endregion

        #region appendSelectClause
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>!!!!!La classe métier qui s'appuie sur cette classe BDD utilise l'ordre des champs ds le select
        ///	Il n'est pas recommandé de changer l'ordre des champs media, produit...
        ///	étape:
        ///		- Sauvegardede l'index du dernier caractère de la requête a son arrivé afin de pouvoir insérer un pmrceau de requête si nécessaire
        ///		- Traitement des champs qualitatifs:
        ///			- Si le tableau preformaté affiche des données de la nomenclature media:
        ///				suivant le niveau de détail demandé, on ajoute les champs à la requete en normalisant leurs noms
        ///				chaque nom représente la nomenclature media (id_m) et le niveau considéré(id_m1, id_m2...)
        ///				Au cas ou le niveau de détail n'est pas repertorié, on applique par défaut le niveau vehicle uniquement et on update la session
        ///			- Si le tableau preformaté affiche des données de la nomenclature produit:
        ///				suivant le détail demandé, on ajoute les champs à la requete en normalisant leurs noms
        ///				chaque nom représente la nomenclature produit (id_p) et le niveau considéré(id_p1, id_p2)
        ///				Au cas ou le niveau de détail n'est pas repertorié, on applique par défaut le détail group uniquement et on update la session
        ///				Pour les niveaux faisant intervenir la nomenclature annonceur, on sauvegarde dans une strig un morceau de requete ramenant l'id_advertiser
        ///			- si la nompenclature produit est la nomenclature principale, on l'insere devant les champs 
        ///			de la nomenclature support qui devient par consequent la omenclature secondaire.
        ///		- Traitement des champs quantitatifs:
        ///			- ON commence par valider la période considérées en fonction de la fréquence de livraison des données
        ///			Si jamais la période n'est pas valide (données non accessiblkes par raport a la frequence), on lance une exception NoData
        ///			- construction de la liste des mois month a attaquer. POur chaquer mois, on fera la somme des lignes a ramener
        ///			- si le tableau nécessite des années, on utilise le champ total_year
        ///			- si le tableau est un mensuel cimulé, on construit la requete en faisant la somme des mois precedent sur la période pour chaque mois a rapatrier
        ///			exemple: periode Mars ==> mai, on prend mars et mars+avril et mars+avril+mai...
        ///			- si le tableau est un mensuel + total, on ramene tous les mois de la liste month plus un champ qui est la somme de tous les mois
        ///		-Traitement de la notion d'annonceur referent ou concurrent:
        ///			si l'univers d'annonceurs concurrents ou l'univers d'annonceurs de reference ne sont pas vides (non exclusif), on concatene a la requete le champ id_advertiser
        ///		- La colonne total est une facilité de programmation pour la construction des tableaux de type 5.
        ///			en effet, en considérant un champ 'total' bidon, on inclu dans la requete un niveau de 
        ///			nomenclature suplémentaire qui sera traite de la meme maniere que la nomenclature principale
        ///			dans les classes metier (valable uniquement pour la ableau de type 5 pour l instant)
        /// </remarks>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">StringBuilder contenant la requete sql</param>
        protected virtual void AppendSelectClause(StringBuilder sql)
        {

            sql.Append("select");

            // Attaquer une table
            int downLoadDate = _session.DownLoadDate;

            #region champs descriptifs
            int mediaIndex = _reportFormat.ToString().IndexOf("edia");
            int productIndex = _reportFormat.ToString().IndexOf("roduct");
            int beginningIndex = sql.Length;
            string annonceurPerso = "", brandPerso = "";

            #region Champs nomenclature media
            if (mediaIndex > -1)
            {
                //nomenclature media présente dans le tableau préformaté
                switch (_session.PreformatedMediaDetail)
                {
                    case CstFormat.PreformatedMediaDetails.vehicle:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle as m1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleCategory:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_category as id_m2, category as m2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_category as id_m2, category as m2, {0}.id_media as id_m3, media as m3", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleMedia:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_media as id_m2, media as m2", _dataTable.Prefix);
                        break;
                    default:
                        _session.PreformatedMediaDetail = CstFormat.PreformatedMediaDetails.vehicle;
                        sql.Append(" {0}.id_vehicle as id_m1, vehicle as m1");
                        break;
                }
            }
            #endregion

            #region Champs nomenclature produit
            if (productIndex > -1)
            {
                string sqlStr = "";
                //nomenclature produit présente dans le tableau préformaté
                switch (_session.PreformatedProductDetail)
                {
                    case CstFormat.PreformatedProductDetails.group:
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupSegment:
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_segment as id_p2, segment as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupBrand:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        brandPerso = string.Format("{0}.id_brand", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_brand as id_p2, brand as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupProduct:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupAdvertiser:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_advertiser as id_p2, advertiser as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiser:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiserBrand:
                        brandPerso = string.Format("{0}.id_brand", _dataTable.Prefix);
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1, {0}.id_brand as id_p2, brand as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiserProduct:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.brand:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        brandPerso = string.Format("{0}.id_brand", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_brand as id_p1, brand as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.brandProduct:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        brandPerso = string.Format("{0}.id_brand", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_brand as id_p1, brand as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.product:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_product as id_p1, product as p1", _dataTable.Prefix);
                        break;

                    //changes to accomodate variété/Announceurs, Variétés/produit, Variétés/Marques 

                    case CstFormat.PreformatedProductDetails.segmentAdvertiser:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_advertiser as id_p2, advertiser as p2 ", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.segmentProduct:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.segmentBrand:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        brandPerso = string.Format("{0}.id_brand", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_brand as id_p2, brand as p2 ", _dataTable.Prefix);
                        break;

                    /*Select clause added for the sector and subsector levels (Finnish version)
                    **/
                    case CstFormat.PreformatedProductDetails.sector:
                        sqlStr = string.Format(" {0}.id_sector as id_p1, sector as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.subSector:
                        sqlStr = string.Format(" {0}.id_subsector as id_p1, subsector as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.sectorAdvertiser:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_sector as id_p1, sector as p1, {0}.id_advertiser as id_p2, advertiser as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.subSectorAdvertiser:
                        annonceurPerso = string.Format("{0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_subsector as id_p1, subsector as p1, {0}.id_advertiser as id_p2, advertiser as p2", _dataTable.Prefix);
                        break;
                    /***************************/

                    default:
                        //throw new ASDynamicTablesDataAccessException("Le format de détail " + _session.PreformatedProductDetail.ToString() + " n'est pas un cas valide.");
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1", _dataTable.Prefix);
                        _session.PreformatedProductDetail = CstFormat.PreformatedProductDetails.group;
                        break;
                }
                if (productIndex < 2)
                {
                    //nomenclature produit en position de parent de la nomenclature media
                    sql.Insert(beginningIndex, sqlStr + (mediaIndex > -1 ? ", " : ""));
                }
                else
                {
                    //nomenclature produit en position d'enfant de la nomenclature media
                    sql.Append(((mediaIndex > -1) ? ", " : " ") + sqlStr);
                }
            }
            #endregion

            #endregion

            #region champs de données quantitatives
            //Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
            //du dernier mois dispo en BDD
            //traitement de la notion de fréquence

            CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
            object[] param = new object[1];
            param[0] = _session;
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName, cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
            string absolutEndPeriod = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

            if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
                throw new NoDataException();

            //Liste des mois de la période sélectionnée
            //Calcul de l'index année dans une table recap (N, N1 ou N2)


            int i = DateTime.Now.Year - int.Parse(_session.PeriodBeginningDate.Substring(0, 4));

            if (DateTime.Now.Year > downLoadDate)
            {
                i = i - 1;
            }

            string year = (i > 0) ? i.ToString() : "";
            string previousYear = (int.Parse(year != "" ? year : "0") + 1).ToString();

            //Détermination du nombre de mois et Instanciation de la liste des champs mois
            int firstMonth = int.Parse(_session.PeriodBeginningDate.Substring(4, 2));
            string[] months = new string[int.Parse(absolutEndPeriod.Substring(4, 2)) -
                firstMonth + 1];
			string[] persoMonths = new string[int.Parse(absolutEndPeriod.Substring(4, 2)) -
			  firstMonth + 1];
            string[] previousYearMonths = (_session.ComparativeStudy) ? new string[months.GetUpperBound(0) + 1] : null;
			string[] persoPreviousYearMonths = (_session.ComparativeStudy) ? new string[months.GetUpperBound(0) + 1] : null;
            //création de la liste des champs période
            bool first = true;
            for (i = 0; i < months.Length; i++)
            {
                months[i] = string.Format("sum(exp_euro_n{0}_{1})", year, (i + firstMonth));
				persoMonths[i] = string.Format("exp_euro_n{0}_{1}", year, (i + firstMonth));
				if (_session.ComparativeStudy) {
					previousYearMonths[i] = string.Format("sum(exp_euro_n{0}_{1})", previousYear, (i + firstMonth));
					persoPreviousYearMonths[i] = string.Format("exp_euro_n{0}_{1}", previousYear, (i + firstMonth));
				}
            }

            //Year
            if (_reportFormat.ToString().IndexOf("_Year") > -1 || _reportFormat == CstFormat.PreformatedTables.productYear_X_Media)
            {

                string comparativeStudyMonths = "";

                for (i = 0; i < months.Length; i++)
                {

                    if (i == months.Length - 1 && months.Length > 1)
                    {
                        sql.AppendFormat("exp_euro_n{0}_{1}) as N{0}", year, (i + firstMonth));
                        comparativeStudyMonths = string.Format ("{0} exp_euro_n{1}_{2}) as N{1}", comparativeStudyMonths, previousYear, (i + firstMonth));
                    }
                    else if (i == 0 && months.Length > 1)
                    {
                        sql.AppendFormat(", sum(exp_euro_n{0}_{1} + ", year, (i + firstMonth));
                        comparativeStudyMonths = string.Format("{0} sum(exp_euro_n{1}_{2} + ", comparativeStudyMonths, previousYear, (i + firstMonth));
                    }
                    else if (months.Length == 1)
                    {
                        sql.AppendFormat(", sum(exp_euro_n{0}_{1}) as N{0}", year, (i + firstMonth));
                        comparativeStudyMonths = string.Format("{0} sum(exp_euro_n{1}_{2}) as N{1}", comparativeStudyMonths, previousYear, (i + firstMonth));
                    }
                    else
                    {
                        sql.AppendFormat("exp_euro_n{0}_{1} + ", year, (i + firstMonth));
                        comparativeStudyMonths = string.Format("{0} exp_euro_n{1}_{2} + ", comparativeStudyMonths, previousYear, (i + firstMonth));
                    }
                }
                if (_session.ComparativeStudy)
                    sql.AppendFormat(", {0}", comparativeStudyMonths);



            }

            //Cumul,
            if (_reportFormat.ToString().IndexOf("_Cumul") > -1)
            {
                int j;
                for (i = 0; i < months.Length; i++)
                {
                    sql.Append(", ");
                    first = true;
                    for (j = 0; j <= i; j++)
                    {
                        if (!first) sql.Append("+");
                        else first = false;
                        sql.Append(months[j]);
                    }
                    sql.AppendFormat(" as N{0}_{1}", year, j);
                    //Year N-1 si désirée et possible
                    if (_session.ComparativeStudy)
                    {
                        sql.Append(", ");
                        first = true;
                        for (j = 0; j <= i; j++)
                        {
                            if (!first) sql.Append("+");
                            else first = false;
                            sql.Append(previousYearMonths[j]);
                        }
                        sql.AppendFormat(" as N{0}_{1}", previousYear, j);
                    }
                }
            }

            //Mensual
            if (_reportFormat.ToString().IndexOf("_Mensual") > -1)
            {
                int cumulIndex = sql.Length;
                first = true;
                string previousYearTotal = "";
                for (i = 0; i < months.Length; i++)
                {
                    sql.AppendFormat(", {0}", months[i]);
                    if (_session.ComparativeStudy) sql.AppendFormat(", {0}", previousYearMonths[i]);
                    if (!first)
                    {
                        sql.Insert(cumulIndex, string.Format("{0}+", months[i]));
                        if (_session.ComparativeStudy) previousYearTotal = String.Format("{0} + {1}", previousYearMonths[i], previousYearTotal);
                    }
                    else
                    {
                        first = false;
                        sql.Insert(cumulIndex, string.Format("{0} as total_N{1}", months[i], year));
						if (_session.ComparativeStudy) previousYearTotal = string.Format("{0} as total_N{1}", previousYearMonths[i], previousYear);
                    }
                }
                sql.Insert(cumulIndex, ", ");
                if (_session.ComparativeStudy) sql.Insert(sql.ToString().IndexOf(" as total_N" + year) + 11 + year.Length, ", " + previousYearTotal);
            }

            //Year+Mensual (ajout des mois)
            if (_reportFormat.ToString().IndexOf("_YearMensual") > -1)
            {
                int cumulIndex = sql.Length;
                first = true;
                for (i = 0; i < months.Length; i++)
                {
                    sql.AppendFormat(", {0} as Mo_{1}", months[i], (i + 1));
                }
            }
            #endregion

            #region Notion de personnalisation des annonceurs?
            if (!string.IsNullOrEmpty(annonceurPerso) || !string.IsNullOrEmpty(brandPerso))
            {
                NomenclatureElementsGroup refElts = null;
                string refString = "", oldRefString = "";
                string[] refLst = null;
                bool hasData = false;

                //items in reference univese
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0))
                {
                    refElts = _session.SecondaryProductUniverses[0].GetGroup(0);
                    //Advertisers references
                    if (refElts != null && refElts.Count() > 0 && refElts.Contains(TNSClassificationLevels.ADVERTISER))
                    {
                        refString = oldRefString = refElts.GetAsString(TNSClassificationLevels.ADVERTISER);
                        refLst = refString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (refLst != null && refLst.Length > 0)
                        {
                            sql.AppendFormat(", sum(decode({0}", annonceurPerso);
                            foreach (string s in refLst)
                            {
                                sql.AppendFormat(",{0},1", s);
                            }
                            sql.Append(", 0)) ");
                            hasData = true;
                        }
                    }
                    //Brands references
                    refString = "";
                    if (!string.IsNullOrEmpty(brandPerso) && refElts != null && refElts.Count() > 0 && refElts.Contains(TNSClassificationLevels.BRAND))
                    {
                        refString = refElts.GetAsString(TNSClassificationLevels.BRAND);
                        refLst = refString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (refLst != null && refLst.Length > 0)
                        {
                            if(hasData) sql.AppendFormat(" + ");
                            else sql.AppendFormat(" , ");
                            sql.AppendFormat(" sum(decode({0}", brandPerso);
                            foreach (string s in refLst)
                            {
                                sql.AppendFormat(",{0},1", s);
                            }
                            sql.Append(", 0)) ");
                            hasData = true;
                        }
                    }
                    if (hasData) sql.Append(" as inref ");
                }
                if (!hasData)
                {
                    sql.Append(", 0 as inref ");
                }
                hasData = false;

                //items in competing univese
                NomenclatureElementsGroup compElts = null;
                string compString = "", oldCompString="";
                string[] compLst = null;
                if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1))
                {
                    compElts = _session.SecondaryProductUniverses[1].GetGroup(0);
                    //Advertisers competing
                    if (compElts != null && compElts.Count() > 0 && compElts.Contains(TNSClassificationLevels.ADVERTISER))
                    {
                        compString = oldCompString =  compElts.GetAsString(TNSClassificationLevels.ADVERTISER);
                        compLst = compString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (compLst != null && compLst.Length > 0)
                        {
                            sql.AppendFormat(", sum(decode({0}", annonceurPerso);
                            foreach (string s in compLst)
                            {
                                sql.AppendFormat(",{0},1", s);
                            }
                            sql.Append(", 0)) ");
                            hasData = true;
                        }
                    }
                    //Brands competing
                    compString = "";
                    if (!string.IsNullOrEmpty(brandPerso) && compElts != null && compElts.Count() > 0 && compElts.Contains(TNSClassificationLevels.BRAND))
                    {
                        compString = compElts.GetAsString(TNSClassificationLevels.BRAND);
                        compLst = compString.Split(new char[1] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                        if (compLst != null && compLst.Length > 0)
                        {
                            if (hasData) sql.AppendFormat(" + ");
                            else sql.AppendFormat(" , ");
                            sql.AppendFormat(" sum(decode({0}", brandPerso);
                            foreach (string s in compLst)
                            {
                                sql.AppendFormat(",{0},1", s);
                            }
                            sql.Append(", 0)) ");
                            hasData = true;
                        }
                    }
                    if (hasData) sql.Append(" as incomp ");
                }
                if (!hasData)
                {
                    sql.Append(", 0 as incomp ");
                }
                if (oldCompString.Length > 0)
                {
                    if (oldRefString.Length > 0)
                    {
                        oldRefString += ",";
                    }
                    oldRefString += oldCompString;
                }
                if (compString.Length > 0)
                {
                    if (refString.Length > 0)
                    {
                        refString += ",";
                    }
                    refString += compString;
                }
                hasData = false;
                //items neutral
                if (!string.IsNullOrEmpty(oldRefString))
                {
                    sql.AppendFormat(", sum(case when {0} not in ({1})  ", annonceurPerso, oldRefString);
                    hasData = true;
                }
                if (!string.IsNullOrEmpty(refString))
                {
                    if( hasData) sql.Append(" and  ");
                    else sql.Append(", sum(case when ");
                    sql.AppendFormat(" {0} not in ({1})   ", brandPerso, refString);
                    hasData = true;
                }
                if (hasData) sql.Append(" then 1 else 0 end) as inneutral ");

                if(!hasData)
                {
                    if (!string.IsNullOrEmpty(annonceurPerso)) sql.AppendFormat(", count(distinct {0}) as inneutral ", annonceurPerso);
                    else if (!string.IsNullOrEmpty(brandPerso)) sql.AppendFormat(", count(distinct {0}) as inneutral ", brandPerso);
                }
            }
            #endregion

            #region Colonne total
            if (_reportFormat == CstFormat.PreformatedTables.productMedia_X_Year ||
                _reportFormat == CstFormat.PreformatedTables.productMedia_X_YearMensual)
                sql.Insert(beginningIndex, " '0' as id_p, 'TOTAL' as p, ");
            if ((_reportFormat == CstFormat.PreformatedTables.mediaProduct_X_Year || _reportFormat == CstFormat.PreformatedTables.mediaProduct_X_YearMensual)
                && (_vehicle == CstDBClassif.Vehicles.names.plurimedia || _vehicle == CstDBClassif.Vehicles.names.PlurimediaWithoutMms))
            {
                sql.Insert(beginningIndex, " '0' as id_m, 'TOTAL' as m, ");
            }
            #endregion

        }
        #endregion

        #region appendFromClause
        /// <summary>
        /// Ajoute la clause from d'un requete sql.
        /// Etapes:
        ///		pour chaque champ qui est susceptible d'être présent dans la requête, on vérifie sa présence 
        ///		effective et on ajoute la table a laquelle il correspond au from
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requete sql</param>
        /// <param name="dataTable">nom de la table de recap à attaquer</param>
        protected virtual void AppendFromClause(StringBuilder sql)
        {

            sql.AppendFormat(" from {0}", _dataTable.SqlWithPrefix);

            #region nomenclature media
            sql.AppendFormat(", {0} ", _recapVehicle.SqlWithPrefix);
            if (sql.ToString().IndexOf("category") > -1)
                sql.AppendFormat(", {0} ", _recapCategory.SqlWithPrefix);
            if (sql.ToString().IndexOf("media") > -1)
                sql.AppendFormat(", {0} ", _recapMedia.SqlWithPrefix);
            #endregion

            #region nomenclature produit
            if (sql.ToString().IndexOf("group_") > -1)
                sql.AppendFormat(", {0} ", _recapGroup.SqlWithPrefix);
            if (sql.ToString().IndexOf("segment") > -1)
                sql.AppendFormat(", {0} ", _recapSegment.SqlWithPrefix);
            if (sql.ToString().IndexOf("brand") > -1)
                sql.AppendFormat(", {0} ", _recapBrand.SqlWithPrefix);
            if (sql.ToString().IndexOf("product") > -1)
                sql.AppendFormat(", {0} ", _recapProduct.SqlWithPrefix);
            if (sql.ToString().IndexOf("advertiser") > -1)
                sql.AppendFormat(", {0} ", _recapAdvertiser.SqlWithPrefix);
            /*Tables added for the sector and subsector levels (Finnish version)
            **/
            if (sql.ToString().IndexOf("sector") > -1)
                sql.AppendFormat(", {0} ", _recapSector.SqlWithPrefix);
            if (sql.ToString().IndexOf("subsector") > -1)
                sql.AppendFormat(", {0} ", _recapSubSector.SqlWithPrefix);
            /******************/
            #endregion

        }
        #endregion

        #region appendJointClause
        /// <summary>
        /// Ajoute a la requête sql les conditions dfe jointures entre la table des recap et les tabmes de 
        /// nomenclature. Pour chaque champ de nomenclmature susceptible d'aparaitre dans la requête, on 
        /// vérifie sa présence effective et si c'est le cas, on effectue la jointure avec la table de recap
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requête sql</param>
        protected virtual void AppendJointClause(StringBuilder sql)
        {

            string linkWord = " where ";

            #region nomenclature produit
            if (sql.ToString().IndexOf(_recapGroup.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_group_ = {2}.id_group_", linkWord, _recapGroup.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapSegment.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_segment = {2}.id_segment", linkWord, _recapSegment.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }

            /* Joint clause for the sector and subsector levels added for the Finnish version
            **/
            if (sql.ToString().IndexOf(_recapSector.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_sector = {2}.id_sector", linkWord, _recapSector.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapSubSector.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_subsector = {2}.id_subsector", linkWord, _recapSubSector.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            /*********************/

            if (sql.ToString().IndexOf(_recapBrand.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_brand = {2}.id_brand", linkWord, _recapBrand.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapProduct.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_product = {2}.id_product", linkWord, _recapProduct.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapAdvertiser.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_advertiser = {2}.id_advertiser", linkWord, _recapAdvertiser.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            #endregion

            #region nomenclature media
            if (sql.ToString().IndexOf(_recapVehicle.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_vehicle = {2}.id_vehicle", linkWord, _recapVehicle.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapCategory.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_category = {2}.id_category", linkWord, _recapCategory.Prefix, _dataTable.Prefix);
                linkWord = " and ";
            }
            if (sql.ToString().IndexOf(_recapMedia.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_media = {2}.id_media", linkWord, _recapMedia.Prefix, _dataTable.Prefix);
            }
            #endregion


        }
        #endregion

        #region appendSelectionClause
        /// <summary>
        /// Ajout de la sélection produit et support du client:
        ///		Pour chaque niveau de la nomenclature produit (resp support), on teste sa présence dans l'arbre 
        ///		SlectionUniversProduct (resp SelectionUniversMedia). SI le niveau est prtésent dans l'arbre,
        ///		on le prend en compte dans la requête
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requête sql</param>
        protected virtual void AppendSelectionClause(StringBuilder sql)
        {

            bool first = true;
			string idSponsorShipCategory = "";
			StringBuilder persoSql;

            #region Sélection media
            //vehicle
            string list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);

			sql.Append(FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, _dataTable.Prefix, true));
            var vehicleIds = new List<long>();
            if (list.Length > 0) vehicleIds = new List<string>(list.Trim().Split(',')).ConvertAll(Convert.ToInt64);

            //on ne teste pas le vehicle si on est en pluri
            var plurimediaDbIds = new List<long> {VehiclesInformation.Get(CstDBClassif.Vehicles.names.plurimedia).DatabaseId};
            long? plurimediaWithoutMmsId = null;
            if (VehiclesInformation.Contains(CstDBClassif.Vehicles.names.PlurimediaWithoutMms))
            {
                plurimediaWithoutMmsId = VehiclesInformation.Get(CstDBClassif.Vehicles.names.PlurimediaWithoutMms).DatabaseId;               
                plurimediaDbIds.Add(VehiclesInformation.Get(CstDBClassif.Vehicles.names.PlurimediaWithoutMms).DatabaseId);
            }
            if (vehicleIds.Any() && vehicleIds.All(p => !plurimediaDbIds.Contains(p)) )
            {
                first = false;
                sql.AppendFormat(" and ( {0}.id_vehicle in ({1})", _dataTable.Prefix, list);
            }
			
            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
            if (vehicleIds.All(p => !plurimediaDbIds.Contains(p)))
            {
                sql.Append(FctUtilities.SQLGenerator.getAccessVehicleList(_session, _dataTable.Prefix, true));
            }
            //category
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.categoryAccess);
            if (list.Length > 0)
            {
                if (first)
                {
                    sql.Append(" and (");
                    first = false;
                }
                else sql.Append(" or");
                sql.AppendFormat(" {0}.id_category in ({1})", _dataTable.Prefix, list);
            }
            //media !!!!!!! necessaire en fonction de la table attaquée?
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.mediaAccess);
            if (list.Length > 0 && (sql.ToString().IndexOf(_dataRadio.Label) > -1 || sql.ToString().IndexOf(_dataTV.Label) > -1 
                || sql.ToString().IndexOf(_dataOutdoor.Label) > -1 || sql.ToString().IndexOf(_dataTactic.Label) > -1))
            {
                if (first)
                {
                    sql.Append(" and (");
                    first = false;
                }
                else sql.Append(" or");
                sql.AppendFormat(" {0}.id_media in ({1}) ", _dataTable.Prefix, list);
            }
            if (!first) sql.Append(")");

            //Droits Parrainage TV
            if (!_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_SPONSORSHIP_TV_ACCESS_FLAG))
            {
				idSponsorShipCategory = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.productClassAnalysisSponsorShipTv);
				if (!string.IsNullOrEmpty(idSponsorShipCategory)) {
					sql.AppendFormat("  and  {0}.id_category not in ( {1}) ", _dataTable.Prefix, idSponsorShipCategory);
				}		      
            }

            //Exclude Internet Display when selection Plurimedia Without Mms
            if (plurimediaWithoutMmsId.HasValue
               && vehicleIds.Any(p => p == plurimediaWithoutMmsId.Value))
            {
                sql.AppendFormat("  and  {0}.id_vehicle not in ( {1},{2}) "
                    , _dataTable.Prefix, VehiclesInformation.Get(CstDBClassif.Vehicles.names.mms).DatabaseId
                    , VehiclesInformation.Get(CstDBClassif.Vehicles.names.internet).DatabaseId);

            }
            #endregion

            #region Sélection produits

            // Sélection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append("  " + _session.PrincipalProductUniverses[0].GetSqlConditions(_dataTable.Prefix, true));

            #endregion

			#region Notion de personnalisation des annonceurs
			
			#region detail produit personnalisable
			int productIndex = _reportFormat.ToString().IndexOf("roduct");
			bool withProductToPersonnalize = false;
			if (productIndex > -1) {
				string sqlStr = "";
				//nomenclature produit présente dans le tableau préformaté
				switch (_session.PreformatedProductDetail) {					
					case CstFormat.PreformatedProductDetails.groupBrand:					
					case CstFormat.PreformatedProductDetails.groupProduct:						
					case CstFormat.PreformatedProductDetails.groupAdvertiser:						
					case CstFormat.PreformatedProductDetails.advertiser:						
					case CstFormat.PreformatedProductDetails.advertiserBrand:						
					case CstFormat.PreformatedProductDetails.advertiserProduct:						
					case CstFormat.PreformatedProductDetails.brand:
                    case CstFormat.PreformatedProductDetails.brandProduct:
					case CstFormat.PreformatedProductDetails.product:					
					case CstFormat.PreformatedProductDetails.segmentAdvertiser:						
					case CstFormat.PreformatedProductDetails.segmentProduct:						
					case CstFormat.PreformatedProductDetails.segmentBrand:
                    /* WARNING !!! the two following tests are added temporarily in order to add specific levels for the Finnish version
                    **/
                    case CstFormat.PreformatedProductDetails.sectorAdvertiser:
                    case CstFormat.PreformatedProductDetails.subSectorAdvertiser:
						withProductToPersonnalize = true;
						break;
					default:
						withProductToPersonnalize = false;
						break;
				}				
			}
			#endregion

			if (withProductToPersonnalize && _session.SecondaryProductUniverses.Count > 0
                && (_session.SecondaryProductUniverses.ContainsKey(0) || _session.SecondaryProductUniverses.ContainsKey(1))) {
				int downLoadDate = _session.DownLoadDate;
				//Détermination du dernier mois accessible en fonction de la fréquence de livraison du client et
				//du dernier mois dispo en BDD
				//traitement de la notion de fréquence

                CoreLayer cl = WebApplicationParameters.CoreLayers[TNS.AdExpress.Constantes.Web.Layers.Id.dateDAL];
                object[] param = new object[1];
                param[0] = _session;
                var dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(string.Format("{0}Bin\\{1}"
                    , AppDomain.CurrentDomain.BaseDirectory, cl.AssemblyName), cl.Class, false
                    , BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
                string absolutEndPeriod = dateDAL.CheckPeriodValidity(_session, _session.PeriodEndDate);

				if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
					throw new NoDataException();

				//Liste des mois de la période sélectionnée
				//Calcul de l'index année dans une table recap (N, N1 ou N2)


				int i = DateTime.Now.Year - int.Parse(_session.PeriodBeginningDate.Substring(0, 4));

				if (DateTime.Now.Year > downLoadDate) {
					i = i - 1;
				}

				string year = (i > 0) ? i.ToString() : "";
				string previousYear = (int.Parse(year != "" ? year : "0") + 1).ToString();

				//Détermination du nombre de mois et Instanciation de la liste des champs mois
				int firstMonth = int.Parse(_session.PeriodBeginningDate.Substring(4, 2));
				string[] months = new string[int.Parse(absolutEndPeriod.Substring(4, 2)) -
					firstMonth + 1];
				string[] previousYearMonths = (_session.ComparativeStudy) ? new string[months.GetUpperBound(0) + 1] : null;
				//création de la liste des condition de la période				
				persoSql = new StringBuilder();
				for (i = 0; i < months.Length; i++) {
					if (i == 0) persoSql.Append(" and ( ");
					else persoSql.Append(" or ");
					persoSql.AppendFormat(" exp_euro_n{0}_{1} > 0 ", year, (i + firstMonth));					
					if (_session.ComparativeStudy) {
						persoSql.AppendFormat(" or exp_euro_n{0}_{1} > 0 ", previousYear, (i + firstMonth));
					}
					if (i == months.Length - 1) persoSql.Append(" )");
				}
				sql.Append(persoSql.ToString());
			}
			#endregion

        }
        #endregion

        #region appendRightClause
        /// <summary>
        /// Ajoute les droits clients à la requête:
        ///		droits produits
        ///		droits media, initialement, on ne l'ai as pas pris en compte mais ils doivent l'être au niveau
        ///		vehicle uniquement. CAD que des qu'un utilisateur a droit à un support d'un vehicle, il a droit 
        ///		à tous les supports. Les droits media doivent encore être ajoutés.
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requête sql</param>
        /// <remarks>
        /// Utilise les méthodes:
        ///		TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(_session, TablePrefixe, bool)
        /// </remarks>
        protected virtual void AppendRightClause(StringBuilder sql)
        {

            TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
            sql.Append(" " + FctUtilities.SQLGenerator.GetClassificationCustomerProductRight(_session, _dataTable.Prefix,  true, module.ProductRightBranches)); 
            //!!!!!!!!!!!!!!!! Pas de gestion des droits de la nomenclature media dans les recap (src : G Facon le 27/09/2004)

			sql.Append(" " + FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, _dataTable.Prefix));
        }
        #endregion

        #region appendActivationLanguageClause
        /// <summary>
        /// Ajoute la notion d'activation et de langage à la requête.
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requête sql</param>
        /// <param name="dataTable">Nom de la table des recap</param>
        protected virtual void AppendActivationLanguageClause(StringBuilder sql)
        {

            //table de recap
            //			sql.Append(" and " + _dataTable.Prefix + ".id_language_i = " + _session.SiteLanguage);

            #region nomenclature produit
            if (sql.ToString().IndexOf(_recapGroup.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapGroup.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapGroup.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapSegment.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapSegment.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapSegment.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapBrand.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapBrand.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapBrand.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapProduct.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapProduct.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapProduct.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapAdvertiser.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapAdvertiser.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapAdvertiser.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }

            /*Added for the sector and subsector levels (Finnish version)
            * */
            if (sql.ToString().IndexOf(_recapSector.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapSector.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapSector.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapSubSector.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapSubSector.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapSubSector.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            /***********************************/
            #endregion

            #region nomenclature media
            if (sql.ToString().IndexOf(_recapVehicle.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapVehicle.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapVehicle.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapCategory.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapCategory.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapCategory.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapMedia.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapMedia.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapMedia.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            #endregion

        }
        #endregion

        #region appendRegroupmentAndOrderClause
        /// <summary>
        /// Ajoute les clause order by et group by a la requête
        /// </summary>
        /// <param name="_session">Session Utilisateur</param>
        /// <param name="sql">sql</param>
        protected virtual void AppendRegroupmentAndOrderClause(StringBuilder sql)
        {

            #region Group
            string groupClause = sql.ToString().Remove(sql.ToString().IndexOf("sum("), sql.Length - sql.ToString().IndexOf("sum("))
                .Remove(0, 6)
                .Replace("as id_m1", "")
                .Replace("as m1", "")
                .Replace("as id_m2", "")
                .Replace("as m2", "")
                .Replace("as id_m3", "")
                .Replace("as m3", "")
                .Replace("as id_m", "")
                .Replace("as m", "")
                .Replace("as id_p1", "")
                .Replace("as p1", "")
                .Replace("as id_p2", "")
                .Replace("as p2", "")
                .Replace("as id_p", "")
                .Replace("as p", "");
            groupClause = groupClause.Remove(groupClause.LastIndexOf(','), groupClause.Length - groupClause.LastIndexOf(','));

            sql.Append(" group by " + groupClause);
            #endregion

            #region Order
            switch (_reportFormat)
            {
                case CstFormat.PreformatedTables.media_X_Year:
                case CstFormat.PreformatedTables.mediaYear_X_Cumul:
                case CstFormat.PreformatedTables.mediaYear_X_Mensual:
                    sql.Append(" order by m1, id_m1");
                    if (sql.ToString().IndexOf("id_m2") > -1) sql.Append(", m2, id_m2");
                    if (sql.ToString().IndexOf("id_m3") > -1) sql.Append(", m3, id_m3");
                    break;
                case CstFormat.PreformatedTables.mediaProduct_X_Year:
                case CstFormat.PreformatedTables.mediaProduct_X_YearMensual:
                    if (_vehicle != CstDBClassif.Vehicles.names.plurimedia && _vehicle != CstDBClassif.Vehicles.names.PlurimediaWithoutMms)
                        sql.Append(" order by m1, id_m1");
                    else
                        sql.Append(" order by m,id_m, m1, id_m1");
                    if (sql.ToString().IndexOf("id_m2") > -1) sql.Append(", m2, id_m2");
                    if (sql.ToString().IndexOf("id_m3") > -1) sql.Append(", m3, id_m3");
                    sql.Append(", id_p1");
                    if (sql.ToString().IndexOf("id_p2") > -1) sql.Append(", p2, id_p2");
                    break;
                case CstFormat.PreformatedTables.productMedia_X_Year:
                case CstFormat.PreformatedTables.productMedia_X_YearMensual:
                    sql.Append(" order by p, id_p, p1, id_p1");
                    if (sql.ToString().IndexOf("id_p2") > -1) sql.Append(", p2, id_p2");
                    sql.Append(", m1, id_m1");
                    if (sql.ToString().IndexOf("id_m2") > -1) sql.Append(", m2, id_m2");
                    if (sql.ToString().IndexOf("id_m3") > -1) sql.Append(", m3, id_m3");
                    break;
                case CstFormat.PreformatedTables.productYear_X_Media:
                case CstFormat.PreformatedTables.product_X_Year:
                case CstFormat.PreformatedTables.productYear_X_Cumul:
                case CstFormat.PreformatedTables.productYear_X_Mensual:
                    sql.Append(" order by p1, id_p1");
                    if (sql.ToString().IndexOf("id_p2") > -1) sql.Append(", p2, id_p2");
                    break;
                default:
                    _reportFormat = CstFormat.PreformatedTables.media_X_Year;
                    sql.Append(" order by m1, id_m1");
                    if (sql.ToString().IndexOf("id_m2") > -1) sql.Append(", m2, id_m2");
                    if (sql.ToString().IndexOf("id_m3") > -1) sql.Append(", m3, id_m3");
                    break;
            }
            #endregion

        }
        #endregion


       

        #endregion

    }
}

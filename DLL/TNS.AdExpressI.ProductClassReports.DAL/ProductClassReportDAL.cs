#region Information
/*
 * Author : G Ragneau
 * Created on : 17/07/2008
 * Modification:
 *      Author - Date - Description
 * 
 * Origine:
 *      Auteur: G. Ragneau
 *      Date de cr�ation: 22/09/2004
 *      Date de modification: 27/09/2004
 *      18/02/2005	A.Obermeyer		rajout Marque en personnalisation
 *      23/08/2005	G. Facon		Solution temporaire pour les IDataSource
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
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

        #region Tables
        Table _recapVehicle = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapVehicle);
        Table _recapCategory = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCategory);
        Table _recapMedia = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMedia);
        Table _recapGroup = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapGroup);
        Table _recapSegment = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSegment);
        Table _recapBrand = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapBrand);
        Table _recapProduct = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapProduct);
        Table _recapAdvertiser = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapAdvertiser);
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
        protected DataSet GetDataSet()
        {
            StringBuilder sql = new StringBuilder(2000);

            #region Construction de la requ�te
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

            #region Execution de la requ�te
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
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
        #endregion

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!droits media
        #region M�thodes Priv�es

        #region getVehicleTableName
        /// <summary>
        /// M�thode priv�e qui d�tecte la table de recap � utiliser en fonction de la s�lection m�dia, produit
        /// et du niveau de d�tail choisi
        ///		d�tection d'une �tude monom�dia ou pluri m�dia ==> recap_tv ... ou recap_pluri
        ///		niveau de d�tail de la nomenclature produit ==> recap ou recap_segment
        /// </summary>
        /// <exception cref="TNS.AdExpress.Web.Exceptions.DynamicTablesDataAccessException">
        /// Lanc�e si aucune table de la base de doon�es ne correspond au vehicle sp�cifi� dans la session utilisateur.
        /// </exception>
        /// <param name="_session">Session utilisateur</param>
        /// <returns>Cha�ne de caract�re correspondant au nom de la table � attaquer</returns>
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
                && detail != CstFormat.PreformatedProductDetails.segmentAdvertiser
                && detail != CstFormat.PreformatedProductDetails.segmentBrand
                && detail != CstFormat.PreformatedProductDetails.segmentProduct
                && detail != CstFormat.PreformatedProductDetails.product;

            #region D�tection du type de table vehicle (pluri ou mono?)
            switch (_vehicle)
            {
                case CstDBClassif.Vehicles.names.cinema:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinema):WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapCinemaSegment);
                case CstDBClassif.Vehicles.names.internet:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternet) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInternetSegment);
                case CstDBClassif.Vehicles.names.outdoor:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoor) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapOutDoorSegment);
                case CstDBClassif.Vehicles.names.radio:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadio) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapRadioSegment);
                case CstDBClassif.Vehicles.names.tv:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTv) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTvSegment);
                case CstDBClassif.Vehicles.names.press:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPress) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPressSegment);
                case CstDBClassif.Vehicles.names.plurimedia:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluri) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapPluriSegment);
                case CstDBClassif.Vehicles.names.mediasTactics:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTactic) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapTacticSegment);
                case CstDBClassif.Vehicles.names.mobileTelephony:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMobileTel) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapMobileTelSegment);
                case CstDBClassif.Vehicles.names.emailing:
                    return (!segment) ? WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapEmailing) : WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapEmailingSegment);
                case CstDBClassif.Vehicles.names.internationalPress:
                case CstDBClassif.Vehicles.names.others:
                case CstDBClassif.Vehicles.names.adnettrack:
                default:
                    throw new ProductClassReportsDALException("Vehicle n� " + _vehicle.GetHashCode() + " is not allowed.");
            }
            #endregion

            return null;
        }
        #endregion

        #region appendSelectClause
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>!!!!!La classe m�tier qui s'appuie sur cette classe BDD utilise l'ordre des champs ds le select
        ///	Il n'est pas recommand� de changer l'ordre des champs media, produit...
        ///	�tape:
        ///		- Sauvegardede l'index du dernier caract�re de la requ�te a son arriv� afin de pouvoir ins�rer un pmrceau de requ�te si n�cessaire
        ///		- Traitement des champs qualitatifs:
        ///			- Si le tableau preformat� affiche des donn�es de la nomenclature media:
        ///				suivant le niveau de d�tail demand�, on ajoute les champs � la requete en normalisant leurs noms
        ///				chaque nom repr�sente la nomenclature media (id_m) et le niveau consid�r�(id_m1, id_m2...)
        ///				Au cas ou le niveau de d�tail n'est pas repertori�, on applique par d�faut le niveau vehicle uniquement et on update la session
        ///			- Si le tableau preformat� affiche des donn�es de la nomenclature produit:
        ///				suivant le d�tail demand�, on ajoute les champs � la requete en normalisant leurs noms
        ///				chaque nom repr�sente la nomenclature produit (id_p) et le niveau consid�r�(id_p1, id_p2)
        ///				Au cas ou le niveau de d�tail n'est pas repertori�, on applique par d�faut le d�tail group uniquement et on update la session
        ///				Pour les niveaux faisant intervenir la nomenclature annonceur, on sauvegarde dans une strig un morceau de requete ramenant l'id_advertiser
        ///			- si la nompenclature produit est la nomenclature principale, on l'insere devant les champs 
        ///			de la nomenclature support qui devient par consequent la omenclature secondaire.
        ///		- Traitement des champs quantitatifs:
        ///			- ON commence par valider la p�riode consid�r�es en fonction de la fr�quence de livraison des donn�es
        ///			Si jamais la p�riode n'est pas valide (donn�es non accessiblkes par raport a la frequence), on lance une exception NoData
        ///			- construction de la liste des mois month a attaquer. POur chaquer mois, on fera la somme des lignes a ramener
        ///			- si le tableau n�cessite des ann�es, on utilise le champ total_year
        ///			- si le tableau est un mensuel cimul�, on construit la requete en faisant la somme des mois precedent sur la p�riode pour chaque mois a rapatrier
        ///			exemple: periode Mars ==> mai, on prend mars et mars+avril et mars+avril+mai...
        ///			- si le tableau est un mensuel + total, on ramene tous les mois de la liste month plus un champ qui est la somme de tous les mois
        ///		-Traitement de la notion d'annonceur referent ou concurrent:
        ///			si l'univers d'annonceurs concurrents ou l'univers d'annonceurs de reference ne sont pas vides (non exclusif), on concatene a la requete le champ id_advertiser
        ///		- La colonne total est une facilit� de programmation pour la construction des tableaux de type 5.
        ///			en effet, en consid�rant un champ 'total' bidon, on inclu dans la requete un niveau de 
        ///			nomenclature supl�mentaire qui sera traite de la meme maniere que la nomenclature principale
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
            string annonceurPerso = "";

            #region Champs nomenclature media
            if (mediaIndex > -1)
            {
                //nomenclature media pr�sente dans le tableau pr�format�
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
                //nomenclature produit pr�sente dans le tableau pr�format�
                switch (_session.PreformatedProductDetail)
                {
                    case CstFormat.PreformatedProductDetails.group:
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupSegment:
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_segment as id_p2, segment as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupBrand:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_brand as id_p2, brand as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupProduct:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.groupAdvertiser:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_group_ as id_p1, group_ as p1, {0}.id_advertiser as id_p2, advertiser as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiser:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiserBrand:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1, {0}.id_brand as id_p2, brand as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.advertiserProduct:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_advertiser as id_p1, advertiser as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.brand:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_brand as id_p1, brand as p1", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.product:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_product as id_p1, product as p1", _dataTable.Prefix);
                        break;

                    //changes to accomodate vari�t�/Announceurs, Vari�t�s/produit, Vari�t�s/Marques 

                    case CstFormat.PreformatedProductDetails.segmentAdvertiser:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_advertiser as id_p2, advertiser as p2 ", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.segmentProduct:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_product as id_p2, product as p2", _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedProductDetails.segmentBrand:
                        annonceurPerso = string.Format(", {0}.id_advertiser", _dataTable.Prefix);
                        sqlStr = string.Format(" {0}.id_segment as id_p1, segment as p1, {0}.id_brand as id_p2, brand as p2 ", _dataTable.Prefix);
                        break;


                    default:
                        //throw new ASDynamicTablesDataAccessException("Le format de d�tail " + _session.PreformatedProductDetail.ToString() + " n'est pas un cas valide.");
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

            #region champs de donn�es quantitatives
            //D�termination du dernier mois accessible en fonction de la fr�quence de livraison du client et
            //du dernier mois dispo en BDD
            //traitement de la notion de fr�quence
            string absolutEndPeriod = FctUtilities.Dates.CheckPeriodValidity(_session, _session.PeriodEndDate);
            if (int.Parse(absolutEndPeriod) < int.Parse(_session.PeriodBeginningDate))
                throw new NoDataException();

            //Liste des mois de la p�riode s�lectionn�e
            //Calcul de l'index ann�e dans une table recap (N, N1 ou N2)


            int i = DateTime.Now.Year - int.Parse(_session.PeriodBeginningDate.Substring(0, 4));

            if (DateTime.Now.Year > downLoadDate)
            {
                i = i - 1;
            }

            string year = (i > 0) ? i.ToString() : "";
            string previousYear = (int.Parse(year != "" ? year : "0") + 1).ToString();

            //D�termination du nombre de mois et Instanciation de la liste des champs mois
            int firstMonth = int.Parse(_session.PeriodBeginningDate.Substring(4, 2));
            string[] months = new string[int.Parse(absolutEndPeriod.Substring(4, 2)) -
                firstMonth + 1];
            string[] previousYearMonths = (_session.ComparativeStudy) ? new string[months.GetUpperBound(0) + 1] : null;
            //cr�ation de la liste des champs p�riode
            bool first = true;
            for (i = 0; i < months.Length; i++)
            {
                months[i] = string.Format("sum(exp_euro_n{0}_{1})", year, (i + firstMonth));
                if (_session.ComparativeStudy)
                    previousYearMonths[i] = string.Format("sum(exp_euro_n{0}_{1})", previousYear, (i + firstMonth));
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
                    //Year N-1 si d�sir�e et possible
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
                        if (_session.ComparativeStudy) previousYearTotal = string.Format("{0} as total_N", previousYearMonths[i], previousYear);
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
            NomenclatureElementsGroup nomenclatureElementsGroup = null;
            string tempString = "";
            if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(0))
            {
                nomenclatureElementsGroup = _session.SecondaryProductUniverses[0].GetGroup(0);
                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0)
                {
                    if (nomenclatureElementsGroup.Contains(TNSClassificationLevels.ADVERTISER)) tempString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
                }
            }
            else if (_session.SecondaryProductUniverses.Count > 0 && _session.SecondaryProductUniverses.ContainsKey(1))
            {
                nomenclatureElementsGroup = _session.SecondaryProductUniverses[1].GetGroup(0);
                if (nomenclatureElementsGroup != null && nomenclatureElementsGroup.Count() > 0)
                {
                    if (nomenclatureElementsGroup.Contains(TNSClassificationLevels.ADVERTISER)) tempString = nomenclatureElementsGroup.GetAsString(TNSClassificationLevels.ADVERTISER);
                }
            }
            if (tempString != null && tempString.Split(',').Length > 0)
                sql.Append(annonceurPerso);
            #endregion

            #region Colonne total
            if (_reportFormat == CstFormat.PreformatedTables.productMedia_X_Year ||
                _reportFormat == CstFormat.PreformatedTables.productMedia_X_YearMensual)
                sql.Insert(beginningIndex, " '0' as id_p, 'TOTAL' as p, ");
            if ((_reportFormat == CstFormat.PreformatedTables.mediaProduct_X_Year || _reportFormat == CstFormat.PreformatedTables.mediaProduct_X_YearMensual)
                && _vehicle == CstDBClassif.Vehicles.names.plurimedia)
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
        ///		pour chaque champ qui est susceptible d'�tre pr�sent dans la requ�te, on v�rifie sa pr�sence 
        ///		effective et on ajoute la table a laquelle il correspond au from
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requete sql</param>
        /// <param name="dataTable">nom de la table de recap � attaquer</param>
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
            #endregion

        }
        #endregion

        #region appendJointClause
        /// <summary>
        /// Ajoute a la requ�te sql les conditions dfe jointures entre la table des recap et les tabmes de 
        /// nomenclature. Pour chaque champ de nomenclmature susceptible d'aparaitre dans la requ�te, on 
        /// v�rifie sa pr�sence effective et si c'est le cas, on effectue la jointure avec la table de recap
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requ�te sql</param>
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
        /// Ajout de la s�lection produit et support du client:
        ///		Pour chaque niveau de la nomenclature produit (resp support), on teste sa pr�sence dans l'arbre 
        ///		SlectionUniversProduct (resp SelectionUniversMedia). SI le niveau est prt�sent dans l'arbre,
        ///		on le prend en compte dans la requ�te
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requ�te sql</param>
        protected virtual void AppendSelectionClause(StringBuilder sql)
        {

            bool first = true;

            #region S�lection media
            //vehicle
            string list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);


            //on ne teste pas le vehicle si on est en pluri
            if (list.Length > 0 && list.IndexOf(VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.plurimedia).ToString()) < 0)
            {
                first = false;
                sql.AppendFormat(" and ( {0}.id_vehicle in ({1})", _dataTable.Prefix, list);
            }
            sql.Append(FctUtilities.SQLGenerator.getAdExpressUniverseCondition(_session, CstWeb.AdExpressUniverse.RECAP_MEDIA_LIST_ID, _dataTable.Prefix, true));
            // V�rifie s'il � toujours les droits pour acc�der aux donn�es de ce Vehicle
            if (list.IndexOf(VehiclesInformation.EnumToDatabaseId(CstDBClassif.Vehicles.names.plurimedia).ToString()) < 0)
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
            //media !!!!!!! necessaire en fonction de la table attaqu�e?
            list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.mediaAccess);
            if (list.Length > 0 && (sql.ToString().IndexOf(_dataRadio.Label) > -1 || sql.ToString().IndexOf(_dataTV.Label) > -1 || sql.ToString().IndexOf(_dataOutdoor.Label) > -1 || sql.ToString().IndexOf(_dataTactic.Label) > -1))
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
                sql.AppendFormat(" and {0}.id_category not in (68) ", _dataTable.Prefix);
            }

            #endregion

            #region S�lection produits

            // S�lection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append("  " + _session.PrincipalProductUniverses[0].GetSqlConditions(_dataTable.Prefix, true));

            #endregion

        }
        #endregion

        #region appendRightClause
        /// <summary>
        /// Ajoute les droits clients � la requ�te:
        ///		droits produits
        ///		droits media, initialement, on ne l'ai as pas pris en compte mais ils doivent l'�tre au niveau
        ///		vehicle uniquement. CAD que des qu'un utilisateur a droit � un support d'un vehicle, il a droit 
        ///		� tous les supports. Les droits media doivent encore �tre ajout�s.
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requ�te sql</param>
        /// <remarks>
        /// Utilise les m�thodes:
        ///		TNS.AdExpress.Web.Functions.SQLGenerator.getAnalyseCustomerProductRight(_session, TablePrefixe, bool)
        /// </remarks>
        protected virtual void AppendRightClause(StringBuilder sql)
        {

            sql.Append(" " + FctUtilities.SQLGenerator.getAnalyseCustomerProductRight(_session, _dataTable.Prefix, true));
            //!!!!!!!!!!!!!!!! Pas de gestion des droits de la nomenclature media dans les recap (src : G Facon le 27/09/2004)

			sql.Append(" " + FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, _dataTable.Prefix));
        }
        #endregion

        #region appendActivationLanguageClause
        /// <summary>
        /// Ajoute la notion d'activation et de langage � la requ�te.
        /// </summary>
        /// <param name="_session">Session utilisateur</param>
        /// <param name="sql">Requ�te sql</param>
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
        /// Ajoute les clause order by et group by a la requ�te
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

            if (sql.ToString().Substring(0, sql.ToString().IndexOf("from")).IndexOf(string.Format("{0}.{1}",_dataTable.Prefix, "id_advertiser")) > 0)
                groupClause += string.Format(", {0}.id_advertiser ", _dataTable.Prefix);


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
                    if (_vehicle != CstDBClassif.Vehicles.names.plurimedia)
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

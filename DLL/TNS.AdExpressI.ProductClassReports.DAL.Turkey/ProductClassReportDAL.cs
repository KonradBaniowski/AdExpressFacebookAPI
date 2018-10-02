using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Exceptions;
using TNS.AdExpress.Domain.Layers;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Date.DAL;
using Table = TNS.AdExpress.Domain.DataBaseDescription.Table;
using FctUtilities = TNS.AdExpress.Web.Core.Utilities;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstFormat = TNS.AdExpress.Constantes.Web.CustomerSessions.PreformatedDetails;
using CstRight = TNS.AdExpress.Constantes.Customer.Right;
using CstDB = TNS.AdExpress.Constantes.DB;

namespace TNS.AdExpressI.ProductClassReports.DAL.Turkey
{
    public class ProductClassReportDAL : DAL.ProductClassReportDAL
    {

        protected Table _recapInterestCenter = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.interestCenter);
        protected Table _recapSpotSubType = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSpotSubType);
        protected Table _recapSpotType = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapSpotType);
        protected Table _recapAdSlogan = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapAdSlogan);

        protected string expenditurePrefix = "exp_";

        #region Constructor

        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session) : base(session)
        {
            if (session.Unit == CustomerSessions.Unit.spot || session.Unit == CustomerSessions.Unit.duration)
                expenditurePrefix = string.Empty;
        }
        #endregion


        #region AppendSelectClause
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
        protected override void AppendSelectClause(StringBuilder sql)
        {

            sql.Append("select");

          
            int downLoadDate = _session.DownLoadDate;

            #region champs descriptifs
            int mediaIndex = _reportFormat.ToString().IndexOf("edia");
            int productIndex = _reportFormat.ToString().IndexOf("roduct");
            int beginningIndex = sql.Length;
            string annonceurPerso = "", brandPerso = "";

            AppendMediaFields(sql, mediaIndex);

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
            IDateDAL dateDAL = (IDateDAL)AppDomain.CurrentDomain.CreateInstanceFromAndUnwrap(AppDomain.CurrentDomain.BaseDirectory + @"Bin\" + cl.AssemblyName,
                cl.Class, false, BindingFlags.CreateInstance | BindingFlags.Instance | BindingFlags.Public, null, param, null, null);
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
            GetMonthsFileds(sql, months, year, firstMonth, persoMonths, previousYearMonths, previousYear, persoPreviousYearMonths);
            bool first = true;
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
                if (_session.ComparativeStudy && previousYearMonths != null)
                {
                    for (i = 0; i < previousYearMonths.Length; i++)
                    {
                        sql.AppendFormat(", {0} as Mo1_{1}", previousYearMonths[i], (i + 1));
                    }
                }
            }
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
        protected override Table GetVehicleTableName(bool productRequired)
        {
                switch (_vehicle)
                {
                    case AdExpress.Constantes.Classification.DB.Vehicles.names.tv:
                        return WebApplicationParameters.GetDataTable(TableIds.recapTv, _session.IsSelectRetailerDisplay);
                    default:
                        throw new Exceptions.ProductClassReportsDALException(
                            $"Vehicle n° {_vehicle.GetHashCode()} is not allowed.");
                }
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
        protected override void AppendFromClause(StringBuilder sql)
        {

            sql.AppendFormat(" from {0}{1} {2}", _dataTable.Sql,_session.Unit.ToString(),_dataTable.Prefix);
          

            #region nomenclature media
            sql.AppendFormat(", {0} ", _recapVehicle.SqlWithPrefix);
            if (sql.ToString().IndexOf("category") > -1)
                sql.AppendFormat(", {0} ", _recapCategory.SqlWithPrefix);
            if (sql.ToString().IndexOf("media") > -1)
                sql.AppendFormat(", {0} ", _recapMedia.SqlWithPrefix);
            if (sql.ToString().IndexOf("INTEREST_CENTER") > -1)
                sql.AppendFormat(", {0} ", _recapInterestCenter.SqlWithPrefix);
            if (sql.ToString().IndexOf("SPOT_SUB_TYPE") > -1)
                sql.AppendFormat(", {0} ", _recapSpotSubType.SqlWithPrefix);
            if (sql.ToString().IndexOf("SPOT_TYPE") > -1)
                sql.AppendFormat(", {0} ", _recapSpotType.SqlWithPrefix);





        
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

            if (sql.ToString().IndexOf("AD_SLOGAN") > -1)
                sql.AppendFormat(", {0} ", _recapAdSlogan.SqlWithPrefix);
          
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
        protected override void AppendSelectionClause(StringBuilder sql)
        {

            bool first = true;
           
          

            #region Sélection media
            //vehicle
            string list = _session.GetSelection(_session.SelectionUniversMedia, CstRight.type.vehicleAccess);

            sql.Append(FctUtilities.SQLGenerator.GetResultMediaUniverse(_session, _dataTable.Prefix, true));
            var vehicleIds = new List<long>();
            if (list.Length > 0) vehicleIds = new List<string>(list.Trim().Split(',')).ConvertAll(Convert.ToInt64);

            //on ne teste pas le vehicle si on est en pluri
            //var plurimediaDbIds = new List<long> { VehiclesInformation.Get(CstDBClassif.Vehicles.names.plurimedia).DatabaseId };
            

            //if (vehicleIds.Any() && vehicleIds.All(p => !plurimediaDbIds.Contains(p)))
                if (vehicleIds.Any())
                {
                first = false;
                sql.AppendFormat(" and ( {0}.id_vehicle in ({1})", _dataTable.Prefix, list);
            }

            // Vérifie s'il à toujours les droits pour accéder aux données de ce Vehicle
            //if (vehicleIds.All(p => !plurimediaDbIds.Contains(p)))
            //{
                sql.Append(FctUtilities.SQLGenerator.getAccessVehicleList(_session, _dataTable.Prefix, true));
            //}
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

          



          

            #endregion

            #region Affiner Media

            bool isPluri = VehiclesInformation.DatabaseIdToEnum(((LevelInformation)_session.SelectionUniversMedia.FirstNode.Tag).ID) == CstDBClassif.Vehicles.names.plurimedia;

            // Affiner Media
            if (!isPluri && _session.PrincipalMediaUniverses != null && _session.PrincipalMediaUniverses.Count > 0)
                sql.Append("  " + _session.PrincipalMediaUniverses[0].GetSqlConditions(_dataTable.Prefix, true));
            #endregion

            #region Sélection produits

            // Sélection de Produits
            if (_session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
                sql.Append("  " + _session.PrincipalProductUniverses[0].GetSqlConditions(_dataTable.Prefix, true));

            #endregion

            

        }


        #endregion

        #region GetMonthsFileds


        protected override void GetMonthsFileds(StringBuilder sql, string[] months, string year, int firstMonth, string[] persoMonths,
           string[] previousYearMonths, string previousYear, string[] persoPreviousYearMonths)
        {
            int i;
            string unitLabel = _session.Unit.ToString();
            //création de la liste des champs période

            for (i = 0; i < months.Length; i++)
            {
                months[i] = $"sum({expenditurePrefix}{unitLabel}_n{year}_{(i + firstMonth)})";
            
                if (_session.ComparativeStudy)
                {
                    previousYearMonths[i] = $"sum({expenditurePrefix}{unitLabel}_n{previousYear}_{(i + firstMonth)})";
                  
                }
            }

            //Year
            if (_reportFormat.ToString().IndexOf("_Year") > -1 ||
                _reportFormat == CstFormat.PreformatedTables.productYear_X_Media)
            {
                string comparativeStudyMonths = "";

                for (i = 0; i < months.Length; i++)
                {
                    if (i == months.Length - 1 && months.Length > 1)
                    {
                        sql.Append($"{expenditurePrefix}{unitLabel}_n{year}_{(i + firstMonth)}) as N{year}");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} {expenditurePrefix}{unitLabel}_n{previousYear}_{(i + firstMonth)}) as N{previousYear}";
                    }
                    else if (i == 0 && months.Length > 1)
                    {
                        sql.Append($", sum({expenditurePrefix}{unitLabel}_n{year}_{(i + firstMonth)} + ");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} sum({expenditurePrefix}{unitLabel}_n{previousYear}_{(i + firstMonth)} + ";
                    }
                    else if (months.Length == 1)
                    {
                        sql.Append($", sum({expenditurePrefix}{unitLabel}_n{year}_{(i + firstMonth)}) as N{year}");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} sum({expenditurePrefix}{unitLabel}_n{previousYear}_{(i + firstMonth)}) as N{previousYear}";
                    }
                    else
                    {
                        sql.Append($"{expenditurePrefix}{unitLabel}_n{year}_{(i + firstMonth)} + ");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} {expenditurePrefix}{unitLabel}_n{previousYear}_{(i + firstMonth)} + ";
                    }
                }
                if (_session.ComparativeStudy)
                    sql.AppendFormat(", {0}", comparativeStudyMonths);
            }
        }
        #endregion

        protected override void AppendMediaFields(StringBuilder sql, int mediaIndex)
        {
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
                        sql.AppendFormat(
                            " {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_category as id_m2, category as m2",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleCategoryMedia:
                        sql.AppendFormat(
                            " {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_category as id_m2, category as m2, {0}.id_media as id_m3, media as m3",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleInterestCenterMedia:
                        sql.AppendFormat(
                            " {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_INTEREST_CENTER as id_m2, INTEREST_CENTER as m2, {0}.id_media as id_m3, media as m3",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.categoryInterestCenterMedia:
                        sql.AppendFormat(
                            " {0}.id_category as id_m1, category as m1, {0}.id_INTEREST_CENTER as id_m2, INTEREST_CENTER as m2, {0}.id_media as id_m3, media as m3",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.vehicleMedia:
                        sql.AppendFormat(" {0}.id_vehicle as id_m1, vehicle as m1, {0}.id_media as id_m2, media as m2",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.spotTypeSpotSubType:
                        sql.AppendFormat(
                            " {0}.ID_SPOT_TYPE as id_m1, SPOT_TYPE as m1, {0}.ID_SPOT_SUB_TYPE as id_m2, SPOT_SUB_TYPE as m2",
                            _dataTable.Prefix);
                        break;
                    case CstFormat.PreformatedMediaDetails.mediaSpotSubType:
                        sql.AppendFormat(
                            " {0}.id_media as id_m1, media as m1, {0}.ID_SPOT_SUB_TYPE as id_m2, SPOT_SUB_TYPE as m2",
                            _dataTable.Prefix);
                        break;
                    default:
                        _session.PreformatedMediaDetail = CstFormat.PreformatedMediaDetails.vehicle;
                        sql.Append(" {0}.id_vehicle as id_m1, vehicle as m1");
                        break;
                }
            }
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
        protected override void AppendJointClause(StringBuilder sql)
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
            if (sql.ToString().IndexOf(_recapAdSlogan.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.ID_AD_SLOGAN = {2}.ID_AD_SLOGAN", linkWord, _recapAdSlogan.Prefix, _dataTable.Prefix);
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

            if (sql.ToString().IndexOf(_recapInterestCenter.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.id_INTEREST_CENTER = {2}.id_INTEREST_CENTER", linkWord, _recapInterestCenter.Prefix, _dataTable.Prefix);
            }
            if (sql.ToString().IndexOf(_recapSpotSubType.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.ID_SPOT_SUB_TYPE = {2}.ID_SPOT_SUB_TYPE", linkWord, _recapSpotSubType.Prefix, _dataTable.Prefix);
            }
            if (sql.ToString().IndexOf(_recapSpotType.SqlWithPrefix) > -1)
            {
                sql.AppendFormat("{0} {1}.ID_SPOT_TYPE = {2}.ID_SPOT_TYPE", linkWord, _recapSpotType.Prefix, _dataTable.Prefix);
            }
            #endregion


        }
        #endregion

        protected override void AppendActivationLanguageClause(StringBuilder sql)
        {
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

            if (sql.ToString().IndexOf(_recapAdSlogan.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapAdSlogan.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapAdSlogan.Prefix, CstDB.ActivationValues.UNACTIVATED);
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
            if (sql.ToString().IndexOf(_recapInterestCenter.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapInterestCenter.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapInterestCenter.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapSpotSubType.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapSpotSubType.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapSpotSubType.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            if (sql.ToString().IndexOf(_recapSpotType.SqlWithPrefix) > -1)
            {
                sql.AppendFormat(" and {0}.id_language = {1}", _recapSpotType.Prefix, _session.DataLanguage);
                sql.AppendFormat(" and {0}.activation < {1}", _recapSpotType.Prefix, CstDB.ActivationValues.UNACTIVATED);
            }
            #endregion
        }
    }
}


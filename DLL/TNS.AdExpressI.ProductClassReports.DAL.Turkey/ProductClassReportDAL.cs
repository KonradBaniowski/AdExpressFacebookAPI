using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using TNS.AdExpress.Constantes.Classification.DB;
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

namespace TNS.AdExpressI.ProductClassReports.DAL.Turkey
{
    public class ProductClassReportDAL : DAL.ProductClassReportDAL
    {
        #region Constructor
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public ProductClassReportDAL(WebSession session) : base(session) { }
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

        protected override void GetMonthsFileds(StringBuilder sql, string[] months, string year, int firstMonth, string[] persoMonths,
            string[] previousYearMonths, string previousYear, string[] persoPreviousYearMonths)
        {
            int i;
            string unitLabel = _session.Unit.ToString();
            //création de la liste des champs période

            for (i = 0; i < months.Length; i++)
            {
                months[i] = $"sum(exp_{unitLabel}_n{year}_{(i + firstMonth)})";
                persoMonths[i] = $"exp_{unitLabel}_n{year}_{(i + firstMonth)}";
                if (_session.ComparativeStudy)
                {
                    previousYearMonths[i] = $"sum(exp_{unitLabel}_n{previousYear}_{(i + firstMonth)})";
                    persoPreviousYearMonths[i] = $"exp_{unitLabel}_n{previousYear}_{(i + firstMonth)}";
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
                        sql.Append($"exp_{unitLabel}_n{year}_{(i + firstMonth)}) as N{year}");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} exp_{unitLabel}_n{previousYear}_{(i + firstMonth)}) as N{previousYear}";
                    }
                    else if (i == 0 && months.Length > 1)
                    {
                        sql.Append($", sum(exp_{unitLabel}_n{year}_{(i + firstMonth)} + ");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} sum(exp_{unitLabel}_n{previousYear}_{(i + firstMonth)} + ";
                    }
                    else if (months.Length == 1)
                    {
                        sql.Append($", sum(exp{unitLabel}_n{year}_{(i + firstMonth)}) as N{year}");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} sum(exp_{unitLabel}_n{previousYear}_{(i + firstMonth)}) as N{previousYear}";
                    }
                    else
                    {
                        sql.Append($"exp_{unitLabel}_n{year}_{(i + firstMonth)} + ");
                        comparativeStudyMonths =
                            $"{comparativeStudyMonths} exp_{unitLabel}_n{previousYear}_{(i + firstMonth)} + ";
                    }
                }
                if (_session.ComparativeStudy)
                    sql.AppendFormat(", {0}", comparativeStudyMonths);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Insertions.DAL.Exceptions;
using CstWeb = TNS.AdExpress.Constantes.Web;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;
namespace TNS.AdExpressI.Insertions.DAL.Poland
{
    public class InsertionsDAL : TNS.AdExpressI.Insertions.DAL.InsertionsDAL
    {

        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="moduleId">Current Module id</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
        }
        #endregion

        #region GetDateFields
        /// <summary>
        /// Ge date fields
        /// </summary>
        /// <param name="vehicleInformation">vehicle Information</param>
        /// <returns>date fields</returns>
        protected override string GetDateFields(VehicleInformation vehicleInformation)
        {
           
            // Date fields           
              return  ", date_media_num ";
        }
        #endregion

        /// <summary>
        /// Append Filters depending on client univers selection and rights
        /// </summary>
        /// <param name="sql">Sql request container</param>
        /// <param name="tData">Data Table Description</param>
        /// <param name="fromDate">Beginning of the period</param>
        /// <param name="toDate">End of the period</param>
        /// <param name="vehicle">Vehicle information</param>
        /// <param name="universId">Current Univers</param>
        /// <param name="filters">FIlters to apply</param>
        protected override void AppendUniversFilters(StringBuilder sql, TNS.AdExpress.Domain.DataBaseDescription.Table tData, int fromDate, int toDate, VehicleInformation vehicle, int universId, string filters)
        {

            #region Period
            /* Determine the period of the study
             * PeriodType correspond to the type of date selection, for example :
             *  dateToDate : means that the date format is YYYYMMDD
             *  dateToDateMonth : means that the date format is YYYYMM
             *  dateToDateWeek : means that the date format is YYYYWW
             * depending on the date format we can create the correct SQL query filter
             * */
            if (_session.PeriodType == CstWeb.CustomerSessions.Period.Type.dateToDate)
            {

                int begin = Convert.ToInt32(_session.PeriodBeginningDate);
                if (begin > fromDate)
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", begin, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, begin);
                    }
                }
                else
                {
                    sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    }
                }
                int end = Convert.ToInt32(_session.PeriodEndDate);
                if (end < toDate)
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", end, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, end);
                    }
                }
                else
                {
                    sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                    if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                    {
                        sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                    }
                }
            }
            else
            {
                sql.AppendFormat(" {1}.date_media_num >= {0}", fromDate, tData.Prefix);
                sql.AppendFormat(" and {1}.date_media_num <= {0}", toDate, tData.Prefix);
                if (_module.Id == CstWeb.Module.Name.NEW_CREATIVES)
                {
                    sql.AppendFormat(" and {0}.date_creation >= to_date('{1} 00:00:00','yyyymmdd HH24:MI:SS') ", tData.Prefix, fromDate);
                    sql.AppendFormat(" and {0}.date_creation <= to_date('{1} 23:59:59','yyyymmdd HH24:MI:SS') ", tData.Prefix, toDate);
                }
            }
            #endregion

            #region Advertiser modules
            /* Versions filter
             * If the customer has filtred the number of the original slogan, we can find this list in the SloganIdList variable.
             * in this case we add an id_slogan filter, like below
             * */
            if (_module.Id == CstWeb.Module.Name.ALERTE_PLAN_MEDIA && !string.IsNullOrEmpty(_session.SloganIdList))
            {
                if(vehicle.Id == CstDBClassif.Vehicles.names.adnettrack)
                    sql.AppendFormat(" and {1}.ID_BANNERS in ( {0} ) ", _session.SloganIdList, tData.Prefix);
                else  sql.AppendFormat(" and {1}.id_slogan in ( {0} ) ", _session.SloganIdList, tData.Prefix);
            }
            // Product classification selection
            /* We can get the product classification selection by using _session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true)
             * */
            if (_module.Id != CstWeb.Module.Name.NEW_CREATIVES && _session.PrincipalProductUniverses != null && _session.PrincipalProductUniverses.Count > 0)
            {
                if (universId < 0)
                {
                    sql.Append(_session.PrincipalProductUniverses[0].GetSqlConditions(tData.Prefix, true));
                }
                else
                {
                    sql.Append(_session.PrincipalProductUniverses[universId].GetSqlConditions(tData.Prefix, true));
                }
            }
            #endregion

            #region Media modules
            /* Get the list of medias selected
             * There are several treenodes to save the univers media, so according to the module we can get the media list
             * */
            string listMediaAccess = string.Empty;
            if (_module.Id == CstWeb.Module.Name.ANALYSE_CONCURENTIELLE || _module.Id == CstWeb.Module.Name.ANALYSE_PORTEFEUILLE)
            {
                Dictionary<CstCustomer.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
                if (selectedVehicles != null && selectedVehicles.ContainsKey(CstCustomer.Right.type.mediaAccess))
                {
                    listMediaAccess = selectedVehicles[CstCustomer.Right.type.mediaAccess];
                }
            }
            if (_module.Id == CstWeb.Module.Name.ANALYSE_PLAN_MEDIA)
            {
                string list = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
                if (list.Length > 0 && vehicle.Id == CstDBClassif.Vehicles.names.internet) list = list.Replace(vehicle.DatabaseId.ToString(),
                    VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId.ToString());
                if (list.Length > 0) sql.AppendFormat(" and ({0}.id_vehicle in ({1})) ", tData.Prefix, list);
            }
            if (listMediaAccess.Length > 0)
            {
                sql.AppendFormat(" and (({1}.id_media in ({0}))) ", listMediaAccess.Substring(0, listMediaAccess.Length), tData.Prefix);
            }

            if (_session.SecondaryMediaUniverses != null && _session.SecondaryMediaUniverses.Count > 0)
                sql.Append(_session.SecondaryMediaUniverses[0].GetSqlConditions(tData.Prefix, true));

            #endregion

            #region Rights
            /* Get product classification rights
             * */
            if (_module == null) throw (new InsertionsDALException("_module parameter cannot be NULL"));
            sql.Append(FctWeb.SQLGenerator.GetClassificationCustomerProductRight(_session, tData.Prefix, true, _module.ProductRightBranches));


            /* Get media classification rights
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.internet)
            {
                sql.Append(SQLGenerator.GetAdNetTrackMediaRight(_session, tData.Prefix, true));
            }
            else
            {
                sql.Append(SQLGenerator.getAnalyseCustomerMediaRight(_session, tData.Prefix, true));
            }

            /* Get rights detail spot to spot TNT
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.tv
                && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG) && !_creaConfig)
            {
                string idTNTCategory = TNS.AdExpress.Domain.Lists.GetIdList(CstWeb.GroupList.ID.category, CstWeb.GroupList.Type.digitalTv);
                if (idTNTCategory != null && idTNTCategory.Length > 0)
                {
                    sql.Append(" and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_category not in (" + idTNTCategory + ")  ");
                }
            }
            #endregion

            #region Sponsorship univers
            /* This test is specific to the sponsoring modules
             * */
            if (_module.Id == CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES
                || _module.Id == CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS)
            {
                string tmp = string.Empty;
                if (_session.CurrentUniversMedia != null && _session.CurrentUniversMedia.Nodes.Count > 0)
                {
                    tmp = _session.GetSelection(_session.CurrentUniversMedia, CstCustomer.Right.type.mediaAccess);
                }
                if (tmp.Length > 0)
                {
                    sql.AppendFormat(" and  {1}.id_media in ({0}) ", tmp, tData.Prefix);
                }
                /* Program classification
                 * */
                sql.Append(SQLGenerator.GetCustomerProgramSelection(_session, tData.Prefix, tData.Prefix, true));
                /* Type of sponsorship selection
                 * */
                sql.Append(SQLGenerator.GetCustomerSponsorshipFormSelection(_session, tData.Prefix, true));
            }
            #endregion

            #region Global rules
            /* Get filters of the product classification levels
             * */
            switch (_module.Id)
            {
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA:
                case CstWeb.Module.Name.ALERTE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA_CONCURENTIELLE:
                case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                case CstWeb.Module.Name.ANALYSE_POTENTIELS:
                    sql.Append(SQLGenerator.getLevelProduct(_session, tData.Prefix, true));
                    break;
            }

            /* Get radio rules
             * */
            if (_module.ModuleType == CstWeb.Module.Type.tvSponsorship)
            {
                sql.Append(SQLGenerator.getAdExpressUniverseCondition(CstWeb.AdExpressUniverse.TV_SPONSORINGSHIP_MEDIA_LIST_ID, tData.Prefix, tData.Prefix, tData.Prefix, true));
            }
            else
            {
                sql.Append(SQLGenerator.GetAdExpressProductUniverseCondition(CstWeb.AdExpressUniverse.EXCLUDE_PRODUCT_LIST_ID, tData.Prefix, true, false));
            }
           
            #endregion

            #region Banners Format Filter
            List<Int64> formatIdList = null;
            VehicleInformation cVehicleInfo = null;
            if (vehicle.Id == CstDBClassif.Vehicles.names.internet)
                cVehicleInfo = VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack);
            else
                cVehicleInfo = vehicle;
            formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[] { cVehicleInfo }));
            if (formatIdList.Count > 0)
                sql.Append(" and " + tData.Prefix + ".ID_" + WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.
                    VehiclesFormatInformation.VehicleFormatInformationList[cVehicleInfo.DatabaseId].FormatTableName).Label 
                    + " in (" + string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()) + ") ");
            #endregion

            #region Filtres
            /* Get filter clause and check zero version
             * According to the module type, we save the classification levels in GenericProductDetailLevel or in GenericMediaDetailLevel
             * GenericProductDetailLevel : product classification levels
             * GenericMediaDetailLevel : media classification levels
             * */
            if (!string.IsNullOrEmpty(filters))
            {
                GenericDetailLevel detailLevels = null;
                switch (_module.Id)
                {
                    case CstWeb.Module.Name.ANALYSE_CONCURENTIELLE:
                    case CstWeb.Module.Name.NEW_CREATIVES:
                    case CstWeb.Module.Name.ANALYSE_PORTEFEUILLE:
                        detailLevels = _session.GenericProductDetailLevel;
                        break;
                    case CstWeb.Module.Name.ANALYSE_PLAN_MEDIA:
                    case CstWeb.Module.Name.ANALYSE_DYNAMIQUE:
                    case CstWeb.Module.Name.ANALYSE_DES_DISPOSITIFS:
                    case CstWeb.Module.Name.ANALYSE_DES_PROGRAMMES:
                        detailLevels = _session.GenericMediaDetailLevel;
                        break;
                }
                sql.Append(GetFiltersClause(tData, detailLevels, filters, vehicle));
                sql.AppendFormat(CheckZeroVersion(tData, detailLevels, vehicle, filters));
            }
            if (_session.SloganIdZoom > -1)
            {//For Russia : _session.SloganIdZoom > long.MinValue (correspond to the absence of ID for the version)
                if (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack)               
                sql.AppendFormat(" and wp.ID_BANNERS={0}", _session.SloganIdZoom);
                else sql.AppendFormat(" and wp.id_slogan={0}", _session.SloganIdZoom);
            }
           

            /* Version selection
             * */
            string slogans = _session.SloganIdList;

            /* Refine vesions
             * */
            if (slogans.Length > 0)
            {
                if (vehicle.Id == CstDBClassif.Vehicles.names.adnettrack) 
                sql.AppendFormat(" and {0}.ID_BANNERS in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans);
                else
                    sql.AppendFormat(" and {0}.id_slogan in({1}) ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, slogans);
            }
            #endregion

            /* For the media type press and international press we can do studies according to the inset option (total, inset excluding, inset)
             * so to get the corresponding filter we use GetJointForInsertDetail method
             * */
            if (vehicle.Id == CstDBClassif.Vehicles.names.press || vehicle.Id == CstDBClassif.Vehicles.names.internationalPress || vehicle.Id == CstDBClassif.Vehicles.names.newspaper
                || vehicle.Id == CstDBClassif.Vehicles.names.magazine)
            {
                sql.Append(FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));
            }

            switch (vehicle.Id)
            {
                case CstDBClassif.Vehicles.names.internet:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ", VehiclesInformation.Get(CstDBClassif.Vehicles.names.adnettrack).DatabaseId, tData.Prefix);
                    break;
                default:
                    sql.AppendFormat(" and {1}.id_vehicle={0} ", vehicle.DatabaseId, tData.Prefix);
                    break;
            }
        }

    }
}

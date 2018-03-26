#region Information
/*
 * Author : B.Masson
 * Created on 29/09/2008
 * Modifications :
 *      Author - Date - Description
 * 
 * 
 * */
#endregion

#region Using
using System;
using System.Data;
using System.Text;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpressI.NewCreatives.DAL.Exceptions;

using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

using CstDBDesc = TNS.AdExpress.Domain.DataBaseDescription;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Utilities;
using System.Collections.Generic;
#endregion

namespace TNS.AdExpressI.NewCreatives.DAL {

    /// <summary>
    /// Extract data for new creatives
    /// </summary>
    public abstract class NewCreativesDAL:INewCreativeResultDAL {

        #region Variables
        /// <summary>
        /// User session
        /// </summary>
        protected WebSession _session;
        /// <summary>
        /// Current vehicle
        /// </summary>
        protected VehicleInformation _vehicleInformation;
        /// <summary>
        /// Sector ID
        /// </summary>
        protected string _idSectors;
        /// <summary>
        /// Begining Date
        /// </summary>
        protected string _beginingDate;
        /// <summary>
        /// End Date
        /// </summary>
        protected string _endDate;
        #endregion

        #region Accessors
        /// <summary>
        /// Get User Session
        /// </summary>
        protected WebSession Session {
            get { return _session; }
        }
        /// <summary>
        /// Get Vehicle
        /// </summary>
        protected VehicleInformation VehicleInformationObject {
            get { return _vehicleInformation; }
        }
        #endregion

        #region Constructor

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Websession</param>
        /// <param name="idSectors">id Sectors</param>
        /// <param name="beginingDate">begining Date</param>
        /// <param name="endDate">end Date</param>
        public NewCreativesDAL(WebSession session, string idSectors, string beginingDate, string endDate) {
            _session = session;
            _idSectors = idSectors;
            _beginingDate = beginingDate;
            _endDate = endDate;

            #region Sélection du vehicle
            string vehicleSelection = _session.GetSelection(_session.SelectionUniversMedia, CstCustomer.Right.type.vehicleAccess);
            if(vehicleSelection == null || vehicleSelection.IndexOf(",") > 0) throw (new NewCreativesDALException("Selection of media is not correct"));
            _vehicleInformation = VehiclesInformation.Get(Int64.Parse(vehicleSelection));
            #endregion
        }
        #endregion

        #region GetData
        /// <summary>
        /// Get Data for new creatives
        /// </summary>
        /// <returns>Data for new creatives</returns>
        public DataSet GetData() {

            #region Variables
            StringBuilder sql = new StringBuilder();
            string detailProductTablesNames = "";
            string detailProductFields = "";
            string detailProductJoints = "";
            string detailProductOrderBy = "";
            string productsRights = "";
            string dataTableNameForGad = "";
            string dataFieldsForGad = "";
            string dataJointForGad = "";
            Table table = null, tableDataMobile = null, bannersCountry = null;
            Schema schAdExpr03 = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03);
            bool useTableDataMobile = false;
            bool applyCountryRights = false;
            const string prefixDataMobile = "dt";
            #endregion

            #region Construction de la requête
            try {
                detailProductTablesNames = _session.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields = _session.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                detailProductOrderBy = _session.GenericProductDetailLevel.GetSqlOrderFields();
                TNS.AdExpress.Domain.Web.Navigation.Module module = TNS.AdExpress.Domain.Web.Navigation.ModulesList.GetModule(_session.CurrentModule);
                productsRights = SQLGenerator.GetClassificationCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, module.ProductRightBranches);
               
                table = GetTable(_vehicleInformation, _session.IsSelectRetailerDisplay);
                useTableDataMobile = (_vehicleInformation.Id == TNS.AdExpress.Constantes.Classification.DB.Vehicles.names.evaliantMobile
                    && !_session.CustomerLogin.CustormerFlagAccess(CstDB.Flags.ID_APPLICATION_MOBILE_CREATIVE_FLAG));
                if (_session.EvaliantCountryAccessList != null)
                    applyCountryRights = (_session.EvaliantCountryAccessList.Length > 0) ? true : false;

                 if(useTableDataMobile) tableDataMobile = WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, _session.IsSelectRetailerDisplay);
                 if (applyCountryRights) bannersCountry = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.bannersCountry);

                dataTableNameForGad = AppendGad(dataTableNameForGad, schAdExpr03, table, ref dataFieldsForGad, ref dataJointForGad);

                // select
                sql.Append("select distinct " + detailProductFields + ","+table.Prefix +".hashcode as versionNb ");

                switch(_session.DetailPeriod) {
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                        sql.Append(", to_char( min(wp.date_creation) , 'YYYYMM' ) as date_creation ");
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                        sql.Append(", to_char( min(wp.date_creation) , 'IYYYIW' ) as date_creation ");
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
                        sql.Append(", to_char( min(wp.date_creation) , 'YYYYMMDD' ) as date_creation ");
                        break;
                 
                }
                sql.Append(dataFieldsForGad+" ");
                
                // from
                
                sql.Append("from " + table.SqlWithPrefix + " , ");
                sql.Append(detailProductTablesNames);
                sql.Append(dataTableNameForGad + " ");
                if (useTableDataMobile)
                    sql.Append("," + tableDataMobile.Sql + " " + prefixDataMobile);
                if(applyCountryRights)
                    sql.Append("," + bannersCountry.SqlWithPrefix);

                // where
				string maxHour ="23:59:59";
                sql.Append(" where " + table.Prefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
				sql.Append(" and " + table.Prefix + ".date_creation <= to_date('" + _endDate + maxHour + "','yyyymmddHH24:MI:SS') ");
                sql.Append(detailProductJoints);
                sql.Append(productsRights);
                sql.Append(" " + dataJointForGad + " ");

                // Sector ID
                if(!string.IsNullOrEmpty(_idSectors))
                    sql.Append(" and " + table.Prefix + ".id_sector in (" + _idSectors + ") ");
                //Filtering with category Application mobile
                if (useTableDataMobile)
                {
                    sql.Append(" and " + table.Prefix + ".HASHCODE = " + prefixDataMobile + ".HASHCODE ");
                   // sql.Append(" and " + table.Prefix + ".ID_BANNERS_MOBILE = " + prefixDataMobile + ".ID_BANNERS ");
                    sql.Append(" and " + prefixDataMobile + ".id_category not in (" + TNS.AdExpress.Domain.Lists.GetIdList(TNS.AdExpress.Constantes.Web.GroupList.Type.applicationMobile) + ") ");
                }

                #region Banners Format Filter
                List<Int64> formatIdList = _session.GetValidFormatSelectedList(new List<VehicleInformation>(new[]{_vehicleInformation}));
                if (formatIdList.Count > 0)
                    sql.Append(" and " + table.Prefix + ".ID_" + WebApplicationParameters.DataBaseDescription.GetTable(WebApplicationParameters.VehiclesFormatInformation.VehicleFormatInformationList[_vehicleInformation.DatabaseId].FormatTableName).Label + " in (" + string.Join(",", formatIdList.ConvertAll(p => p.ToString()).ToArray()) + ") ");
                #endregion

                #region Banners Country rights
                if (applyCountryRights) {
                    sql.AppendFormat(" and {0}.hashcode = {1}.hashcode ", bannersCountry.Prefix, table.Prefix);
                    sql.AppendFormat(" and {0}.id_country in ({1})", bannersCountry.Prefix, _session.EvaliantCountryAccessList);
                }
                #endregion

                // group by
                sql.Append(" group by " + detailProductFields + ","+ table.Prefix + ".hashcode ");
                if(dataFieldsForGad.Length>0)
                    sql.Append(dataFieldsForGad);
                
                // order by
                sql.Append(" order by " + detailProductOrderBy + ", date_creation ");
            }
            catch(Exception err) {
                throw (new NewCreativesDALException("Unable to build request for new creatives : " + sql, err));
            }
            #endregion

            #region Execution de la requête
            try {
                return _session.Source.Fill(sql.ToString());
            }
            catch(System.Exception err) {
                throw (new NewCreativesDALException("Unable to load data for new creatives : " + sql, err));
            }
            #endregion

        }

        protected virtual string AppendGad(string dataTableNameForGad, Schema schAdExpr03, Table table, ref string dataFieldsForGad,
            ref string dataJointForGad)
        {
            if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
            {
                try
                {
                    dataTableNameForGad = ", " + schAdExpr03.Sql + SQLGenerator.GetTablesForGad(_session) + " " +
                                          CstDB.Tables.GAD_PREFIXE;
                    dataFieldsForGad = ", " + SQLGenerator.GetFieldsAddressForGad();
                    dataJointForGad = "and " + SQLGenerator.GetJointForGad(table.Prefix);
                }
                catch (SQLGeneratorException)
                {
                    ;
                }
            }
            return dataTableNameForGad;
        }

        #endregion

        #region protected Methods
        /// <summary>
        ///Get table
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <param name="isRetailerSelection">Is Retailer Selectioned</param>
        /// <returns>Table</returns>
        private Table GetTable(VehicleInformation vehicle, bool isRetailerSelection) {
            switch (vehicle.Id) {
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return WebApplicationParameters.GetDataTable(TableIds.banners, isRetailerSelection);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return WebApplicationParameters.GetDataTable(TableIds.banners_mobile, isRetailerSelection);              
                default:
                    throw (new SQLGeneratorException("Impossible de déterminer la table à traiter"));
            }
        }
        #endregion

        public string BeginingDate { get { return _beginingDate; } set { _beginingDate = value; } }

    }

}

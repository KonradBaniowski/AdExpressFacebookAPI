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
using System.Windows.Forms;

using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Exceptions;

using TNS.AdExpressI.NewCreatives.DAL.Exceptions;

using TNS.FrameWork.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

using CstDBDesc = TNS.AdExpress.Domain.DataBaseDescription;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstCustomer = TNS.AdExpress.Constantes.Customer;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Utilities;
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
        protected Int64 _idSector;
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
        public NewCreativesDAL(WebSession session, Int64 idSector, string beginingDate, string endDate) {
            this._session = session;
            this._idSector = idSector;
            this._beginingDate = beginingDate;
            this._endDate = endDate;

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
            Table table = null;
            #endregion

            #region Construction de la requête
            try {
                detailProductTablesNames = _session.GenericProductDetailLevel.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label);
                detailProductFields = _session.GenericProductDetailLevel.GetSqlFields();
                detailProductJoints = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                detailProductOrderBy = _session.GenericProductDetailLevel.GetSqlOrderFields();
                productsRights = WebFunctions.SQLGenerator.getAnalyseCustomerProductRight(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);

                // select
                sql.Append("select distinct " + detailProductFields + ", hashcode as versionNb ");

                switch(_session.DetailPeriod) {
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                        sql.Append(", to_char( min(wp.date_creation) , 'YYYYMM' ) as date_creation ");
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                        sql.Append(", to_char( min(wp.date_creation) , 'YYYYIW' ) as date_creation ");
                        break;
                    case WebConstantes.CustomerSessions.Period.DisplayLevel.dayly:
                        sql.Append(", to_char( min(wp.date_creation) , 'YYYYMMDD' ) as date_creation ");
                        break;
                    default:
                        break;
                }
                
                // from
                table = GetTable(_vehicleInformation);
                sql.Append("from " + table.SqlWithPrefix + " , ");
                sql.Append(detailProductTablesNames);

                // where
                sql.Append(" where " + table.Prefix + ".date_creation >= to_date('" + _beginingDate + "','yyyymmdd') ");
                sql.Append(" and " + table.Prefix + ".date_creation <= to_date('" + _endDate + "','yyyymmdd') ");
                sql.Append(detailProductJoints);
                sql.Append(productsRights);

                // Sector ID
                if(_idSector != -1)
                    sql.Append(" and " + table.Prefix + ".id_sector in (" + _idSector + ") ");

                // group by
                sql.Append(" group by " + detailProductFields + ", hashcode ");
                
                // order by
                sql.Append(" order by " + detailProductOrderBy + ", date_creation ");
            }
            catch(System.Exception err) {
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
        #endregion

        #region protected Methods
        /// <summary>
        ///Get table
        /// </summary>
        /// <param name="vehicle">Vehicle Information</param>
        /// <returns>Table</returns>
        private Table GetTable(VehicleInformation vehicle) {
            switch (vehicle.Id) {
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.banners);
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    return WebApplicationParameters.DataBaseDescription.GetTable(TableIds.banners_mobile);
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                default:
                    throw (new SQLGeneratorException("Impossible de déterminer la table à traiter"));
            }
        }
        #endregion

    }

}

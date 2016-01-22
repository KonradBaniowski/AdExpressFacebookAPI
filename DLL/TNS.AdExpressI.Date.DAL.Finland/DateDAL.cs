using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.FrameWork.DB.Common;
using System.Data;
using TNS.FrameWork.Date;

namespace TNS.AdExpressI.Date.DAL.Finland {
    /// <summary>
    /// This class inherits from the Date DAL Class which provides all the methods to determine specific dates for specific modules.
    /// </summary>
    public class DateDAL : TNS.AdExpressI.Date.DAL.DateDAL {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DateDAL() : base() { }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Client session</param>
        public DateDAL(WebSession session) : base(session) { }
        #endregion

        #region GetTendenciesLastAvailableDate
        /// <summary>
        /// Get Tendencies Last Available Date
        /// </summary>
        /// <returns>Last Available Date</returns>
        public override DateTime GetTendenciesLastAvailableDate() {

            DateTime date = new DateTime(1, 1, 1);
            StringBuilder sql = new StringBuilder();
            Table tableName = WebApplicationParameters.GetDataTable(TableIds.tendencyMonth, false);
            IDataSource dataSource = GetDataSource();

            sql.Append(" SELECT max(date_period) as lastAvailableDate ");
            sql.AppendFormat(" FROM {0} ", tableName.Sql);

            #region Execution of the query
            try {
                /* Execute the SQL request
                 * */
                DataSet ds = dataSource.Fill(sql.ToString());
                /* Return the last date
                 * */
                if (ds != null && ds.Tables[0].Rows.Count > 0) {
                    string dateStr = ds.Tables[0].Rows[0]["lastAvailableDate"].ToString();
                    DateTime lastDay = new DateTime(int.Parse(dateStr.Substring(0, 4)), int.Parse(dateStr.Substring(4, 2)), 1).AddMonths(1).AddDays(-1);
                    return (lastDay);
                }
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Error while trying to get the last publication date", err));
            }
            #endregion

            return date;
        }
        #endregion

    }
}

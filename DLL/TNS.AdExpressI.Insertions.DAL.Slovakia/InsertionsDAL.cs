using System;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.Insertions.DAL.Slovakia
{
    public class InsertionsDAL :DAL.InsertionsDAL
    {
        #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="module">Current Module</param>
        public InsertionsDAL(WebSession session, Int64 moduleId)
            : base(session, moduleId)
        {
        }
        #endregion

        public override string GetVersionMinParutionDate(string idVersion, VehicleInformation vehicleInformation)
        {
            string minDate = string.Empty;
            Table table = SQLGenerator.GetDataTable(vehicleInformation, Module.Type.analysis, false);
            var sql = new StringBuilder(1000);

            sql.AppendFormat(" select distinct id_slogan, associated_file, date_media_num  from {0} where id_slogan={1} order by date_media_num asc", table.SqlWithPrefix, idVersion);

            var ds = _session.Source.Fill(sql.ToString());

            if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count > 0)
            {
                minDate = Convert.ToString(ds.Tables[0].Rows[0][1]);
            }
            return minDate;
        }
    }
}

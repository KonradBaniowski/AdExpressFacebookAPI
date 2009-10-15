#region Information
/*
 * Author : D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using TNS.AdExpress.Web.Core.Sessions;

using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web.Navigation;
using WebNavigation = TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CustomerRightConstante = TNS.AdExpress.Constantes.Customer.Right;
using VehicleClassificationCst = TNS.AdExpress.Constantes.Classification.DB.Vehicles.names;

namespace TNS.AdExpressI.Classification.DAL.SqlServerRussia
{
    public class VehiclesDAL : TNS.AdExpressI.Classification.DAL.VehiclesDAL{

        #region Constructors
        /// <summary>
        /// Default Constructor
        /// </summary>
        /// <param name="session">User session</param>
        public VehiclesDAL(WebSession session)
            : base(session)
        {
        }
        #endregion

        #region GetData()
        /// <summary>
        /// This method provides SQL queries to get the media typeclassification level's items.
        /// The data are filtered by customer's media rights and selected working set.		
        /// </summary>
        /// <returns>Data set 
        /// with media type's identifiers ("idMediaType" column) and media type's labels ("mediaType" column).
        /// </returns>
        /// <exception cref="TNS.AdExpressI.Classification.DAL.Exceptions.DetailMediaDALException">
        /// Impossible to execute query
        /// </exception>
        public override DataSet GetData()
        {
            DataSet ds = new DataSet();
            StringBuilder sql = new StringBuilder();


            try
            {
                using (SqlConnection conn = new SqlConnection(GetConnectionString()))
                {
                    SqlCommand cmd = conn.CreateCommand();
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "cl_media_type_get";

                    //cmd.Parameters.Add("@SEARCH_SET_ID", SqlDbType.Int);
                    //cmd.Parameters["@SEARCH_SET_ID"].Value = SearchSetId;

                    conn.Open();

                    SqlDataReader rs = cmd.ExecuteReader();
                    DataTable dt = new DataTable();
                    using (rs)
                    {
                        dt.Load(rs);
                    }

                    if (dt.Rows.Count > 0)
                    {
                        // add datatables to dataset 
                        dt.Columns[0].ColumnName = "idMediaType";
                        dt.Columns[1].ColumnName = "mediaType";
                        ds.Tables.Add(dt);

                    }

                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                string sTemp = ex.Message;
            }



            return ds;
        }
        #endregion

         private string GetConnectionString()
        {
            #region Building the connection string

            string Server = "194.58.36.199";
            string Username = "TNSRussie";
            string Password = "adextest1";
            string Database = "Media_2007_f15";

            string ConnectionString = "Data Source=" + Server + ";";
            ConnectionString += "User ID=" + Username + ";";
            ConnectionString += "Password=" + Password + ";";
            ConnectionString += "Initial Catalog=" + Database;

            #endregion
           
            return ConnectionString;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Data.SqlClient;
using System.Data;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using WebConstant = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Date.DAL.Russia
{
    /// <summary>
    /// This class inherits from the Date DAL Class which provides all the methods to determine specific dates for specific modules.
    /// </summary>
    public class DateDAL : TNS.AdExpressI.Date.DAL.DateDAL
    {

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

        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// TODO : TO IMPLEMENT FOR RUSSIA
        /// </summary>
        /// <returns>Year</returns>
        public override int GetLastLoadedYear()
        {
            DateTime last_month;

            if (_session == null)
                return DateTime.Now.Year;

            // Choosed Media
            string selectedMedia = _session.CustomerDataFilters.SelectedMediaType;

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[cl_media_last_year_get]";
                    cmd.Parameters.Add("@id_Media", SqlDbType.SmallInt);
                    cmd.Parameters.Add("@last_month", SqlDbType.SmallDateTime);
                    cmd.Parameters["@last_month"].Direction = ParameterDirection.InputOutput;

                    // SP set Parameters
                    cmd.Parameters["@id_Media"].Value = Int16.Parse(selectedMedia);
                    cmd.Parameters["@last_month"].Value = DBNull.Value;

                    // SP Execute
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["@last_month"].Value == DBNull.Value)
                    {
                        throw (new Exception.DateDALException("Empty last_month from DB"));
                    }
                    else
                    {
                        last_month = Convert.ToDateTime(cmd.Parameters["@last_month"].Value);
                    }

                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new Exception.DateDALException("Impossible to execute query", err));
            }
            #endregion

            return last_month.Year;
        }

        /// <summary>
        /// Get the last publication date
        /// TODO : TO IMPLEMENT FOR RUSSIA
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <returns>Date</returns>
        public override string GetLatestPublication(List<Int64> selectedVehicleList)
        {
            Dictionary<CustormerConstantes.Right.type, string> selectedVehicles = _session.CustomerDataFilters.SelectedVehicles;
            DateTime latest_pub_date;

            // Choosed Media
            string selectedMedia = _session.CustomerDataFilters.SelectedMediaType;

            // Choosed Vehicle List    
            string vehiclesList = "";
            if (selectedVehicles != null && selectedVehicles.ContainsKey(CustormerConstantes.Right.type.mediaAccess))
            {
                vehiclesList = selectedVehicles[CustormerConstantes.Right.type.mediaAccess];
            }

            //if (vehiclesList == "" || vehiclesList == null)
            //{
            //    throw (new Exception.DateDALException("Selected Vehicles list is empty"));
            //}


            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[cl_vehicle_latest_pub_date_get]";
                    cmd.Parameters.Add("@selected_media", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@selected_vehicles", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@latest_pub_date", SqlDbType.SmallDateTime);
                    cmd.Parameters["@latest_pub_date"].Direction = ParameterDirection.InputOutput;

                    // SP set Parameters
                    cmd.Parameters["@selected_media"].Value = selectedMedia;
                    cmd.Parameters["@selected_vehicles"].Value = vehiclesList;
                    cmd.Parameters["@latest_pub_date"].Value = DBNull.Value;

                    // SP Execute
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["@latest_pub_date"].Value == DBNull.Value)
                    {
                        throw (new Exception.DateDALException("Empty latest_pub_date from DB"));
                    }
                    else
                    {
                        latest_pub_date = Convert.ToDateTime(cmd.Parameters["@latest_pub_date"].Value);
                    }
                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new Exception.DateDALException("Impossible to execute query", err));
            }
            #endregion

            string latest_pub_date_str = "";

            latest_pub_date_str = latest_pub_date.Year.ToString();
            latest_pub_date_str += (latest_pub_date.Month < 10) ? ("0" + latest_pub_date.Month.ToString()) : latest_pub_date.Month.ToString();
            latest_pub_date_str += (latest_pub_date.Day < 10) ? ("0" + latest_pub_date.Day.ToString()) : latest_pub_date.Day.ToString();

            return latest_pub_date_str;

        }

        #region GetCalendarStartDate
        /// <summary>
        /// Get the calendar starting date
        /// </summary>
        /// <returns>The year corresponding to the starting date : date format yyyy example 2008</returns>
        public override int GetCalendarStartDate()
        {
            // Choosed Media
            string selectedMedia = _session.CustomerDataFilters.SelectedMediaType;
            // Media Schedule subcategory always is empty
            if (selectedMedia == "" && _session.CurrentModule == WebConstant.Module.Name.ANALYSE_PLAN_MEDIA)
                selectedMedia = "0";

            DateTime start_date;

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[cl_media_start_date_get]";
                    cmd.Parameters.Add("@id_Media", SqlDbType.NVarChar);
                    cmd.Parameters.Add("@start_date", SqlDbType.SmallDateTime);
                    cmd.Parameters["@start_date"].Direction = ParameterDirection.InputOutput;

                    // SP set Parameters
                    cmd.Parameters["@id_Media"].Value = selectedMedia;
                    cmd.Parameters["@start_date"].Value = DBNull.Value;

                    // SP Execute
                    cmd.ExecuteNonQuery();
                    if (cmd.Parameters["@start_date"].Value == DBNull.Value)
                    {
                        throw (new Exception.DateDALException("Empty start_date from DB"));
                    }
                    else
                    {
                        start_date = Convert.ToDateTime(cmd.Parameters["@start_date"].Value);
                    }

                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new Exception.DateDALException("Impossible to execute query", err));
            }
            #endregion

            return start_date.Year;

        }
        #endregion

        #region GetFirstDayNotEnabled
        /// <summary>
        /// Calculate the first day from which we don't have data in the database
        /// We check for the vehicles list selected (associated to the media type selected) the last publication date in the data table.
        /// if we don't have data, we calculate the previous year according to the startYear variable and the datetime returned is the last day of the previous year.
        /// <remarks>
        /// This method is used in the date selection page to show periods that have not yet been completely uploaded
        /// and this information is represented by using dark gray color in the calendar 
        /// </remarks>
        /// <example>
        /// The sub media selected : press
        /// startYear : 2008
        /// case 1 :
        ///     last publication date : 15/06/2008
        ///     value returned : last publication date + periodicity (daily : periodicity = 1 day, weekly : periodicity = 7 days, monthly : periodicity = 1 month)
        /// case 2 :
        ///     last publication date : not found = no data
        ///     value returned : 31/12/2007
        /// </example>
        /// </summary>
        /// <param name="selectedVehicleList">Media type identifier List</param>
        /// <param name="startYear">Used to calculate the fist day not enable : date format yyyy, example : 2008</param>
        public override DateTime GetFirstDayNotEnabled(List<Int64> selectedVehicleList, int startYear)
        {

            string lastDate_str = string.Empty;
            DateTime lastDate;

            // Get last publication date
            lastDate_str = GetLatestPublication(selectedVehicleList);

            // Calculate the previous year
            startYear--;

            // If there is no data, we return the last day of the previous year
            if (lastDate_str.Length == 0)
                lastDate_str = startYear + "1231";

            lastDate = new DateTime(Convert.ToInt32(lastDate_str.Substring(0, 4)), Convert.ToInt32(lastDate_str.Substring(4, 2)), Convert.ToInt32(lastDate_str.Substring(6, 2)));

            return lastDate.AddDays(1);

        }
        #endregion

        #region GetLastAvailableDate
        /// <summary>
        /// Get the last available date for which we have data in the data base (for the media type selected)
        /// </summary>
        /// <param name="vehicleId">Media Type Identifier</param>
        /// <returns>DataSet containing the result of the SQL request</returns>
        public override DataSet GetLastAvailableDate(Int64 vehicleId)
        {

            DataSet ds = new DataSet();

            #region Execution of the query
            try
            {
                using (SqlConnection conn = (SqlConnection)GetDataSource().GetSource())
                {
                    SqlCommand cmd = conn.CreateCommand();
                    conn.Open();

                    // SP Construction    
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandText = "[dbo].[cl_media_last_date_get]";
                    cmd.Parameters.Add("@id_Media", SqlDbType.SmallInt);

                    // SP set Parameters
                    cmd.Parameters["@id_Media"].Value = Convert.ToInt16(vehicleId);

                    // SP Execute
                    ds.Tables.Add();
                    ds.Tables[0].Columns.Add("availableDate", Type.GetType("System.DateTime"));
                    SqlDataReader dr = cmd.ExecuteReader();
                    while (dr.Read())
                    {
                        ds.Tables[0].Rows.Add(dr[0]);
                    }

                    conn.Close();
                }
            }
            catch (System.Exception err)
            {
                throw (new Exception.DateDALException("Impossible to execute query", err));
            }
            #endregion

            return ds;
        }
        #endregion

        #region Get Media type's last available date  which has data
        /// <summary>
        /// Get Media type's last available date  which has data 
        /// </summary>
        /// <remarks>Use particularly in  Product class analysis modules</remarks>
        /// <param name="idVehicle">Media type ID</param>
        /// <param name="webSession">Session</param>
        /// <returns>Last available month which has data : YYYYMM</returns>
        public override string CheckAvailableDateForMedia(long idVehicle)
        {

            string lastAvailableDate = (DateTime.Now.Year - 2).ToString();

            //TODO : Implements in Russia

            return lastAvailableDate;

        }
        #endregion

        #region Check validity of the period depending on data delivering frequency
        /// <summary>
        /// Check period depending on data delivering frequency
        /// </summary>
        /// <param name="_session">User session</param>
        /// <param name="EndPeriod">End period</param>
        /// <returns>Period End</returns>
        public override string CheckPeriodValidity(WebSession _session, string EndPeriod)
        {
            // TODO FOR RUSSIA SPECIFICATIONS !!!!!!!!!!!!!
            int year = Convert.ToInt32(EndPeriod.Substring(0, 4));
            int month = Convert.ToInt32(EndPeriod.Substring(4, 2));
            DateTime endPeriodDate = (new DateTime(year, month, 1));
            DateTime lastPeriodDate = (new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).AddMonths(-2);

            if (endPeriodDate >= lastPeriodDate)
            {
                return lastPeriodDate.ToString("yyyyMM");
            }

            return endPeriodDate.ToString("yyyyMM");
        }
        #endregion

    }
}

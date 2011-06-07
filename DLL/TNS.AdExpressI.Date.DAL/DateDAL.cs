#region Information
/*
 * Author : Y R'kaina
 * Created on : 09/10/2009
 * Modification:
 *      Author - Date - Description
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Domain.Classification;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Date;


namespace TNS.AdExpressI.Date.DAL {
    /// <summary>
    /// This class provides all the methods to determine specific dates for specific modules.
    /// The specific dates can be the last day in which we have data in the database or the last loaded year. 
    /// </summary>
    public class DateDAL : IDateDAL{

        #region Variables
        /// <summary>
        /// Client session
        /// </summary>
        protected TNS.AdExpress.Web.Core.Sessions.WebSession _session = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DateDAL()
        {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">Customer session</param>
        public DateDAL(WebSession session)
        {
            _session = session;
        }
        #endregion

        #region IDateDAL Membres

        #region GetFirstDayNotEnabled
        /// <summary>
        /// Calculate the first day from which we don't hava data in the database
        /// </summary>
        /// <param name="webSession">The customer session</param>
        /// <param name="selectedVehicle">The madia type selected by the customer</param>
        /// <param name="startYear">Used to calculate the fist day not enable</param>
        public virtual DateTime GetFirstDayNotEnabled(TNS.AdExpress.Web.Core.Sessions.WebSession webSession, long selectedVehicle, int startYear) {

            AtomicPeriodWeek week = new AtomicPeriodWeek(DateTime.Now);
            DateTime firstDayOfWeek = week.FirstDay;
            DateTime publicationDate;
            string lastDate = string.Empty;
            IDataSource dataSource = webSession.Source;

            switch (VehiclesInformation.DatabaseIdToEnum(selectedVehicle)) {
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    lastDate = GetLatestPublication(webSession, selectedVehicle);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(1);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    lastDate = GetLatestPublication(webSession, selectedVehicle);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    firstDayOfWeek = publicationDate.AddDays(7);
                    return firstDayOfWeek;
                case DBClassificationConstantes.Vehicles.names.internet:
                    lastDate = GetLatestPublication(webSession, selectedVehicle);
                    startYear--;
                    if (lastDate.Length == 0) lastDate = startYear + "1231";
                    publicationDate = new DateTime(Convert.ToInt32(lastDate.Substring(0, 4)), Convert.ToInt32(lastDate.Substring(4, 2)), Convert.ToInt32(lastDate.Substring(6, 2)));
                    publicationDate = publicationDate.AddMonths(1);
                    firstDayOfWeek = new DateTime(publicationDate.Year, publicationDate.Month, 1);
                    return firstDayOfWeek;
            }

            return firstDayOfWeek;

        }
        #endregion

        #region GetLastLoadedYear
        /// <summary>
        /// Get the last loaded year in the database for the recap tables (product class analysis modules)
        /// </summary>
        /// <returns>Year</returns>
        public virtual int GetLastLoadedYear()
        {

            #region Tables initilization
            Table recapInfo;

            try {
                recapInfo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.recapInfo);
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Impossible to get table names or schema label", err));
            }
            #endregion

            #region Requête
            string sql = "select current_year from " + recapInfo.Sql + " where id_recap_info = (select max(id_recap_info) from " + recapInfo.Sql + ")";
            #endregion

            #region Execution de la requête
            try {
                IDataSource source = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.productClassAnalysis);
                DataSet ds = source.Fill(sql.ToString());
                return (int.Parse(ds.Tables[0].Rows[0]["current_year"].ToString()));
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Impossible to get the last loaded year in the database for the recap tables", err));
            }
            #endregion

        }
        #endregion

        #region GetLatestPublication
        /// <summary>
        /// Get the last publication date
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <param name="idVehicle">Media typez identifier</param>
        /// <returns>Date</returns>
        public virtual string GetLatestPublication(WebSession webSession, Int64 idVehicle)
        {

            string sql = string.Empty;
            int positionUnivers = 1;
            string mediaList = string.Empty;
            string tableName = string.Empty;
            IDataSource dataSource = webSession.Source;

            #region Construction de la requête

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                case DBClassificationConstantes.Vehicles.names.internet:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInternet, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataPress, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMagazine, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataTv, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataRadio, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.instore:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInStore, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.cinema:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataCinema, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataAdNetTrack, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataEvaliantMobile, webSession.IsSelectRetailerDisplay).Sql;
                    break;
            }

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle)) {
                case DBClassificationConstantes.Vehicles.names.internet:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + tableName;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    sql += " select max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                    sql += " from " + tableName;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                    sql += " select min(last_date) as last_date ";
                    sql += " from (";

                    #region Media selection
                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
                        mediaList += webSession.GetSelection((TreeNode)webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess) + ",";
                    else {
                        while (webSession.CompetitorUniversMedia[positionUnivers] != null) {
                            mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                            positionUnivers++;
                        }
                    }
                    #endregion

                    if (mediaList.Length > 0) {

                        string[] strs = mediaList.Substring(0, mediaList.Length - 1).Split(',');
                        int i = 0;

                        while (i < strs.Length) {
                            if (i > 0) {
                                sql += " UNION ";
                            }

                            sql += " select id_media, max(" + DBConstantes.Fields.DATE_MEDIA_NUM + ") last_date ";
                            sql += " from " + tableName;
                            sql += " where id_media = " + strs[i] + "";
                            sql += " group by id_media ";

                            i += 1;
                        }
                    }

                    sql += " )";
                    break;
            }
            #endregion

            #region Execution de la requête
            try {
                DataSet ds = dataSource.Fill(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err) {
                throw (new Exception.DateDALException("Error while trying to get the last publication date", err));
            }
            #endregion

        }
        #endregion

        #region GetCalendarStartDate
        /// <summary>
        /// Get the calendar starting date
        /// </summary>
        /// <returns>The year corresponding to the starting date : date format yyyy example 2008</returns>
        public virtual int GetCalendarStartDate()
        {
            int nbYears = WebApplicationParameters.DataNumberOfYear;
            if (_session.CurrentModule == WebConstantes.Module.Name.ANALYSE_DYNAMIQUE)
            {
                nbYears = nbYears - 2;
                return DateTime.Now.AddYears(-nbYears).Year;
            }
            else
            {
                nbYears = nbYears - 1;
                return DateTime.Now.AddYears(-nbYears).Year;
            }

        }
        #endregion

        #endregion

    }
}

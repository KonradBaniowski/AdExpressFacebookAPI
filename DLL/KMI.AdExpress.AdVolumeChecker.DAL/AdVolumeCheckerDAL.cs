using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using KMI.AdExpress.AdVolumeChecker.DAL.Exceptions;
using KMI.AdExpress.AdVolumeChecker.Domain;
using Oracle.DataAccess.Client;
using cst = KMI.AdExpress.AdVolumeChecker.Domain.Constantes;

namespace KMI.AdExpress.AdVolumeChecker.DAL {
    /// <summary>
    /// Ad Volume Checker DAL
    /// </summary>
    public class AdVolumeCheckerDAL {

        #region Get Classification Data
        /// <summary>
        /// Get Classification Data
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns></returns>
        public static DataSet GetClassificationLevelItems(string connectionString, cst.ClassificationLevelId classificationLevel, string searchedText) {

            OracleConnection connection = new OracleConnection(connectionString);

            #region Ouverture de la base de données
            bool DBToClosed = false;
            // On teste si la base est déjà ouverte
            if (connection.State == System.Data.ConnectionState.Closed) {
                DBToClosed = true;
                try {
                    connection.Open();
                }
                catch (System.Exception et) {
                    throw (new AdVolumeCheckerDALException("Impossible d'ouvrir la base de données", et));
                }
            }
            #endregion

            StringBuilder sql = new StringBuilder();
            OracleCommand sqlCommand = null;
            searchedText = searchedText.ToUpper();

            switch (classificationLevel) { 
                case cst.ClassificationLevelId.Advertiser:
                    sql.Append(" select id_advertiser as id, advertiser as label ");
                    sql.Append(" from adexpr03.advertiser        ");
                    sql.AppendFormat(" where advertiser like '%{0}%'   ", searchedText.Replace("'", " "));
                    break;
                case cst.ClassificationLevelId.Product:
                    sql.Append(" select id_product as id, product as label ");
                    sql.Append(" from adexpr03.product        ");
                    sql.AppendFormat(" where product like '%{0}%'   ", searchedText.Replace("'", " "));
                    break;
                case cst.ClassificationLevelId.Version:
                    break;
            }

            sql.Append(" and id_language = 33          ");
            sql.Append(" and activation < 50           ");

            sqlCommand = new OracleCommand(sql.ToString());
            sqlCommand.Connection = connection;
            sqlCommand.CommandType = CommandType.Text;

            OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
            DataSet ds = new DataSet();
            adapter.Fill(ds);

            #region Fermeture de la base de données
            try {
                if (sqlCommand != null) sqlCommand.Dispose();
                if (DBToClosed) connection.Close();
            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerDALException("Impossible to close database", et));
            }
            #endregion

            return ds;

        }
        #endregion

        #region Get Data
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns></returns>
        public static DataSet GetData(string connectionString, Int64 mediaId, DateTime startDate, DateTime endDate, string versionList, string advertiserList, string productList, bool isIn) {

            DataSet ds = new DataSet();

            try {
                OracleConnection connection = new OracleConnection(connectionString);
                List<FilterInformation> filterInformationList = FilterInformations.FilterInformationList;

                #region Ouverture de la base de données
                bool DBToClosed = false;
                // On teste si la base est déjà ouverte
                if (connection.State == System.Data.ConnectionState.Closed) {
                    DBToClosed = true;
                    try {
                        connection.Open();
                    }
                    catch (System.Exception et) {
                        throw (new AdVolumeCheckerDALException("Impossible to open database", et));
                    }
                }
                #endregion

                StringBuilder sql = new StringBuilder();
                OracleCommand sqlCommand = null;

                sql.Append(" SELECT   media, id_media,JOUR,TRANCHE_HORAIRE ,sum( duration) as duration FROM ( ");
                sql.Append("    SELECT media, id_media, ");
                sql.Append("        (CASE                                                                 ");
                sql.Append("            WHEN TOP_DIFFUSION  between 030000 AND 035959   THEN '03H - 04H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 040000 AND 045959   THEN '04H - 05H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 050000 AND 055959   THEN '05H - 06H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 060000 AND 065959   THEN '06H - 07H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 070000 AND 075959   THEN '07H - 08H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 080000 AND 085959   THEN '08H - 09H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 090000 AND 095959   THEN '09H - 10H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 100000 AND 105959   THEN '10H - 11H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 110000 AND 115959   THEN '11H - 12H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 120000 AND 125959   THEN '12H - 13H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 130000 AND 135959   THEN '13H - 14H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 140000 AND 145959   THEN '14H - 15H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 150000 AND 155959   THEN '15H - 16H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 160000 AND 165959   THEN '16H - 17H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 170000 AND 175959   THEN '17H - 18H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 180000 AND 185959   THEN '18H - 19H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 190000 AND 195959   THEN '19H - 20H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 200000 AND 205959   THEN '20H - 21H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 210000 AND 215959   THEN '21H - 22H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 220000 AND 225959   THEN '22H - 23H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 230000 AND 235959   THEN '23H - 24H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 240000 AND 240059   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 000001 AND 005959   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 010000 AND 015959   THEN '01H - 02H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                sql.Append("            END) TRANCHE_HORAIRE ");
                sql.Append("        , to_char( to_date(date_media_num,'yyyymmdd'),'DAY') JOUR ");
                sql.Append("        , sum(duration) duration FROM ( ");
                sql.Append("                SELECT am.media,dt.id_media, ");          
                sql.Append("                       date_media_num ");
                sql.Append("                    ,top_diffusion,sum(duration) duration ");
                sql.Append("                FROM ");
                sql.Append("                    ADEXPR03.DATA_TV  dt, adexpr03.all_media_33 am ");
                sql.Append("                WHERE ");
                sql.Append("                    am.id_media = dt.id_media ");
                sql.AppendFormat("                    and date_media_num>={0} and     date_media_num<={1} ", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
                sql.AppendFormat("                    and dt.id_media = {0} ", mediaId);
                if(advertiserList.Length > 0)
                    sql.Append("                    and dt.id_advertiser " + (isIn ? "in" : "not in") + " ("+ advertiserList +") ");
                if(productList.Length > 0)
                    sql.Append("                    and dt.id_product " + (isIn ? "in" : "not in") + " (" + productList + ") ");
                if (versionList.Length > 0)
                    sql.Append("                    and dt.id_slogan " + (isIn ? "in" : "not in") + " (" + versionList + ") ");

                foreach (FilterInformation filter in filterInformationList) {
                    sql.AppendFormat("                    and dt.{0} {1} ({2}) ", filter.Field, filter.Operator, filter.Ids);
                }

                sql.Append("                GROUP BY media, dt.id_media,  date_media_num,top_diffusion order by  media, dt.id_media,  date_media_num ,top_diffusion ) ");
                sql.AppendFormat("    WHERE  date_media_num>={0} and    date_media_num<={1} ", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
                sql.Append("    GROUP BY media, id_media,date_media_num,top_diffusion ORDER BY  media, id_media,JOUR  ,TRANCHE_HORAIRE ) ");
                sql.Append(" GROUP BY  media, id_media,JOUR,TRANCHE_HORAIRE ORDER BY  media, id_media,JOUR ,TRANCHE_HORAIRE ");

                sqlCommand = new OracleCommand(sql.ToString());
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;

                OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
                adapter.Fill(ds);

                #region Fermeture de la base de données
                try {
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et) {
                    throw (new AdVolumeCheckerDALException("Impossible to close database", et));
                }
                #endregion
            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerDALException("Error while getting data from database (Method GetData) : " + et.Message, et));
            }

            return ds;

        }
        #endregion

        #region Get Data By Top Diffusion
        /// <summary>
        /// Get Data By Top Diffusion
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns></returns>
        public static DataSet GetDataByTopDiffusion(string connectionString, Int64 mediaId, DateTime startDate, DateTime endDate, string versionList, string advertiserList, string productList, bool isIn) {

            DataSet ds = new DataSet();

            try {
                OracleConnection connection = new OracleConnection(connectionString);
                List<FilterInformation> filterInformationList = FilterInformations.FilterInformationList;

                #region Ouverture de la base de données
                bool DBToClosed = false;
                // On teste si la base est déjà ouverte
                if (connection.State == System.Data.ConnectionState.Closed) {
                    DBToClosed = true;
                    try {
                        connection.Open();
                    }
                    catch (System.Exception et) {
                        throw (new AdVolumeCheckerDALException("Impossible to open database", et));
                    }
                }
                #endregion

                StringBuilder sql = new StringBuilder();
                OracleCommand sqlCommand = null;

                sql.Append("    SELECT media, id_media, ");
                sql.Append("        (CASE                                                                 ");
                sql.Append("            WHEN TOP_DIFFUSION  between 030000 AND 035959   THEN '03H - 04H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 040000 AND 045959   THEN '04H - 05H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 050000 AND 055959   THEN '05H - 06H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 060000 AND 065959   THEN '06H - 07H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 070000 AND 075959   THEN '07H - 08H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 080000 AND 085959   THEN '08H - 09H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 090000 AND 095959   THEN '09H - 10H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 100000 AND 105959   THEN '10H - 11H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 110000 AND 115959   THEN '11H - 12H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 120000 AND 125959   THEN '12H - 13H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 130000 AND 135959   THEN '13H - 14H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 140000 AND 145959   THEN '14H - 15H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 150000 AND 155959   THEN '15H - 16H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 160000 AND 165959   THEN '16H - 17H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 170000 AND 175959   THEN '17H - 18H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 180000 AND 185959   THEN '18H - 19H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 190000 AND 195959   THEN '19H - 20H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 200000 AND 205959   THEN '20H - 21H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 210000 AND 215959   THEN '21H - 22H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 220000 AND 225959   THEN '22H - 23H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 230000 AND 235959   THEN '23H - 24H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 240000 AND 240059   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 000001 AND 005959   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 010000 AND 015959   THEN '01H - 02H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                sql.Append("            END) TRANCHE_HORAIRE ");
                sql.Append("        , to_char( to_date(date_media_num,'yyyymmdd'),'DAY') JOUR, top_diffusion ");
                sql.Append("        , sum(duration) duration FROM ( ");
                sql.Append("                SELECT am.media,dt.id_media, date_media_num");
                sql.Append("                    ,top_diffusion,sum(duration) duration ");
                sql.Append("                FROM ");
                sql.Append("                    ADEXPR03.DATA_TV  dt, adexpr03.all_media_33 am ");
                sql.Append("                WHERE ");
                sql.Append("                    am.id_media = dt.id_media ");
                sql.AppendFormat("                    and date_media_num>={0} and     date_media_num<={1} ", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
                sql.AppendFormat("                    and dt.id_media = {0} ", mediaId);
                if (advertiserList.Length > 0)
                    sql.Append("                    and dt.id_advertiser " + (isIn ? "in" : "not in") + " (" + advertiserList + ") ");
                if (productList.Length > 0)
                    sql.Append("                    and dt.id_product " + (isIn ? "in" : "not in") + " (" + productList + ") ");
                if (versionList.Length > 0)
                    sql.Append("                    and dt.id_slogan " + (isIn ? "in" : "not in") + " (" + versionList + ") ");

                foreach (FilterInformation filter in filterInformationList) {
                    sql.AppendFormat("                    and dt.{0} {1} ({2}) ", filter.Field, filter.Operator, filter.Ids);
                }

                sql.Append("                GROUP BY media, dt.id_media,  date_media_num,top_diffusion order by  media, dt.id_media,  date_media_num ,top_diffusion ) ");
                sql.AppendFormat("    WHERE  date_media_num>={0} and date_media_num<={1} ", startDate.ToString("yyyyMMdd"), endDate.ToString("yyyyMMdd"));
                sql.Append("    GROUP BY media, id_media,date_media_num,top_diffusion ORDER BY  media, id_media,JOUR, TRANCHE_HORAIRE, top_diffusion ");

                sqlCommand = new OracleCommand(sql.ToString());
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;

                OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
                adapter.Fill(ds);

                #region Fermeture de la base de données
                try {
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et) {
                    throw (new AdVolumeCheckerDALException("Impossible to close database", et));
                }
                #endregion
            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerDALException("Error while getting data from database (Method GetData) : " + et.Message, et));
            }

            return ds;

        }
        #endregion

        #region Get Previous Last Top Diffusion Data
        /// <summary>
        /// Get Previous Last Top Diffusion Data
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns></returns>
        public static DataSet GetPreviousLastTopDiffusionData(string connectionString, Int64 mediaId, DateTime startDate, DateTime endDate, string versionList, string advertiserList, string productList, bool isIn) {

            DataSet ds = new DataSet();

            try {
                OracleConnection connection = new OracleConnection(connectionString);
                List<FilterInformation> filterInformationList = FilterInformations.FilterInformationList;

                #region Ouverture de la base de données
                bool DBToClosed = false;
                // On teste si la base est déjà ouverte
                if (connection.State == System.Data.ConnectionState.Closed) {
                    DBToClosed = true;
                    try {
                        connection.Open();
                    }
                    catch (System.Exception et) {
                        throw (new AdVolumeCheckerDALException("Impossible to open database", et));
                    }
                }
                #endregion

                StringBuilder sql = new StringBuilder();
                OracleCommand sqlCommand = null;

                sql.Append(" SELECT media, dt.id_media, ");
                sql.Append("  (CASE                                                                 ");
                sql.Append("   WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                sql.Append("   END) TRANCHE_HORAIRE ");
                sql.Append("  , to_char( to_date(date_media_num,'yyyymmdd'),'DAY') JOUR, top_diffusion ");
                sql.Append("  , sum(duration) duration ");
                sql.Append(" FROM ADEXPR03.DATA_TV  dt, adexpr03.all_media_33 am ");
                sql.Append(" WHERE am.id_media = dt.id_media ");
                sql.AppendFormat(" and date_media_num={0} ", startDate.AddDays(-1).ToString("yyyyMMdd"));
                sql.AppendFormat(" and dt.id_media = {0} ", mediaId);
                sql.Append(" and top_diffusion  between 020000 AND 025959 ");
                if (advertiserList.Length > 0)
                    sql.Append("                    and dt.id_advertiser " + (isIn ? "in" : "not in") + " (" + advertiserList + ") ");
                if (productList.Length > 0)
                    sql.Append("                    and dt.id_product " + (isIn ? "in" : "not in") + " (" + productList + ") ");
                if (versionList.Length > 0)
                    sql.Append("                    and dt.id_slogan " + (isIn ? "in" : "not in") + " (" + versionList + ") ");

                foreach (FilterInformation filter in filterInformationList) {
                    sql.AppendFormat("                    and dt.{0} {1} ({2}) ", filter.Field, filter.Operator, filter.Ids);
                }
                sql.Append("    GROUP BY media, dt.id_media,date_media_num,top_diffusion ORDER BY  media, dt.id_media,JOUR, TRANCHE_HORAIRE, top_diffusion ");

                sqlCommand = new OracleCommand(sql.ToString());
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;

                OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
                adapter.Fill(ds);

                #region Fermeture de la base de données
                try {
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et) {
                    throw (new AdVolumeCheckerDALException("Impossible to close database", et));
                }
                #endregion
            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerDALException("Error while getting data from database (Method GetData) : " + et.Message, et));
            }

            return ds;

        }
        #endregion

        #region Get Detail Data
        /// <summary>
        /// Get Data
        /// </summary>
        /// <param name="connectionString">Connection String</param>
        /// <returns></returns>
        public static DataSet GetDetailData(string connectionString, Int64 mediaId, DateTime date, string slot, string versionList, string advertiserList, string productList, bool isIn) {

            DataSet ds = new DataSet();

            try {
                OracleConnection connection = new OracleConnection(connectionString);
                List<FilterInformation> filterInformationList = FilterInformations.FilterInformationList;

                #region Ouverture de la base de données
                bool DBToClosed = false;
                // On teste si la base est déjà ouverte
                if (connection.State == System.Data.ConnectionState.Closed) {
                    DBToClosed = true;
                    try {
                        connection.Open();
                    }
                    catch (System.Exception et) {
                        throw (new AdVolumeCheckerDALException("Impossible d'ouvrir la base de données", et));
                    }
                }
                #endregion

                StringBuilder sql = new StringBuilder();
                OracleCommand sqlCommand = null;

                sql.Append(" select distinct id_slogan, date_media_num, DATE_MEDIA_REAL,ad.id_advertiser, ad.advertiser, md.id_media, md.media, pr.id_product, pr.product, sc.id_sector, sc.sector, gr.id_group_, gr.group_, top_diffusion, duration, id_commercial_break, id_rank, duration_commercial_break, number_message_commercial_brea, wp.id_category as idCategory, advertising_agency ");
                sql.Append("       , (CASE                                                                ");
                sql.Append("            WHEN TOP_DIFFUSION  between 030000 AND 035959   THEN '03H - 04H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 040000 AND 045959   THEN '04H - 05H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 050000 AND 055959   THEN '05H - 06H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 060000 AND 065959   THEN '06H - 07H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 070000 AND 075959   THEN '07H - 08H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 080000 AND 085959   THEN '08H - 09H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 090000 AND 095959   THEN '09H - 10H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 100000 AND 105959   THEN '10H - 11H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 110000 AND 115959   THEN '11H - 12H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 120000 AND 125959   THEN '12H - 13H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 130000 AND 135959   THEN '13H - 14H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 140000 AND 145959   THEN '14H - 15H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 150000 AND 155959   THEN '15H - 16H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 160000 AND 165959   THEN '16H - 17H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 170000 AND 175959   THEN '17H - 18H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 180000 AND 185959   THEN '18H - 19H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 190000 AND 195959   THEN '19H - 20H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 200000 AND 205959   THEN '20H - 21H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 210000 AND 215959   THEN '21H - 22H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 220000 AND 225959   THEN '22H - 23H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 230000 AND 235959   THEN '23H - 24H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 240000 AND 240059   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 000001 AND 005959   THEN '24H - 01H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 010000 AND 015959   THEN '01H - 02H'  ");
                sql.Append("            WHEN TOP_DIFFUSION  between 020000 AND 025959   THEN '02H - 03H'  ");
                sql.Append("            END) TRANCHE_HORAIRE, to_char( to_date(date_media_num,'yyyymmdd'),'DAY') JOUR ");
                sql.Append(" from adexpr03.data_tv wp, adexpr03.advertising_agency adva, adexpr03.advertiser ad, adexpr03.media md, adexpr03.product pr, adexpr03.sector sc, adexpr03.group_ gr ");
                sql.AppendFormat(" Where  wp.id_media={0}  ", mediaId);
                              
                    sql.AppendFormat(" and wp.date_media_num={0} ", date.ToString("yyyyMMdd"));
                sql.Append(GetTopDiffusionSQL(slot));
                sql.Append(" and wp.id_vehicle = 3 ");
                sql.Append(" and ad.id_advertiser=wp.id_advertiser and ad.id_language=33 and ad.activation<50 ");
                sql.Append(" and md.id_media=wp.id_media and md.id_language=33 and md.activation<50 ");
                sql.Append(" and pr.id_product=wp.id_product and pr.id_language=33 and pr.activation<50 ");
                sql.Append(" and sc.id_sector=wp.id_sector and sc.id_language=33 and sc.activation<50 ");
                sql.Append(" and gr.id_group_=wp.id_group_ and gr.id_language=33 and gr.activation<50 ");
                if (advertiserList.Length > 0)
                    sql.Append("                    and wp.id_advertiser " + (isIn ? "in" : "not in") + " (" + advertiserList + ") ");
                if (productList.Length > 0)
                    sql.Append("                    and wp.id_product " + (isIn ? "in" : "not in") + " (" + productList + ") ");
                if(versionList.Length > 0)
                    sql.Append("                    and wp.id_slogan " + (isIn ? "in" : "not in") + " (" + versionList + ") ");

                foreach (FilterInformation filter in filterInformationList) {
                    sql.AppendFormat("                    and dt.{0} {1} ({2}) ", filter.Field, filter.Operator, filter.Ids);
                }

                sql.Append(" and adva.id_advertising_agency(+)=wp.id_advertising_agency  and adva.id_language(+)=33  and adva.activation(+)<50  ");
                sql.Append(" group by id_slogan,date_media_num,DATE_MEDIA_REAL,ad.id_advertiser,ad.advertiser,md.id_media,md.media,pr.id_product,pr.product,sc.id_sector,sc.sector,gr.id_group_,gr.group_,top_diffusion,duration,id_commercial_break,id_rank,duration_commercial_break,number_message_commercial_brea,wp.id_category , advertising_agency   ");
                sql.Append(" order by wp.date_media_num,DATE_MEDIA_REAL,wp.top_diffusion ");

                sqlCommand = new OracleCommand(sql.ToString());
                sqlCommand.Connection = connection;
                sqlCommand.CommandType = CommandType.Text;

                OracleDataAdapter adapter = new OracleDataAdapter(sqlCommand);
                adapter.Fill(ds);

                #region Fermeture de la base de données
                try {
                    if (sqlCommand != null) sqlCommand.Dispose();
                    if (DBToClosed) connection.Close();
                }
                catch (System.Exception et) {
                    throw (new AdVolumeCheckerDALException("Impossible to close database", et));
                }
                #endregion

            }
            catch (System.Exception et) {
                throw (new AdVolumeCheckerDALException("Error while getting data from database (Method GetDetailData) : " + et.Message, et));
            }

            return ds;

        }
        #endregion

        #region Get Top Diffusion SQL
        private static string GetTopDiffusionSQL(string slot) {

            switch (slot) {
                case "03H - 04H": return "and wp.top_diffusion between 030000 AND 035959 ";
                case "04H - 05H": return "and wp.top_diffusion between 040000 AND 045959 ";
                case "05H - 06H": return "and wp.top_diffusion between 050000 AND 055959 ";
                case "06H - 07H": return "and wp.top_diffusion between 060000 AND 065959 ";
                case "07H - 08H": return "and wp.top_diffusion between 070000 AND 075959 ";
                case "08H - 09H": return "and wp.top_diffusion between 080000 AND 085959 ";
                case "09H - 10H": return "and wp.top_diffusion between 090000 AND 095959 ";
                case "10H - 11H": return "and wp.top_diffusion between 100000 AND 105959 ";
                case "11H - 12H": return "and wp.top_diffusion between 110000 AND 115959 ";
                case "12H - 13H": return "and wp.top_diffusion between 120000 AND 125959 ";
                case "13H - 14H": return "and wp.top_diffusion between 130000 AND 135959 ";
                case "14H - 15H": return "and wp.top_diffusion between 140000 AND 145959 ";
                case "15H - 16H": return "and wp.top_diffusion between 150000 AND 155959 ";
                case "16H - 17H": return "and wp.top_diffusion between 160000 AND 165959 ";
                case "17H - 18H": return "and wp.top_diffusion between 170000 AND 175959 ";
                case "18H - 19H": return "and wp.top_diffusion between 180000 AND 185959 ";
                case "19H - 20H": return "and wp.top_diffusion between 190000 AND 195959 ";
                case "20H - 21H": return "and wp.top_diffusion between 200000 AND 205959 ";
                case "21H - 22H": return "and wp.top_diffusion between 210000 AND 215959 ";
                case "22H - 23H": return "and wp.top_diffusion between 220000 AND 225959 ";
                case "23H - 24H": return "and wp.top_diffusion between 230000 AND 235959 ";
                case "24H - 01H": return "and (wp.top_diffusion between 240000 AND 240059 or wp.top_diffusion between 000001 AND 005959 )";
                case "01H - 02H": return "and wp.top_diffusion between 010000 AND 015959 ";
                case "02H - 03H": return "and wp.top_diffusion between 020000 AND 025959 ";
                default: return "";

            }

        }
        #endregion

    }
}


using System;
using System.Data;
using System.Text;
using System.Windows.Forms;
using Oracle.DataAccess.Client;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.DB.Common;
using DBSchema = TNS.AdExpress.Constantes.DB.Schema;
using DBTables = TNS.AdExpress.Constantes.DB.Tables;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using CustormerConstantes = TNS.AdExpress.Constantes.Customer;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using TNS.AdExpress.Domain.Translation;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Web.Core.Utilities;

namespace TNS.AdExpress.Web.Core.DataAccess.Selections.Medias
{
    /// <summary>
    /// Description résumée de MediaPublicationDatesDataAccess.
    /// </summary>
    public class MediaPublicationDatesDataAccess
    {
        #region Get all the publications for the selected media 
        /// <summary>
        ///  Calculates and returns the dataset for the Media Plan 	 
        /// </summary>
        /// <param name="dateBegin">Starting date</param>
        /// <param name="dateEnd">Ending date</param>		
        /// <returns>dataset with all the publications of media within the defined period</returns>
        internal static DataSet GetAllPublications(int dateBegin, int dateEnd)
        {
            #region variables
            StringBuilder sql = new StringBuilder(350);
            IDataSource dataSource = WebApplicationParameters.DataBaseDescription.GetDefaultConnection(DefaultConnectionIds.publication);
            #endregion

            #region Construction of the query

            #region Select
            sql.Append(" select distinct ");
            sql.Append("id_media,date_media_num as publication_date");
            #endregion

            #region From
            sql.Append(" from ");
            sql.Append(WebApplicationParameters.GetDataTable(TableIds.dataPressAPPM, false).SqlWithPrefix);
            #endregion

            #region Where
            sql.Append(" where ");
            sql.Append("date_media_num >=" + dateBegin + " and date_media_num<=" + dateEnd);
            #endregion

            #region Order by
            sql.Append(" order by ");
            sql.Append("id_media,date_media_num");
            #endregion

            #endregion

            #region Execution of the query
            try
            {
                return (dataSource.Fill(sql.ToString()));
            }
            catch (System.Exception err)
            {
                throw (new Exception("GetAllPublications:: Error while executing the query for the Media Publication Data Access ", err));
            }
            #endregion

        }
        #endregion

        /// <summary>
        /// Get Internet last month publication date
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="periodType">Type de période</param>
        /// <param name="moduleType">Type de module</param>
        /// <param name="idVehicle">ID média</param>
        /// <returns></returns>
        public static string GetLatestPublication(WebSession webSession, WebConstantes.CustomerSessions.Period.DisplayLevel periodType, WebConstantes.Module.Type moduleType, Int64 idVehicle)
        {

            string sql = "";

            #region Construction de la requête
            sql += " select max(" + SQLGenerator.GetDateFieldName(moduleType, periodType) + ") last_date ";
            sql += " from " + WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label + ".";

            switch (periodType)
            {
                case WebConstantes.CustomerSessions.Period.DisplayLevel.yearly:
                case WebConstantes.CustomerSessions.Period.DisplayLevel.monthly:
                    sql += WebApplicationParameters.GetDataTable(TableIds.monthData, webSession.IsSelectRetailerDisplay).Label;
                    break;
                case WebConstantes.CustomerSessions.Period.DisplayLevel.weekly:
                    sql += WebApplicationParameters.GetDataTable(TableIds.weekData, webSession.IsSelectRetailerDisplay).Label;
                    break;
                default:
                    throw (new Exception("Le détails période sélectionné est incorrect pour le choix de la table"));
            }
            #endregion
            sql += "  where id_vehicle=" + idVehicle;

            #region Execution de la requête
            try
            {
                DataSet ds = webSession.Source.Fill(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err)
            {
                throw (new Exception("Erreur dans la récupération de la date de dernière parution d'un média", err));
            }
            #endregion

        }
        /// <summary>
        /// Get last publication date
        /// </summary>
        /// <param name="webSession">Session client</param>
        /// <param name="idVehicle">Id média</param>
		/// <param name="dataSource">Data source</param>
        /// <returns>Date</returns>
        public static string GetLatestPublication(WebSession webSession, Int64 idVehicle, IDataSource dataSource)
        {

            string sql = string.Empty;
            int positionUnivers = 1;
            string mediaList = string.Empty;
            string tableName = string.Empty;

            #region Construction de la requête

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle))
            {
                case DBClassificationConstantes.Vehicles.names.internet:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInternet, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMarketingDirect, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMail, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataPress, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.others:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataTv, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.radio:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataRadio, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.dooh:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataOutDoor, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.instore:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInStore, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.indoor:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataInDoor, webSession.IsSelectRetailerDisplay).Sql;
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
                case DBClassificationConstantes.Vehicles.names.newspaper:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataNewspaper, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.magazine:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMagazine, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.mms:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataMms, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.search:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataSearch, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.social:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataSocial, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.audioDigital:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataAudioDigital, webSession.IsSelectRetailerDisplay).Sql;
                    break;
                case DBClassificationConstantes.Vehicles.names.paidSocial:
                    tableName = WebApplicationParameters.GetDataTable(TableIds.dataPaidSocial, webSession.IsSelectRetailerDisplay).Sql;
                    break;

            }

            switch (VehiclesInformation.DatabaseIdToEnum(idVehicle))
            {
                case DBClassificationConstantes.Vehicles.names.internet:
                case DBClassificationConstantes.Vehicles.names.directMarketing:
                case DBClassificationConstantes.Vehicles.names.mailValo:
                    sql += string.Format(" select max({0}) last_date ", DBConstantes.Fields.DATE_MEDIA_NUM);
                    sql += string.Format(" from {0}", tableName);
                    break;
                case DBClassificationConstantes.Vehicles.names.press:
                case DBClassificationConstantes.Vehicles.names.newspaper:
                case DBClassificationConstantes.Vehicles.names.magazine:
                case DBClassificationConstantes.Vehicles.names.internationalPress:
                case DBClassificationConstantes.Vehicles.names.radio:
                case DBClassificationConstantes.Vehicles.names.radioGeneral:
                case DBClassificationConstantes.Vehicles.names.radioSponsorship:
                case DBClassificationConstantes.Vehicles.names.radioMusic:
                case DBClassificationConstantes.Vehicles.names.tv:
                case DBClassificationConstantes.Vehicles.names.tvGeneral:
                case DBClassificationConstantes.Vehicles.names.tvSponsorship:
                case DBClassificationConstantes.Vehicles.names.tvAnnounces:
                case DBClassificationConstantes.Vehicles.names.tvNonTerrestrials:
                case DBClassificationConstantes.Vehicles.names.others:
                case DBClassificationConstantes.Vehicles.names.outdoor:
                case DBClassificationConstantes.Vehicles.names.dooh:
                case DBClassificationConstantes.Vehicles.names.indoor:
                case DBClassificationConstantes.Vehicles.names.instore:
                case DBClassificationConstantes.Vehicles.names.cinema:
                case DBClassificationConstantes.Vehicles.names.adnettrack:
                case DBClassificationConstantes.Vehicles.names.evaliantMobile:
                case DBClassificationConstantes.Vehicles.names.czinternet:
                case DBClassificationConstantes.Vehicles.names.mms:
                case DBClassificationConstantes.Vehicles.names.search:
                case DBClassificationConstantes.Vehicles.names.social:
                case DBClassificationConstantes.Vehicles.names.audioDigital:
                case DBClassificationConstantes.Vehicles.names.paidSocial:
                    sql += " select min(last_date) as last_date ";
                    sql += " from (";

                    #region Media selection
                    if (webSession.CurrentModule == WebConstantes.Module.Name.ANALYSE_PORTEFEUILLE)
                        mediaList += webSession.GetSelection((TreeNode)webSession.ReferenceUniversMedia, CustormerConstantes.Right.type.mediaAccess) + ",";
                    else
                    {
                        while (webSession.CompetitorUniversMedia[positionUnivers] != null)
                        {
                            mediaList += webSession.GetSelection((TreeNode)webSession.CompetitorUniversMedia[positionUnivers], CustormerConstantes.Right.type.mediaAccess) + ",";
                            positionUnivers++;
                        }
                    }
                    #endregion

                    if (mediaList.Length > 0)
                    {

                        string[] strs = mediaList.Substring(0, mediaList.Length - 1).Split(',');
                        int i = 0;

                        while (i < strs.Length)
                        {
                            if (i > 0)
                            {
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
            try
            {
                DataSet ds = dataSource.Fill(sql);
                if (ds != null && ds.Tables[0].Rows.Count > 0)
                    return (ds.Tables[0].Rows[0]["last_date"].ToString());
                return null;
            }
            catch (System.Exception err)
            {
                throw (new Exception("Erreur dans la récupération de la date de dernière parution d'un média", err));
            }
            #endregion

        }

    }
}


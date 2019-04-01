using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;


namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class CalendarEngine : DAL.Engines.CalendarEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        /// <param name="module">Module</param>
        public CalendarEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region GetGad
        protected override void GetGad(ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = "";
             dataFieldsForGad = "";
            dataJointForGad = "";
        }
        #endregion

        #region Global Query in case of per day/week/month option
        protected override string GetDateMediaNumFieldWithAlias()
        {
            switch (_webSession.DetailPeriod)
            {
                case CustomerSessions.Period.DisplayLevel.dayly:
                    return DBConstantes.Fields.DATE_MEDIA_NUM + " as \"date_media_num\"";
                case CustomerSessions.Period.DisplayLevel.weekly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYWW') as \"date_media_num\"";
                case CustomerSessions.Period.DisplayLevel.monthly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYMM') as \"date_media_num\"";
                default:
                    return "";
            }
        }
        protected override string GetDateMediaNumField()
        {
            switch (_webSession.DetailPeriod)
            {
                case CustomerSessions.Period.DisplayLevel.dayly:
                    return DBConstantes.Fields.DATE_MEDIA_NUM;
                case CustomerSessions.Period.DisplayLevel.weekly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYWW')";
                case CustomerSessions.Period.DisplayLevel.monthly:
                    return "to_char(to_date(date_media_num,'YYYYMMDD'),'YYYYMM')";
                default:
                    return "";
            }
        }
        #endregion

        protected override long CountDataRows()
        {
            #region Variables
            string dataTableName = "";
            string dataTableNameForGad = "";
            string dataFieldsForGad = "";
            string dataJointForGad = "";
            string detailProductTablesNames = "";
            string detailProductFields = "";
            string detailProductJoints = "";
            string detailProductOrderBy = "";
            string unitFieldNameSumWithAlias = "";
            string productsRights = "";
            string sql = "";
            string list = "";
            //int positionUnivers=1;
            string mediaList = "";
            bool premier;
            string dataJointForInsert = "";
            string listProductHap = "";
            string mediaRights = "";
            string mediaAgencyTable = string.Empty;
            string mediaAgencyJoins = string.Empty;

            string dataGroupby = "";
            long nbRows = 0;
            #endregion

            #region Construction de la requête

            dataTableName = GetQueryFields(dataTableName, ref detailProductTablesNames, ref detailProductFields, ref detailProductJoints,
                ref unitFieldNameSumWithAlias, ref dataGroupby, ref mediaRights, ref productsRights, ref detailProductOrderBy,
                ref dataJointForInsert, ref listProductHap, ref dataTableNameForGad, ref dataFieldsForGad, ref dataJointForGad);

            sql += " select count(*) as NbROWS from ( "; //start count

            sql += " select " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad + "," + unitFieldNameSumWithAlias;
            sql += ", " + GetDateMediaNumFieldWithAlias() + "";
            sql += " from " + mediaAgencyTable + dataTableName;
            if (detailProductTablesNames.Length > 0)
                sql += ", " + detailProductTablesNames;
            sql += " " + dataTableNameForGad;
            // Période
            sql += " where " + DBConstantes.Fields.DATE_MEDIA_NUM + " >=" + _beginingDate;
            sql += " and " + DBConstantes.Fields.DATE_MEDIA_NUM + " <=" + _endDate;

            // Autopromo
            string idMediaLabel = string.Empty;

            if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                idMediaLabel = "id_media_evaliant";
            else if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms)
                idMediaLabel = "id_media_mms";

            if (_vehicleInformation.Autopromo && (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile
                || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.mms))
            {

                Table tblAutoPromo = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.autoPromo);

                if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoAdvertiser)
                    sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".auto_promotion = 0 ";
                else if (_webSession.AutoPromo == WebConstantes.CustomerSessions.AutoPromo.exceptAutoPromoHoldingCompany)
                {
                    sql += " and (" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, "
                        + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_holding_company) not in ( ";
                    sql += " select distinct " + idMediaLabel + ", id_holding_company ";
                    sql += " from " + tblAutoPromo.Sql + " ";
                    sql += " where " + idMediaLabel + " is not null ";
                    sql += " ) ";
                }
            }

            sql += GetFormatClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            sql += GetPurchaseModeClause(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            // Jointures Produits
            sql += " " + detailProductJoints;
            sql += " " + dataJointForGad;
            sql += " " + mediaAgencyJoins;
            //Jointures encart
            if (DBClassificationConstantes.Vehicles.names.press == _vehicleInformation.Id
                || DBClassificationConstantes.Vehicles.names.internationalPress == _vehicleInformation.Id
                || DBClassificationConstantes.Vehicles.names.newspaper == _vehicleInformation.Id
                || DBClassificationConstantes.Vehicles.names.magazine == _vehicleInformation.Id
                )
                sql += " " + dataJointForInsert;

            #region Sélection de Médias
            sql += GetMediaSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            #endregion

            #region Sélection de Produits
            sql += " " + GetProductData();
            #endregion

            //Media Universe
            sql += " " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

            #region Sélection support
            // media
            sql += " " + GetMediaSelection(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
            #endregion

            // Media rights
            sql += " " + productsRights;
            // Products rights
            sql += mediaRights;
            sql += listProductHap;
            // Group by
            sql += " group by " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media, " + detailProductFields + dataFieldsForGad;
            sql += "," + GetDateMediaNumField() + "";

            sql += dataGroupby; // hashcode pour Evaliant

            // Order by
            sql += " order by " + detailProductOrderBy + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media";
            sql += "," + GetDateMediaNumField() + "";

            sql += " ) "; //end count
            #endregion

            #region Execution de la requête
            try
            {
                var ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                    nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));
          
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to load data for  calendar of advertising activity : " + sql, err));
            }
            #endregion

            return nbRows;

        }

    }
}

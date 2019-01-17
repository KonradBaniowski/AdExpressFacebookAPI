using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using WebConstantes = TNS.AdExpress.Constantes.Web;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class InsertionDetailEngine : DAL.Engines.InsertionDetailEngine
    {
        #region Variables
        /// <summary>
        /// Time Slot
        /// </summary>
        protected string _timeSlot;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, string timeSlot)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
            _timeSlot = timeSlot;
        }
        #endregion

        #region ComputeData
        /// <summary>
        /// Get data for media detail insertion
        /// </summary>	
        /// <returns>liste des publicités pour un média donné</returns>
        protected override DataSet ComputeData()
        {

            #region Variables
            var sql = new StringBuilder(5000);
            bool allPeriod = string.IsNullOrEmpty(_timeSlot);
            #endregion

            try
            {
                string sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _webSession.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
                string tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert,
                    _webSession.IsSelectRetailerDisplay);
                string sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
                string sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

                //Select
                sql.Append(" select distinct");
                if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.Append(" , advertising_agency");
                    sqlGroupBy += " , advertising_agency";
                }

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

                sql.Append(" from ");
                sql.AppendFormat(" {0} ", tableName);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                    sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

                if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Joints conditions
                sql.Append(" Where ");

                sql.AppendFormat(" {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
                sql.AppendFormat(" and {0}.date_media_num>={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
                sql.AppendFormat(" and {0}.date_media_num<={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

                if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
                    sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
                sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv && !string.IsNullOrEmpty(_timeSlot))
                {
                    sql.AppendFormat(" and {0}", GetTopDiffusionFilter(_timeSlot));
                }

                string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                string product = GetProductData();
                string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                 WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                string orderby = GetOrderByDetailMedia(allPeriod);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.AppendFormat(" and {0}.id_advertising_agency(+)={1}.id_advertising_agency ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql.AppendFormat(" and {0}.id_language(+)={1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix, _webSession.DataLanguage);
                    sql.AppendFormat(" and {0}.activation(+)<{1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                }

                #region Droits
                //liste des produit hap
                sql.Append(listProductHap);
                sql.Append(product);
                sql.Append(productsRights);
                sql.Append(mediaRights);

                //Media Universe
                sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                //Rights detail spot to spot TNT
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
              && !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                {
                    string idTntCategory = TNS.AdExpress.Domain.Lists
                        .GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.digitalTv);
                    if (!string.IsNullOrEmpty(idTntCategory))
                    {
                        sql.AppendFormat(" and {0}.id_category not in ({1})  ", WebApplicationParameters
                       .DataBaseDescription.DefaultResultTablePrefix, idTntCategory);
                    }
                }

                #endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


                // Order by
                sql.AppendFormat("  {0}", orderby);

            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
            }

            #region Query execution
            try
            {
                return _webSession.Source.Fill(sql.ToString());
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to exectute query of  media detail : {0}",
                    sql.ToString()), err));
            }
            #endregion

        }
        #endregion

        #region Get Top Diffusion Filter
        /// <summary>
        /// Get Top Diffusion Filter
        /// </summary>
        /// <returns>Filter</returns>
        private string GetTopDiffusionFilter(string timeSlot)
        {
            switch (timeSlot)
            {
                case "00H - 01H":
                    return " top_diffusion >= 240000 and top_diffusion <= 245959 ";
                case "01H - 02H":
                    return " top_diffusion >= 250000 and top_diffusion <= 255959 ";
                case "02H - 03H":
                    return " top_diffusion >= 020000 and top_diffusion <= 025959 ";
                case "03H - 04H":
                    return " top_diffusion >= 030000 and top_diffusion <= 035959 ";
                case "04H - 05H":
                    return " top_diffusion >= 040000 and top_diffusion <= 045959 ";
                case "05H - 06H":
                    return " top_diffusion >= 050000 and top_diffusion <= 055959 ";
                case "06H - 07H":
                    return " top_diffusion >= 060000 and top_diffusion <= 065959 ";
                case "07H - 08H":
                    return " top_diffusion >= 070000 and top_diffusion <= 075959 ";
                case "08H - 09H":
                    return " top_diffusion >= 080000 and top_diffusion <= 085959 ";
                case "09H - 10H":
                    return " top_diffusion >= 090000 and top_diffusion <= 095959 ";
                case "10H - 11H":
                    return " top_diffusion >= 100000 and top_diffusion <= 105959 ";
                case "11H - 12H":
                    return " top_diffusion >= 110000 and top_diffusion <= 115959 ";
                case "12H - 13H":
                    return " top_diffusion >= 120000 and top_diffusion <= 125959 ";
                case "13H - 14H":
                    return " top_diffusion >= 130000 and top_diffusion <= 135959 ";
                case "14H - 15H":
                    return " top_diffusion >= 140000 and top_diffusion <= 145959 ";
                case "15H - 16H":
                    return " top_diffusion >= 150000 and top_diffusion <= 155959 ";
                case "16H - 17H":
                    return " top_diffusion >= 160000 and top_diffusion <= 165959 ";
                case "17H - 18H":
                    return " top_diffusion >= 170000 and top_diffusion <= 175959 ";
                case "18H - 19H":
                    return " top_diffusion >= 180000 and top_diffusion <= 185959 ";
                case "19H - 20H":
                    return " top_diffusion >= 190000 and top_diffusion <= 195959 ";
                case "20H - 21H":
                    return " top_diffusion >= 200000 and top_diffusion <= 205959 ";
                case "21H - 22H":
                    return " top_diffusion >= 210000 and top_diffusion <= 215959 ";
                case "22H - 23H":
                    return " top_diffusion >= 220000 and top_diffusion <= 225959 ";
                case "23H - 24H":
                    return " top_diffusion >= 230000 and top_diffusion <= 235959 ";
                default:
                    return string.Empty;
            }
        }
        #endregion

        protected override long CountDataRows()
        {
            #region Variables
            var sql = new StringBuilder(5000);
            bool allPeriod = string.IsNullOrEmpty(_timeSlot);
            long nbRows = 0;
            #endregion

            try
            {
                string sqlFields = _webSession.GenericInsertionColumns.GetSqlFields(null);
                string sqlGroupBy = _webSession.GenericInsertionColumns.GetSqlGroupByFields(null);
                string sqlConstraintFields = _webSession.GenericInsertionColumns.GetSqlConstraintFields();
                string tableName = SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, WebConstantes.Module.Type.alert,
                    _webSession.IsSelectRetailerDisplay);
                string sqlTables = _webSession.GenericInsertionColumns.GetSqlTables(WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label, null);
                string sqlConstraintTables = _webSession.GenericInsertionColumns.GetSqlConstraintTables(DBConstantes.Schema.ADEXPRESS_SCHEMA);

                //Select
                sql.Append(" select count(*) as NbROWS from "); //start count
                sql.Append(" select distinct");
                if (sqlFields.Length > 0) sql.AppendFormat(" {0}", sqlFields);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.Append(" , advertising_agency");
                    sqlGroupBy += " , advertising_agency";
                }

                if (sqlConstraintFields.Length > 0)
                {
                    sql.AppendFormat(" , {0}", sqlConstraintFields);//Fields for constraint management
                    sqlGroupBy += string.Format(" , {0}", _webSession.GenericInsertionColumns.GetSqlConstraintGroupByFields());
                }

                sql.Append(" from ");
                sql.AppendFormat(" {0} ", tableName);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                    sql.Append(", " + WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).SqlWithPrefix);

                if (sqlTables.Length > 0) sql.AppendFormat(" ,{0}", sqlTables);

                if (sqlConstraintTables.Length > 0)
                    sql.Append(" , " + sqlConstraintTables);//Tables pour la gestion des contraintes métiers

                // Joints conditions
                sql.Append(" Where ");

                sql.AppendFormat(" {0}.id_media={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _idMedia);
                sql.AppendFormat(" and {0}.date_media_num>={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _beginingDate);
                sql.AppendFormat(" and {0}.date_media_num<={1} ", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, _endDate);

                if (_webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                    WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null).Length > 0)
                    sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlJoins(_webSession.DataLanguage,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, null));
                sql.Append("  " + _webSession.GenericInsertionColumns.GetSqlContraintJoins());

                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv && !string.IsNullOrEmpty(_timeSlot))
                {
                    sql.AppendFormat(" and {0}", GetTopDiffusionFilter(_timeSlot));
                }

                string listProductHap = GetExcludeProducts(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                string product = GetProductData();
                string productsRights = SQLGenerator.GetClassificationCustomerProductRight(_webSession,
                 WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true, _module.ProductRightBranches);
                string mediaRights = SQLGenerator.getAnalyseCustomerMediaRight(_webSession,
                WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix, true);
                string orderby = GetOrderByDetailMedia(allPeriod);

                if (_webSession.GenericInsertionColumns.ContainColumnItem(GenericColumnItemInformation.Columns.agenceMedia))
                {
                    sql.AppendFormat(" and {0}.id_advertising_agency(+)={1}.id_advertising_agency ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    sql.AppendFormat(" and {0}.id_language(+)={1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix, _webSession.DataLanguage);
                    sql.AppendFormat(" and {0}.activation(+)<{1} ",
                        WebApplicationParameters.DataBaseDescription.GetTable(TableIds.advertisingAgency).Prefix,
                        TNS.AdExpress.Constantes.DB.ActivationValues.UNACTIVATED);
                }

                #region Droits
                //liste des produit hap
                sql.Append(listProductHap);
                sql.Append(product);
                sql.Append(productsRights);
                sql.Append(mediaRights);

                //Media Universe
                sql.Append(" " + GetMediaUniverse(WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix));

                //Rights detail spot to spot TNT
                if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.tv
              && !_webSession.CustomerLogin.CustormerFlagAccess(DBConstantes.Flags.ID_DETAIL_DIGITAL_TV_ACCESS_FLAG))
                {
                    string idTntCategory = TNS.AdExpress.Domain.Lists
                        .GetIdList(WebConstantes.GroupList.ID.category, WebConstantes.GroupList.Type.digitalTv);
                    if (!string.IsNullOrEmpty(idTntCategory))
                    {
                        sql.AppendFormat(" and {0}.id_category not in ({1})  ", WebApplicationParameters
                       .DataBaseDescription.DefaultResultTablePrefix, idTntCategory);
                    }
                }

                #endregion

                //Group by
                sql.Append(" group by ");
                if (sqlGroupBy.Length > 0) sql.Append(sqlGroupBy);


                // Order by
                sql.AppendFormat("  {0}", orderby);

                sql.Append(" ) "); //end count

            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to build the query {0}", sql.ToString()), err));
            }

            #region Query execution
            try
            {
                var ds = _webSession.Source.Fill(sql.ToString());
                if (ds != null && ds.Tables[0] != null && ds.Tables[0].Rows.Count == 1)
                    nbRows = (Int64.Parse(ds.Tables[0].Rows[0]["NbROWS"].ToString()));
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException(string.Format("Impossible to exectute query of  media detail : {0}",
                    sql.ToString()), err));
            }
            #endregion

            return nbRows;
        }
    }
}

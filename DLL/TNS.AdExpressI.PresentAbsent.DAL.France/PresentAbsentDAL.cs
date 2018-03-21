using System;
using TNS.AdExpress.Domain.DataBaseDescription;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.PresentAbsentDAL.Exceptions;
using CstDBClassif = TNS.AdExpress.Constantes.Classification.DB;
using FctWeb = TNS.AdExpress.Web.Core.Utilities;
using CstDB = TNS.AdExpress.Constantes.DB;
using CstWeb = TNS.AdExpress.Constantes.Web;

namespace TNS.AdExpressI.PresentAbsent.DAL.France
{
    /// <summary>
    /// Default  module absent/present report data access layer.
    /// Uses the methods defined in the parent class.
    /// </summary>
    public class PresentAbsentDAL : DAL.PresentAbsentDAL
    {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="session">User Session</param>
        public PresentAbsentDAL(WebSession session)
            : base(session)
        {
        }
        #endregion

        protected override Table GetGadtable()
        {
            Table tblGad = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.gad);
            if (
                _session.CustomerLogin.CustormerFlagAccess(
                    (long)TNS.AdExpress.Constantes.Customer.DB.Flag.id.leFac.GetHashCode()))
                tblGad = WebApplicationParameters.DataBaseDescription.GetTable(TableIds.leFac);
            return tblGad;
        }


        protected override string InitQueryParams(CstDB.TableType.Type type, string dataTableName, Schema schAdEx, CustomerPeriod customerPeriod, Table tblGad
        , ref string productTableName, ref string productFieldName, ref string columnDetailLevel, ref string groupByFieldName, ref string productJoinCondition
        , ref string universFilter, ref string unitFieldNameSumWithAlias, ref string groupByOptional, ref string dataJointForInsert
        , ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            try
            {
                // Get table name and date field according to the table type parameter
                string dateField;
                string adexprSchema = WebApplicationParameters.DataBaseDescription.GetSchema(SchemaIds.adexpr03).Label;
                switch (type)
                {
                    case CstDB.TableType.Type.dataVehicle4M:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.alert, _session.IsSelectRetailerDisplay);
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.dataVehicle:
                        dataTableName = FctWeb.SQLGenerator.GetVehicleTableSQLForDetailResult(_vehicleInformation.Id, CstWeb.Module.Type.analysis, _session.IsSelectRetailerDisplay);
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.DATE_MEDIA_NUM;
                        break;
                    case CstDB.TableType.Type.webPlan:
                        dataTableName = WebApplicationParameters.GetDataTable(TableIds.monthData, _session.IsSelectRetailerDisplay).SqlWithPrefix;
                        dateField = WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + "." + CstDB.Fields.WEB_PLAN_MEDIA_MONTH_DATE_FIELD;
                        break;
                    default:
                        throw (new PresentAbsentDALException("Unable to determine the type of table to use."));
                }

                /* Get the SQL tables  corresponding to the classification's 
                level selected (for FROM clause).*/
                productTableName = _session.GenericProductDetailLevel.GetSqlTables(schAdEx.Label);
                if (!string.IsNullOrEmpty(productTableName)) productTableName = "," + productTableName;

                /* Get the SQL  fields corresponding to the classification's 
                level selected for  SELECT clause.*/
                productFieldName = _session.GenericProductDetailLevel.GetSqlFields();
                columnDetailLevel = string.Format("{0}.{1}", WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix,
                    ((DetailLevelItemInformation)_session.GenericColumnDetailLevel.Levels[0]).GetSqlFieldIdWithoutTablePrefix());

                /* Get the SQL fields corresponding to the classification's 
                level selected for  GROUP BY clause.*/
                groupByFieldName = _session.GenericProductDetailLevel.GetSqlGroupByFields();

                /* Get the SQL joins  code .*/
                productJoinCondition = _session.GenericProductDetailLevel.GetSqlJoins(_session.DataLanguage, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                //Get Universe filters
                universFilter = GetUniversFilter(type, dateField, customerPeriod);

                // Get unit selected field
                unitFieldNameSumWithAlias = FctWeb.SQLGenerator.GetUnitFieldNameSumWithAlias(_session, type);

                //Treatment specific to the medium evaliant : group by list of banners
                if ((_vehicleInformation.Id == CstDBClassif.Vehicles.names.adnettrack || _vehicleInformation.Id == CstDBClassif.Vehicles.names.evaliantMobile)
                    && _session.Unit == CstWeb.CustomerSessions.Unit.versionNb)
                {
                    if (type == CstDB.TableType.Type.webPlan)
                    {
                        groupByOptional = $",{adexprSchema}.LISTNUM_TO_CHAR({WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix}.list_banners) ";
                    }
                    else
                    {
                        groupByOptional = $",{WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix}.hashcode ";
                    }
                }

                //Treatment specific to the medium PRESS : option INSET 
                if (CstDBClassif.Vehicles.names.press == _vehicleInformation.Id || CstDBClassif.Vehicles.names.internationalPress == _vehicleInformation.Id
                    || _vehicleInformation.Id == CstDBClassif.Vehicles.names.newspaper
                    || _vehicleInformation.Id == CstDBClassif.Vehicles.names.magazine)
                    dataJointForInsert = FctWeb.SQLGenerator.GetJointForInsertDetail(_session, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);

                //Get filter options for the GAD (company's informations)
                if (_session.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                {
                    try
                    {
                        dataTableNameForGad = ", " + tblGad.SqlWithPrefix;
                        dataFieldsForGad = ", " + FctWeb.SQLGenerator.GetFieldsAddressForGad(tblGad.Prefix);
                        dataJointForGad = "and " + FctWeb.SQLGenerator.GetJointForGad(tblGad.Prefix, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
                    }
                    catch (SQLGeneratorException)
                    {
                        ;
                    }
                }
            }
            catch (Exception e)
            {
                throw (new PresentAbsentDALException("Unable to intialise request parameters :" + e.Message));
            }
            return dataTableName;
        }


    }
}

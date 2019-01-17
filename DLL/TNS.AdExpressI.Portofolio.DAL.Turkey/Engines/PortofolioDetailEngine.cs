using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using WebConstantes = TNS.AdExpress.Constantes.Web;


namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class PortofolioDetailEngine : DAL.Engines.PortofolioDetailEngine
    {
        #region PortofolioDetailEngine
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd)
        {
        }
        #endregion

        #region GetFieldsAddressForGad
        protected override string GetFieldsAddressForGad()
        {
            return "";
        }
        #endregion

        protected override void GetGad(ref string dataTableNameForGad, ref string dataFieldsForGad, ref string dataJointForGad)
        {
            dataTableNameForGad = "";
            dataFieldsForGad = "";
            dataJointForGad = "";
        }

        protected override string GetUnitFieldsNameUnionForPortofolio()
        {
            return SQLGenerator.GetUnitFieldsNameUnionForPortofolioMulti(_webSession);
        }

        protected override string GetUnitFieldsName4M(DBConstantes.TableType.Type type)
        {
            return SQLGenerator.GetUnitFieldsNameMulti(_webSession, type, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
        }

        public override string GetUnitFieldsName(WebSession webSession, DBConstantes.TableType.Type type, string dataTablePrefixe)
        {
            List<UnitInformation> unitsList = webSession.GetSelectedUnits();
            var sqlUnit = new StringBuilder();
            if (!string.IsNullOrEmpty(dataTablePrefixe))
                dataTablePrefixe += ".";
            else
                dataTablePrefixe = "";
            for (int i = 0; i < unitsList.Count; i++)
            {
                if (i > 0) sqlUnit.Append(",");
                switch (type)
                {
                    case DBConstantes.TableType.Type.dataVehicle:
                    case DBConstantes.TableType.Type.dataVehicle4M:
                        if (unitsList[i].Id != CustomerSessions.Unit.versionNb)
                            sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseField, unitsList[i].Id.ToString());
                        else
                            sqlUnit.AppendFormat("  to_char({1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseField, unitsList[i].Id.ToString());
                        break;
                    case DBConstantes.TableType.Type.webPlan:
                        if (unitsList[i].Id != CustomerSessions.Unit.versionNb)
                            sqlUnit.AppendFormat("sum({0}{1}) as {2}", dataTablePrefixe, unitsList[i].DatabaseMultimediaField, unitsList[i].Id.ToString());
                        else
                            sqlUnit.AppendFormat(GetUnit(i, unitsList));
                        break;
                }
            }
            return sqlUnit.ToString();
        }

        protected override long CountDataRows()
        {

            #region Variables
            string sql = string.Empty, sql4M = string.Empty, sqlDataVehicle = string.Empty, sqlWebPlan = string.Empty;
            string groupByFieldNameWithoutTablePrefix = string.Empty;
            string orderFieldName = string.Empty, orderFieldNameWithoutTablePrefix = string.Empty;
            string productFieldNameWithoutTablePrefix = string.Empty, unitField = string.Empty, dataFieldsForGadWithoutTablePrefix = string.Empty;
            CustomerPeriod customerPeriod = _webSession.CustomerPeriodSelected;
            long nbRows = 0;
            #endregion

            #region Construction de la requête
            try
            {
                orderFieldName = _webSession.GenericProductDetailLevel.GetSqlOrderFields();
                orderFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlOrderFieldsWithoutTablePrefix();
                groupByFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlGroupByFieldsWithoutTablePrefix();

                if (customerPeriod.IsSliding4M)
                {
                    sql4M = GetRequest(DBConstantes.TableType.Type.dataVehicle4M);
                    sql4M += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
                    sql = sql4M;
                }
                else if (!customerPeriod.IsDataVehicle && !customerPeriod.IsWebPlan)
                {
                    sql = GetRequest(DBConstantes.TableType.Type.dataVehicle);
                    sql += " order by " + orderFieldName + "," + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".id_media ";
                }
                else
                {

                    if (customerPeriod.IsDataVehicle)
                    {
                        sqlDataVehicle = GetRequest(DBConstantes.TableType.Type.dataVehicle);
                        sql = sqlDataVehicle;
                    }

                    if (customerPeriod.IsWebPlan)
                    {
                        sqlWebPlan = GetRequest(DBConstantes.TableType.Type.webPlan);
                        sql = sqlWebPlan;
                    }

                    if (customerPeriod.IsDataVehicle && customerPeriod.IsWebPlan)
                    {
                        productFieldNameWithoutTablePrefix = _webSession.GenericProductDetailLevel.GetSqlFieldsWithoutTablePrefix();
                        if (_webSession.GenericProductDetailLevel.ContainDetailLevelItem(DetailLevelItemInformation.Levels.advertiser))
                            dataFieldsForGadWithoutTablePrefix = GetFieldsAddressForGad();
                        sql = "";
                        string unitSelect = GetUnitFieldsNameUnionForPortofolio();
                        if (unitSelect != null && unitSelect.Length > 0) unitSelect = ", " + unitSelect;
                     
                        sql += " select id_media, " + productFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix + unitSelect;
                        if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                        {
                            sql += string.Format(", {0}", UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.versionNb).Id.ToString());
                        }
                        sql += " from (";
                        sql += sqlDataVehicle;
                        sql += " UNION ";
                        sql += sqlWebPlan;
                        sql += " ) ";
                        sql += " group by id_media, " + groupByFieldNameWithoutTablePrefix + dataFieldsForGadWithoutTablePrefix;
                        if (_vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.adnettrack || _vehicleInformation.Id == DBClassificationConstantes.Vehicles.names.evaliantMobile)
                        {
                            sql += string.Format(", {0}", UnitsInformation.Get(WebConstantes.CustomerSessions.Unit.versionNb).Id.ToString());
                        }
                    }

                    sql += " order by " + orderFieldNameWithoutTablePrefix + ", id_media ";

                   
                }
                sql = " select count(*) as NbROWS from ( " + sql +" ) "; // count
            }
            catch (System.Exception err)
            {
                throw (new PortofolioDALException("Impossible to build detail media Portotofolio query " + sql, err));
            }
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
                throw (new PortofolioDALException("Impossible to exectute detail media Portotofolio query" + sql, err));
            }

            #endregion

            return nbRows;
        }
    }
}

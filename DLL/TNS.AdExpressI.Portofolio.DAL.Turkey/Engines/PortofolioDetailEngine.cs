using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using Module = TNS.AdExpress.Domain.Web.Navigation.Module;
using DBConstantes = TNS.AdExpress.Constantes.DB;

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
    }
}

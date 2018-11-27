using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Web.Core.Utilities;
using TNS.AdExpressI.Portofolio.DAL.Exceptions;
using DBConstantes = TNS.AdExpress.Constantes.DB;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.DAL.Turkey.Engines
{
    public class StructureEngine : DAL.Engines.StructureEngine
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
        /// <param name="hourBeginningList">Hour beginning list</param>
        /// <param name="hourEndList">Hour end list</param>
        public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList)
            : base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd, hourBeginningList, hourEndList)
        {
        }
        #endregion

        protected override string GetUnitFieldsNameForPortofolio()
        {
            return SQLGenerator.GetUnitFieldsNameForPortofolioMulti(_webSession, DBConstantes.TableType.Type.dataVehicle, WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix);
        }

        #region Get Hour Interval
        /// <summary>
        /// Get Hour Interval
        /// </summary>
        /// <param name="hourBegin">Hour Begin</param>
        /// <param name="hourEnd">Hour End</param>
        /// <returns>String SQL</returns>
        protected override string GetHourInterval(double hourBegin, double hourEnd)
        {
            string sql = "";
            switch (_vehicleInformation.Id)
            {
                case DBClassificationConstantes.Vehicles.names.tv:
                    if (hourBegin == 240000)
                    {
                        sql += " and ((" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>= " + hourBegin;
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<= 255959)";
                        sql += " or (" + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>= 20000";
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion< " + hourEnd + "))";
                    }
                    else
                    {
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion>=" + hourBegin;
                        sql += " and " + WebApplicationParameters.DataBaseDescription.DefaultResultTablePrefix + ".top_diffusion<" + hourEnd;
                    }
                    return sql;
                default:
                    throw new PortofolioDALException("GetHourInterval : Vehicle unknown.");
            }
        }
        #endregion
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;

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
    }
}

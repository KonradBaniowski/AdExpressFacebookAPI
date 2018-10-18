using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class PortofolioDetailEngine : Portofolio.Engines.PortofolioDetailEngine
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">vehicleInformation</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public PortofolioDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool showInsertions, bool showCreatives)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, showInsertions, showCreatives)
        {
        }
        #endregion

        protected override List<UnitInformation> GetValidUnitForResult()
        {
            return _webSession.GetSelectedUnits();
        }
    }
}

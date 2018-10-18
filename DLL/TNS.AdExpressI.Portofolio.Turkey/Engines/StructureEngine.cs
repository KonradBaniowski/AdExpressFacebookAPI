using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Constantes.FrameWork.Results;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.Exceptions;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;

namespace TNS.AdExpressI.Portofolio.Turkey.Engines
{
    public class StructureEngine : Portofolio.Engines.StructureEngine
    {
        #region Constructor
        /// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Client Session</param>
		/// <param name="vehicleInformation">Vehicle</param>
		/// <param name="idMedia">Id media</param>
		/// <param name="periodBeginning">Period Beginning </param>
		/// <param name="periodEnd">Period End</param>
		public StructureEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, Dictionary<string, double> hourBeginningList, Dictionary<string, double> hourEndList, bool excel)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd, hourBeginningList, hourEndList, excel)
        {
        }
        #endregion

        protected override List<UnitInformation> GetValidUnitForResult()
        {
            return _webSession.GetSelectedUnits();
        }
    }
}

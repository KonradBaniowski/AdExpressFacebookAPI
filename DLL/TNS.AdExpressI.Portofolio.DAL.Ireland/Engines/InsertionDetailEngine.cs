using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Portofolio.DAL.Ireland.Engines {
    public class InsertionDetailEngine : DAL.Engines.InsertionDetailEngine {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle information</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public InsertionDetailEngine(WebSession webSession, VehicleInformation vehicleInformation, Module module,
            long idMedia, string periodBeginning, string periodEnd, string adBreak) :
            base(webSession, vehicleInformation, module, idMedia, periodBeginning, periodEnd, adBreak) {
        }
        #endregion

        /// <summary>
		/// Get data for media detail insertion
		/// </summary>	
		/// <returns>liste des publicités pour un média donné</returns>
        /*protected override DataSet ComputeData() {
            return null;
        }*/

        #region GetDateCoverField
        protected override void GetDateCoverField(StringBuilder sql, ref string sqlGroupBy) {
        }
        #endregion

    }
}

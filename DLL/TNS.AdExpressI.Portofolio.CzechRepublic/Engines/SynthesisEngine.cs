#region Information
// Author: A. Rousseau
// Creation date: 06/07/2009
// Modification date:
#endregion

#region Using
using System;
using System.Data;
using System.Collections.Generic;
using System.Text;
using System.Reflection;

using AbstractResult = TNS.AdExpressI.Portofolio.Engines;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpress.Constantes.FrameWork.Results;
using DBClassificationConstantes = TNS.AdExpress.Constantes.Classification.DB;
using DBCst = TNS.AdExpress.Constantes.DB;
using WebCst = TNS.AdExpress.Constantes.Web;
using WebFunctions = TNS.AdExpress.Web.Functions;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.AdExpressI.Portofolio.DAL;
using TNS.AdExpress.Web.Core.Utilities;

using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Units;
using TNS.AdExpress.Domain.Translation;

using TNS.FrameWork.WebResultUI;
using TNS.FrameWork.Date;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Web;
#endregion

namespace TNS.AdExpressI.Portofolio.CzechRepublic.Engines {
    /// <summary>
    /// CzechRepublic synthesis engine
    /// </summary>
    public class SynthesisEngine : AbstractResult.SynthesisEngine {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicle">Vehicle</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public SynthesisEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd) {
        }
        #endregion

        #region Protected Methods Override
        /// <summary>
        /// Compute Data product Number in Tracking
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data product Number in Vehicle
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data Average Duration Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataAverageDurationEcran(bool isAlertModule) {
            return null;
        }
        /// <summary>
        /// Compute Data Spot Number By Ecran (not display because the field cobranding is always at 0 for tv and radio)
        /// </summary>
        /// <param name="isAlertModule">isAlertModule</param>
        /// <returns>List of result</returns>
        protected override List<ICell> ComputeDataSpotNumberByEcran(bool isAlertModule) {
            return null;
        }
        //_displayTypeSale
        protected override List<ICell> ComputeDataNetworkType(bool isAlertModule) {
            return null;
        }
        #endregion

    }
}

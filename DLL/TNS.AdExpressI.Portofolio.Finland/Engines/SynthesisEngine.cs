#region Information
// Author: Y. R'kaina
// Creation date: 29/08/2008
// Modification date:
#endregion

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

namespace TNS.AdExpressI.Portofolio.Finland.Engines {

    public class SynthesisEngine : AbstractResult.SynthesisEngine{

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

        #region GetPeriod
        /// <summary>
        /// GetPeriod
        /// </summary>
        protected override void GetPeriod(bool isAlertModule, string firstDate, string lastDate, out DateTime dtFirstDate, out DateTime dtLastDate) {
            dtFirstDate = DateTime.Today;
            dtLastDate = DateTime.Today;

            if (isAlertModule) {
                if (firstDate.Length > 0) {
                    dtFirstDate = new DateTime(int.Parse(firstDate.Substring(0, 4)), int.Parse(firstDate.Substring(4, 2)), int.Parse(firstDate.Substring(6, 2)));
                }

                if (lastDate.Length > 0) {
                    dtLastDate = new DateTime(int.Parse(lastDate.Substring(0, 4)), int.Parse(lastDate.Substring(4, 2)), int.Parse(lastDate.Substring(6, 2)));
                }
            }
            else {
                dtFirstDate = WebFunctions.Dates.getPeriodBeginningDate(_periodBeginning, _webSession.PeriodType);
                dtLastDate = WebFunctions.Dates.getPeriodEndDate(_periodEnd, _webSession.PeriodType);
            }
        }
        #endregion		

        #region Protected Methods Override
        protected override List<ICell> ComputeDataProductNumberInTracking(bool isAlertModule) {
            return null;
        }

        protected override List<ICell> ComputeDataProductNumberInVehicle(bool isAlertModule) {
            return null;
        }
        #endregion

    }

    #region Class Data Unit
    /// <summary>
    /// Data Unit
    /// </summary>
    public class DataUnit : AbstractResult.DataUnit
    {
        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        public DataUnit(IPortofolioDAL portofolioDAL, VehicleInformation vehicleInformation)
            : base(portofolioDAL, vehicleInformation)
        {
        }
        #endregion
    }
    #endregion
}

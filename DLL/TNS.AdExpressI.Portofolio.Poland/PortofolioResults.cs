#region Information
// Author: Y. R'kaina
// Creation date: 25/11/2008
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using AbstractResult = TNS.AdExpressI.Portofolio;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.FrameWork.WebResultUI;
using TNS.AdExpressI.Portofolio.Exceptions;

namespace TNS.AdExpressI.Portofolio.Poland {
    /// <summary>
    /// Poland Portofolio class result
    /// </summary>
    public class PortofolioResults : AbstractResult.PortofolioResults {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public PortofolioResults(WebSession webSession) : base(webSession) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public PortofolioResults(WebSession webSession, string adBreak, string dayOfWeek) : base(webSession, adBreak, dayOfWeek) {
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        /// <param name="periodBeginning">Period Beginning</param>
        /// <param name="periodEnd">period end</param>
        /// <param name="tableType">tableType</param>
        public PortofolioResults(WebSession webSession, TNS.AdExpress.Constantes.DB.TableType.Type tableType) : base(webSession, tableType) {
        }
        #endregion

        #region Implementation of abstract methods
        /// <summary>
        /// Get ResultTable for some portofolio result
        ///  - DETAIL_PORTOFOLIO
        ///  - CALENDAR
        ///  - SYNTHESIS (only result table)
        /// </summary>
        /// <param name="webSession">Customer session</param>
        /// <returns>Result Table</returns>
        public override ResultTable GetResultTable() {
            Engines.SynthesisEngine result = null;
            try {
                switch (_webSession.CurrentTab) {
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.DETAIL_PORTOFOLIO:
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.CALENDAR:
                        return base.GetResultTable();
                    case TNS.AdExpress.Constantes.FrameWork.Results.Portofolio.SYNTHESIS:
                        result = new Engines.SynthesisEngine(_webSession, _vehicleInformation, _idMedia, _periodBeginning, _periodEnd);
                        break;
                    default:
                        throw (new PortofolioException("Impossible to identified current tab "));
                }
            }
            catch (System.Exception err) {
                throw (new PortofolioException("Impossible to compute portofolio results", err));
            }

            return result.GetResultTable();
        }
        /// <summary>
        /// Get visual list
        /// </summary>
        /// <param name="beginDate">begin Date</param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        public override Dictionary<string, string> GetVisualList(string beginDate, string endDate) {
            return null;
        }

        /// <summary>
        /// Get view of the vehicle (HTML)
        /// </summary>
        /// <param name="excel">True for excel result</param>
        /// <param name="resultType">Result Type (Synthesis, MediaDetail)</param>
        /// <returns>HTML code</returns>
        public override string GetVehicleViewHtml(bool excel, int resultType) {

            return "";
        }

        #endregion

    }
}

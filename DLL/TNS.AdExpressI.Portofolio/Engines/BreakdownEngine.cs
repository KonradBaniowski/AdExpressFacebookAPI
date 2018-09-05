using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.AdExpress.Domain.Classification;
using TNS.AdExpress.Domain.Level;
using TNS.AdExpress.Domain.Results;
using TNS.AdExpress.Web.Core.Sessions;
using TNS.AdExpressI.Portofolio.Exceptions;
using TNS.FrameWork.WebResultUI;

namespace TNS.AdExpressI.Portofolio.Engines
{
    public class BreakdownEngine : Engine
    {
        #region Variables
        /// <summary>
        /// Determine if render will be into excel file
        /// </summary>
        protected bool _excel = false;
        /// <summary>
        /// Level
        /// </summary>
        protected DetailLevelItemInformation _level;
        #endregion

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Client Session</param>
        /// <param name="vehicleInformation">Vehicle</param>
        /// <param name="idMedia">Id media</param>
        /// <param name="periodBeginning">Period Beginning </param>
        /// <param name="periodEnd">Period End</param>
        public BreakdownEngine(WebSession webSession, VehicleInformation vehicleInformation, Int64 idMedia, string periodBeginning, string periodEnd, bool excel, DetailLevelItemInformation level)
            : base(webSession, vehicleInformation, idMedia, periodBeginning, periodEnd)
        {
            _excel = excel;
            _level = level;
        }
        #endregion

        #region Abstract methods implementation
        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override string BuildHtmlResult()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }

        /// <summary>
		/// Build Html result
		/// </summary>
		/// <returns></returns>
		protected override GridResult BuildGridResult()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }

        /// <summary>
        /// Build Html result
        /// </summary>
        /// <returns></returns>
        protected override ResultTable ComputeResultTable()
        {
            throw new PortofolioException("The method or operation is not implemented.");
        }
        #endregion
    }
}

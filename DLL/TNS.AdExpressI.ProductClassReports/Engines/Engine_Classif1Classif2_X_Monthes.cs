using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassReports.Engines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Classif2 X Monthes
    /// </summary>
    public class Engine_Classif1Classif2_X_Monthes : Engine_Classif1Classif2_X_Periods
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public Engine_Classif1Classif2_X_Monthes(WebSession session, int result)
            : base(session, result)
        {
            _monthlyExtended = true;
        }
        #endregion

    }
}

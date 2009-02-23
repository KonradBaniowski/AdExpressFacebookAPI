#region Information
/*
 * Author : G Ragneau
 * Created on : 12/01/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion


using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.ProductClassReports.GenericEngines
{

    /// <summary>
    /// Implement an engine to build a report presented as Classif1-Classif2 X Years
    /// </summary>
    public class GenericEngine_Classif1Classif2_X_Years : GenericEngine_Classif1Classif2_X_Periods
    {

        #region Constructor
        /// <summary>
        /// Defualt constructor
        /// </summary>
        /// <param name="session">User session</param>
        /// <param name="result">Report type</param>
        public GenericEngine_Classif1Classif2_X_Years(WebSession session, int result)
            : base(session, result)
        {
            _monthlyExtended = false;
        }
        #endregion

    }
}

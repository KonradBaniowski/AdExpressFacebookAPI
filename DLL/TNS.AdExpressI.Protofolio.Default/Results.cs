#region Information
// Author: G. Facon
// Creation date: 20/03/2007
// Modification date:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using AbstractResult=TNS.AdExpressI.Portofolio;
using TNS.AdExpress.Web.Core.Sessions;

namespace TNS.AdExpressI.Portofolio.Default {
    /// <summary>
    /// Default Portofolio class result
    /// </summary>
    public class Results:AbstractResult.PortofolioResults {

        #region Constructor
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>
        public Results(WebSession webSession):base(webSession){
        }
        #endregion

    }
}

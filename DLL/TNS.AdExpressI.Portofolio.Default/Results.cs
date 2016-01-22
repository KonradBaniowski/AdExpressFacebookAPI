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

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		public Results(WebSession webSession, string adBreak, string dayOfWeek)
			: base(webSession,adBreak,dayOfWeek) {
		}
		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="webSession">Customer Session</param>
		/// <param name="periodBeginning">Period Beginning</param>
		/// <param name="periodEnd">period end</param>
		/// <param name="tableType">tableType</param>
		public Results(WebSession webSession, TNS.AdExpress.Constantes.DB.TableType.Type tableType) 
		: base(webSession,tableType){
		}
         /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="webSession">Customer Session</param>		
        /// <param name="resultType">resultType</param>
        public Results(WebSession webSession, int resultType)
            : base(webSession, resultType)
        {
        }
        #endregion

    }
}

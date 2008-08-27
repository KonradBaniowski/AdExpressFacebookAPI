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
		//    /// <summary>
		///// Constructor
		///// </summary>
		///// <param name="webSession">Customer Session</param>
		///// <param name="dateParution">date Parution</param>
		///// <param name="dateCover">Date cover</param>
		/////<param name="nameMedia">Name media</param>
		/////<param name="nbrePages">nb pages</param>
		/////<param name="pageAnchor">Page anchor</param>
		//public Results(WebSession webSession, string dateParution, string dateCover, string nameMedia, string nbrePages, string pageAnchor)
		//    : base(webSession, dateParution, dateCover, nameMedia, nbrePages, pageAnchor) {			
		//}
		///// <summary>
		///// Constructor
		///// </summary>
		///// <param name="idMedia">Id media</param>
		///// <param name="dateCover">Date cover</param>
		/////<param name="fileName1">file Name </param>
		/////<param name="fileName2">nb pages</param>
		/////<param name="pageAnchor">file Name</param>
		//public Results(long idMedia, string dateCover, string fileName1, string fileName2)
		//    : base(idMedia, dateCover, fileName1, fileName2) {			
		//}
        #endregion

    }
}

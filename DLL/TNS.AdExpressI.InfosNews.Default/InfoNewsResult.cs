using System;
using System.IO;
using TNS.AdExpress.Web.Core.Sessions;
using System.Collections;
using TNS.AdExpressI.InfoNews;


namespace TNS.AdExpressI.InfosNews.Default {
	public class InfoNewsResult : TNS.AdExpressI.InfoNews.InfoNewsResult  {

		 #region Constructor
        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="session">User session</param>
		/// <param name="themeName">theme Name</param>
		public InfoNewsResult(WebSession session, string themeName)
			: base(session, themeName)
        {
        }
        #endregion
	}
}

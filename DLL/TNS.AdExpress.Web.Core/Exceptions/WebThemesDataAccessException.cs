#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
    /// <summary>
    /// Web themes data access Excpetion
    /// </summary>
    public class WebThemesDataAccessException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public WebThemesDataAccessException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public WebThemesDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public WebThemesDataAccessException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}

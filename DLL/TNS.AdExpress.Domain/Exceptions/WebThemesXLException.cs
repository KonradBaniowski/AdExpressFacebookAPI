#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
    /// <summary>
    /// Web themes data access Excpetion
    /// </summary>
    public class WebThemesXLException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public WebThemesXLException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public WebThemesXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public WebThemesXLException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}

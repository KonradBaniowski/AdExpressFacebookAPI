#region Informations
// Author: G. Facon
// Creation date: 21/02/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
    /// <summary>
    /// Web language data access Excpetion
    /// </summary>
    public class WebLanguagesDataAccessException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public WebLanguagesDataAccessException():base(){	
        }

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public WebLanguagesDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public WebLanguagesDataAccessException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}

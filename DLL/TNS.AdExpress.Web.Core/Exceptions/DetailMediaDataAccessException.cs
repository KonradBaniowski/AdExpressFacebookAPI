#region Informations
// Auteur: Y. R'kaina
// Date de création: 11/08/08
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
    /// <summary>
    /// Detail Media DataAccess Exception
    /// </summary>
    class DetailMediaDataAccessException : BaseException {

        #region Constructor
		/// <summary>
        /// Base constructor
		/// </summary>
		public DetailMediaDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public DetailMediaDataAccessException(string message):base(message){			
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Exception source</param>
        public DetailMediaDataAccessException(string message, System.Exception innerException) : base(message, innerException) {
		}
		#endregion

    }
}

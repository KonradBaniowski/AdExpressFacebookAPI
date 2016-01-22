#region Information
// Author: Y R'kaina
// Creation: 09/10/2009
// Last modifications:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Date.DAL.Exception {

    /// <summary>
    /// Throw when an error occure during date treatment
    /// </summary>
    public class DateDALException : BaseException {
        
        #region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DateDALException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DateDALException(string message):base(message){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public DateDALException(string message, System.Exception innerException) : base(message, innerException) {
		}
		#endregion

    }
}

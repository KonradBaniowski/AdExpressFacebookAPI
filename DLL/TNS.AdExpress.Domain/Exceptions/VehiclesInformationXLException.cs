#region Informations
// Author: Y. R'kaina
// Creation date: 07/08/2008
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions {
    /// <summary>
    /// Vehicle Exception
    /// </summary>
    public class VehiclesInformationXLException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public VehiclesInformationXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public VehiclesInformationXLException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public VehiclesInformationXLException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

    }
}

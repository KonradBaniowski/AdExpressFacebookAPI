#region Informations
// Author: G. Facon
// Creation date: 28/03/2006 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// When AdExpress loads detail levels information
	/// </summary>
	public class DetailLevelsInformationXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public DetailLevelsInformationXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public DetailLevelsInformationXLException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public DetailLevelsInformationXLException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}

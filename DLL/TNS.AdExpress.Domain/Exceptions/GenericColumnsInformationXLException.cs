#region Informations
// Author: D. Mussuma
// Creation date: 16/07/2008 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// When AdExpress loads detail level items information
	/// </summary>
	public class GenericColumnsInformationXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericColumnsInformationXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public GenericColumnsInformationXLException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public GenericColumnsInformationXLException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}

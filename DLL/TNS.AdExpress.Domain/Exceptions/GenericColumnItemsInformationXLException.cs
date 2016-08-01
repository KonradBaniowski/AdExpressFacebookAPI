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
	public class GenericColumnItemsInformationXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericColumnItemsInformationXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public GenericColumnItemsInformationXLException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public GenericColumnItemsInformationXLException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}
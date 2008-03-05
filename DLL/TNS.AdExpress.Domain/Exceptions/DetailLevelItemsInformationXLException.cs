#region Informations
// Author: G. Facon
// Creation date: 27/03/2006 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// When AdExpress loads detail level items information
	/// </summary>
	public class DetailLevelItemsInformationXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public DetailLevelItemsInformationXLException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public DetailLevelItemsInformationXLException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public DetailLevelItemsInformationXLException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
	}
}

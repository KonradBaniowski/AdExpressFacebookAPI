#region Information
// Author: G. Facon
// Creation date: 09/11/2005 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {

	/// <summary>
	/// Thrown when log Tracking Event in the data base
	/// </summary>
	public class TrackingDataAccessExeption:BaseException {

		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public TrackingDataAccessExeption():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public TrackingDataAccessExeption(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public TrackingDataAccessExeption(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

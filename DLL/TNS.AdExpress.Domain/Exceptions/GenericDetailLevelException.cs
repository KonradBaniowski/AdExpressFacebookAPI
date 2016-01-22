#region Informations
// Author: G. Facon
// Creation date: 30/03/2006 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions{
	/// <summary>
	/// Detail Level management
	/// </summary>
	public class GenericDetailLevelException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericDetailLevelException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public GenericDetailLevelException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public GenericDetailLevelException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

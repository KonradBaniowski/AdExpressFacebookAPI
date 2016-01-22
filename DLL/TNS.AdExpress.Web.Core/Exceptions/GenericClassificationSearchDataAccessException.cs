#region Informations
// Author: Y. R'kaina
// Creation date: 14/12/2006 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Thrown when is impossible to get data in generic classification search
	/// </summary>
	public class GenericClassificationSearchDataAccessException:BaseException{

		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public GenericClassificationSearchDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public GenericClassificationSearchDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public GenericClassificationSearchDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

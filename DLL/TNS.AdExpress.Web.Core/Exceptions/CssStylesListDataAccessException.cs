#region Informations
// Author: D. Mussuma
// Creation date: 21/02/2007 
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// When AdExpress loads CSS files list
	/// </summary>
	public class CssStylesListDataAccessException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public CssStylesListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public CssStylesListDataAccessException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public CssStylesListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

#region Informations
// Author: G. Facon
// Creation date: 07/12/2004 
// Modification date: 07/12/2004 (G. Facon)
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions {

	/// <summary>
	/// Thrown when is impossible to initialize product Items list used to determine an AdExpress universe
	/// </summary>
	public class ProductListException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public ProductListException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public ProductListException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public ProductListException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

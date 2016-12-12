#region Informations
// Author: D. Mussuma
// Creation date: 26/10/2007
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.Classification.Universe {

	/// <summary>
	/// Capacity Exception
	/// </summary>
	public class CapacityException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public CapacityException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public CapacityException(string message):base(message){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public CapacityException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}
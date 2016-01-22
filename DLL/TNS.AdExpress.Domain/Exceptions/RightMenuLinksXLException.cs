#region Informations
// Author: 
// Creation date: 
// Modification date: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Domain.Exceptions {
	/// <summary>
    /// Thrown when is impossible to load the data relating to the Right Menu Links
	/// </summary>
	public class RightMenuLinksXLException:BaseException{
		
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public RightMenuLinksXLException():base(){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public RightMenuLinksXLException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public RightMenuLinksXLException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

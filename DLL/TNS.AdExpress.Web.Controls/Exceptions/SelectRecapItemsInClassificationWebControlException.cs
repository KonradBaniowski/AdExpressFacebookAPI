using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions {
	class SelectRecapItemsInClassificationWebControlException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public SelectRecapItemsInClassificationWebControlException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public SelectRecapItemsInClassificationWebControlException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public SelectRecapItemsInClassificationWebControlException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}

using System;
using System.Collections.Generic;
using System.Text;

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions {
	class SelectItemsInClassificationWebControlException : BaseException {
		#region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public SelectItemsInClassificationWebControlException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public SelectItemsInClassificationWebControlException(string message):base(message){
		
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
		public SelectItemsInClassificationWebControlException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}

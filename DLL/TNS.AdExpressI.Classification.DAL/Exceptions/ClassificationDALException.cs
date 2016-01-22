#region Information
/*
 * Author :  D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Classification.DAL.Exceptions {
	/// <summary>
	/// Exception class for Classification DAL
	/// </summary>
	public class ClassificationDALException : BaseException {
		
		#region Constructors
		
		/// <summary>
		/// Base constructor
		/// </summary>
		public ClassificationDALException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="innerException">Inner Exception</param>
		public ClassificationDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
		public ClassificationDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

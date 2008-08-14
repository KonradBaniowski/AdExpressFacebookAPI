#region Information
// Author: Y. Rkaina
// Creation date: 17/03/2007
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Portofolio.Exceptions {
	/// <summary>
	/// Portofolio result exception
	/// </summary>
	public class PortofolioException : BaseException{

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public PortofolioException():base(){
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public PortofolioException(string message):base(message){
		}

		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
        public PortofolioException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}

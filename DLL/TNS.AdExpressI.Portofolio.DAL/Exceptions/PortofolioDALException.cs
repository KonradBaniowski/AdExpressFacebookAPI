#region Information
// Author: G. Facon
// Creation date: 26/03/2007
// Modification date:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Portofolio.DAL.Exceptions {
	/// <summary>
	/// Portofolio Data access exception
	/// </summary>
	public class PortofolioDALException : BaseException{

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public PortofolioDALException():base(){
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public PortofolioDALException(string message):base(message){
		}

		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
        public PortofolioDALException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}

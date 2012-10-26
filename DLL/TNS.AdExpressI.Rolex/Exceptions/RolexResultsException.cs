#region Information
// Author: G. Facon
// Creation date: 17/03/2007
// Modification date:
#endregion

using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Rolex.Exceptions{
	/// <summary>
	/// Media Schedule Report Exception
	/// </summary>
    public class RolexResultsException : BaseException
    {

		#region Constructeur
		/// <summary>
		/// Constructor
		/// </summary>
		public RolexResultsException():base(){
		}

		/// <summary>
        /// Constructor
		/// </summary>
		/// <param name="message">Error Message</param>
		public RolexResultsException(string message):base(message){
		}

		/// <summary>
        /// Constructor
		/// </summary>
        /// <param name="message">Error Message</param>
		/// <param name="innerException">Inner Exception</param>
        public RolexResultsException(string message, System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion

	}
}

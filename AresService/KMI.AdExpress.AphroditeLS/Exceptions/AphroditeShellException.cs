using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.Aphrodite.Exceptions {
    /// <summary>
    /// Aphrodite Service Exception
    /// </summary>
    public class AphroditeShellException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public AphroditeShellException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public AphroditeShellException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public AphroditeShellException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}

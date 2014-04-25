using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.Exceptions {
    /// <summary>
    /// Ad Volume Checker Exception
    /// </summary>
    public class AdVolumeCheckerShellException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public AdVolumeCheckerShellException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public AdVolumeCheckerShellException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public AdVolumeCheckerShellException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}

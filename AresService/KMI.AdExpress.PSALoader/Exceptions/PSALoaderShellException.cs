using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.PSALoader.Exceptions {
    /// <summary>
    /// PSA Service Exception
    /// </summary>
    public class PSALoaderShellException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public PSALoaderShellException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public PSALoaderShellException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public PSALoaderShellException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}

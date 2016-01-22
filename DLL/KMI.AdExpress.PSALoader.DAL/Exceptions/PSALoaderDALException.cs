using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.PSALoader.DAL.Exceptions {
    /// <summary>
    /// PSA DAL Exception
    /// </summary>
    class PSALoaderDALException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public PSALoaderDALException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public PSALoaderDALException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public PSALoaderDALException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}

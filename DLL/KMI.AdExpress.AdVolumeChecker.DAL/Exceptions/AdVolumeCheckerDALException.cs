using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.DAL.Exceptions {
    /// <summary>
    /// Ad Volume Checker DAL Exception
    /// </summary>
    public class AdVolumeCheckerDALException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public AdVolumeCheckerDALException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public AdVolumeCheckerDALException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public AdVolumeCheckerDALException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace KMI.AdExpress.AdVolumeChecker.Rules.Exceptions {
    public class AdVolumeCheckerRulesException : BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public AdVolumeCheckerRulesException():base(){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public AdVolumeCheckerRulesException(string message):base(message){}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public AdVolumeCheckerRulesException(string message, System.Exception innerException) : base(message, innerException) { }
		#endregion

    }
}

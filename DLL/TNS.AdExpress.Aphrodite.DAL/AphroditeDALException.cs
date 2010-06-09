using System;
using TNS.FrameWork.Exceptions;


namespace KMI.AdExpress.Aphrodite.DAL {
    /// <summary>
    /// Aphrodite DAL Exception
    /// </summary>
    public class AphroditeDALException:BaseException {

        #region Constructors
		/// <summary>
		/// Base constructor
		/// </summary>
		public AphroditeDALException():base(){			
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		public AphroditeDALException(string message):base(message){
		}

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="message">Error message</param>
		/// <param name="innerException">Inner Exception</param>
        public AphroditeDALException(string message,System.Exception innerException)
            : base(message,innerException) {
		}
		#endregion
    }
}

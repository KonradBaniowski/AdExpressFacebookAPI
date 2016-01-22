using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.AdvertisingAgency.Exceptions
{
    /// <summary>
    /// Excepion class for Advertising Agency report
    /// </summary>
    public class AdvertisingAgencyException : BaseException{

        #region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AdvertisingAgencyException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AdvertisingAgencyException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public AdvertisingAgencyException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

    }
}

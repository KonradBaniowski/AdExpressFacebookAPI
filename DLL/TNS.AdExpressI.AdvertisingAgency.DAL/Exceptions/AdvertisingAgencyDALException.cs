using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;


namespace TNS.AdExpressI.AdvertisingAgency.DAL.Exceptions
{
    /// <summary>
    /// Excepion class for Advertising Agency data access layer
    /// </summary>
    public class AdvertisingAgencyDALException : BaseException
    {
        #region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AdvertisingAgencyDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public AdvertisingAgencyDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public AdvertisingAgencyDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
    }
}

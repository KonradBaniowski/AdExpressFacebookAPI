using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions {
    public class PurchaseModeListDataAccessException : BaseException {

        #region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PurchaseModeListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PurchaseModeListDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public PurchaseModeListDataAccessException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion

    }
}

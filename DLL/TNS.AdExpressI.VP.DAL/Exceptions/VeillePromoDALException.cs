using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.VP.DAL.Exceptions
{
    public class VeillePromoDALException:BaseException{
    
        #region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VeillePromoDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public VeillePromoDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public VeillePromoDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
    }
}

#region Informations
/*
 * Author : B.Masson
 * Created on 29/09/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.NewCreatives.Exceptions {

    /// <summary>
    /// Excepion class for new creatives
    /// </summary>
    public class NewCreativesException : BaseException {

        #region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public NewCreativesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public NewCreativesException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public NewCreativesException(string message, System.Exception innerException): base(message, innerException){
		}
		#endregion

    }
}

#region Informations
/*
 * Author : G Ragneau
 * Created on 26/03/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.LostWon.Exceptions
{
	/// <summary>
	/// Excepion class for LostWon report
	/// </summary>
	public class LostWonException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public LostWonException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public LostWonException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public LostWonException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

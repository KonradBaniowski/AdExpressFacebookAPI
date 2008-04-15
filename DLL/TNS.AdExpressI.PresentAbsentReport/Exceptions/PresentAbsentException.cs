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

namespace TNS.AdExpressI.PresentAbsent.Exceptions{
	/// <summary>
	/// Excepion class for present/absent report
	/// </summary>
	public class PresentAbsentException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PresentAbsentException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PresentAbsentException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public PresentAbsentException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

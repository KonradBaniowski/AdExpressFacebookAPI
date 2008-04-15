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

namespace TNS.AdExpressI.PresentAbsentDAL.Exceptions{
	/// <summary>
	/// Excepion class for present/absent data access layer
	/// </summary>
	public class PresentAbsentDALException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PresentAbsentDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PresentAbsentDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public PresentAbsentDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

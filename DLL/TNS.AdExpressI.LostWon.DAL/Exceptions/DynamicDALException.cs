#region Informations
/*
 * Author : G Ragneau
 * Created on 18/04/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.DynamicReport.DAL.Exceptions
{
	/// <summary>
	/// Excepion class for dynamic data access layer
	/// </summary>
	public class DynamicDALException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DynamicDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DynamicDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public DynamicDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

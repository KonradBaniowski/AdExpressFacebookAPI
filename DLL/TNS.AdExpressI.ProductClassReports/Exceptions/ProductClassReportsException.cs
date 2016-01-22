#region Informations
/*
 * Author : G Ragneau
 * Created on 30/06/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.ProductClassReports.Exceptions
{
	/// <summary>
	/// Excepion class for product class report
	/// </summary>
	public class ProductClassReportsException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductClassReportsException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductClassReportsException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public ProductClassReportsException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

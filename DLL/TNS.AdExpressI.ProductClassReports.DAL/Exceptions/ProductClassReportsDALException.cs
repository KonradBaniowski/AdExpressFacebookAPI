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

namespace TNS.AdExpressI.ProductClassReports.DAL.Exceptions
{
	/// <summary>
	/// Excepion class for product class report
	/// </summary>
	public class ProductClassReportsDALException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductClassReportsDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductClassReportsDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public ProductClassReportsDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

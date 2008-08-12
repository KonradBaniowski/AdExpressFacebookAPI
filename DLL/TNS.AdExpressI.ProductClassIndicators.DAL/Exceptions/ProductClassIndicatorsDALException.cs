#region Informations
/*
 * Author : G Ragneau
 * Created on 30/07/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.ProductClassIndicators.DAL.Exceptions
{
	/// <summary>
	/// Excepion class for product class indicator
	/// </summary>
	public class ProductClassIndicatorsDALException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductClassIndicatorsDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductClassIndicatorsDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public ProductClassIndicatorsDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

    }
}

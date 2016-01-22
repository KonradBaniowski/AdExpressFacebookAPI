#region Informations
/*
 * Author : G Ragneau
 * Created on 01/08/2008
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.ProductClassIndicators.Exceptions
{
	/// <summary>
	/// Excepion class for product class indicator
	/// </summary>
	public class ProductClassIndicatorsException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProductClassIndicatorsException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProductClassIndicatorsException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public ProductClassIndicatorsException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

    }
}

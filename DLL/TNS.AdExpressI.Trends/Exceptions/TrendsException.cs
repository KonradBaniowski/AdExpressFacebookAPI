#region Informations
/*
 * Author : D Mussuma
 * Created on 20/11/2009
 * Modifications:
 *      Author - Date - Description
 * 
 *  
 */
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Trends.Exceptions
{
	/// <summary>
	/// Excepion class for present/absent report
	/// </summary>
	public class TrendsException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TrendsException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TrendsException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public TrendsException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

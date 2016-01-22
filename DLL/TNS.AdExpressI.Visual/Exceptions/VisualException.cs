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

namespace TNS.AdExpressI.Visual.Exceptions
{
	/// <summary>
	/// Excepion class for Visual
	/// </summary>
	public class VisualException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public VisualException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public VisualException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
        public VisualException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

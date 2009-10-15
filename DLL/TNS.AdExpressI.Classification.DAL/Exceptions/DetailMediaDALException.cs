#region Information
/*
 * Author : D. Mussuma
 * Created on : 15/07/2009
 * Modification:
 *      Author - Date - Description
 * 
 * 
 */
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpressI.Classification.DAL.Exceptions {
	/// <summary>
	/// Excepion class for detail  media data access
	/// </summary>
	public class DetailMediaDALException : BaseException {
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DetailMediaDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DetailMediaDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DetailMediaDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

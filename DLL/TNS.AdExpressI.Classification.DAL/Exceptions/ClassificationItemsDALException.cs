#region Information
/*
 * Author :  D. Mussuma
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
	/// Excepion class for Classification Items DAL
	/// </summary>
	public class ClassificationItemsDALException : BaseException {
		
		#region Constructors
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ClassificationItemsDALException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ClassificationItemsDALException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ClassificationItemsDALException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion
	}
}

#region Informations
// Auteur: D. V. Mussuma
// Date de création: 12/10/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Exceptions Fonctions
	/// </summary>
	public class FunctionsException:BaseException{
		
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public FunctionsException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public FunctionsException(string message):base(message){
		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public FunctionsException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

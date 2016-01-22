#region Informations
// Auteur: G. Ragneau
// Date de création: 24/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// UI: Fonctions
	/// </summary>
	public class FunctionsUIException:BaseException{
		
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public FunctionsUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public FunctionsUIException(string message):base(message){
		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public FunctionsUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

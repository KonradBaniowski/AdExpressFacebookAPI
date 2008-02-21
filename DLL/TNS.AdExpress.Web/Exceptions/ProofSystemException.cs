#region Informations
// Auteur: D. Mussuma 
// Date de création: 30/01/2007 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// ProofSystemException thrown whenever an error occured while processing data for Justificatis Presse module
	/// </summary>
	public class ProofSystemException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ProofSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ProofSystemException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ProofSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

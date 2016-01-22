#region Informations
// Auteur: K. Shehzad
// Date de création: 20/07/2005 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// IHM Synthèses
	/// </summary>
	public class SynthesisUIException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SynthesisUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message"></param>
		public SynthesisUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SynthesisUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

#region Informations
// Auteur: G. Facon
// Date de création: 19/07/2005 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Bases de données synthèse
	/// </summary>
	public class SynthesisDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SynthesisDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SynthesisDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SynthesisDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
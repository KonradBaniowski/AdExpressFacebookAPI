#region Informations
// Auteur: G. Facon
// Date de création: 09/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Appm.Exceptions{
	/// <summary>
	/// Exception
	/// </summary>
	public class AppmConfigDataAccessException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AppmConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AppmConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public AppmConfigDataAccessException(string message, System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}
#region Informations
// Auteur: G. Facon
// Date de cr�ation: 02/06/2005
// Date de modification: 02/06/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception du serveur de demande de r�sultat
	/// </summary>
	public class ParameterDataAccessException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ParameterDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ParameterDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ParameterDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}
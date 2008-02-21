#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception du chargement de la configuration du reseau du serveur
	/// </summary>
	public class NetworkConfigurationDataAccessException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public NetworkConfigurationDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public NetworkConfigurationDataAccessException(string message):base(message){
				
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public NetworkConfigurationDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
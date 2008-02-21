#region Informations
// Auteur: G. Facon
// Date de cr�ation: 18/05/2005
// Date de modification: 18/05/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Exceptions{
	/// <summary>
	/// Exception de la configuration du reseau du serveur
	/// </summary>
	public class NetworkConfigurationException:BaseException{
			
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public NetworkConfigurationException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public NetworkConfigurationException(string message):base(message){
				
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public NetworkConfigurationException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
#region Informations
// Auteur: D. V. Mussuma
// Date de création: 24/11/2005
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception de l'affichage des statistiques clients
	/// </summary>
	public class ClientUIException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ClientUIException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ClientUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ClientUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

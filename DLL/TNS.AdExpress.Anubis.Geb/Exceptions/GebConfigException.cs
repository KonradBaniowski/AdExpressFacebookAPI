#region Informations
// Auteur : B.Masson
// Date de création : 21/04/2006
// Date de modification :
#endregion

using System;

namespace TNS.AdExpress.Anubis.Geb.Exceptions{
	/// <summary>
	/// Classe d'exception du chargement de la configuration du plugin Geb (GebConfigException)
	/// </summary>
	public class GebConfigException:System.Exception{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public GebConfigException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public GebConfigException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception</param>
        public GebConfigException(string message, System.Exception innerException)
            : base(message, innerException) {
		}
		#endregion

	}
}

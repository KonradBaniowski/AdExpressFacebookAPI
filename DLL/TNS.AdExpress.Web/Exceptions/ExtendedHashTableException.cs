#region Informations
// Auteur: G. RAGNEAU
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Gestion des exceptions de la hashtable de gestion d'index de tableaux pour la génération des tableaux dynamiques
	/// </summary>
	public class ExtendedHashTableException:System.ArgumentException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ExtendedHashTableException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public ExtendedHashTableException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ExtendedHashTableException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
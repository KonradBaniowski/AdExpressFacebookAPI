#region Informations
/* Auteur: G. RAGNEAU
 * Date de création:
 * Date de modification: 11/08/2005
 *      24/07/2008 - G Ragneau - Moved from TNS.AdExpress.Web
 * */
#endregion

using System;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Error managment in ExtendedHashtable class
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
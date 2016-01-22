#region Informations
// Auteur: D. Mussuma
// Date de création:
// Date de modification: 22/01/2007
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Gestion des exceptions de la classe d'accès aux données des pays
	/// </summary>
	public class CountryListDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CountryListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CountryListDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CountryListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

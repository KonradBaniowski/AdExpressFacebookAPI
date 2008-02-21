#region Informations
// Auteur: ?
// Date de cr�ation:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de donn�es: APPM Emplacement du plan
	/// </summary>
	public class LocationPlanTypesDataAccessException : BaseException{
	
		#region Constructeurs
		/// <summary>
		/// Constructeur
		/// </summary>
		public LocationPlanTypesDataAccessException(){

		}
		
		/// <summary>
		/// constructeur
		/// </summary>
		/// <param name="message">message</param>
		public LocationPlanTypesDataAccessException(string message):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public LocationPlanTypesDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

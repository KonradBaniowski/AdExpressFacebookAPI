#region Informations
// Auteur: G. Facon 
// Date de création: 22/07/2005
// Date de modification: 22/07/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{

	/// <summary>
	/// Erreur lors de la sélection des dates dans le portefeuille
	/// </summary>
	public class PortofolioDataAccessException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PortofolioDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PortofolioDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

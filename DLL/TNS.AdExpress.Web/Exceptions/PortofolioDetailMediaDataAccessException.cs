#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 09/12/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{

	/// <summary>
	/// Erreur lors du r�sultat du d�tail du portefeuille d'un support
	/// </summary>
	public class  PortofolioDetailMediaDataAccessException  :BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioDetailMediaDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PortofolioDetailMediaDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PortofolioDetailMediaDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

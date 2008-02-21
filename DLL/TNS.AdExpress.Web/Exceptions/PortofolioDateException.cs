#region Informations
// Auteur: G. Facon 
// Date de création: 22/07/2005
// Date de modification: 22/07/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Exception sur la date dans le module portefeuille
	/// </summary>
	public class PortofolioDateException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioDateException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PortofolioDateException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PortofolioDateException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

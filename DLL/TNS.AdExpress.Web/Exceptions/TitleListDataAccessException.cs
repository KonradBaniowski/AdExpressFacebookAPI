#region Informations
// Auteur: D. Mussuma
// Date de création:
// Date de modification: 21/07/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Gestion des exceptions de TitleListDataAccessException
	/// </summary>
	public class TitleListDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TitleListDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TitleListDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TitleListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

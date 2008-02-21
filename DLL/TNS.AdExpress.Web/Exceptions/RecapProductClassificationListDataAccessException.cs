#region Informations
// Auteur: D. Mussuma 
// Date de cr�ation: 08/09/2004 
// Date de modification: 08/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;


namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	///  Base de donn�es: Liste de produits pour les analyses sectorielles
	/// </summary>
	public class RecapProductClassificationListDataAccessException:BaseException{
		
		#region Constructeurs
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RecapProductClassificationListDataAccessException(): base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RecapProductClassificationListDataAccessException(string message): base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RecapProductClassificationListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

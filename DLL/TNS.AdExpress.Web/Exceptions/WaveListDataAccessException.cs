#region Informations
// Auteur: D. V. Mussuma
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Classe de gestion des exceptions lors du chargement des vagues 
	/// </summary>
	public class WaveListDataAccessException : BaseException{

		#region Constructeurs
		/// <summary>
		/// constructeur	
		/// </summary>
		public WaveListDataAccessException(): base(){			
		}

		/// <summary>
		/// constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WaveListDataAccessException(string message): base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WaveListDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

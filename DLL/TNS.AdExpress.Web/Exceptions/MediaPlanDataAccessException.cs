#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 07/12/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DA: Classe d'exception de l'obtention des donn�es pour les plans m�dia
	/// </summary>
	public class MediaPlanDataAccessException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPlanDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPlanDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPlanDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

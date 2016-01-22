#region Informations
// Auteur: G. Facon 
// Date de cr�ation: 08/12/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// UI: Classe d'exception de la g�n�ration des donn�es pour les plans m�dia
	/// </summary>
	public class MediaPlanUIException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPlanUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPlanUIException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPlanUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

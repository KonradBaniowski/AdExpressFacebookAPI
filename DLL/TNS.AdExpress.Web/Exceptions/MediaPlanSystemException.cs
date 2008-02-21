#region Informations
// Auteur: G. Facon
// Date de création: 29/10/2004 
// Date de modification: 29/10/2004 (G. Facon)
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Exception lors de la création du résultat d'un plan média
	/// </summary>
	public class MediaPlanSystemException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaPlanSystemException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public MediaPlanSystemException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public MediaPlanSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

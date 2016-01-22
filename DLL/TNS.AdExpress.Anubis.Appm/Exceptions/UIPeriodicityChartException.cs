#region Informations
// Auteur: G. Ragneau
// Date de création: 29/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Appm.Exceptions{
	/// <summary>
	/// Exception levée à la génération du graphique APPM pour pdf "Périodicité"
	/// </summary>
	public class UIPeriodicityChartException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public UIPeriodicityChartException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public UIPeriodicityChartException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public UIPeriodicityChartException(string message, System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}
#region Informations
// Auteur: G. Ragneau
// Date de création: 30/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Appm.Exceptions{
	/// <summary>
	/// Exception levée à la génération du graphique APPM pour pdf "PDV Plan"
	/// </summary>
	public class UIPDVPlanChartException:BaseException{
			
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public UIPDVPlanChartException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public UIPDVPlanChartException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="err">Exception relative</param>
		/// <param name="innerException">Exception d'origine</param>
		public UIPDVPlanChartException(string message, System.Exception innerException):base(message,innerException){
		}


		#endregion
	}
}
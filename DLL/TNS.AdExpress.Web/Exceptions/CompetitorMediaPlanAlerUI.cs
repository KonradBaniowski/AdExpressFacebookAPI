#region Informations
// Auteur: G. FACON
// Date de cr�ation:06/10/2005
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// UI:Classe d'exception de la g�n�ration des donn�es pour l'alert plan m�dia concurentiel
	/// </summary>
	public class CompetitorMediaPlanAlerUI:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public CompetitorMediaPlanAlerUI():base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public CompetitorMediaPlanAlerUI(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public CompetitorMediaPlanAlerUI(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

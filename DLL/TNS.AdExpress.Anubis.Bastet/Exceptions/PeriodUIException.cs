#region Informations
// Auteur: D. V. Mussuma
// Date de création: 16/12/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception de l'affichage des de la période
	/// </summary>
	public class  PeriodUIException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public  PeriodUIException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public  PeriodUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public  PeriodUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

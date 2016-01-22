#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 18/11/2005
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception d'acc�s des donn�es des statistiques cliets
	/// </summary>
	public class BastetExcelException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public BastetExcelException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public BastetExcelException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public BastetExcelException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

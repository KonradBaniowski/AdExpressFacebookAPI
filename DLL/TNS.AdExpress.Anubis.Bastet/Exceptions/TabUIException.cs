#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 16/12/2006
// Date de modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Bastet.Exceptions{
	/// <summary>
	/// Classe d'exception de l'affichage des onglets les plus utilis�s
	/// </summary>
	public class TabUIException:BaseException{
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TabUIException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TabUIException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TabUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

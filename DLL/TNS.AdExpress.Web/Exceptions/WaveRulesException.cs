#region Informations
// Auteur: D. V. Mussuma
// Date de cr�ation: 16/08/2005
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// Classe de gestion des exceptions lors de le s�lection automatique d'une vague.
	/// </summary>
	public class WaveRulesException : BaseException{	
		

		#region Constructeurs
		/// <summary>
		/// constructeur
		/// </summary>
		public WaveRulesException(): base(){			
		}

		/// <summary>
		/// constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public WaveRulesException(string message): base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public WaveRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

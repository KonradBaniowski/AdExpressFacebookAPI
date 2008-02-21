#region Informations
// Auteur: D. Mussuma 
// Date de cr�ation: 08/09/2004 
// Date de modification: 08/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;


namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	///  Classe d'exception de la g�n�ration des donn�es pour les vari�t�s 
	/// </summary>
	public class RecapAdvertiserDataAccessException:BaseException{

		#region Constructeurs

		/// <summary>
		/// Constructeur de base
		/// </summary>
		public RecapAdvertiserDataAccessException(): base(){			
		}
		
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public RecapAdvertiserDataAccessException(string message): base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public RecapAdvertiserDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion


	}
}

#region Informations
// Auteur: D. V. Mussuma, Y. Rkaina
// Date de création: 23/05/2006
// Date de modification:
#endregion


using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Sobek.Exceptions
{
	/// <summary>
	/// Description résumée de SobekConfigDataAccessException.
	/// </summary>
	public class SobekConfigDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SobekConfigDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SobekConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SobekConfigDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

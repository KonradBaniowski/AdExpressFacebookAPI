#region Informations
// Auteur: D. V. Mussuma, Y. Rkaina
// Date de cr�ation: 23/05/2006
// Date de modification:
#endregion


using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Sobek.Exceptions{
	/// <summary>
	/// Description r�sum�e de SobekTextFileSystemException.
	/// </summary>
	public class TefnoutExcelSystemException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public TefnoutExcelSystemException():base(){
	    }

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public TefnoutExcelSystemException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public TefnoutExcelSystemException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

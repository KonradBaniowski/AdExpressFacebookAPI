#region Informations
// Auteur: Y. Rkaina
// Date de cr�ation: 10/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Selket.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class SelketPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SelketPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public SelketPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public SelketPdfException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}
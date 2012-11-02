#region Informations
// Auteur: Y. Rkaina
// Date de création: 10/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Amon.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class AmonPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AmonPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AmonPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public AmonPdfException(string message, Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}

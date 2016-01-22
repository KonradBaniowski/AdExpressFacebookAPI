#region Informations
// Auteur: Y. Rkaina
// Date de création: 10/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Ptah.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class PtahPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PtahPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public PtahPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public PtahPdfException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}

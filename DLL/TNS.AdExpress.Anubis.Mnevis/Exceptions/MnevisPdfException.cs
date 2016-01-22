#region Informations
// Auteur: Y. Rkaina
// Date de création: 25/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Mnevis.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class MnevisPdfException:BaseException {

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MnevisPdfException():base()	{			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public MnevisPdfException(string message):base(message) {
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public MnevisPdfException(string message, System.Exception innerException):base(message,innerException)  {
		}


		#endregion
	}
}

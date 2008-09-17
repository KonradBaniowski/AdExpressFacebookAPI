#region Informations
// Auteur: Y. Rkaina
// Date de création: 10/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Miysis.Exceptions{
	/// <summary>
	/// Exception
	/// </summary>
	public class MiysisPdfException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MiysisPdfException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public MiysisPdfException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
		public MiysisPdfException(string message, System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

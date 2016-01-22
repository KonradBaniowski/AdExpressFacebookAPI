#region Informations
// Auteur: Y. Rkaina
// Date de cr�ation: 10/08/2006
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Dedoum.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class DedoumConfigDataAccessException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DedoumConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public DedoumConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public DedoumConfigDataAccessException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}



using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Selket.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class SelketConfigDataAccessException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SelketConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public SelketConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public SelketConfigDataAccessException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}



using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Ptah.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class PtahConfigDataAccessException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PtahConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public PtahConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public PtahConfigDataAccessException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}



using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Thoueris.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class ThouerisConfigDataAccessException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ThouerisConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ThouerisConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public ThouerisConfigDataAccessException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}

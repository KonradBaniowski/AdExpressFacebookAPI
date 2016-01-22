using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Anubis.Amon.Exceptions
{
	/// <summary>
	/// Exception
	/// </summary>
	public class AmonConfigDataAccessException:BaseException{
	
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public AmonConfigDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public AmonConfigDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		/// <param name="innerException">Exception d'origine</param>
        public AmonConfigDataAccessException(string message, System.Exception innerException)
            : base(message, innerException)
        {
		}
		#endregion

	}
}

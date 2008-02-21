#region Information
// Date de création : A.Obermeyer 4/01/2004
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Erreur dans le DataAccess de l'analyse portefeuille 
	/// </summary>
	public class PortofolioAnalysisDataAccessException:BaseException{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioAnalysisDataAccessException():base(){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PortofolioAnalysisDataAccessException(string message):base(message){
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PortofolioAnalysisDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

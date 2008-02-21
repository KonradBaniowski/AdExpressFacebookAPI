#region Informations
// Auteur: Y. R'kaina
// Date de création: 17/01/2007 
// Date de modification
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	///  Bases de données synthèse
	/// </summary>
	public class SectorDataSynthesisDataAccessException:BaseException{

		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SectorDataSynthesisDataAccessException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SectorDataSynthesisDataAccessException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SectorDataSynthesisDataAccessException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion

	}
}

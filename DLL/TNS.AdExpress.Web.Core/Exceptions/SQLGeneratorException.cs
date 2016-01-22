#region Informations
// Auteur: G. Facon 
// Date de création: 14/09/2004 
// Date de modification: 14/09/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Core.Exceptions{
	/// <summary>
	/// Exception SQLGenerator
	/// </summary>
	public class SQLGeneratorException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public SQLGeneratorException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public SQLGeneratorException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public SQLGeneratorException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

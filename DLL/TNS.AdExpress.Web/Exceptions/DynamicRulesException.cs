#region Informations
// Auteur: G. Facon 
// Date de création: 09/11/2004 
// Date de modification: 09/11/2004 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DynamicRulesException thrown whenever an error occured while processing data for dynamic analysis module
	/// </summary>
	public class DynamicRulesException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DynamicRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DynamicRulesException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DynamicRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

#region Informations
// Auteur: G. Ragneau 
// Date de création: 04/10/2004 
// Date de modification: 
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// DynamicTableRulesException thrown whenever an error occured while processing data for dynamic table module
	/// </summary>
	public class DynamicTableRulesException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DynamicTableRulesException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DynamicTableRulesException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DynamicTableRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

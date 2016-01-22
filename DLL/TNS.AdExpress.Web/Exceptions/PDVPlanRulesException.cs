#region Informations
// Author: K. Shehzad
// Date of creation: 02/08/2005 
// Date of modification:
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions{
	/// <summary>
	/// Base de données: Tableaux dynamiques
	/// </summary>
	public class PDVPlanRulesException:BaseException{
		
		#region Constructeur		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PDVPlanRulesException():base(){			
		}
		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public PDVPlanRulesException(string message):base(message){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PDVPlanRulesException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}

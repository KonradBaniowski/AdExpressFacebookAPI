#region Informations
// Auteur: G .RAGNEAU
// Date de création:
// Date de modification: 11/08/2005
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Exceptions
{
	/// <summary>
	/// DynamicTablesUIException thrown whenever an error occured while generating UI for sector analysis module
	/// </summary>
	public class DynamicTablesUIException:BaseException{
		
		#region Constructeur
		
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public DynamicTablesUIException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		public DynamicTablesUIException(string message):base(message){			
		}


		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public DynamicTablesUIException(string message,System.Exception innerException):base(message,innerException){
		}
		#endregion
	}
}
#region Informations
// Auteur: D. Mussuma
// Date de création: 18/07/2008
// Date de modification: 18/07/2008
#endregion

using System;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Description résumée de ModuleException.
	/// </summary>
	public class ResultsOptionsWebControlException:System.Exception{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public ResultsOptionsWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public ResultsOptionsWebControlException(string message)
			: base(message) {
		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public ResultsOptionsWebControlException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}

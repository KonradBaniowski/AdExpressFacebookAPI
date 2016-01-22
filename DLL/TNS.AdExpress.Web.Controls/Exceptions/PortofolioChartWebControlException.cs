#region Informations
// Auteur: D. Mussuma
// Date de création: 18/07/2008
// Date de modification: 18/07/2008
#endregion

using System;
using TNS.FrameWork.Exceptions;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Description résumée de ModuleException.
	/// </summary>
	public class PortofolioChartWebControlException : BaseException { 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public PortofolioChartWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public PortofolioChartWebControlException(string message)
			: base(message) {
		
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">Message d'erreur</param>
		/// <param name="innerException">Exception source</param>
		public PortofolioChartWebControlException(string message, System.Exception innerException)
			: base(message, innerException) {
		}
		#endregion
	}
}

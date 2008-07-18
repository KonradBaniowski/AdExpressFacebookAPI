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
	public class PortofolioChartWebControlException:System.Exception{ 
		
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


		#endregion
	}
}

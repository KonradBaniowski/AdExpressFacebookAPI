#region Informations
// Auteur: G. Facon
// Date de création: 04/05/2004
// Date de modification: 04/05/2004
#endregion

using System;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Description résumée de ModuleException.
	/// </summary>
	public class MediaSelectionWebControlException:System.Exception{ 
		
		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public MediaSelectionWebControlException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public MediaSelectionWebControlException(string message):base(message){
		
		}


		#endregion
	}
}

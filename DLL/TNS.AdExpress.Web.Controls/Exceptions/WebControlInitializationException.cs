#region Informations
// Auteur: G. Facon 
// Date de création: 14/05/2004 
// Date de modification: 14/05/2004 
#endregion

using System;

namespace TNS.AdExpress.Web.Controls.Exceptions{
	/// <summary>
	/// Exception lancée lors de l'initialisation d'un composant
	/// </summary>
	public class WebControlInitializationException:System.Exception{

		#region Constructeur
		/// <summary>
		/// Constructeur de base
		/// </summary>
		public WebControlInitializationException():base(){			
		}

		/// <summary>
		/// Constructeur
		/// </summary>
		/// <param name="message">message d'erreur</param>
		public WebControlInitializationException(string message):base(message){
		}
		#endregion
	}
}

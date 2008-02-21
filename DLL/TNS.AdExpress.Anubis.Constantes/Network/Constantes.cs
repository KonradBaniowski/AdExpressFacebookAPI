#region Informations
// Auteur: G. Facon
// Date de création: 20/05/2005
// Date de modification: 20/05/2005
#endregion

using System;

namespace TNS.AdExpress.Anubis.Constantes.Network{

	/// <summary>
	/// Constantes liées au serveur
	/// </summary>
	public class Server{
		/// <summary>
		/// Code retourné par le serveur
		/// </summary>
		public enum code{
			/// <summary>
			/// Le message a bien été reçu et sera traité
			/// </summary>
			ok,
			/// <summary>
			/// Un problème est survenu
			/// </summary>
			ko
		}
	}
}

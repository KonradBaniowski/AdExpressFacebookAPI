#region Informations
// Auteur: G. Facon
// Date de cr�ation: 20/05/2005
// Date de modification: 20/05/2005
#endregion

using System;

namespace TNS.AdExpress.Anubis.Constantes.Network{

	/// <summary>
	/// Constantes li�es au serveur
	/// </summary>
	public class Server{
		/// <summary>
		/// Code retourn� par le serveur
		/// </summary>
		public enum code{
			/// <summary>
			/// Le message a bien �t� re�u et sera trait�
			/// </summary>
			ok,
			/// <summary>
			/// Un probl�me est survenu
			/// </summary>
			ko
		}
	}
}

#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

using System;

namespace TNS.AdExpress.Anubis.Constantes.Application{

	/// <summary>
	/// Constantes de la configuration de l'application
	/// </summary>
	public class Configuration{
		/// <summary>
		/// Fichier de configuration du reseau
		/// </summary>
		public const string NETWORK_FILE="Network.xml";
		/// <summary>
		/// Fichier de configuration des plug-ins
		/// </summary>
		public const string PLUGINS_FILE="Plugins.xml";
		/// <summary>
		/// Fichier de configuration des plug-ins
		/// </summary>
		public const string DATABASE_FILE="DataBase.xml";
		/// <summary>
		/// Fichier de configuration de l'application Anubis
		/// </summary>
		public const string ANUBIS_CONFIGURATION_FILE="Anubis.xml";

		/// <summary>
		/// Non du répertoire contenant les configurations
		/// </summary>
		public const string CONFIGURATION_DIRECTORY="Configuration";

	}
}

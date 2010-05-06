using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Ares.Constantes {
    /// <summary>
    /// Classe définnissant le chemin d'accès au fichier xml
    /// </summary>
    public class ConfigurationFile {
        /// <summary>
        /// Fichier de configuration Nyx
        /// </summary>
        public const string REQUESTER_CONFIGURATION_FILENAME = @"RequesterConfiguration.xml";
        /// <summary>
        /// Web parameters configuration file
        /// </summary>
        public const string DATABASE_CONFIGURATION_FILENAME = @"Database.xml";
        /// <summary>
        /// Fichier de configuration des plugins
        /// </summary>
        public const string PLUGIN_CONFIGURATION_FILENAME = @"PluginConfiguration.xml";
        /// <summary>
        /// Fichier de configuration d'un service client au LS
        /// </summary>
        public const string LS_CLIENT_CONFIGURATION_FILENAME = @"LsClientConfiguration.xml";
        /// <summary>
        /// Fichier de configuration d'un service client au LS
        /// </summary>
        public const string LS_SERVER_CONFIGURATION_FILENAME = @"LinkServerParameters.xml";
        /// <summary>
        /// Fichier de configuration d'une application LS
        /// </summary>
        public const string APPLICATION_PARAMETERS_CONFIGURATION_FILENAME = @"ApplicationParameters.xml";

    }
}

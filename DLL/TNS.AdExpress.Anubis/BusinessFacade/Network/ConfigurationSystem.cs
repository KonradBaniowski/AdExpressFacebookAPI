#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

using System;
using System.IO;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisCommon=TNS.AdExpress.Anubis.Common;
using TNSAnubisDA=TNS.AdExpress.Anubis.DataAccess;

namespace TNS.AdExpress.Anubis.BusinessFacade.Network{
	/// <summary>
	/// Chargement de la configuration du reseau pour le serveur
	/// </summary>
	public class ConfigurationSystem{

		/// <summary>
		/// Chargement de la configuration du reseau pour le serveur
		/// </summary>
		/// <param name="xmlFilePath">Chemin du fichier de configuration</param>
		/// <returns>Configuration du reseau</returns>
		public static TNSAnubisCommon.Network.Configuration Load(string xmlFilePath){
			if(xmlFilePath==null)throw(new ArgumentNullException("Le chemin du fichier de configuration du reseau est null"));
			if(!File.Exists(xmlFilePath)) throw(new FileNotFoundException("Le chemin du fichier de configuration du reseau n'est pas valide"));
			try{
				return(TNSAnubisDA.Network.ConfigurationDataAccess.Load(xmlFilePath));
			}
			catch(System.Exception err){
				throw(new TNSAnubisExceptions.NetworkConfigurationSystemException("Impossible de charger la configuration",err));
			}	
		}
	}
}

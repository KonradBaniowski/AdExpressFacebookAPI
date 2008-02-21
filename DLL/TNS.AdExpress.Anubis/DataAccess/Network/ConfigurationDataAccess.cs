#region Informations
// Auteur: G. Facon
// Date de création: 19/05/2005
// Date de modification: 19/05/2005
#endregion

using System;
using System.Xml;
using System.IO;
using TNSAnubisExceptions=TNS.AdExpress.Anubis.Exceptions;
using TNSAnubisCommon=TNS.AdExpress.Anubis.Common;

namespace TNS.AdExpress.Anubis.DataAccess.Network{
	/// <summary>
	/// Chargement du fichier de configuration du reseau pour le serveur
	/// </summary>
	public class ConfigurationDataAccess{

		/// <summary>
		/// Méthode static qui charge le fichier de configuration du reseau pour le serveur
		/// </summary>
		/// <param name="xmlFilePath">Chemin du fichier XML</param>	
		/// <returns>Configuration du reseau pour le serveur</returns>
		internal static TNSAnubisCommon.Network.Configuration Load(string xmlFilePath){

			#region Varaibles
			XmlTextReader reader=null;
			string name="";
			string ip=null;
			int port=-1;
			#endregion

			try{
				#region Lecture du fichier
				reader=new XmlTextReader(xmlFilePath);
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "networkConfiguration":
								if(reader.GetAttribute("name")!=null)name=reader.GetAttribute("name");
								if(reader.GetAttribute("ip")!=null)ip=reader.GetAttribute("ip");
								if(reader.GetAttribute("port")!=null)port=int.Parse(reader.GetAttribute("port"));
								break;
						}
					}
				}
				#endregion

				#region Fermeture du fichier
				if(reader!=null)reader.Close();
				#endregion

				return(new TNSAnubisCommon.Network.Configuration(name,ip,port));
			}
			#region Traitement des erreurs
			catch(System.Exception err){
				throw(new TNSAnubisExceptions.NetworkConfigurationDataAccessException("Impossible de charger le fichier de configuration du reseau",err));
			}
			#endregion
		}
	}
}

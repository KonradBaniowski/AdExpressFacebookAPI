#region Informations
// Auteur : B.Masson
// Date de création : 13/03/2007
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.HermesPress.Exceptions;

namespace TNS.AdExpress.Anubis.HermesPress.DataAccess{
	/// <summary>
	/// Classe du chargement de la configuration du fichier de config du plugin Hermes Presse
	/// </summary>
	public class ConfigurationDataAccess{
		
		/// <summary>
		/// Chargement du fichier XML
		/// </summary>
		/// <param name="source">Source de données</param>
		/// <param name="configuration">Objet Configuration</param>
		internal static void Load(IDataSource source, Common.Configuration configuration){

			#region Variables
			XmlTextReader Reader;
			string valueXml="";
			#endregion

			try{
				Reader=(XmlTextReader)source.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "CustomerMail":
								valueXml=Reader.GetAttribute("server");
								if (valueXml!=null) 
									configuration.CustomerMailServer = valueXml;
								valueXml=Reader.GetAttribute("port");
								if (valueXml!=null) 
									configuration.CustomerMailPort = int.Parse(valueXml);
								valueXml=Reader.GetAttribute("from");
								if (valueXml!=null) 
									configuration.CustomerMailFrom =valueXml;
								valueXml=Reader.GetAttribute("configurationReportFilePath");
								if (valueXml!=null) 
									configuration.ConfigurationReportFilePath = valueXml;
								break;
						}
					}
				}
			}
			catch(System.Exception err){
				throw(new ConfigurationDataAccessException("Impossible de charger la configuration du plugin HermesRadioTV",err));
			}
		}

	}
}

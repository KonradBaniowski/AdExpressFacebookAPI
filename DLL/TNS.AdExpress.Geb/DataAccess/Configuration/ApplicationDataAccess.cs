#region Informations
// Auteur : B.Masson
// Date de création : 12/04/2006
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Xml;
using GebConfiguration=TNS.AdExpress.Geb.Configuration;
using GebExceptions=TNS.AdExpress.Geb.Exceptions;

namespace TNS.AdExpress.Geb.DataAccess.Configuration{
	/// <summary>
	/// Classe du chargement de la fréquence
	/// </summary>
	public class ApplicationDataAccess{
		
		/// <summary>
		/// Charge les mails destinataires
		/// </summary>
		/// <param name="xmlFilePath">Chemin du fichier</param>
		/// <returns>Liste des mails destinataires</returns>
		public static GebConfiguration.Application Get(string xmlFilePath){

			#region Variables
			XmlTextReader Reader;
			string valueXml="";
			#endregion

			try{
				Reader=new XmlTextReader(xmlFilePath);
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "frequency":
								valueXml = Reader.GetAttribute("value");
								if(valueXml==null) throw(new GebExceptions.ApplicationDataAccessException("Impossible de charger la fréquence"));
								if(valueXml.Length==0) throw(new GebExceptions.ApplicationDataAccessException("Impossible de charger la fréquence"));
								break;
						}
					}
				}
				Reader.Close();
				return(new GebConfiguration.Application(int.Parse(valueXml)));
			}
			catch(System.Exception err){
				throw (new GebExceptions.ApplicationDataAccessException("Impossible de charger la fréquence dans le fichier de configuration",err));
			}
		}

	}
}

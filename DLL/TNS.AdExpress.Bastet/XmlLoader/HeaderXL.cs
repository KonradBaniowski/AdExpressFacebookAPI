#region Informations
// Auteur: B. Masson
// Date de création: 23/02/2007
// Date de modification: 
#endregion

using System;
using System.Xml;
using System.Collections;
using TNS.AdExpress.Bastet.Common.Headers;
using TNS.AdExpress.Bastet.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Bastet.XmlLoader {
	/// <summary>
	/// Classe pour le chargement du header
	/// </summary>
    public class HeaderXL {
		
		#region Chargement du header xml
		/// <summary>
		/// Méthode static qui renseigne une Hashtable contenant des Header à partir des données d'un fichier XML
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		internal static Hashtable GetDescription(IDataSource source){
			Hashtable headers=new Hashtable(); 
			string name="";
			XmlTextReader reader=null;
			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							case "header":
								if (reader.GetAttribute("name")!=null){
									name = reader.GetAttribute("name");
									headers.Add((name=reader.GetAttribute("name")), new Header());
								}
								break;
							case "menuItem":
								if (name.CompareTo("")!=0 && reader.GetAttribute("name")!=null && reader.GetAttribute("name").Length>0
									&& reader.GetAttribute("idWebText")!=null && reader.GetAttribute("idWebText").Length>0
									&& reader.GetAttribute("Text")!=null && reader.GetAttribute("Text").Length>0
									&& reader.GetAttribute("href")!=null && reader.GetAttribute("href").Length>0
									&& reader.GetAttribute("id")!=null && reader.GetAttribute("id").Length>0
									&& reader.GetAttribute("display")!=null && reader.GetAttribute("display").Length>0){

									((Header)(headers[name])).MenuItems.Add(
										new HeaderMenuItem(
										int.Parse(reader.GetAttribute("id")),
										bool.Parse(reader.GetAttribute("display")),
										Int64.Parse(reader.GetAttribute("idWebText")),
										reader.GetAttribute("Text"),
										reader.GetAttribute("href")));
								
								}
								break;
						}
					}				
				}
				name="";

				#region Fermeture du fichier
				if(source.GetSource()!=null)source.Close();
				#endregion
			
			}
			#region Traitement des erreurs
			catch(System.Exception e){

				#region Fermeture du fichier
				if(source.GetSource()!=null)source.Close();
				#endregion

				throw(new HeaderDataAccessException(" Erreur : "+e.Message)); 
			}		
			#endregion
			
			return (headers);
		}
		#endregion

	}
}

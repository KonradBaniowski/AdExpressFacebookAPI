#region Informations
// Author:  
// Creation Date: 
// Modification Date:
//	11/08/2005	G. Facon	New Exception Management and property name
#endregion

using System;
using System.Data;
using System.Xml;
using System.IO;
using System.Collections;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.Core.Exceptions;


namespace TNS.AdExpress.Web.Core.DataAccess.Navigation{


	///<summary>
	/// Class used to load the data relating to the page headings
	/// </summary>
	///  <stereotype>utility</stereotype>
	public class HeaderDataAccess {	

		#region Constructor
		/// <summary>
		/// Constructor
		/// </summary>
		static HeaderDataAccess(){
		}
		#endregion

		#region Load XML file
		
		/// <summary>
		/// static Method which load from a file XML the Hashtable containing Headers 
		/// </summary>
		/// <param name="pathXMLFile">Xml file path</param>	
		/// Hashtable instanciée qui va être renseigné avec des objets Header contenant les spécifications de chaque entêtes du site
		/// <param name="headers">Instanciate Hashtable which will be indicated the specifications of each Headers objects</param>
		public static void Load(string pathXMLFile, Hashtable headers){
			bool giveLanguage=false;
			bool giveSession=false;
			string target="";
            bool displayInPopUp = false;
			XmlTextReader reader=null;
			try{
				string path=pathXMLFile;
				if(File.Exists(path)){
					reader=new XmlTextReader(path);
					string name="";
					while(reader.Read()){
						if(reader.NodeType==XmlNodeType.Element){
							switch(reader.LocalName){
								case "header":
									if (reader.GetAttribute("name")!=null && reader.GetAttribute("imgUrl")!=null && reader.GetAttribute("flashUrl")!=null){
										name = reader.GetAttribute("name");
										headers.Add((name=reader.GetAttribute("name")), new Header(reader.GetAttribute("imgUrl"), reader.GetAttribute("flashUrl"),reader.GetAttribute("missingFlashUrl")));
									}
									break;
								case "menuItem":
									giveLanguage=false;
									giveSession=false;
									target="";
                                    displayInPopUp = false;
									if (name.CompareTo("")!=0 && reader.GetAttribute("name")!=null && reader.GetAttribute("idWebText")!=null && reader.GetAttribute("href")!=null){
										if(reader.GetAttribute("language")!=null){
											giveLanguage=bool.Parse(reader.GetAttribute("language"));
										}
										if(reader.GetAttribute("idSession")!=null){
											giveSession=bool.Parse(reader.GetAttribute("idSession"));
										}
										if(reader.GetAttribute("target")!=null){
											target=reader.GetAttribute("target");
										}
                                        if(reader.GetAttribute("displayInPopUp") != null) {
                                            displayInPopUp = bool.Parse(reader.GetAttribute("displayInPopUp"));
                                        }
                                        ((Header)(headers[name])).MenuItems.Add(new HeaderMenuItem(Int64.Parse(reader.GetAttribute("idWebText")), reader.GetAttribute("href"), giveLanguage, giveSession, target, displayInPopUp));
									}
									break;
							}					
						}				
					}
					name="";
					#region Close the file
					if(reader!=null)reader.Close();
					#endregion
				}
			}

				#region Errors management
			catch(System.Exception e){
				#region Close the file
				if(reader!=null)reader.Close();
				#endregion
				throw(new HeaderException("Impossible to load header XML file",e)); 
			}		
			#endregion
			
		}
		#endregion

	}
}

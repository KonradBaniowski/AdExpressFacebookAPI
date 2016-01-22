#region Information
// Auteur G. Facon
// Date de création: 05/08/2005
/*Date de modification:
 *	11/08/2005 : G. RAGNEAU - exception
*/

#endregion


using System;
using System.Xml;
using System.IO;
using System.Collections;
using TNS.AdExpress.Anubis.Common.Configuration;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Anubis.Exceptions;

namespace TNS.AdExpress.Anubis.DataAccess.Configuration{
	/// <summary>
	/// Description résumée de PluginsDataAccess.
	/// </summary>
	public class PluginsDataAccess{
			
		/// <summary>
		/// Charge la liste des plug-ins à traiter à partir d'un fichier XML
		/// </summary>
		/// <returns>Liste des plug-ins</returns>
		internal static Hashtable GetPlugins(IDataSource dataSource){
			Hashtable list=new Hashtable();
			XmlTextReader Reader;
			string Value;
			string name="";
			string assemblyName="";
			string classe="";
			string configurationFilePath="";
			string resultsRoot = "";
			int resultType=0;
			int longevity=-1;
			try{
				Reader=(XmlTextReader)dataSource.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						name="";
						assemblyName="";
						configurationFilePath="";
						classe="";
						longevity=-1;
						resultsRoot="";

						switch(Reader.LocalName){
							case "plugin":
								//Nom du plug-in
								Value=Reader.GetAttribute("name");
								if(Value!=null) name=Value;

								//Type de résultat correspondant au plu-in
								Value=Reader.GetAttribute("resultType");
								if(Value!=null) resultType=int.Parse(Value);

								// Non de l'assembly du plug-in
								Value=Reader.GetAttribute("assembly");
								if(Value!=null)assemblyName=Value;

								// Non de la classe de démarrage
								Value=Reader.GetAttribute("class");
								if(Value!=null)classe=Value;

								// Chemin du fichier de configuration
								Value=Reader.GetAttribute("configurationFilePath");
								if(Value!=null)configurationFilePath=Value;

								//Longévité du résultat.
								Value=Reader.GetAttribute("resultLongevity");
								if(Value!=null) longevity=int.Parse(Value);

								// Non de l'assembly du plug-in
								Value=Reader.GetAttribute("resultsRoot");
								if(Value!=null)resultsRoot=Value;

								// On ajoute le plug-in à la liste
								list.Add(resultType,new Plugin(name,resultType,assemblyName,classe,configurationFilePath,resultsRoot,longevity)); 
								break;
						}
					}
				}
				Reader.Close();
				return(list);
			}
			catch(System.Exception err){
				throw (new PluginsDataAccessException("Impossible de charger une des plug-in à partir du fichier",err));
			}
		}
	}
}

#region Informations
// Author: B.Masson
// Creation Date: 26/01/2007
// Modification Date:
#endregion

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.IO;
using TNS.AdExpress.Web.Core.Translation;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Web.Core.Navigation;
using TNS.AdExpress.Web.Core;

namespace TNS.AdExpress.Web.Core.DataAccess.Navigation{
	/// <summary>
	/// Class used to know the data of a module group
	/// </summary>
	public class ModuleCategoryListDataAccess{
		
		#region Load Xml file
		/// <summary>
		/// Load module group list
		/// </summary>
		/// <param name="pathXMLFile">Xml file path</param>
		/// <param name="HtModuleCategory">Module categories list</param>
		public static void Load(string pathXMLFile, Hashtable HtModuleCategory){
			XmlTextReader Reader=null;
			HtModuleCategory.Clear();
			try{
				string path=AppDomain.CurrentDomain.BaseDirectory+@"config\"+pathXMLFile;
				if(File.Exists(path)){
					Reader=new XmlTextReader(path);

					while(Reader.Read()){
						if(Reader.NodeType==XmlNodeType.Element){
							switch(Reader.LocalName){
								case "moduleCategory":
									if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("webTextId")!=null){
										HtModuleCategory.Add(Int64.Parse(Reader.GetAttribute("id")), new ModuleCategory(Int64.Parse(Reader.GetAttribute("id")),Int64.Parse(Reader.GetAttribute("webTextId"))));
									}
									break;
							}
						}
					}
				}
			}
			catch(System.Exception e){
				throw(new ModuleCategoryListDataAccessException("Error : "+e.Message)); 
			}
			finally{
				#region Close the file
				if(Reader!=null)Reader.Close();
				#endregion
			}
		}
		#endregion
		
	}
}

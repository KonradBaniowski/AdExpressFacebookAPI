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
using TNS.AdExpress.Constantes;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain.XmlLoader{
	/// <summary>
	/// Class used to know the data of a module group
	/// </summary>
    public class ModuleCategoryListXL {
		
		#region Load Xml file
		/// <summary>
		/// Load module group list
		/// </summary>
		/// <param name="pathXMLFile">Xml file path</param>
		/// <param name="HtModuleCategory">Module categories list</param>
		public static void Load(IDataSource source, Hashtable HtModuleCategory){
			XmlTextReader Reader=null;
			HtModuleCategory.Clear();
			try{
				Reader=(XmlTextReader)source.GetSource();

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
			catch(System.Exception e){
				throw(new ModuleCategoryListXLException("Error : "+e.Message)); 
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

#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Xml;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Hermes;
using TNSHSSException=TNS.AdExpress.Hermes.ScheduleServer.Exceptions;

namespace TNS.AdExpress.Hermes.ScheduleServer.DataAccess{
	/// <summary>
	/// Load Rules DataAccess
	/// </summary>
	public class RulesDataAccess{

		/// <summary>
		/// Load Rules descriptions
		/// </summary>
		/// <param name="source">DataSource</param>
		/// <returns>List of rules</returns>
		public static Hashtable Load(IDataSource source){

			#region Variables
			XmlTextReader Reader;
			Hashtable rules=new Hashtable();
			Int64 id=0;
			string tableName="";
			Int64 listId=0;
			string hourBegin="";
			string hourEnd="";
			Int64 diffusionId=0;
			Int64 pluginId=0;
			#endregion

			try{
				Reader=(XmlTextReader)source.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "rule":
								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("id").Length>0
									&& Reader.GetAttribute("tableName")!=null && Reader.GetAttribute("tableName").Length>0
									&& Reader.GetAttribute("listId")!=null && Reader.GetAttribute("listId").Length>0
									&& Reader.GetAttribute("pluginId")!=null && Reader.GetAttribute("pluginId").Length>0){
									id = Int64.Parse(Reader.GetAttribute("id"));
									tableName = Reader.GetAttribute("tableName");
									listId = Int64.Parse(Reader.GetAttribute("listId"));
									pluginId = Int64.Parse(Reader.GetAttribute("pluginId"));
								}
								else{
									throw(new XmlException("One attribute is invalid"));
								}
								
								if(Reader.GetAttribute("hourBegin")!=null && Reader.GetAttribute("hourEnd")!=null){
									hourBegin = Reader.GetAttribute("hourBegin");
									hourEnd = Reader.GetAttribute("hourEnd");
								}
								
								if(Reader.GetAttribute("diffusionId")!=null){
									diffusionId = Int64.Parse(Reader.GetAttribute("diffusionId"));
								}
								else{
									diffusionId=0;
								}
								
								rules.Add(id,new Rule(id,tableName,listId,hourBegin,hourEnd,diffusionId,pluginId));
								break;
						}
					}
				}
				Reader.Close();
				return(rules);
			}
			catch(System.Exception err){
				throw(new TNSHSSException.RulesDataAccessException("Impossible to load rules xml parameters",err));
			}		
		}
	}
}


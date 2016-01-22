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
	/// Load Schedule DataAccess
	/// </summary>
	public class ScheduleDataAccess{
		
		/// <summary>
		/// Load Schedule descriptions
		/// </summary>
		/// <param name="source">DataSource</param>
		/// <param name="rulesList">Rules List</param>
		/// <returns>Schedule Object</returns>
		public static Schedule Load(IDataSource source, Hashtable rulesList){

			#region Variables
			XmlTextReader Reader;
			Schedule schedule=new Schedule();
			string day="";
			string hour="";
			DateTime hourDateTime=new DateTime();
			Int64 ruleId=0;
			string days="";
			#endregion

			try{
				Reader=(XmlTextReader)source.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "day":
								if(Reader.GetAttribute("id")!=null && Reader.GetAttribute("id").Length>0){
									day = Reader.GetAttribute("id");
									schedule[(DayOfWeek)int.Parse(day)] = new ArrayList();
								}
								else throw(new XmlException("One attribute is invalid"));
								break;
							case "hour":
								if(Reader.GetAttribute("time")!=null && Reader.GetAttribute("time").Length>0){
									hour = Reader.GetAttribute("time");
									hourDateTime = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day,int.Parse(hour.Substring(0,2)),int.Parse(hour.Substring(2,2)),0,0);
								}
								else throw(new XmlException("One attribute is invalid"));
								break;
							case "rule":
								if(Reader.GetAttribute("ruleId")!=null && Reader.GetAttribute("ruleId").Length>0 && Reader.GetAttribute("days")!=null && Reader.GetAttribute("days").Length>0){
									ruleId = Int64.Parse(Reader.GetAttribute("ruleId"));
									days = Reader.GetAttribute("days");
									
									schedule[(DayOfWeek)int.Parse(day)].Add(new ScheduledRule((TNS.AdExpress.Hermes.Rule)rulesList[ruleId],hourDateTime,new ArrayList(days.Split(','))));
								}
								else throw(new XmlException("One attribute is invalid"));
								break;
						}
					}
				}
				Reader.Close();
				return (schedule);
			}
			catch(System.Exception err){
				throw(new TNSHSSException.ScheduleDataAccessException("Impossible to load schedule xml parameters",err));
			}
		}

	}
}

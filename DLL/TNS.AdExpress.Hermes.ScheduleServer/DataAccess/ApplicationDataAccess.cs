#region Informations
// Auteur : B.Masson
// Date de création : 12/02/2007
// Date de modification :
#endregion

using System;
using System.Collections;
using System.Xml;
using HermesConfiguration=TNS.AdExpress.Hermes.ScheduleServer.Configuration;
using HermesException=TNS.AdExpress.Hermes.ScheduleServer.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Hermes.ScheduleServer.DataAccess{
	/// <summary>
	/// Classe du chargement de la configuration de l'application Hermes Schedule Server
	/// </summary>
	public class ApplicationDataAccess{
		
		/// <summary>
		/// Load configuration
		/// </summary>
		/// <param name="source">DataSource</param>
		/// <returns>Configuration</returns>
		public static HermesConfiguration.Application Load(IDataSource source){
			
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
							case "frequency":
								if(Reader.GetAttribute("value")!=null && Reader.GetAttribute("value").Length>0){
									valueXml = Reader.GetAttribute("value");
								}
								else{
									throw(new XmlException("One attribute is invalid"));
								}
								break;
						}
					}
				}
				Reader.Close();
				return(new HermesConfiguration.Application(int.Parse(valueXml)));
			}
			catch(System.Exception err){
				throw (new HermesException.ApplicationDataAccessException("Impossible to load application xml configuration",err));
			}
		}
	}
}

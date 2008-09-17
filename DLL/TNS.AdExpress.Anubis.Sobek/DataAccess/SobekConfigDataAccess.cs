#region Information
/*
Author : D. Mussuma, Y. Rkaina
Creation : 23/05/2006
Last Modifications : 
*/
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Sobek;
using TNS.AdExpress.Anubis.Sobek.Common;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Sobek.DataAccess
{
	/// <summary>
	/// Description résumée de SobekConfigDataAccess.
	/// </summary>
	public class SobekConfigDataAccess
	{
		/// <summary>
		/// Load Sobek configuration
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		internal static void Load(IDataSource dataSrc, SobekConfig cfg){
			XmlTextReader Reader;
			string Value="60";
			try{
				Reader=(XmlTextReader)dataSrc.GetSource();
				// Parse XML file
				while(Reader.Read()){
					
					if(Reader.NodeType==XmlNodeType.Element){
						
						switch(Reader.LocalName){
							case "CustomerMail":
								Value=Reader.GetAttribute("server");
								if (Value!=null) cfg.CustomerMailServer = Value;
								Value=Reader.GetAttribute("port");
								if (Value!=null) cfg.CustomerMailPort = int.Parse(Value);
								Value=Reader.GetAttribute("from");
								if (Value!=null) cfg.CustomerMailFrom =Value;
								Value=Reader.GetAttribute("subject");
								if (Value!=null) cfg.CustomerMailSubject = Value;
								Value=Reader.GetAttribute("webServer");
								if (Value!=null) cfg.WebServer = Value;
								break;
							case "TextFilePath":
								Value=Reader.GetAttribute("path");
								if (Value!=null) cfg.TextFilePath = Value;
								break;																				
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new Exceptions.SobekConfigDataAccessException("Impossible de charger Sobek configuration",err));
			}
		}
	}
}

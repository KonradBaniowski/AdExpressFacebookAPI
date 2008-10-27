#region Informations
// Auteur: Y. R'kaina
// Date de création: 05/02/2007
// Date de modification:
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Amset.Common;
using TNS.AdExpress.Anubis.Amset;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Amset.DataAccess{
	/// <summary>
	/// btient les données pour la génération des configurations de Amset
	/// </summary>
	public class AmsetConfigDataAccess{

		/// <summary>
		/// Load Amset configuration
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		internal static void Load(IDataSource dataSrc, AmsetConfig cfg){
			XmlTextReader Reader;
			string Value="";
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
							case "ExcelPath":
								Value=Reader.GetAttribute("path");
								if (Value!=null) cfg.ExcelPath = Value;
								break;
                            case "ThemePath":
                                Value = Reader.GetAttribute("path");
                                if (Value != null && Value.Length > 0) cfg.ThemePath = Value;
                                else cfg.ThemePath = AppDomain.CurrentDomain.BaseDirectory;
                                break;										
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new Exceptions.AmsetDataAccessException("Impossible de charger Amset configuration",err));
			}
		}
	}
}

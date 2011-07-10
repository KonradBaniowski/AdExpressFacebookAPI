#region Informations
///////////////////////////////////////////////////////////
//  BastetConfigDataAccess.cs
//  Implementation of the Class BastetDataAccess
//  Created on:      29-mar.-2006 16:51:11
//  Original author: D.V. Mussuma
///////////////////////////////////////////////////////////
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Tefnout.Common;
using TNS.AdExpress.Anubis.Tefnout;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Tefnout.DataAccess {
	/// <summary>
	/// Obtient les données pour la génération des configurations de Tefnout
	/// </summary>
	public class TefnoutConfigDataAccess {
							
		/// <summary>
		/// Load Tefnout configuration
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		internal static void Load(IDataSource dataSrc, TefnoutConfig cfg){
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
				throw (new Exceptions.TefnoutDataAccessException("Impossible de charger Tefnout configuration",err));
			}
		}

			

	}

}
#region Information
// Auteur G. Facon
// Date de création: 09/08/2005
// Date de modification:
/*		18/08/2005 : G. RAGNEAU - Pdf layout properties
 * */
#endregion

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Appm.Common;
using TNS.AdExpress.Anubis.Appm.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Appm.DataAccess{
	/// <summary>
	/// AppmConfigDataAccess provide access to the configuration data
	/// </summary>
	public class AppmConfigDataAccess{
			
		/// <summary>
		/// Load Anubis configuration
		/// </summary>
        /// <param name="dataSrc">Data Source</param>
        /// <param name="cfg">Appm Configuration</param>
		internal static void Load(IDataSource dataSrc, AppmConfig cfg){
			XmlTextReader Reader;
			string Value="60";
			try{
				Reader=(XmlTextReader)dataSrc.GetSource();
				// Parse XML file
				while(Reader.Read()){
					string name="Arial";
					string size="10";
					FontStyle fontStyle = 0;
					if(Reader.NodeType==XmlNodeType.Element){
						fontStyle = 0;
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
							case "PdfPath":
								Value=Reader.GetAttribute("path");
								if (Value!=null) cfg.PdfPath = Value;
								break;
                            case "ThemePath":
                                Value = Reader.GetAttribute("path");
                                if (Value != null && Value.Length > 0) cfg.ThemePath = Value;
                                else cfg.ThemePath = AppDomain.CurrentDomain.BaseDirectory;
                                break;
                            case "ScanPath":
                                Value = Reader.GetAttribute("path");
                                if (Value != null) cfg.ScanPath = Value;
                                break;
							case "PdfCreatorPilot":
								Value=Reader.GetAttribute("login");
								if (Value!=null) cfg.PdfCreatorPilotLogin = Value;
								Value=Reader.GetAttribute("pass");
								if (Value!=null) cfg.PdfCreatorPilotPass = Value;
								break;
							case "Html2Pdf":
								Value=Reader.GetAttribute("login");
								if (Value!=null) cfg.Html2PdfLogin = Value;
								Value=Reader.GetAttribute("pass");
								if (Value!=null) cfg.Html2PdfPass = Value;
								break;
							case "PdfProperties":
								Value=Reader.GetAttribute("author");
								if (Value!=null) cfg.PdfAuthor = Value;
								Value=Reader.GetAttribute("subject");
								if (Value!=null) cfg.PdfSubject = Value;
								Value=Reader.GetAttribute("title");
								if (Value!=null) cfg.PdfTitle = Value;
								Value=Reader.GetAttribute("producer");
								if (Value!=null) cfg.PdfProducer = Value;
								Value=Reader.GetAttribute("keywords");
								if (Value!=null) cfg.PdfKeyWords = Value;
								break;							
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new AppmConfigDataAccessException("Unable to load Appm configuration",err));
			}
		}

	}
}


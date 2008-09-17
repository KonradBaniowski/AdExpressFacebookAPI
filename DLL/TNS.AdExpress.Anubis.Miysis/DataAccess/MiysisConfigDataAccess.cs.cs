#region Information
// Auteur Y. Rkaina
// Date de création: 10/08/2006
#endregion

using System;
using System.Collections;
using System.Data;
using System.Drawing;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Miysis.Common;
using TNS.AdExpress.Anubis.Miysis.Exceptions;
using TNS.AdExpress.Common;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Miysis.DataAccess{
	/// <summary>
	/// MiysisConfigDataAccess provide access to the configuration data
	/// </summary>
	public class MiysisConfigDataAccess{
		/// <summary>
		/// Load Anubis configuration
		/// </summary>
		/// <param name="dataSource">Data Source</param>
		internal static void Load(IDataSource dataSrc, MiysisConfig cfg){
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
							case "Margins":
								Value=Reader.GetAttribute("top");
								if (Value!=null) cfg.TopMargin = double.Parse(Value);
								Value=Reader.GetAttribute("bottom");
								if (Value!=null) cfg.BottomMargin = double.Parse(Value);
								Value=Reader.GetAttribute("left");
								if (Value!=null) cfg.LeftMargin = double.Parse(Value);
								Value=Reader.GetAttribute("right");
								if (Value!=null) cfg.RightMargin = double.Parse(Value);
								break;
							case "Header":
								Value=Reader.GetAttribute("size");
								if (Value!=null) cfg.HeaderHeight = double.Parse(Value);
								break;
							case "Footer":
								Value=Reader.GetAttribute("size");
								if (Value!=null) cfg.FooterHeight = double.Parse(Value);
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
							case "DefaultFont":
								name=Reader.GetAttribute("name");
								if (name==null) name = "Arial";
								size=Reader.GetAttribute("size");
								if (size==null) size = "10";
								Value=Reader.GetAttribute("bold");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Bold;
								Value=Reader.GetAttribute("italic");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Italic;
								Value=Reader.GetAttribute("underline");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Underline;
								cfg.DefaultFont = new Font(name, float.Parse(size),fontStyle);
								Value=Reader.GetAttribute("color");
								if (Value!=null)cfg.SetDefaultFontColor(Value);
								break;
							case "HeaderFont":
								name=Reader.GetAttribute("name");
								if (name==null) name = "Arial";
								size=Reader.GetAttribute("size");
								if (size==null) size = "10";
								Value=Reader.GetAttribute("bold");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Bold;
								Value=Reader.GetAttribute("italic");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Italic;
								Value=Reader.GetAttribute("underline");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Underline;
								cfg.HeaderFont = new Font(name, float.Parse(size),fontStyle);
								Value=Reader.GetAttribute("color");
								if (Value!=null)cfg.SetHeadersFontColor(Value);
								break;
							case "TitleFont":
								name=Reader.GetAttribute("name");
								if (name==null) name = "Arial";
								size=Reader.GetAttribute("size");
								if (size==null) size = "10";
								Value=Reader.GetAttribute("bold");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Bold;
								Value=Reader.GetAttribute("italic");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Italic;
								Value=Reader.GetAttribute("underline");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Underline;
								cfg.TitleFont = new Font(name, float.Parse(size),fontStyle);
								Value=Reader.GetAttribute("color");
								if (Value!=null)cfg.SetTitleFontColor(Value);
								break;
							case "MainPageTitleFont":
								name=Reader.GetAttribute("name");
								if (name==null) name = "Arial";
								size=Reader.GetAttribute("size");
								if (size==null) size = "10";
								Value=Reader.GetAttribute("bold");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Bold;
								Value=Reader.GetAttribute("italic");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Italic;
								Value=Reader.GetAttribute("underline");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Underline;
								cfg.MainPageTitleFont = new Font(name, float.Parse(size),fontStyle);
								Value=Reader.GetAttribute("color");
								if (Value!=null)cfg.SetMainPageTitleFontColor(Value);
								break;
							case "MainPageDefaultFont":
								name=Reader.GetAttribute("name");
								if (name==null) name = "Arial";
								size=Reader.GetAttribute("size");
								if (size==null) size = "10";
								Value=Reader.GetAttribute("bold");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Bold;
								Value=Reader.GetAttribute("italic");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Italic;
								Value=Reader.GetAttribute("underline");
								if (Value!=null) 
									if (Convert.ToBoolean(Value))fontStyle = fontStyle|FontStyle.Underline;
								cfg.MainPageDefaultFont = new Font(name, float.Parse(size),fontStyle);
								Value=Reader.GetAttribute("color");
								if (Value!=null)cfg.SetMainPageFontColor(Value);
								break;
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new MiysisConfigDataAccessException("Unable to load Miysis configuration",err));
			}
		}
	}
}

#region Information
// Auteur Y. Rkaina
// Date de création: 10/08/2006
#endregion

using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Dedoum.Common;
using TNS.AdExpress.Anubis.Dedoum.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.Dedoum.DataAccess
{
	/// <summary>
	/// MiysisConfigDataAccess provide access to the configuration data
	/// </summary>
    public class DedoumConfigDataAccess
    {
		/// <summary>
		/// Load Anubis configuration
		/// </summary>
        /// <param name="dataSrc">Data Source</param>
        /// <param name="cfg">configuration</param>
		internal static void Load(IDataSource dataSrc, DedoumConfig cfg){
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
                            case "CreativesPath":
                                Value = Reader.GetAttribute("path");
                                if (Value != null) cfg.CreativesPath = Value;
                                break;
                            case "creativesSourcePath":
                                Value = Reader.GetAttribute("path");
                                if (Value != null) cfg.CreativesSourcePath = Value;
                                break;
                            case "columnsCongif":
                                 if(Reader.GetAttribute("idVehicle")!=null && Reader.GetAttribute("columnsIds")!=null)
                                cfg.ColumnsCongifs.Add(Convert.ToInt64(Reader.GetAttribute("idVehicle")), Reader.GetAttribute("columnsIds"));
                                break;
                        }
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new DedoumConfigDataAccessException("Unable to load Dedoum configuration",err));
			}
		}
	}
}

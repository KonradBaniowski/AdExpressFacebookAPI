#region Information
// Auteur G. Ragneau
// Date de création: 05/08/2005
// Date de modification:

#endregion


using System;
using System.Collections;
using System.Data;
using System.IO;
using System.Xml;

using TNS.AdExpress.Anubis.Common.Configuration;
using TNS.AdExpress.Anubis.Exceptions;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Anubis.DataAccess.Configuration{
	/// <summary>
	/// Description résumée de PluginsDataAccess.
	/// </summary>
	public class AnubisConfigDataAccess{
			
		/// <summary>
		/// Load Anubis configuration
		/// </summary>
		/// <param name="dataSrc">Data Source</param>
		/// <param name="cfg">Configuration à charger</param>
		internal static void Load(IDataSource dataSrc, AnubisConfig cfg){
			XmlTextReader Reader;
			string Value="60";
			try{
				Reader=(XmlTextReader)dataSrc.GetSource();
				// Parse XML file
				while(Reader.Read()){
					if(Reader.NodeType==XmlNodeType.Element){
						switch(Reader.LocalName){
							case "JobsUpDateInterval":
								Value=Reader.GetAttribute("time");
								if (Value!=null) cfg.JobInterval = int.Parse(Value);
								break;
							case "JobsUpDateErrorDelay":
								Value=Reader.GetAttribute("time");
								if (Value!=null) cfg.UpdateErrorDelay = int.Parse(Value);
								break;
							case "Distribution":
								Value=Reader.GetAttribute("jobsNumber");
								if (Value!=null) cfg.JobsNumber = int.Parse(Value);
								Value=Reader.GetAttribute("distributionTime");
								if (Value!=null) cfg.DistributionInterval = int.Parse(Value);
								break;

							case "Trace":
								Value=Reader.GetAttribute("logFilePath");
								if (Value!=null) cfg.TracePath = Value;
								break;
						}
					}
				}
				Reader.Close();
			}
			catch(System.Exception err){
				throw (new AnubisConfigDataAccessException("Unable to load anubis configuration",err));
			}
		}
	}
}

#region Informations
// Auteur: D. Mussuma
// Création: 25/05/2009
// Modification:
#endregion
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using TNS.AdExpress.Domain;
using TNS.AdExpress.Domain.Exceptions;
using WebCst = TNS.AdExpress.Constantes.Web;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain.Layers;
using TNS.FrameWork.DB.Common;

namespace TNS.AdExpress.Domain.XmlLoader {
	public class InfoNewsXL {

		 /// Load Info news items
        /// </summary>
        /// <param name="dataSource">Data Source</param>
		public static void LoadInfoNews(IDataSource dataSource, ref RulesLayer rulesLayer, Dictionary<WebCst.ModuleInfosNews.Directories, Results.InfoNewsItem> infoNewsItems, List<Results.InfoNewsItem> infoNewsSortedItems, ref string _newsVirtualDirectory, ref string _newsFileName, ref bool _enableNews) {
			
			 XmlTextReader reader=null;
			 WebCst.ModuleInfosNews.Directories id;
			 long webTextId;
			 string virtualPath = "", physicalPath = "";
			 int nbMaxItemsToShow = -1;
			 try {
				 reader = (XmlTextReader)dataSource.GetSource();
				
				 while (reader.Read()) {
					 if (reader.NodeType == XmlNodeType.Element) {
						 switch (reader.LocalName) {
							 case "directory":								
								 if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
								 id = (WebCst.ModuleInfosNews.Directories)Enum.Parse(typeof(WebCst.ModuleInfosNews.Directories), reader.GetAttribute("id"), true);
								 if (reader.GetAttribute("webTextId") == null || reader.GetAttribute("webTextId").Length == 0) throw (new InvalidXmlValueException("Invalid webTextId parameter"));
								 webTextId = Int64.Parse(reader.GetAttribute("webTextId"));
								 if (reader.GetAttribute("virtualPath") == null || reader.GetAttribute("virtualPath").Length == 0) throw (new InvalidXmlValueException("Invalid virtualPath parameter"));
								 virtualPath = reader.GetAttribute("virtualPath");
								 if (reader.GetAttribute("physicalPath") == null || reader.GetAttribute("physicalPath").Length == 0) throw (new InvalidXmlValueException("Invalid physicalPath parameter"));
								 physicalPath = reader.GetAttribute("physicalPath");
								 if (reader.GetAttribute("nbMaxItemsToShow") == null || reader.GetAttribute("nbMaxItemsToShow").Length == 0) nbMaxItemsToShow = -1;
								 else nbMaxItemsToShow = int.Parse(reader.GetAttribute("nbMaxItemsToShow"));
								 infoNewsItems.Add(id,new Results.InfoNewsItem(id,webTextId,virtualPath,physicalPath,nbMaxItemsToShow));
								 infoNewsSortedItems.Add(new Results.InfoNewsItem(id, webTextId, virtualPath, physicalPath, nbMaxItemsToShow));
								 break;
							 case "RulesLayer":
								 if (reader.GetAttribute("name") != null && reader.GetAttribute("assemblyName") != null && reader.GetAttribute("class") != null &&
									reader.GetAttribute("name").Length > 0 && reader.GetAttribute("assemblyName").Length > 0 && reader.GetAttribute("class").Length > 0) {
									 rulesLayer = new RulesLayer(reader.GetAttribute("name"), reader.GetAttribute("assemblyName"), reader.GetAttribute("class"));
                                }
                                break;
                            case "news":
						         if (reader.GetAttribute("physicalPath") != null)
						         {
						             _newsVirtualDirectory = reader.GetAttribute("physicalPath");
						         }
						         if (reader.GetAttribute("file") != null)
						         {
                                    _newsFileName = reader.GetAttribute("file");
						         }
                                if (reader.GetAttribute("enableNews") != null)
                                {
                                    _enableNews = bool.Parse(reader.GetAttribute("enableNews"));
                                }
                                break;
						 }
					 }
				 }
				 #region Close the file
				 if (reader != null) reader.Close();
				 #endregion
			 }

			 #region Errors management
			 catch (System.Exception e) {
				 #region Close the file
				 if (reader != null) reader.Close();
				 #endregion
				 throw (new InfoNewsXLException("Impossible to load InfoNwes XML file", e));
			 }
			 #endregion
			 //return (infoNews);
		}
	}
}

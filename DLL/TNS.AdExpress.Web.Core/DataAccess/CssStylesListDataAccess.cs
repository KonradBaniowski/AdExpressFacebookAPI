#region Informations
// Author: D. Mussuma
// Creation date: 21/02/2007
// Modification date:
#endregion

using System;
using System.Data;
using System.Xml;
using System.Collections;
using System.IO;
using TNS.AdExpress.Domain.Translation;
using TNS.AdExpress.Constantes;
using TNS.AdExpress.Web.Core.Exceptions;
using TNS.AdExpress.Domain.Web.Navigation;
using TNS.AdExpress.Web.Core;
using TNS.FrameWork.DB.Common;
using CoreExceptions=TNS.AdExpress.Web.Core.Exceptions;


namespace TNS.AdExpress.Web.Core.DataAccess{

	/// <summary>
	/// This class is used to load AdExpress styles list
	/// </summary>
	public class CssStylesListDataAccess{

		/// <summary>
		/// Load AdExpress styles list
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <returns>Hashtable contains AdExpress styles</returns>
		/// <example> This example shows the contents of the XML file
		///		<cssStyles>
		///			<cssStyle key="AdExpress" path="Css\AdExpress.css" />
		///			<cssStyle key="MediaSchedule" path="Css\MediaSchedule.css" />
		///			<cssStyle key="GenericUI" path="Css\GenericUI.css" />	
		///		</cssStyles>
		/// </example>
		/// <exception cref="System.Exception">Thrown when is impossible to load the CssConfiguration XML file</exception>
		public static Hashtable Load(IDataSource source){
			
			#region variables
			Hashtable list= new Hashtable();
			XmlTextReader reader=null;
			string cssKey = null, cssPath = null, cssContent = null;			
			#endregion

			try{
				reader=(XmlTextReader)source.GetSource();
				while(reader.Read()){
					if(reader.NodeType==XmlNodeType.Element){
						switch(reader.LocalName){
							//Element Css						
							case "cssStyle":
								if ((reader.GetAttribute("key")!=null && reader.GetAttribute("key").Length>0) && 
									(reader.GetAttribute("path")!=null && reader.GetAttribute("path").Length>0)){
									cssKey = reader.GetAttribute("key");
									cssPath = reader.GetAttribute("path");
									cssContent = LoadCssStyles(cssPath);
									if(cssContent!=null && cssContent.Length>0)
										list.Add(cssKey,cssContent); 
								}
							break;
						}
					}
				}
			}
			catch(System.Exception err){
				source.Close();
				throw(new CoreExceptions.CssStylesListDataAccessException("Impossible to load the CssConfiguration XML file",err));
			}

			return list;
		}

		#region CSS Style
		/// <summary>
		/// This method returns the css style of the page
		/// </summary>
		/// <param name="cssPath">Css file path</param>
		/// <returns>Css style of the page</returns>
		private static string LoadCssStyles(string cssPath){
			string styleCss = "";
			StreamReader monStreamReader = new StreamReader(AppDomain.CurrentDomain.BaseDirectory+cssPath); 
			styleCss = "<style>"+monStreamReader.ReadToEnd()+"</style>";	
			monStreamReader.Close();				
			return styleCss;
		}
		#endregion
	}
}

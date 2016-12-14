#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Collections;
using System.Text;
using TNS.FrameWork.DB.Common;
using CoreExceptions = TNS.Classification.Exceptions;

namespace TNS.Classification.DataAccess {
	class UniverseLevelsCustomStylesDataAccess {
		/// <summary>
		/// Load all universe's levels styles
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <returns>universe's levels styles</returns>
		public static Dictionary<long, System.Web.UI.WebControls.Style> Load(IDataSource source) {
			Dictionary<long, System.Web.UI.WebControls.Style> list = new Dictionary<long, System.Web.UI.WebControls.Style>();
			XmlTextReader reader = null;
			long id = 0;			
			System.Web.UI.WebControls.Style style= null;
			try {
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "level":
								id = 0;							
								if ((reader.GetAttribute("id") != null && reader.GetAttribute("id").Length > 0)) {
									id = long.Parse(reader.GetAttribute("id"));
									style = new System.Web.UI.WebControls.Style();
									list.Add(id, style);
								}
								else {
									throw (new XmlException("Invalid Attribute for level"));
								}
								break;
							case "style":
								if ((reader.GetAttribute("width") != null && reader.GetAttribute("width").Length > 0)) {
									style.Width = new System.Web.UI.WebControls.Unit( int.Parse(reader.GetAttribute("width")));
								}								
								else {
									throw (new XmlException("Invalid Attribute for style"));
								}
								break;

						}
					}
				}
				source.Close();
				return (list);
			}
			catch (System.Exception err) {
				source.Close();
				throw (new CoreExceptions.UniverseLevelsDataAccessException("Impossible to load the UniverseLevelsCustomStyles XML file", err));
			}
		}
	}
}

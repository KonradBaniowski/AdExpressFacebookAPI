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
using TNS.Classification.Universe;

namespace TNS.Classification.DataAccess {

	public class UniverseLevelsDataAccess {

		/// <summary>
		/// Load all universe's levels
		/// </summary>
		/// <param name="source">Data Source</param>
		/// <returns>universe's levels</returns>
		public static Dictionary<long, UniverseLevel> Load(IDataSource source) {
			Dictionary<long, UniverseLevel> list = new Dictionary<long, UniverseLevel>();			
			XmlTextReader reader = null;
			long id = 0;
			string dbTable = null;
			int labelId = 0;
			string dbId = null;
			try {
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "level":
								 id = 0;
								 dbTable = null;
								 dbId = null;
								 labelId = 0;
								if ((reader.GetAttribute("id") != null && reader.GetAttribute("id").Length > 0) &&
									(reader.GetAttribute("dbTable") != null && reader.GetAttribute("dbTable").Length > 0) &&
									(reader.GetAttribute("labelId") != null && reader.GetAttribute("labelId").Length > 0) &&
									(reader.GetAttribute("dbId") != null && reader.GetAttribute("dbId").Length > 0)) 
								{
									id = long.Parse(reader.GetAttribute("id"));
									dbTable = reader.GetAttribute("dbTable");
									labelId = int.Parse(reader.GetAttribute("labelId"));
									dbId = reader.GetAttribute("dbId");
									list.Add(id, new UniverseLevel(id, dbTable, labelId, dbId));
								}
								else {
									throw (new XmlException("Invalid Attribute for level"));
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
				throw (new CoreExceptions.UniverseLevelsDataAccessException("Impossible to load the UniverseLevels XML file", err));
			}
		}
	}
}

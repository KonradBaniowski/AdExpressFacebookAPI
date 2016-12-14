#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using CoreExceptions = TNS.Classification.Exceptions;
using TNS.Classification.Universe;

namespace TNS.Classification.DataAccess {
	public class UniverseBranchesDataAccess {
		/// <summary>
		/// Load universe levels hierarchy
		/// </summary>
		/// <param name="source"></param>
		/// <returns></returns>
		public static Dictionary<int, UniverseBranch> Load(IDataSource source) {
			//Dictionary<int, List<UniverseLevel>> list = new Dictionary<int, List<UniverseLevel>>();
			Dictionary<int, UniverseBranch> list = new Dictionary<int, UniverseBranch>();
			List<UniverseLevel> levels = new List<UniverseLevel>();
			UniverseBranch universeBranch = null;
			XmlTextReader reader = null;
			int id = 0;
			int labelId = 0;
			try {
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "branch":
								if (id != 0) {
									//list.Add(id, levels);
									list.Add(id, universeBranch);
								}
								id = 0;
								if ((reader.GetAttribute("id") != null && reader.GetAttribute("id").Length > 0)
									&& (reader.GetAttribute("labelId") != null && reader.GetAttribute("labelId").Length > 0)) {
									id = int.Parse(reader.GetAttribute("id"));
									labelId = int.Parse(reader.GetAttribute("labelId"));
									levels = new List<UniverseLevel>();
									universeBranch = new UniverseBranch(id, levels, labelId);
								}
								else {
									throw (new XmlException("Invalide Attribute for branch"));
								}
								break;
							case "level":
								if ((reader.GetAttribute("id") != null && reader.GetAttribute("id").Length > 0)) {
									levels.Add(UniverseLevels.Get(int.Parse(reader.GetAttribute("id"))));
								}
								else {
									throw (new XmlException("Invalide Attribute for level"));
								}
								break;
						}
					}
				}
				if (id != 0 && universeBranch != null) list.Add(id, universeBranch);// list.Add(id, levels);
				source.Close();
				return (list);
			}
			catch (System.Exception err) {
				source.Close();
				throw (new CoreExceptions.UniverseBranchesDataAccessException("Impossible to load the GenericDetailLevel XML file", err));
			}
		}

	}
}

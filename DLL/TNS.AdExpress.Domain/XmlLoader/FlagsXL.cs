#region Informations
// Author: D. Mussuma
// Creation date: 23/03/2008 
// Modification date:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;

using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
namespace TNS.AdExpress.Domain.XmlLoader {
	/// <summary>
	/// Load flags description
	/// </summary>
	public class FlagsXL {

		/// <summary>
		/// Load flag description list
		/// </summary>
		/// <param name="source">source</param>
		/// <returns>flag list</returns>
		public static Dictionary<Int64, Flag> Load(IDataSource source) {

			#region Variables
			Dictionary<Int64, Flag> list = null;
			XmlTextReader reader = null;
			string description = "";
			Int64 id;
			Flag flag = null;
			#endregion
			try {
				source.Open();
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "flag":
								if (reader.GetAttribute("description") == null || reader.GetAttribute("description").Length == 0) throw (new InvalidXmlValueException("Invalid description parameter"));
								description = reader.GetAttribute("description");
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
								id = Int64.Parse(reader.GetAttribute("id"));
								if (list == null)list = new Dictionary<Int64, Flag>();
								flag = new Flag(id, description);								
								list.Add(id, flag);
								break;
						}
					}
				}
			}
			catch (System.Exception err) {
				#region Close the file
				if (source.GetSource() != null) source.Close();
				#endregion

				throw (new Exception(" Error : ", err));
			}
			source.Close();
			return (list);
			
		}
	}
}

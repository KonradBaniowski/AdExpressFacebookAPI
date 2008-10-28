#region Informations
// Author: D. Mussuma
// Creation date: 10/09/2008 
// Modification date:
#endregion
using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using TNS.AdExpress.Constantes.Web;

using TNS.FrameWork.DB.Common;
using TNS.FrameWork.Exceptions;
using TNS.AdExpress.Domain;
namespace TNS.AdExpress.Domain.XmlLoader {
	/// <summary>
	/// Decripition of differents list use in the site
	/// </summary>
	public class ListsDescriptionXL {

		/// <summary>
		/// Load inset description list
		/// </summary>
		/// <param name="source">source</param>
		/// <returns>Inset list</returns>
		public static Dictionary<CustomerSessions.InsertType, long> Load(IDataSource source) {

			#region Variables
			Dictionary<CustomerSessions.InsertType, long> list = new Dictionary<CustomerSessions.InsertType, long>();
			XmlTextReader reader = null;
			string id;
			long dataBaseId;
			#endregion

			try {
				source.Open();
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "inset":
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid id parameter"));
								id = reader.GetAttribute("id");								
								if (reader.GetAttribute("dataBaseId") == null || reader.GetAttribute("dataBaseId").Length == 0) throw (new InvalidXmlValueException("Invalid data base Id parameter"));;
								dataBaseId = long.Parse(reader.GetAttribute("dataBaseId"));
								list.Add((CustomerSessions.InsertType)Enum.Parse(typeof(CustomerSessions.InsertType), id, true), dataBaseId);								
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
		/// <summary>
		/// Load group list 
		/// </summary>
		/// <param name="source">source</param>
		/// <returns>group list</returns>
		public static Dictionary<GroupList.ID, Dictionary<GroupList.Type, string>> LoadGroupList(IDataSource source) {

			#region Variables
			Dictionary<GroupList.ID, Dictionary<GroupList.Type, string>> groupsDesciption = new Dictionary<GroupList.ID, Dictionary<GroupList.Type, string>>();

			Dictionary<GroupList.Type, string> lists = new Dictionary<GroupList.Type, string>();
			XmlTextReader reader = null;
			GroupList.Type keyType;
			GroupList.ID group;
			string values;
			string oldGroup = "";
			#endregion

			try {
				source.Open();
				reader = (XmlTextReader)source.GetSource();
				while (reader.Read()) {
					if (reader.NodeType == XmlNodeType.Element) {
						switch (reader.LocalName) {
							case "list":
								
								if (reader.GetAttribute("group") == null || reader.GetAttribute("group").Length == 0) throw (new InvalidXmlValueException("Invalid group parameter"));
								group = (GroupList.ID)Enum.Parse(typeof(GroupList.ID), reader.GetAttribute("group"), true);
								if(oldGroup !=null && oldGroup.Length>0 && !oldGroup.Equals(reader.GetAttribute("group"))){
									groupsDesciption.Add((GroupList.ID)Enum.Parse(typeof(GroupList.ID), oldGroup, true), lists);
									lists= new Dictionary<GroupList.Type,string>();
								}
								if (reader.GetAttribute("id") == null || reader.GetAttribute("id").Length == 0) throw (new InvalidXmlValueException("Invalid data keyType parameter")); ;
								keyType = (GroupList.Type)Enum.Parse(typeof(GroupList.Type), reader.GetAttribute("id"), true);
								if (reader.GetAttribute("listIds") == null || reader.GetAttribute("listIds").Length == 0) throw (new InvalidXmlValueException("Invalid listIds parameter"));
								values = reader.GetAttribute("listIds");
								lists.Add(keyType, values);
								oldGroup = reader.GetAttribute("group");
								break;
						}
					}
				}
				if (oldGroup != null && oldGroup.Length > 0) {
					groupsDesciption.Add((GroupList.ID)Enum.Parse(typeof(GroupList.ID), oldGroup, true), lists);
					lists = new Dictionary<GroupList.Type, string>();
				}

			}
			catch (System.Exception err) {

				#region Close the file
				if (source.GetSource() != null) source.Close();
				#endregion

				throw (new Exception(" Error : ", err));
			}
			source.Close();
			return (groupsDesciption);

		}
	}
}

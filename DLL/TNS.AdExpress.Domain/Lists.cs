#region Informations
// Auteur: D. Mussuma
// Création: 10/09/2008
// Modification:
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using TNS.FrameWork.DB.Common;
using TNS.AdExpress.Domain.XmlLoader;
using TNS.AdExpress.Constantes.Web;
using TNS.AdExpress.Domain.Exceptions;

namespace TNS.AdExpress.Domain {
	/// <summary>
	/// Contains differents lists to use in results computes
	/// <example>Inset list will be use for press results</example>
	/// </summary>
	public class Lists {
		
		#region variables
		/// <summary>
		/// Groups desciptions 
		/// </summary>
		private static Dictionary<GroupList.ID, Dictionary<GroupList.Type, string>> _groupsDesciption = new Dictionary<GroupList.ID, Dictionary<GroupList.Type, string>>();
		
		#endregion

		#region Accessors		
		#endregion

		#region Methods public
		/// <summary>
		/// Get Id list
		/// </summary>
		/// <param name="id">Id type of list</param>
		public static string GetIdList(GroupList.Type id) {
			string fieldsIdList = "";
			try {
				if (_groupsDesciption != null && _groupsDesciption.Count > 0) {
					foreach (KeyValuePair<GroupList.ID, Dictionary<GroupList.Type, string>> kpv in _groupsDesciption) {
						if (kpv.Value.ContainsKey(id)) 
							return kpv.Value[id];
					}
				}
				return fieldsIdList;
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to retreive the id list", err));
			}
		}
		/// <summary>
		/// Get Id list
		/// </summary>
		/// <param name="id">Id type of list</param>
		/// <param name="idGroup">Id group of list</param>
		public static string GetIdList(GroupList.ID idGroup,GroupList.Type id) {
			string fieldsIdList = "";
			try {
				if (_groupsDesciption != null && _groupsDesciption.Count > 0 && _groupsDesciption.ContainsKey(idGroup)
					&& _groupsDesciption[idGroup] != null && _groupsDesciption[idGroup].ContainsKey(id)) {
					fieldsIdList = _groupsDesciption[idGroup][id];
				}				
				return fieldsIdList;
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to retreive the id list", err));
			}
		}
		/// <summary>
		/// Get Id list
		/// <example>Get Insets Id 108,85,999</example>
		/// </summary>
		public static string GetIdList(GroupList.ID idGroup) {
			string fieldsIdList = "";
			try {
				if (_groupsDesciption != null && _groupsDesciption.Count > 0 && _groupsDesciption.ContainsKey(idGroup)) {
					foreach (KeyValuePair<GroupList.Type, string> kpv in _groupsDesciption[idGroup]) {
						fieldsIdList += kpv.Value + ",";
					}
					if (fieldsIdList != null && fieldsIdList.Length > 0) fieldsIdList = fieldsIdList.Substring(0, fieldsIdList.Length - 1);
				}
				return fieldsIdList;
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to retreive the id list", err));
			}
		}

		/// <summary>
		/// Initialisation of list from  XML file
		/// </summary>
		/// <param name="source">Dat source</param>
		public static void Init(IDataSource source) {
			_groupsDesciption.Clear();
			_groupsDesciption = ListsDescriptionXL.LoadGroupList(source);						
		}
		#endregion
	}
}

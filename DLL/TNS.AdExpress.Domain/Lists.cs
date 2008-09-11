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
		///<summary>
		/// Insets description list
		/// </summary>
		private static Dictionary<CustomerSessions.InsertType, long> _insetList = new Dictionary<CustomerSessions.InsertType, long>();
		#endregion

		#region Accessors
		/// <summary>
		/// Get insets description list
		/// </summary>
		public static Dictionary<CustomerSessions.InsertType, long> InsetList {
			get { return _insetList; }
		}
		#endregion

		#region Methods public
		/// <summary>
		/// Get Inset
		/// </summary>
		public static long GetInset(CustomerSessions.InsertType id) {
			try {
				return (_insetList[id]);
			}
			catch (System.Exception err) {
				throw (new ArgumentException("impossible to retreive the requested inset", err));
			}
		}
		/// <summary>
		/// Get Insets Id list
		/// <example>108,85,999</example>
		/// </summary>
		public static string GetInsetIdList() {
			string fieldsIdList = "";
			if (_insetList != null && _insetList.Count>0) {
				foreach (KeyValuePair<CustomerSessions.InsertType, long> kpv in _insetList) {
					fieldsIdList += kpv.Value + ",";
				}
				if (fieldsIdList != null && fieldsIdList.Length > 0) fieldsIdList = fieldsIdList.Substring(0, fieldsIdList.Length - 1);
			}
			return fieldsIdList;
		}

		/// <summary>
		/// Initialisation of list from  XML file
		/// </summary>
		/// <param name="source">Dat source</param>
		public static void Init(IDataSource source) {
			_insetList.Clear();
			Dictionary<CustomerSessions.InsertType, long> insets = ListsDescriptionXL.Load(source);
			try {
				foreach (KeyValuePair<CustomerSessions.InsertType, long> currentInset in insets) {
					_insetList.Add(currentInset.Key, currentInset.Value);
				}
			}
			catch (System.Exception err) {
				throw (new ListsException("Impossible to init the inset list", err));
			}
		}
		#endregion
	}
}

#region Informations
// Auteur: D. Mussuma
// Création: 19/11/2007
// Modification:
#endregion
using System;
using System.Collections.Generic;
using System.Text;

namespace TNS.Classification.Universe {
	/// <summary>
	/// Class corresponding to universe level item
	/// </summary>
	public class UniverseLevel {

		#region Variables
		///<summary>Level Id.</summary>		
		private long _id;

		///<summary>Corresponding table name.</summary>		
		private string _tableName;

		/// <summary>
		/// Level Label ID
		/// </summary>
		private int _labelId = 0;

		/// <summary>
		/// Level's Database field identifier for AdExpress
		/// </summary>
		private string _dataBaseIdField;
		#endregion

		#region Properties
		/// <summary>
		/// Get Level  ID
		/// </summary>		
		public long ID {
			get { return _id; }			
		}

		/// <summary>
		/// Get Level Label ID
		/// </summary>
		public int LabelId {
			get { return _labelId; }			
		}

		/// <summary>
		///<summary>Get table name.</summary>		
		/// </summary>
		public string TableName {
			get { return _tableName; }
		}

		/// <summary>
		///<summary>Get dataBase Id Field.</summary>		
		/// </summary>
		public string DataBaseIdField {
			get { return _dataBaseIdField; }
		}
		#endregion

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="id">level Identifier</param>
		/// <param name="tableName">Table name</param>
		/// <param name="labelId">Label Identifier</param>
		/// <param name="dataBaseAliasIdField">Level's database field identifier</param>
		public UniverseLevel(long id, string tableName, int labelId,string dataBaseIdField) {
			if (tableName == null || tableName.Length < 1) throw (new ArgumentException("Invalid argument tableName"));
			if (id < 0) throw (new ArgumentException("Invalid argument id"));
			if (labelId < 0) throw (new ArgumentException("Invalid argument labelId"));
			if (dataBaseIdField == null || dataBaseIdField.Length < 1) throw (new ArgumentException("Invalid argument dataBaseIdField"));
			_id = id;
			_tableName = tableName;
			_labelId = labelId;
			_dataBaseIdField = dataBaseIdField;
		}
	}
}
